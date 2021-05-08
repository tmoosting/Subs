using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingController : MonoBehaviour
{
    public static TrainingController Instance;

    [Header("General Settings")]
    public bool enableTrainingMode;

    private void Awake()
    {
        Instance = this;
    }

}
