using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "New Destroyer", menuName = "Destroyer", order = 51)]
public class Destroyer : Ship
{

    Lookout lookout;
    Radar radar;
    Sonar sonar;


    private void Update()
    {

       
        if (Input.GetKey(KeyCode.B))
        {
            GetComponent<Rigidbody2D>().AddForce(transform.up * 10 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.C))
        {;
            Debug.Log("current velocity: " + GetComponent<Rigidbody2D>().velocity.magnitude);
        }

        //if (Input.GetKey(KeyCode.B))
        //   transform.position += transform.up * 2 * Time.deltaTime;

    }
}
