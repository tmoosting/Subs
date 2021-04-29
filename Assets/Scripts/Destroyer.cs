using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "New Destroyer", menuName = "Destroyer", order = 51)]
public class Destroyer : Ship
{

    Lookout lookout;
    Radar radar;
    Sonar sonar;

 

    float signedSqrt(float x)
    {
        float r = Mathf.Sqrt(Mathf.Abs(x));
        if (x < 0)
        {
            return -r;
        }
        else
        {
            return r;
        }
    }

    public float speed = 1.0f;
    public float steerSpeed = 10.0f;
    public float movementThresold = 10.0f;
    float verticalInput;
    float movementFactor;
    float horizontalInput;
    float steerFactor ;

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        steerFactor = Mathf.Lerp(steerFactor, -horizontalInput, Time.deltaTime);
        transform.Rotate(0.0f, 0, steerFactor * steerSpeed);






        if (Input.GetKeyDown(KeyCode.B))
        {
            if (gameObject.name == "LeadDestroyer")
            { 
                transform.Rotate(0, 0, -0.5f);
                Debug.Log(name + " current.. " + GetCurrentBearing());
                Debug.Log(name + " target.. " + GetTargetBearing());
            }
   


            //GetComponent<Rigidbody2D>().AddForce(transform.up * 10 * Time.deltaTime);
        }

       
         
    }
}
