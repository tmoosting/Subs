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

        float turnSpeed = GameController.Instance.destroyerTurnSpeed * rb.velocity.magnitude;

      //  int shipBearing = GetBearing();

        //   if (shipBearing > targetBearing) 
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, targetBearing);


        var newRotation = Quaternion.LookRotation(transform.position - targetPosition, Vector3.forward);
        newRotation.x = 0f;
        newRotation.y = 0f; 
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * turnSpeed);
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
