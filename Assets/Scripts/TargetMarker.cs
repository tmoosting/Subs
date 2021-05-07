using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ship>() != null)
        {
            if (collision.gameObject.GetComponent<Ship>() == GameController.Instance.selectedShip)
            {
                collision.gameObject.GetComponent<Ship>().ReachTargetMarker();
                gameObject.SetActive(false);
            }
           
        }
    }
}
