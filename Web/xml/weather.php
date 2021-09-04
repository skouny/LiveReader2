<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
require_once('../livereader2/database.php');
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Functions
function PrintWeatherXml($teams) {
    header('Content-Type: text/xml; charset=utf-8');
    $writer = new XMLWriter();
    $writer->openURI('php://output');
    $writer->startDocument('1.0','UTF-8');
    $writer->setIndent(4);
    $writer->startElement('Weather');
    foreach (explode(',', $teams) as $team) {
        if (!empty($team)) {
            $writer->startElement('Team');
            $writer->writeAttribute('Name', $team);
            $writer->writeRaw(OpenWeatherMap($team));
            $writer->endElement();
        }
    }
    $writer->endElement();
    $writer->endDocument();
    $writer->flush();
}
function OpenWeatherMap($HomeTeam) {
    // Load existing record
    $record = mysqlQuerySelectFirst("SELECT * FROM `football_teams_grounds` WHERE `GrName`='{$HomeTeam}' LIMIT 1;");
    // if not exists create an empty one
    if (!empty($record)) {
        // if LastUpdated < 10 min, return the xml
        if (datetimeDiffFromNow($record['WeatherUpdate']) < 600) {
            $xml = $record['WeatherXml'];
        } else {
            // else, download & return fresh xml
            $xml = OpenWeatherMapRefresh($record);
        }
        return $xml;
    }
}
function OpenWeatherMapRefresh($record) { // download fresh xml
    $url = OpenWeatherUrl($record);
    if (!empty($url)) {
        $doc = simplexml_load_file($url);
        if (!empty($doc)) {
            foreach ($doc as $key => $value) {
                $xml .= $value->asXML() . "\n";
            }
            $now = (new DateTime())->format('Y-m-d H:i:s');
            // update record and return the new xml
            if (!empty($xml)) {
                mysqlQuery("UPDATE `football_grounds` SET `WeatherXml`='{$xml}',`WeatherUpdate`='{$now}' WHERE `Id`='{$record['Id']}';");
                return $xml;
            }
            // if anything wrong return the old xml
            if (!empty($record['WeatherXml'])) {
                return $record['WeatherXml'];
            }
        }
    }
    return '';
}
function OpenWeatherUrl($record) {
    $appid = '74abe769088422691fba18ab066e1a14';
    if (!empty($record['Latitude']) && !empty($record['Longitude'])) {
        $url = "http://api.openweathermap.org/data/2.5/forecast/city?mode=xml&units=metric&lat={$record['Latitude']}&lon={$record['Longitude']}&APPID={$appid}";
        return $url;
    }
    if (!empty($record['Location'])) {
        return "http://api.openweathermap.org/data/2.5/forecast/city?mode=xml&units=metric&q={$record['Location']}&APPID={$appid}";
    }
    return '';
}
function datetimeDiffFromNow($time) {
    $now = (new DateTime())->format('Y-m-d H:i:s');
    $datetime1 = strtotime($now);
    $datetime2 = strtotime($time);
    $interval = $datetime1 - $datetime2;
    return $interval;
}
//$teams = 'ΜΟΝΑΡΚΑΣ,ΣΙΑΤΛ';
$teams = $_POST['teams'];
PrintWeatherXml($teams);
?>
<?php // end gzip compressing
  while (@ob_end_flush());
?>