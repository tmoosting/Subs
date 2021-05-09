using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observation 
{
   
    public enum Type {ALLY, UBOAT, PERISCOPE, TORPEDO}

    Type type;
    Time firstSpottedTime;
    Time lastSpottedTime;
    Vector3 firstSpottedLocation;
    Vector3 lastSpottedLocation;


    Observation(Time timeStamp)
    {

    }
    

}
