<?php
/**
 * This PHP file controlls the updating of the score when a player reaches a new high score

 *   the scrip connects to the database and checks the stored player score and curen achived score.
 *   if the scored score is >= to the current achive score the scrip exits
 *   else it will update the score in the db.
 */


    $conn = mysqli_connect('fdb1028.awardspace.net', '4560965_sdg', '7PSC8juP6+7oP_Ix', '4560965_sdg', '3306');

    // check connection
    if(mysqli_connect_errno()){
        echo "1: Connection Failed"; // connection failed
        exit();
    }

    $playerName = $_POST["playerName"];
    $playerScore = $_POST["playerScore"];

    // check if player name is already in use
    $checkPlayerScore = "SELECT player_score FROM highscore WHERE player_name = '" .$playerName. "';";
    $check = mysqli_query($conn, $checkPlayerScore) or die("2: Check Failed"); // check querry failed

    // if check if there are any results from the db
    if(mysqli_num_rows($check) > 0){
        $row = mysqli_fetch_assoc($check);
        $storedScore = $row['player_score'];
        if($storedScore >= $playerScore){ // check if the stored score is greater or equal to the current score
            exit();
        }
    }

    // update the score
    $UpdateScore = "UPDATE highscore SET player_score = '".$playerScore."' WHERE player_name = '" .$playerName. "';";
    mysqli_query($conn, $UpdateScore) or die("4: Update Failed"); // querry failed

    echo "0";
    
    $conn->close();

?>

