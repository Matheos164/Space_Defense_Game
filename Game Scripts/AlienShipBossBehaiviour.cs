/*
    This File controlls the how the boss enemy behaivs

    #TODO
        - Add SFX
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienShipBossBehaiviour : MonoBehaviour {
    [Header("Object Status")]
    public Transform target;
    private Vector3 targetPosition;
    public bool inPosition = false;
    public float speed = 5f;
    public int health = 5;
    public float shootingDelay = 8.5f;
    private bool invincibale = true;

    [Header("Mini Enemys Settings")]
    public Transform[] SpawnPoints;
    public float SpawnDelay = 2f;
    public float spawnTimer = 24f;

    [Header("Animatio Settings")]
    public Animator thrusterAnimation;
    public Animator Explotion;

    [Header("Dependencys")]
    public GameObject MiniShips;
    public Transform FirePoint;
    public GameObject Shield;
    public GameObject shotPrefab;
    private SpriteRenderer spriteRenderer;
    private Camera camera;
    public delegate void bossDefeated();
    public event bossDefeated onBossDestroyed;
    private EdgeCollider2D ec;
    public AudioClip ShootSFX;
    public AudioClip boomSFX;


    // Initialization
    void Start() {

        ec = this.GetComponent<EdgeCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        int side = Random.Range(0, 4);
        Vector3 spawnPosition = Vector3.zero;
        camera = Camera.main;
        float hight = camera.orthographicSize * 2f;
        float width = hight * camera.aspect;

        // Determine spawn and target positions
        switch (side) {
            case 0: // Top
                spawnPosition = new Vector3(Random.Range(-width / 2f, width / 2f), hight / 2f + 1f, 0f);
                targetPosition = new Vector3(spawnPosition.x, hight / 2f - 1.8f, 0f);
                break;
            case 1: // Bottom
                spawnPosition = new Vector3(Random.Range(-width / 2f, width / 2f), -hight / 2f + 1f, 0f);
                targetPosition = new Vector3(spawnPosition.x, -hight / 2f + 1.8f, 0f);
                break;
            case 2: // Left
                spawnPosition = new Vector3(-width / 2f - 1f, Mathf.Clamp(Random.Range(-hight / 2f, hight / 2f), -hight / 2f + 1f, hight / 2f - 1f), 0f);
                targetPosition = new Vector3(-width / 2f + 3f, spawnPosition.y, 0f);
                break;
            case 3: // Right
                spawnPosition = new Vector3(width / 2f + 1f, Mathf.Clamp(Random.Range(-hight / 2f, hight / 2f), -hight / 2f + 1f, hight / 2f - 1f), 0f);
                targetPosition = new Vector3(width / 2f - 3f, spawnPosition.y, 0f);
                break;
        }
        transform.position = spawnPosition;
        StartCoroutine(MoveIn());
    }

    // Main loop makes sure that the enemy is facing the target at all times and changes the thruster animation
    void Update() {
        Rotation();
        if (inPosition) {
            thrusterAnimation.SetBool("isInPosition", true);
        }
    }

    // Move the boss to the target position
    IEnumerator MoveIn() {
        thrusterAnimation.SetBool("isInPosition", false);
        invincibale = true;
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.8f);
        inPosition = true;
        Shield.SetActive(true);
        invincibale = false;
        yield return new WaitForSeconds(SpawnDelay);
        StartCoroutine(SpawnEnemies());
        StartCoroutine(Shoot());
    }

    // Rotate to face the target
    void Rotation() {
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    // Spawn mini enemies based on spawnTimer
    IEnumerator SpawnEnemies() {
        while (health != 0) {
            if (inPosition) {
                foreach (Transform spawnpoint in SpawnPoints) {
                    Animator SpawnController = spawnpoint.GetComponent<Animator>();
                    SpawnController.SetTrigger("Spawn");
                    yield return new WaitForSeconds(SpawnController.GetCurrentAnimatorStateInfo(0).length - 0.2f);
                    GameObject miniEnemiy = Instantiate(MiniShips, spawnpoint.position, spawnpoint.rotation);
                    AlienShipBehaiviour alienShip = miniEnemiy.GetComponent<AlienShipBehaiviour>();
                    alienShip.target = target;
                }
            }
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    // Handle collision with shots
    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Shot" && !invincibale) {
            if (health == 0) { // check for health, if health does not = 0 than enemy moves to new position
                ec.enabled = false;
                invincibale = true;
                spriteRenderer.sprite = null;
                Explotion.SetBool("isDead", true);
                Sounds.instance.PlayFX(boomSFX, transform, 0.5f);
                StartCoroutine(ExplotionAnimation());

                GameLoop gameLoop = FindObjectOfType<GameLoop>();
                gameLoop.AddScore(3);
                if(onBossDestroyed != null){
                    onBossDestroyed.Invoke();
                }
            } else {
                health--;
                NewPosition();
            }
        }
    }

    // Explosion animation and destroy the game object
    IEnumerator ExplotionAnimation() {
        yield return new WaitForSeconds(Explotion.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    // Subscribe and unsubscribe to shield status updates
    private void OnEnable() {
        EnemyShieldBehaiviour.ShieldChangeStatus += UpdateInvincibility;
    }

    private void OnDisable() {
        EnemyShieldBehaiviour.ShieldChangeStatus -= UpdateInvincibility;
    }

    // Update invincibility based on shield status
    private void UpdateInvincibility(bool shieldActive) {
        invincibale = shieldActive;
    }

    // Move the boss to a new position after getting hit
    void NewPosition() {
        EnemyShieldBehaiviour enemyShieldBehaiviour = FindObjectOfType<EnemyShieldBehaiviour>();
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 newPosition = new Vector3(Mathf.Cos(angle) * 5f, Mathf.Sin(angle) * 5f, 0f);
        targetPosition = newPosition;
        inPosition = false;
        enemyShieldBehaiviour.isActive = true;
        invincibale = true;
        speed = 10;
        StartCoroutine(MoveIn());
    }

    // Shooting that calls the shot for the enemy
    IEnumerator Shoot() {
        yield return new WaitForSeconds(1.5f);
        while (health != 0) {
            if (inPosition) {
                Instantiate(shotPrefab, FirePoint.position, FirePoint.rotation);
                Sounds.instance.PlayFX(ShootSFX, transform, 0.5f);
            }
            yield return new WaitForSeconds(shootingDelay);
        }
    }
}
