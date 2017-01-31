using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHelper : MonoBehaviour
{
    public AudioSource source;
    public float delayAudioTime;
    private float currentAudioTime;

    void Awake()
    {
        currentAudioTime = delayAudioTime;
    }

    void Update()
    {
        if(currentAudioTime > 0)
        {
            currentAudioTime -= Time.deltaTime;
        }
        else
        {
            source.Play();
        }
    }
}
