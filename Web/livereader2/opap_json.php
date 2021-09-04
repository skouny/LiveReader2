<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php // Dependances
require_once('database.php');
?>
<?php // Error reporting
//error_reporting(E_ALL);
//ini_set('display_errors', 'On');
?>
<?php // Functions
function currentDay() {
    $dt = new DateTime();
    $hour = intval($dt->format('H'));
    if ($hour < 8) $dt->modify('-1 day');
    return $dt->format('Y-m-d');
}
function getDayId($date=NULL) {
    if (empty($date)) $date = currentDay();
    $datetime1 = date_create('2015-08-20');
    $datetime2 = date_create($date);
    $interval = date_diff($datetime1, $datetime2);
    return $interval->format('%a');
}
function getTeamId($teamName) {
    // Connect to database
	$mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, 'sports');
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
    // Create command
    $value = $mysqli->real_escape_string($teamName);
    $sql = "SELECT TEAM_ID('1', 0, '{$value}') AS `Id`;";
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
    $rows = array();
    while($row = $query->fetch_assoc()) {
        $rows[] = $row;
    }
	// free query set
	$query->free();
	// Close the connection
	$mysqli->close();
	// return
    if (count($rows) > 0) {
        $result = $rows[0];
        return $result['Id'];
    } else {
        return 0;
    }
}
function getChampId($champName) {
    // Connect to database
	$mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, 'sports');
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
    // Create command
    $value = $mysqli->real_escape_string($champName);
    $sql = "SELECT CHAMP_ID('1', '0', '{$champName}') AS `Id`;";
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
    $rows = array();
    while($row = $query->fetch_assoc()) {
        $rows[] = $row;
    }
	// free query set
	$query->free();
	// Close the connection
	$mysqli->close();
	// return
    if (count($rows) > 0) {
        $result = $rows[0];
        return $result['Id'];
    } else {
        return 0;
    }
}
function readMatch($match) {
    $sql = "SELECT * FROM `football_matches` WHERE DATE(`StartTime`)=DATE('{$match['StartTime']}') AND `ChampId`='{$match['ChampId']}' AND `HomeTeamId`='{$match['HomeTeamId']}' AND `AwayTeamId`='{$match['AwayTeamId']}'";
    return mysqlQuerySelectFirst($sql);
}
function updateMatch($match, $match_odds) {
	set_time_limit(360000);
	$match['ChampId'] = getChampId($match['Champ']);
    $match['HomeTeamId'] = getTeamId($match['HomeTeam']);
    $match['AwayTeamId'] = getTeamId($match['AwayTeam']);
    if ( !empty($match['ChampId']) && !empty($match['HomeTeamId']) && !empty($match['AwayTeamId']) ) {
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
    if (!isIdenticalValues($match1, $match2, 'TV')) return FALSE;
    return TRUE;
}
function isIdenticalValues($match1, $match2, $index) {
    if (!isset($match1[$index]) && !isset($match2[$index])) return TRUE;
    if (isset($match1[$index]) && isset($match2[$index]) && $match1[$index] == $match2[$index]) return TRUE;
    return FALSE;
}
?>
<?php // Execute
header('Content-Type: text/plain; charset=utf-8');
date_default_timezone_set("Europe/Athens"); // <= SOS!!! It is for correct datetime translation!!
set_time_limit(3600);
ini_set('max_execution_time', 3600);
ini_set('max_input_time', 3600);
// Init
$sportId = "s-441";
if (isset($_GET['d'])) {
    $drawNumber = $_GET['d'];
} else {
    $drawNumber = getDayId();
}
$locale = "gr";
$url = "https://pamestoixima.opap.gr/forward/web/services/rs/iFlexBetting/retail/games/15104/0.json?shortTourn=true&startDrawNumber={$drawNumber}&endDrawNumber={$drawNumber}&sportId={$sportId}&marketIds=0&marketIds=0A&marketIds=1&marketIds=69&marketIds=68&marketIds=20&marketIds=21&marketIds=8&locale={$locale}&brandId=defaultBrand&channelId=0";
$json = file_get_contents($url);
$items = json_decode($json);
$result = array('Identical' => 0, 'Identical-log' => ""
              , 'Changed' => 0, 'Changed-log' => ""
              , 'Inserted' => 0, 'Inserted-log' => ""
              , 'Error' => 0, 'Error-log' => "");
// loop
foreach($items as $item) {
    // Set markets
    foreach ($item->markets as $market) {
        switch ($market->i) {
            case '0': $market1X2 = $market; break; // 1-X-2
            case '20': $marketUO25 = $market; break; // Under/Over 2.5
            case '69': $marketGGNG = $market; break; // GG/NG
        }
    }
    // Set match info
    $match = array();
    $match['Champ'] = $item->lexicon->resources->{$item->ti.'_sh'};
	//$match['Championship'] = $item->lexicon->resources->{$item->ti};
    if (empty($match['Champ'])) $match['Champ'] = mb_substr($item->lexicon->resources->{$item->ti}, 0, 3, 'UTF-8');
    $match['StartTime'] = date('Y-m-d H:i:s', $item->kdt / 1000);
    $match['HomeTeam'] = $item->lexicon->resources->{$item->hi};
    $match['AwayTeam'] = $item->lexicon->resources->{$item->ai};
    if (isset($item->tvId)) $match['TV'] = intval($item->tvId); else $match['TV'] = 0;
    // Set match odds info
    $match_odds = array();
    $match_odds['Code'] = $item->code;
    $match_odds['EE'] = $market1X2->mins;
    $match_odds['AdvHome'] = 0;
    $match_odds['AdvAway'] = 0;
    $match_odds['FT_1'] = $market1X2->codes[0]->oddPerSet->{'1'};
    $match_odds['FT_X'] = $market1X2->codes[1]->oddPerSet->{'1'};
    $match_odds['FT_2'] = $market1X2->codes[2]->oddPerSet->{'1'};
    $match_odds['Under'] = $marketUO25->codes[0]->oddPerSet->{'1'};
    $match_odds['Over'] = $marketUO25->codes[1]->oddPerSet->{'1'};
    $match_odds['Goal'] = $marketGGNG->codes[0]->oddPerSet->{'1'};
    $match_odds['NoGoal'] = $marketGGNG->codes[1]->oddPerSet->{'1'};
    // Update Database
    switch (updateMatch($match, $match_odds)) {
        case 0: // nothing to be done, match exists and up to date
            $result['Identical'] += 1;
            break;
        case 1: // match exists but changed
            $result['Changed'] += 1;
            break;
        case 2: // match not exist and inserted successfully
            $result['Inserted'] += 1;
            break;
        case 3: // one or more Id is missing, match can not be inserted or update it
            $result['Error'] += 1;
            break;
    }
}
// Write the result
echo 'Errors: ', $result['Error'], ', Inserted: ', $result['Inserted'], ', Changed: ', $result['Changed'], ', Identical: ', $result['Identical'];
?>
<?php // end gzip compressing
while (@ob_end_flush());
?>