using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class MixerControl : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioMixerSnapshot[] snapshots;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log(volume) * 20);
        if (volume == 0)
            mixer.SetFloat("MasterVolume", -80);
    }

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log(volume) * 20);
        if (volume == 0)
            mixer.SetFloat("MusicVolume", -80);
    }

    public void SetGameSoundsVolume(float volume)
    {
        mixer.SetFloat("GameSoundsVolume", Mathf.Log(volume) * 20);
        if (volume == 0)
            mixer.SetFloat("GameSoundsVolume", -80);
    }

    public void SwitchToReverb()
    {
        snapshots[1].TransitionTo(0.0f);
    }

    public void SwitchToNoReverb()
    {
        snapshots[0].TransitionTo(0.0f);
    }
}
