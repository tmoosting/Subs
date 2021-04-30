using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "New Destroyer", menuName = "Destroyer", order = 51)]
public class Destroyer : Ship
{

    Lookout lookout;
    Radar radar;
    Sonar sonar;





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



        if (Input.GetKeyDown(KeyCode.C))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (gameObject.name == "Destroyer (1)")
            {
                // RedistributeVelocity(); 
                Debug.Log("rb velo x: " + rb.velocity.x + "    y :   " + rb.velocity.y); 

            }
        }


            if (Input.GetKeyDown(KeyCode.B))
        {
            if (gameObject.name == "Destroyer (1)")
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();

                float magnitude = rb.velocity.magnitude;

                rb.velocity = new Vector2(0, 0);
                rb.AddForce(transform.up * magnitude);
                //    transform.Rotate(0, 0, -0.5f); 
                Debug.Log("velo x: " + GetComponent<Rigidbody2D>().velocity.x);
                Debug.Log("velo y: " + GetComponent<Rigidbody2D>().velocity.y);
            }
    


            //GetComponent<Rigidbody2D>().AddForce(transform.up * 10 * Time.deltaTime);
        }

       
         
    }
}
