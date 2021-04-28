using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public static GameController Instance;


    List<Destroyer> destroyerList = new List<Destroyer>();
    List<Merchant> merchantList = new List<Merchant>();
    List<Uboat> uboatList = new List<Uboat>();
    List<Ship> shipList = new List<Ship>();


    [HideInInspector]
    public Ship selectedShip { get; private set; }
    
    [Header("Assign Objects")]
    public GameObject shipHolder;

    [Header("General Settings")]
    public float gameSpeed;
    public float magnitudeThird;
    public float magnitudeHalf;
    public float magnitudeStandard;
    public float magnitudeFlank;

    [Header("Destroyer Settings")]
    public float destroyerEnginePower;
    public float destroyerTurnSpeed;
     


    [Header("Uboat Settings")]
    public float uboatEnginePower;  


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

        SetSelectedShip(destroyerList[0]);
        foreach (ShipBar bar in UIController.Instance.shipBarList)
        {
            if (bar.containedShip == GameController.Instance.selectedShip)
                bar.SetSprite(3);
        }  
    }
    void InitializeShips()
    {
        foreach (Transform child in shipHolder.transform)
        {
            if (child.gameObject.GetComponent<Destroyer>() != null)
                destroyerList.Add(child.gameObject.GetComponent<Destroyer>());
            else if (child.gameObject.GetComponent<Merchant>() != null)
                merchantList.Add(child.gameObject.GetComponent<Merchant>());
            else if (child.gameObject.GetComponent<Uboat>() != null)
                uboatList.Add(child.gameObject.GetComponent<Uboat>());
          
        }
        foreach (Ship ship in destroyerList)
            shipList.Add(ship);
        foreach (Ship ship in merchantList)
            shipList.Add(ship);
        foreach (Ship ship in uboatList)
            shipList.Add(ship);

        Debug.Log("Found  " + destroyerList.Count + " Destroyer, " + merchantList.Count + " Merchant, " + uboatList.Count + " Uboat");
    }




    public void SetSelectedShip(Ship ship)
    {
        selectedShip = ship;
        UIController.Instance.LoadShipInShipWindow(selectedShip);
        if (CameraController.Instance.zoomedToShip == true)
            CameraController.Instance.ZoomToShip(ship);

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
    public List<Ship> GetAllShips()
    {
        return shipList;
    }
}
