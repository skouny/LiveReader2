<?php
require_once('database.php');
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Set headers
header('Content-Type: text/xml; charset=utf-8');
// Write xml
$sql="SELECT * FROM `football_betvirus_standings` WHERE `LeagueId`='".$_GET['LeagueId']."' AND TeamName='".$_GET['TeamName']."';";
writeTableXml($sql, function($writer, $row) {
	$writer->startElement('Team');
	$writer->writeAttribute('Id', $row['Id']);
	$writer->writeAttribute('LeagueId', $row['LeagueId']);
	$writer->writeAttribute('TeamName', $row['TeamName']);
	$writer->writeAttribute('TeamId', $row['TeamId']);
	$writer->endElement();
});
?>