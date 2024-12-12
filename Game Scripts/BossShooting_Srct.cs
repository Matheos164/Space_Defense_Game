/*
    This File controlls the boss shot behaiviour

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShooting_Srct : MonoBehaviour
{
    [Header("Object Settings")]   
    public float orbitSpeed = 10f;
    public float moveSpeed = 0.1f;
    [Header("Object Dependency")]
    private Rigidbody2D rb;
    private Vector2 centerPoint;
    public Animator explotion;
    private SpriteRenderer spriteRenderer;

    // get the rigidBody and Sprite Render component
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // FixiedUpdate function calculates the orbit and speed of the object
    void FixedUpdate()
    {
        // Calculate the direction to the center
        Vector2 directionToCenter = centerPoint - rb.position;
        
        // Calculate the tangent direction for the orbital movement
        Vector2 tangentDirection = Vector2.Perpendicular(directionToCenter).normalized;
        
        // Apply the orbit and move towards the center
        Vector2 orbitMovement = tangentDirection * orbitSpeed;
        Vector2 centerMovement = directionToCenter.normalized * moveSpeed;
        
        rb.velocity = orbitMovement + centerMovement;
    }

    // OnCollisionEnter2D function checks for collition with the planet or shot and triggers the animation
    public void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Planet" || collision.gameObject.tag == "Shot" || collision.gameObject.tag == "shield" ){
            orbitSpeed = 0f;
            moveSpeed = 0f;
            spriteRenderer.sprite = null;
            explotion.SetTrigger("Hit");
            StartCoroutine(ExplotionAnimation());
        }
    }

    // wait for animation to complete than destroy game object
    IEnumerator ExplotionAnimation(){
        yield return new WaitForSeconds(explotion.GetCurrentAnimatorStateInfo(0).length);
        DestroyImmediate(gameObject);
    }
}