<?php

// start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php

// Init
require_once('livereader2/database.php');
require_once('livereader2/mix.php');
error_reporting(0);
date_default_timezone_set('Europe/Athens');
header('Content-Type: text/html; charset=utf-8');
?>
<?php

// The coupon table

function htmlTable($sport) {
    $matches = GetMatches(strtolower($sport) . '_live_mix');
    $table = '<table id="TableCoupon">';
    $table .= htmlTableHeaders();
    foreach ($matches as $PairId => $sources) {
        $Opap = $sources['Opap'];
        if (isset($Opap)) {
            $Mixed = GetMixedMatchFootball($Opap, $sources['Futbol24'], $sources['SportingBet'], $sources['NowGoal']);
            $table .= htmlTableRow($Mixed);
        }
    }
    $table .= '</table>';
    return $table;
}

function htmlTableHeaders() {
    return '<tr>'
            . '<th>Champ</th>'
            . '<th>StartTime</th>'
            . '<th>Code</th>'
            . '<th>HomeTeam</th>'
            . '<th>AwayTeam</th>'
            . '<th>ScoreHT</th>'
            . '<th>Score</th>'
            . '<th>Minute</th>'
            . '<th>StatusId</th>'
            . '<th></th>'
            . '</tr>';
}

function htmlLiveStats($match) {
    if (empty($match['ScoreHT'])) {
        $HomeGoalsHT = -1;
        $AwayGoalsHT = -1;
    } else {
        $scoreHT = explode('-', $match['ScoreHT']);
        $HomeGoalsHT = intval($scoreHT[0]);
        $AwayGoalsHT = intval($scoreHT[1]);
    }
    if (empty($match['Score'])) {
        return '';
    } else {
        $score = explode('-', $match['Score']);
        $HomeGoals = intval($score[0]);
        $AwayGoals = intval($score[1]);
        if (($HomeGoalsHT == -1 && $HomeGoals > 0) || ($AwayGoalsHT == -1 && $AwayGoals > 0)) {
            $HomeGoalsHT = $HomeGoals;
            $AwayGoalsHT = $AwayGoals;
        }
        if ($HomeGoalsHT == -1 && $AwayGoalsHT == -1)
            return '';
    }
    if ($match['StatusId'] == 'Half1' || $match['StatusId'] == 'HT' || $match['StatusId'] == 'Half2') {
        return '<a href="http://livereader.ddns.net:60800/xml/livestats.php?html&HomeTeam=' . $match['HomeTeam'] . '&AwayTeam=' . $match['AwayTeam'] . '&HomeGoalsHT=' . $HomeGoalsHT . '&AwayGoalsHT=' . $AwayGoalsHT . '&HomeGoals=' . $HomeGoals . '&AwayGoals=' . $AwayGoals . '&Minute=' . $match['Minute'] . '" target="_blank">LiveStats!</a>';
    }
    return '';
}

function htmlTableRow($match) {
    $row = '<tr>';
    $row .= '<td>' . $match['Champ'] . '</td>';
    $row .= '<td>' . $match['StartTime'] . '</td>';
    $row .= '<td>' . $match['WebId'] . '</td>';
    $row .= '<td>' . $match['HomeTeam'] . '</td>';
    $row .= '<td>' . $match['AwayTeam'] . '</td>';
    $row .= '<td>' . $match['ScoreHT'] . '</td>';
    $row .= '<td>' . $match['Score'] . '</td>';
    $row .= '<td>' . $match['Minute'] . '</td>';
    $row .= '<td>' . $match['StatusId'] . '</td>';
    $row .= '<td>' . htmlLiveStats($match) . '</td>';
    $row .= '</tr>';
    return $row;
}

?>
<?php

$sport = $_GET['sport'];
if (isset($sport)) {
    echo htmlTable($sport);
}
?>
<?php

// end gzip compressing
while (@ob_end_flush());
?>