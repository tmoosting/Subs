using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Captain : Agent
{
     

    Vector3 startPos;

    [Header("Brain")]
    List<Ship> spottedShips = new List<Ship>();
    List<Torpedo> spottedTorpedoes = new List<Torpedo>();
    Dictionary<Ship, float> lastSightings = new Dictionary<Ship, float>();

    [Header("MLAgents")]
    [SerializeField] private Transform targetTransform;
    public TrainerBox captainBox;
    


    private void Awake()
    {
        startPos = transform.position;
    }


    public void SightAShip(Ship ship)
    { 
        if (spottedShips.Contains(ship) == false)
        { 
            Debug.Log("spotted " + ship.name);
            spottedShips.Add(ship);
        }
    } 
    public void DetectSonar(Ship ship)
    { 
        if (spottedShips.Contains(ship) == false)
        {
            // Sound the alarm! 
            Debug.Log("spotted " + ship.name);
            spottedShips.Add(ship);
        }
    }
    public void SightATorpedo(Torpedo torpedo)
    {
        if (spottedTorpedoes.Contains(torpedo) == false)
        {
            // Sound the extra loud alarm! 
            spottedTorpedoes.Add(torpedo);
        }
    }



    //  ML FUNCTIONS -----------------------------------------------------------



    public override void OnEpisodeBegin()
    {
        // transform.position = Vector3.zero;
        transform.position = startPos;
        //   Debug.Log("begin");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;

        // k_MaxDistance is a hard-coded value based on what you think the max distance a ship can be away from its target.
        float k_MaxDistance = 10;
        var normalizedDistance = Vector3.Distance(transform.position, targetTransform.position) / k_MaxDistance;

        sensor.AddObservation(direction);
        sensor.AddObservation(normalizedDistance);
        //    sensor.AddObservation(transform.position);
        //     sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (TrainingController.Instance.doneInitializing == true)
        {
            if (TrainingController.Instance.trainingMode == TrainingController.TrainingMode.CONTSIMPLE)
            {
                float moveX = actions.ContinuousActions[0];
                float moveY = actions.ContinuousActions[1];
                float moveSpeed = 1f;
                transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;
            }
            else if (TrainingController.Instance.trainingMode == TrainingController.TrainingMode.DISCBEARING)
            {
                if (GetComponent<Ship>().machineBearingSet == false)
                {
                    int bearing = actions.DiscreteActions[0];
                    GetComponent<Ship>().SetCourse(bearing);
                    GetComponent<Ship>().machineBearingSet = true;
                }

            }
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //  ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //    continuousActions[0] = Input.GetAxis("Horizontal");
        //    continuousActions[1] = Input.GetAxisRaw("Vertical");
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
