<?php
require_once('database.php');
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Set headers
header('Content-Type: text/plain; charset=utf-8');
// Insert or update $_POST data into database
mysqlInsertOrUpdateFromArray('sofa_football_standings', $_POST);
?>