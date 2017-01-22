using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioObserver : MonoBehaviour
{
    public AudioSource[] musicTracks;
    public int currentTrack;

    void Update()
    {
        if(WaveStatusController.instance)
        {
            transform.position = WaveStatusController.instance.transform.position;
        }
    }

    int lastTrack = -1;
    void UpdateCurrentTrack()
    {
        if(lastTrack != currentTrack)
        {
            if(lastTrack != -1)
            {
                musicTracks[lastTrack].DOFade(0, 1f);
            }
            if(currentTrack > musicTracks.Length)
            {
                musicTracks[currentTrack].DOFade(1, 1f);
            }
            lastTrack = currentTrack;
        }
    }

}
