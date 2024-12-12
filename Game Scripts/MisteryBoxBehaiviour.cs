using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class MisteryBoxBehaiviour : MonoBehaviour
{
    [Header("Game Objects Dependencies")]
    private Player_Scrt player;
    private Planet_Scrt planet;
    private ShieldBehaiviour shield;
    private SpriteRenderer sr;
    private BoxCollider2D ec;
    public AudioClip CollectionFX;


    void Start(){
        // get the planet, shield, and player script and get the components
        player = FindObjectOfType<Player_Scrt>();
        shield = FindObjectOfType<ShieldBehaiviour>();
        planet = FindAnyObjectByType<Planet_Scrt>();
        sr = this.GetComponent<SpriteRenderer>();
        ec = this.GetComponent<BoxCollider2D>();
    }

    // collisiton check to see if the power up has been collected
    public void OnCollisionEnter2D(Collision2D collision) {
        int rand = Random.Range(0,6);
        if(collision.gameObject.tag == "Shot"){ //check if the colition was with a 'shot' object 
            // based on random number specific power up/power down will get activated
            switch (rand){
                case 0:
                StartCoroutine(InvertedMovement());
                    break;
                case 1:
                StartCoroutine(ShipStall());
                    break;
                case 2:
                    StartCoroutine(GetHealth());
                    break;
                case 3:
                    StartCoroutine(TrippleTrouble());
                    break;
                case 4:
                    StartCoroutine(TempBoost());
                    break;
                case 5:
                    StartCoroutine(ShieldStall());
                    break;
            }
            Sounds.instance.PlayFX(CollectionFX, transform, 0.5f); // play sound
        }
    }

    // Power Down - stall the ship for 5 seconds
    IEnumerator ShipStall(){
        player.canShoot = false;
        player.canMove = false;
        sr.enabled = false;
        ec.enabled = false;
        yield return new WaitForSeconds(5);
        player.canShoot = true;
        player.canMove = true;
        Destroy(gameObject);
    }

    // Power Down - invert ship movement controlls for 10s 
    IEnumerator InvertedMovement(){
        player.isInverted = true;
        sr.enabled = false;
        ec.enabled = false;
        yield return new WaitForSeconds(10);
        player.isInverted = false;
        Destroy(gameObject);
    }

    // Power Down - stall the player shield for 5 seconds
    IEnumerator ShieldStall(){
        shield.canActivate = false;
        shield.currentActiveFramesDuration = 0f;
        sr.enabled = false;
        ec.enabled = false;
        yield return new WaitForSeconds(5);
        shield.canActivate = true;
        Destroy(gameObject);
    }

    // Power Up - Give the player the ability to tripple shot
    IEnumerator TrippleTrouble(){
        player.TripleShot = true;
        player.fireTime = 0.7f;
        sr.enabled = false;
        ec.enabled = false;
        yield return new WaitForSeconds(10);
        player.TripleShot = false;
        player.fireTime = 0.5f;
        Destroy(gameObject);
    }

    // Power Up - Give the player a health point
    IEnumerator GetHealth(){
        planet.health++;
        if(planet.health > 7){
            sr.enabled = false;
            ec.enabled = false;
            yield return new WaitForSeconds(12);
            planet.health--;
        }
        Destroy(gameObject);
    }

    // Power Up - Reduce player shooting delay for 10 seconds
    IEnumerator TempBoost(){
        player.fireTime = 0.1f;
        sr.enabled = false;
        ec.enabled = false;
        yield return new WaitForSeconds(10);
        player.fireTime = 0.5f;
        Destroy(gameObject);
    }
}
