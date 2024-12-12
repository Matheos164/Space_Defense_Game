using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoubleXdamage : MonoBehaviour
{
    [Header("Object Dependency")]
    private Player_Scrt player;
    private SpriteRenderer sr;
    private EdgeCollider2D ec;
    public AudioClip CollectionFX;

    
    void Start()
    {
        // get the player script and get the components
        player = FindObjectOfType<Player_Scrt>();
        sr = this.GetComponent<SpriteRenderer>();
        ec = this.GetComponent<EdgeCollider2D>();
    }

    // collisiton check to see if the power up has been collected
    public void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Shot"){ //check if the colition was with a 'shot' object
            StartCoroutine(TempBoost()); 
            Sounds.instance.PlayFX(CollectionFX, transform, 0.5f); // play sound
        }
    }

    // power up for a Double damagae
    IEnumerator TempBoost(){
        player.fireTime = 0.1f; // et the fire rate delay to 0.1
        // make the object invisible to not intervene with gameplay
        sr.enabled = false;
        ec.enabled = false;
        // make the boost last 10s than reajust the player and destroy the object.
        yield return new WaitForSeconds(10);
        player.fireTime = 0.5f;
        Destroy(gameObject);
    }
}
