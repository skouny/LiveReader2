<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
function getOpapLive() {
    $MixedDict = array();
    $Matches = GetMatches('football_live_mix');
    foreach ($Matches as $PairId => $Sources) {
		if (isset($Sources['Opap'])) {
            $Code = $Sources['Opap']['WebId'];
			$Mixed = GetMixedMatchFootball($Sources['Opap'], $Sources['FlashScore'], $Sources['Futbol24'], $Sources['NowGoal']);
            $MixedDict[$Code] = $Mixed;
        }
    }
    return $MixedDict;
}
function updatePlay90($name, $xml) {
	// Connect to database
	$mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, 'play90');
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
	// Prepare vars
	$data = $mysqli->real_escape_string($xml);
	$length = strlen($xml);
    // Create command
    $sql = "INSERT INTO `xml_hacker` (`Name`,`Data`,`Length`)";
	$sql .= " VALUES ('{$name}','{$data}','{$length}')";
	$sql .= " ON DUPLICATE KEY UPDATE `Data`=VALUES(`Data`),`Length`=VALUES(`Length`);";
	// Execute the command
	$query = $mysqli->query($sql);
	// If execution failed
	if ($mysqli->errno) {
        echo "<pre>";
        echo "Failed to execute query!\n\n";
        echo "ERROR ({$mysqli->errno}): {$mysqli->error}\n\n";
        echo "SQL COMMAND: {$sql}";
        die("</pre>");
	}
	// Close the connection
	$mysqli->close();
}
?>
<?php
require_once('../livereader2/database.php');
require_once('../livereader2/mix.php');
error_reporting(0);
date_default_timezone_set('Europe/Athens');
if (isset($_GET['day'])) {
    $day = new DateTime($_GET['day']);
} else {
    $hour_now = intval(date("H"));
    if ($hour_now >= 8) {
        $day = new DateTime(date('Y-m-d', strtotime("now")));
    } else {
        $day = new DateTime(date('Y-m-d', strtotime("-1 day")));
    }
}
$filename = $day->format('Y-m-d').'.xml';
$startTimeFrom = $day->format('Y-m-d') . " 08:00:00";
$startTimeTo = $day->modify('+1 day')->format('Y-m-d') . " 07:59:59";
$matches = mysqlQuerySelect("SELECT * FROM `football_opap_matches` WHERE `StartTime` >= '{$startTimeFrom}' AND `StartTime` <= '{$startTimeTo}' AND `CommentFlag`=1;");
$date_now = new DateTime();
$time_now = strtotime("now");
if ($time_now >= strtotime($startTimeFrom) && $time_now <= strtotime($startTimeTo)) {
    $opap_live = getOpapLive();
}
$writer = new XMLWriter();
//$writer->openURI('php://output');
$writer->openMemory();
$writer->startDocument('1.0','UTF-8');
$writer->setIndent(4);
$writer->startElement('List');
// Start Loop
foreach ($matches as $match) {
    $code = $match['Code'];
    if (isset($opap_live)) {
        $live = $opap_live[$code];
        $ScoreHT = $live['ScoreHT'];
        $Score = $live['Score'];
        $Minute = $live['Minute'];
        $Status = $live['StatusId'];
    } else {
        $ScoreHT = $match['ScoreHomeHT'] . '-' . $match['ScoreAwayHT'];
        $Score = $match['ScoreHomeFT'] . '-' . $match['ScoreAwayFT'];
        $Minute = '';
        $Status = (strlen($Score) > 1) ? 'FT' : '';
    }
    $writer->startElement('Match');
        $writer->writeAttribute('Champ', $match['Champ']);
        $writer->writeAttribute('StartTime', date_create($match['StartTime'])->format('Y-m-d\TH:i:s'));
		$writer->writeAttribute('Code', $code);
		$writer->writeAttribute('HomeTeam', $match['HomeTeam']);
		$writer->writeAttribute('AwayTeam', $match['AwayTeam']);
        $writer->writeAttribute('ScoreHT', $ScoreHT);
        $writer->writeAttribute('Score', $Score);
        $writer->writeAttribute('Minute', $Minute);
        $writer->writeAttribute('Status', $Status);
        $writer->writeAttribute('Comment', $match['Comment']);
        $writer->writeAttribute('CommentOdd', $match['CommentOdd']);
        $writer->writeAttribute('CommentStatus', $match['CommentStatus']);
	$writer->endElement();
}
// end loop
$writer->endElement();
$writer->endDocument();
$xml = $writer->outputMemory(TRUE);
$writer->flush();
if (isset($_GET['play90'])) {
	header('Content-Type: text/plain; charset=utf-8');
	updatePlay90($filename,$xml);
	echo 'Hacker file updated => ' . $filename;
} else {
	header('Content-Type: text/xml; charset=utf-8');
	echo $xml;
}
?>
<?php // end gzip compressing
  while (@ob_end_flush());
?>