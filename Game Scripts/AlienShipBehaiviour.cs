/*
    This Script controlls how the Alien Ship Behaves and oparates

    #TODO
        - Improve the cordianate choosing behaiviour
            - make it not choos any cords that are close or in the (0,0) position
            - make the ship choses cordinantes based on where it spawned on the map
                - If spawned in the -x, -y quadrent of the map it will chose a cord in that quadren etc.
        - Fix hovering Animation when in idle state
        - Add SFX
        
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienShipBehaiviour : MonoBehaviour
{
    [Header("Object Settings")]
    public Transform target;
    public float speed = 1f;
    public int health = 2;
    public bool inPosition = false;
    public Vector2 sceneMaxBoundary;
    public Vector2 sceneMinBoundary;
    public float delay = 10f;
    public float fireTime = 0.5f;
    public float shotSpeed = 6f;

    [Header("Object Dependency")]
    public Transform FirePoint;
    public GameObject shotPrefab;
    private EdgeCollider2D ec;
    public AudioClip ShotFX;
    public AudioClip explotion;
    private Camera camera;
    private Vector3 targetPosition;

    [Header("Animation Settings")]
    public Animator MoveAnimator;
    public Animator ExplotionAnimator;
    public Animator ShotAnimator;

    // Start is called before the first frame update
    void Start()
    {
        // -- Old revetion of the target position code
        // when the object is created it choses a random x and y location on the map and the var targetPosition is assigned
        // Vector2 targetPosition = new Vector2(
        //     Random.Range(sceneMinBoundary.x, sceneMaxBoundary.x),
        //     Random.Range(sceneMinBoundary.y, sceneMaxBoundary.y)
        // );

        // calls the move function and StartCoroutine moves the object to the position over time and not instantly
        // if(!inPosition){
        //     StartCoroutine(Move(targetPosition));
        // }

        ec = this.GetComponent<EdgeCollider2D>();

        int side = Random.Range(0, 4);
        Vector3 spawnPosition = Vector3.zero;
        camera = Camera.main;
        float hight = camera.orthographicSize * 2f;
        float width = hight * camera.aspect;

        // Determine spawn and target positions
        switch (side) {
            case 0: // Top
                spawnPosition = new Vector3(Random.Range(-width / 2f, width / 2f), hight / 2f + 1f, 0f);
                targetPosition = new Vector3(spawnPosition.x, hight / 2f - 0.8f, 0f);
                break;
            case 1: // Bottom
                spawnPosition = new Vector3(Random.Range(-width / 2f, width / 2f), -hight / 2f + 1f, 0f);
                targetPosition = new Vector3(spawnPosition.x, -hight / 2f + 0.8f, 0f);
                break;
            case 2: // Left
                spawnPosition = new Vector3(-width / 2f - 1f, Mathf.Clamp(Random.Range(-hight / 2f, hight / 2f), -hight / 2f + 1f, hight / 2f - 1f), 0f);
                targetPosition = new Vector3(-width / 2f + 9f, spawnPosition.y, 0f);
                break;
            case 3: // Right
                spawnPosition = new Vector3(width / 2f + 1f, Mathf.Clamp(Random.Range(-hight / 2f, hight / 2f), -hight / 2f + 1f, hight / 2f - 1f), 0f);
                targetPosition = new Vector3(width / 2f - 9f, spawnPosition.y, 0f);
                break;
        }
        transform.position = spawnPosition;
        StartCoroutine(Move(targetPosition));

    }


    // Update is called once per frame
    void Update()
    {
        Rotation();

        // check if object is in position which trigers the object to start shooting at the player
        if(inPosition && Time.time > delay){
            ShotAnimator.SetTrigger("Shot");
            StartCoroutine(ShotAnimation());
            delay = Time.time + fireTime;
        }
    }

    // Moves the object to the chosen X and Y position
    IEnumerator Move(Vector2 targetPosition){
        while(Vector2.Distance(transform.position, targetPosition) > 0.1f){
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            yield return null;
        }
        transform.position = targetPosition;
        inPosition = true;
        MoveAnimator.SetBool("atDestination", true);
    }

    // Rotation keeps the object facing the player object 
    void Rotation(){
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
    }

    // OnCollisionEnter2D checks if there was a collisiin and checks what the object health is at to check if the explotion trigger should be called 
    public void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Shot"){
            if(health == 0){
                ec.enabled = false;
                ExplotionAnimator.SetBool("Explode", true);
                Sounds.instance.PlayFX(explotion, transform, 0.5f);
                StartCoroutine(ExplotionAnimation());
                GameLoop gameLoop = FindObjectOfType<GameLoop>();
                gameLoop.AddScore(2);
            }else{
                health--;
            }
        }
    }

    // runs the explotion animation and destroys the object when enemation is complete
    IEnumerator ExplotionAnimation(){
        yield return new WaitForSeconds(ExplotionAnimator.GetCurrentAnimatorStateInfo(0).length -0.5f);
        Destroy(gameObject);
    }

    // runs the shooting animation and after the shot is fired it sets the state to 'reload' as the ship idle state
    IEnumerator ShotAnimation(){
        yield return new WaitForSeconds(ShotAnimator.GetCurrentAnimatorStateInfo(0).length);
        Shoot();
        ShotAnimator.SetTrigger("Reload");
    }



    // Shoot func that calls the shot object
    void Shoot(){
        GameObject Proj = Instantiate(shotPrefab, FirePoint.position, FirePoint.rotation);
        Rigidbody2D rb = Proj.GetComponent<Rigidbody2D>();
        rb.velocity = FirePoint.right * shotSpeed;
        Sounds.instance.PlayFX(ShotFX, transform, 0.5f);
    }

}
