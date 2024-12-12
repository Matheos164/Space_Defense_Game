using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrippleShot : MonoBehaviour
{
    [Header("Object Dependency")]
    private Player_Scrt player;
    private SpriteRenderer sr;
    private EdgeCollider2D ec;
    public AudioClip CollectionFX;

    void Start(){
        // get the player script and get the components
        player = FindObjectOfType<Player_Scrt>();
        sr = this.GetComponent<SpriteRenderer>();
        ec = this.GetComponent<EdgeCollider2D>();
    }

    // collisiton check to see if the power up has been collected
    public void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Shot"){ //check if the colition was with a 'shot' object 
            StartCoroutine(TrippleTrouble());
            Sounds.instance.PlayFX(CollectionFX, transform, 0.5f); // play sfx
        }
    }

    // Tripple shot enables 2 other shooting points for the player to have 3 shots at the same time for 10s
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
}
