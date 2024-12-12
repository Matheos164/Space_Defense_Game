<?php

/**
 * This PHP file controlls pulling the scores from the database to populate the scoreboard page in the game
 *
 */

    $conn = mysqli_connect('fdb1028.awardspace.net', '4560965_sdg', '7PSC8juP6+7oP_Ix', '4560965_sdg', '3306');

    // check connection
    if(mysqli_connect_errno()){
        echo "1: Connection Failed"; // connection failed
        exit();
    }

    $playerName = $_POST["playerName"];

    // check if there are any records in the db
    $checkPlayerRecords = "SELECT * FROM highscore";
    $check = mysqli_query($conn, $checkPlayerRecords) or die("2: Check Failed"); // check querry failed

    if(mysqli_num_rows($check) == 0){
        echo "3: No Resaults Found!";
        exit();
    }

    // select the top 10 players with the higest score
    $getTopPlayers = "SELECT player_name, player_score FROM highscore ORDER BY player_score DESC LIMIT 10";
    $data = mysqli_query($conn, $getTopPlayers) or die("4: Failed to get scores"); // querry failed

    // add the data to the array
    $arr = array();
    foreach($data as $row){
        $arr[] = $row;
    }

    $getPlayerScore = "SELECT player_name, player_score FROM highscore WHERE player_name = '" .$playerName. "';";
    $getScore =  mysqli_query($conn, $getPlayerScore) or die("5: Failed to get player");

    $ThisPlayer = "";
    if(mysqli_num_rows($getScore) > 0){
        $row = mysqli_fetch_assoc($getScore);
        $ThisPlayer = $row;
    }
    
    array_push($arr, $ThisPlayer);

    // encode the array into a json object and echo it
    echo json_encode($arr);
    
    $conn->close();

