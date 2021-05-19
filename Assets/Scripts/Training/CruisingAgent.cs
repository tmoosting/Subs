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
        float xPos = Random.Range(-6f, 6);
        float yPos = Random.Range(6, -6);
        if (xPos < 1 & xPos > -1)
            xPos *= 4;
        if (yPos < 1 & yPos > -1)
            yPos *= 4;
        targetTransform.localPosition = new Vector3(xPos, yPos, targetTransform.localPosition.z);
        //   targetTransform.localPosition = new Vector3(xPos, targetTransform.localPosition.y, targetTransform.localPosition.z);

        // randomize submerged or surfaced
        if (Random.Range(0, 2) == 0)
            targetTransform.GetComponent<Uboat>().Resurface();
        else
            targetTransform.GetComponent<Uboat>().Submerge();

        GetComponent<Captain>().ResetCaptain();
    }

    
    public override void CollectObservations(VectorSensor sensor)
    {
        Ship ship = GetComponent<Ship>();

        float max_dist = 10;
        float normalized_angle = (float)(((GetComponent<Ship>().GetTargetBearing() % 360) - 180f) / 180f);
   //    float normalized_angle = Vector3.SignedAngle(transform.forward, targetTransform.position - transform.position, Vector3.up) / 180f;
   //   float normalized_distance = Vector3.Distance(targetTransform.position, transform.position) / max_dist;
        float normalized_prev_distance = (targetTransform.position - prevPos).magnitude / max_dist;
          float normalized_distance = (targetTransform.position - transform.position).magnitude / max_dist;

     
        //   int normObtain = ((int)Mathf.Round(GetComponent<Ship>().obtainLocationBearing(targetTransform.position)) - 180f) / 180f;

        //float obtainLocBearing = (float)ship.obtainLocationBearing(targetTransform.position);
        //int difference_in_bearing = (int)Mathf.Round(obtainLocBearing - ship.GetCurrentBearing() % 360);
        //difference_in_bearing = difference_in_bearing > 180 ? 360 - difference_in_bearing : difference_in_bearing;
        //float normal = (difference_in_bearing - 90f) / 90f;

        int difference_in_bearing = (int)Mathf.Round((float)ship.obtainLocationBearing(targetTransform.position) - ship.GetCurrentBearing() % 360);
        difference_in_bearing = difference_in_bearing > 180 ? 360 - difference_in_bearing : difference_in_bearing;
        float normal = difference_in_bearing / 180f;

        float absNormal = Mathf.Abs(normal);


        //Debug.Log("obtain loc bearing : " + ship.obtainLocationBearing(targetTransform.position));
        //Debug.Log("curr bearing : " + ship.GetCurrentBearing());
        //Debug.Log("diff bearing : " + difference_in_bearing);
        //Debug.Log("normal bearing : " + normal);
 

        sensor.AddObservation(absNormal); 
        sensor.AddObservation(normalized_distance);
      //  Debug.Log("norm bearing : " + normal);
      //   Debug.Log("norm angle : " + normalized_angle);
     //   Debug.Log("norm dist : " + normalized_distance);
      //  Debug.Log("adding reward: " + (1 - Mathf.Abs(normalized_angle)) * rewardFactor);
       // AddReward((1 - Mathf.Abs(normalized_angle)) * rewardFactor);   
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Ship ship = GetComponent<Ship>();
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
                // subtract one, counterclockwise
                if (currentBearing - 1 <= 0 )
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
                if (currentBearing + 1 >= 360 )
                    newBearing = 0;
                else
                    newBearing = currentBearing + 1;
            }
            GetComponent<Ship>().SetCourse(newBearing);
            //  GetComponent<Ship>().SetEngineSpeed((Ship.Engine)engine ); 
            ///
            //int bearing = actions.DiscreteActions[0];
            //GetComponent<Ship>().SetCourse(bearing);
            // Debug.Log("bearing: " + bearing);

            float max_dist = 15;
            float normalized_angle = (float)(((GetComponent<Ship>().GetTargetBearing() % 360) - 180f) / 180f);
            //    float normalized_angle = Vector3.SignedAngle(transform.forward, targetTransform.position - transform.position, Vector3.up) / 180f;
            //   float normalized_distance = Vector3.Distance(targetTransform.position, transform.position) / max_dist;
            float normalized_prev_distance = (targetTransform.position - prevPos).magnitude / max_dist;
            float normalized_distance = (targetTransform.position - transform.position).magnitude / max_dist;

            int difference_in_bearing = (int)Mathf.Round((float)ship.obtainLocationBearing(targetTransform.position) - ship.GetCurrentBearing() % 360);
            difference_in_bearing = difference_in_bearing > 180 ? 360 - difference_in_bearing : difference_in_bearing;
            float normal = difference_in_bearing / 180f;

            float absNormal = Mathf.Abs(normal);


            //Debug.Log("obtain loc bearing : " + ship.obtainLocationBearing(targetTransform.position));
            //Debug.Log("curr bearing : " + ship.GetCurrentBearing());
            //Debug.Log("diff bearing : " + difference_in_bearing);
            //Debug.Log("normal bearing : " + normal);

            float rewardFactor = -0.1f;
            AddReward(absNormal * rewardFactor);

            if (normalized_prev_distance > normalized_distance)
                AddReward(rewardFactor * 5f);
            //if (normalized_prev_distance < normalized_distance)
            //    AddReward(-rewardFactor * 5f);
        }


    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        //   discreteActions[0] = int.Parse(UIController.Instance.trainingEngineInputField.text);
        discreteActions[0] = int.Parse(UIController.Instance.bearingInputField.text);



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
