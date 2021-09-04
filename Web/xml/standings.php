<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
require_once('../livereader2/database.php');
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Set headers
header('Content-Type: text/xml; charset=utf-8');
// Connect to database
$mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, DATABASE);
// If connection failed die...
if ($mysqli->connect_errno) {
	die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
}
// Change character set to utf8
$mysqli->set_charset('utf8');
// Create command
$sql = "SELECT * FROM `sofa_football_leagues`";
// Execute the command
$result = $mysqli->query($sql);
// If execution failed
if ($mysqli->errno) {
	die('Failed to execute query: (' . $mysqli->errno . ') ' . $mysqli->error);
}
$champs = array();
while($row = $result->fetch_array()) {
    $champs[] = $row;
}
// free result set
$result->free();
// write xml
$writer = new XMLWriter();
$writer->openURI('php://output');
$writer->startDocument('1.0','UTF-8');
$writer->setIndent(4);
$writer->startElement('Standings');
foreach ($champs as $champ) {
    // start champ element
    $writer->startElement('Champ');
	$writer->writeAttribute('Name', $champ['ChampName']);
    $writer->writeAttribute('GroupIndex', $champ['GroupIndex']);
    $writer->writeAttribute('GroupName', $champ['GroupName']);
	$writer->writeAttribute('Promotion', $champ['Promotion']);
	$writer->writeAttribute('PromotionPlayoff', $champ['PromotionPlayoff']);
	$writer->writeAttribute('EuropaLeague', $champ['EuropaLeague']);
	$writer->writeAttribute('EuropaLeaguePlayoff', $champ['EuropaLeaguePlayoff']);
	$writer->writeAttribute('RelegationPlayoff', $champ['RelegationPlayoff']);
	$writer->writeAttribute('Relegation', $champ['Relegation']);
    // get the teams of each champ
    $sql = "SELECT * FROM `sofa_football_standings` WHERE (`ChampId` = {$champ['ChampId']} AND `GroupIndex` = {$champ['GroupIndex']})";
    // Execute the command
    $result = $mysqli->query($sql);
    // If execution failed
    if ($mysqli->errno) {
	    die('Failed to execute query: (' . $mysqli->errno . ') ' . $mysqli->error);
    }
    while($team = $result->fetch_array()) {
        writeTeamElement($writer, $team);
    }
    // free result set
    $result->free();
    // end champ element
    $writer->endElement();
}
$writer->endElement();
$writer->endDocument();
$writer->flush();
// Close the connection
$mysqli->close();

// Write Team function
function writeTeamElement($writer, $team) {
	$writer->startElement('Team');
    if (empty($team['TeamNameGr'])) {
        $writer->writeAttribute('Name', htmlspecialchars_decode($team['TeamName']));
    } else {
        $writer->writeAttribute('Name', htmlspecialchars_decode($team['TeamNameGr']));
    }
	$writer->writeAttribute('TeamId', $team['TeamId']);
	// Overall
	$writer->writeAttribute('Place', $team['Place']);
	$writer->writeAttribute('TotalGames', $team['TotalGames']);
	$writer->writeAttribute('TotalPoints', $team['TotalPoints']);
	$writer->writeAttribute('TotalWins', $team['TotalWins']);
	$writer->writeAttribute('TotalDraws', $team['TotalDraws']);
	$writer->writeAttribute('TotalLosts', $team['TotalLosts']);
	$writer->writeAttribute('TotalGoalsFor', $team['TotalGoalsFor']);
	$writer->writeAttribute('TotalGoalsAgainst', $team['TotalGoalsAgainst']);
	$writer->writeAttribute('TotalGoalDeference', $team['TotalGoalDeference']);
	// Home
	$writer->writeAttribute('PlaceHome', $team['PlaceHome']);
	$writer->writeAttribute('HomeGames', $team['HomeGames']);
	$writer->writeAttribute('HomePoints', $team['HomePoints']);
	$writer->writeAttribute('HomeWins', $team['HomeWins']);
	$writer->writeAttribute('HomeDraws', $team['HomeDraws']);
	$writer->writeAttribute('HomeLosts', $team['HomeLosts']);
	$writer->writeAttribute('HomeGoalsFor', $team['HomeGoalsFor']);
	$writer->writeAttribute('HomeGoalsAgainst', $team['HomeGoalsAgainst']);
	$writer->writeAttribute('HomeGoalDeference', $team['HomeGoalDeference']);
	// Away
	$writer->writeAttribute('PlaceAway', $team['PlaceAway']);
	$writer->writeAttribute('AwayGames', $team['AwayGames']);
	$writer->writeAttribute('AwayPoints', $team['AwayPoints']);
	$writer->writeAttribute('AwayWins', $team['AwayWins']);
	$writer->writeAttribute('AwayDraws', $team['AwayDraws']);
	$writer->writeAttribute('AwayLosts', $team['AwayLosts']);
	$writer->writeAttribute('AwayGoalsFor', $team['AwayGoalsFor']);
	$writer->writeAttribute('AwayGoalsAgainst', $team['AwayGoalsAgainst']);
	$writer->writeAttribute('AwayGoalDeference', $team['AwayGoalDeference']);
	// Extra
	$writer->writeAttribute('FormInfo', $team['FormInfo']);
	$writer->writeAttribute('Matches', $team['Matches']);
	//$matches = simplexml_load_string($team['Matches']);
	//$writer->startElement('Matches');
	//$writer->writeRaw($team['Matches']);
	//$writer->endElement();
	$writer->endElement();
}
?>
<?php // end gzip compressing
  while (@ob_end_flush());
?>