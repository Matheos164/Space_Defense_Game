using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [Header("Object Dependency")]
    public AudioMixer audioMixer;

    // controll all the game's sound fx
    public void SoundFXMixer(float level){ 
        audioMixer.SetFloat("SFXParm", Mathf.Log10(level)*20f);
    }
    

    // controll the game's music volume
    public void MusicMixer(float level){
        audioMixer.SetFloat("MusicParm", Mathf.Log10(level)*20f);
    }


}
