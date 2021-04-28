using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Ship : MonoBehaviour
{
  
    public enum ShipType { DESTROYER, MERCHANT, UBOAT  }
    public enum Engine { Still, Third, Half, Standard, Flank  }
    

    [SerializeField]
    public ShipType shipType;
  
    public Engine currentEngine;
    public int currentBearing;
    bool engineReverse;

    private void Awake()
    {
        currentBearing = GetBearing();
    }


    private void OnMouseDown()
    {
        GameController.Instance.SetSelectedShip(this);
    }

    public void SetCourse (int bearing)
    {

        // rotate around z axis proportional to current speed
    }
    public void SetEngineSpeed(Engine engine)
    {
        currentEngine = engine;
    }

    


    public float GetSpeed()
    {
        return GetComponent<Rigidbody2D>().velocity.magnitude;
    }
    public int GetBearing()
    { 
        return (360- Mathf.Abs((int)transform.rotation.eulerAngles.z));
    }

 


    public void SetEngineSpeed()
    {

    }

}
