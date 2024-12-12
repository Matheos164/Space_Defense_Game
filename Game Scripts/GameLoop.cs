using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
    [Header("Game Variables")]
    public int playerScore = 0;
    public float spawnRate = 6f;
    private float spawnTimer;
    private bool bossActive = false;
    private bool newScore = false;
    
    [Header("Game Objects Dependencies")]
    public GameObject Astroid;
    public GameObject AlienShip;
    public GameObject AlienBoss;
    public GameObject GameOverUI;
    public GameObject Earth;
    public GameObject[] powerUps;
    public Image[] ShieldBars;

    [Header("Other Dependencies")]
    public Transform Target;
    public Camera MainCamera;
    private Planet_Scrt planet_Scrt;
    private MusicController musicController;
    public TextMeshProUGUI scoreText;
    public AudioSource Music;
    public TextMeshProUGUI NHS;
    
    

    
    
    void Start()
    {
        // intitilize the game and grab the planet scrip and music script, and start the power up timer spawners
        InitializeGame(); 
        planet_Scrt = Earth.GetComponent<Planet_Scrt>();
        musicController = Music.GetComponent<MusicController>();
        StartCoroutine(SpawnPowerUpRotation());
    }

    void Update()
    {
        UpdateGame(); // update the game constantly
    }

    // intilize game stats when game starts
    void InitializeGame() {
        spawnTimer = 0f;
        Time.timeScale = 1f;
        spawnEnemies();
        UpdateText();
    }


    // updategame that controlls spawn rate, score UI text updates and Game Over call
    void UpdateGame(){
        // change spawn rate based on player score
        if(playerScore % 25 == 0 && playerScore !> 100){
            spawnRate = spawnRate - 0.5f;
        }

        // spawn timer between each enemy
        spawnTimer += Time.deltaTime;
        if(spawnTimer >= spawnRate){
            spawnEnemies();
            spawnTimer = 0f;
        }

        if(planet_Scrt.health == 0){ // constantly check player HP
            GameOver();
        }

        UpdateText(); // keep player score update on screen

    }

    //Game over logic that triggers the game over screen and stops the game
    void GameOver(){
        GameOverUI.SetActive(true);
        musicController.canPlay = false;
        Music.Stop();
        
        if(newScore){
            NHS.enabled = true;
        }

        UpdateDbScore(); // call the update db to update stored score
        Time.timeScale = 0f;
    }

    // astroids spawner that indicates its status based on the player score
    void spawnAstroid(){

        // change the astroid speed based on player score
        float speedMultiplier = 1f;
        if(playerScore < 15){
            speedMultiplier = 1.5f;
        }else if(playerScore <= 25){
            speedMultiplier = 2.2f;
        }else if(playerScore <= 32){
            speedMultiplier = 2.7f;
        }else if(playerScore > 40){
            speedMultiplier = 3.2f;
        }
        
        // chose a random spawn location and creat a instance of the astroid
        Vector2 spawnLocation = RandomLocation();
        GameObject astroid = Instantiate(Astroid, spawnLocation, Quaternion.identity);
        AstroidBehaiviour astroidBehaiviour = astroid.GetComponent<AstroidBehaiviour>();
        astroidBehaiviour.location = Target;
        astroidBehaiviour.speed = astroidBehaiviour.speed * Random.Range(1f, speedMultiplier);
        astroidBehaiviour.spinSpeed = Random.Range(2f, 4.5f);

    }


    // alien ship spawner that indicates its status based on the score 
    void spawnAlienShip(){

        // set the alien ship stats based on player score
        int alienHealth = 1;
        float alienSpeed = 2f;
        if(playerScore < 15){
            alienHealth = 1;
            alienSpeed = 2f;
        }else if(playerScore <= 25){
            alienHealth = 2;
            alienSpeed = 2.5f;
        }else if(playerScore > 32){
            alienHealth = 3;
            alienSpeed = 3.1f;
        }

        // create a instance of the ship at random location
        Vector2 spawnLocation = RandomLocation();
        GameObject alien = Instantiate(AlienShip, spawnLocation, Quaternion.identity);
        AlienShipBehaiviour alienShipBehaiviour = alien.GetComponent<AlienShipBehaiviour>();
        alienShipBehaiviour.health = alienHealth;
        alienShipBehaiviour.speed = alienSpeed;
        alienShipBehaiviour.target = Target;

    }

    // Boss spawner that indicates its status based on player score
    void spawnBoss(){

        // set the boss status based on player score
        bossActive = true;
        int BossHealth = 4;
        float BossShootingDellay = 12f;
        float miniEnemiesSpawnDelay = 50f;
        if(playerScore == 20){
            BossHealth = 4;
            BossShootingDellay = 12f;
            miniEnemiesSpawnDelay = 50f;
        }else if(playerScore == 40){
            BossHealth = 3;
            BossShootingDellay = 10f;
            miniEnemiesSpawnDelay = 40f;
        }else if(playerScore >= 60){
            BossHealth = 4;
            BossShootingDellay = 7f;
            miniEnemiesSpawnDelay = 35f;
        }

        //create an instance of the boss and stop spawns of any other enemy types
        Vector2 spawnLocation = RandomLocation();
        GameObject alienBoss = Instantiate(AlienBoss, spawnLocation, Quaternion.identity);
        AlienShipBossBehaiviour alienShipBossBehaiviour = alienBoss.GetComponent<AlienShipBossBehaiviour>();
        alienShipBossBehaiviour.target = Target;
        alienShipBossBehaiviour.health = BossHealth;
        alienShipBossBehaiviour.shootingDelay = BossShootingDellay;
        alienShipBossBehaiviour.spawnTimer = miniEnemiesSpawnDelay;
        alienShipBossBehaiviour.onBossDestroyed += bossDestroyed;
    }

    // set the boss active to false to resume enemies spawn
    void bossDestroyed(){
        bossActive = false;
    }

    // Spawn Enemies based off player score
    void spawnEnemies(){
        // empty return if boss is active to prevent spawns of other enemies when boss is active
        if(bossActive){
            return;
        }

        // spawn only astroids if player score is lower than 17
        if(playerScore < 17){
            spawnAstroid();
        }else if(playerScore >= 17 && playerScore % 25 != 0){ // randomly spawn astroids and enemy aliens 
            if(Random.Range(0, 2) == 0){
                spawnAstroid();
            }else{
                spawnAlienShip();
            }
        }else if(playerScore % 25 == 0){ // if score is equaly devisable by 25 spawn boss enemy
            spawnBoss();
        }
    }

    // random location "Generator" thats adapts to the camera size
    Vector2 RandomLocation(){
        // get the camera sizing and get a random x and y cord
        float height = MainCamera.orthographicSize * 2f;
        float width = height * MainCamera.aspect;
        float xCord = Random.Range(-width, width);
        float yCord = Random.Range(-height, height);

        // adjust the cords based of the selection by half the width or height
        if(xCord > 0){
            xCord += width / 2;
        }else{
            xCord -= width / 2;
        }

        if(yCord > 0){
            yCord += height / 2;
        }else{
            yCord -= height / 2;
        }

        return new Vector2(xCord, yCord);
    }

    // add and update the score 
    public void AddScore(int val){
        playerScore += val;
    }

    // update the scor UI element
    void UpdateText(){
        scoreText.text ="" + playerScore;
        
    }

    // Power Up spawner 
    public void SpawnPowerUp(){
        // get a random location on the canvas
        Vector2 location = PowerUpRandLoc();
        int randomRange = Random.Range(0, powerUps.Length);
        GameObject selectedPowerUp = powerUps[randomRange]; // select a random power up to spawn

        Instantiate(selectedPowerUp, location, Quaternion.identity); // create an instance of the selected power up
    }

    // random location generator for power ups
    Vector2 PowerUpRandLoc(){
        // get the camer dimentions
        float height = MainCamera.orthographicSize * 2f;
        float width = height * MainCamera.aspect;
        Vector2 RandPosition;

        // get a random x and y cord based on camera dimentions
        do{
            float xCord = Random.Range(-width/2f + 3f , width/2f - 3f);
            float yCord = Random.Range(-height/2f + 3f , height/2f - 3f);

            RandPosition = new Vector2(xCord, yCord);
        }while(Vector2.Distance(RandPosition, Vector2.zero) < 5f);

        return RandPosition;
    }

    // continuislt spwan power ups at random intervals
    IEnumerator SpawnPowerUpRotation(){
        while(true){
            yield return new WaitForSeconds(Random.Range(20, 60)); // random delay between 20 to 60 seconds  
            SpawnPowerUp(); // spawn power up
        }
    }

    // Initiates the coroutine to update the player's score
    public void UpdateDbScore(){
        StartCoroutine(UpdateScore());
    }

    // Coroutine to process updating the player's score in the db
    IEnumerator UpdateScore(){
        // create a form 
        WWWForm form = new WWWForm();
        form.AddField("playerName", PlayerPrefs.GetString("PlayerName"));
        form.AddField("playerScore", playerScore);

        // WWW www = new WWW("http://localhost/sqlconnect/updateDb.php", form);
        WWW www = new WWW("http://spacedefence.atwebpages.com/updateDb.php", form); //make request to the server to update the db with player info
        yield return www;
        
        //check return
        if(www.text == "0"){
            Debug.Log("Player Score Updated in DB!");
            newScore = true;
        }else{
            Debug.Log("Player Score Was Not Updated in DB " + www.text);
        }
        
    }
}
