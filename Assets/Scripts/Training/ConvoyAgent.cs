using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ConvoyAgent : Agent
{
     

    [Header("State Settings")]
    public bool panicMode;

    [Header("Scene Settings")] 
    public Ship.Engine startEngine;  

    [Header("Observation")]
    [Tooltip("space: 1")] public bool sendAgentAngle; // angle agentship is facing  
    [Tooltip("space: 3")] public bool sendTargetDirectionVector; // vector3 
    [Tooltip("space: 1")] public bool sendTargetDistance; // float
    [Tooltip("space: 1")] public bool sendPanicMode; // bool
    
    [Header("Actions")]
    public bool useSpecificBearing;
    public bool useAdjustmentBearing;
    public bool setEngineSpeed;

    [Header("Rewards")]
    public bool baseNegativeTick;
    public float baseReward;
    public bool penalizeIncreasedDistance;
    public bool rewardDecreasedDistance;
    public float distanceRewardMultiplier;  
    public float minMerchantRewardRange; 
    public float maxMerchantRewardRange; 

    Vector3 prevPos; 
    int previousCounter = 0;
    float maxDistance;
    Ship ship;
    Ship nearestMerchant;
    Ship nearestUboat;
    bool uboatSpotted = false;
    bool merchantRangeSet = false;

    // for debug 
    float agentAngle;
    int adjBearingAction = 0;
    float distanceReward = 0f;
    float angleReward = 0f;
    float locBearing = 0f;
    float prevDistanceNormalized = 0f;
    float currentDistanceNormalized = 0f; 


    private void Awake()
    {
        ship = GetComponent<Ship>();
        prevPos = transform.position; 
    }
    private void FixedUpdate()
    {
        previousCounter++;
        if (previousCounter >= 2)
        {
            previousCounter = 0;
            prevPos = transform.position; 
        } 

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (transform.parent.name == "SearcherBox (1)")
            { 
                Debug.Log("agentAngle : " + agentAngle);
                Debug.Log("adjBearingAction : " + adjBearingAction);
                Debug.Log("locBearing : " + locBearing);
                Debug.Log("prevDistanceNormalized : " + prevDistanceNormalized);
                Debug.Log("currentDistanceNormalized : " + currentDistanceNormalized);
                Debug.Log("distanceReward : " + distanceReward);  
                Debug.Log("angleReward : " + angleReward);
            }


        }
    }


    public override void OnEpisodeBegin()
    {
        TrainingController.Instance.LogTrainingAttempt();

        GameController.Instance.ResetStartSituation();
         
        agentAngle = 0f;
        adjBearingAction = 0;
        distanceReward = 0f;
        angleReward = 0f;
        locBearing = 0f;
        prevDistanceNormalized = 0f;
        currentDistanceNormalized = 0f;
          
        GetComponent<Ship>().SetEngineSpeed(startEngine);

        // resets observations 
        GetComponent<Captain>().ResetCaptain();

        // one of our white squares is ~0.73 distance
        maxDistance = GetComponent<Lookout>().rangeMax * 2;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       if (GameController.Instance.GetAlliedOngoingObservations().Count != 0)
        {
            Vector3 targetDirection = new Vector3(0, 0, 0);
            float targetDistance = 0;

            if (panicMode == false)
            {
                // find nearest merchant
                float minDistance = 999999f;
                foreach (Observation obs in GameController.Instance.GetAlliedOngoingObservations())
                {
                    if (obs.observedShip.shipType == Ship.ShipType.MERCHANT)
                    {
                        if (Vector3.Distance(transform.position, obs.observedShip.transform.position) < minDistance)
                        {
                            minDistance = Vector3.Distance(transform.position, obs.observedShip.transform.position);
                            nearestMerchant = obs.observedShip;
                        }
                    }
                }
                targetDirection = (nearestMerchant.transform.position - transform.position).normalized;
                targetDistance = Vector3.Distance(nearestMerchant.transform.position, transform.position) / maxDistance;
            }
            else if (panicMode == true)
            {
                uboatSpotted = false;
                float minDistance = 999999f;
                foreach (Observation obs in GameController.Instance.GetAlliedOngoingObservations())
                {
                    if (obs.observedShip.shipType == Ship.ShipType.UBOAT)
                    {
                        uboatSpotted = true;
                        if (Vector3.Distance(transform.position, obs.observedShip.transform.position) < minDistance)
                        {
                            minDistance = Vector3.Distance(transform.position, obs.observedShip.transform.position);
                            nearestUboat = obs.observedShip;
                        }
                    }
                }
                if (uboatSpotted == true)
                {
                    targetDirection = (nearestUboat.transform.position - transform.position).normalized;
                    targetDistance = Vector3.Distance(nearestUboat.transform.position, transform.position) / maxDistance;
                }
            }

            float agentAngleNormalized = (float)((ship.GetCurrentBearing() % 360) / 360f);
            agentAngle = agentAngleNormalized; // for debug

            if (sendAgentAngle == true)
                sensor.AddObservation(agentAngleNormalized);
            if (sendPanicMode == true)
                sensor.AddObservation(panicMode);
            if (sendTargetDirectionVector == true)
                sensor.AddObservation(targetDirection);
            if (sendTargetDistance == true)
                sensor.AddObservation(targetDistance);

            if (merchantRangeSet == false)
            {
                // manually set 
                //merchantRangeSet = true;
                //float distance = Vector3.Distance(nearestMerchant.transform.position, transform.position);
                //minMerchantRewardRange = distance - (distance * 0.1f);
                //maxMerchantRewardRange = distance + (distance * 0.1f); 
            }
        }
       else
        {
            if (sendAgentAngle == true)
                sensor.AddObservation(0);
            if (sendPanicMode == true)
                sensor.AddObservation(0);
            if (sendTargetDirectionVector == true)
            {
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
            } 
            if (sendTargetDistance == true)
                sensor.AddObservation(0); 

        }

    }

    public override void OnActionReceived(ActionBuffers actions)
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
         

            AddStepRewards();
    }
    void AddStepRewards()
    {
      
        if (panicMode == false)
        {
            if (nearestMerchant != null)
            {
                float distance = Vector3.Distance(nearestMerchant.transform.position, transform.position);

                if (distance > minMerchantRewardRange && distance < maxMerchantRewardRange)
                    AddReward(baseReward);
                else
                    AddReward(-baseReward);


                if (ship.GetCurrentBearing() == nearestMerchant.GetCurrentBearing())
                    AddReward(baseReward);
                else
                    AddReward(-baseReward);
            } 
        }

        else if (panicMode == true)
        {
            if (uboatSpotted == true)
            {
                prevDistanceNormalized = (nearestUboat.transform.position - prevPos).magnitude / maxDistance;
                currentDistanceNormalized = (nearestUboat.transform.position - transform.position).magnitude / maxDistance;
            }
        
        }
      //  prevDistanceNormalized = (chaserBox.enemyShip.transform.position - prevPos).magnitude / maxDistance;
    //    currentDistanceNormalized = (chaserBox.enemyShip.transform.position - transform.position).magnitude / maxDistance;

        if (baseNegativeTick == true)
        {
            AddReward(-baseReward);

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
                distanceReward = baseReward * distanceRewardMultiplier;// for debug
                AddReward((baseReward * distanceRewardMultiplier));
            }
        }
 

    }

 

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        //discreteActions[0] = int.Parse(UIController.Instance.bearingInputField.text);
        //if (setEngineSpeed == true)
        //{
        //    int enemyBearing = ((int)GetComponent<Ship>().obtainLocationBearing(chaserBox.enemyShip.transform.position));
        //    if (ship.GetCurrentBearing() == enemyBearing)
        //        discreteActions[1] = 4;
        //    else
        //        discreteActions[1] = 2;
        //    ship.SetEngineSpeed((Ship.Engine)discreteActions[1]);
        //} 

    }
  
     
    public void DepthChargesThrown()
    {
        if (panicMode== true)
        {
             SetReward(+1f);
            TrainingController.Instance.LogTrainingSuccess();
            EndEpisode();
        }
    }
    public void TimeToPanic()
    {
        panicMode = true;
    }
    public void TimeToChill()
    { 
        panicMode = false;

    }
}
