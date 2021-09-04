<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
date_default_timezone_set("Europe/Athens");
header('Content-Type: text/xml; charset=utf-8');
//header('Content-Type: application/json; charset=utf-8');
$dayId = (isset($_GET['d'])) ? $_GET['d'] : '';
$url = "http://www.opap.gr/web/services/rs/betting/availableBetGames/sport/program/4100/0/sport-1/{$dayId}.json";
$context = stream_context_create(
    array('http' =>
        array(
            'method'  => 'GET',
            'header'  => 'LOCALE: en_GB'
        )
    )
);
$result = file_get_contents($url, FALSE, $context);
$json = json_decode($result);
//print_r($json); die();
?>
<?php
// Write XML
$writer = new XMLWriter();
$writer->openURI('php://output');
$writer->startDocument('1.0','UTF-8');
$writer->setIndent(4);
$writer->startElement('Matches');
// Start Loop
foreach ($json->betGames as $match) {
    $writer->startElement('Match');
        $writer->writeAttribute('StartTime', date('Y-m-d H:i', $match->betEndDate / 1000));
        // properties
        foreach ($match->properties->prop as $prop) {
            switch($prop->id) {
                case 6:
                    $writer->writeAttribute('Code', $prop->value);
                    break;
                case 11:
                    $writer->writeAttribute('ScoreHomeHT', $prop->value);
                    break;
                case 12:
                    $writer->writeAttribute('ScoreAwayHT', $prop->value);
                    break;
                case 13:
                    $writer->writeAttribute('ScoreHomeFT', $prop->value);
                    break;
                case 14:
                    $writer->writeAttribute('ScoreAwayFT', $prop->value);
                    break;
                case 30:
                    $id = $prop->value;
                    $writer->writeAttribute('AwayTeam', $match->lexicon->resources->$id);
                    break;
                case 31:
                    $id = $prop->value;
                    $writer->writeAttribute('HomeTeam', $match->lexicon->resources->$id);
                    break;
                case 46:
                    $id = $prop->value;
                    $id2 = str_replace('-short','',$id);
                    $writer->writeAttribute('Country', $match->lexicon->resources->$id2);
                    $writer->writeAttribute('Champ', $match->lexicon->resources->$id);
                    break;
            }
        }
        // codes
        foreach ($match->codes as $code) {
            switch($code->code->value) {
                case 0:
                    $writer->writeAttribute('Odd1', $code->odd);
                    break;
                case 1:
                    $writer->writeAttribute('OddX', $code->odd);
                    break;
                case 2:
                    $writer->writeAttribute('Odd2', $code->odd);
                    break;
                case 25:
                    $writer->writeAttribute('Under', $code->odd);
                    break;
                case 26:
                    $writer->writeAttribute('Over', $code->odd);
                    break;
                case 29:
                    $writer->writeAttribute('GoalGoal', $code->odd);
                    break;
                case 30:
                    $writer->writeAttribute('NoGoal', $code->odd);
                    break;
            }
        }
        $writer->writeAttribute('Status', $match->status);
	$writer->endElement();
}
// end loop
$writer->endElement();
$writer->endDocument();
$writer->flush();
?>
<?php // end gzip compressing
  while (@ob_end_flush());
?>
