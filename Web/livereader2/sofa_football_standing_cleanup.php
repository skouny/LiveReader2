<?php
require_once('database.php');
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Execute
if (isset($_GET['champ_id']) && isset($_GET['group_index']) && isset($_GET['places_count'])) {
    $champ_id = $_GET['champ_id'];
    $group_index = $_GET['group_index'];
    $places_count = $_GET['places_count'];
    $sql = "DELETE FROM `sofa_football_standings`";
    $sql .= " WHERE (`ChampId` = {$champ_id} AND `GroupIndex` = {$group_index} AND `Place` > {$places_count})";
    mysqlQuery($sql);
    echo "cleanup done";
} else {
    echo "cleanup error";
}
?>
