<?php // Start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php // Dependencies
require_once('database.php');
?>
<?php // Functions
function getYears() {
    $sql = "SELECT DISTINCT YEAR(`StartTime`) AS `Year` FROM `football_matches` ORDER BY `Year` DESC";
    return mysqlQuerySelect($sql);
}
function getDays($year) {
    $sql = "SELECT DISTINCT DATE(`StartTime`) AS `Day`";
    $sql .= " FROM `football_matches`";
    $sql .= " WHERE YEAR(`StartTime`) = '{$year}'";
    $sql .= " ORDER BY `Day` DESC";
    return mysqlQuerySelect($sql);
}
function getDayMatches($day, $orderby, $champId) {
    $sql = "SELECT * FROM `football_opap_matches`";
    $sql .= " WHERE DATE(`StartTime`) = '{$day}'";
    if (!empty($champId)) $sql .= " AND `ChampId` = '{$champId}'";
    if(!empty($orderby)) $sql .= " ORDER BY `{$orderby}` ASC";
    return mysqlQuerySelect($sql);
}
function getDayChamps($day) {
    $sql = "SELECT DISTINCT `Champ`,`ChampId` FROM `football_matches`";
    $sql .= " WHERE DATE(`StartTime`) = '{$day}'";
    $sql .= " ORDER BY `Champ` ASC";
    return mysqlQuerySelect($sql);
}
function getMatchById($match_id) {
    $sql = "SELECT * FROM `football_opap_matches`";
    $sql .= " WHERE `Id` = {$match_id}";
    return mysqlQuerySelectFirst($sql);
}
function getHistoryMatches($match_id, $same_champ, $same_home_team, $same_away_team, $use_handicap, $months_back, 
$same_odd1, $same_oddX, $same_odd2, $same_odd_under, $same_odd_over, $same_odd_goal, $same_odd_nogoal, $plus_minus) {
    $match = getMatchById($match_id);
    $start_time = $match['StartTime'];
    $champId = $match['ChampId'];
    $home_team_id = $match['HomeTeamId'];
    $away_team_id = $match['AwayTeamId'];
    $odd1 = $match['FT_1'];
    $odd1_min = $odd1 - $plus_minus - 0.001;
    $odd1_max = $odd1 + $plus_minus + 0.001;
    $oddX = $match['FT_X'];
    $oddX_min = $oddX - $plus_minus - 0.001;
    $oddX_max = $oddX + $plus_minus + 0.001;
    $odd2 = $match['FT_2'];
    $odd2_min = $odd2 - $plus_minus - 0.001;
    $odd2_max = $odd2 + $plus_minus + 0.001;
    $under = $match['Under'];
    $under_min = $under - $plus_minus - 0.001;
    $under_max = $under + $plus_minus + 0.001;
    $over = $match['Over'];
    $over_min = $over - $plus_minus - 0.001;
    $over_max = $over + $plus_minus + 0.001;
    $goal = $match['Goal'];
    $goal_min = $goal - $plus_minus - 0.001;
    $goal_max = $goal + $plus_minus + 0.001;
    $nogoal = $match['NoGoal'];
    $nogoal_min = $nogoal - $plus_minus - 0.001;
    $nogoal_max = $nogoal + $plus_minus + 0.001;
    $sql = "SELECT * FROM `football_opap_matches`";
    $sql .= " WHERE `StartTime` < '{$start_time}'";
    $sql .= " AND `StartTime` > DATE_ADD('{$start_time}', INTERVAL -{$months_back} MONTH)";
    $sql .= " AND `ScoreHomeFT` IS NOT NULL AND `ScoreAwayFT` IS NOT NULL";
    if ($same_champ) $sql .= " AND `ChampId` = {$champId}";
    if ($same_home_team) $sql .= " AND `HomeTeamId` = {$home_team_id}";
    if ($same_away_team) $sql .= " AND `AwayTeamId` = {$away_team_id}";
    if ($use_handicap == 1) $sql .= " AND (`AdvHome` > 0 OR `AdvAway` > 0)";
    if ($use_handicap == 2) $sql .= " AND ((`AdvHome` IS NULL) OR `AdvHome` = 0) AND ((`AdvAway` IS NULL) OR `AdvAway` = 0)";
    if ($same_odd1) $sql .= " AND `FT_1` > {$odd1_min} AND `FT_1` < {$odd1_max}";
    if ($same_oddX) $sql .= " AND `FT_X` > {$oddX_min} AND `FT_X` < {$oddX_max}";
    if ($same_odd2) $sql .= " AND `FT_2` > {$odd2_min} AND `FT_2` < {$odd2_max}";
    if ($same_odd_under) $sql .= " AND `Under` > {$under_min} AND `Under` < {$under_max}";
    if ($same_odd_over) $sql .= " AND `Over` > {$over_min} AND `Over` < {$over_max}";
    if ($same_odd_goal) $sql .= " AND `Goal` > {$goal_min} AND `Goal` < {$goal_max}";
    if ($same_odd_nogoal) $sql .= " AND `NoGoal` > {$nogoal_min} AND `NoGoal` < {$nogoal_max}";
    $sql .= " ORDER BY `StartTime` DESC;";
    return mysqlQuerySelect($sql);
}
function saveComment($match_id, $comment) {
    $sql = "UPDATE `football_matches_odds` SET `Comment`='$comment' WHERE `MatchId`={$match_id}";
    mysqlQuery($sql);
}
function saveCommentFlag($match_id, $flag) {
    $sql = "UPDATE `football_matches_odds` SET `CommentFlag`='$flag' WHERE `MatchId`={$match_id}";
    mysqlQuery($sql);
}
function saveCommentOdd($match_id, $odd) {
    $sql = "UPDATE `football_matches_odds` SET `CommentOdd`='$odd' WHERE `MatchId`={$match_id}";
    mysqlQuery($sql);
}
function saveCommentStatus($match_id, $status) {
    $sql = "UPDATE `football_matches_odds` SET `CommentStatus`='$status' WHERE `MatchId`={$match_id}";
    mysqlQuery($sql);
}
function writeStartTime($start_time) {
    echo date('H:i', strtotime($start_time));
}
function writeOdd($odd) {
    if (empty($odd)) {
        echo '-';
    } else if ($odd < 10) {
        echo number_format($odd, 2);
    } else {
        echo number_format($odd, 1);
    }
}
function writeOddDiff($odd) {
    if (empty($odd)) {
        echo '-';
    } else if ($odd < 10) {
        echo writeProfit($odd, 2);
    } else {
        echo writeProfit($odd, 1);
    }
}
function writeOddFair($count, $total) {
    $odd = ($count > 0) ? $total / $count : 0;
    writeOdd($odd);
}
function writeOddFairDiff($odd, $count, $total) {
    $fair = ($count > 0) ? $total / $count : 0;
    $diff = ($fair > 0) ? $fair - $odd : 0;
    writeOddDiff($diff);
}
function writeProfit($profit, $decimals = 2) {
    if ($profit > 0) {
        echo '<span class="green">'.number_format($profit, $decimals).'</span>';
    } else if ($profit < 0) {
        echo '<span class="red">'.number_format($profit, $decimals).'</span>';
    } else {
        echo '<span class="black">'.number_format($profit, $decimals).'</span>';
    }
}
function writeProfitAvg($count, $total) {
    $profit = ($total > 0) ? $count / $total : 0;
    writeProfit($profit);
}
function writeHandicap($handicap) {
    if (empty($handicap)) {
        echo '';
    } else {
        echo $handicap;
    }
}
function writeMatchTitle($record) {
    echo "{$record['Code']}# {$record['HomeTeam']} - {$record['AwayTeam']}: {$record['ScoreHomeFT']}-{$record['ScoreAwayFT']}";
}
function writePercentage($count, $total) {
    if ($total > 0) {
        echo number_format(100 * $count / $total) . "%";
    } else {
        echo "-";
    }
}
?>
<?php // Arguments
if (isset($_POST['match_id'])) {
    $match_id = $_POST['match_id'];
    if (isset($_POST['comment'])) {
        $comment = $_POST['comment'];
        saveComment($match_id, $comment);
        die("Done!");
    } else if (isset($_POST['comment_flag'])) {
        $flag = $_POST['comment_flag'];
        saveCommentFlag($match_id, $flag);
        die("Done!");
    } else if (isset($_POST['comment_odd'])) {
        $odd = $_POST['comment_odd'];
        saveCommentOdd($match_id, $odd);
        die("Done!");
    } else if (isset($_POST['comment_status'])) {
        $status = $_POST['comment_status'];
        saveCommentStatus($match_id, $status);
        die("Done!");
    } else {
        $same_champ = $_POST['same_champ'];
        $same_home_team = $_POST['same_home_team'];
        $same_away_team = $_POST['same_away_team'];
        $use_handicap = $_POST['use_handicap'];
        $months_back = $_POST['months_back'];
        $same_odd1 = $_POST['same_odd1'];
        $same_oddX = $_POST['same_oddX'];
        $same_odd2 = $_POST['same_odd2'];
        $same_odd_under = $_POST['same_odd_under'];
        $same_odd_over = $_POST['same_odd_over'];
        $same_odd_goal = $_POST['same_odd_goal'];
        $same_odd_nogoal = $_POST['same_odd_nogoal'];
        $plus_minus = $_POST['plus_minus'];
        $parentMatch = getMatchById($match_id);
        $historyMatches = getHistoryMatches($match_id, $same_champ, $same_home_team, $same_away_team, $use_handicap, $months_back, $same_odd1, $same_oddX, $same_odd2, $same_odd_under, $same_odd_over, $same_odd_goal, $same_odd_nogoal, $plus_minus);
        $prehistory = getHistoryMatches($match_id, FALSE, TRUE, TRUE, FALSE, $months_back, FALSE, FALSE, FALSE, FALSE, FALSE, FALSE, FALSE, $plus_minus);
    }
?>
<div id="tabs" style="margin: 0px;padding: 0px;">
    <ul>
        <li><a href="#tabsMatchesOverall">Matches Overall</a></li>
        <li><a href="#tabsMatches">Matches</a></li>
        <li><a href="#tabsPrehistoryOverall">Prehistory Overall</a></li>
        <li><a href="#tabsPrehistory">Prehistory</a></li>
    </ul>
    <div id="tabsMatches" style="margin: 0px;padding: 0px;">
        <table style="margin: 0px;padding: 0px;white-space: nowrap;">
            <tr>
                <th>#</th>
                <th>StartTime</th>
                <th>Champ</th>
                <th>1</th>
                <th>H</th>
                <th>HomeTeam</th>
                <th>X</th>
                <th>AwayTeam</th>
                <th>H</th>
                <th>2</th>
                <th>U</th>
                <th>O</th>
                <th>GG</th>
                <th>NG</th>
                <th>HT</th>
                <th>FT</th>
            </tr>
        <?php
            $count = 0;
            $count1 = 0;
            $countX = 0;
            $count2 = 0;
            $countUnder = 0;
            $countOver = 0;
            $countGoal = 0;
            $countNoGoal = 0;
            $profit1 = 0;
            $profitX = 0;
            $profit2 = 0;
            $profitUnder = 0;
            $profitOver = 0;
            $profitGoal = 0;
            $profitNoGoal = 0;
            if (count($historyMatches) > 0) {
                foreach($historyMatches as $match) {
                    $count += 1;
        ?>
            <tr>
                <th><?php echo $count; ?></th>
                <td><?php echo $match['StartTime']; ?></td>
                <td><?php echo $match['Champ']; ?></td>
                <td><?php echo writeOdd($match['FT_1']); ?></td>
                <td><?php echo writeHandicap($match['AdvHome']); ?></td>
                <td><?php echo $match['HomeTeam']; ?></td>
                <td><?php echo writeOdd($match['FT_X']); ?></td>
                <td><?php echo $match['AwayTeam']; ?></td>
                <td><?php echo writeHandicap($match['AdvAway']); ?></td>
                <td><?php echo writeOdd($match['FT_2']); ?></td>
                <td><?php echo writeOdd($match['Under']); ?></td>
                <td><?php echo writeOdd($match['Over']); ?></td>
                <td><?php echo writeOdd($match['Goal']); ?></td>
                <td><?php echo writeOdd($match['NoGoal']); ?></td>
                <td><?php echo $match['ScoreHomeHT'].'-'.$match['ScoreAwayHT']; ?></td>
                <td><?php echo $match['ScoreHomeFT'].'-'.$match['ScoreAwayFT']; ?></td>
            </tr>
        <?php
                    $scoreHomeFT = intval($match['ScoreHomeFT']);
                    $scoreAwayFT = intval($match['ScoreAwayFT']);
                    // 1-X-2 count
                    if ($scoreHomeFT > $scoreAwayFT) {
                        $count1 += 1;
                        $profit1 += $match['FT_1'] - 1;
                        $profitX += -1;
                        $profit2 += -1;
                    }
                    else if ($scoreHomeFT < $scoreAwayFT) {
                        $count2 += 1;
                        $profit1 += -1;
                        $profitX += -1;
                        $profit2 += $match['FT_2'] - 1;
                    }
                    else {
                        $countX += 1;
                        $profit1 += -1;
                        $profitX += $match['FT_X'] - 1;
                        $profit2 += -1;
                    }
                    // Under/Over count
                    if ($scoreHomeFT + $scoreAwayFT > 2.5) {
                        $countOver += 1;
                        $profitOver += $match['Over'] - 1;
                        $profitUnder += -1;
                    } else {
                        $countUnder += 1;
                        $profitOver += -1;
                        $profitUnder += $match['Under'] - 1;
                    }
                    // Goal/NoGoal count
                    if ($scoreHomeFT > 0 && $scoreAwayFT > 0) {
                        $countGoal += 1;
                        $profitGoal += $match['Goal'] - 1;
                        $profitNoGoal += -1;
                    } else {
                        $countNoGoal += 1;
                        $profitGoal += -1;
                        $profitNoGoal += $match['NoGoal'] - 1;
                    }
                }
            } else {
                echo "<tr><td colspan='16'>Nothing found</td></tr>";
            }
        ?>
        </table>
    </div>
    <div id="tabsMatchesOverall" style="margin: 0px;padding: 0px;">
        <table style="margin: 0px;padding: 0px;white-space: nowrap;">
            <tr>
                <th><button onclick="matchSelected(<?php echo $match_id; ?>)">Refresh</button></th>
                <th>1</th>
                <th>X</th>
                <th>2</th>
                <th>U</th>
                <th>O</th>
                <th>GG</th>
                <th>NG</th>
                <th>Comment<button onclick="saveComment2(<?php echo $parentMatch['Id']; ?>)" style="float: right;">Save</button></th>
            </tr>
            <tr>
                <th>Success</th>
                <td><?php echo $count1; ?></td>
                <td><?php echo $countX; ?></td>
                <td><?php echo $count2; ?></td>
                <td><?php echo $countUnder; ?></td>
                <td><?php echo $countOver; ?></td>
                <td><?php echo $countGoal; ?></td>
                <td><?php echo $countNoGoal; ?></td>
                <td rowspan="7" style="padding: 0px;margin: 0px;">
                    <textarea id="textarea<?php echo $parentMatch['Id']; ?>" rows="7" cols="50" style="resize: none;"><?php echo $parentMatch['Comment']; ?></textarea>
                </td>
            </tr>
            <tr>
                <th>Percent</th>
                <td><?php writePercentage($count1, $count); ?></td>
                <td><?php writePercentage($countX, $count); ?></td>
                <td><?php writePercentage($count2, $count); ?></td>
                <td><?php writePercentage($countUnder, $count); ?></td>
                <td><?php writePercentage($countOver, $count); ?></td>
                <td><?php writePercentage($countGoal, $count); ?></td>
                <td><?php writePercentage($countNoGoal, $count); ?></td>
            </tr>
            <tr>
                <th>Odd</th>
                <td><?php writeOdd($parentMatch['FT_1']); ?></td>
                <td><?php writeOdd($parentMatch['FT_X']); ?></td>
                <td><?php writeOdd($parentMatch['FT_2']); ?></td>
                <td><?php writeOdd($parentMatch['Under']); ?></td>
                <td><?php writeOdd($parentMatch['Over']); ?></td>
                <td><?php writeOdd($parentMatch['Goal']); ?></td>
                <td><?php writeOdd($parentMatch['NoGoal']); ?></td>
            </tr>
            <tr>
                <th>Fair Odd</th>
                <td><?php writeOddFair($count1, $count); ?></td>
                <td><?php writeOddFair($countX, $count); ?></td>
                <td><?php writeOddFair($count2, $count); ?></td>
                <td><?php writeOddFair($countUnder, $count); ?></td>
                <td><?php writeOddFair($countOver, $count); ?></td>
                <td><?php writeOddFair($countGoal, $count); ?></td>
                <td><?php writeOddFair($countNoGoal, $count); ?></td>
            </tr>
            <tr>
                <th>Odd Diff</th>
                <td><?php writeOddFairDiff($parentMatch['FT_1'], $count1, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['FT_X'], $countX, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['FT_2'], $count2, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['Under'], $countUnder, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['Over'], $countOver, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['Goal'], $countGoal, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['NoGoal'], $countNoGoal, $count); ?></td>
            </tr>
            <tr>
                <th>Profit</th>
                <td><?php writeProfit($profit1); ?></td>
                <td><?php writeProfit($profitX); ?></td>
                <td><?php writeProfit($profit2); ?></td>
                <td><?php writeProfit($profitUnder); ?></td>
                <td><?php writeProfit($profitOver); ?></td>
                <td><?php writeProfit($profitGoal); ?></td>
                <td><?php writeProfit($profitNoGoal); ?></td>
            </tr>
            <tr>
                <th>Profit Avg</th>
                <td><?php writeProfitAvg($profit1, $count); ?></td>
                <td><?php writeProfitAvg($profitX, $count); ?></td>
                <td><?php writeProfitAvg($profit2, $count); ?></td>
                <td><?php writeProfitAvg($profitUnder, $count); ?></td>
                <td><?php writeProfitAvg($profitOver, $count); ?></td>
                <td><?php writeProfitAvg($profitGoal, $count); ?></td>
                <td><?php writeProfitAvg($profitNoGoal, $count); ?></td>
            </tr>
        </table>
    </div>
    <div id="tabsPrehistory" style="margin: 0px;padding: 0px;">
        <table style="margin: 0px;padding: 0px;white-space: nowrap;">
            <tr>
                <th>#</th>
                <th>StartTime</th>
                <th>Champ</th>
                <th>1</th>
                <th>H</th>
                <th>HomeTeam</th>
                <th>X</th>
                <th>AwayTeam</th>
                <th>H</th>
                <th>2</th>
                <th>U</th>
                <th>O</th>
                <th>GG</th>
                <th>NG</th>
                <th>HT</th>
                <th>FT</th>
            </tr>
        <?php
            $count = 0;
            $count1 = 0;
            $countX = 0;
            $count2 = 0;
            $countUnder = 0;
            $countOver = 0;
            $countGoal = 0;
            $countNoGoal = 0;
            $profit1 = 0;
            $profitX = 0;
            $profit2 = 0;
            $profitUnder = 0;
            $profitOver = 0;
            $profitGoal = 0;
            $profitNoGoal = 0;
            if (count($prehistory) > 0) {
                foreach($prehistory as $match) {
                    $count += 1;
        ?>
            <tr>
                <th><?php echo $count; ?></th>
                <td><?php echo $match['StartTime']; ?></td>
                <td><?php echo $match['Champ']; ?></td>
                <td><?php echo writeOdd($match['FT_1']); ?></td>
                <td><?php echo writeHandicap($match['AdvHome']); ?></td>
                <td><?php echo $match['HomeTeam']; ?></td>
                <td><?php echo writeOdd($match['FT_X']); ?></td>
                <td><?php echo $match['AwayTeam']; ?></td>
                <td><?php echo writeHandicap($match['AdvAway']); ?></td>
                <td><?php echo writeOdd($match['FT_2']); ?></td>
                <td><?php echo writeOdd($match['Under']); ?></td>
                <td><?php echo writeOdd($match['Over']); ?></td>
                <td><?php echo writeOdd($match['Goal']); ?></td>
                <td><?php echo writeOdd($match['NoGoal']); ?></td>
                <td><?php echo $match['ScoreHomeHT'].'-'.$match['ScoreAwayHT']; ?></td>
                <td><?php echo $match['ScoreHomeFT'].'-'.$match['ScoreAwayFT']; ?></td>
            </tr>
        <?php
                    $scoreHomeFT = intval($match['ScoreHomeFT']);
                    $scoreAwayFT = intval($match['ScoreAwayFT']);
                    // 1-X-2 count
                    if ($scoreHomeFT > $scoreAwayFT) {
                        $count1 += 1;
                        $profit1 += $match['FT_1'] - 1;
                        $profitX += -1;
                        $profit2 += -1;
                    }
                    else if ($scoreHomeFT < $scoreAwayFT) {
                        $count2 += 1;
                        $profit1 += -1;
                        $profitX += -1;
                        $profit2 += $match['FT_2'] - 1;
                    }
                    else {
                        $countX += 1;
                        $profit1 += -1;
                        $profitX += $match['FT_X'] - 1;
                        $profit2 += -1;
                    }
                    // Under/Over count
                    if ($scoreHomeFT + $scoreAwayFT > 2.5) {
                        $countOver += 1;
                        $profitOver += $match['Over'] - 1;
                        $profitUnder += -1;
                    } else {
                        $countUnder += 1;
                        $profitOver += -1;
                        $profitUnder += $match['Under'] - 1;
                    }
                    // Goal/NoGoal count
                    if ($scoreHomeFT > 0 && $scoreAwayFT > 0) {
                        $countGoal += 1;
                        $profitGoal += $match['Goal'] - 1;
                        $profitNoGoal += -1;
                    } else {
                        $countNoGoal += 1;
                        $profitGoal += -1;
                        $profitNoGoal += $match['NoGoal'] - 1;
                    }
                }
            } else {
                echo "<tr><td colspan='16'>Nothing found</td></tr>";
            }
        ?>
        </table>
    </div>
    <div id="tabsPrehistoryOverall" style="margin: 0px;padding: 0px;">
        <table style="margin: 0px;padding: 0px;white-space: nowrap;">
            <tr>
                <th><button onclick="matchSelected(<?php echo $match_id; ?>)">Refresh</button></th>
                <th>1</th>
                <th>X</th>
                <th>2</th>
                <th>U</th>
                <th>O</th>
                <th>GG</th>
                <th>NG</th>
                <th>Comment<button onclick="saveComment2(<?php echo $parentMatch['Id']; ?>)" style="float: right;">Save</button></th>
            </tr>
            <tr>
                <th>Success</th>
                <td><?php echo $count1; ?></td>
                <td><?php echo $countX; ?></td>
                <td><?php echo $count2; ?></td>
                <td><?php echo $countUnder; ?></td>
                <td><?php echo $countOver; ?></td>
                <td><?php echo $countGoal; ?></td>
                <td><?php echo $countNoGoal; ?></td>
                <td rowspan="7" style="padding: 0px;margin: 0px;">
                    <textarea id="textarea<?php echo $parentMatch['Id']; ?>" rows="7" cols="50" style="resize: none;"><?php echo $parentMatch['Comment']; ?></textarea>
                </td>
            </tr>
            <tr>
                <th>Percent</th>
                <td><?php writePercentage($count1, $count); ?></td>
                <td><?php writePercentage($countX, $count); ?></td>
                <td><?php writePercentage($count2, $count); ?></td>
                <td><?php writePercentage($countUnder, $count); ?></td>
                <td><?php writePercentage($countOver, $count); ?></td>
                <td><?php writePercentage($countGoal, $count); ?></td>
                <td><?php writePercentage($countNoGoal, $count); ?></td>
            </tr>
            <tr>
                <th>Odd</th>
                <td><?php writeOdd($parentMatch['FT_1']); ?></td>
                <td><?php writeOdd($parentMatch['FT_X']); ?></td>
                <td><?php writeOdd($parentMatch['FT_2']); ?></td>
                <td><?php writeOdd($parentMatch['Under']); ?></td>
                <td><?php writeOdd($parentMatch['Over']); ?></td>
                <td><?php writeOdd($parentMatch['Goal']); ?></td>
                <td><?php writeOdd($parentMatch['NoGoal']); ?></td>
            </tr>
            <tr>
                <th>Fair Odd</th>
                <td><?php writeOddFair($count1, $count); ?></td>
                <td><?php writeOddFair($countX, $count); ?></td>
                <td><?php writeOddFair($count2, $count); ?></td>
                <td><?php writeOddFair($countUnder, $count); ?></td>
                <td><?php writeOddFair($countOver, $count); ?></td>
                <td><?php writeOddFair($countGoal, $count); ?></td>
                <td><?php writeOddFair($countNoGoal, $count); ?></td>
            </tr>
            <tr>
                <th>Odd Diff</th>
                <td><?php writeOddFairDiff($parentMatch['FT_1'], $count1, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['FT_X'], $countX, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['FT_2'], $count2, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['Under'], $countUnder, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['Over'], $countOver, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['Goal'], $countGoal, $count); ?></td>
                <td><?php writeOddFairDiff($parentMatch['NoGoal'], $countNoGoal, $count); ?></td>
            </tr>
            <tr>
                <th>Profit</th>
                <td><?php writeProfit($profit1); ?></td>
                <td><?php writeProfit($profitX); ?></td>
                <td><?php writeProfit($profit2); ?></td>
                <td><?php writeProfit($profitUnder); ?></td>
                <td><?php writeProfit($profitOver); ?></td>
                <td><?php writeProfit($profitGoal); ?></td>
                <td><?php writeProfit($profitNoGoal); ?></td>
            </tr>
            <tr>
                <th>Profit Avg</th>
                <td><?php writeProfitAvg($profit1, $count); ?></td>
                <td><?php writeProfitAvg($profitX, $count); ?></td>
                <td><?php writeProfitAvg($profit2, $count); ?></td>
                <td><?php writeProfitAvg($profitUnder, $count); ?></td>
                <td><?php writeProfitAvg($profitOver, $count); ?></td>
                <td><?php writeProfitAvg($profitGoal, $count); ?></td>
                <td><?php writeProfitAvg($profitNoGoal, $count); ?></td>
            </tr>
        </table>
    </div>
</div>
<?php
    die();
}
$fields = array(
    'Champ' => 'Champ',
    'StartTime' => 'Time',
    'Code' => 'Code',
    'FT_1' => '1',
    'AdvHome' => 'H1',
    'HomeTeam' => 'HomeTeam',
    'FT_X' => 'X',
    'AwayTeam' => 'AwayTeam',
    'AdvAway' => 'H2',
    'FT_2' => '2',
    'Under' => 'U',
    'Over' => 'O',
    'Goal' => 'GG',
    'NoGoal' => 'NG'
);
if (isset($_GET['year'])) $year = $_GET['year'];
if (isset($_GET['day'])) $day = $_GET['day'];
if (isset($_GET['orderby'])) $orderby = $_GET['orderby']; else $orderby = 'StartTime';
if (isset($_GET['champid'])) $champId = $_GET['champid']; else $champId = 0;
?>
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="UTF-8">
        <title>Opap Hacker</title>
        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
        <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/jquery-ui.min.js"></script>
        <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
        <script>
            $(function () {
                // default values
                if (localStorage.getItem("use_handicap") === null) { localStorage.setItem("use_handicap", '0'); }
                if (localStorage.getItem("months_back") === null) { localStorage.setItem("months_back", '12'); }
                if (localStorage.getItem("same_odd1") === null) { localStorage.setItem("same_odd1", '1'); }
                if (localStorage.getItem("same_oddX") === null) { localStorage.setItem("same_oddX", '1'); }
                if (localStorage.getItem("same_odd2") === null) { localStorage.setItem("same_odd2", '1'); }
                if (localStorage.getItem("plus_minus") === null) { localStorage.setItem("plus_minus", '0.00'); }
                // set values
                $('#checkboxChamp').prop('checked', localStorage.getItem("same_champ"));
                $('#checkboxHomeTeam').prop('checked', localStorage.getItem("same_home_team"));
                $('#checkboxAwayTeam').prop('checked', localStorage.getItem("same_away_team"));
                $('#selectHandicap').val(localStorage.getItem("use_handicap"));
                $("#selectMonthsBack").val(localStorage.getItem("months_back"));
                $('#checkboxOdd1').prop('checked', localStorage.getItem("same_odd1"));
                $('#checkboxOddX').prop('checked', localStorage.getItem("same_oddX"));
                $('#checkboxOdd2').prop('checked', localStorage.getItem("same_odd2"));
                $('#checkboxUnder').prop('checked', localStorage.getItem("same_odd_under"));
                $('#checkboxOver').prop('checked', localStorage.getItem("same_odd_over"));
                $('#checkboxGoal').prop('checked', localStorage.getItem("same_odd_goal"));
                $('#checkboxNoGoal').prop('checked', localStorage.getItem("same_odd_nogoal"));
                $("#selectPlusMinus").val(localStorage.getItem("plus_minus"));
            });
            function checkboxChampChanged() { localStorage.setItem("same_champ", $("#checkboxChamp").is(":checked") ? '1' : ''); }
            function checkboxHomeTeamChanged() { localStorage.setItem("same_home_team", $("#checkboxHomeTeam").is(":checked") ? '1' : ''); }
            function checkboxAwayTeamChanged() { localStorage.setItem("same_away_team", $("#checkboxAwayTeam").is(":checked") ? '1' : ''); }
            function selectHandicapChanged() { localStorage.setItem("use_handicap", $("#selectHandicap").val()); }
            function selectMonthsBackChanged() { localStorage.setItem("months_back", $("#selectMonthsBack").val()); }
            function checkboxOdd1Changed() { localStorage.setItem("same_odd1", $("#checkboxOdd1").is(":checked") ? '1' : ''); }
            function checkboxOddXChanged() { localStorage.setItem("same_oddX", $("#checkboxOddX").is(":checked") ? '1' : ''); }
            function checkboxOdd2Changed() { localStorage.setItem("same_odd2", $("#checkboxOdd2").is(":checked") ? '1' : ''); }
            function checkboxUnderChanged() { localStorage.setItem("same_odd_under", $("#checkboxUnder").is(":checked") ? '1' : ''); }
            function checkboxOverChanged() { localStorage.setItem("same_odd_over", $("#checkboxOver").is(":checked") ? '1' : ''); }
            function checkboxGoalChanged() { localStorage.setItem("same_odd_goal", $("#checkboxGoal").is(":checked") ? '1' : ''); }
            function checkboxNoGoalChanged() { localStorage.setItem("same_odd_nogoal", $("#checkboxNoGoal").is(":checked") ? '1' : ''); }
            function selectPlusMinusChanged() { localStorage.setItem("plus_minus", $("#selectPlusMinus").val()); }
            function yearChanged() {
                var year = $("#selectYear").val();
                location.href = '?year=' + year;
            }
            function dayChanged(champid) {
                var year = $("#selectYear").val();
                var day = $("#selectDay").val();
                var orderby = $("#selectOrderBy").val();
                if (typeof champid === 'undefined') champid = $("#selectChamp").val();
                location.href = '?year=' + year + '&day=' + day + '&orderby=' + orderby + '&champid=' + champid;
            }
            function matchSelected(match_id) {
                $("#dialog" + match_id).empty();
                $.post("", {
                    match_id: match_id,
                    same_champ: localStorage.getItem("same_champ"),
                    same_home_team: localStorage.getItem("same_home_team"),
                    same_away_team: localStorage.getItem("same_away_team"),
                    use_handicap: localStorage.getItem("use_handicap"),
                    months_back: localStorage.getItem("months_back"),
                    same_odd1: localStorage.getItem("same_odd1"),
                    same_oddX: localStorage.getItem("same_oddX"),
                    same_odd2: localStorage.getItem("same_odd2"),
                    same_odd_under: localStorage.getItem("same_odd_under"),
                    same_odd_over: localStorage.getItem("same_odd_over"),
                    same_odd_goal: localStorage.getItem("same_odd_goal"),
                    same_odd_nogoal: localStorage.getItem("same_odd_nogoal"),
                    plus_minus: localStorage.getItem("plus_minus")
                })
                .done(function (text) {
                    try {
                        $("#dialog" + match_id).html(text);
                        $("#tabs").tabs();
                    } catch (err) {
                        $("#dialog" + match_id).html(err);
                    } finally {
                        $("#dialog" + match_id).dialog({
                            autoOpen: true
                            , width: 900
                            , height: 480
                        });
                    }
                }, "text");
            }
            function pageRefresh() {
                location.reload(true);
            }
        </script>
        <script>
            function saveComment(match_id, comment) {
                $.post("", {
                    match_id: match_id,
                    comment: comment
                })
                .done(function (text) {
                    alert(text);
                }, "text");
            }
            function saveComment1(match_id) {
                var comment = $("#comment" + match_id).val();
                saveComment(match_id, comment);
            }
            function saveComment2(match_id) {
                var comment = $("#textarea" + match_id).val();
                saveComment(match_id, comment);
                $("#comment" + match_id).val(comment);
            }
            function saveCommentFlag(match_id) {
                var flag = $("#publish" + match_id).is(":checked") ? '1' : '';
                $.post("", {
                    match_id: match_id,
                    comment_flag: flag
                })
                .done(function (text) {
                    if (text != "Done!") alert(text);
                }, "text");
            }
            function saveCommentOdd(match_id) {
                var odd = $("#commentOdd" + match_id).val();
                odd = odd.replace(",", ".").replace("-", "");
                $.post("", {
                    match_id: match_id,
                    comment_odd: odd
                })
                .done(function (text) {
                    if (text != "Done!") alert(text);
                }, "text");
            }
            function saveCommentStatus(match_id) {
                var status = $("#selectCommentStatus" + match_id).val();
                $.post("", {
                    match_id: match_id,
                    comment_status: status
                })
                .done(function (text) {
                    if (text != "Done!") alert(text);
                }, "text");
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
            tr.click:hover {
                cursor: pointer;
            }
            th {
                background-color: gray;
            }
            td {
                text-align: center;
                padding-left: 4px;
                padding-right: 4px;
            }
            .hidden {
                display: none;
            }
            .scroll {
                overflow: scroll;
            }
            label {
                margin-left: 12px;
            }
            .green {
                font-weight: bold;
                color: green;
            }
            .red {
                font-weight: bold;
                color: red;
            }
            .black {
                font-weight: bold;
                color: black;
            }
        </style>
    </head>
    <body>
        <label for="selectYear">Year:</label>
        <select id="selectYear" onchange="yearChanged()">
<?php foreach (getYears() as $record) {
     $iYear = $record['Year'];
     if (empty($year)) $year = $iYear; ?>
            <option<?php if ($year == $iYear) echo ' selected="selected"'; ?>><?php echo $iYear; ?></option>
<?php } ?>
        </select>
        <label for="selectDay">Day:</label>
        <select id="selectDay" onchange="dayChanged(0)">
<?php foreach (getDays($year) as $record) {
     $iDay = $record['Day'];
     $dayText = date('Y-m-d D',strtotime($iDay));
     if (empty($day)) $day = $iDay; ?>
            <option value="<?php echo $iDay; ?>"<?php if ($day == $iDay) echo ' selected="selected"'; ?>><?php echo $dayText; ?></option>
<?php } ?>
        </select>
        <label for="selectChamp">Champ:</label>
        <select id="selectChamp" onchange="dayChanged()">
            <option value="0">(ALL)</option>
<?php foreach (getDayChamps($day) as $record) {
     $iChampId = $record['ChampId']; ?>
            <option value="<?php echo $iChampId; ?>"<?php if ($champId == $iChampId) echo ' selected="selected"'; ?>><?php echo $record['Champ']; ?></option>
<?php } ?>
        </select>
        <label for="selectOrderBy">OrderBy:</label>
        <select id="selectOrderBy" onchange="dayChanged()">
<?php foreach($fields as $field => $name) { ?>
            <option value="<?php echo $field; ?>"<?php if ($orderby == $field) echo ' selected="selected"'; ?>><?php echo $name; ?></option>
<?php } ?>
        </select>
        <a href="../xml/hacker.php?day=<?php echo $day; ?>" target="_blank">Feed</a>
        <button onclick="pageRefresh()">Refresh</button>
        <br />
        <label for="checkboxChamp">Champ</label><input type="checkbox" id="checkboxChamp" onchange="checkboxChampChanged()" />
        <label for="checkboxHomeTeam">HomeTeam</label><input type="checkbox" id="checkboxHomeTeam" onchange="checkboxHomeTeamChanged()" />
        <label for="checkboxAwayTeam">AwayTeam</label><input type="checkbox" id="checkboxAwayTeam" onchange="checkboxAwayTeamChanged()" />
        <label for="selectHandicap">Handicap:</label>
        <select id="selectHandicap" onchange="selectHandicapChanged()">
            <option value="0">Any</option>
            <option value="1">Yes</option>
            <option value="2">No</option>
        </select>
        <label for="selectMonthsBack">Months Back:</label>
        <select id="selectMonthsBack" onchange="selectMonthsBackChanged()">
            <option>1</option>
            <option>2</option>
            <option>3</option>
            <option>6</option>
            <option>9</option>
            <option>12</option>
            <option>18</option>
            <option>24</option>
            <option>36</option>
            <option>48</option>
            <option>60</option>
            <option>72</option>
            <option>84</option>
            <option>96</option>
            <option>108</option>
            <option>120</option>
            <option>132</option>
            <option>144</option>
            <option>156</option>
            <option>168</option>
            <option>180</option>
            <option>192</option>
        </select>
        <br />
        <label for="checkboxOdd1">1</label><input type="checkbox" id="checkboxOdd1" onchange="checkboxOdd1Changed()" />
        <label for="checkboxOddX">X</label><input type="checkbox" id="checkboxOddX" onchange="checkboxOddXChanged()" />
        <label for="checkboxOdd2">2</label><input type="checkbox" id="checkboxOdd2" onchange="checkboxOdd2Changed()" />
        <label for="checkboxUnder">Under</label><input type="checkbox" id="checkboxUnder" onchange="checkboxUnderChanged()" />
        <label for="checkboxOver">Over</label><input type="checkbox" id="checkboxOver" onchange="checkboxOverChanged()" />
        <label for="checkboxGoal">Goal</label><input type="checkbox" id="checkboxGoal" onchange="checkboxGoalChanged()" />
        <label for="checkboxNoGoal">NoGoal</label><input type="checkbox" id="checkboxNoGoal" onchange="checkboxNoGoalChanged()" />
        <label for="selectPlusMinus">&PlusMinus;</label>
        <select id="selectPlusMinus" onchange="selectPlusMinusChanged()">
            <option>0.00</option>
            <option>0.05</option>
            <option>0.10</option>
            <option>0.15</option>
            <option>0.20</option>
            <option>0.25</option>
            <option>0.30</option>
            <option>0.40</option>
            <option>0.50</option>
            <option>0.60</option>
            <option>0.70</option>
            <option>1.00</option>
            <option>1.50</option>
            <option>2.00</option>
            <option>2.50</option>
            <option>3.00</option>
            <option>4.00</option>
            <option>5.00</option>
        </select>
        <br />
        <table>
            <tr>
                <th>Champ</th>
                <th>Time</th>
                <th>Code</th>
                <th>1</th>
                <th>H</th>
                <th>HomeTeam</th>
                <th>X</th>
                <th>AwayTeam</th>
                <th>H</th>
                <th>2</th>
                <th>U</th>
                <th>O</th>
                <th>GG</th>
                <th>NG</th>
                <th>HT</th>
                <th>FT</th>
                <th>Comment</th>
                <th>Odd</th>
                <th>Status</th>
                <th>Publish</th>
            </tr>
<?php foreach (getDayMatches($day, $orderby, $champId) as $record) { ?>
            <tr class="click">
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php echo $record['Champ']; ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php writeStartTime($record['StartTime']); ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php echo $record['Code']; ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php writeOdd($record['FT_1']); ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php writeHandicap($record['AdvHome']); ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php echo $record['HomeTeam']; ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php writeOdd($record['FT_X']); ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php echo $record['AwayTeam']; ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php writeHandicap($record['AdvAway']); ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php writeOdd($record['FT_2']); ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php writeOdd($record['Under']); ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php writeOdd($record['Over']); ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php writeOdd($record['Goal']); ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php writeOdd($record['NoGoal']); ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php echo $record['ScoreHomeHT'].'-'.$record['ScoreAwayHT']; ?></td>
                <td onclick="matchSelected(<?php echo $record['Id']; ?>)"><?php echo $record['ScoreHomeFT'].'-'.$record['ScoreAwayFT']; ?></td>
                <td style="padding: 0px;margin: 0px;">
                    <input type="text" id="comment<?php echo $record['Id']; ?>" value="<?php echo $record['Comment']; ?>" style="width: 300px;" />
                    <button onclick="saveComment1(<?php echo $record['Id']; ?>)" style="padding: 0px;margin: 0px;">Save</button>
                    <div id="dialog<?php echo $record['Id']; ?>" class="" title="<?php writeMatchTitle($record); ?>" style="margin: 2px;padding: 2px;display: none;"></div>
                </td>
                <td>
                    <input type="text" 
                        id="commentOdd<?php echo $record['Id']; ?>" 
                        value="<?php writeOdd($record['CommentOdd']); ?>" 
                        onchange="saveCommentOdd(<?php echo $record['Id']; ?>)" 
                        style="width: 40px; text-align: center;" 
                    />
                </td>
                <td>
                    <select id="selectCommentStatus<?php echo $record['Id']; ?>" onchange="saveCommentStatus(<?php echo $record['Id']; ?>)">
                        <option<?php if ($record['CommentStatus'] == "Pending") echo ' selected="selected"'; ?>>Pending</option>
                        <option<?php if ($record['CommentStatus'] == "Won") echo ' selected="selected"'; ?>>Won</option>
                        <option<?php if ($record['CommentStatus'] == "Lost") echo ' selected="selected"'; ?>>Lost</option>
                        <option<?php if ($record['CommentStatus'] == "Refund") echo ' selected="selected"'; ?>>Refund</option>
                    </select>
                </td>
                <td>
                    <input type="checkbox" id="publish<?php echo $record['Id']; ?>" <?php echo ($record['CommentFlag'] == 1) ? "checked=\"checked\"" : ""; ?> onchange="saveCommentFlag(<?php echo $record['Id']; ?>)" />
                </td>
            </tr>
<?php } ?>
        </table>
    </body>
</html>
<?php // End gzip compressing
while (@ob_end_flush());
?>
