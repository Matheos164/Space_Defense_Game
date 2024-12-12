using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBoxBehaiviour : MonoBehaviour
{
    [Header("Game Objects Dependencies")]
    private Planet_Scrt planet;
    private SpriteRenderer sr;
    private BoxCollider2D ec;
    public AudioClip CollectionFX; 

    void Start(){
        // get the planet script and get the components
        planet = FindObjectOfType<Planet_Scrt>();
        sr = this.GetComponent<SpriteRenderer>();
        ec = this.GetComponent<BoxCollider2D>();
    }

    // collisiton check to see if the power up has been collected
    public void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Shot"){ //check if the colition was with a 'shot' object 
            StartCoroutine(GetHealth());
            Sounds.instance.PlayFX(CollectionFX, transform, 0.5f); // play sound
        }
    }

    // Get health power up
    IEnumerator GetHealth(){
        planet.health++; // increase planet health
        if(planet.health > 7){ // check if planet health is above 7 
            sr.enabled = false;
            ec.enabled = false;
            yield return new WaitForSeconds(12); // give temp health point for 12s if health is above 7
            planet.health--;
        }
        Destroy(gameObject);
    }
}
