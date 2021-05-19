using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shifter : MonoBehaviour
{
     private int frames = 0;
    // Update is called once per frame
    void Update()
    {
        frames++;
        if (frames % 100 == 0) {
            int xshift = Random.Range(-2,2);
            int yshift = Random.Range(-2,2);
            transform.position = transform.position + new Vector3(xshift, yshift, 0);
        }
    }
}
