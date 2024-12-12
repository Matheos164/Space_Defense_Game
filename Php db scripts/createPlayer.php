<?php
/**
 * This PHP file controlls the comunication to the server to create a player profile.

 *  This script connects to a localhost database and checks if a provide player name exists
 *  if name is not in use it creates a profile for the player
 */

    $conn = mysqli_connect('fdb1028.awardspace.net', '4560965_sdg', '7PSC8juP6+7oP_Ix', '4560965_sdg', '3306');

    // check connection
    if(mysqli_connect_errno()){
        echo "1: Connection Failed"; // connection failed
        exit();
    }

    $playerName = $_POST["playerName"];

    // check if player name is already in use
    $checkPlayerExists = "SELECT player_name FROM highscore WHERE player_name = '" .$playerName. "';";
    $check = mysqli_query($conn, $checkPlayerExists) or die("2: Name Check Failed"); // check querry failed

    if(mysqli_num_rows($check) > 0){ // check if name already exists
        echo "3: Name Already exist";
        exit();
    }

    // create the profile
    $CreateProfile = "INSERT INTO highscore (player_name) VALUES ('".$playerName."');";
    mysqli_query($conn, $CreateProfile) or die("4: Player Creation Failed"); // querry failed

    echo "0";

    $conn->close();

?>

