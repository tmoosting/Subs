using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CruisingAgent : Agent
{


    Vector3 startPos;
    Vector3 prevPos;
    int prevPosCounter = 0;

    public Transform targetTransform;


    private void Awake()
    {
        startPos = transform.position;
        prevPos = transform.position;
    }
    private void Update()
    {
        if (Vector3.Distance(targetTransform.position, transform.position) < 15f)
        {
            // TriggerSubmergedProximity();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            LogTargetRelativebearing();
        }
        LogTargetRelativebearing();
        prevPosCounter++;
        if (prevPosCounter >= 5)
        {
            prevPosCounter = 0;
            prevPos = transform.position;
        }

    }
    void LogTargetRelativebearing()
    {
        //   Debug.Log("uboat has " +(int) GetComponent<Ship>().obtainLocationBearing(targetTransform.position));
        UIController.Instance.bearingInputField.text = ((int)GetComponent<Ship>().obtainLocationBearing(targetTransform.position)).ToString();
    }

    public override void OnEpisodeBegin()
    {
        TrainingController.Instance.LogTrainingAttempt();
        transform.position = startPos;
        transform.rotation = Quaternion.identity;

        // reset agent physics settings
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;

        // reset uboat physics settings
        Rigidbody2D rbsub = targetTransform.GetComponent<Rigidbody2D>();
        rbsub.velocity = Vector3.zero;
        rbsub.angularVelocity = 0;

        GetComponent<Ship>().SetEngineSpeed(Ship.Engine.Standard);
        UIController.Instance.bearingInputField.text = "0";
        UIController.Instance.trainingEngineInputField.text = "0";

        // randomize uboat pos
        float xPos = Random.Range(-13f, 7);
        float yPos = Random.Range(13, 7);
        targetTransform.localPosition = new Vector3(xPos, yPos, targetTransform.localPosition.z);
        //   targetTransform.localPosition = new Vector3(xPos, targetTransform.localPosition.y, targetTransform.localPosition.z);

        // randomize submerged or surfaced
        if (Random.Range(0, 2) == 0)
            targetTransform.GetComponent<Uboat>().Resurface();
        else
            targetTransform.GetComponent<Uboat>().Submerge();

        GetComponent<Captain>().ResetCaptain();
    }

    float rewardFactor = 0.01f;
    public override void CollectObservations(VectorSensor sensor)
    {
        float max_dist = 15;
        float normalized_angle = (float)(((GetComponent<Ship>().GetTargetBearing() % 360) - 180f) / 180f);
   //    float normalized_angle = Vector3.SignedAngle(transform.forward, targetTransform.position - transform.position, Vector3.up) / 180f;
   //   float normalized_distance = Vector3.Distance(targetTransform.position, transform.position) / max_dist;
        float normalized_prev_distance = (targetTransform.position - prevPos).magnitude / max_dist;
          float normalized_distance = (targetTransform.position - transform.position).magnitude / max_dist;

        if (normalized_prev_distance > normalized_distance)
            AddReward(-0.01f);

        sensor.AddObservation(normalized_angle); 
        sensor.AddObservation(normalized_distance);
         Debug.Log("norm angle : " + normalized_angle);
     //   Debug.Log("norm dist : " + normalized_distance);
      //  Debug.Log("adding reward: " + (1 - Mathf.Abs(normalized_angle)) * rewardFactor);
      //  AddReward((1 - Mathf.Abs(normalized_angle)) * rewardFactor);   
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
      //  Debug.Log("discrete action: " + actions.DiscreteActions[0]);
      
      
        //int difference = Mathf.Abs(actions.DiscreteActions[0] - (int)GetComponent<Ship>().obtainLocationBearing(targetTransform.position));
        //AddReward(-0.1f * difference);

        if (TrainingController.Instance.doneInitializing == true)
        {
            int actionValue = actions.DiscreteActions[0];
            int currentBearing = GetComponent<Ship>().GetCurrentBearing();
            int newBearing = currentBearing;

            if (actionValue == 0)
            {
                // subtract one
                if (currentBearing - 1 == 0 || currentBearing - 1 == -1)
                    newBearing = 360;
                else
                    newBearing = currentBearing - 1;
            }
            else if (actionValue == 1)
            {
                // do nothing
                newBearing = currentBearing; 
            }
            else if (actionValue == 2)
            {
                if (currentBearing + 1 == 360 || currentBearing + 1 == 361)
                    newBearing = 0;
                else
                    newBearing = currentBearing + 1;
            }

            GetComponent<Ship>().SetCourse(newBearing);
            //  GetComponent<Ship>().SetEngineSpeed((Ship.Engine)engine );

        
        }


    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        



    }
   
    private void OnCollisionEnter2D(Collision2D collision)
    { 
        if (collision.gameObject.TryGetComponent<Uboat>(out Uboat uboat))
        {
            SetReward(+1f); 
            TrainingController.Instance.LogTrainingSuccess(); 
            EndEpisode();
        }

    }




}
