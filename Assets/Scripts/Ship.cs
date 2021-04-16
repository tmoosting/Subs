using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Ship : ScriptableObject
{
  
    public enum ShipType { DESTROYER, ESCORT, MERCHANT, UBOAT  }

    [SerializeField]
    ShipType shipType;


    Sprite shipSprite;




}
