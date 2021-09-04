<?php // Start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
// Dependencies
require_once('database.php');
// Functions
function getQueryDict() {
    $dict = array();
    $sql = "SELECT * FROM `football_queries` ORDER BY `Id` ASC;";
    $result = mysqlQuerySelect($sql);
    foreach ($result as $row) {
        $dict[$row['Id']] = $row['Name'];
    }
    return $dict;
}
function getTrueQueries($champ_id, $date_start, $date_end, $query_ids) {
    $dict = array();
    $sql = "SELECT * FROM `football_opap_queries`";
    $sql .= " WHERE `ChampId` = '{$champ_id}'";
    $sql .= " AND `StartTime` >= '{$date_start}'";
    $sql .= " AND `StartTime` <= '{$date_end}'";
    $matches = mysqlQuerySelect($sql);
    foreach ($matches as $match) {
        foreach ($query_ids as $queryId) {
            if ($match[$queryId]) $dict[$match['Id']][] = $queryId;
        }
    }
    return $dict;
}
switch ($_GET['action']) {
    case 'champ':
        header('Content-Type: application/json; charset=utf-8');
        $champ_id = $_GET['champ_id'];
        $date_start = $_GET['date_start'];
        $date_end = $_GET['date_end'];
        $query_ids = explode(',', $_GET['query_ids']);
        $result = getTrueQueries($champ_id, $date_start, $date_end, $query_ids);
        echo json_encode($result);
        //echo print_r($result);
        break;
    case 'dict':
        header('Content-Type: application/json; charset=utf-8');
        $queryIds = getQueryDict();
        echo json_encode($queryIds);
        //echo print_r($queryIds);
        break;
}
?>
<?php // End gzip compressing
while (@ob_end_flush());
?>
