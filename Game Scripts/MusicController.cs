using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Header("Object Dependency")]
    public AudioSource AudioSource;
    public AudioClip[] Music;
    public bool canPlay = true;

    void Start() { 
        AudioSource = GetComponent<AudioSource>(); // get the audio source component

        if (Music.Length > 0) { 
                PlayRandomTrack(); // play random track
            } 
    }

    void Update(){
        // check if track has completed and if the game is still going 
        if(!AudioSource.isPlaying && canPlay){
            PlayRandomTrack(); // play new track
        }
    }

    // play random track form array 
    void PlayRandomTrack(){
        int rand = Random.Range(0, Music.Length); // select random number based on array length
        // assigne the clip and play it
        AudioSource.clip = Music[rand];
        AudioSource.Play();
    }
}
