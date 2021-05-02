using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "New Suboat", menuName = "Uboat", order = 51)]
public class Uboat : Ship
{

    public GameObject torpedoTube;
    public bool submerged = false;


    public void FireTorpedo()
    {
        GameObject obj = Instantiate(GameController.Instance.torpedoPrefab);
        obj.transform.position = torpedoTube.transform.position;
        obj.transform.rotation = transform.rotation;

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * GameController.Instance.torpedoInitialForce);
        SoundController.Instance.PlayTorpedoFireSound();

    }

}
