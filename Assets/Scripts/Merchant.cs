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
}
