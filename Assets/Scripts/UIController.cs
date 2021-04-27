using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;


    [Header("Assigns")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI engineText;
    public TextMeshProUGUI bearingText;

   // [HideInInspector]
    public List<ShipBar> shipBarList = new List<ShipBar>();

    private void Awake()
    {
        Instance = this;
        nameText.gameObject.SetActive(false);
    }



    public void UpdateSelectedShip(Ship ship)
    {
       // Debug.Log("Selecting " + ship.name);
        nameText.gameObject.SetActive(true);
        nameText.text = ship.name;
        engineText.text = ship.engine.ToString();
        bearingText.text = ship.bearing.ToString();


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
