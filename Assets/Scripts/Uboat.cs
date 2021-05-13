using System;
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
    private bool torpedoCooldownActive = false;
    private Merchant targetMerchant = null;
    private Destroyer seenDestroyer = null;

    private System.Random r = null;

    private void Start()
    {
        r = new System.Random();

        targetLocation = transform.position;

        InvokeRepeating("SearchShips", 0.1f, 1.0f);
    }

    private void Update()
    {
        if (TrainingController.Instance.enableTrainingMode == false)
        {
            // Manually set target to go to.
            if (movingToTarget)
            {
                // Get back to the surface.
                if (submerged)
                {
                    Resurface();
                }

                if (currentEngine != Engine.Standard)
                {
                    currentEngine = Engine.Standard;
                }

                SetCourseToLocation(targetLocation);
            }
            // Fleeing from destroyer seen in SearchShips;
            else if (seenDestroyer != null)
            {
                targetLocation = seenDestroyer.transform.position;

                // Determine whether the u-boat should still flee the scene.
                if (Vector3.Distance(transform.position, targetLocation) <= GameController.Instance.uboatFleeDistance * 1.25f)
                {
                    // Set engine into overdrive, get out of there fast!
                    if (currentEngine != Engine.Flank)
                    {
                        currentEngine = Engine.Flank;
                    }

                    // Submerge to become harder to notice.
                    if (!submerged)
                    {
                        Submerge();
                    }

                    // Set course away from destroyers (180 deg away)
                    SetCourse((int)Math.Round((obtainLocationBearing(targetLocation) + 180) % 360));
                }
                // U-boat is far enough away from the destroyers, it is now safe to resurface and to start looking around again.
                else
                {
                    seenDestroyer = null;
                    targetLocation = transform.position;

                    // Get back to the surface.
                    if (submerged)
                    {
                        Resurface();
                    }

                    // Temporarily disable engine until next move is determined.
                    if (currentEngine != Engine.Still)
                    {
                        currentEngine = Engine.Still;
                    }

                    // Everything has been reset, including merchants, u-boat needs to start searching again.
                    SearchShips();
                }
            }
            // No ships found, roam around randomly.
            else if (roaming)
            {
                // Get back to the surface.
                if (submerged)
                {
                    Resurface();
                }

                if (currentEngine != Engine.Standard)
                {
                    currentEngine = Engine.Standard;
                }
                SetCourseToLocation(targetLocation);
            }
            // Merchant found in SearchShips.
            else if (targetMerchant != null)
            {
                targetLocation = targetMerchant.transform.position;

                // Check to see whether the merchant is within engagement distance.
                float distanceToTarget = Vector3.Distance(transform.position, targetLocation);
                if (distanceToTarget <= GameController.Instance.engagementDistance)
                {
                    if (currentEngine != Engine.Flank)
                    {
                        currentEngine = Engine.Flank;
                    }

                    // Submerge to become harder to notice.
                    if (!submerged && distanceToTarget <= GameController.Instance.engagementDistance * 0.5f)
                    {
                        Submerge();
                    }

                    // TODO: Maybe add better ballistics trigonometry.

                    // Instead of shooting directly at the merchant ship, the uboat will
                    // try to shoot in front of the merchant ship in order to intersect
                    // the path of the torpedo with the path of the merchant.

                    // Find time of torpedo to target location.
                    float timeToTarget = distanceToTarget / (GameController.Instance.torpedoMaxSpeed * 0.05f);

                    // Simulate target location to shoot at by multiplying time with the
                    // speed vector of the merchant.
                    Vector3 shootingIntersection = targetLocation;
                    shootingIntersection.x += (targetMerchant.GetComponent<Rigidbody2D>().velocity.x) * timeToTarget;
                    shootingIntersection.y += (targetMerchant.GetComponent<Rigidbody2D>().velocity.y) * timeToTarget;

                    SetCourseToLocation(shootingIntersection);

                    // Fire only when the submarine's bearing is close to the target bearing.

                    //// Debug.Log($"{gameObject.name} The difference between target and current bearing is {Math.Abs(GetTargetBearing() - GetCurrentBearing())}");
                    if (!torpedoCooldownActive && Math.Abs(GetTargetBearing() - GetCurrentBearing()) <= 3)
                    {
                        FireTorpedo();
                    }
                }
                else
                {
                    // Get back to the surface.
                    if (submerged)
                    {
                        Resurface();
                    }

                    if (currentEngine != Engine.Standard)
                    {
                        currentEngine = Engine.Standard;
                    }
                    SetCourseToLocation(targetLocation);
                }
            }
            // Default 'no action'.
            else
            {
                if (currentEngine != Engine.Still)
                {
                    currentEngine = Engine.Still;
                }
            }
        }
       
    }

    public void FireTorpedo()
    {
        // Enable cooldown on torpedo.
        torpedoCooldownActive = true;
        StartCoroutine(TorpedoCooldownReset());

        // Create new torpedo object.
        GameObject obj = Instantiate(GameController.Instance.torpedoPrefab);

        // Set position and rotation.
        obj.transform.position = torpedoTube.transform.position;
        obj.transform.rotation = transform.rotation;

        // Add force to torpedo.
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * GameController.Instance.torpedoInitialForce);

        SoundController.Instance.PlayTorpedoFire();
    }

    IEnumerator TorpedoCooldownReset()
    {
        // Wait for pre-defined seconds.
        yield return new WaitForSeconds(GameController.Instance.torpedoCooldown);
        torpedoCooldownActive = false;
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
        gameObject.layer = LayerMask.NameToLayer("underwater"); 

        // Change color of submerged uboat.
        Color tempColor = gunSprite.GetComponent<SpriteRenderer>().color;
        tempColor.a = 0.4f;
        gunSprite.GetComponent<SpriteRenderer>().color = tempColor;
        GetComponent<SpriteRenderer>().color = tempColor;
    }

    public void Resurface()
    {
        submerged = false;
        gameObject.layer = LayerMask.NameToLayer("uboat");
        // Change color of surfaced uboat.
        Color tempColor = gunSprite.GetComponent<SpriteRenderer>().color;
        tempColor.a = 1f;
        gunSprite.GetComponent<SpriteRenderer>().color = tempColor;
        GetComponent<SpriteRenderer>().color = tempColor; 

    }

    private void setRandomTargetLocation()
    {
        targetLocation = transform.position;

        int minMaxRand = (int)(GameController.Instance.maxSearchRange / 2);

        // Set target location to: x_target = x_current + rand[-1;1]*(rand[0, minMaxRand] + minMaxRand).
        targetLocation.x += (float)(((r.NextDouble() * minMaxRand) + minMaxRand) * (r.Next(0, 2) == 1 ? 1 : -1));
        // Set target location to: y_target = y_current + rand[-1;1]*(rand[0, minMaxRand] + minMaxRand).
        targetLocation.y += (float)(((r.NextDouble() * minMaxRand) + minMaxRand) * (r.Next(0, 2) == 1 ? 1 : -1));
    }

    private void SearchShips()
    {
        if (!movingToTarget)
        {
            // First check whether uboats are near destroyers by looking at all available destroyers and determining whether they are within fleeing distance.

            // Fetch list of destroyers from GameController
            List<Destroyer> destroyerList = GameController.Instance.GetDestroyers();

            // For each destroyer check to see whether it is in fleeing range (uboatFleeDistance).
            foreach (Destroyer destroyer in destroyerList)
            {
                float distanceBetweenDestroyerAndUBoat = Vector3.Distance(transform.position, destroyer.transform.position);

                // If a destroyer is near, flee away from it.
                if (distanceBetweenDestroyerAndUBoat <= GameController.Instance.uboatFleeDistance)
                {
                    targetMerchant = null;
                    roaming = false;

                    seenDestroyer = destroyer;
                    return;
                }
            }

            // Check whether it is already chasing a merchant.
            // Only when there is no merchant in sight or when the current merchant is too far away
            // should it search for a new merchant.
            if (targetMerchant == null || Vector3.Distance(transform.position, targetMerchant.transform.position) > GameController.Instance.maxSearchRange * 0.10f)
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
                // Deactivate roaming.
                roaming = false;
            }
        } 
    }


    public void ThrowPillenwerfer()
    {
        SoundController.Instance.PlayBubble();
        GameObject obj = Instantiate(GameController.Instance.pillenwerferPrefab);
        obj.transform.position = transform.position;
        StartCoroutine(DestroyAfterDelay(GameController.Instance.pillenwerferDuration, obj));
    }
   

    IEnumerator DestroyAfterDelay(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }

}
