/*
    This Script controlls how the Shield behaivs

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBehaiviour : MonoBehaviour
{

    [Header("Object Dependency")]
    public GameObject shield;
    public KeyCode shieldActivate = KeyCode.Space;
    public KeyCode shieldActivate_Controller = KeyCode.JoystickButton2;
    public Animator ShieldAnimation;
    public Image[] ShieldBars;
    public AudioClip DeactivationSFX;
    [Header("Object Settings")]
    public float cooldown = 5f; 
    public float currentActiveFramesDuration = 0f;
    public float maxActivationDuration = 100f; 
    public bool isOnCooldown = false;
    public bool canActivate = true;


    void Start()
    {
        if(shield != null){ //set the shield to the deactivated state at the start of the game
            shield.SetActive(false);
        }
    }

    void Update()
    {
        // check for limitiations and activate shield when button pressed
        if(Input.GetKeyDown(shieldActivate) && canActivate && !isOnCooldown){
            StartCoroutine(ActivateShield());
        }else if(Input.GetKeyUp(shieldActivate)){ // deactivate when key released
            DeactivateShield();
        }
        
        // if shield is active, increase active duration
        if(shield.activeSelf && currentActiveFramesDuration < maxActivationDuration){
            currentActiveFramesDuration++;

        // if shield reaches max active duration, disable it and set it on a cooldown
        }else if(shield.activeSelf && currentActiveFramesDuration >= maxActivationDuration){
            DeactivateShield();
            StartCoroutine(coolDown());

        }else if(currentActiveFramesDuration != 0 && !isOnCooldown && !shield.activeSelf){
            currentActiveFramesDuration--;
            
        }

        UpdateBar(currentActiveFramesDuration);
    }


    // Activate the shield 
    IEnumerator ActivateShield(){
        canActivate = false;
        yield return null;

        if(!isOnCooldown){ // if not on cooldown activate the shield
            Sounds.instance.PlayFX(DeactivationSFX, transform, 0.5f);
            shield.SetActive(true);
            ShieldAnimation.SetBool("Activate", true);
        }
        canActivate = true;
    }

    // deactivate the shield
    void DeactivateShield(){
        shield.SetActive(false);
        ShieldAnimation.SetBool("Activate", false);

    }

    // cooldown for the shield
    IEnumerator coolDown(){
        // set the shield on cooldown mode
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown * Time.deltaTime);
        currentActiveFramesDuration = 0f;
        isOnCooldown = false;
    } 

    // update the shield bar gui based on the active duration of the shield 
    public void UpdateBar(float Duration){
        float durationPerBar = maxActivationDuration / ShieldBars.Length;

        for(int i = 0; i < ShieldBars.Length; i++){
            if(Duration > durationPerBar * i){
                ShieldBars[i].gameObject.SetActive(true);
            }else{
                ShieldBars[i].gameObject.SetActive(false);
            }
        }
    }
}
