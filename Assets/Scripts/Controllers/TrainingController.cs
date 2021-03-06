using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class TrainingController : MonoBehaviour
{
    public static TrainingController Instance;

    public enum TrainingMode {CONTSIMPLE, DISCBEARING}
    // BEHAVIOR SETTINGS:
    // CONTSIMPLE: obs: 4, cont 2
    // DISCBEARING: obs 4, disc 1/360
     
    
    [Header("Settings")]
    public bool enableTrainingMode; // disables sonar ping. disable middlemousemove, disable some gamecontroller stuff
    public int numberOfAttempts;



    int trainingSuccessCount = 0;
    int trainingAttemptCount = 0;

   [HideInInspector]public bool doneInitializing; // because lazy

    private void Awake()
    {
        Instance = this; 
    }


    public void LogTrainingSuccess()
    {
        trainingSuccessCount++;
        UIController.Instance.UpdateTrainingResults(trainingSuccessCount, trainingAttemptCount);
    }
    public void LogTrainingFail()
    {
        UIController.Instance.UpdateTrainingResults(trainingSuccessCount, trainingAttemptCount);

    }
    public void LogTrainingAttempt()
    {
        trainingAttemptCount++;
        UIController.Instance.UpdateTrainingResults(trainingSuccessCount, trainingAttemptCount);

        if (trainingAttemptCount >= numberOfAttempts)
        {
            Debug.Log("Finished " + numberOfAttempts + " attempts with " + trainingSuccessCount + " successes");
            EditorApplication.isPlaying = false;
        }
    }

    //void InitializeShips()
    //{
    //    // getting errors, commented out to leave it for now
    //    foreach (Destroyer destroyer in destroyerList)
    //    {
    //        if (destroyer.gameObject.GetComponent<Captain>() != null)
    //        {
    //            if (trainingMode == TrainingMode.CONTSIMPLE)
    //            { 
    //                //BehaviorParameters behaviorParamaters = destroyer.gameObject.GetComponent<BehaviorParameters>();
    //                //behaviorParamaters.BrainParameters.VectorObservationSize = 4; 
    //                //ActionSpec actionSpec = new ActionSpec(2, new int[0]);
    //                //behaviorParamaters.BrainParameters.ActionSpec = actionSpec; 
    //            }
    //            else if (trainingMode == TrainingMode.DISCBEARING)
    //            {
    //               //  BehaviorParameters behaviorParamaters = destroyer.gameObject.GetComponent<BehaviorParameters>();                   
    //                //behaviorParamaters.BrainParameters.VectorObservationSize = 4; 
    //                //int[] discreteBranches = new int[1];        
    //                //ActionSpec actionSpec = new ActionSpec(0, discreteBranches);
    //                //actionSpec.BranchSizes = new int [1];
    //                //actionSpec.NumDiscreteActions = 360;
    //                //behaviorParamaters.BrainParameters.ActionSpec = actionSpec;

    //                destroyer.SetEngineSpeed(destroyerStartSpeed);

    //            }

    //        }

    //    }

    //    doneInitializing = true;
    //}



    //void FindTrainerBoxes()
    //{
    //    if (trainerBoxTransformParent.activeSelf == true)
    //    {
    //        foreach (Transform child in trainerBoxTransformParent.transform)
    //        {
    //            if (child.gameObject.GetComponent<TrainerBox>() != null)
    //                trainerBoxes.Add(child.gameObject.GetComponent<TrainerBox>());
    //        }
    //    }
    //    if (trainerBoxBearingParent.activeSelf == true)
    //    {
    //        foreach (Transform child in trainerBoxBearingParent.transform)
    //        {
    //            if (child.gameObject.GetComponent<TrainerBox>() != null)
    //                trainerBoxes.Add(child.gameObject.GetComponent<TrainerBox>());
    //        }
    //    }
    //    if (trainerBoxBearingNoWallsParent.activeSelf == true)
    //    {
    //        foreach (Transform child in trainerBoxBearingNoWallsParent.transform)
    //        {
    //            if (child.gameObject.GetComponent<TrainerBox>() != null)
    //                trainerBoxes.Add(child.gameObject.GetComponent<TrainerBox>());
    //        }
    //    }
    //}
    //void FindShips()
    //{
    //    // loop through trainer boxes to find ships within them
    //    foreach (TrainerBox box in trainerBoxes)
    //    {
    //        foreach (Transform child in box.gameObject.transform)
    //        {
    //            if (child.gameObject.GetComponent<Destroyer>() != null)
    //                destroyerList.Add(child.gameObject.GetComponent<Destroyer>());
    //            else if (child.gameObject.GetComponent<Merchant>() != null)
    //                merchantList.Add(child.gameObject.GetComponent<Merchant>());
    //            else if (child.gameObject.GetComponent<Uboat>() != null)
    //                uboatList.Add(child.gameObject.GetComponent<Uboat>());
    //        }
    //    }

    //    foreach (Ship ship in destroyerList)
    //        shipList.Add(ship);
    //    foreach (Ship ship in merchantList)
    //        shipList.Add(ship);
    //    foreach (Ship ship in uboatList)
    //        shipList.Add(ship);

    //    foreach  (Ship ship in shipList)
    //    {
    //        anySelectedShip = ship;
    //    }
    //  //  Debug.Log("Found Training Ships: " + destroyerList.Count + " Destroyer, " + merchantList.Count + " Merchant, " + uboatList.Count + " Uboat");
    //}


}
