using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ChaserAgent : Agent
{
     

    [Header("Assigns")]
    public ChaserBox chaserBox;

    [Header("Scene Settings")]
    public bool stationaryAgent;
    public Ship.Engine startEngine;
    public float uboatMaxPlacementRange;
    public float uboatBoxSize;

    [Header("Observation")]
    [Tooltip("space: 3")] public bool useEnemyDirectionVector; // vector3 
    [Tooltip("space: 1")] public bool useEnemyDistance; // float
    [Tooltip("space: 1")] public bool useAgentAngle; // angle agentship is facing  
    [Tooltip("space: 2")] public bool useGeospatial; // send cos, sin
    [Tooltip("space: 3")] public bool useEnemyLocation; // send vector3
    [Tooltip("space: 3")] public bool useAgentLocation; // vector3

    //    [Tooltip("space: 1")] public bool useEnemyDirectionAngle; // enemylocationbearingangle
    //    [Tooltip("space: 1")] public bool useEnemyDirectionRelativeToCurrentBearingAngle; // difference in own angle and enemy's

    [Header("Actions")]
    public bool useSpecificBearing;
    public bool useAdjustmentBearing;
    public bool setEngineSpeed;

    [Header("Rewards")]
    public float baseReward;
    public bool baseNegativeTick;
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

    // for debug
    float enemyDistance;
    float agentAngle;
    int adjBearingAction = 0;
    float distanceReward = 0f;
    float angleReward = 0f;
    float locBearing = 0f;
    float prevDistanceNormalized = 0f;
    float currentDistanceNormalized = 0f;

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
            prevPos = transform.position;
            prevBearingDifference = GetDifferenceInBearingAbsolute();
        }
        UIController.Instance.bearingInputField.text = ((int)GetComponent<Ship>().obtainLocationBearing(chaserBox.enemyShip.transform.position)).ToString();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (transform.parent.name == "ChaserBox")
            {
                Debug.Log("enemyDistance : " + enemyDistance);
                Debug.Log("agentAngle : " + agentAngle);
                Debug.Log("adjBearingAction : " + adjBearingAction);
                Debug.Log("locBearing : " + locBearing);
                Debug.Log("prevDistanceNormalized : " + prevDistanceNormalized);
                Debug.Log("currentDistanceNormalized : " + currentDistanceNormalized);
                Debug.Log("distanceReward : " + distanceReward);
                Debug.Log("prevBearingDifference : " + prevBearingDifference);
                Debug.Log("GetDifferenceInBearingAbsolute : " + GetDifferenceInBearingAbsolute());
                Debug.Log("angleReward : " + angleReward);
            }
        

        }
    }
 

    public override void OnEpisodeBegin()
    {
        chaserBox.ResetScene();

         enemyDistance = 0f;
         agentAngle = 0f;
         adjBearingAction = 0;
         distanceReward = 0f;
         angleReward = 0f;
         locBearing = 0f;
         prevDistanceNormalized = 0f;
         currentDistanceNormalized = 0f;

        if (stationaryAgent == true)
            GetComponent<Ship>().SetEngineSpeed(Ship.Engine.Still);
        else
            GetComponent<Ship>().SetEngineSpeed(startEngine);

        // resets observations 
        GetComponent<Captain>().ResetCaptain();

        // one of our white squares is ~0.73 distance
        //  maxDistance = Vector3.Distance(transform.position, chaserBox.enemyShip.transform.position);
        maxDistance = 10;
    }
    double AngleToTargetRadian(Vector3 location)
    {
        float deltaX = location.x - transform.position.x;
        float deltaY = location.y - transform .position.y;
        return Mathf.Atan2(deltaY, deltaX); 
    }
   
    public override void CollectObservations(VectorSensor sensor)
    {

    //    Debug.Log("locbear " + ship.obtainLocationBearing(chaserBox.enemyShip.transform.position));
        // BELOW: three different normalizing approaches. MIND: use same one for enemyAtAngleNormalized and  enemyBearingRelativeToCurrentNormalized

        // north:0/1  east:0.25  south:0.5  west:0.75  
        float agentAngleNormalized = (float)((ship.GetCurrentBearing() % 360) / 360f);
        agentAngle = agentAngleNormalized; // for debug
        //float agentAngleNormalizedMin = (float)(((ship.GetCurrentBearing()-5) % 360) / 360f);
        //float agentAngleNormalizedPlus = (float)(((ship.GetCurrentBearing()+5) % 360) / 360f);

        //   Debug.Log("agent angle: " + agentAngleNormalized); 


        // north:-1  east:-0.5  south:0  west:0.5  
        //   float agentAngleNormalized = (float)(((ship.GetCurrentBearing() % 360) - 180f) / 180f);

        // north: 1   east:  0.5  south: 00  west:   0.5
        //     float agentAngleNormalized = (float)Mathf.Abs((((ship.GetCurrentBearing() % 360) - 180f) / 180f));
        //     float direction = ship.GetCurrentBearing() > 0 && ship.GetCurrentBearing() < 180 ? 1 : ship.GetCurrentBearing() > 180 && ship.GetCurrentBearing() < 360 ? -1 : 0;

        // north:-1  east:-0.5  south:0  west:0.5  
        float enemyAtAngleNormalized = (float)(((float)ship.obtainLocationBearing(chaserBox.enemyShip.transform.position) % 360) / 360f);
        float enemySin = Mathf.Sin(2 * Mathf.PI * enemyAtAngleNormalized);
        float enemyCos = Mathf.Cos(2 * Mathf.PI * enemyAtAngleNormalized);
        // == angles

        // normalized vector3
        Vector3 enemyDirection = (chaserBox.enemyShip.transform.position - transform.position).normalized;

        //// at 90 when enemy is due east of agent
        //int enemyBearingRelativeToCurrent = (int)Mathf.Round((float)ship.obtainLocationBearing(chaserBox.enemyShip.transform.position) - ship.GetCurrentBearing() % 360);
        //// north:-1  east:-0.5  south:0  west:0.5  
        //float enemyBearingRelativeToCurrentNormalized = (float)((enemyBearingRelativeToCurrent % 360) / 360f);

        //distance 
        float distanceNormalized = Vector3.Distance(chaserBox.enemyShip.transform.position, transform.position) / maxDistance;
        enemyDistance = distanceNormalized; // for debug

        if (useAgentAngle == true)
            sensor.AddObservation(agentAngleNormalized);
        if (useGeospatial == true)
        {
            sensor.AddObservation(enemySin);
            sensor.AddObservation(enemyCos); 
        }
        if (useEnemyDirectionVector == true)
            sensor.AddObservation(enemyDirection);
        if (useEnemyDistance == true)
            sensor.AddObservation(distanceNormalized);
        if (useEnemyLocation == true)
        {
            sensor.AddObservation(chaserBox.enemyShip.transform.position);
        }
        if (useAgentLocation == true)
        {
            sensor.AddObservation(transform.position); 
        }
        //  if (useEnemyDirectionAngle == true)
        //      sensor.AddObservation(enemyAtAngleNormalized);       
        //if (useEnemyDirectionRelativeToCurrentBearingAngle == true)
        //    sensor.AddObservation(enemyBearingRelativeToCurrentNormalized); 

    }

    public override void OnActionReceived(ActionBuffers actions)
    { 
        if (stationaryAgent == false)
        {
            if (useSpecificBearing == true)
            {
                GetComponent<Ship>().SetCourse(actions.DiscreteActions[0]);
            }

            else if (useAdjustmentBearing == true)
            {
                int actionValue0 = actions.DiscreteActions[0];
                adjBearingAction = actionValue0; // for debug
                int currentBearing = GetComponent<Ship>().GetCurrentBearing();
                int newBearing = currentBearing;
                if (actionValue0 == 0)
                {
                    // subtract one, counterclockwise
                    if (currentBearing - 1 <= 0)
                        newBearing = 359;
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
                //   Debug.Log("setcourse" + newBearing);
                ship.SetCourse(newBearing);
            }

            if (setEngineSpeed == true)
            {
                ship.SetEngineSpeed((Ship.Engine)actions.DiscreteActions[1]);
            }
        }

      
        AddStepRewards();  
    }
    void AddStepRewards()
    {
         prevDistanceNormalized = (chaserBox.enemyShip.transform.position - prevPos).magnitude / maxDistance;
         currentDistanceNormalized = (chaserBox.enemyShip.transform.position - transform.position).magnitude / maxDistance;

        if (baseNegativeTick == true)
        {
            AddReward(-baseReward );

        }
        if (penalizeIncreasedDistance == true)
        { 
            if (prevDistanceNormalized < currentDistanceNormalized)
            {
                distanceReward = baseReward * distanceRewardMultiplier * -1;// for debug
                AddReward((baseReward * distanceRewardMultiplier) * -1);
            }
        }
        if (rewardDecreasedDistance == true)
        {
            if (prevDistanceNormalized > currentDistanceNormalized)
            {
                distanceReward = baseReward * distanceRewardMultiplier ;// for debug
                AddReward((baseReward * distanceRewardMultiplier));
            }
        }

        if (penalizeIncreasedAngle == true)
        {
            if (prevBearingDifference  < GetDifferenceInBearingAbsolute())
            {
                angleReward = baseReward * angleRewardMultiplier * -1;// for debug
                AddReward((baseReward * angleRewardMultiplier) * -1);
            }
        }
        if (rewardDecreasedAngle == true)
        {
            if (prevBearingDifference > GetDifferenceInBearingAbsolute())
            {
                angleReward = baseReward * angleRewardMultiplier;// for debug
                AddReward((baseReward * angleRewardMultiplier) );
            }
        }

      
    }


    float GetDifferenceInBearingAbsolute()
    {
        int difference_in_bearing = (int)Mathf.Round((float)ship.obtainLocationBearing(chaserBox.enemyShip.transform.position) - ship.GetCurrentBearing() % 360);
        difference_in_bearing = difference_in_bearing > 180 ? 360 - difference_in_bearing : difference_in_bearing;
        float difference_in_bearing_normal = difference_in_bearing / 180f;
        float difference_in_bearing_normal_abs = Mathf.Abs(difference_in_bearing_normal);
        locBearing = difference_in_bearing_normal_abs;
        return difference_in_bearing_normal_abs;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions; 
        discreteActions[0] = int.Parse(UIController.Instance.bearingInputField.text);
     //   ship.SetEngineSpeed((Ship.Engine)3);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Uboat>(out Uboat uboat))
        {
            if (collision.gameObject.GetComponent<Ship>() == chaserBox.enemyShip)
            {
                Debug.Log("hit");
                SetReward(+1f);
                TrainingController.Instance.LogTrainingSuccess();
                EndEpisode();
            }
           
        }

    }


    //  float both = Mathf.Cos(agentAngleNormalized) + Mathf.Sin(agentAngleNormalized);
    //  Debug.Log("norm: "+ agentAngleNormalized + "  sum: "+ both);
    //Debug.Log("norm: "+ agentAngleNormalized);
    //float both = Mathf.Cos(agentAngleNormalized) + Mathf.Sin(agentAngleNormalized);
    //Debug.Log("cos: " + Mathf.Cos(agentAngleNormalized) + "  sin   " + Mathf.Sin(agentAngleNormalized) + " both: "+ both);
    //Debug.Log("normmin: " + agentAngleNormalizedMin);
    //float bothmin = Mathf.Cos(agentAngleNormalizedMin) + Mathf.Sin(agentAngleNormalizedMin);
    //Debug.Log("cos: " + Mathf.Cos(agentAngleNormalizedMin) + "  sin   " + Mathf.Sin(agentAngleNormalizedMin) + " bothmin: " + bothmin);
    //Debug.Log("normplus: " + agentAngleNormalizedPlus);
    //float bothplus = Mathf.Cos(agentAngleNormalizedPlus) + Mathf.Sin(agentAngleNormalizedPlus);
    //Debug.Log("cos: " + Mathf.Cos(agentAngleNormalizedPlus) + "  sin   " + Mathf.Sin(agentAngleNormalizedPlus) + " bothplus: " + bothplus);

}
