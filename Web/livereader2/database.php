<?php
// Database authentication
define('HOSTNAME', 'localhost');
define('DATABASE', 'livereader2');
define('USERNAME', 'myroot');
define('PASSWORD', 'zJp4$81s');
// Execute SQL query without return the result
function mysqlQuery($sql) {
	// Connect to database
	$mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, DATABASE);
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
	// Execute the command
	$mysqli->query($sql);
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
// Execute SQL query and return all records and fields as array
function mysqlQuerySelect($sql) {
	// Connect to database
	$mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, DATABASE);
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
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
	return $rows;
}
// Execute SQL query and return the first result
function mysqlQuerySelectFirst($sql) {
	$rows = mysqlQuerySelect($sql);
    if (count($rows) > 0) {
        return $rows[0];
    } else {
        return NULL;
    }
}
// Insert or update array to table
function mysqlInsertOrUpdateFromArray($table, $array) {
	// Connect to database
	$mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, DATABASE);
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
    // Create command
    $fields = NULL; $values = NULL; $pairs = NULL;
	foreach ($array as $key => $val) {
        // If string use real_escape_string
        if (is_string($val)) {
            $value = $mysqli->real_escape_string($val);
        } else {
            $value = $val;
        }
        // Make field-value strings
		if ( empty($fields) ) { $fields = "`{$key}`"; }
		else { $fields .= ", `{$key}`"; }
		if ( empty($values) ) { $values = "'{$value}'"; }
		else { $values .= ", '{$value}'"; }
		if ( empty($pairs) ) { $pairs = "`{$key}`='{$value}'"; }
		else { $pairs .= ", `{$key}`='{$value}'"; }
	}
    $sql = "INSERT INTO `{$table}` ({$fields}) VALUES ({$values}) ON DUPLICATE KEY UPDATE {$pairs};";
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
// Export table to xml
function writeTableXml($sql, $writeMatchXml) {
	// Connect to database
	$mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, DATABASE);
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
	// Execute the command
	$result = $mysqli->query($sql);
	// If execution failed
	if ($mysqli->errno) {
		die('Failed to execute query: (' . $mysqli->errno . ') ' . $mysqli->error);
	}
	// Write xml
	$writer = new XMLWriter();
	$writer->openURI('php://output');
	$writer->startDocument('1.0','UTF-8');
	$writer->setIndent(4);
	$writer->startElement('Live');
	while($row = $result->fetch_array())
	{
		$writeMatchXml($writer, $row);
	}
	$writer->endElement();
	$writer->endDocument();
	$writer->flush();
	// free result set
	$result->free();
	// Close the connection
	$mysqli->close();
}
// Get matches from *_live_mix tables
function GetMatches($table, $champ, $day) {
	// Connect to database
	$mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, DATABASE);
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
	// Create the command
	$sql = "SELECT * FROM `{$table}` WHERE `Id` IS NOT NULL";
	if (is_numeric($champ)) $sql .= " AND `ChampId` = '{$champ}'";
	elseif (!empty($champ)) $sql .= " AND `Champ` = '{$mysqli->real_escape_string($champ)}'";
	if (isset($day)) {
		$now = new DateTime();
		$now_day = $now->format('Y-m-d');
		$now_hour = intval($now->format('H'));
		if ($day == 'today'){
			$range = humanTodayRange();
		} elseif ($day == 'yesterday') {
			$range = humanYesterdayRange();
		} else {
			$range = humanDayRange($day);
		}
		$beginDT = $range[0];
		$endDT = $range[1];
		$sql .= " AND `StartTime` >= '{$beginDT}' AND `StartTime` < '{$endDT}'";
	}
	$sql .= " ORDER BY `SourceId`,`StartTime` ASC";
	// Execute the command
	$result = $mysqli->query($sql);
	// If execution failed
	if ($mysqli->errno) {
            die('Failed to execute query: (' . $mysqli->errno . ') ' . $mysqli->error);
	}
	// Get unmixed matches
        $matches = array();
	while($match = $result->fetch_array()) {
            $PairId = $match['PairId'];
            $pairs= explode("-", $PairId);
            if (count($pairs) === 2) {
                $ReversedPairId = $pairs[1] . "-" . $pairs[0];
                if (array_key_exists($ReversedPairId,$matches )) {
                    $matches[$ReversedPairId][$match['Source']] = $match;
                } else {
                    $matches[$PairId][$match['Source']] = $match;
                }
            } else {
                $matches[$match['Id']] = $match;
            }
	}
	// free result set
	$result->free();
	// Close the connection
	$mysqli->close();
	return $matches;
}
// Get matches dates from *_live_mix tables
function GetMatchesDays($table) {
    // Connect to database
    $mysqli = new mysqli(HOSTNAME, USERNAME, PASSWORD, DATABASE);
    // If connection failed die...
    if ($mysqli->connect_errno) {
            die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
    }
    // Change character set to utf8
    $mysqli->set_charset('utf8');
    // Create the command
    $sql = "SELECT DISTINCT HUMAN_DAY(`StartTime`) AS `Date` FROM `{$table}` ORDER BY `Date` DESC";
    // Execute the command
    $query = $mysqli->query($sql);
    // If execution failed
    if ($mysqli->errno) {
            die('Failed to execute query: (' . $mysqli->errno . ') ' . $mysqli->error);
    }
    $rows = array();
    while($row = $query->fetch_assoc()) {
        $rows[] = $row['Date'];
    }
    // free query set
    $query->free();
    // Close the connection
    $mysqli->close();
    // return
    return $rows;
}
function humanToday() {
    $now = new DateTime();
    $now_hour = intval($now->format('H'));
    if ($now_hour < 8) {
        $interval = DateInterval::createFromDateString('-1 day');
        $yesterday = $now->add($interval);
        return $yesterday->format('Y-m-d');
    } else {
        return $now->format('Y-m-d');
    }
}
function humanYesterday() {
    $interval = DateInterval::createFromDateString('-1 day');
    $yesterday = (new DateTime(humanToday()))->add($interval);
    return $yesterday->format('Y-m-d');
}
function humanDayRange($day) {
    $interval = DateInterval::createFromDateString('1 day');
    $beginDay = new DateTime($day);
    $endDay = (new DateTime($day))->add($interval);
    $beginDT = $beginDay->format('Y-m-d') . ' 08:00:00';
    $endDT = $endDay->format('Y-m-d') . ' 08:00:00';
    return array($beginDT, $endDT);
}
function humanTodayRange() {
    return humanDayRange(humanToday());
}
function humanYesterdayRange() {
    return humanDayRange(humanYesterday());
}
function humanYesterdayBegin() {
    return humanDayRange(humanYesterday())[0];
}
?>