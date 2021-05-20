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
    [HideInInspector] Dictionary<Ship, Vector3> shipStartingPositions = new Dictionary<Ship, Vector3>();
    [HideInInspector] Dictionary<Ship, Quaternion> shipStartingRotations = new Dictionary<Ship, Quaternion>();


    [Header("Assign Objects")]
    public GameObject shipHolder;
    public GameObject torpedoPrefab;
    public GameObject explosionPrefab;
    public GameObject depthChargesPrefab;
    public GameObject pillenwerferPrefab;

    [Header("General Settings")] 
    [Range(0.1f,10)] public float gameSpeed;
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
    public float panicModeCooldownTime; // if no uboats and torpedoes spotted for this time, disable panicmode on all destroyers
    

    [Header("Merchant Settings")] 
    public float merchantAcceleration;
    public float merchantStandardSpeed;
    public float merchantTurnSpeed;

    [Header("Uboat Settings")]
    public float uboatStandardSpeedAbove = 18f;
    public float uboatStandardSpeedBelow = 8f;
    public float uboatAccelerationAbove = 8f;
    public float uboatAccelerationBelow = 5f;
    public float uboatTurnSpeedAbove = 1.4f;
    public float uboatTurnSpeedBelow = 0.6f;
    public float torpedoInitialForce = 60f;
    public float torpedoMaxSpeed = 140f;
    public float torpedoAccelerationRate = 110f;
    public float torpedoImpactDelay = 0.12f; 

    public float maxSearchRange = 50.0f;
    public float restartSearchRange = 9.0f;

    public float maxRoamRange = 40.0f;
    public float restartRoam = 2.5f;

    public float engagementDistance = 8.0f;

    public float torpedoCooldown = 5.0f;

    public float uboatFleeDistance = 6.0f;
    public float uboatAssistDistance = 12.0f;

    public bool enablePillenwerfers;
    public float pillenwerferDuration = 30.0f;
    public float pillenwerferCooldown = 75.0f;
    public float pillenwerferInitialForce = 30.0f;

    bool speedSet = false;
    int counter = 0;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    { 
        if (TrainingController.Instance.enableTrainingMode == false)
            InitializeProgram();
       
    }
    float timeTracker; 

    private void Update()
    {
        if (TrainingController.Instance.enableTrainingMode == false)
        {
            if (speedSet == false)
            {
                counter++;
                if (counter > 20)
                {
                    speedSet = true;
                    SetSelectedShip(destroyerList[0]);
                }
            }
            if (uboatList.Count == 0)
                UIController.Instance.UboatsGone();
            if (merchantList.Count == 0)
                UIController.Instance.Merchantsgone();
            Time.timeScale = gameSpeed * 0.75f;
            timeTracker += Time.deltaTime;
            foreach (Observation obs in GetAlliedOngoingObservations())
                if (obs.observedShip.shipType == Ship.ShipType.UBOAT)
                {
                    timeTracker = 0;
                }

            if (timeTracker >= panicModeCooldownTime)
            {
                timeTracker = 0;
                foreach (Destroyer destroyer in destroyerList)
                {
                    if (destroyer.GetComponent<ConvoyAgent>() != null)
                    {
                        destroyer.GetComponent<ConvoyAgent>().TimeToChill();
                    }
                }
            }
        }
         
    }
    public void SetGameSpeed(float speed)
    {
        gameSpeed = speed;
    }
    void InitializeProgram()
    {
         
            FindShips();
            
            InitializeShips();

            SetSelectedShip(destroyerList[0]);
            foreach (ShipBar bar in UIController.Instance.shipBarList)
                if (bar.containedShip == GameController.Instance.selectedShip)
                    bar.SetSprite(3);
             

            UIController.Instance.LoadShipsIntoShipBars();
            UIController.Instance.CreateShipCards();
            UpdateHighlights();



        SetSelectedShip(destroyerList[0]);
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
            shipStartingPositions.Add(ship, ship.transform.position);
            shipStartingRotations.Add(ship, ship.transform.rotation);
            ship.SetCourse(ship.GetCurrentBearing()); // same as 999
        }
        foreach (Ship ship in merchantList)
        {
            ship.SetCourse(999); // its current bearing becomes its targert
            ship.SetEngineSpeed(Ship.Engine.Standard);
        }
        foreach (Ship ship in destroyerList)
        {
            ship.SetCourse(999); // its current bearing becomes its targert
            ship.SetEngineSpeed(Ship.Engine.Half);
        }
     //   destroyerList[0].SetEngineSpeed(Ship.Engine.Still);
        StartCoroutine(ShipLogging(loggingInterval));

        // destroyers always have all allied ships observed  
        foreach (Destroyer destroyer in destroyerList) 
            destroyer.GetComponent<Captain>().GatherComms(); 
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
    public List<Observation> GetAlliedOngoingObservations()
    {
        List<Observation> returnList = new List<Observation>();

        foreach (Destroyer destroyer in destroyerList)        
            foreach (Observation obs in destroyer.GetComponent<Captain>().ongoingObservations)            
                if (returnList.Contains(obs) == false)
                    returnList.Add(obs);
        foreach (Merchant merchant in merchantList)
            foreach (Observation obs in merchant.GetComponent<Captain>().ongoingObservations)
                if (returnList.Contains(obs) == false)
                    returnList.Add(obs); 

        return returnList;
    }

    public void DestroyShip (Ship ship)
    {
        if (selectedShip == ship)
        {
            int index = GetAllShips().IndexOf(selectedShip);
            if (index + 1 != GetAllShips().Count)
                SetSelectedShip(GetAllShips()[index + 1]);
            else
                SetSelectedShip(GetAllShips()[0]);
        }
        foreach (ShipBar shipBar in UIController.Instance.shipBarList)
        {
            if (shipBar.containedShip == ship)
                shipBar.ShipDestroyed();
        }
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
 
    public void ResetStartSituation()
    {
        foreach (Ship ship in shipStartingPositions.Keys)        
            ship.transform.position = shipStartingPositions[ship];
        foreach (Ship ship in shipStartingRotations.Keys)
            ship.transform.rotation = shipStartingRotations[ship];
        foreach (Ship ship in shipList)
        {
            Rigidbody2D rb = ship.GetComponent<Rigidbody2D>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0;
        }
        UIController.Instance.bearingInputField.text = "0";
        UIController.Instance.trainingEngineInputField.text = "0";
    }
}
