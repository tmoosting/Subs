using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Ship : MonoBehaviour
{
  
    public enum ShipType { DESTROYER, MERCHANT, UBOAT  }
    public enum Engine { Still, Third, Half, Standard, Flank  }
    

    [SerializeField]
    public ShipType shipType;
  
    public Engine engine;
    public int bearing;
    bool engineReverse;





    public void SetCourse (int bearing)
    {

        // rotate around z axis proportional to current speed
    }
    public void SetEngineSpeed(float kmperhour)
    {


    }

    



}
