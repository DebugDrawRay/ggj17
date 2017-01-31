using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioObserver : MonoBehaviour
{
    public AudioSource[] musicTracks;
    public AudioSource endCueSource;
    public AudioClip loseClip;
    public AudioClip winClip;

    private int currentTrack;

    public static AudioObserver instance;
    void Awake()
    {
        instance = this;
    }

	void Start()
	{
		musicTracks[currentTrack].DOFade(1, 2f);
	}

    void Update()
    {
        if(WaveStatusController.instance)
        {
            transform.position = WaveStatusController.instance.transform.position;
        }
        UpdateCurrentTrack();
    }

    int lastTrack = -1;
    void UpdateCurrentTrack()
    {
        if(lastTrack != currentTrack)
        {
            if(currentTrack < musicTracks.Length)
            {
				if (lastTrack != -1)
				{
					musicTracks[lastTrack].DOFade(0, 1f);
				}
				musicTracks[currentTrack].DOFade(1, 1f);
				lastTrack = currentTrack;
			}
        }
    }

    public void ChangeTrack(int index)
    {
        currentTrack = index;
    }

    public void TriggerEndGame(bool win)
    {
        musicTracks[currentTrack].DOFade(0, .5f);
        if (win)
        {
            endCueSource.clip = winClip;
        }
        else
        {
            endCueSource.clip = loseClip;
        }
        endCueSource.Play();
    }
}
