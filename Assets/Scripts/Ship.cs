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
    public int targetBearing;
    bool engineReverse;

    private void Awake()
    {
        currentBearing = GetCurrentBearing();
    }


 
    private void FixedUpdate()
    {
        // Engine logic
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (currentEngine == Engine.Third)
            if (rb.velocity.magnitude < GameController.Instance.magnitudeThird)
                PowerEngine();
        if (currentEngine == Engine.Half)
            if (rb.velocity.magnitude < GameController.Instance.magnitudeHalf)
                PowerEngine();
        if (currentEngine == Engine.Standard)
            if (rb.velocity.magnitude < GameController.Instance.magnitudeStandard)
                PowerEngine();
        if (currentEngine == Engine.Flank)
            if (rb.velocity.magnitude < GameController.Instance.magnitudeFlank)
                PowerEngine();


        // Turning logic   
        if (GetCurrentBearing() == 0 || GetCurrentBearing() == 360)
        {
            // pointing straight up 
            if (GetTargetBearing() < 180 )
                transform.Rotate(0, 0, -GetTurnSpeed()); // moves clockwise
            else
                transform.Rotate(0, 0, GetTurnSpeed()); // moves counterclockwise
        }
        else // bearing is not 0
        {
            if (GetCurrentBearing() < GetTargetBearing())
            {
                if (GetTargetBearing() - GetCurrentBearing() < 180 )                
                    transform.Rotate(0, 0, -GetTurnSpeed()); // moves clockwise                
                else                
                    transform.Rotate(0, 0, GetTurnSpeed()); // moves counterclockwise                
            }
            else if (GetCurrentBearing() > GetTargetBearing())
            {
                if (GetCurrentBearing() - GetTargetBearing() < 180)                
                    transform.Rotate(0, 0, GetTurnSpeed()); // moves counterclockwise                
                else                
                    transform.Rotate(0, 0, -GetTurnSpeed()); // moves clockwise                
            }  
        }
     

        // Update current forces




    }

    float GetTurnSpeed()
    {
        return 0.05f;
    }
    void PowerEngine()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        if (shipType == ShipType.DESTROYER)
            rb.AddForce(transform.up * GameController.Instance.destroyerEnginePower * Time.deltaTime);
        else if (shipType == ShipType.MERCHANT)
            rb.AddForce(transform.up * GameController.Instance.destroyerEnginePower * Time.deltaTime);
        else if (shipType == ShipType.UBOAT)
            rb.AddForce(transform.up * GameController.Instance.uboatEnginePower * Time.deltaTime);
         
    }


    public void SetCourse (int bearing)
    {
        if (bearing == 999)
            targetBearing = currentBearing;
        else
          targetBearing = bearing; 
    }
    public void SetEngineSpeed(Engine engine)
    {
        currentEngine = engine;
    }
    public void SetReverse (bool reversed)
    {
        engineReverse = reversed;
    }
    


    public float GetSpeed()
    {
        return GetComponent<Rigidbody2D>().velocity.magnitude;
    }
    public int GetCurrentBearing()
    {
         return (360- Mathf.Abs((int)transform.rotation.eulerAngles.z)); 
    }
    public int GetTargetBearing()
    {
        return targetBearing;
    }




    private void OnMouseDown()
    {
        GameController.Instance.SetSelectedShip(this);
    }
}
