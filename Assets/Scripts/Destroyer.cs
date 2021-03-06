using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "New Destroyer", menuName = "Destroyer", order = 51)]
public class Destroyer : Ship
{
     
    int depthChargesremaining = 66;
    bool depthChargeCooldownActive = false;

    void RedistributeVelocity()
    {

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float previousMagnitude = rb.velocity.magnitude;
        Vector2 previousVelocity = rb.velocity;
        rb.velocity = new Vector2(previousVelocity.x * 1.1f, previousVelocity.y * 0.9f);
    }
    private void Update()
    {
        //horizontalInput = Input.GetAxis("Horizontal");
        //steerFactor = Mathf.Lerp(steerFactor, -horizontalInput, Time.deltaTime);
        //transform.Rotate(0.0f, 0, steerFactor * steerSpeed);
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    if (gameObject.name == "Destroyer (1)")
        //    {
        //        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        //        float magnitude = rb.velocity.magnitude;

        //        rb.velocity = new Vector2(0, 0);
        //        rb.AddForce(transform.up * magnitude);
        //        //    transform.Rotate(0, 0, -0.5f); 
        //        Debug.Log("velo x: " + GetComponent<Rigidbody2D>().velocity.x);
        //        Debug.Log("velo y: " + GetComponent<Rigidbody2D>().velocity.y);
        //    }
        //}

        if (movingToTarget)
        {
            SetCourseToLocation(targetLocation);
        }
        
        if (depthChargeCooldownActive == false)
        {
            foreach (Uboat uboat in GameController.Instance.GetUboats())
            { 
                if (Vector3.Distance(uboat.gameObject.transform.position, transform.position) < GameController.Instance.depthChargeTriggerRange)
                {
                    FireDepthCharges(uboat);

                }
            }
        }
   
    }

    public void TestFireDepthCharges( )
    {
        depthChargeCooldownActive = true;
        StartCoroutine(DepthChargeCooldownReset());
        depthChargesremaining -= 6;
        Vector3 direction = new Vector3 (1,1,0);
        GameController.Instance.DepthChargesAt(transform.position, direction);

         
    }
    public void FireDepthCharges(Uboat uboat)
    {
        depthChargeCooldownActive = true;
        StartCoroutine(DepthChargeCooldownReset());
        depthChargesremaining -= 6;
        Vector3 direction = (transform.position - uboat.transform.position).normalized;
        GameController.Instance.DepthChargesAt(transform.position, direction);
        uboat.EatDepthCharge();
        if (GetComponent<ConvoyAgent>() != null)
        {
            GetComponent<ConvoyAgent>().DepthChargesThrown();
        }

        // sink subs in a specific direction. extra explosion on hit
    }
    
    IEnumerator DepthChargeCooldownReset()
    {
        yield return new WaitForSeconds(GameController.Instance.depthChargeCooldown); 
        depthChargeCooldownActive = false;
    }

    // collision: sinks uboats and merchants, weakest destroyer sinks
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.GetComponent<Ship>() != null)
    //    {
    //        Ship hitShip = collision.gameObject.GetComponent<Ship>();
    //        if (ignoreCollisionList.Contains(hitShip) == false)
    //        {
    //            ignoreCollisionList.Add(hitShip);
    //            Rigidbody2D rb = GetComponent<Rigidbody2D>();
    //            Rigidbody2D rbHit = hitShip.GetComponent<Rigidbody2D>();

    //            if (hitShip.shipType == ShipType.UBOAT || hitShip.shipType == ShipType.MERCHANT)
    //            {
    //                //  hitShip.GetRammed();
    //            }
    //            else if (hitShip.shipType == ShipType.DESTROYER)
    //            {
    //                //if (rb.velocity.magnitude >= rbHit.velocity.magnitude)
    //                //    hitShip.GetRammed();
    //                //else
    //                //    GetRammed();
    //            }
    //            rb.velocity = Vector3.zero;
    //            rb.angularVelocity = 0;
    //        }

    //    }
    //}


}
