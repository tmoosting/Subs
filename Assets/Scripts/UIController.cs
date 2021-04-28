using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;


    [Header("Assigns")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI engineSpeedText;
    public TextMeshProUGUI bearingText;
    public GameObject followIcon;
     
    
     

    // [HideInInspector]
    public List<ShipBar> shipBarList = new List<ShipBar>();

    private void Awake()
    {
        Instance = this;
        nameText.gameObject.SetActive(false);
        followIcon.gameObject.SetActive(false);
    }
    private void Update()
    {
        LeftRightSelection(); 
        UpdateShipWindow();
    } 
    public void LoadShipInShipWindow(Ship ship)
    { 
        nameText.gameObject.SetActive(true);
        nameText.text = ship.name;
        UpdateShipWindow();
    }
    void UpdateShipWindow()
    {
        // continuous updates
        Ship selectedShip = GameController.Instance.selectedShip;
        engineSpeedText.text = selectedShip.GetSpeed().ToString();
        bearingText.text = selectedShip.GetBearing().ToString();

    }



    // ------------------ Called from ShipWindow
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
        GameController.Instance.selectedShip.SetEngineSpeed(Ship.Engine.Still);

    }
    public void SetFlankEngine()
    {
        GameController.Instance.selectedShip.SetEngineSpeed(Ship.Engine.Flank);

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
}
