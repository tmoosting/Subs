using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//[CreateAssetMenu(fileName = "New Merchant", menuName = "Merchant", order = 51)]
public class Merchant : Ship
{

    private void Update()
    {
        if (movingToTarget)
        {
            SetCourseToLocation(targetLocation);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ship>() != null)
        {
            Ship hitShip = collision.gameObject.GetComponent<Ship>();
            if (ignoreCollisionList.Contains(hitShip) == false)
            {
                ignoreCollisionList.Add(hitShip);
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                Rigidbody2D rbHit = hitShip.GetComponent<Rigidbody2D>();

                if (hitShip.shipType == ShipType.UBOAT)
                {
                    hitShip.GetRammed();
                }
                else if (hitShip.shipType == ShipType.MERCHANT)
                {
                    if (rb.velocity.magnitude >= rbHit.velocity.magnitude)
                        hitShip.GetRammed();
                    else
                        GetRammed();
                }
                rb.velocity = Vector3.zero;
                rb.angularVelocity = 0;
            }
        
        }
    }
}
