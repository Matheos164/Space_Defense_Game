/*
    This class controlls the shield behiviour for the enemy boss


*/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyShieldBehaiviour : MonoBehaviour
{
    [Header("Object Settings")]
    private int hitCount = 0;
    public int hitsToDeactivate = 10;
    public float ReactivationTimer = 2f;
    public bool isActive = true;
    [Header("Object Dependency")]
    public GameObject shield;
    private Renderer renderer;
    private EdgeCollider2D edgeCollider2D;
    public delegate void ShieldStatus(bool isActive);
    public static event ShieldStatus ShieldChangeStatus;


    // The Start functiong grabs the render and edge colider component attached and sets the shield status to deactivated
    void Start(){
        renderer = GetComponent<Renderer>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();
        shield.SetActive(false);
    }

    // the Update checks for how many hits there are on the shield and calls the shield deactivation function
    void Update()
    {
        if(hitCount >= hitsToDeactivate){ // check if hitCount is equal or greater than the number to deactivate
            StartCoroutine(Deactivat());
        }
    }


    // The Deactivat function controlls deactivating and reactivating the shield
    IEnumerator Deactivat(){
        // check for the collider and deactivate the shield
        if(edgeCollider2D != null){ 
            edgeCollider2D.enabled = false;
            isActive = false;
            ShieldChangeStatus?.Invoke(isActive); // send what the status of the shield
        }

        float startTime = Time.time; // start the timer
        while(Time.time - startTime < ReactivationTimer){ // run a loop for the deactivated durashion that makes the render flash to indicate the shield is deactivated
            renderer.enabled = !renderer.enabled;
            yield return new WaitForSeconds(0.2f);
        }

        // Re-enable the shield after the loop is completed and reset the hit counter
        if(edgeCollider2D != null && renderer != null){
            edgeCollider2D.enabled = true;
            renderer.enabled = true;
            hitCount = 0;
            isActive = true;
            ShieldChangeStatus?.Invoke(isActive); // send what the status of the shield
        }

    }

    // OnCollisionEnter2D checks for collision with the shot object
    public void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Shot"){
            hitCount++; // increase the hit count if there was a colition
        }
    }
}
