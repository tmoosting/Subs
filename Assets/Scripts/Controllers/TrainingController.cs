using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Assigns")]
    public GameObject trainerBoxParent;

    [Header("General Settings")]
    public bool enableTrainingMode;
    public TrainingMode trainingMode;
    public Ship.Engine destroyerStartSpeed;
    public Color successColor;
    public Color failColor;

    [HideInInspector] public bool doneInitializing = false;
    int trainingSuccessCount = 0;
    int trainingFailCount = 0;

    List<TrainerBox> trainerBoxes = new List<TrainerBox>();
    List<Destroyer> destroyerList = new List<Destroyer>();
    List<Merchant> merchantList = new List<Merchant>();
    List<Uboat> uboatList = new List<Uboat>();
    List<Ship> shipList = new List<Ship>();


    private void Awake()
    {
        Instance = this;
        if (enableTrainingMode == true)
        {
            FindTrainerBoxes();
            FindShips();
            InitializeShips();
        }
    }
    private void Start()
    {
    
      
    }

    void InitializeShips()
    {
        // getting errors, commented out to leave it for now
        foreach (Destroyer destroyer in destroyerList)
        {
            if (destroyer.gameObject.GetComponent<Captain>() != null)
            {
                if (trainingMode == TrainingMode.CONTSIMPLE)
                { 
                    //BehaviorParameters behaviorParamaters = destroyer.gameObject.GetComponent<BehaviorParameters>();
                    //behaviorParamaters.BrainParameters.VectorObservationSize = 4; 
                    //ActionSpec actionSpec = new ActionSpec(2, new int[0]);
                    //behaviorParamaters.BrainParameters.ActionSpec = actionSpec; 
                }
                else if (trainingMode == TrainingMode.DISCBEARING)
                { 
                    //BehaviorParameters behaviorParamaters = destroyer.gameObject.GetComponent<BehaviorParameters>();
                    //behaviorParamaters.BrainParameters.VectorObservationSize = 4; 
                    //int[] discreteBranches = new int[1];        
                    //ActionSpec actionSpec = new ActionSpec(0, discreteBranches);
                    //actionSpec.BranchSizes = new int [1];
                    //actionSpec.NumDiscreteActions = 360;
                    //behaviorParamaters.BrainParameters.ActionSpec = actionSpec;
                }

            }

        }
        foreach (Destroyer destroyer in destroyerList)
        {
            destroyer.SetEngineSpeed(destroyerStartSpeed);
        }
        doneInitializing = true;
    }



    void FindTrainerBoxes()
    {
        foreach (Transform child in trainerBoxParent.transform)
        {
            if (child.gameObject.GetComponent<TrainerBox>() != null) 
                trainerBoxes.Add(child.gameObject.GetComponent<TrainerBox>()); 
        }
    }
    void FindShips()
    {
        // loop through trainer boxes to find ships within them
        foreach (TrainerBox box in trainerBoxes)
        {
            foreach (Transform child in box.gameObject.transform)
            {
                if (child.gameObject.GetComponent<Destroyer>() != null)
                    destroyerList.Add(child.gameObject.GetComponent<Destroyer>());
                else if (child.gameObject.GetComponent<Merchant>() != null)
                    merchantList.Add(child.gameObject.GetComponent<Merchant>());
                else if (child.gameObject.GetComponent<Uboat>() != null)
                    uboatList.Add(child.gameObject.GetComponent<Uboat>());
            }
        }
     
        foreach (Ship ship in destroyerList)
            shipList.Add(ship);
        foreach (Ship ship in merchantList)
            shipList.Add(ship);
        foreach (Ship ship in uboatList)
            shipList.Add(ship);
        Debug.Log("Found Training Ships: " + destroyerList.Count + " Destroyer, " + merchantList.Count + " Merchant, " + uboatList.Count + " Uboat");
    }
    public void LogTrainingSuccess()
    {
        trainingSuccessCount++;
        UIController.Instance.UpdateTrainingResults(trainingSuccessCount, trainingFailCount);
    }
    public void LogTrainingFail()
    {
        trainingFailCount++;
        UIController.Instance.UpdateTrainingResults(trainingSuccessCount, trainingFailCount);

    }


}
