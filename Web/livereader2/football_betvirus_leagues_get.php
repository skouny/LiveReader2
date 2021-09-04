<?php
require_once('database.php');
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Set headers
header('Content-Type: text/xml; charset=utf-8');
// Write xml
writeTableXml("SELECT * FROM `football_betvirus_leagues`", function($writer, $row) {
	$writer->startElement('League');
	$writer->writeAttribute('Id', $row['Id']);
	$writer->endElement();
});
?>