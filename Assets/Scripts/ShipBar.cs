using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipBar : MonoBehaviour
{
    
    
    public GameObject shipIcon;
    public GameObject shipName;
    bool containsShip = false;

    [HideInInspector]
    public Ship containedShip;

    private void Start()
    { 
        gameObject.SetActive(false);
    }

    public void AssignShip(Ship ship)
    {
        containedShip = ship;
        shipName.GetComponent<TextMeshProUGUI>().text = ship.gameObject.name;
        gameObject.SetActive(true); 
        SetSprite(1); 
        containsShip = true;
    }

    private void OnMouseEnter()
    {
        if (GameController.Instance.selectedShip != containedShip)
            SetSprite(2);
    }
    private void OnMouseExit()
    {
        if (GameController.Instance.selectedShip != containedShip)
            SetSprite(1);
    }
    private void OnMouseDown()
    {
        if (containsShip == true)
        {
            if (GameController.Instance.selectedShip != containedShip)
            {
                GameController.Instance.SetSelectedShip(containedShip);
                foreach (ShipBar bar in UIController.Instance.shipBarList)
                    if (bar.containsShip == true)
                       bar.SetSprite(1);
                SetSprite(3);
            }
        }
      
    }

    void SetSprite(int mode)
    {
        
        if (containedShip.shipType == Ship.ShipType.DESTROYER)
        {
            if (mode == 1)
                shipIcon.GetComponent<SpriteRenderer>().sprite = SpriteController.Instance.iconDestroyer;
            else if (mode == 2)
                shipIcon.GetComponent<SpriteRenderer>().sprite = SpriteController.Instance.iconDestroyerGreen;
            else if (mode == 3)
                shipIcon.GetComponent<SpriteRenderer>().sprite = SpriteController.Instance.iconDestroyerGreener;
        }
        else if (containedShip.shipType == Ship.ShipType.MERCHANT)
        {
            if (mode == 1)
                shipIcon.GetComponent<SpriteRenderer>().sprite = SpriteController.Instance.iconMerchant;
            else if (mode == 2)
                shipIcon.GetComponent<SpriteRenderer>().sprite = SpriteController.Instance.iconMerchantGreen;
            else if (mode == 3)
                shipIcon.GetComponent<SpriteRenderer>().sprite = SpriteController.Instance.iconMerchantGreener;
        }
        else if (containedShip.shipType == Ship.ShipType.UBOAT)
        {
            if (mode == 1)
                shipIcon.GetComponent<SpriteRenderer>().sprite = SpriteController.Instance.iconUboat;
            else if (mode == 2)
                shipIcon.GetComponent<SpriteRenderer>().sprite = SpriteController.Instance.iconUboatGreen;
            else if (mode == 3)
                shipIcon.GetComponent<SpriteRenderer>().sprite = SpriteController.Instance.iconUboatGreener;
        }

    }
}
