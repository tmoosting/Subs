using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRigid : MonoBehaviour
{

   


    [SerializeField]
    public GameObject bow;
    [SerializeField]
    float turnSpeed = 0.062f;
    [SerializeField]
    float moveSpeed = 35f;
    [SerializeField]
    float stopDistance = 1f;


    


    private Vector3 targetPosition;
    private float targetDistance;
    private bool enRoute = false;




    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MoveToPosition();
            //  targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            enRoute = true;
        }

        if (enRoute)
        {
            float actualTurnSpeed = turnSpeed * targetDistance; 
            float actualMoveSpeed = moveSpeed * targetDistance;

            //Important /!\ : you need to add Linear drag on your rigidbody or it will keep adding
            GetComponent<Rigidbody2D>().AddForce(transform.up * actualMoveSpeed * Time.deltaTime);

            var newRotation = Quaternion.LookRotation(transform.position - targetPosition, Vector3.forward);
            newRotation.x = 0f;
            newRotation.y = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * actualTurnSpeed);


            // within distance
          // if (Vector3.Distance (bow.transform.position, targetPosition) < stopDistance)
            if (Vector2.Distance (bow.transform.position, targetPosition) < stopDistance)
            {
                // stop
                enRoute = false;
            }
        }
    




    }

    void MoveToPosition()
    {
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetDistance = Vector3.Distance(targetPosition, transform.position);
    }


}
