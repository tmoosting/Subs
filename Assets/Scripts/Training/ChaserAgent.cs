using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ChaserAgent : Agent
{

    // work out combined system
    // copy in cruising agent, modular

    [Header("Assigns")]
    public ChaserBox chaserBox;

    [Header("Scene Settings")]
    public Ship.Engine startEngine;

    [Header("Observation")]
    [Tooltip("space: 1")] public bool useAgentAngle;
    [Tooltip("space: 3")] public bool useEnemyDirectionVector;
    [Tooltip("space: 1")] public bool useEnemyDirectionAngle;
    [Tooltip("space: 1")] public bool useEnemyDirectionRelativeToCurrentBearingAngle;
    [Tooltip("space: 1")] public bool useEnemyDistance;

    [Header("Actions")]
    public bool useSpecificBearing;
    public bool useAdjustmentBearing;

    [Header("Rewards")]
    public float baseReward;
    public bool penalizeIncreasedDistance;
    public bool rewardDecreasedDistance;
    public float distanceRewardMultiplier;
    public bool penalizeIncreasedAngle;
    public bool rewardDecreasedAngle;
    public float angleRewardMultiplier;

    Vector3 prevPos;
    float prevBearingDifference;
    int previousCounter = 0;
    float maxDistance;
    Ship ship;

    // if not in locator range, go straight up 

    private void Awake()
    {
        ship = GetComponent<Ship>();
        prevPos = transform.position;
        prevBearingDifference = 0;
    }
    private void FixedUpdate()
    {
        previousCounter++;
        if (previousCounter >= 2)
        {
            previousCounter = 0;
            prevBearingDifference = GetDifferenceInBearingAbsolute();
        }
        UIController.Instance.bearingInputField.text = ((int)GetComponent<Ship>().obtainLocationBearing(chaserBox.enemyShip.transform.position)).ToString();

    }
    public override void OnEpisodeBegin()
    {
        chaserBox.ResetScene();

        GetComponent<Ship>().SetEngineSpeed(startEngine);

        // resets observations 
        GetComponent<Captain>().ResetCaptain();

        // one of our white squares is ~0.73 distance
        maxDistance = Vector3.Distance(transform.position, chaserBox.enemyShip.transform.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    { 
        // BELOW: three different normalizing approaches. MIND: use same one for enemyAtAngleNormalized and  enemyBearingRelativeToCurrentNormalized

        // north:0/1  east:0.25  south:0.5  west:0.75  
        float agentAngleNormalized = (float)((ship.GetCurrentBearing() % 360) / 360f);

        // north:-1  east:-0.5  south:0  west:0.5  
        //   float agentAngleNormalized = (float)(((ship.GetCurrentBearing() % 360) - 180f) / 180f);

        // north: 1   east:  0.5  south: 00  west:   0.5
        //     float agentAngleNormalized = (float)Mathf.Abs((((ship.GetCurrentBearing() % 360) - 180f) / 180f));
        //     float direction = ship.GetCurrentBearing() > 0 && ship.GetCurrentBearing() < 180 ? 1 : ship.GetCurrentBearing() > 180 && ship.GetCurrentBearing() < 360 ? -1 : 0;

        // north:-1  east:-0.5  south:0  west:0.5  
        float enemyAtAngleNormalized = (float)(((float)ship.obtainLocationBearing(chaserBox.enemyShip.transform.position) % 360) / 360f);
        

        // normalized vector3
        Vector3 enemyDirection = (chaserBox.enemyShip.transform.position - transform.position).normalized;

        // at 90 when enemy is due east of agent
        int enemyBearingRelativeToCurrent = (int)Mathf.Round((float)ship.obtainLocationBearing(chaserBox.enemyShip.transform.position) - ship.GetCurrentBearing() % 360);
        // north:-1  east:-0.5  south:0  west:0.5  
        float enemyBearingRelativeToCurrentNormalized = (float)((enemyBearingRelativeToCurrent % 360) / 360f);

        //distance 
        float distanceNormalized = Vector3.Distance(chaserBox.enemyShip.transform.position, transform.position) / maxDistance;

        if (useAgentAngle == true)
            sensor.AddObservation(agentAngleNormalized);
        if (useEnemyDirectionVector == true)
            sensor.AddObservation(enemyDirection);
        if (useEnemyDirectionAngle == true)
            sensor.AddObservation(enemyAtAngleNormalized);       
        if (useEnemyDirectionRelativeToCurrentBearingAngle == true)
            sensor.AddObservation(enemyBearingRelativeToCurrentNormalized); 
        if (useEnemyDistance == true)
            sensor.AddObservation(distanceNormalized);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Ship ship = GetComponent<Ship>();

        if (useSpecificBearing == true)
        { 
            GetComponent<Ship>().SetCourse(actions.DiscreteActions[0]);
        }

        else if (useAdjustmentBearing == true)
        {
            int actionValue0 = actions.DiscreteActions[0];
            int currentBearing = GetComponent<Ship>().GetCurrentBearing();
            int newBearing = currentBearing;
            if (actionValue0 == 0)
            {
                // subtract one, counterclockwise
                if (currentBearing - 1 <= 0)
                    newBearing = 360;
                else
                    newBearing = currentBearing - 1;
            }
            else if (actionValue0 == 1)
            {
                // do nothing
                newBearing = currentBearing;
            }
            else if (actionValue0 == 2)
            {
                if (currentBearing + 1 >= 360)
                    newBearing = 0;
                else
                    newBearing = currentBearing + 1;
            }
            GetComponent<Ship>().SetCourse(newBearing);
        }

        AddStepRewards();  
    }
    void AddStepRewards()
    {
        float prevDistanceNormalized = (chaserBox.transform.position - prevPos).magnitude / maxDistance;
        float currentDistanceNormalized = (chaserBox.transform.position - transform.position).magnitude / maxDistance;

        if (penalizeIncreasedDistance == true)
        { 
            if (prevDistanceNormalized < currentDistanceNormalized)
                AddReward(baseReward * distanceRewardMultiplier);
        }
        if (rewardDecreasedDistance == true)
        {
            if (prevDistanceNormalized < currentDistanceNormalized)
                AddReward((baseReward * distanceRewardMultiplier)*-1);
        }

        if (penalizeIncreasedAngle == true)
        {
            if (prevBearingDifference  < GetDifferenceInBearingAbsolute())
                AddReward(baseReward * angleRewardMultiplier);
        }
        if (rewardDecreasedAngle == true)
        {
            if (prevBearingDifference > GetDifferenceInBearingAbsolute())
                AddReward((baseReward * angleRewardMultiplier) * -1);
        }

      
    }


    float GetDifferenceInBearingAbsolute()
    {
        int difference_in_bearing = (int)Mathf.Round((float)ship.obtainLocationBearing(chaserBox.enemyShip.transform.position) - ship.GetCurrentBearing() % 360);
        difference_in_bearing = difference_in_bearing > 180 ? 360 - difference_in_bearing : difference_in_bearing;
        float difference_in_bearing_normal = difference_in_bearing / 180f;
        float difference_in_bearing_normal_abs = Mathf.Abs(difference_in_bearing_normal);
        return difference_in_bearing_normal_abs;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions; 
        discreteActions[0] = int.Parse(UIController.Instance.bearingInputField.text);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Uboat>(out Uboat uboat))
        {
            if (collision.gameObject.GetComponent<Ship>() == chaserBox.enemyShip)
            {
                SetReward(+1f);
                TrainingController.Instance.LogTrainingSuccess();
                EndEpisode();
            }
           
        }

    }
}
