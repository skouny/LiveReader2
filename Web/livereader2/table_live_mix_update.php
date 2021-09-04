<?php
require_once('database.php');
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Set headers
header('Content-Type: text/plain; charset=utf-8');
// Sport
$sport = strtolower($_GET['sport']);
// Insert or update $_POST data into database
switch ($sport) {
    case 'football':
        mysqlInsertOrUpdateFromArray('football_live_mix', $_POST);
        break;
    case 'basket':
        mysqlInsertOrUpdateFromArray('basket_live_mix', $_POST);
        break;
}
?>