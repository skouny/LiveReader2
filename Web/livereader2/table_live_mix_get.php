<?php
require_once('database.php');
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Set headers
header('Content-Type: text/xml; charset=utf-8');
// Get
$sport = strtolower($_GET['sport']);
$source = $_GET['source'];
$sql = "SELECT * FROM `{$sport}_live_mix` WHERE `Source` LIKE '{$source}'";
// Write xml
if ($sport == 'football') {
	writeTableXml($sql, function($writer, $row) {
		$writer->startElement('Match');
		$writer->writeAttribute('Id', $row['Id']);
		$writer->writeAttribute('Source', $row['Source']);
		$writer->writeAttribute('WebId', $row['WebId']);
		$writer->writeAttribute('StartTime', $row['StartTime']);
		$writer->writeAttribute('Champ', $row['Champ']);
		$writer->writeAttribute('HomeTeam', $row['HomeTeam']);
		$writer->writeAttribute('AwayTeam', $row['AwayTeam']);
		$writer->writeAttribute('Score', $row['Score']);
		$writer->writeAttribute('ScoreHT', $row['ScoreHT']);
		$writer->writeAttribute('Minute', $row['Minute']);
		$writer->writeAttribute('Status', $row['Status']);
		$writer->writeAttribute('YellowCards', $row['YellowCards']);
		$writer->writeAttribute('RedCards', $row['RedCards']);
		$writer->writeAttribute('CornerKicks', $row['CornerKicks']);
		$writer->writeAttribute('Shots', $row['Shots']);
		$writer->writeAttribute('ShotsOnGoal', $row['ShotsOnGoal']);
		$writer->writeAttribute('Fouls', $row['Fouls']);
		$writer->writeAttribute('BallPossession', $row['BallPossession']);
		$writer->writeAttribute('Saves', $row['Saves']);
		$writer->writeAttribute('Offsides', $row['Offsides']);
		$writer->writeAttribute('KickOff', $row['KickOff']);
		$writer->writeAttribute('HomeEvents', $row['HomeEvents']);
		$writer->writeAttribute('AwayEvents', $row['AwayEvents']);
		$writer->writeAttribute('HomeScored', $row['HomeScored']);
		$writer->writeAttribute('AwayScored', $row['AwayScored']);
		$writer->writeAttribute('Modified', $row['Modified']);
		$writer->endElement();
	});
}
else if ($sport == 'basket') {
	writeTableXml($sql, function($writer, $row) {
		$writer->startElement('Match');
		$writer->writeAttribute('Id', $row['Id']);
		$writer->writeAttribute('Source', $row['Source']);
		$writer->writeAttribute('WebId', $row['WebId']);
		$writer->writeAttribute('StartTime', $row['StartTime']);
		$writer->writeAttribute('Champ', $row['Champ']);
		$writer->writeAttribute('HomeTeam', $row['HomeTeam']);
		$writer->writeAttribute('AwayTeam', $row['AwayTeam']);
		$writer->writeAttribute('Score', $row['Score']);
		$writer->writeAttribute('ScoreHT', $row['ScoreHT']);
		$writer->writeAttribute('ScoreQ1', $row['ScoreQ1']);
		$writer->writeAttribute('ScoreQ2', $row['ScoreQ2']);
		$writer->writeAttribute('ScoreQ3', $row['ScoreQ3']);
		$writer->writeAttribute('ScoreQ4', $row['ScoreQ4']);
		$writer->writeAttribute('StandingPoints', $row['StandingPoints']);
		$writer->writeAttribute('Minute', $row['Minute']);
		$writer->writeAttribute('Status', $row['Status']);
		$writer->writeAttribute('HomeScored', $row['HomeScored']);
		$writer->writeAttribute('AwayScored', $row['AwayScored']);
		$writer->writeAttribute('Modified', $row['Modified']);
		$writer->endElement();
	});
}
?>