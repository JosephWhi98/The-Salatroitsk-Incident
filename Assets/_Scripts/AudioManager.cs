using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    public AudioMixer mixer;
    public AudioMixerSnapshot open;
    public AudioMixerSnapshot closed;

    public void Start()
    {
        closed.TransitionTo(0f);
        //DontDestroyOnLoad(this.gameObject);
        SnapAudioOpen();
    }

    public void SnapAudioOpen()
    {
        open.TransitionTo(1f);
    }

    public void SnapAudioClose()
    {
        closed.TransitionTo(1f);
    }
}
