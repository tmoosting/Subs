using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class OpenAgent : Agent
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
        float xPos = Random.Range(-21f, 15f);
        float yPos = Random.Range(-15f, 21f);
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
      // sensor.AddObservation((int)GetComponent<Ship>().GetTargetBearing());
        sensor.AddObservation((int)GetComponent<Ship>().obtainLocationBearing(targetTransform.position));
        //bool sonarDetected = false;
        //bool lookoutDetected = false;
        //foreach (Observation obs in GetComponent<Captain>().ongoingObservations)
        //{
        //    if (obs.observedShip.transform == targetTransform)
        //    {
        //        Vector3 direction = (obs.observedShip.transform.position - transform.position).normalized;
        //        float k_MaxDistance = 10;
        //        var normalizedDistance = Vector3.Distance(transform.position, obs.observedShip.transform.position) / k_MaxDistance;
        //        sensor.AddObservation(direction);
        //        sensor.AddObservation(normalizedDistance);

        //        if (obs.type == Observation.Type.SONAR)
        //        {
        //            sonarDetected = true;
        //        }
        //        else if (obs.type == Observation.Type.LOOKOUT)
        //        {
        //            lookoutDetected = true;
        //        }
        //    }
        //}
        //if (sonarDetected == false && lookoutDetected == false)
        //{
        //    // provide nonsense info
        //    Vector3 direction = (transform.position - transform.position).normalized;
        //    float k_MaxDistance = 10;
        //    var normalizedDistance = Vector3.Distance(transform.position, transform.position) / k_MaxDistance;
        //    sensor.AddObservation(direction);
        //    sensor.AddObservation(normalizedDistance);
        //}
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        float k_MaxDistance = 15;
        var normalizedDistance = Vector3.Distance(targetTransform.position, transform.position) / k_MaxDistance;
        sensor.AddObservation(direction);
        sensor.AddObservation(normalizedDistance); 
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float distance = Vector3.Distance(targetTransform.position, transform.position);
        if (distance > Vector3.Distance(targetTransform.position, prevPos))
            AddReward(-0.01f);
        if (TrainingController.Instance.doneInitializing == true)
        {

          //    Debug.Log("getting bearing: " + actions.DiscreteActions[0]);
         //   int engine = actions.DiscreteActions[0];
            int bearing = actions.DiscreteActions[0];
            // int engineInt = actions.DiscreteActions[1]; 
            //   Debug.Log(GetComponent<Ship>().name + " getting engine value " + engineInt + " which is engine "+ (Ship.Engine)engineInt);

            GetComponent<Ship>().SetCourse(bearing);
            //  GetComponent<Ship>().SetEngineSpeed((Ship.Engine)engine );

            int difference = Mathf.Abs(actions.DiscreteActions[0]- (int)GetComponent<Ship>().obtainLocationBearing(targetTransform.position));
         //   AddReward(-0.1f * difference);
        }


    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //if (Input.GetMouseButtonDown(2))
        //{
        //    Vector3 targetVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //    continuousActions[0] = targetVector.x;
        //    continuousActions[1] = Input.GetAxisRaw("Vertical");
        //    continuousActions[2] = Input.GetAxisRaw("Vertical");

        //}

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions; 
     //   discreteActions[0] = int.Parse(UIController.Instance.trainingEngineInputField.text);
        discreteActions[0] = int.Parse(UIController.Instance.bearingInputField.text);







    }
    public void TriggerSubmergedProximity()
    {
        SetReward(+1f);
        //  Debug.Log("succes!---------------------------------------------------------");
        TrainingController.Instance.LogTrainingSuccess();
        prevPos = transform.position;

        EndEpisode();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        //   Debug.Log("hit!---------------------------------------------------------------");
        if (collision.gameObject.TryGetComponent<Uboat>(out Uboat uboat))
        {
            SetReward(+1f);
            //     Debug.Log("succes!---------------------------------------------------------");
            TrainingController.Instance.LogTrainingSuccess();
            //    captainBox.ColorSuccess();
            EndEpisode();
        }
 
    }




}
