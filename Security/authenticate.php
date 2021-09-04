<?php // Main
require_once('database.php');
if (isset($_POST['Username']) && isset($_POST['Key'])){
	$username = $_POST['Username'];
	$password = mysqlQuerySelect("SELECT `Password` FROM `users` WHERE `Username`='{$username}';");
	$password = $password['Password'];
	$key = $_POST['Key'];
	if (strlen($username) >= 6 && strlen($password) >= 8) {
		echo hash('sha512', "[~{$username}@{$password}#{$key}&]");
	}
	// Log It
	$log = mysqlQuery("INSERT INTO `users_log` (`Username`, `Key`, `IP`) VALUES ('{$username}', '{$key}', '{$_SERVER['REMOTE_ADDR']}');");
}
?>