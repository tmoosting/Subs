using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//[CreateAssetMenu(fileName = "New Suboat", menuName = "Uboat", order = 51)]
public class Uboat : Ship
{

    public GameObject torpedoTube;
    public GameObject gunSprite;
    public bool submerged = false;

    private bool roaming = false;
    private bool fleeing = false;
    private bool attacking = false;
    private bool assisting = false;
    private bool torpedoCooldownActive = false;
    private Merchant targetMerchant = null;
    private Destroyer seenDestroyer = null;
    private Ship targetShip = null;

    private System.Random r = null;

    private void Start()
    {
        r = new System.Random();

        targetLocation = transform.position;
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
            else if (fleeing)
            {
                if (targetShip != null)
                {
                    targetLocation = targetShip.transform.position;

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
                        targetShip = null;
                        targetLocation = transform.position;
                        fleeing = false;

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

                        // After fleeing the destroyer the uboat should wait for a new command.
                    }
                }
                else
                {
                    targetShip = null;
                    targetLocation = transform.position;
                    fleeing = false;

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

                    // After fleeing the destroyer the uboat should wait for a new command.
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
            else if (attacking)
            {
                if (targetShip != null)
                {
                    targetLocation = targetShip.transform.position;

                    // Check to see whether the merchant is within engagement distance.
                    float distanceToTarget = Vector3.Distance(transform.position, targetLocation);
                    if (distanceToTarget <= GameController.Instance.engagementDistance)
                    {
                        if (currentEngine != Engine.Flank)
                        {
                            currentEngine = Engine.Flank;
                        }

                        // Determine pseudo-avarage speed of the torpedo when fired
                        float initalScalarSpeedOfTorpedo = (GameController.Instance.torpedoInitialForce / GameController.Instance.torpedoPrefab.GetComponent<Rigidbody2D>().mass) * Time.fixedDeltaTime;
                        float timeToMaxScalarSpeedOfTorpedo = (GameController.Instance.torpedoMaxSpeed - initalScalarSpeedOfTorpedo) / GameController.Instance.torpedoAccelerationRate;
                        float scalarSpeedOfTorpedo = GameController.Instance.torpedoMaxSpeed / GameController.Instance.knotsPerMagnitude - initalScalarSpeedOfTorpedo * timeToMaxScalarSpeedOfTorpedo;

                        Vector3 merchantPosition = targetLocation;
                        Vector3 uboatPosition = transform.position;
                        Vector3 delta_pos = merchantPosition - uboatPosition;

                        Vector3 merchantVelocity = targetShip.GetComponent<Rigidbody2D>().velocity;

                        // Set up quadratic equation to solve the time taken to the target where target is the target boat and time take is the time traveled by the torpedo.
                        float delta_velocity_merchant_torpedo = Vector3.Dot(merchantVelocity, merchantVelocity) - (float)(Math.Pow(scalarSpeedOfTorpedo, 2));
                        float magnitude_position_delta = Vector3.Dot(delta_pos, delta_pos);
                        float magnitude_position_delta_velocity = 2 * Vector3.Dot(merchantVelocity, delta_pos);

                        // Solve quadratic equation where of traveltime to target.
                        float determinant = (float)(Math.Sqrt(Math.Pow(magnitude_position_delta_velocity, 2) - 4 * delta_velocity_merchant_torpedo * magnitude_position_delta));

                        if (determinant >= 0)
                        {
                            float timeToTarget = determinant == 0 ? ((-1 * magnitude_position_delta_velocity) / (2 * delta_velocity_merchant_torpedo)) :
                                ((-1 * magnitude_position_delta_velocity - determinant) / (2 * delta_velocity_merchant_torpedo));

                            // Simulate target location to shoot at by multiplying time with the
                            // speed vector of the merchant.
                            Vector3 shootingIntersection = merchantPosition + merchantVelocity * timeToTarget;

                            SetCourseToLocation(shootingIntersection);

                            // Fire only when the submarine's bearing is close to the target bearing.

                            //// Debug.Log($"{gameObject.name} The difference between target and current bearing is {Math.Abs(GetTargetBearing() - GetCurrentBearing())}");
                            if (!torpedoCooldownActive && Math.Abs(GetTargetBearing() - GetCurrentBearing()) <= 3)
                            {
                                FireTorpedo();
                            }
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
                else
                {
                    attacking = false;
                    assisting = false;
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

        int minMaxRand = (int)(GameController.Instance.maxRoamRange / 2);

        // Set target location to: x_target = x_current + rand[-1;1]*(rand[0, minMaxRand] + minMaxRand).
        targetLocation.x += (float)(((r.NextDouble() * minMaxRand) + minMaxRand) * (r.Next(0, 2) == 1 ? 1 : -1));
        // Set target location to: y_target = y_current + rand[-1;1]*(rand[0, minMaxRand] + minMaxRand).
        targetLocation.y += (float)(((r.NextDouble() * minMaxRand) + minMaxRand) * (r.Next(0, 2) == 1 ? 1 : -1));
    }

    public Merchant getTargetedMerchant()
    {
        return targetMerchant;
    }

    public Ship getTargetedShip()
    {
        return targetShip;
    }

    public float distanceToTargetShip()
    {
        if (targetShip == null)
            return float.MaxValue;
        return Vector3.Distance(targetShip.transform.position, transform.position);
    }

    public void releaseTargetMerchant()
    {
        targetMerchant = null;
    }

    public bool isFleeing()
    {
        return fleeing;
    }

    public bool isAttacking()
    {
        return attacking;
    }

    public bool isRoaming()
    {
        return roaming;
    }

    public bool isAssisting()
    {
        return assisting;
    }

    public SortedList<float, Ship> observeShips()
    {
        List<Ship> shipList = GameController.Instance.GetAllShips();

        SortedList<float, Ship> observationFromCurrentUboat = new SortedList<float, Ship>();

        Vector3 ownPosition = transform.position;

        // Look for all ships, except for other uboats, within a certain observation radius.
        foreach (Ship ship in shipList)
        {
            // Ignore ships of type uboat.
            if (ship.shipType == Ship.ShipType.UBOAT)
                continue;

            float distanceToShip = Vector3.Distance(ownPosition, ship.transform.position);

            // Only add ship to observation if it is within observation distance.
            if (distanceToShip <= GameController.Instance.maxSearchRange)
            {
                observationFromCurrentUboat.Add(distanceToShip, ship);
            }
        }

        return observationFromCurrentUboat;
    }

    public List<Uboat> findHelpingUboats(float ownDistance, Destroyer destroyer)
    {
        List<Uboat> assistList = new List<Uboat>();

        // Make a list of all uboats which are not the current uboat.
        var uboatList = GameController.Instance.GetUboats().Where(val => val != this).ToList();

        Debug.Log($"There are in total {uboatList.Count} other uboats.");

        foreach (Uboat uboat in uboatList)
        {
            float distanceToDestroyer = Vector3.Distance(uboat.transform.position, destroyer.transform.position);

            // Add all uboats to assist list who are not fleeing themselves, who are further away from the destroyer
            // than your own distance and who are within assist distance.
            if (!uboat.isFleeing() && distanceToDestroyer > ownDistance && distanceToDestroyer <= GameController.Instance.uboatAssistDistance)
                assistList.Add(uboat);
        }

        return assistList;
    }

    public void randomRoam()
    {
        fleeing = false;
        assisting = false;
        attacking = false;

        if (!roaming)
        {
            setRandomTargetLocation();

            // Activate roaming.
            roaming = true;

            return;
        }
        // Current random location target found, restart search to a different location.
        if (Vector3.Distance(transform.position, targetLocation) <= GameController.Instance.restartRoam)
        {
            setRandomTargetLocation();
        }
    }

    public void fleeFromShip(Ship _targetShip)
    {
        targetShip = _targetShip;
        attacking = false;
        assisting = false;
        roaming = false;
        fleeing = true;
    }

    public void attackShip(Ship _targetShip)
    {
        targetShip = _targetShip;
        attacking = true;
        assisting = false;
        roaming = false;
        fleeing = false;
    }

    public void assist(Ship _targetShip)
    {
        targetShip = _targetShip;
        attacking = true;
        assisting = true;
        roaming = false;
        fleeing = false;
    }

    public Ship getCurrentTargetedShip()
    {
        return targetShip;
    }

    public class UboatEqualityComparer : IEqualityComparer<Uboat>
    {
        #region IEqualityComparer<Uboat> Members

        public bool Equals(Uboat x, Uboat y)
        {
            return (x == y);
        }

        public int GetHashCode(Uboat obj)
        {
            return (obj.GetHashCode());
        }

        #endregion
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(System.Object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;

        return (this == (Uboat)obj);
    }
}
