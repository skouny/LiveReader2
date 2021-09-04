<?php
// <editor-fold defaultstate="collapsed" desc="Init">
error_reporting(0);
date_default_timezone_set('Europe/Athens');
header('Content-Type: text/html; charset=utf-8');
require_once 'authenticate.php';
// </editor-fold>
// <editor-fold defaultstate="collapsed" desc="Input">
if (!empty($_GET)) {
    $sport = (empty($_GET['sport'])) ? 'Football' : $_GET['sport'];
    $day = (empty($_GET['day'])) ? '' : $_GET['day'];
    $source = (empty($_GET['source'])) ? '' : $_GET['source'];
    $champ = (empty($_GET['champ'])) ? '' : $_GET['champ'];
}
if (!empty($_POST)) {
    if (!empty($_POST['RefreshRecord'])) {
        $array = $_POST['RefreshRecord'];
        $sport = $array['Sport'];
        $source = $array['Source'];
        $webId = $array['WebId'];
        if (!empty($sport) && !empty($source) && !empty($webId)) {
            $table = strtolower("{$sport}_live_mix");
            $sportId = (strtolower($sport) == 'basket') ? 2 : 1;
            $sql = "UPDATE `{$table}`";
            $sql .= " SET `PairId` = 'REFRESH'";
            $sql .= " WHERE `Source`='{$source}' AND `WebId`='{$webId}';";
        }
    } else if (!empty($_POST['DeleteRecord'])) {
        $array = $_POST['DeleteRecord'];
        $sport = $array['Sport'];
        $source = $array['Source'];
        $webId = $array['WebId'];
        if (!empty($sport) && !empty($source) && !empty($webId)){
            $table = strtolower("{$sport}_live_mix");
            $sql = "DELETE FROM `{$table}` WHERE `Source`='{$source}' AND `WebId`='{$webId}';";
        }
    }
    // If command exists
    if (!empty($sql)) {
        db::executeNonQuery($sql);
    }
    die();
}
// </editor-fold>
// <editor-fold defaultstate="collapsed" desc="Functions (Matches)">
function findGroupedMatch(&$grouped_matches, &$match) {
    foreach ($grouped_matches as $key => $grouped_match) {
        if (groupedMatchContains($grouped_match, $match)) {
            return $key;
        }
    }
    return 0;
}
function groupedMatchContains(&$grouped_match, &$match) {
    $count = 0;
    $mixed_match = $grouped_match['*'];
    if (isset($mixed_match) && isset($match)) {
        // Champ
        if (!empty($match['ChampId']) && !empty($mixed_match['ChampId']) && $match['ChampId'] == $mixed_match['ChampId']) {
            $count += 1;
        }
        // Straight
        if (!empty($match['HomeTeamId']) && !empty($mixed_match['HomeTeamId']) && $match['HomeTeamId'] == $mixed_match['HomeTeamId']) {
            $count += 1;
        }
        if (!empty($match['AwayTeamId']) && !empty($mixed_match['AwayTeamId']) && $match['AwayTeamId'] == $mixed_match['AwayTeamId']) {
            $count += 1;
        }
        // Reversed
        if (!empty($match['HomeTeamId']) && !empty($mixed_match['AwayTeamId']) && $match['HomeTeamId'] == $mixed_match['AwayTeamId']) {
            $count += 1;
        }
        if (!empty($match['AwayTeamId']) && !empty($mixed_match['HomeTeamId']) && $match['AwayTeamId'] == $mixed_match['HomeTeamId']) {
            $count += 1;
        }
        // Decide
        if ($count >= 2) {
            return TRUE;
        }
    }
    return FALSE;
}
function updateGroupedMatch(&$grouped_match, &$match) {
    $grouped_match['*'] = updateMixedMatch($grouped_match['*'], $match);
    $grouped_match[$match['Source']] = $match;
    return $grouped_match;
}
function createGroupedMatch(&$match) {
    $grouped_match = array();
    $grouped_match['*'] = createMixedMatch($match);
    $grouped_match[$match['Source']] = $match;
    return $grouped_match;
}
function createMixedMatch(&$match) {
    $mixed_match = array();
    $mixed_match['ChampId'] = $match['ChampId'];
    $mixed_match['HomeTeamId'] = $match['HomeTeamId'];
    $mixed_match['AwayTeamId'] = $match['AwayTeamId'];
    return $mixed_match;
}
function updateMixedMatch(&$mixed_match, &$match) {
    if (empty($mixed_match['ChampId']) && !empty($match['ChampId'])) {
        $mixed_match['ChampId'] = $match['ChampId'];
    }
    if (empty($mixed_match['HomeTeamId']) && !empty($match['HomeTeamId'])) {
        $mixed_match['HomeTeamId'] = $match['HomeTeamId'];
    }
    if (!empty($mixed_match['HomeTeamId']) && !empty($match['HomeTeamId']) && $mixed_match['HomeTeamId'] !== $match['HomeTeamId']) {
        $mixed_match['HomeTeamId'] = "0";
    }
    if (empty($mixed_match['AwayTeamId']) && !empty($match['AwayTeamId'])) {
        $mixed_match['AwayTeamId'] = $match['AwayTeamId'];
    }
    if (!empty($mixed_match['AwayTeamId']) && !empty($match['AwayTeamId']) && $mixed_match['AwayTeamId'] !== $match['AwayTeamId']) {
        $mixed_match['AwayTeamId'] = "0";
    }
    return $mixed_match;
}
// </editor-fold>
// <editor-fold defaultstate="collapsed" desc="Matches">
$table = strtolower("{$sport}_live_mix");
$sql = "SELECT * FROM `{$table}`";
$sql .= " WHERE `Source` IS NOT NULL";
if (!empty($day)) {
    $sql .= " AND HUMAN_DAY(`StartTime`) = '{$day}'";
}
$sql .= " ORDER BY (TIME(`StartTime`) = '08:00:00'), `StartTime`, `Source` DESC;";
// Execute the command
db::executeQuery($sql);
$grouped_matches = array();
$maxSources = 0;
while ($match = db::$query->fetch_assoc()) {
    // Find grouped match
    $key = findGroupedMatch($grouped_matches, $match);
    if ($key) { // If found, insert
        $grouped_matches[$key] = updateGroupedMatch($grouped_matches[$key], $match);
    } else {// If not found, add new match group
        $key = count($grouped_matches) + 1;
        $grouped_matches[$key] = createGroupedMatch($match);
    }
    $sources_count = count($grouped_matches[$key]);
    if ($maxSources < $sources_count) $maxSources = $sources_count;
}
// Free query set
db::freeQuery();
//echo "<pre>";print_r($grouped_matches);echo "</pre>";
// </editor-fold>
// <editor-fold defaultstate="collapsed" desc="Functions (Html)">
function writeChamp($name, $id) {
    $cssClass = "";
    if (empty($id)) {
        $cssClass .= "red";
    }
    return linkChamp($name) . textBox($name, $cssClass) . linkChampId($id);
}
function writeTeam($name, $id) {
    $cssClass = "";
    if (empty($id)) {
        $cssClass .= "red";
    }
    return linkTeam($name) . textBox($name, $cssClass) . linkTeamId($id, $cssClass);
}
function textBox($text, $cssClass){
    return "<input type=\"text\" value=\"{$text}\" class=\"{$cssClass}\" />";
}
function linkChampId($id){
    return "<a href=\"#\" onclick=\"openChampById({$id});return false;\">{$id}</a>";
}
function linkChamp($name){
    return "<a href=\"#\" onclick=\"openChampByName('" . urlencode($name) . "');return false;\">&</a>";
}
function linkTeamId($id, $cssClass){
    return "<a href=\"#\" onclick=\"openTeamById({$id});return false;\" class=\"{$cssClass}\">{$id}</a>";
}
function linkTeam($name){
    return "<a href=\"#\" onclick=\"openTeamByName('" . urlencode($name) . "');return false;\">&</a>";
}
// </editor-fold>
// <editor-fold defaultstate="collapsed" desc="Html">
echo '<table>';
echo "<tr class=\"row0\">";
echo "<th>#</th>";
echo "<th>Source</th>";
echo "<th>Champ</th>";
echo "<th>StartTime</th>";
echo "<th>Code</th>";
echo "<th>HomeTeam</th>";
echo "<th>AwayTeam</th>";
echo "<th>ScoreHT</th>";
echo "<th>Score</th>";
echo "<th>Minute</th>";
echo "<th>Status</th>";
echo "<th>Modified</th>";
echo "</tr>";
foreach ($grouped_matches as $index => $grouped_match) {
    if (empty($source) || array_key_exists($source, $grouped_match)) {
        if (empty($source) || empty($champ) || $champ == '[ALL]' || $grouped_match[$source]['Champ'] == $champ) {
            if (isset($rowClass) && $rowClass == 'row1') {$rowClass = 'row2';} else {$rowClass = 'row1';}
            foreach ($grouped_match as $key => $match) {
                echo "<tr class=\"{$rowClass}\">";
                if ($key == '*') {
                    $rowspan = count($grouped_match);
                    if (isset($i)){ $i++; } else{ $i = 1; }
                    if ($rowspan == 2) {
                        $iClass = "red";
                    } else if ($maxSources == $rowspan) {
                         $iClass = "green";
                    } else {
                        $iClass = "orange";
                    }
                    echo "<td rowspan=\"{$rowspan}\" class=\"aa {$iClass}\">{$i}</td>";
                }
                echo "<td>" . $key . "</td>";
                echo "<td>" . writeChamp($match['Champ'], $match['ChampId']) . "</td>";
                echo "<td>" . $match['StartTime'] . "</td>";
                echo "<td>" . $match['WebId'] . "</td>";
                echo "<td>" . writeTeam($match['HomeTeam'], $match['HomeTeamId']) . "</td>";
                echo "<td>" . writeTeam($match['AwayTeam'], $match['AwayTeamId']) . "</td>";
                echo "<td>" . $match['ScoreHT'] . "</td>";
                echo "<td>" . $match['Score'] . "</td>";
                echo "<td>" . $match['Minute'] . "</td>";
                echo "<td>" . $match['Status'] . "[" . $match['StatusId'] . "] </td>";
                echo "<td>" . $match['Modified'] . "</td>";
                if ($key != '*') {
                    echo "<td>";
                    echo "<button class=\"refreshRecord\" onclick=\"refreshRecord('{$sport}','{$key}','{$match['WebId']}')\">R</button>";
                    echo "<button class=\"deleteRecord\" onclick=\"deleteRecord('{$sport}','{$key}','{$match['WebId']}')\">X</button>";
                    echo "</td>";
                } else {
                    //echo "<td>";
                    //echo "<button onclick=\"showDialog('".base64_encode("<div><pre>".print_r($grouped_match, TRUE)."</pre></div>")."', 600, 400)\">J</button>";
                    //echo "</td>";
                }
                echo "</tr>";
            }
        }
    }
}
echo '</table>';
// </editor-fold>
