using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{

    [SerializeField] private Transform targetTransform;

    Vector3 startPos;
    private void Awake()
    {
        startPos = transform.position;
    }

    public override void OnEpisodeBegin()
    {
     //   transform.position = startPos;
        transform.position = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {

        Debug.Log(actions.ContinuousActions[0]);

        Debug.Log(actions.ContinuousActions[1]);
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];
        float moveSpeed = 1f;
        transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.TryGetComponent<Goal>(out Goal goal))
        //{
        //    SetReward(+1f);
        //    EndEpisode();
        //}
        //if (other.TryGetComponent<Wall>(out Wall wall))
        //{
        //    SetReward(-1f);
        //    EndEpisode();
        //}
    }

   
}
