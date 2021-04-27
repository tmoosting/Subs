using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public static GameController Instance;


    List<Destroyer> destroyerList = new List<Destroyer>();
    List<Merchant> merchantList = new List<Merchant>();
    List<Uboat> uboatList = new List<Uboat>();


    [HideInInspector]
    public Ship selectedShip;
    
    [Header("Assign Objects")]
    public GameObject shipHolder;

    [Header("General Settings")]
    public float gameSpeed;

    [Header("Destroyer Settings")]
    public float destroyerFullSpeed;
    public float destroyerStandardSpeed;
    public float destroyerHalfSpeed;


    public float destroyerAccelerationSpeed; // merchants use same for now
    public float destroyerDrag;    
    public float destroyerTurnRate;
    public int depthChargeStock;


    [Header("Uboat Settings")]
    public float uboatAccelerationSpeed;  // also  
    public float uboatResistance;
    public float uboatMaxSpeed;
    public float uboatTurnRate;
    public int torpedoStock;
    public int maxSubmergeTime;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        InitializeProgram();
    }
    void InitializeProgram()
    {
        InitializeShips();
        UIController.Instance.LoadShipsIntoShipBars();

    }
    void InitializeShips()
    {
        foreach (Transform child in shipHolder.transform)
        {
            if (child.gameObject.GetComponent<Destroyer>() != null)
                destroyerList.Add(child.gameObject.GetComponent<Destroyer>());
            else if (child.gameObject.GetComponent<Uboat>() != null)
                uboatList.Add(child.gameObject.GetComponent<Uboat>());
            else if (child.gameObject.GetComponent<Merchant>() != null)
                merchantList.Add(child.gameObject.GetComponent<Merchant>()); 
        }
        Debug.Log("Found  " + destroyerList.Count + " Destroyer, " + merchantList.Count + " Merchant, " + uboatList.Count + " Uboat");
    }




    public void SetSelectedShip(Ship ship)
    {
        selectedShip = ship;
        UIController.Instance.UpdateSelectedShip(selectedShip);
    }


   public List<Destroyer> GetDestroyers()
    { 
        return destroyerList;
    }
    public List<Merchant> GetMerchants()
    {
        return merchantList;
    }
    public List<Uboat> GetUboats()
    {
        return uboatList;
    }
}
