<?php // start gzip compressing
    ob_start('ob_gzhandler');
    header('Content-Encoding: gzip');
 ?>
<?php // init
    require_once('../livereader2/database.php');
 ?>
<?php // functions
function CreateStats() {
    $stats = array();
    $stats['Count'] = 0;
    $stats['1'] = 0;
    $stats['X'] = 0;
    $stats['2'] = 0;
    $stats['1orX'] = 0;
    $stats['1or2'] = 0;
    $stats['Xor2'] = 0;
    $stats['Over15'] = 0;
    $stats['Under15'] = 0;
    $stats['Over25'] = 0;
    $stats['Under25'] = 0;
    $stats['Over35'] = 0;
    $stats['Under35'] = 0;
    $stats['GG'] = 0;
    $stats['NG'] = 0;
    return $stats;
}
function CalcStats($matches) {
    $stats = CreateStats();
    foreach ($matches as $match) {
        $stats['Count'] += 1;
        // 1-X-2
        if ($match['ScoreHomeFT'] > $match['ScoreAwayFT']) {
            $stats['1'] += 1;
            $stats['1orX'] += 1;
            $stats['1or2'] += 1;
        } else if ($match['ScoreHomeFT'] < $match['ScoreAwayFT']) {
            $stats['2'] += 1;
            $stats['1or2'] += 1;
            $stats['Xor2'] += 1;
        } else {
            $stats['X'] += 1;
            $stats['1orX'] += 1;
            $stats['Xor2'] += 1;
        }
        // Over/Under 1.5
        if ($match['ScoreHomeFT'] + $match['ScoreAwayFT'] >= 2) {
            $stats['Over15'] += 1;
        } else {
            $stats['Under15'] += 1;
        }
        // Over/Under 2.5
        if ($match['ScoreHomeFT'] + $match['ScoreAwayFT'] >= 3) {
            $stats['Over25'] += 1;
        } else {
            $stats['Under25'] += 1;
        }
        // Over/Under 3.5
        if ($match['ScoreHomeFT'] + $match['ScoreAwayFT'] >= 4) {
            $stats['Over35'] += 1;
        } else {
            $stats['Under35'] += 1;
        }
        // GG/NG
        if ($match['ScoreHomeFT'] > 0 && $match['ScoreAwayFT'] > 0) {
            $stats['GG'] += 1;
        } else {
            $stats['NG'] += 1;
        }
    }
    return $stats;
}
function AddStats($total, $stats) {
    $total['Count'] += $stats['Count'];
    $total['1'] += $stats['1'];
    $total['X'] += $stats['X'];
    $total['2'] += $stats['2'];
    $total['1orX'] += $stats['1orX'];
    $total['1or2'] += $stats['1or2'];
    $total['Xor2'] += $stats['Xor2'];
    $total['Over15'] += $stats['Over15'];
    $total['Under15'] += $stats['Under15'];
    $total['Over25'] += $stats['Over25'];
    $total['Under25'] += $stats['Under25'];
    $total['Over35'] += $stats['Over35'];
    $total['Under35'] += $stats['Under35'];
    $total['GG'] += $stats['GG'];
    $total['NG'] += $stats['NG'];
    return $total;
}
function WriteTableMatches($title, $matches, $stats) { ?>
<table>
    <tr>
        <th colspan="6"><?php echo $title; ?></th>
    </tr>
    <tr>
        <td colspan="6"><?php echo WriteTableStats($stats); ?></td>
    </tr>
    <tr>
        <th>StartTime</th>
        <th>Champ</th>
        <th>HomeTeam</th>
        <th>AwayTeam</th>
        <th>HT</th>
        <th>FT</th>
    </tr>
    <?php foreach ($matches as $match) { ?>
    <tr>
        <td><?php echo date('Y-m-d H:i', strtotime($match['StartTime'])); ?></td>
        <td><?php echo $match['Champ']; ?></td>
        <td><?php echo $match['HomeTeam']; ?></td>
        <td><?php echo $match['AwayTeam']; ?></td>
        <td><?php echo $match['ScoreHomeHT'].'-'.$match['ScoreAwayHT']; ?></td>
        <td><?php echo $match['ScoreHomeFT'].'-'.$match['ScoreAwayFT']; ?></td>
    </tr>
    <?php } ?>
</table><?php }
function WriteTableStats($stats) { ?>
<table>
    <tr>
        <th colspan="14"><?php echo 'Total Matches = '.$stats['Count']; ?></th>
    </tr>
    <tr>
        <th>1</th>
        <th>X</th>
        <th>2</th>
        <th>1orX</th>
        <th>1or2</th>
        <th>Xor2</th>
        <th>Over 1.5</th>
        <th>Under 1.5</th>
        <th>Over 2.5</th>
        <th>Under 2.5</th>
        <th>Over 3.5</th>
        <th>Under 3.5</th>
        <th>NG</th>
        <th>GG</th>
    </tr>
    <tr>
        <td><?php echo $stats['1']; ?></td>
        <td><?php echo $stats['X']; ?></td>
        <td><?php echo $stats['2']; ?></td>
        <td><?php echo $stats['1orX']; ?></td>
        <td><?php echo $stats['1or2']; ?></td>
        <td><?php echo $stats['Xor2']; ?></td>
        <td><?php echo $stats['Over15']; ?></td>
        <td><?php echo $stats['Under15']; ?></td>
        <td><?php echo $stats['Over25']; ?></td>
        <td><?php echo $stats['Under25']; ?></td>
        <td><?php echo $stats['Over35']; ?></td>
        <td><?php echo $stats['Under35']; ?></td>
        <td><?php echo $stats['NG']; ?></td>
        <td><?php echo $stats['GG']; ?></td>
    </tr>
    <tr>
        <td><?php echo number_format(100 * $stats['1'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['X'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['2'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['1orX'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['1or2'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['Xor2'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['Over15'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['Under15'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['Over25'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['Under25'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['Over35'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['Under35'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['NG'] / $stats['Count']) . "%"; ?></td>
        <td><?php echo number_format(100 * $stats['GG'] / $stats['Count']) . "%"; ?></td>
    </tr>
</table><?php }
?>
<?php // Get Data
if (isset($_GET['HomeTeam']) && isset($_GET['AwayTeam']) && is_numeric($_GET['HomeGoals']) && is_numeric($_GET['AwayGoals'])) {
    $HomeTeam = $_GET['HomeTeam'];
    $AwayTeam = $_GET['AwayTeam'];
    $HomeGoals = intval($_GET['HomeGoals']);
    $AwayGoals = intval($_GET['AwayGoals']);
    if (is_numeric($_GET['HomeGoalsHT']) && is_numeric($_GET['AwayGoalsHT'])) {
        $HomeGoalsHT = intval($_GET['HomeGoalsHT']);
        $AwayGoalsHT = intval($_GET['AwayGoalsHT']);
    } else {
        $HomeGoalsHT = -1;
        $AwayGoalsHT = -1;
    }
    $Minute = intval($_GET['Minute']);
    // HomeTeam matches
    $sql = "SELECT * FROM `football_matches` WHERE `StartTime` >= '2000-01-01'";
    $sql .= " AND `HomeTeamId` = GET_FOOTBALL_TEAM_ID('{$HomeTeam}', 0)";
    if ($HomeGoalsHT == 0 && $AwayGoalsHT == 0) {
        $sql .= " AND `ScoreHomeHT` = 0";
        $sql .= " AND `ScoreAwayHT` = 0";
    } else if ($HomeGoalsHT >= 0 && $AwayGoalsHT >= 0) {
        $sql .= " AND `ScoreHomeHT` >= {$HomeGoalsHT}";
        $sql .= " AND `ScoreAwayHT` >= {$AwayGoalsHT}";
    }
    $sql .= " AND `ScoreHomeFT` >= {$HomeGoals}";
    $sql .= " AND `ScoreAwayFT` >= {$AwayGoals}";
    $sql .= " ORDER BY `StartTime` DESC LIMIT 12";
    $HomeTeamMatches = mysqlQuerySelect($sql);
    $HomeTeamStats = CalcStats($HomeTeamMatches);
    // AwayTeam matches
    $sql = "SELECT * FROM `football_matches` WHERE `StartTime` >= '2000-01-01'";
    $sql .= " AND `AwayTeamId` = GET_FOOTBALL_TEAM_ID('{$AwayTeam}', 0)";
    if ($HomeGoalsHT == 0 && $AwayGoalsHT == 0) {
        $sql .= " AND `ScoreHomeHT` = 0";
        $sql .= " AND `ScoreAwayHT` = 0";
    } else if ($HomeGoalsHT >= 0 && $AwayGoalsHT >= 0) {
        $sql .= " AND `ScoreHomeHT` >= {$HomeGoalsHT}";
        $sql .= " AND `ScoreAwayHT` >= {$AwayGoalsHT}";
    }
    $sql .= " AND `ScoreHomeFT` >= '{$HomeGoals}'";
    $sql .= " AND `ScoreAwayFT` >= '{$AwayGoals}'";
    $sql .= " ORDER BY `StartTime` DESC LIMIT 12";
    $AwayTeamMatches = mysqlQuerySelect($sql);
    $AwayTeamStats = CalcStats($AwayTeamMatches);
    // Prehistory matches
    $sql = "SELECT * FROM `football_matches` WHERE `StartTime` >= '2000-01-01'";
    $sql .= " AND `HomeTeamId` = GET_FOOTBALL_TEAM_ID('{$HomeTeam}', 0)";
    $sql .= " AND `AwayTeamId` = GET_FOOTBALL_TEAM_ID('{$AwayTeam}', 0)";
    if ($HomeGoalsHT == 0 && $AwayGoalsHT == 0) {
        $sql .= " AND `ScoreHomeHT` = 0";
        $sql .= " AND `ScoreAwayHT` = 0";
    } else if ($HomeGoalsHT >= 0 && $AwayGoalsHT >= 0) {
        $sql .= " AND `ScoreHomeHT` >= {$HomeGoalsHT}";
        $sql .= " AND `ScoreAwayHT` >= {$AwayGoalsHT}";
    }
    $sql .= " AND `ScoreHomeFT` >= '{$HomeGoals}'";
    $sql .= " AND `ScoreAwayFT` >= '{$AwayGoals}'";
    $sql .= " ORDER BY `StartTime` DESC LIMIT 12";
    $PrehistoryMatches = mysqlQuerySelect($sql);
    $PrehistoryStats = CalcStats($PrehistoryMatches);
    // Total
    $TotalStats = CreateStats();
    $TotalStats = AddStats($TotalStats, $HomeTeamStats);
    $TotalStats = AddStats($TotalStats, $AwayTeamStats);
    $TotalStats = AddStats($TotalStats, $PrehistoryStats);
    // Per 100
    $Percent = CreateStats();
    $Percent['Sign1'] = number_format(100 * $TotalStats['1'] / $TotalStats['Count']);
    $Percent['SignX'] = number_format(100 * $TotalStats['X'] / $TotalStats['Count']);
    $Percent['Sign2'] = number_format(100 * $TotalStats['2'] / $TotalStats['Count']);
    $Percent['Over15'] = number_format(100 * $TotalStats['Over15'] / $TotalStats['Count']);
    $Percent['Under15'] = number_format(100 * $TotalStats['Under15'] / $TotalStats['Count']);
    $Percent['Over25'] = number_format(100 * $TotalStats['Over25'] / $TotalStats['Count']);
    $Percent['Under25'] = number_format(100 * $TotalStats['Under25'] / $TotalStats['Count']);
    $Percent['Over35'] = number_format(100 * $TotalStats['Over35'] / $TotalStats['Count']);
    $Percent['Under35'] = number_format(100 * $TotalStats['Under35'] / $TotalStats['Count']);
    $Percent['GG'] = number_format(100 * $TotalStats['GG'] / $TotalStats['Count']);
    $Percent['NG'] = number_format(100 * $TotalStats['NG'] / $TotalStats['Count']);
    // Fix Percentage
    if ($Minute > 70) {
        
    }
    // Title
    $Title = "LiveStats# {$HomeTeam} - {$AwayTeam}: {$HomeGoals}-{$AwayGoals}, Min: {$Minute}&acute;";
    // Chart URL
    $ChartURL = "http://play90.gr/LiveChart.aspx?Sign1={$Sign1_100}&SignX={$SignX_100}&Sign2={$Sign2_100}&Over15={$Over15_100}&Under15={$Under15_100}&Over25={$Over25_100}&Under25={$Under25_100}&Over35={$Over35_100}&Under35={$Under35_100}&NG={$NG_100}&GG={$GG_100}";
} else { die("Incorrect arguments"); }
?>
<?php // html or xml?
if (isset($_GET['html'])) {
    header('Content-Type: text/html; charset=utf-8');
?>
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8" />
        <title>Live Stats - Details</title>
        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
        <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/jquery-ui.min.js"></script>
        <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
        <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/canvasjs/1.7.0/canvasjs.min.js"></script>
        <script>
            function LiveChart(Id, Title, Sign1, SignX, Sign2, Over15, Under15, Over25, Under25, Over35, Under35, NG, GG) {
                var url = encodeURI("http://play90.gr/LiveChart.aspx" + "?Sign1=" + Sign1 + "&SignX=" + SignX + "&Sign2=" + Sign2 + "&Over15=" + Over15 + "&Under15=" + Under15 + "&Over25=" + Over25 + "&Under25=" + Under25 + "&Over35=" + Over35 + "&Under35=" + Under35 + "&NG=" + NG + "&GG=" + GG);
                $("#" + Id).html('<iframe width="610px" height="280px" src="' + url + '"'
                + ' style="border: none; margin: 0px; padding: 0px; overflow: hidden;"'
                + ' sandbox="allow-same-origin allow-scripts"></iframe>');
                $("#" + Id).dialog({
                    title: Title
                    , autoOpen: true
                    , width: 'auto'
                    , height: 'auto'
                    , resizable: false
                });
                $("#" + Id).css( "display", "inline-block" );
                $("#" + Id).focus();
            }
        </script>
        <style>
            body {
                font-size: 11px;
                font-family: Verdana;
                margin: 0px;
            }
            table {
                border-collapse: collapse;
                border-spacing: 0px;
                margin: 4px;
                display: inline-block;
                vertical-align: top;
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
            td {
                text-align: center;
                padding-left: 4px;
                padding-right: 4px;
            }
        </style>
    </head>
    <body>
        <?php WriteTableStats($TotalStats); ?>
        <a href="<?php echo '?xml&HomeTeam='.$HomeTeam.'&AwayTeam='.$AwayTeam.'&HomeGoalsHT='.$HomeGoalsHT.'&AwayGoalsHT='.$AwayGoalsHT.'&HomeGoals='.$HomeGoals.'&AwayGoals='.$AwayGoals.'&Minute='.$Minute; ?>" target="_blank">View Xml Feed</a>
        <button onclick="<?php echo "LiveChart('dialog','{$Title}',{$Sign1_100},{$SignX_100},{$Sign2_100},{$Over15_100},{$Under15_100},{$Over25_100},{$Under25_100},{$Over35_100},{$Under35_100},{$NG_100},{$GG_100})"; ?>">Live Chart</button>
        <div id="dialog" style="display: none;"></div>
        <br />
        <?php WriteTableMatches('HomeTeam', $HomeTeamMatches, $HomeTeamStats); ?>
        <?php WriteTableMatches('AwayTeam', $AwayTeamMatches, $AwayTeamStats); ?>
        <?php WriteTableMatches('Prehistory', $PrehistoryMatches, $PrehistoryStats); ?>
    </body>
</html>
<?php
} else if (isset($_GET['xml'])) {
	header('Content-Type: text/xml; charset=utf-8');
    $writer = new XMLWriter();
	$writer->openURI('php://output');
	$writer->startDocument('1.0','UTF-8');
	$writer->setIndent(4);
	$writer->startElement('livestats');
        $writer->writeAttribute('Sign1', $Sign1_100);
        $writer->writeAttribute('SignX', $SignX_100);
        $writer->writeAttribute('Sign2', $Sign2_100);
        $writer->writeAttribute('Over15', $Over15_100);
        $writer->writeAttribute('Under15', $Under15_100);
        $writer->writeAttribute('Over25', $Over25_100);
        $writer->writeAttribute('Under25', $Under25_100);
        $writer->writeAttribute('Over35', $Over35_100);
        $writer->writeAttribute('Under35', $Under35_100);
        $writer->writeAttribute('GG', $GG_100);
        $writer->writeAttribute('NG', $NG_100);
        $writer->writeAttribute('Title', $Title);
    $writer->endElement();
	$writer->endDocument();
	$writer->flush();
} else {
    echo "Unknown output";
}
?>
<?php // end gzip compressing
    while (@ob_end_flush());
?>
