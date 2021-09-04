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
// Functions
function GoalCount($score) {
    if (!empty($score)) {
        $goals = explode('-', $score);
        if (count($goals) == 2) {
            return intval($goals[0]) + intval($goals[1]);
        }
    }
    return 0;
}
// Read data
$matches = mysqlQuerySelect("SELECT * FROM `football_live_mix` WHERE `Source` LIKE 'NowGoal';");
//print_r($matches);die();
// Write XML
$writer = new XMLWriter();
$writer->openURI('php://output');
$writer->startDocument('1.0','UTF-8');
$writer->setIndent(4);
$writer->startElement('List');
// Start Loop
foreach ($matches as $match) {
    $status = $match['StatusId'];
    $goals = GoalCount($match['Score']);
    if ($goals == 1 && ($status == 'Half1' || $status == 'HT') ) {
        $writer->startElement('Match');
            $writer->writeAttribute('Id', $match['WebId']);
            $writer->writeAttribute('StartTime', date_create($match['StartTime'])->format('Y-m-d\TH:i:s'));
		    $writer->writeAttribute('Champ', $match['Champ']);
		    $writer->writeAttribute('HomeTeam', $match['HomeTeam']);
		    $writer->writeAttribute('AwayTeam', $match['AwayTeam']);
            $writer->writeAttribute('Score', $match['Score']);
            $writer->writeAttribute('Status', $status);
            $writer->writeAttribute('BetType', "Over {$goals}.5");
            $writer->writeAttribute('BetMoney', '3€');
	    $writer->endElement();
    }
}
// end loop
$writer->endElement();
$writer->endDocument();
$writer->flush();
?>
<?php // end gzip compressing
  while (@ob_end_flush());
?>