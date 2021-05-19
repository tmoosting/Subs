using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using Michsky.UI.ModernUIPack;

public class UIController : MonoBehaviour
{
    public static UIController Instance;


    [Header("Assigns")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI currentEngineSpeedText;
    public TextMeshProUGUI targetEngineSpeedText;
    public TextMeshProUGUI engineSelectorText;
    public HorizontalSelector engineSelector;
    public TextMeshProUGUI currentBearingText;
    public TextMeshProUGUI targetBearingText;
    public GameObject followIcon;
    public TMP_InputField bearingInputField;
    public TMP_InputField trainingEngineInputField;
    public GameObject shipCardPrefab;
    public GameObject shipCardParent; 
    public GameObject moveToMarker; 
    public GameObject setBearingButton;
    public TextMeshProUGUI trainingCounter;
    public Slider speedSlider;
    public TextMeshProUGUI speedText;


    public List<ShipBar> shipBarList = new List<ShipBar>();
   [HideInInspector]
    public List<ShipCard> shipCardList = new List<ShipCard>();
    [HideInInspector]
    public bool strategyViewEnabled;

    private void Awake()
    {
        Instance = this;
        nameText.gameObject.SetActive(false);
        followIcon.gameObject.SetActive(false);
        moveToMarker.SetActive(false); 
        bearingInputField.onValueChanged.AddListener(delegate { BearingInputValueChanged(); });
    }
    private void Update()
    { 
        LeftRightSelection(); 
        UpdateShipWindow();
        CheckForStrategyView();

        if (Input.GetMouseButtonDown(2))
        {
            if (TrainingController.Instance.enableTrainingMode == false)
            {
                // toggle-type middle mouse click
                GameController.Instance.selectedShip.ToggleTargetMovement(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (GameController.Instance.selectedShip.movingToTarget)
                {
                    moveToMarker.SetActive(true);
                    Vector3 markerPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
                    moveToMarker.transform.position = markerPos;
                }
                else
                    moveToMarker.SetActive(false);
            }
            else
            {
                // do this in openagent heuristic
            }

        }


        if (Input.GetKeyDown(KeyCode.P))
            GameController.Instance.useTurnCorrection = !GameController.Instance.useTurnCorrection;

        if (Input.GetKeyDown(KeyCode.T))
            if (GameController.Instance.selectedShip.shipType == Ship.ShipType.UBOAT)
                GameController.Instance.selectedShip.GetComponent<Uboat>().FireTorpedo();

        if (Input.GetKeyDown(KeyCode.U))
            if (GameController.Instance.selectedShip.shipType == Ship.ShipType.UBOAT)
                GameController.Instance.selectedShip.GetComponent<Uboat>().ToggleSubmerge();

        if (Input.GetKeyDown(KeyCode.X))
            if (GameController.Instance.selectedShip.shipType == Ship.ShipType.DESTROYER)
                GameController.Instance.selectedShip.GetComponent<Destroyer>().TestFireDepthCharges();

        if (Input.GetKeyDown(KeyCode.O))
            if (GameController.Instance.selectedShip.shipType == Ship.ShipType.DESTROYER)
                LogOngoingObservations();

        if (Input.GetKeyDown(KeyCode.B))
            if (GameController.Instance.selectedShip.shipType == Ship.ShipType.UBOAT)
                GameController.Instance.selectedShip.GetComponent<Uboat>().ThrowPillenwerfer();
    } 
    void LogOngoingObservations()
    {
        Debug.Log("Obs ");
        if (GameController.Instance.selectedShip.GetComponent<Captain>() != null)
        {
            foreach (Observation obs in GameController.Instance.selectedShip.GetComponent<Captain>().ongoingObservations)
            {
          
            }
        }
     
    }
    void CheckForStrategyView()
    {
        if (shipCardList.Count > 0)
        {
            if (strategyViewEnabled == true)
            {
                foreach (ShipCard card in shipCardList)
                {
                    if (card.gameObject != null)
                    {
                        card.gameObject.SetActive(true);
                        card.transform.position = card.containedShip.transform.position;
                    }
                   
                }
            }
            else
            {
                foreach (ShipCard card in shipCardList)
                {
                    if (card.gameObject != null)
                        card.gameObject.SetActive(false);
                }
            }
        }
      
    }
    public void LoadShipInShipWindow(Ship ship)
    { 
        nameText.gameObject.SetActive(true);
        nameText.text = ship.name; 
         
        if (ship.currentEngine == Ship.Engine.Still)
            engineSelector.index = 0;
        if (ship.currentEngine == Ship.Engine.Third)
            engineSelector.index = 1;
        if (ship.currentEngine == Ship.Engine.Half)
            engineSelector.index = 2;
        if (ship.currentEngine == Ship.Engine.Standard)
            engineSelector.index = 3;
        if (ship.currentEngine == Ship.Engine.Flank)
            engineSelector.index = 4;
        engineSelector.UpdateUI();

        bearingInputField.text = ship.GetTargetBearing().ToString();

        UpdateShipWindow();
    }
    void UpdateShipWindow()
    {
        // for continuous updates

        Ship selectedShip = GameController.Instance.selectedShip;
        if (selectedShip != null)
        {
            currentEngineSpeedText.text = selectedShip.GetspeedInKnots().ToString("F1") + " knots";
            currentBearingText.text = selectedShip.GetCurrentBearing().ToString();
            targetBearingText.text = "Target: " + selectedShip.GetTargetBearing().ToString();
            if (selectedShip.movingToTarget == true)
            {
                targetBearingText.fontStyle = FontStyles.Underline | FontStyles.Bold;
                setBearingButton.gameObject.SetActive(false);
                bearingInputField.gameObject.SetActive(false);
            }
            else
            {
                //    targetBearingText.fontStyle ^= FontStyles.Bold;
                //  targetBearingText.fontStyle ^= FontStyles.Underline;            
                targetBearingText.fontStyle = FontStyles.Normal;
                setBearingButton.gameObject.SetActive(true);
                bearingInputField.gameObject.SetActive(true);
            }
        }
         
         
      
           
    }


    public void CreateShipCards()
    {
        foreach (Ship ship in GameController.Instance.GetAllShips())
        { 
            GameObject obj = Instantiate(shipCardPrefab);
            ShipCard card = obj.GetComponent<ShipCard>();
            card.AssignShip(ship);
            card.transform.position = ship.transform.position;
            obj.transform.SetParent(shipCardParent.transform);
            obj.transform.localScale = new Vector3(1, 1, 1);
        }
    }
     





    // ------------------ Called from ShipWindow UI
    public void SetStillEngine( )
    {
        GameController.Instance.selectedShip.SetEngineSpeed(Ship.Engine.Still);
    }
    public void SetThirdEngine()
    {
        GameController.Instance.selectedShip.SetEngineSpeed(Ship.Engine.Third);

    }
    public void SetHalfEngine()
    {
        GameController.Instance.selectedShip.SetEngineSpeed(Ship.Engine.Half);

    }
    public void SetStandardEngine()
    {
        GameController.Instance.selectedShip.SetEngineSpeed(Ship.Engine.Standard);

    }
    public void SetFlankEngine()
    {
        GameController.Instance.selectedShip.SetEngineSpeed(Ship.Engine.Flank);

    }
    public void SetReverse(bool reversed)
    {
        GameController.Instance.selectedShip.SetReverse(reversed);
    }

    void BearingInputValueChanged()
    {
        if (int.Parse(bearingInputField.text) > 360)
            bearingInputField.text = "360";

    }
    public void SetBearing()
    {  
        if (GameController.Instance.selectedShip != null)
        GameController.Instance.selectedShip.SetCourse(int.Parse(bearingInputField.text));
    }





    void LeftRightSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            int index = GameController.Instance.GetAllShips().IndexOf(GameController.Instance.selectedShip);
            if (index + 1 != GameController.Instance.GetAllShips().Count)
                GameController.Instance.SetSelectedShip(GameController.Instance.GetAllShips()[index + 1]);
            else
                GameController.Instance.SetSelectedShip(GameController.Instance.GetAllShips()[0]);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            int index = GameController.Instance.GetAllShips().IndexOf(GameController.Instance.selectedShip);
            if (index - 1 > -1)
                GameController.Instance.SetSelectedShip(GameController.Instance.GetAllShips()[index - 1]);
            else
                GameController.Instance.SetSelectedShip(GameController.Instance.GetAllShips()[GameController.Instance.GetAllShips().Count - 1]);
        }
    }

    public void LoadShipsIntoShipBars()
    {
        foreach (Destroyer destroyer in GameController.Instance.GetDestroyers()) 
            foreach (ShipBar bar in shipBarList) 
                if (bar.containedShip == null)
                {
                    bar.AssignShip(destroyer);
                    break;
                }
        foreach (Merchant merchant in GameController.Instance.GetMerchants())
            foreach (ShipBar bar in shipBarList)
                if (bar.containedShip == null)
                {
                    bar.AssignShip(merchant);
                    break;
                }
        foreach (Uboat uboat in GameController.Instance.GetUboats())
            foreach (ShipBar bar in shipBarList)
                if (bar.containedShip == null)
                {
                    bar.AssignShip(uboat);
                    break;
                }

    }

    public void UpdateTrainingResults (int successes, int fails)
    {
        trainingCounter.text = "Success: " + successes + "\nAttempts: " + fails;
    }
    public void SetSpeedText (float speed)
    {
        speedText.text = speed.ToString("F1");
    }
}
