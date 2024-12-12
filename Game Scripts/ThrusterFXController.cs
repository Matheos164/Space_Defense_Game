using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterFXController : MonoBehaviour
{
    [Header("Object Dependency")]
    public AudioSource AudioSource;
    public AudioClip SoundFx;

    //assigne the audi clip and set the volume
    void Start() { 
        AudioSource = GetComponent<AudioSource>(); 
        AudioSource.clip = SoundFx;
        AudioSource.loop = true;
        AudioSource.volume = 0.5f;
    }

    // check if the controlls btn is clicked to play the audio clip and stop when its not clicked
    void Update(){
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)){
            AudioSource.Play();
        }else if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)){
            AudioSource.Stop();
        }
    }
}
