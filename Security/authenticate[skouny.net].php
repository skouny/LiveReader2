<?php // Main
if (isset($_POST['Username']) && isset($_POST['Key'])){
	$username = 'skouny';
	$password = 'zero@bit';
	$key = $_POST['Key'];
	if ($username == $_POST['Username']) {
		echo hash('sha512', "[~{$username}@{$password}#{$key}&]");
	}
}
?>