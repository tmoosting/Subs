using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManual : MonoBehaviour
{

    public enum ShipPhysics { RIGID, LOOSE }
    float rotationSpeed = 100f;
    float accelerationForce = 0.3f; 
    float maxSpeed = 5.0f;
    float liftOffForce = 5f;
    bool crashed;


    float currentSpeed = 0f;
    float timeElapsed = 0f;

    void Start()
    {
        InitializeShip();
    }

    public void InitializeShip()
    {
     //   transform.position = new Vector3(-0.5f, Random.Range(-4.7f, -5.3f));
    //    transform.localEulerAngles = new Vector3(0f, 0f, 90f); 
        timeElapsed = 0f;
        currentSpeed = 0f; 


    }

    private void Update()
    {
        if (Mathf.Abs(currentSpeed) > maxSpeed)
        {
            if (currentSpeed < -0)
            {
                currentSpeed = -maxSpeed;

            }
            else
            {
                currentSpeed = maxSpeed;
            }
        }
        if (!crashed)
        {
            timeElapsed += Time.deltaTime;
            GetMovement();
        }
    }
    private void GetMovement()
    {
        ManualMove();
    }
    private void ManualMove()
    {
        if (Input.GetAxisRaw("Horizontal") > 0f)
        {
            transform.Rotate(Vector3.forward * -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0f)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetAxisRaw("Vertical") > 0f)
        {
            currentSpeed += accelerationForce;
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= liftOffForce;
                if (currentSpeed < 0)
                {
                    currentSpeed = 0;
                }
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += liftOffForce;
                if (currentSpeed > 0)
                {
                    currentSpeed = 0;
                }
            }
        }
        transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);
    }
}
