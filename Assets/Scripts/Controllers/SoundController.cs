using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    AudioSource audioSource;
    public AudioClip torpedoFire;
    public AudioClip torpedoHit;
    public AudioClip sonarPing;
    public AudioClip chargeSplash;
    public AudioClip chargeSink;
    public AudioClip cannonFire;
    public AudioClip bubble;
    
    

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>(); 
    }

    public void PlayTorpedoFire()
    {
        audioSource.PlayOneShot(torpedoFire);
    }
    public void PlayTorpedoHit()
    {
        audioSource.PlayOneShot(torpedoHit);
    } 
    public void PlaySonarPing()
    { 
        if (TrainingController.Instance.enableTrainingMode == false)
        audioSource.PlayOneShot(sonarPing);
    }
    public void PlayChargeSplash()
    {
        audioSource.PlayOneShot(chargeSplash);
    }
    public void PlayChargeSink()
    {
        audioSource.PlayOneShot(chargeSink);
    }
    public void PlayBubble()
    {
        audioSource.PlayOneShot(bubble);
    }
}
