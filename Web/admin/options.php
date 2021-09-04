<?php
// <editor-fold defaultstate="collapsed" desc="Init">
//error_reporting(0);
date_default_timezone_set('Europe/Athens');
header('Content-Type: application/json; charset=utf-8');
require_once 'authenticate.php';
// </editor-fold>

function getDays($sport) {
    $table = strtolower("{$sport}_live_mix");
    // Create the command
    $sql = "SELECT DISTINCT HUMAN_DAY(`StartTime`) AS `Date` FROM `{$table}`";
    $sql .= " ORDER BY `Date` DESC";
    // Execute the command
    db::executeQuery($sql);
    $rows = array();
    $i = 1;
    while ($row = db::$query->fetch_assoc()) {
        $rows[$i++] = $row['Date'];
    }
    // Free query set
    db::freeQuery();
    // return
    return $rows;
}
function getSources($sport, $day) {
    $table = strtolower("{$sport}_live_mix");
    // Create the command
    $sql = "SELECT DISTINCT `Source` FROM `{$table}`";
    $sql .= " WHERE HUMAN_DAY(`StartTime`) = '{$day}'";
    $sql .= " ORDER BY `Source` DESC";
    // Execute the command
    db::executeQuery($sql);
    $rows = array();
    $i = 1;
    while ($row = db::$query->fetch_assoc()) {
        $rows[$i++] = $row['Source'];
    }
    // Free query set
    db::freeQuery();
    // return
    return $rows;
}
function getChamps($sport, $day, $source) {
    $table = strtolower("{$sport}_live_mix");
    // Create the command
    $sql = "SELECT DISTINCT `Champ` FROM `{$table}`";
    $sql .= " WHERE HUMAN_DAY(`StartTime`) = '{$day}'";
    $sql .= " AND `Source` = '{$source}'";
    $sql .= " ORDER BY `Champ` ASC";
    // Execute the command
    db::executeQuery($sql);
    $rows = array();
    $rows[0] = '[ALL]';
    $i = 1;
    while ($row = db::$query->fetch_assoc()) {
        $rows[$i++] = $row['Champ'];
    }
    // Free query set
    db::freeQuery();
    // return
    return $rows;
}
// Data
$data = array();
// Sport
$data['sport'] = array();
$data['sport'][1] = 'Football';
$data['sport'][2] = 'Basket';
$sport = (!empty($_GET['sport'])) ? $_GET['sport'] : $data['sport'][1];
// Day
$data['day'] = getDays('Football');
$day = (!empty($_GET['day'])) ? $_GET['day'] : $data['day'][1];
// Source
$data['source'] = getSources($sport, $day);
$source = (!empty($_GET['source'])) ? $_GET['source'] : $data['source'][1];
// Champ
$data['champ'] = getChamps($sport, $day, $source);
$champ = (!empty($_GET['champ'])) ? $_GET['champ'] : $data['champ'][0];

echo json_encode($data);
