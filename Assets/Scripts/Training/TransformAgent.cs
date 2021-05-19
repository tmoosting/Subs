using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TransformAgent : Agent
{


    Vector3 startPos;

    public Transform targetTransform;
    public TrainerBox captainBox;


    private void Awake()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        if (Vector3.Distance(targetTransform.position, transform.position) < 15f)
        {
           // TriggerSubmergedProximity();
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;


        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        GetComponent<Ship>().SetEngineSpeed(Ship.Engine.Still);

        // randomize uboat pos
        float xPos = Random.Range(-4.1f, 2f);
        float yPos = Random.Range(2.65f, -3.39f);
        targetTransform.localPosition = new Vector3(xPos, yPos, targetTransform.localPosition.z);

        // randomize submerged or surfaced
        if (Random.Range(0, 2) == 0)
            targetTransform.GetComponent<Uboat>().Resurface();
        else
            targetTransform.GetComponent<Uboat>().Submerge();

        GetComponent<Captain>().ResetCaptain();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        bool sonarDetected = false;
        bool lookoutDetected = false;
        foreach (Observation obs in GetComponent<Captain>().ongoingObservations)
        { 
            if (obs.observedShip.transform == targetTransform)
            {                
                Vector3 direction = (obs.observedShip.transform.position - transform.position).normalized;
                float k_MaxDistance = 10;
                var normalizedDistance = Vector3.Distance(transform.position, obs.observedShip.transform.position) / k_MaxDistance;
                sensor.AddObservation(direction);
                sensor.AddObservation(normalizedDistance);

                if(obs.type == Observation.Type.SONAR)
                { 
                    sonarDetected = true;
                }
                else if (obs.type == Observation.Type.LOOKOUT)
                {
                    lookoutDetected = true;
                }
            }

        }
        if (sonarDetected == false && lookoutDetected == false)
        {
            // provide nonsense info
            Vector3 direction = (transform.position - transform.position).normalized;
            float k_MaxDistance = 10;
            var normalizedDistance = Vector3.Distance(transform.position, transform.position) / k_MaxDistance;
            sensor.AddObservation(direction);
            sensor.AddObservation(normalizedDistance);
        }
        // k_MaxDistance is a hard-coded value based on what you think the max distance a ship can be away from its target.
        
        
        //    sensor.AddObservation(transform.position);
        //     sensor.AddObservation(targetTransform.position);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (TrainingController.Instance.doneInitializing == true)
        {

            float moveX = actions.ContinuousActions[0];
            float moveY = actions.ContinuousActions[1];
            float moveSpeed = 2f;
            transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;

        }


    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {

        //  ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //    continuousActions[0] = Input.GetAxis("Horizontal");
        //    continuousActions[1] = Input.GetAxisRaw("Vertical");

    }
    public void TriggerSubmergedProximity()
    {
        SetReward(+1f);
      //    Debug.Log("succes!---------------------------------------------------------");
        TrainingController.Instance.LogTrainingSuccess();
        captainBox.ColorSuccess();
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
            captainBox.ColorSuccess();
            EndEpisode();
        }
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            //    Debug.Log("Fail!-------------------------------------------------------------");

            SetReward(-1f);
            TrainingController.Instance.LogTrainingFail();

            //     SoundController.Instance.PlayTorpedoHitSound();
            //   GameObject explosion = Instantiate(GameController.Instance.explosionPrefab);
            //   explosion.transform.position = transform.position;
            //   ParticleSystem explosionParticles = explosion.GetComponent<ParticleSystem>();
            //   explosionParticles.Play();
            captainBox.ColorFail();

            EndEpisode();
        }
    }




}
