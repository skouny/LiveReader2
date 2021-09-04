<?php // Start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
// Dependencies
require_once('database.php');
// Functions
function getDayId($date) {
    $datetime1 = date_create('2004-09-25');
    $datetime2 = date_create($date);
    $interval = date_diff($datetime1, $datetime2);
    return $interval->format('%a');
}
function getFootballUrl($dayId) {
    return "http://www.opap.gr/web/services/rs/betting/availableBetGames/sport/program/4100/0/sport-1/{$dayId}.xml?localeId=el_GR";
}
function readOpapXml($dayId) {
    $url = getFootballUrl($dayId);
    $xml = simplexml_load_file($url);
    return $xml;
}
function importCSV2DB($filename) {
    set_time_limit(900); // allow the script to run for 15 min (php default: 30 sec)
    $time_start = microtime(true);
    echo '<h3>Filename: '.$filename.'</h3>';
    $result = array('Identical' => 0
    , 'Identical-log' => ""
    , 'Changed' => 0
    , 'Changed-log' => ""
    , 'Inserted' => 0
    , 'Inserted-log' => ""
    , 'Error' => 0
    , 'Error-log' => ""
    );
    $row_index = 0;
    $col = array();
    if (($handle = fopen($filename, "r")) !== FALSE) {
        while (($row_data = fgetcsv($handle, 10000, ";")) !== FALSE) {
            $match = array();
            $match_odds = array();
            if ($row_index == 0) { // title line
                foreach ($row_data as $col_index => $col_name) {
                    $col[$col_name] = $col_index;
                }
            } else { // data line
                $match['Champ'] = $row_data[$col['Champ']];
                $match['StartTime'] = $row_data[$col['Date']].' '.$row_data[$col['StartTime']];
                $match['HomeTeam'] = $row_data[$col['HomeTeam']];
                $match['AwayTeam'] = $row_data[$col['AwayTeam']];
                $match['Status'] = $row_data[$col['Status']];
                $match['ScoreHomeHT'] = $row_data[$col['ScoreHomeHT']];
                $match['ScoreAwayHT'] = $row_data[$col['ScoreAwayHT']];
                $match['ScoreHomeFT'] = $row_data[$col['ScoreHomeFT']];
                $match['ScoreAwayFT'] = $row_data[$col['ScoreAwayFT']];
                $match['TV'] = $row_data[$col['TV']];
                $match_odds['BookId'] = 1;
                $match_odds['Code'] = $row_data[$col['Code']];
                $match_odds['EE'] = $row_data[$col['EE']];
                $match_odds['AdvHome'] = $row_data[$col['AdvHome']];
                $match_odds['AdvAway'] = $row_data[$col['AdvAway']];
                $match_odds['FT_1'] = $row_data[$col['Odd1']];
                $match_odds['FT_X'] = $row_data[$col['OddX']];
                $match_odds['FT_2'] = $row_data[$col['Odd2']];
                $match_odds['Under'] = $row_data[$col['Under']];
                $match_odds['Over'] = $row_data[$col['Over']];
                $match_odds['Goal'] = $row_data[$col['Goal']];
                $match_odds['NoGoal'] = $row_data[$col['NoGoal']];
                // Save match to DB
                $match['ChampId'] = getChampId($match['Champ']);
                if ($match['ChampId'] == 0) $match['ChampId'] = insertChamp($match['Champ'], '');
                $match['HomeTeamId'] = getTeamId($match['HomeTeam']);
                $match['AwayTeamId'] = getTeamId($match['AwayTeam']);
                switch (updateMatch($match, $match_odds)) {
                    case 0: // nothing to be done, match exists and up to date
                        $result['Identical'] += 1;
                        $result['Identical-log'] .= logMatch($match, $match_odds) . "<br />";
                        break;
                    case 1: // match exists but changed
                        $result['Changed'] += 1;
                        $result['Changed-log'] .= logMatch($match, $match_odds) . "<br />";
                        break;
                    case 2: // match not exist and inserted successfully
                        $result['Inserted'] += 1;
                        $result['Inserted-log'] .= logMatch($match, $match_odds) . "<br />";
                        break;
                    case 3: // one or more Id is missing, match can not be inserted or update it
                        $result['Error'] += 1;
                        $result['Error-log'] .= logMatch($match, $match_odds) . "<br />";
                        break;
                }
            }
            $row_index++;
        }
        fclose($handle);
    }
    $time_took = microtime(true) - $time_start;
    echo '<h4>Time took: '.number_format($time_took, 1).' sec</h4>';
    if ($result['Error'] > 0) {
        echo '<h3>Errors '.$result['Error'].'</h3>';
        echo $result['Error-log'];
    }
    if ($result['Inserted'] > 0) {
        echo '<h3>Inserted '.$result['Inserted'].'</h3>';
        echo $result['Inserted-log'];
    }
    if ($result['Changed'] > 0) {
        echo '<h3>Changed '.$result['Changed'].'</h3>';
        echo $result['Changed-log'];
    }
    if ($result['Identical'] > 0) {
        echo '<h3>Identical '.$result['Identical'].'</h3>';
        echo $result['Identical-log'];
    }
}
function importOpapXml2DB($dayId) {
    set_time_limit(300); // allow the script to run for 5 min (php default: 30 sec)
    $result = array('Identical' => 0
    , 'Identical-log' => ""
    , 'Changed' => 0
    , 'Changed-log' => ""
    , 'Inserted' => 0
    , 'Inserted-log' => ""
    , 'Error' => 0
    , 'Error-log' => ""
    , 'Unknown' => 0
    );
    $xml = readOpapXml($dayId);
    // Read all <betGames>
    foreach($xml->betGames as $betGame) {
        $match = array(); // match db record
        $match_odds = array(); // match_odds db record
        $match_extra = array(); // match extra info
        // Read StartTime
        $match['StartTime'] = date_create($betGame->betEndDate)->format('Y-m-d H:i:s');
        // Read Status
        switch (trim($betGame->attributes()['status'])) {
            case 'cancelled': $match['Status'] = 'Cancelled'; break;
            default: $match['Status'] = ''; break;
        }
        // Read all <properties>
        foreach($betGame->properties->prop as $prop) {
            $prop_attributes = $prop->attributes();
            $prop_id = trim($prop_attributes['id']);
            $prop_val = trim($prop);
            switch ($prop_id) {
                case "6": $match_odds['Code'] = $prop_val; break;
                case "11": $match['ScoreHomeHT'] = $prop_val; break;
                case "12": $match['ScoreAwayHT'] = $prop_val; break;
                case "13": $match['ScoreHomeFT'] = $prop_val; break;
                case "14": $match['ScoreAwayFT'] = $prop_val; break;

                case "20": $match_odds['AdvHome'] = $prop_val; break;
                case "21": $match_odds['AdvAway'] = $prop_val; break;
                case "28": $match_odds['EE'] = $prop_val; break;
                case "30":
                    foreach($betGame->lexicon->resources->entry as $entry) {
						if(trim($entry->key)==$prop_val) $match['HomeTeam'] = trim($entry->value);
					}
					break;
                case "31":
                    foreach($betGame->lexicon->resources->entry as $entry) {
						if(trim($entry->key)==$prop_val) $match['AwayTeam'] = trim($entry->value);
					}
					break;
                case "42": $match_extra['UnderOverBound'] = $prop_val; break;
                case "45": $match['TV'] = $prop_val; break;
                case "46":
                    foreach($betGame->lexicon->resources->entry as $entry) {
						if(trim($entry->key)==$prop_val) $match['Champ'] = trim($entry->value);
                        if(trim($entry->key)==trim(str_replace("-short","",$prop_val))) $match_extra['Championship'] = trim($entry->value);
					}
                    break;
            }
        }
        // Set status by score
        if (empty($match['Status'])) {
            if (isset($match['ScoreHomeFT']) && isset($match['ScoreAwayFT'])) {
                $match['Status'] = 'FT';
            } else {
                $result['Unknown'] += 1;
            }
        }
        // Read all <codes>
        foreach($betGame->codes as $codes) {
            $code_attributes = $codes->attributes();
            $code = trim($code_attributes['code']);
            $odd = trim($code_attributes['odd']);
            switch ($code) {
                case "0": $match_odds['FT_1'] = $odd; break;
                case "1": $match_odds['FT_X'] = $odd; break;
                case "2": $match_odds['FT_2'] = $odd; break;
                case "3": $match_odds['1orX'] = $odd; break;
                case "4": $match_odds['1or2'] = $odd; break;
                case "5": $match_odds['Xor2'] = $odd; break;
                case "6": break; // not used
                case "7": break; // not used
                case "8": break; // not used
                case "9": $match_odds['HT_1'] = $odd; break;
                case "10": $match_odds['HT_X'] = $odd; break;
                case "11": $match_odds['HT_2'] = $odd; break;
                case "12": $match_odds['1-1'] = $odd; break;
                case "13": $match_odds['X-1'] = $odd; break;
                case "14": $match_odds['2-1'] = $odd; break;
                case "15": $match_odds['1-X'] = $odd; break;
                case "16": $match_odds['X-X'] = $odd; break;
                case "17": $match_odds['2-X'] = $odd; break;
                case "18": $match_odds['1-2'] = $odd; break;
                case "19": $match_odds['X-2'] = $odd; break;
                case "20": $match_odds['2-2'] = $odd; break;
                case "21": $match_odds['Goals0-1'] = $odd; break;
                case "22": $match_odds['Goals2-3'] = $odd; break;
                case "23": $match_odds['Goals4-6'] = $odd; break;
                case "24": $match_odds['Goals7+'] = $odd; break;
                case "25": $match_odds['Under'] = $odd; break;
                case "26": $match_odds['Over'] = $odd; break;
                case "27": break; // not used
                case "28": break; // not used
                case "29": $match_odds['Goal'] = $odd; break;
                case "30": $match_odds['NoGoal'] = $odd; break;
                case "31": break; // not used
                case "32": break; // not used
                case "33": break; // not used
                case "34": break; // not used
                case "35": break; // not used
                case "36": $match_odds['Score0-0'] = $odd; break;
                case "37": $match_odds['Score1-0'] = $odd; break;
                case "38": $match_odds['Score2-0'] = $odd; break;
                case "39": $match_odds['Score3-0'] = $odd; break;
                case "40": $match_odds['Score4-0'] = $odd; break;
                case "41": $match_odds['Score5-0'] = $odd; break;
                case "42": $match_odds['Score2-1'] = $odd; break;
                case "43": $match_odds['Score3-1'] = $odd; break;
                case "44": $match_odds['Score4-1'] = $odd; break;
                case "45": $match_odds['Score5-1'] = $odd; break;
                case "46": $match_odds['Score3-2'] = $odd; break;
                case "47": $match_odds['Score4-2'] = $odd; break;
                case "48": $match_odds['Score5-2'] = $odd; break;
                case "49": $match_odds['Score4-3'] = $odd; break;
                case "50": $match_odds['Score5-3'] = $odd; break;
                case "51": $match_odds['Score5-4'] = $odd; break;
                case "52": $match_odds['Score1-1'] = $odd; break;
                case "53": $match_odds['Score2-2'] = $odd; break;
                case "54": $match_odds['Score3-3'] = $odd; break;
                case "55": $match_odds['Score4-4'] = $odd; break;
                case "56": $match_odds['Score5-5'] = $odd; break;
                case "57": $match_odds['Score4-5'] = $odd; break;
                case "58": $match_odds['Score3-4'] = $odd; break;
                case "59": $match_odds['Score3-5'] = $odd; break;
                case "60": $match_odds['Score2-3'] = $odd; break;
                case "61": $match_odds['Score2-4'] = $odd; break;
                case "62": $match_odds['Score2-5'] = $odd; break;
                case "63": $match_odds['Score1-2'] = $odd; break;
                case "64": $match_odds['Score1-3'] = $odd; break;
                case "65": $match_odds['Score1-4'] = $odd; break;
                case "66": $match_odds['Score1-5'] = $odd; break;
                case "67": $match_odds['Score0-1'] = $odd; break;
                case "68": $match_odds['Score0-2'] = $odd; break;
                case "69": $match_odds['Score0-3'] = $odd; break;
                case "70": $match_odds['Score0-4'] = $odd; break;
                case "71": $match_odds['Score0-5'] = $odd; break;
            }
        }
        // Save match to DB
        $match['ChampId'] = getChampId($match['Champ']);
        if ($match['ChampId'] == 0) $match['ChampId'] = insertChamp($match['Champ'], $match_extra['Championship']);
        $match['HomeTeamId'] = getTeamId($match['HomeTeam']);
        $match['AwayTeamId'] = getTeamId($match['AwayTeam']);
        switch (updateMatch($match, $match_odds)) {
            case 0: // nothing to be done, match exists and up to date
                $result['Identical'] += 1;
                $result['Identical-log'] .= logMatch($match, $match_odds) . "<br />";
                break;
            case 1: // match exists but changed
                $result['Changed'] += 1;
                $result['Changed-log'] .= logMatch($match, $match_odds) . "<br />";
                break;
            case 2: // match not exist and inserted successfully
                $result['Inserted'] += 1;
                $result['Inserted-log'] .= logMatch($match, $match_odds) . "<br />";
                break;
            case 3: // one or more Id is missing, match can not be inserted or update it
                $result['Error'] += 1;
                $result['Error-log'] .= logMatch($match, $match_odds) . "<br />";
                break;
        }
    }
    saveOpapXml2DB($dayId, $xml, $result['Error'], $result['Unknown']);
    // return
    return $result;
}
function saveOpapXml2DB($dayId, $xml, $errors, $unknown) {
    // Connect to database
	$mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, DATABASE);
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
    // Create command
    $programStartDate = date_create($xml->programStartDate)->format('Y-m-d H:i:s');
    $programEndDate = date_create($xml->programEndDate)->format('Y-m-d H:i:s');
    $betGames = $xml->betGames->count();
    if (empty($errors)) $errors = 0;
    $xmlFile = $mysqli->real_escape_string(gzencode($xml->asXML(), 9));
    $sql = "INSERT INTO `football_opap_xml` (`DayId`, `programStartDate`, `programEndDate`, `betGames`, `Errors`, `Unknown`, `File.xml.gz`) VALUES ('{$dayId}', '{$programStartDate}', '{$programEndDate}', '{$betGames}', '{$errors}', '{$unknown}', '{$xmlFile}')";
    $sql .= " ON DUPLICATE KEY UPDATE `DayId`='{$dayId}', `programStartDate`='{$programStartDate}', `programEndDate`='{$programEndDate}', `betGames`='{$betGames}', `Errors`='{$errors}', `Unknown`='{$unknown}', `File.xml.gz`='{$xmlFile}';";
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
function getOpapXmlFromDB($dayId) {
    $sql = "SELECT * FROM `football_opap_xml` WHERE `DayId`='{$dayId}';";
    return mysqlQuerySelectFirst($sql);
}
function getTeamId($teamName) {
    $sql = "SELECT GET_FOOTBALL_TEAM_ID('{$teamName}', 0) AS `Id`;";
    $query = mysqlQuerySelectFirst($sql);
    if (empty($query)) return 0; else return $query['Id'];
}
function getChampId($champName) {
    $sql = "SELECT GET_FOOTBALL_CHAMP_ID('{$champName}') AS `Id`;";
    $query = mysqlQuerySelectFirst($sql);
    if (empty($query)) return 0; else return $query['Id'];
}
function insertChamp($GrNameShort, $GrNameFull) {
    $sql = "INSERT INTO `football_champs` (`GrNameShort`,`GrNameFull`) VALUES ('{$GrNameShort}','{$GrNameFull}');";
    mysqlQuery($sql);
    return getChampId($GrNameShort);
}
function readMatch($match) {
    $sql = "SELECT * FROM `football_matches` WHERE DATE(`StartTime`)=DATE('{$match['StartTime']}') AND `ChampId`='{$match['ChampId']}' AND `HomeTeamId`='{$match['HomeTeamId']}' AND `AwayTeamId`='{$match['AwayTeamId']}'";
    return mysqlQuerySelectFirst($sql);
}
function updateMatch($match, $match_odds) {
    if ($match['ChampId'] != 0 && $match['HomeTeamId'] != 0 && $match['AwayTeamId'] != 0) {
        $existing = readMatch($match);
        if (empty($existing)) { // match not exist, so insert it with odds
            mysqlInsertOrUpdateFromArray('football_matches', $match);
            $existing = readMatch($match); // read to get the Id
            updateMatchOdds($existing, $match_odds);
            return 2;
        } else { // match exist
            $match['Id'] = $existing['Id'];
            // update odds every time (because they may changed)
            updateMatchOdds($existing, $match_odds);
            // check if match needs update
            if (isIdenticalMatches($existing, $match)) { // nothing to be done, match exists and up to date
                return 0;
            } else { // match exists but changed
                mysqlInsertOrUpdateFromArray('football_matches', $match); // update the match
                return 1;
            }
        }
    } else { // one or more Id is missing, match can not be inserted or update it
        return 3;
    }
}
function updateMatchOdds($match, $match_odds) {
    $match_odds['MatchId'] = $match['Id'];
    $match_odds['BookId'] = 1;
    mysqlInsertOrUpdateFromArray('football_matches_odds', $match_odds);
}
function isIdenticalMatches($match1, $match2) {
    if (!isIdenticalValues($match1, $match2, 'StartTime')) return FALSE;
    if (!isIdenticalValues($match1, $match2, 'Champ')) return FALSE;
    if (!isIdenticalValues($match1, $match2, 'HomeTeam')) return FALSE;
    if (!isIdenticalValues($match1, $match2, 'AwayTeam')) return FALSE;
    if (!isIdenticalValues($match1, $match2, 'ScoreHomeHT')) return FALSE;
    if (!isIdenticalValues($match1, $match2, 'ScoreAwayHT')) return FALSE;
    if (!isIdenticalValues($match1, $match2, 'ScoreHomeFT')) return FALSE;
    if (!isIdenticalValues($match1, $match2, 'ScoreAwayFT')) return FALSE;
    if (!isIdenticalValues($match1, $match2, 'Status')) return FALSE;
    if (!isIdenticalValues($match1, $match2, 'TV')) return FALSE;
    return TRUE;
}
function isIdenticalValues($match1, $match2, $index) {
    if (!isset($match1[$index]) && !isset($match2[$index])) return TRUE;
    if (isset($match1[$index]) && isset($match2[$index]) && $match1[$index] == $match2[$index]) return TRUE;
    return FALSE;
}
function logMatch($match, $match_odds) {
    $StartTime = date_create($match['StartTime'])->format('Y-m-d H:i');
    $ScoreHomeFT = isset($match['ScoreHomeFT']) ? $match['ScoreHomeFT'] : '';
    $ScoreAwayFT = isset($match['ScoreAwayFT']) ? $match['ScoreAwayFT'] : '';
    $ScoreHomeHT = isset($match['ScoreHomeHT']) ? $match['ScoreHomeHT'] : '';
    $ScoreAwayHT = isset($match['ScoreAwayHT']) ? $match['ScoreAwayHT'] : '';
    return "{$StartTime}#{$match_odds['Code']}: {$match['Champ']}, {$match['HomeTeam']} - {$match['AwayTeam']}"
        . " {$ScoreHomeFT}-{$ScoreAwayFT} ({$ScoreHomeHT}-{$ScoreAwayHT}), {$match['Status']}";
}
// Update one day and get the result in json
if (!empty($_GET['dayid']) || !empty($_GET['date'])) {
    header('Content-Type: application/json; charset=utf-8');
    $dayId = $_GET['dayid'];
    if (empty($dayId)) $dayId = getDayId($_GET['date']);
    echo json_encode(importOpapXml2DB($dayId));
}
else if (!empty($_GET['file'])) { // download opap xml from db
    header('Content-Type: text/xml; charset=utf-8');
    $dayId = $_GET['file'];
    $opapXml = getOpapXmlFromDB($dayId);
    echo gzdecode($opapXml['File.xml.gz']);
}
else if (!empty($_GET['info'])) {  // get opap xml db record
    header('Content-Type: application/json; charset=utf-8');
    $dayId = $_GET['info'];
    $opapXml = getOpapXmlFromDB($dayId);
    $info = array();
    $info['games'] = $opapXml['betGames'];
    $info['errors'] = $opapXml['Errors'];
    $info['unknown'] = $opapXml['Unknown'];
    $info['bytes'] = strlen($opapXml['File.xml.gz']);
    $info['modified'] = $opapXml['Modified'];
    echo json_encode($info);
}
else { header('Content-Type: text/html; charset=utf-8');
    $beginDate = date_create('2010-09-11');
    $endDate = date_create(date('Y-m-d')); // today
    date_add($endDate, date_interval_create_from_date_string('10 days')); // add 10 days
?>
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="UTF-8">
        <title>Opap Xml</title>
        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
        <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/jquery-ui.min.js"></script>
        <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
        <script>
            $(function () {
                $("#menu").menu();
            });
            function update(dayId) {
                $("#label" + dayId).html("Updating...");
                var request = $.ajax({
                    async: "true",
                    url: "?dayid=" + dayId,
                    type: "GET",
                    cache: "false",
                    dataType: "json"
                });
                request.done(function (responce) {
                    try {
                        $("#dialog" + dayId).html(writeDayResponce(responce));
                        $("#label" + dayId).html("OK");
                        updateDayInfo(dayId);
                    } catch (err) {
                        $("#dialog" + dayId).html(responce);
                        $("#label" + dayId).html(err);
                    } finally {
                        $("#dialog" + dayId).dialog({
                            autoOpen: true
                            , width: 'auto'
                            , height: 'auto'
                        });
                    }
                });
                request.fail(function (jqXHR, textStatus) {
                    $("#label" + dayId).html(textStatus);
                });
                request.always(function () {
                    //
                })
            }
            function writeDayResponce(result) {
                var s = "";
                if (result['Error'] > 0) {
                    s += "<h3>Error: " + result['Error'] + "</h3>";
                    s += result['Error-log'];
                }
                if (result['Inserted'] > 0) {
                    s += "<h3>Inserted: " + result['Inserted'] + "</h3>";
                    s += result['Inserted-log'];
                }
                if (result['Changed'] > 0) {
                    s += "<h3>Changed: " + result['Changed'] + "</h3>";
                    s += result['Changed-log'];
                }
                if (result['Identical'] > 0) {
                    s += "<h3>Identical: " + result['Identical'] + "</h3>";
                    s += result['Identical-log'];
                }
                return s;
            }
            function updateDayInfo(dayId) {
                var request = $.ajax({
                    async: "true",
                    url: "?info=" + dayId,
                    type: "GET",
                    cache: "false",
                    dataType: "json"
                });
                request.done(function (responce) {
                    $("#games" + dayId).html(responce['games']);
                    $("#errors" + dayId).html(responce['errors']);
                    $("#unknown" + dayId).html(responce['unknown']);
                    $("#bytes" + dayId).html(responce['bytes'] + " bytes");
                    $("#modified" + dayId).html(responce['modified']);
                });
                request.fail(function (jqXHR, textStatus) {
                    $("#label" + dayId).html(textStatus);
                });
                request.always(function () {
                    //
                })
            }
        </script>
        <style>
            body, button, ul, li, td {
                font-size: 12px;
                font-family: Verdana;
            }
            ul#menu {
                border: 0px;
                margin-bottom: 10px;
            }
            li {
                display: inline;
            }
            table {
                border-collapse: collapse;
                border-spacing: 0px;
                white-space: nowrap;
            }
            table, td, th {
                border: 1px solid black;
            }
            tr {
                background-color: lightgray;
            }
            tr:hover {
                background-color: khaki;
            }
            th {
                background-color: gray;
            }
            td, th {
                padding-left: 4px;
                padding-right: 4px;
            }
            .center {
                text-align: center;
            }
            .left {
                text-align: left;
            }
            .right {
                text-align: right;
            }
            .hidden {
                display: none;
            }
            .scroll {
                overflow: scroll;
            }
        </style>
    </head>
    <body>
        <ul id="menu">
            <?php
                $endYear = intval(date_format($endDate, "Y"));
                for ($i = $endYear; $i >= 2010; $i--) {
                    echo "<li><a href=\"?year={$i}\">{$i}</a></li>";
                }
            ?>
            <li><a href="?import">Import</a></li>
        </ul>
        <?php if (isset($_GET['import'])) {
            echo '<h2>Import has been deactivated in code</h2>';
            //importCSV2DB('C:\\Users\\Yiannis\\Downloads\\matches2000-01.csv');
        } else { ?>
        <table>
            <tr>
                <th>Date</th>
                <th>DayId</th>
                <th>Games</th>
                <th>Errors</th>
                <th>Unknown</th>
                <th>File.xml.gz</th>
                <th>Modified</th>
                <th></th>
                <th></th>
            </tr>
            <?php
                if (empty($_GET['year'])) $year = date("Y"); else $year = $_GET['year'];
                for ($month=12;$month>=1;$month--) {
                    $days = cal_days_in_month(CAL_GREGORIAN, $month, $year);
                    for ($day=$days;$day>=1;$day--) {
                        $iDate = date_create("{$year}-{$month}-{$day}");
                        $date = $iDate->format('Y-m-d');
                        $dayId = getDayId($date);
                        $opapXml = getOpapXmlFromDB($dayId);
                        $dataLength = strlen($opapXml['File.xml.gz']);
                        if ($iDate >= $beginDate && $iDate <= $endDate) {
                            $dayText = date('Y-m-d D',strtotime($date));
                            echo '<tr>';
                            echo "<td class=\"left\">{$dayText}</td>";
                            echo "<td class=\"center\"><a href=\"http://www.opap.gr/web/services/rs/betting/availableBetGames/sport/program/4100/0/sport-1/{$dayId}.xml?localeId=el_GR\" target=\"_blank\">{$dayId}</a></td>";
                            echo "<td class=\"center\" id=\"games{$dayId}\">{$opapXml['betGames']}</td>";
                            echo "<td class=\"center\" id=\"errors{$dayId}\">{$opapXml['Errors']}</td>";
                            echo "<td class=\"center\" id=\"unknown{$dayId}\">{$opapXml['Unknown']}</td>";
                            echo "<td class=\"right\"><a href=\"?file={$dayId}\" id=\"bytes{$dayId}\" target=\"_blank\">{$dataLength} bytes</a></td>";
                            echo "<td class=\"center\" id=\"modified{$dayId}\">{$opapXml['Modified']}</td>";
                            echo "<td class=\"center\"><button onclick=\"update({$dayId})\">Update</button></td>";
                            echo "<td class=\"center\"><label id=\"label{$dayId}\"></label><div id=\"dialog{$dayId}\" class=\"hidden scroll\" title=\"Date: {$date}\"></div></td>";
                            echo '</tr>';
                        }
                    }
                }
            ?>
        </table>
        <?php } ?>
    </body>
</html>
<?php }
// End gzip compressing
while (@ob_end_flush());
?>
