using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observation 
{
   
    public enum Type {SONAR, LOOKOUT, COMMS  }

    public bool ongoing;
    public Type type;
    public Ship observerShip;
    public Ship observedShip;
     

    public float firstSpottedTime;
    public float lastSpottedTime; 
    public Dictionary<Vector3, float> positionLog = new Dictionary<Vector3, float>(); // holds positions and time at which it was recorded


    public Observation(Ship observer, Ship observed, Type givenType  )
    { 
        type = givenType;
        observerShip = observer;
        observedShip = observed;
        firstSpottedTime = Time.time; 
        lastSpottedTime = Time.time; 
        positionLog.Add(observedShip.transform.position, Time.time);
        ongoing = true;
    }

    public void RegisterObservation()
    {
        if (positionLog.ContainsKey(observedShip.transform.position) == false)
        positionLog.Add(observedShip.transform.position, Time.time);
    }

    public void FinishObservation()
    { 
       if (type != Type.COMMS)
                ongoing = false; 
         
    }

}
