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
    public GameObject torpedoPrefab;
    public GameObject explosionPrefab;
    public GameObject depthChargesPrefab; 

    [Header("General Settings")] 
    [Range(0.1f,3)] public float gameSpeed;
    public float knotsPerMagnitude;
    public float loggingInterval; // in seconds

    [Header("Turn Correction")]
    public bool useTurnCorrection;
    public float steeringCorrectionMultiplier;
    public float speedRemnantRemoveAmount;

    [Header("Destroyer Settings")]
    public float destroyerAcceleration;
    public float destroyerStandardSpeed; // in knots
    public float destroyerTurnSpeed;
    public float depthChargeTriggerRange;
    public float depthChargeCooldown;
    public float depthChargeExplodeDelay; // keep higher than cooldown value for sound effect

    [Header("Merchant Settings")] 
    public float merchantAcceleration;
    public float merchantStandardSpeed;
    public float merchantTurnSpeed;

    [Header("Uboat Settings")]
    public float uboatStandardSpeedAbove;
    public float uboatStandardSpeedBelow;
    public float uboatAccelerationAbove;
    public float uboatAccelerationBelow;
    public float uboatTurnSpeedAbove;
    public float uboatTurnSpeedBelow;
    public float torpedoInitialForce;
    public float torpedoMaxSpeed;
    public float torpedoAccelerationRate;
    public float torpedoImpactDelay; 
    public float maxSearchRange = 75.0f;
    public float restartSearch = 2.5f;
    public float engagementDistance = 5.0f;
    public float torpedoCooldown = 5.0f;
    public float uboatFleeDistance = 6.0f;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (TrainingController.Instance.enableTrainingMode == false)
            InitializeProgram();
    }
    private void Update()
    { 
        Time.timeScale = gameSpeed*0.75f;
    }
    public void SetGameSpeed(float speed)
    {
        gameSpeed = speed;
    }
    void InitializeProgram()
    {
            FindShips();
            // select a ship and highlight it
            SetSelectedShip(destroyerList[0]);
            foreach (ShipBar bar in UIController.Instance.shipBarList)
                if (bar.containedShip == GameController.Instance.selectedShip)
                    bar.SetSprite(3);

            // set ship engines, bearings, specifics
            InitializeShips();

            UIController.Instance.LoadShipsIntoShipBars();
            UIController.Instance.CreateShipCards();
            UpdateHighlights();
        
    }
    void FindShips()
    {
        // Find the ships and add to list
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

    void InitializeShips()
    {
        foreach (Ship ship in shipList)
        {
            ship.SetCourse(ship.GetCurrentBearing()); // same as 999
        }
        foreach (Ship ship in merchantList)
        {
            ship.SetCourse(999); // its current bearing becomes its targert
            ship.SetEngineSpeed(Ship.Engine.Standard);
        }

        StartCoroutine(ShipLogging(loggingInterval));
      
    }
    IEnumerator ShipLogging(float time)
    {
        yield return new WaitForSeconds(time);
        foreach (Ship ship in shipList)        
            ship.LogPosition();
        StartCoroutine(ShipLogging(loggingInterval));
    }

    public void SetSelectedShip(Ship ship)
    {
        selectedShip = ship;
        UIController.Instance.LoadShipInShipWindow(selectedShip);
        if (CameraController.Instance.followMode == true)
            CameraController.Instance.ZoomToShip(ship);
        UpdateHighlights(); 
    }

    void UpdateHighlights()
    {
        foreach (ShipBar bar in UIController.Instance.shipBarList)
            if (bar.containsShip == true)
                bar.SetSprite(1);
        foreach (ShipCard card in UIController.Instance.shipCardList)
            if (card.containsShip == true)
                card.SetSprite(1);

        foreach (ShipBar bar in UIController.Instance.shipBarList)
            if (bar.containedShip == selectedShip)
                bar.SetSprite(3);
        foreach (ShipCard card in UIController.Instance.shipCardList)
            if (card.containedShip == selectedShip)
                card.SetSprite(3);
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


    public void DestroyShip (Ship ship)
    {
        shipList.Remove(ship);
        if (ship.shipType == Ship.ShipType.DESTROYER)
            destroyerList.Remove(ship.GetComponent<Destroyer>());
        if (ship.shipType == Ship.ShipType.MERCHANT)
            merchantList.Remove(ship.GetComponent<Merchant>());
        if (ship.shipType == Ship.ShipType.UBOAT)
            uboatList.Remove(ship.GetComponent<Uboat>());

        Destroy(ship.gameObject);
    }


    public void TorpedoImpactAt(Vector3 position)
    {
        SoundController.Instance.PlayTorpedoHit();
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = position;
        ParticleSystem explosionParticles = explosion.GetComponent<ParticleSystem>();
        explosionParticles.Play();
    }

    public void DepthChargesAt (Vector3 position, Vector3 direction)
    {
      // Debug.Log("direction x: " + direction.x + " direction y: " + direction.y + "  direction z: " + direction.z);

        SoundController.Instance.PlayChargeSplash(); 
        GameObject explosion1 = Instantiate(depthChargesPrefab);
        explosion1.transform.position = position;
        ParticleSystem explosionParticles1 = explosion1.GetComponent<ParticleSystem>();
        explosion1.transform.rotation = Quaternion.Euler(-90, 90, -90); 
        explosionParticles1.Play(); 

    }
 

}
