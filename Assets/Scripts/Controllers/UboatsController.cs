using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UboatsController : MonoBehaviour
{
    public static UboatsController Instance;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("setUboatActions", 0.1f, 1.0f);
    }

    /*// Update is called once per frame
    void Update()
    {
        
    }*/

    private bool otherUboatsInBetterPos(KeyValuePair<float, Ship> kvp_ship, Dictionary<Uboat, SortedList<float, Ship>> uboatObservations)
    {
        // For each uboat in the uboatObservationDictionary, check whether this uboat has a better distance (shorter) than the input uboat.
        foreach (KeyValuePair<Uboat, SortedList<float, Ship>> kvp_2 in uboatObservations)
        {
            // Look at each observation made by this uboat.
            foreach (KeyValuePair<float, Ship> kvp_ship_2 in kvp_2.Value)
            {
                // Check whether observation of this uboat is also in sight of input uboat. If this is the case, check to see who has the better
                // or rather shorter distance.
                if (kvp_ship.Value == kvp_ship_2.Value && kvp_ship.Key > kvp_ship_2.Key)
                {
                    return true;
                }
            }
        }

        // Non of the other observations are better.
        return false;
    }

    private void setUboatActions()
    {
        if (TrainingController.Instance.enableTrainingMode == false)
        { 
            List<Uboat> uboatList = GameController.Instance.GetUboats();

            Dictionary<Uboat, SortedList<float, Ship>> uboatObservations = new Dictionary<Uboat, SortedList<float, Ship>>(new Uboat.UboatEqualityComparer());

            // For each available uboat, start a wide observation of all ships except uboats.
            foreach (Uboat uboat in uboatList)
            {
                // Add all observation to dictionary entry.
                uboatObservations.Add(uboat, uboat.observeShips());
            }

            // Find all uboats who have made no observations (other ships too far away to observe).
            var noObservation = uboatObservations.Where(val => val.Value.Count == 0).ToList();

            // For each uboat which has no observations, start random roam and remove from dictionary (these are finished).
            foreach (KeyValuePair<Uboat, SortedList<float, Ship>> kvp in noObservation)
            {
                kvp.Key.randomRoam();
                uboatObservations.Remove(kvp.Key);
            }

            // For each uboat look at all observations.
            foreach (Uboat uboat_key in uboatObservations.Keys.ToList())
            {
                List<float> removeObservations = new List<float>();

                float bestDistance = float.MaxValue;
                Ship targetShip = null;

                // Go through all observations of current uboat.
                foreach (KeyValuePair<float, Ship> kvp_ship in uboatObservations[uboat_key])
                {
                    // If uboat observed destroyer, start fleeing.
                    if (kvp_ship.Value.shipType == Ship.ShipType.DESTROYER && !uboat_key.isAssisting())
                    {
                        if (kvp_ship.Key <= GameController.Instance.uboatFleeDistance)
                        {
                            uboat_key.fleeFromShip(kvp_ship.Value);

                            // Gather list of uboats who can help.
                            List<Uboat> assistList = uboat_key.findHelpingUboats(kvp_ship.Key, (Destroyer)kvp_ship.Value);

                            foreach (Uboat uboat in assistList)
                            {
                                uboat.assist(kvp_ship.Value);
                            }
                        }
                    }
                    // If uboat observed merchant, consider attacking.
                    else if (kvp_ship.Value.shipType == Ship.ShipType.MERCHANT && uboat_key.distanceToTargetShip() > GameController.Instance.restartSearchRange && !uboat_key.isFleeing() && !uboat_key.isAssisting())
                    {
                        // Check whether other uboats are in a better position. Pass a subset of the current dictionary where the current uboat is removed from the dictionary.
                        // If current observation is better in another uboat, remove it from the observations of current uboat.
                        if (otherUboatsInBetterPos(kvp_ship, uboatObservations.Where(val => val.Key != uboat_key).ToDictionary(x => x.Key, x => x.Value)))
                            removeObservations.Add(kvp_ship.Key);
                        // This uboat has the best observation. Find the merchant which is closest (kind of redundant because this is a sorted list, but oh well).
                        else if (kvp_ship.Key < bestDistance)
                        {
                            bestDistance = kvp_ship.Key;
                            targetShip = kvp_ship.Value;
                        }
                    }
                }

                // If not fleeing and a target has been found, attack.
                if (!uboat_key.isFleeing() && targetShip != null)
                    uboat_key.attackShip(targetShip);
                else if (!uboat_key.isFleeing() && !uboat_key.isAssisting() && !uboat_key.isAttacking())
                    uboat_key.randomRoam();

                // Remove all observations of current uboat which were better for another uboats.
                uboatObservations[uboat_key] = new SortedList<float, Ship>(uboatObservations[uboat_key].Where(val => !removeObservations.Contains(val.Key)).ToDictionary(x => x.Key, x => x.Value));
            }
        }
    }
}
