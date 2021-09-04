<?php

// Init
require_once('database.php');
require_once('mix.php');
//error_reporting(0);
date_default_timezone_set('Europe/Athens');
header('Content-Type: text/html; charset=utf-8');
//header('Content-Type: text/plain; charset=utf-8');
?>
<?php

function SendAlert($msg) {
    $to = '<alexander.arvanitis@gmail.com>,<prantalosz@gmail.com>,<skouny@gmail.com>';
    $from = "alerts@play90.gr";
    $subject = "Livescore Alert!";
    $message = wordwrap($msg, 70); // use wordwrap() if lines are longer than 70 characters
    $headers = "From: {$from}";
    return @mail($to, $subject, $message, $headers, "-f " . $from);
}

function SendAlertHtml($msg) {
    $to = 'Alex <alexander.arvanitis@gmail.com>, Zac <prantalosz@gmail.com>, Yiannis <skouny@gmail.com>';
    $from = 'alerts@play90.gr';
    $subject = 'Livescore Alert!';
    // To send HTML mail, the Content-type header must be set
    $headers = 'MIME-Version: 1.0' . "\r\n";
    $headers .= 'Content-type: text/html; charset=utf-8' . "\r\n";
    // Create email headers
    $headers .= 'From: ' . $from . "\r\n" .
            'Reply-To: ' . $to . "\r\n" .
            'X-Mailer: PHP/' . phpversion();
    // Compose a simple HTML email message
    $message = "<html><body>{$msg}</body></html>";
    // Sending email
    return @mail($to, $subject, $message, $headers);
}

function MatchFields($match) {
    $fields = '';
    $fields .= '<td>' . $match['Source'] . '</td>';
    $fields .= '<td>' . $match['Champ'] . '</td>';
    $fields .= '<td>' . $match['StartTime'] . '</td>';
    $fields .= '<td>' . $match['WebId'] . '</td>';
    $fields .= '<td>' . $match['HomeTeam'] . '</td>';
    $fields .= '<td>' . $match['AwayTeam'] . '</td>';
    $fields .= '<td>' . $match['ScoreHT'] . '</td>';
    $fields .= '<td>' . $match['Score'] . '</td>';
    $fields .= '<td>' . $match['Minute'] . '</td>';
    $fields .= '<td>' . $match['Status'] . '</td>';
    $fields .= '<td>' . $match['Modified'] . '</td>';
    return $fields;
}

function TableAlerts($matches) {
    $isEmpty = true;
    $table = '<table border="1">';
    foreach ($matches as $key => $sources) {
        if (strpos($key, '-') && isset($sources['Opap'])) { // Matched Opap
            if (isInPlay($sources['Opap']['StartTime']) && isActive($sources) && isStartTimeCorrect($sources)) {
                if (count($sources) == 1 || (isset($sources['Futbol24']) && isDelayed($sources['Futbol24']['Modified'])) || (isset($sources['NowGoal']) && isDelayed($sources['NowGoal']['Modified']))
                ) {
                    $first = true;
                    foreach ($sources as $name => $source) {
                        $match = $source; // source is the match
                        $table .= "<tr>";
                        if ($first) {
                            $count = count($sources);
                            $table .= "<td rowspan='{$count}'>{$key}</td>";
                            $first = false;
                        }
                        $table .= MatchFields($match);
                        $table .= "</tr>";
                        $isEmpty = false;
                    }
                }
            }
        } else if (substr($key, 0, 5) == 'Opap#') { // Unmatched Opap
            $match = $sources; // sources is the match
            if (isLive($match['StartTime'])) {
                $table .= "<tr>";
                $table .= "<td>Unmatched</td>";
                $table .= MatchFields($match);
                $table .= "</tr>";
                $isEmpty = false;
            }
        }
    }
    $table .= '</table>';
    if ($isEmpty)
        return null;
    return $table;
}

function isActive($sources) {
    // Check Opap status
    if (isset($sources['Opap'])) {
        if ($sources['Opap']['Status'] == 'blocked') {
            return false;
        }
    }
    // Check FlashScore status
    if (isset($sources['FlashScore'])) {
        if ($sources['FlashScore']['Status'] == 'Cancelled') {
            return false;
        }
        if ($sources['FlashScore']['Status'] == 'FRO') {
            return false;
        }
        if ($sources['FlashScore']['Status'] == 'Postponed') {
            return false;
        }
        if ($sources['FlashScore']['Status'] == 'Awaiting updates') {
            return false;
        }
    }
    // Check Futbol24 status
    if (isset($sources['Futbol24'])) {
        if ($sources['Futbol24']['Status'] == 'Postp.') {
            return false;
        }
        if ($sources['Futbol24']['Status'] == 'FT Only') {
            return false;
        }
        if ($sources['Futbol24']['Status'] == 'Susp.') {
            return false;
        }
        if ($sources['Futbol24']['Status'] == 'ABD') {
            return false;
        }
    }
    // Check NowGoal status
    if (isset($sources['NowGoal'])) {
        if ($sources['NowGoal']['Status'] == 'Postp.') {
            return false;
        }
        if ($sources['NowGoal']['Status'] == 'Pend.') {
            return false;
        }
        if ($sources['NowGoal']['Status'] == 'Abd') {
            return false;
        }
    }
    return true;
}
function isStartTimeCorrect($sources) {
    if (isset($sources['Opap']) && isset($sources['NowGoal'])) {
        if ($sources['Opap']['StartTime'] != $sources['NowGoal']['StartTime']) {
            return false;
        }
    }
    return true;
}
function isDelayed($strDate) {
    $diff = intval(dateDifferenceNow($strDate));
    if ($diff <= -360) { // 6min
        return true;
    }
    return false;
}

function isInPlay($strDate) {
    if (isLive($strDate) && !isHalfTime($strDate)) {
        return true;
    }
    return false;
}

function isLive($strDate) {
    $diff = intval(dateDifferenceNow($strDate));
    if ($diff <= -360 && $diff >= -6300) { //
        return true;
    }
    return false;
}

function isHalfTime($strDate) {
    $diff = intval(dateDifferenceNow($strDate));
    if ($diff <= -2700 && $diff >= -4320) { // between 45' and 45+15+12=72' is halftime break
        return true;
    }
    return false;
}

function dateDifferenceNow($strDate) {
    return dateDifference($strDate, 'now');
}

function dateDifference($strDate1, $strDate2) {
    $date1 = new DateTime($strDate1);
    $date2 = new DateTime($strDate2);
    return $date1->getTimestamp() - $date2->getTimestamp();
}

?>
<?php

$sport = 'football';
$champ = null;
$day = humanToday();
$matches = GetMatches(strtolower($sport) . '_live_mix', $champ, $day);
$table = TableAlerts($matches);
$now = (new DateTime('now'))->format('Y-m-d H:i:s');
if (!empty($table)) {
    if (isset($_GET['sendmail'])) {
        //SendAlertHtml("Now: {$now}<br />{$table}<br /><a href='http://livereader.play90.gr/livereader2/alerts.php'>Recheck Now</a>");
    }
    echo "Now: {$now}<br />";
    //echo dateDifferenceNow('2017-06-16 18:58:58') . '<br />';
    //echo IsLive('2017-06-16 16:40:00');
    echo $table;
} else {
    echo "There is no alerts! Everything seems to be OK.";
}
?>