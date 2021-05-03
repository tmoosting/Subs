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

     
    List<Vector2> logList = new List<Vector2>();


    private void Awake()
    {
        currentBearing = GetCurrentBearing();
       //  captain = new Captain();
    }


    private void FixedUpdate()
    {            
        PowerEngine();
        TurnShip();   
    }
     

    public void EatTorpedo()
    {
        StartCoroutine(DestroyAfterDelay(0.3f));
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    IEnumerator DestroyAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        GameController.Instance.DestroyShip(this);
        
    }

    public void LogPosition()
    {
        logList.Add(new Vector2(transform.position.x, transform.position.y));
    //    Debug.Log("logging for " + gameObject.name + " x: " + transform.position.x);
    }

    void PowerEngine()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float maxMagnitude = 0f;

        if (shipType == ShipType.DESTROYER || shipType == ShipType.MERCHANT)
        {
            maxMagnitude = GameController.Instance.destroyerStandardSpeed / GameController.Instance.knotsPerMagnitude;
        }
        else if (shipType == ShipType.UBOAT)
        {
            if (GetComponent<Uboat>().submerged == true)
                maxMagnitude = GameController.Instance.uboatStandardSpeedBelow / GameController.Instance.knotsPerMagnitude;
            else
                maxMagnitude = GameController.Instance.uboatStandardSpeedAbove / GameController.Instance.knotsPerMagnitude;
        }

        if (currentEngine == Engine.Still)
            maxMagnitude *= 0f;
        if (currentEngine == Engine.Third)        
            maxMagnitude *= 0.33f;
        if (currentEngine == Engine.Half)
            maxMagnitude *= 0.5f;
        if (currentEngine == Engine.Standard)
            maxMagnitude *= 1f;
        if (currentEngine == Engine.Flank)
            maxMagnitude *= 1.4f;

        if (rb.velocity.magnitude < maxMagnitude)
            AddEngineForce(); 
    }
    void AddEngineForce()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (shipType == ShipType.DESTROYER)
            rb.AddForce(transform.up * GameController.Instance.destroyerAcceleration * Time.deltaTime);
        else if (shipType == ShipType.MERCHANT)
            rb.AddForce(transform.up * GameController.Instance.destroyerAcceleration * Time.deltaTime);
        else if (shipType == ShipType.UBOAT)
            rb.AddForce(transform.up * GameController.Instance.uboatAccelerationAbove * Time.deltaTime);
    }



    void TurnShip()
    {
        if (Mathf.Abs(GetCurrentBearing() - GetTargetBearing()) > 0.1f)
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
        if (GameController.Instance.useTurnCorrection == true)
        {
            if (GetCurrentBearing() == 90 || GetCurrentBearing() == 270)
                EliminateYSpeedRemnant();
            if (GetCurrentBearing() == 360 || GetCurrentBearing() == 0 || GetCurrentBearing() == 180)
                EliminateXSpeedRemnant();
        }
    }

   

    void TurnClockwise()
    {
        // grab base speed from BaseController
        float baseRotation = -GetTurnSpeed();
        // multiply by current speed to limit turning at slow/nonspeeds
        float actualRotation = baseRotation * GetVelocity();
        // rotate clockwise
         transform.Rotate(0, 0, actualRotation);

        if (GameController.Instance.useTurnCorrection == true)
        {
            if (GetCurrentBearing() == 360 || GetCurrentBearing() == 0 || GetCurrentBearing() < 90)
                AdjustTowardsX(actualRotation, true, true);
            else if (GetCurrentBearing() >= 90 && GetCurrentBearing() < 180)
                AdjustTowardsY(actualRotation, true, false);
            else if (GetCurrentBearing() >= 180 && GetCurrentBearing() < 270)
                AdjustTowardsX(actualRotation, false, false);
            else if (GetCurrentBearing() >= 270 && GetCurrentBearing() < 360)
                AdjustTowardsY(actualRotation, false, true);
        }
    }
    void TurnAntiClockwise()
    {
        float baseRotation = GetTurnSpeed();
        float actualRotation = baseRotation * GetVelocity();
        transform.Rotate(0, 0, actualRotation);

        if (GameController.Instance.useTurnCorrection == true)
        {
            if (GetCurrentBearing() == 0 || GetCurrentBearing() > 270)
                AdjustTowardsX(-actualRotation, false, true);
            else if (GetCurrentBearing() > 180 && GetCurrentBearing() <= 270)
                AdjustTowardsY(-actualRotation, false, false);
            else if (GetCurrentBearing() > 90 && GetCurrentBearing() <= 180)
                AdjustTowardsX(-actualRotation, true, false);
            else if (GetCurrentBearing() > 0 && GetCurrentBearing() <= 90)
                AdjustTowardsY(-actualRotation, true, true);
        } 

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
    void EliminateXSpeedRemnant()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 previousVelocity = rb.velocity;
        rb.velocity = new Vector2(previousVelocity.x * GameController.Instance.speedRemnantRemoveAmount, previousVelocity.y);

    }
    void EliminateYSpeedRemnant()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 previousVelocity = rb.velocity;
        rb.velocity = new Vector2(previousVelocity.x, previousVelocity.y * GameController.Instance.speedRemnantRemoveAmount);
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


    public float GetspeedInKnots()
    {
        return GetComponent<Rigidbody2D>().velocity.magnitude * GameController.Instance.knotsPerMagnitude;
    }
    public float GetVelocity()
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
    float GetTurnSpeed()
    {
        if (shipType == ShipType.UBOAT)
        {
            if (GetComponent<Uboat>().submerged == true)
                return GameController.Instance.uboatTurnSpeedBelow;
            else
                return GameController.Instance.uboatTurnSpeedAbove;
        }
        else
        {
            return GameController.Instance.destroyerTurnSpeed;
        }
    }



    private void OnMouseDown()
    {
        GameController.Instance.SetSelectedShip(this);
    }
}
