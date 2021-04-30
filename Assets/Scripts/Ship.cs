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

    Captain captain;

    private void Awake()
    {
        currentBearing = GetCurrentBearing();
        captain = new Captain();
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
        if (Mathf.Abs( GetCurrentBearing() - GetTargetBearing()) > 0.1f)
        {
            if (GetCurrentBearing() == 0 || GetCurrentBearing() == 360)
            {
                // pointing straight up 
                if (GetTargetBearing() < 180)
                    TurnClockwise();
                else
                    TurnAntiClockwise();
            }
            else // bearing is not 0
            {
                if (GetCurrentBearing() < GetTargetBearing())
                {
                    if (GetTargetBearing() - GetCurrentBearing() < 180)
                        TurnClockwise();
                    else
                        TurnAntiClockwise();
                }
                else if (GetCurrentBearing() > GetTargetBearing())
                {
                    if (GetCurrentBearing() - GetTargetBearing() < 180)
                        TurnAntiClockwise();
                    else
                        TurnClockwise();
                }
            }
        }


        // manually kill speeds at tiny angles :(
        //if (( GetCurrentBearing() > 85 && GetCurrentBearing() < 95) || (GetCurrentBearing() > 265 && GetCurrentBearing() < 275))
        //{
        //    EliminateYSpeedRemnant();
        //}
        //if ( (GetCurrentBearing() > 355  || GetCurrentBearing() < 5) || (GetCurrentBearing() > 175 && GetCurrentBearing() < 185))
        //{
        //    EliminateXSpeedRemnant();
        //}
        if (GetCurrentBearing() == 90 || GetCurrentBearing() == 270)
            EliminateYSpeedRemnant();
        if (GetCurrentBearing() == 360 || GetCurrentBearing() == 0 || GetCurrentBearing() == 180)
            EliminateXSpeedRemnant();

    }
    void EliminateXSpeedRemnant()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 previousVelocity = rb.velocity;
        rb.velocity = new Vector2(previousVelocity.x * GameController.Instance.speedRemnantRemoveAmount, previousVelocity.y) ;

    }
    void EliminateYSpeedRemnant()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 previousVelocity = rb.velocity;
        rb.velocity = new Vector2(  previousVelocity.x, previousVelocity.y * GameController.Instance.speedRemnantRemoveAmount);
    }

    void TurnClockwise()
    {
        // grab base speed from BaseController
        float baseRotation = -GetTurnSpeed();
        // multiply by current speed to limit turning at slow/nonspeeds
        float actualRotation = baseRotation * GetSpeed();
        // rotate clockwise
         transform.Rotate(0, 0, actualRotation);

        if (GetCurrentBearing() == 360 || GetCurrentBearing() == 0 || GetCurrentBearing() < 90)
            AdjustTowardsX(actualRotation, true, true);
        else if (GetCurrentBearing() >= 90 && GetCurrentBearing() < 180)
            AdjustTowardsY(actualRotation, true, false);
        else if (GetCurrentBearing() >= 180 && GetCurrentBearing() < 270)
            AdjustTowardsX(actualRotation, false, false);
        else if (GetCurrentBearing() >= 270 && GetCurrentBearing() < 360)
            AdjustTowardsY(actualRotation, false, true);
    }
    void TurnAntiClockwise()
    {
        float baseRotation = GetTurnSpeed();
        float actualRotation = baseRotation * GetSpeed();
        transform.Rotate(0, 0, actualRotation);



        if ( GetCurrentBearing() == 0 || GetCurrentBearing() > 270)
            AdjustTowardsX(-actualRotation, false, true);
        else if (GetCurrentBearing() > 180 && GetCurrentBearing() <= 270)
           AdjustTowardsY(-actualRotation, false, false);
         else if (GetCurrentBearing() > 90 && GetCurrentBearing() <= 180)
           AdjustTowardsX(-actualRotation, true, false);
         else if (GetCurrentBearing() > 0 && GetCurrentBearing() <= 90)
            AdjustTowardsY(-actualRotation, true, true);
        //if (GetCurrentBearing() == 360 || GetCurrentBearing() == 0)
        //    AdjustTowardsX(actualRotation, false, false); 
        //else if (GetCurrentBearing() > 0 && GetCurrentBearing() <= 90)
        //    AdjustTowardsY(actualRotation, true, false);
        //else if (GetCurrentBearing() > 90 && GetCurrentBearing() <= 180)
        //    AdjustTowardsX(actualRotation, true, false);
        //else if (GetCurrentBearing() > 180 && GetCurrentBearing() <= 270)
        //    AdjustTowardsY(actualRotation, false, true);
        //else if (GetCurrentBearing() > 270 && GetCurrentBearing() < 360)
        //    AdjustTowardsX(actualRotation, false, true);

    }
    void AdjustTowardsX(float actualRotation, bool positiveXAdjustment, bool positiveYAdjustment)
    {
        float multiplier = (100 + (actualRotation * GameController.Instance.steeringCorrectionMultiplier)) / 100;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 previousVelocity = rb.velocity;
        float speedSwapped = rb.velocity.y - (rb.velocity.y * multiplier);
        if (positiveYAdjustment == false)
            speedSwapped = -speedSwapped;
        if (positiveXAdjustment == true && positiveYAdjustment == true)
            rb.velocity = new Vector2(previousVelocity.x + speedSwapped, previousVelocity.y - speedSwapped);
        else if (positiveXAdjustment == true && positiveYAdjustment == false)
            rb.velocity = new Vector2(previousVelocity.x + speedSwapped, previousVelocity.y + speedSwapped);
        else if (positiveXAdjustment == false && positiveYAdjustment == true)
            rb.velocity = new Vector2(previousVelocity.x - speedSwapped, previousVelocity.y - speedSwapped);
        else if (positiveXAdjustment == false && positiveYAdjustment == false) 
            rb.velocity = new Vector2(previousVelocity.x - speedSwapped, previousVelocity.y + speedSwapped); 
    }
    void AdjustTowardsY(float actualRotation, bool positiveXAdjustment, bool positiveYAdjustment)
    {
        float multiplier = (100 + (actualRotation * GameController.Instance.steeringCorrectionMultiplier)) / 100;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 previousVelocity = rb.velocity;
        float speedSwapped = rb.velocity.x - (rb.velocity.x * multiplier);
        if (positiveXAdjustment == false)
            speedSwapped = -speedSwapped;
        if (positiveXAdjustment == true && positiveYAdjustment == true)
            rb.velocity = new Vector2(previousVelocity.x - speedSwapped, previousVelocity.y + speedSwapped);
        else if (positiveXAdjustment == true && positiveYAdjustment == false)
            rb.velocity = new Vector2(previousVelocity.x - speedSwapped, previousVelocity.y - speedSwapped);
        else if (positiveXAdjustment == false && positiveYAdjustment == true)
            rb.velocity = new Vector2(previousVelocity.x + speedSwapped, previousVelocity.y + speedSwapped);
        else if (positiveXAdjustment == false && positiveYAdjustment == false)
            rb.velocity = new Vector2(previousVelocity.x + speedSwapped, previousVelocity.y - speedSwapped); 
    }

    float GetTurnSpeed()
    {
        if (shipType == ShipType.UBOAT)
        {
            return GameController.Instance.uboatTurnSpeed;
        }
        else
        {
            return GameController.Instance.destroyerTurnSpeed;
        }
             
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
