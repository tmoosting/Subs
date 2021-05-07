using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Skipper : Agent
{
    [SerializeField] private Transform targetTransform;


    Vector3 startPos;
    private void Awake()
    {
        startPos = transform.position;
    }
    public override void OnEpisodeBegin()
    {
        transform.position = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //   Debug.Log(actions.ContinuousActions[0].ToString());
        ////   Debug.Log(actions.ContinuousActions[1]);
        //   float moveX = actions.ContinuousActions[0];
        ////   float moveY = 1f;
        //  float moveY = actions.ContinuousActions[1];
        //   float moveSpeed = 1f;
        //   transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;

        Debug.Log("branch 0: " + actions.ContinuousActions[0]);

        Debug.Log("branch 1: " + actions.ContinuousActions[1]);
        //    Debug.Log(actions.DiscreteActions[0]);

        // Debug.Log(actions.DiscreteActions[1]);
        //  float moveX = actions.ContinuousActions[0]; 
        //  float moveY = actions.ContinuousActions[1];
       // float moveSpeed = 0.1f;
        //    transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> discreteActions = actionsOut.ContinuousActions;
        //   discreteActions[0] = Input.GetAxis("Horizontal");
        //   discreteActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Uboat>(out Uboat uboat))
        {
            SetReward(+1f);
            Debug.Log("succes!");
            EndEpisode();
        }
        if (collision.gameObject.TryGetComponent<Merchant>(out Merchant merchant))
        {
            SetReward(-1f);

            SoundController.Instance.PlayTorpedoHitSound();
            GameObject explosion = Instantiate(GameController.Instance.explosionPrefab);
            explosion.transform.position = transform.position;
            ParticleSystem explosionParticles = explosion.GetComponent<ParticleSystem>();
            explosionParticles.Play();

            EndEpisode();
        }

    }
    private void OnTriggerEnter(Collider other)
    {

    }
}
