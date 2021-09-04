<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php // functions
require_once('../livereader2/database.php');
require_once('../livereader2/mix.php');
function GetFootballXml($matches) {
	$writer = new XMLWriter();
	$writer->openMemory();
	$writer->startDocument('1.0','UTF-8');
	$writer->setIndent(4);
	$writer->startElement('MatchDay');
	// Start Loop
	foreach ($matches as $PairId => $sources) {
		$Opap = $sources['Opap'];
		if (isset($Opap)) {
			$Mixed = GetMixedMatchFootball($Opap, $sources['FlashScore'], $sources['Futbol24'], $sources['NowGoal']);
			$HomeTeam = $Mixed['HomeTeam'];
			$AwayTeam = $Mixed['AwayTeam'];
			$HomeScored = $Mixed['HomeScored'];
			$AwayScored = $Mixed['AwayScored'];
			$Champ = $Mixed['Champ'];
			$Minute = $Mixed['Minute'];
			$Status = $Mixed['StatusId'];
			$Modified = $Mixed['Modified'];
			$StartTime = $Mixed['StartTime'];
			$Score = explode('-', $Mixed['Score']);
			$ScoreHT = explode('-', $Mixed['ScoreHT']);
			$HomeEvents = $Mixed['HomeEvents'];
			$AwayEvents = $Mixed['AwayEvents'];
			$RedCards = explode('-', $Mixed['RedCards']);
			$YellowCards = explode('-', $Mixed['YellowCards']);
			$CornerKicks = explode('-', $Mixed['CornerKicks']);
			$Shots = explode('-', $Mixed['Shots']);
			$ShotsOnGoal = explode('-', $Mixed['ShotsOnGoal']);
			$Fouls = explode('-', $Mixed['Fouls']);
			$BallPossession = explode('-', $Mixed['BallPossession']);
			$Saves = explode('-', $Mixed['Saves']);
			$Offsides = explode('-', $Mixed['Offsides']);
			$KickOff = explode('-', $Mixed['KickOff']);
			$Substitutions = explode('-', $Mixed['Substitutions']);
			$writer->startElement('Match');
				$writer->writeAttribute('Champ', $Champ);
				$writer->writeAttribute('Minute', $Minute);
				$writer->writeAttribute('Status', $Status);
				$writer->writeAttribute('Modified', myDT($Modified));
				$writer->startElement('StartTime');
					$writer->writeAttribute('Value', myDT($StartTime));
				$writer->endElement();
				$writer->startElement('HomeTeam');
					$writer->writeAttribute('Name', $HomeTeam);
					$writer->writeAttribute('Score', $Score[0]);
					$writer->writeAttribute('ScoreHT', $ScoreHT[0]);
					$writer->writeAttribute('RedCards', $RedCards[0]);
					$writer->writeAttribute('YellowCards', $YellowCards[0]);
					$writer->writeAttribute('CornerKicks', $CornerKicks[0]);
					$writer->writeAttribute('Shots', $Shots[0]);
					$writer->writeAttribute('ShotsOnGoal', $ShotsOnGoal[0]);
					$writer->writeAttribute('Fouls', $Fouls[0]);
					$writer->writeAttribute('BallPossession', $BallPossession[0]);
					$writer->writeAttribute('Saves', $Saves[0]);
					$writer->writeAttribute('Offsides', $Offsides[0]);
					$writer->writeAttribute('KickOff', $KickOff[0]);
					$writer->writeAttribute('Substitutions', $Substitutions[0]);
					$writer->writeAttribute('Scored', myDT($HomeScored));
					$writer->writeRaw($HomeEvents);
				$writer->endElement();
				$writer->startElement('AwayTeam');
					$writer->writeAttribute('Name', $AwayTeam);
					$writer->writeAttribute('Score', $Score[1]);
					$writer->writeAttribute('ScoreHT', $ScoreHT[1]);
					$writer->writeAttribute('RedCards', $RedCards[1]);
					$writer->writeAttribute('YellowCards', $YellowCards[1]);
					$writer->writeAttribute('CornerKicks', $CornerKicks[1]);
					$writer->writeAttribute('Shots', $Shots[1]);
					$writer->writeAttribute('ShotsOnGoal', $ShotsOnGoal[1]);
					$writer->writeAttribute('Fouls', $Fouls[1]);
					$writer->writeAttribute('BallPossession', $BallPossession[1]);
					$writer->writeAttribute('Saves', $Saves[1]);
					$writer->writeAttribute('Offsides', $Offsides[1]);
					$writer->writeAttribute('KickOff', $KickOff[1]);
					$writer->writeAttribute('Substitutions', $Substitutions[1]);
					$writer->writeAttribute('Scored', myDT($AwayScored));
					$writer->writeRaw($AwayEvents);
				$writer->endElement();
				$writer->startElement('Opap');
					$writer->writeAttribute('Code', $Opap['WebId']);
                                        $writer->writeAttribute('Status', $Opap['Status']);
					$writer->writeAttribute('HomeAdv', '0');
					$writer->writeAttribute('AwayAdv', '0');
				$writer->endElement();
			$writer->endElement();
		}
	}
	// end loop
	$writer->endElement();
	$writer->endDocument();
	$xml = $writer->outputMemory(TRUE);
	$writer->flush();
	return $xml;
}
function GetBasketXml($matches) {
	$writer = new XMLWriter();
	$writer->openMemory();
	$writer->startDocument('1.0','UTF-8');
	$writer->setIndent(4);
	$writer->startElement('MatchDay');
	// Start Loop
	foreach ($matches as $PairId => $sources) {
		$Opap = $sources['Opap'];
		if (isset($Opap)) {
			$Mixed = GetMixedMatchBasket($Opap, $sources['NowGoal']);
			$HomeTeam = $Mixed['HomeTeam'];
			$AwayTeam = $Mixed['AwayTeam'];
			$HomeScored = $Mixed['HomeScored'];
			$AwayScored = $Mixed['AwayScored'];
			$Champ = $Mixed['Champ'];
			$Minute = $Mixed['Minute'];
			$Status = $Mixed['StatusId'];
			$Modified = $Mixed['Modified'];
			$StartTime = $Mixed['StartTime'];
			$Score = explode('-', $Mixed['Score']);
			$ScoreHT = explode('-', $Mixed['ScoreHT']);
			$ScoreQ1 = explode('-', $Mixed['ScoreQ1']);
			$ScoreQ2 = explode('-', $Mixed['ScoreQ2']);
			$ScoreQ3 = explode('-', $Mixed['ScoreQ3']);
			$ScoreQ4 = explode('-', $Mixed['ScoreQ4']);
			$StandingPoints = $Mixed['StandingPoints'];
			$writer->startElement('Match');
				//$writer->writeAttribute('PairId', $PairId);
				$writer->writeAttribute('Champ', $Champ);
				$writer->writeAttribute('Minute', $Minute);
				$writer->writeAttribute('Status', $Status);
				$writer->writeAttribute('Modified', myDT($Modified));
				$writer->startElement('StartTime');
					$writer->writeAttribute('Value', myDT($StartTime));
				$writer->endElement();
				$writer->startElement('HomeTeam');
					$writer->writeAttribute('Name', $HomeTeam);
					$writer->writeAttribute('Score', $Score[0]);
					$writer->writeAttribute('ScoreHT', $ScoreHT[0]);
					$writer->writeAttribute('ScoreQ1', $ScoreQ1[0]);
					$writer->writeAttribute('ScoreQ2', $ScoreQ2[0]);
					$writer->writeAttribute('ScoreQ3', $ScoreQ3[0]);
					$writer->writeAttribute('ScoreQ4', $ScoreQ4[0]);
					$writer->writeAttribute('StandingPoints', $StandingPoints[0]);
					$writer->writeAttribute('Scored', myDT($HomeScored));
					$writer->writeRaw();
				$writer->endElement();
				$writer->startElement('AwayTeam');
					$writer->writeAttribute('Name', $AwayTeam);
					$writer->writeAttribute('Score', $Score[1]);
					$writer->writeAttribute('ScoreHT', $ScoreHT[1]);
					$writer->writeAttribute('ScoreQ1', $ScoreQ1[1]);
					$writer->writeAttribute('ScoreQ2', $ScoreQ2[1]);
					$writer->writeAttribute('ScoreQ3', $ScoreQ3[1]);
					$writer->writeAttribute('ScoreQ4', $ScoreQ4[1]);
					$writer->writeAttribute('StandingPoints', $StandingPoints[1]);
					$writer->writeAttribute('Scored', myDT($AwayScored));
					$writer->writeRaw($AwayEvents);
				$writer->endElement();
				$writer->startElement('Opap');
					$writer->writeAttribute('Code', $Opap['WebId']);
                    $writer->writeAttribute('Status', $Opap['Status']);
					$writer->writeAttribute('HomeAdv', '0');
					$writer->writeAttribute('AwayAdv', '0');
				$writer->endElement();
			$writer->endElement();
		}
	}
	// end loop
	$writer->endElement();
	$writer->endDocument();
	$xml = $writer->outputMemory(TRUE);
	$writer->flush();
	return $xml;
}
function myDT($str) {
	return date_create($str)->format('Y-m-d\TH:i:s');
}
?>
<?php // execute
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Set headers
header('Content-Type: text/xml; charset=utf-8');
// Get Params
if (empty($_GET['sport'])) $sport = 'football'; else $sport = strtolower($_GET['sport']);
if (empty($_GET['day'])) $day = 'today'; else $day = strtolower($_GET['day']);
// Select case
if ($sport == 'football' && $day == 'today') {
	$file = "../../play90Worker/Data/OpapFootball.xml";
	$modified = filemtime($file);
	// update disk file every 10sec
	if (empty($modified) || (time() - $modified) > 5) {
		$xml = GetFootballXml(GetMatches('football_live_mix', null, $day));
		file_put_contents($file, $xml);
	} else {
		$xml = file_get_contents($file);
	}
	// Write Output
	echo $xml;
}
elseif ($sport == 'football' && $day == 'yesterday') {
	$file = "../../play90Worker/Data/OpapFootballYesterday.xml";
	$modified = filemtime($file);
	// update disk file every 10sec
	if (empty($modified) || (time() - $modified) > 5) {
		$xml = GetFootballXml(GetMatches('football_live_mix', null, $day));
		file_put_contents($file, $xml);
	} else {
		$xml = file_get_contents($file);
	}
	// Write Output
	echo $xml;
}
elseif ($sport == 'basket' && $day == 'today') {
	$file = "../../play90Worker/Data/OpapBasket.xml";
	$modified = filemtime($file);
	// update disk file every 2sec
	if (empty($modified) || (time() - $modified) > 5) {
		$xml = GetBasketXml(GetMatches('basket_live_mix', null, $day));
		file_put_contents($file, $xml);
	} else {
		$xml = file_get_contents($file);
	}
	// Write Output
	echo $xml;
}
elseif ($sport == 'basket' && $day == 'yesterday') {
	$file = "../../play90Worker/Data/OpapBasketYesterday.xml";
	$modified = filemtime($file);
	// update disk file every 2sec
	if (empty($modified) || (time() - $modified) > 5) {
		$xml = GetBasketXml(GetMatches('basket_live_mix', null, $day));
		file_put_contents($file, $xml);
	} else {
		$xml = file_get_contents($file);
	}
	// Write Output
	echo $xml;
}
?>
<?php // end gzip compressing
  while (@ob_end_flush());
?>