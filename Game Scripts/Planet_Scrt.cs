/*
    This class controlls the planet object which is part of the player objective to protect

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Planet_Scrt : MonoBehaviour
{
    [Header("Object Settings")]
    public float rotationSpeed = 50f;
    public int health = 3;
    public GameObject planet;
    public AudioClip hitSound;

    [Header("Animation Settings")]
    public Animator Explotion;
    public Image[] HealthBars;


    // the update function controlls the rotation of the plannet
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed*Time.deltaTime);
        UpdateHealthBar(health);
    }

    // check for colitions with enemy or enemy shot tags 
    public void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "EnemyShot" || collision.gameObject.tag == "Enemy"){
            Sounds.instance.PlayFX(hitSound, transform, 0.5f); // play sfx
            // check the planet health
            if(health <= 0){
                Explotion.SetBool("boom", true);
                StartCoroutine(ExplotionAnimation());
                Destroy(planet);
            }else{
                health = health - 1;
            }
        }
            
    }

    // runs the explotion animation and destroys the object when enemation is complete
    IEnumerator ExplotionAnimation(){
        yield return new WaitForSeconds(Explotion.GetCurrentAnimatorStateInfo(0).length);
    }

    // update the health bar in the game gui
    public void UpdateHealthBar(int HealthPoints){
        //set the number of bars based on player HP
        for(int i = 0; i < HealthBars.Length; i++){
            if(HealthPoints > i){
                HealthBars[i].gameObject.SetActive(true);
            }else{
                HealthBars[i].gameObject.SetActive(false);
            }
        }
    }


}

