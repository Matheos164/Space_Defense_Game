/*
    This Scrip controlls the player behaiviour

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XInput;

public class Player_Scrt : MonoBehaviour
{
    [Header("Object Settings")]
    public float AngularyRadious, RotationRadious;
    public float x, y, angle = 0;
    public float SpinSpeed = 2f;
    public bool canShoot = true;
    public bool canMove = true;
    public bool isInverted = false;
    public bool TripleShot = false;
    bool Orbit_Left;
    bool Orbit_Right;
    public float delay = 0f;
    public float fireTime = 0.5f;
    public float shotSpeed = 10f;
    [Header("Object Dependency")]
    public Transform RotationCenter;
    public Animator animator;
    public AudioClip ShotFX;
    public AudioClip ThrusterFX;
    public Transform FirePoint;
    public Transform[] OtherFirePoints;
    public GameObject shotPrefab;



    // the update function controlls the action of the player object
    void Update() {
        //NONE FUNCTIONAL CODE - Controlls whether to use controller or MKB input
        // if (GameControllerManager.instance.useController){
        //     if(isInverted){
        //         Orbit_Left = Input.GetKey(KeyCode.Joystick1Button1);
        //         Orbit_Right = Input.GetKey(KeyCode.Joystick1Button0);
        //     }else{
        //         Orbit_Left = Input.GetKey(KeyCode.Joystick1Button0);
        //         Orbit_Right = Input.GetKey(KeyCode.Joystick1Button1);
        //     }
        // }else{
        //     if(isInverted){
        //         Orbit_Left = Input.GetKey(KeyCode.D);
        //         Orbit_Right = Input.GetKey(KeyCode.A);
        //     }else{
        //         Orbit_Left = Input.GetKey(KeyCode.A);
        //         Orbit_Right = Input.GetKey(KeyCode.D);
        //     }
        // }

        // check if input is inverted and use inpuds accordingly 
        if(isInverted){
            Orbit_Left = Input.GetKey(KeyCode.D);
            Orbit_Right = Input.GetKey(KeyCode.A);
        }else{
            Orbit_Left = Input.GetKey(KeyCode.A);
            Orbit_Right = Input.GetKey(KeyCode.D);
        }

        // key codes for actions
        bool Player_Shoot = Input.GetKey(KeyCode.Mouse0);

        // calculate the rotation radious
        x = RotationCenter.position.x + Mathf.Cos(angle) * RotationRadious;
        y = RotationCenter.position.y + Mathf.Sin(angle) * RotationRadious;

        // move the object and call the animation
        transform.position = new Vector2(x, y);
        if(Orbit_Left && canMove ){
            angle = angle + Time.deltaTime * AngularyRadious;
            animator.SetBool("IsMoving", true);
            gameObject.transform.Rotate(0f, 0f, SpinSpeed * Time.deltaTime * SpinSpeed);

        }else if(Orbit_Right && canMove){
            angle = angle - Time.deltaTime * AngularyRadious;
            animator.SetBool("IsMoving", true);
            gameObject.transform.Rotate(0f, 0f, -SpinSpeed * Time.deltaTime * SpinSpeed);

        }else{
            animator.SetBool("IsMoving", false);
        }
        
        if(angle >= 360){
            angle = 0;
        }

        // controll the shooting with a delay to prevent rapid fire spam
        if(Player_Shoot && Time.time > delay){
            Shoot();
            delay = Time.time + fireTime;
        }
        
    }



    // shooting function
    void Shoot(){
        if(canShoot){ // check if player can shoot 
            if(TripleShot){ // if the player has tripple shot enable the other 2 shooting point
                foreach(Transform point in OtherFirePoints){
                    GameObject Proj = Instantiate(shotPrefab, point.position, point.rotation);
                    Rigidbody2D rb = Proj.GetComponent<Rigidbody2D>();
                    rb.velocity = point.up * shotSpeed;
                    Sounds.instance.PlayFX(ShotFX, transform, 0.5f);
                }
            } else { // create an instance of the shot
                GameObject Proj = Instantiate(shotPrefab, FirePoint.position, FirePoint.rotation);
                Rigidbody2D rb = Proj.GetComponent<Rigidbody2D>();
                rb.velocity = FirePoint.up * shotSpeed;
                Sounds.instance.PlayFX(ShotFX, transform, 0.5f);
            }
        }
        
    }


    
}
