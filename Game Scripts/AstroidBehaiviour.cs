/*
    This File controlls the Astroid behaiviour

    #TODO
        - Add SFX
*/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AstroidBehaiviour : MonoBehaviour
{    
    [Header("Object Settings")]
    public Transform location;
    public float speed = 1f;
    public float spinSpeed = 1f;
    private Vector2 movement;

    [Header("Other Settings")]
    private Rigidbody2D rb;
    private EdgeCollider2D ec;
    public AudioClip explotion;
    
    [Header("Animation Settings")]
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // initilize the rigid boddy of the object
        rb = this.GetComponent<Rigidbody2D>();
        ec = this. GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    // the Update function makes the the object move towards the specified object no matter where the object is positioned
    void FixedUpdate()
    {
        // if(transform){
            Vector3 direction = location.position - transform.position;
            direction.Normalize();
            movement = direction;

            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime * spinSpeed);
            rb.MovePosition((Vector2)transform.position + (movement * speed *Time.deltaTime));
        // }
        

    }

    // OnCollisionEnter2D checks for contact and sets off an explotion animation
    public void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "shield" || collision.gameObject.tag == "Planet" || collision.gameObject.tag == "Shot"){
            speed = 0f;
            ec.enabled = false;
            animator.SetBool("Hit", true);
            Sounds.instance.PlayFX(explotion, transform, 0.5f);
            StartCoroutine(ExplotionAnimation());

            if(collision.gameObject.tag == "Shot"){
                GameLoop gameLoop = FindObjectOfType<GameLoop>();
                gameLoop.AddScore(1);
            }
        }
    }

    // ExplotionAnimation waits till the animation is complete and than destroys the game object
    IEnumerator ExplotionAnimation(){
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length -0.5f);
        Destroy(gameObject);
    }

}
