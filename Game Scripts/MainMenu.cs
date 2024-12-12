using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [Header("Object Dependency")]
    public Toggle MKB;
    public Toggle Controller;
    public Slider Music;
    public Slider SFX;
    public TMP_InputField playerName;
    public Button playerNameSubmit;
    public GameObject firstLaunchPannel;
    public GameObject firstLaunchPannelBox;
    public TMP_Text playerNameText;
    public TMP_Text[] leaderBoard;
    private const string FirstlaunchKey = "FirstLaunch";


    void Awake(){
        // MKB.onValueChanged.AddListener(delegate{onToggleChange(false);});
        // Controller.onValueChanged.AddListener(delegate{onToggleChange(true);});
        // InitializeToggles();

        // check if the player name is set 
        //if it has not it clears the player prefrences
        if(!PlayerNameSet()){
            PlayerPrefs.DeleteAll(); 
            PlayerPrefs.Save();
        }
        
        InitilizeGame(); // initize the game
    }

    // intilize to check if the game is being launched for the first time
    private void InitilizeGame(){
        if(!isFirstlaunch()){ // if game is launche for the first time
            // set the firs launch key
            PlayerPrefs.SetInt(FirstlaunchKey, 1);
            PlayerPrefs.Save();
            // activate the first launch menu
            firstLaunchPannelBox.SetActive(true);
            firstLaunchPannel.SetActive(true);
        }else if(isFirstlaunch()){ // if game has been launched before 
            // set the player creation pannel to false 
            firstLaunchPannel.SetActive(false);
            firstLaunchPannelBox.SetActive(false);
            playerNameText.SetText("Profile: " + PlayerPrefs.GetString("PlayerName")); // set the player name in main menu
            
        }
    }

    // check if first launch key has been created in PlayerPref
    static bool isFirstlaunch(){ 
        return PlayerPrefs.HasKey(FirstlaunchKey);
    }

    // check if player name is set in PlayerPref
    static bool PlayerNameSet(){
        return PlayerPrefs.HasKey("PlayerName");
    }



    // NONE FUNCTIONAL CODE - functions would controlls the toggles to insure they are not active at the same time
    // The functions would also controll input types whether it would be a controller or MKB
    
    // private void InitializeToggles(){
    //     if (GameControllerManager.instance != null) {
    //         if (GameControllerManager.instance.useController){
    //             Controller.isOn = true;
    //             MKB.isOn = false;
    //         } else {
    //             MKB.isOn = true;
    //             Controller.isOn = false;
    //         }
    //     }
    // }

    // private void onToggleChange(bool useController){
    //     if(t1.isOn){
    //         t2.isOn = false;
    //         GameControllerManager.instance.SetInputMode(!useController);
    //         Debug.Log(useController ? "Keyboard On" : "Controller On");
    //         Debug.Log(useController);
    //     }else if (t2.isOn){
    //         t1.isOn = false;
    //         GameControllerManager.instance.SetInputMode(useController);
    //         Debug.Log(useController ?  "Controller On" : "Keyboard On");
    //         Debug.Log(useController);
    //     }else if(!t1.isOn && !t2.isOn){
    //         t1.isOn = true;
    //         t2.isOn = false;
    //         GameControllerManager.instance.SetInputMode(!useController);
    
    //     }
    // }


    // Check if the player name is greater than 3 charecters
    public void VerifyPlayerName(){
        playerNameSubmit.interactable = playerName.text.Length >= 3;
    }


    // create the player profile
    public void createPlayer(){
        StartCoroutine(CreatePlayerProfile());
    }

    IEnumerator CreatePlayerProfile(){
        // create a form andadd player name to the form
        WWWForm form = new WWWForm();
        form.AddField("playerName", playerName.text);

        // make the request to the php file hosted 
        // WWW www = new WWW("http://localhost/sqlconnect/createPlayer.php", form);
        WWW www = new WWW("http://spacedefence.atwebpages.com/createPlayer.php", form);
        yield return www;
        string responseText = www.text.Trim();

        // check the responce
        if(responseText == "0"){
            Debug.Log("Player Added to DB!");
            // Set the firstpannel to false and set PlayerPrefs
            firstLaunchPannel.SetActive(false);
            firstLaunchPannelBox.SetActive(false);
            PlayerPrefs.SetString("PlayerName", playerName.text);
            PlayerPrefs.SetInt("playerScore", 0);
            PlayerPrefs.Save();
            playerNameText.SetText("Profile: " + PlayerPrefs.GetString("PlayerName"));
        }else{
            Debug.Log("Player Addition toi the DB was Unsucssessful " + www.text);
        }
    }

    // start the game and load the main game scene
    public void StartGame(){
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(1);
    }

    // Call the scores
    public void ScoreBoardScreen(){
        StartCoroutine(getScores());
    }

    IEnumerator getScores(){
        // create web form
        WWWForm form = new WWWForm();
        form.AddField("playerName", PlayerPrefs.GetString("PlayerName"));

        // WWW www = new WWW("http://localhost/sqlconnect/PlayerScores.php", form);
        WWW www = new WWW("http://spacedefence.atwebpages.com/PlayerScores.php", form); // send the request to the php file hosted
        yield return www;

        // get json responce
        string json = www.text;
        Debug.Log(json);
        // parse and populate scoreboard fileds with data
        int i = 0;
        Player[] players = JsonUtility.FromJson<PlayerList>("{\"players\":" + json + "}").players; 
        foreach (Player player in players) { 
            if(i == 10){
                break;
            }
            Debug.Log($"Player Name: {player.player_name}, Score: {player.player_score}"); 
            leaderBoard[i].SetText($"{i+1}. {player.player_name} | Score: {player.player_score}");
            i++;
        }

        leaderBoard[10].SetText($"You'r Score: {players[10].player_score}");
    }

    // quit the application
    public void Quit(){
        Application.Quit();
    }

    // restart the game when on game over screen
    public void RestartGame() {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart the current game scene
    }

    // load the main menu when on game over screen
    public void GoToMainMenu() {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadSceneAsync(0); // Load the main menu scene
    }
}
