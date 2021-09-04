<?php
require_once('database.php');
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Set headers
header('Content-Type: text/xml; charset=utf-8');
// Write xml
writeTableXml("SELECT * FROM `sofa_football_leagues`", function($writer, $row) {
	$writer->startElement('Champ');
	$writer->writeAttribute('Id', $row['ChampId']);
    $writer->writeAttribute('Name', $row['ChampName']);
    $writer->writeAttribute('GroupIndex', $row['GroupIndex']);
    $writer->writeAttribute('GroupName', $row['GroupName']);
    $writer->writeAttribute('Url', $row['Url']);
    $writer->writeAttribute('ContentIndex', $row['ContentIndex']);
	$writer->endElement();
});
?>