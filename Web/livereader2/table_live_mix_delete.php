<?php
require_once('database.php');
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Set headers
header('Content-Type: text/plain; charset=utf-8');
// Execute
$msg = "Nothing happend";
$range = humanTodayRange();
$beginDT = $range[0];
$endDT = $range[1];
if (isset($_GET['sport'])) {
    $sport = $_GET['sport'];
    $table = strtolower("{$sport}_live_mix");
    if (isset($_GET['source'])) {
        $source = $_GET['source'];
        if (isset($_GET['webid'])) { // delete the selected webid
            $webid = $_GET['webid'];
            $sql = "DELETE FROM `{$table}` WHERE `Id` LIKE '{$source}#{$webid}'";
            $msg = "Match {$source}#{$webid} removed from database";
        } else { // delete all source
            $sql = "DELETE FROM `{$table}` WHERE `Source` LIKE '{$source}'";
			$sql .= " AND `StartTime` >= '{$beginDT}' AND `StartTime` < '{$endDT}'";
            $msg = "Source {$source} removed from database";
        }
	} elseif (isset($_GET['delete-old-records'])) { // delete records before yesterday
		$yesterdayBegin = humanYesterdayBegin();
		$sql = "DELETE FROM `{$table}` WHERE `StartTime` < '{$yesterdayBegin}'";
        $msg = "Sport {$sport} is now clean before yesterday";
    } else { // delete all records
        $sql = "DELETE FROM `{$table}` WHERE `StartTime` >= '{$beginDT}' AND `StartTime` < '{$endDT}'";
        $msg = "Sport {$sport} is now empty";
    }
    mysqlQuery($sql);
}
die($msg);
?>