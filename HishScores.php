<?php
	$serverName = "localhost";
	$username = "root";
	$password = "";
	$dbName = "spacefury";

	// Make connection
	$conn = new mysqli($serverName, $username, $password, $dbName);

	// Check connection
	if (!$conn) {
		die("Connection Failed. ".mysqli_connect_error());
	}	
	else echo("Connection Success");

	// Get data
	$sql = "SELECT Id, Pseudo, Stage, Level, Time, Stars, Date FROM hishscores";
	$result = mysqli_query($conn, $sql);

	// Display results
	if (mysqli_num_rows($result) > 0) {
		echo("Ok");
		while ($row = mysqli_fetch_assoc($result)) {
			echo "ID: ".$row['Id']." Pseudo: ".$row["Pseudo"]." Level: ".$row['Stage'].$row['Level']." Time: ".$row['Time']."<br>";
		}
	}	else echo("Nothing");
?>