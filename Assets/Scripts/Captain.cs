using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captain : MonoBehaviour
{


    Vector3 startPos;

    [Header("Brain")]
    // for every potential ship, captain has a List<Observation>. Each observation lasts from first to last sighting.
    // so that when a ship goes out of view, and later is sighted again, a new observation is made
    public List<Observation> ongoingObservations = new List<Observation>(); // holds current 'ongoing' observations, refreshed each update
    Dictionary<Ship, List<Observation>> shipObservations = new Dictionary<Ship, List<Observation>>();
    List<Ship> spottedSonarsThisCycle = new List<Ship>();
    List<Ship> spottedLookoutsThisCycle = new List<Ship>();







    private void Awake()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        // refresh  ongoingObservations list
        ongoingObservations = new List<Observation>();
        foreach (Ship ship in shipObservations.Keys)
            // only check latest index for each ship's obs list because that is the newest observation
            if (shipObservations[ship][shipObservations[ship].Count - 1].ongoing == true)
                ongoingObservations.Add(shipObservations[ship][shipObservations[ship].Count - 1]); 
    }




    public void DetectLookout(Ship ship)
    {
        Debug.Log("LOOKOUT: " + ship.name);
        spottedLookoutsThisCycle.Add(ship);

        // check that it's not already being observed
        bool shipCurrentlyBeingObserved = false;
        foreach (Observation obs in ongoingObservations)
            if (obs.observedShip == ship)
                shipCurrentlyBeingObserved = true;

        if (shipCurrentlyBeingObserved == false)
        {
            // make a new observation, passing the observant ship and the observed ship 
            Observation obs = new Observation(GetComponent<Ship>(), ship, Observation.Type.LOOKOUT);

            // check if it already has an Obs list for the ship, then add the observation
            if (shipObservations.ContainsKey(ship) == false)
                shipObservations.Add(ship, new List<Observation>());
            shipObservations[ship].Add(obs);
        }
        else
        {
            // Log the observation in the ongoing Observation's log list
            foreach (Observation obs in ongoingObservations)
                if (obs.observedShip == ship)
                    obs.RegisterObservation();

        }
    }
    public void DetectSonar(GameObject obj)
    {
 

        if (obj.GetComponent<Pillenwerfer>() != null)
        {
            // it's a decoy!
        }
        else
        {
            Ship ship = obj.GetComponent<Ship>();
            //    Debug.Log("catching " + ship.name + " on sonar");
            spottedSonarsThisCycle.Add(ship);


            // check that it's not already being observed
            bool shipCurrentlyBeingObserved = false;
            foreach (Observation obs in ongoingObservations)
                if (obs.observedShip == ship)
                    shipCurrentlyBeingObserved = true;

            if (shipCurrentlyBeingObserved == false)
            {
                // make a new observation, passing the observant ship and the observed ship 
                Observation obs = new Observation(GetComponent<Ship>(), ship, Observation.Type.SONAR);

                // check if it already has an Obs list for the ship, then add the observation
                if (shipObservations.ContainsKey(ship) == false)
                    shipObservations.Add(ship, new List<Observation>());
                shipObservations[ship].Add(obs);
            }
            else
            {
                // Log the observation in the ongoing Observation's log list
                foreach (Observation obs in ongoingObservations)
                    if (obs.observedShip == ship)
                        obs.RegisterObservation();

            }
        }
         

    }
    public void SightATorpedo(Torpedo torpedo)
    {
        // panic!!
    }
    public void LookoutCycle()
    {
        // called after each Lookout cycle, to check for ongoing-observation-ships that were not observed this round
        foreach (Observation obs in ongoingObservations)
        {
            if (obs.ongoing == true)
                if (obs.type == Observation.Type.LOOKOUT)
                    if (spottedLookoutsThisCycle.Contains(obs.observedShip) == false)
                        obs.FinishObservation();
        }
        // clear the cycle list 
        spottedLookoutsThisCycle = new List<Ship>();
    }
    public void SonarCycle()
    {
        // called after each Sonar cycle, to check for ongoing-observation-ships that were not observed this round
        foreach (Observation obs in ongoingObservations)
        {
            if (obs.ongoing == true)
                if (obs.type == Observation.Type.SONAR)
                    if (spottedSonarsThisCycle.Contains(obs.observedShip) == false)
                        obs.FinishObservation();
        }
        // clear the cycle list 
        spottedSonarsThisCycle = new List<Ship>();
    }
   
    public void ResetCaptain()
    {
        // for training purposes
        ongoingObservations = new List<Observation>();
        shipObservations = new Dictionary<Ship, List<Observation>>();
        spottedSonarsThisCycle = new List<Ship>();
        spottedLookoutsThisCycle = new List<Ship>();
    }
}
