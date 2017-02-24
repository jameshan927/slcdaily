<?php

function insert_user($user) {
	global $db;
	
	$sql = "INSERT INTO users ";
	$sql .= "(name, email) ";
	$sql .= "VALUES (";
	$sql .= "'" . $user['name'] . "',";
	$sql .= "'" . $user['email'] . "'";
	$sql .= ");";
	
	$result = db_query($db, $sql);
	if($result) {
		return true;
    } else {
		echo db_error($db);
		db_close($db);
		exit;
	}
	
}
?>