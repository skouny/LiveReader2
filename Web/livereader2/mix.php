<?php
$GLOBALS['SPORT'] = array('Football', 'Basket');
$GLOBALS['STATUS_ID'] = array('Unknown', 'Waiting', 'Cancelled', 'Abd', 'Pause', 'Qrt1', 'Half1', 'Qrt2', 'HT', 'Qrt3', 'Half2', 'Qrt4', 'FT', 'OT');
function GetLatestTime() { // string input/output
	$args = func_get_args();
	$timestamp = 0;
	foreach ($args as $arg) {
		$time = strtotime($arg);
		if ($timestamp < $time) $timestamp = $time;
	}
	return date('Y-m-d H:i:s', $timestamp);
}
function GetFirstScoredTime($score, $key, $source1, $source2, $source3) {
    $list = array();
    //
    $time1 = strtotime($source1[$key]);
    $time2 = strtotime($source2[$key]);
    $time3 = strtotime($source3[$key]);
    //
    if ($score == $source1['Score'] && $time1 > 0) $list[] += $time1;
    if ($score == $source2['Score'] && $time2 > 0) $list[] += $time2;
    if ($score == $source3['Score'] && $time3 > 0) $list[] += $time3;
    //
    $timestamp = 0;
    foreach ($list as $time) {
        if (empty($timestamp)) {
            $timestamp = $time;
        } elseif ($timestamp > $time) {
            $timestamp = $time;
        }
	}
	return date('Y-m-d H:i:s', $timestamp);
}
function MixMinute($minute1, $minute2, $minute3) { // string input/output
	if (empty($minute1) && empty($minute2) && empty($minute3)) return '';
    if (empty($minute1) && empty($minute2) && !empty($minute3)) return $minute3;
    if (empty($minute1) && !empty($minute2) && empty($minute3)) return $minute2;
    if (!empty($minute1) && empty($minute2) && empty($minute3)) return $minute1;
	$half = 0;
	$overtime = 0;
	if ($minute1 == '45+' || $minute2 == '45+' || $minute3 == '45+') {
		$half = 1;
		$overtime = 0.5;
	}
	if ($minute1 == '90+' || $minute2 == '90+' || $minute3 == '90+') {
		$half = 2;
		$overtime = 0.5;
	}
    $count = 0;
	if (empty($minute1)) $i1 = 0; else { $i1 = intval($minute1); $count++; }
	if (empty($minute2)) $i2 = 0; else { $i2 = intval($minute2); $count++; }
    if (empty($minute3)) $i3 = 0; else { $i3 = intval($minute3); $count++; }
	$avg = ceil(($i1 + $i2 + $i3 + $overtime) / $count);
	if ($half == 1 && $avg > 45) return '45+';
	if ($half == 2 && $avg > 90) return '90+';
	return strval($avg);
}
function MinuteToInt($minute) { // rerurns: error=0, extratime = +0.5
	if ($minute == '45+') return 45.5;
	if ($minute == '90+') return 90.5;
	return intval($minute);
}
function MixStatus($status1, $status2, $status3, $status4) { // string input/output
	$status = 'Unknown';
	if (StatusIsValid($status1)) $status = $status1;
	else if (StatusIsValid($status2)) $status = GetTheLatestStatus($status, $status2);
	else if (StatusIsValid($status3)) $status = GetTheLatestStatus($status, $status3);
    else if (StatusIsValid($status4)) $status = GetTheLatestStatus($status, $status4);
	return $status;
}
function StatusIsValid($status) {
	if (isset($status) && strlen($status) > 0 && $status != 'Unknown') {
		if (in_array($status, $GLOBALS['STATUS_ID'])) return true;
	}
	return false;
}
function GetTheLatestStatus($status1, $status2) {
	$i1 = array_search($status1, $GLOBALS['STATUS_ID']);
	$i2 = array_search($status2, $GLOBALS['STATUS_ID']);
	if ($i1 > $i2) return $status1; else return $status2;
}
function ScoreConfirmed() {
    $scores = func_get_args();
    $count = count($scores);
    if ($count > 0) {
        // Allways return the first score if valid (opap)
        if (ScoreIsValid($scores[0])) return $scores[0];
        // Else confirm 2 of the rest
        if ($count > 1) {
	    for ($i = 1; $i < $count; $i++) {
                $score1 = $scores[$i];
                if (ScoreIsValid($score1) && $count > $i + 1) {
                    for ($j = $i + 1; $j < $count; $j++) {
                        $score2 = $scores[$j];
                        if ($score1 == $score2) return $score1;
                    }
                }
            }
            // If not confimed return any valid
            for ($i = 1; $i < $count; $i++) {
                $score = $scores[$i];
                if (ScoreIsValid($score)) return $score;
            }
        }
    }
    // If not returned so far...
    return '';
}
function ScoreIsValid($score) {
	if (isset($score)) {
		$s = explode('-', $score);
		if (count($s) == 2 && is_numeric($s[0]) && is_numeric($s[1])) {
			return true;
		}
	}
	return false;
}
function GetGreaterPair() {
	$args = func_get_args();
	$max_val = array(0, 0);
	foreach ($args as $arg) {
		$val = explode('-', $arg);
		if ($max_val[0] < intval($val[0])) $max_val[0] = intval($val[0]);
		if ($max_val[1] < intval($val[1])) $max_val[1] = intval($val[1]);
	}
	return strval($max_val[0]) . '-' . strval($max_val[1]);
}
function MatchReversed($Source1, $Source2) {
    if (empty($Source1)) { return FALSE; }
    if (empty($Source2)) { return FALSE; }
    if ($Source1['HomeTeamId'] === $Source2['AwayTeamId']) {
        return TRUE;
    }
    if ($Source1['AwayTeamId'] === $Source2['HomeTeamId']) {
        return TRUE;
    }
}
function PairReverse($pair) {
    if (!empty($pair)) {
        $s = explode("-", $pair);
        if (count($s) === 2) {
           return $s[1] . "-" . s[0];
        }
    }
    return "";
}
function MatchReverse($Source) {
    $Reversed = $Source; //this is copy in php, not reference
    $Reversed['PairId'] = PairReverse($Source['PairId']);
    $Reversed['HomeTeam'] = $Source['AwayTeam'];
    $Reversed['HomeTeamId'] = $Source['AwayTeamId'];
    $Reversed['AwayTeam'] = $Source['HomeTeam'];
    $Reversed['AwayTeamId'] = $Source['HomeTeamId'];
    $Reversed['Score'] = PairReverse($Source['Score']);
    $Reversed['ScoreHT'] = PairReverse($Source['ScoreHT']);
    $Reversed['RedCards'] = PairReverse($Source['RedCards']);
    $Reversed['YellowCards'] = PairReverse($Source['YellowCards']);
    // ...
    $Reversed['HomeEvents'] = $Source['AwayEvents'];
    $Reversed['AwayEvents'] = $Source['HomeEvents'];
    $Reversed['HomeScored'] = $Source['AwayScored'];
    $Reversed['AwayScored'] = $Source['HomeScored'];
    return $Reversed;
}
function GetMixedMatchFootball($Opap, $FlashScore, $Futbol24, $NowGoal) {
    if (MatchReversed($Opap, $FlashScore)) { $FlashScore = MatchReverse($FlashScore); }
    if (MatchReversed($Opap, $Futbol24)) { $Futbol24 = MatchReverse($Futbol24); }
    if (MatchReversed($Opap, $NowGoal)) { $NowGoal = MatchReverse($NowGoal); }
    $Mixed['PairId'] = $Opap['PairId'];
    $Mixed['WebId'] = $Opap['WebId'];
    $Mixed['StartTime'] = $Opap['StartTime'];
    $Mixed['HomeTeam'] = $Opap['HomeTeam'];
    $Mixed['AwayTeam'] = $Opap['AwayTeam'];
    $Mixed['Champ'] = $Opap['Champ'];// $FlashScore['Score'];
    $Mixed['Score'] = ScoreConfirmed($Opap['Score'], $FlashScore['Score'], $Futbol24['Score'], $NowGoal['Score']);
    $Mixed['ScoreHT'] = ScoreConfirmed($Opap['ScoreHT'], $FlashScore['ScoreHT'], $Futbol24['ScoreHT'], $NowGoal['ScoreHT']);
    $Mixed['HomeScored'] = GetFirstScoredTime($Mixed['Score'], 'HomeScored', $FlashScore, $NowGoal, $Futbol24);
    $Mixed['AwayScored'] = GetFirstScoredTime($Mixed['Score'], 'AwayScored', $FlashScore, $NowGoal, $Futbol24);
    $Mixed['Minute'] = MixMinute($FlashScore['Minute'], $NowGoal['Minute'], $Futbol24['Minute']);
    $Mixed['StatusId'] = MixStatus($Opap['StatusId'], $FlashScore['StatusId'], $Futbol24['StatusId'], $NowGoal['StatusId']);
    $Mixed['Modified'] = GetLatestTime($Opap['Modified'], $FlashScore['Modified'], $NowGoal['Modified'], $Futbol24['Modified']);
    $Mixed['HomeEvents'] = $NowGoal['HomeEvents'];
    $Mixed['AwayEvents'] = $NowGoal['AwayEvents'];
    $Mixed['RedCards'] = GetGreaterPair($FlashScore['RedCards'], $Futbol24['RedCards']);
    $Mixed['YellowCards'] = $NowGoal['YellowCards'];
    $Mixed['CornerKicks'] = $NowGoal['CornerKicks'];
    $Mixed['Shots'] = $NowGoal['Shots'];
    $Mixed['ShotsOnGoal'] = $NowGoal['ShotsOnGoal'];
    $Mixed['Fouls'] = $NowGoal['Fouls'];
    $Mixed['BallPossession'] = $NowGoal['BallPossession'];
    $Mixed['Saves'] = $NowGoal['Saves'];
    $Mixed['Offsides'] = $NowGoal['Offsides'];
    $Mixed['KickOff'] = $NowGoal['KickOff'];
    $Mixed['Substitutions'] = $NowGoal['Substitutions'];
    return $Mixed;
}
function GetMixedMatchBasket($Opap, $NowGoal) {
    $Mixed['PairId'] = $Opap['PairId'];
    $Mixed['WebId'] = $Opap['WebId'];
    $Mixed['HomeTeam'] = $Opap['HomeTeam'];
    $Mixed['AwayTeam'] = $Opap['AwayTeam'];
    $Mixed['HomeScored'] = $NowGoal['HomeScored'];
    $Mixed['AwayScored'] = $NowGoal['AwayScored'];
    $Mixed['Champ'] = $Opap['Champ'];
    $Mixed['Minute'] = $NowGoal['Minute'];
    $Mixed['StatusId'] = MixStatus($Opap['StatusId'], $NowGoal['StatusId']);
    $Mixed['Modified'] = GetLatestTime($Opap['Modified'], $NowGoal['Modified']);
    $Mixed['StartTime'] = $Opap['StartTime'];
    $Mixed['Score'] = ScoreConfirmed($Opap['Score'], $NowGoal['Score']);
    $Mixed['ScoreHT'] = ScoreConfirmed($Opap['ScoreHT'], $NowGoal['ScoreHT']);
    $Mixed['ScoreQ1'] = ScoreConfirmed($Opap['ScoreQ1'], $NowGoal['ScoreQ1']);
    $Mixed['ScoreQ2'] = ScoreConfirmed($Opap['ScoreQ2'], $NowGoal['ScoreQ2']);
    $Mixed['ScoreQ3'] = ScoreConfirmed($Opap['ScoreQ3'], $NowGoal['ScoreQ3']);
    $Mixed['ScoreQ4'] = ScoreConfirmed($Opap['ScoreQ4'], $NowGoal['ScoreQ4']);
    $Mixed['StandingPoints'] = $NowGoal['StandingPoints'];
    return $Mixed;
}
?>