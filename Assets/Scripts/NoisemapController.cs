using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisemapController : MonoBehaviour
{
    public static NoisemapController Instance;

    void Awake()
    {
        Instance = this;
    }

    
}
