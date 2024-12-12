/*
    This scrip fiile controlls the shooting behaiviour for the player spaceship

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingBehaviour : MonoBehaviour
{

    [Header("Object Dependency")]
    private Vector2 boundary;
    [Header("Object Settings")]
    public float speed = 10;
    public string ColideWith;

    // Start is called before the first frame update
    void Start()
    {
        // get the boundarys of the camera object 
        boundary = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        // check if the object is outside the set boundary
        if(gameObject.transform.position.x > boundary.x || gameObject.transform.position.y > boundary.y || gameObject.transform.position.x < -boundary.x || gameObject.transform.position.y < -boundary.y){
            // destroty the object if it is
            Destroy(gameObject);
        }
        
    }

    // detect if the shot has collided with a specified object
    public void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == ColideWith || collision.gameObject.tag == "shield"){
            // destroy the object if it has collided
            Destroy(gameObject);
        }  
    }
}
