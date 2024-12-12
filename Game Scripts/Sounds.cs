using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    [Header("Object Dependency")]
    public static Sounds instance;
    public AudioSource AudioFX;
    public AudioClip Sound;

    private void Awake(){
        if(instance == null){ // assigne the current instance
            instance = this;
        }

        if (AudioFX == null){ // assigne the audio sourec component
            AudioFX = GetComponent<AudioSource>();
        }
    }

    // play an audio effect at a position
    public void PlayFX(AudioClip audioClip, Transform spawnTrans, float volume){
        AudioSource audioSource = Instantiate(AudioFX, spawnTrans.position, Quaternion.identity); // create an instance of the audio clip

        // assigne the audio clip and volume and play it
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength); // destroy the instance when the audio has reached the final length
    }


    // plays btn click sound
    public void ButtonClickFX(){
        if (AudioFX != null && Sound != null){
            AudioFX.PlayOneShot(Sound); // play the instance of the sound
        }
    }

}
