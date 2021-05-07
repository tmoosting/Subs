using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "New Suboat", menuName = "Uboat", order = 51)]
public class Uboat : Ship
{

    public GameObject torpedoTube;
    public GameObject gunSprite;
    public bool submerged = false;

    private bool roaming = false;
    private Merchant targetMerchant = null;
    

    private void Start()
    {
        
        targetLocation = transform.position;

        InvokeRepeating("SearchShips", 0.1f, 5.0f);
    }

    private void Update()
    {
        if (targetMerchant != null)
        {
            SetCourseToLocation(targetLocation);
            targetLocation = targetMerchant.transform.position;
        }
        else if (movingToTarget)
        {
            SetCourseToLocation(targetLocation);
        }
        else if (roaming)
        {
            SetCourseToLocation(targetLocation);
        }
    }



    public void FireTorpedo()
    {
        GameObject obj = Instantiate(GameController.Instance.torpedoPrefab);
        obj.transform.position = torpedoTube.transform.position;
        obj.transform.rotation = transform.rotation;

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * GameController.Instance.torpedoInitialForce);
        SoundController.Instance.PlayTorpedoFireSound();

    }

    public void ToggleSubmerge()
    {
        if (submerged == true)
            Resurface();
        else
            Submerge();
    }
    public void Submerge()
    {
        submerged = true;
        Color tempColor = gunSprite.GetComponent<SpriteRenderer>().color;
        tempColor.a = 0.4f;
        gunSprite.GetComponent<SpriteRenderer>().color = tempColor;
        GetComponent<SpriteRenderer>().color = tempColor;
    }

    public void Resurface()
    {
        submerged = false;
        Color tempColor = gunSprite.GetComponent<SpriteRenderer>().color;
        tempColor.a = 1f;
        gunSprite.GetComponent<SpriteRenderer>().color = tempColor;
        GetComponent<SpriteRenderer>().color = tempColor; 

    }

    private void setRandomTargetLocation()
    {
        System.Random r = new System.Random();

        targetLocation = transform.position;

        int minMaxRand = (int)(GameController.Instance.maxSearchRange / 2);

        // Set target location to: x_target = x_current + rand[-minMaxRand, minMaxRand].
        targetLocation.x += (float)(r.NextDouble() * (r.Next(0, 1) == 1 ? 1 : -1) * minMaxRand);
        targetLocation.y += (float)(r.NextDouble() * (r.Next(0, 1) == 1 ? 1 : -1) * minMaxRand);
    }

    private void SearchShips()
    {
        if (movingToTarget == false)
        {
            // Check whether it is already chasing a merchant.
            // Only when there is no merchant in sight or when the current merchant is too far away
            // should it search for a new merchant.
            if (targetMerchant == null || Vector3.Distance(transform.position, targetMerchant.transform.position) > GameController.Instance.maxSearchRange)
            {
                float bestDistance = float.MaxValue;

                // Fetch list of merchants from GameController
                List<Merchant> merchantList = GameController.Instance.GetMerchants();

                // For each merchant check to see whether it is in range of the radar range (maxSearchRange)
                foreach (Merchant merchant in merchantList)
                {
                    float distanceBetweenMerchantAndUBoat = Vector3.Distance(transform.position, merchant.transform.position);

                    // If current merchant is in radar range and closer than the current best merchant, set new target.
                    if (distanceBetweenMerchantAndUBoat <= GameController.Instance.maxSearchRange && distanceBetweenMerchantAndUBoat < bestDistance)
                    {
                        bestDistance = distanceBetweenMerchantAndUBoat;
                        targetMerchant = merchant;
                    }
                }
            }

            // No target has been found and uboat is not roaming: start roaming.
            if (targetMerchant == null && !roaming)
            {
                setRandomTargetLocation();

                // Activate roaming.
                roaming = true;
            }
            else if (targetMerchant == null && roaming)
            {
                // Current random location target found, restart search to a different location.
                if (Vector3.Distance(transform.position, targetLocation) <= GameController.Instance.restartSearch)
                {
                    setRandomTargetLocation();
                }
            }
            else if (targetMerchant != null)
            {
                // Set target to location of merchant.
                targetLocation = targetMerchant.transform.position;
                // Deactivate roaming.
                roaming = false;
            }
        } 
    }


   

}
