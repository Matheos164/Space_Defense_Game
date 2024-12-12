/*
    This class is non functional and was part of the iput system
    All the code here is left untouch to preserver where the project got left out

*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerManager : MonoBehaviour
{
    [Header("Object Dependency")]
    public static GameControllerManager instance;
    public bool useController = false;

    // callt he instance of tthis object oin awake makin sure game object does not get destroyed
    private void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadInput();
        } else {
            Destroy(gameObject);
        }
    }
    
    // set the input mode to controller or MKB and also save the option to the playerPref
    public void SetInputMode(bool controller){
        useController = controller;
        PlayerPrefs.SetInt("UseController", controller ? 1 : 0);
        PlayerPrefs.Save();
    }

    // load the input prefrance
    private void LoadInput(){
        if(PlayerPrefs.HasKey("UseController")){
            useController = PlayerPrefs.GetInt("UseController") == 1;
        }
    }
}
