<?php

ob_start();
define("PRIVATE_PATH", dirname(__FILE__));
define("PROJECT_PATH", dirname(PRIVATE_PATH));

require_once('database.php');
require_once('query.php');

include 'header.php';

$db = db_connect();

$success = false;
$errors = array();
$user = array(
    'name' => '',
	'email' => ''
);

if($_SERVER['REQUEST_METHOD'] == 'POST') {
   
   if(isset($_POST['name'])) { $user['name'] = $_POST['name']; }
   if(isset($_POST['email'])) { $user['email'] = $_POST['email']; }
   
   $result = insert_user($user);
   if($result === true) {
      $new_id = db_insert_id($db);
	  $success = true;
   } else {
	   $errors = $result;
   }
}

?>

<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>SLC Today</title>
    <link href="css/bootstrap.min.css" rel="stylesheet">
	<link href="css/register_style.css" rel="stylesheet">
  </head>
  <body>

  	<?php if($success) { echo "<div class=\"alert alert-success\" role=\"alert\">Successfully registered!</div>"; } ?>

  
<div id="main-conent">

	<div id="register">
			
	  <br />
	  <h2>Sign up for SLC Daily!</h2>
	  	<br />
		<form action="register.php" method="post">
		   <br />
		   Name:
		   <input type="text" name="name" value="<?php if($success) { echo ''; } else { echo $user['name']; } ?>" /><br /><br />
		   Email:
		   <input type="text" name="email" value="<?php if($success) { echo ''; } else { echo $user['email']; } ?>" /><br />
		   <br /> <br />
		   <input type="submit" class="btn btn-primary btn-lg" name="submit" value="Register" /> <br />
		</form>
		
	</div>

</div>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="js/bootstrap.min.js"></script>
</body>
</html>