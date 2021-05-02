using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    AudioSource audioSource;
    public AudioClip torpedoFire;
    public AudioClip torpedoHit;
    
    

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>(); 
    }

    public void PlayTorpedoFireSound()
    {
        audioSource.PlayOneShot(torpedoFire);
    }
    public void PlayTorpedoHitSound()
    {
        audioSource.PlayOneShot(torpedoHit);
    }
    
}
