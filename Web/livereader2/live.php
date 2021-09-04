<?php

// start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php

// Init
require_once('authentication.php');
require_once('mix.php');
error_reporting(0);
date_default_timezone_set('Europe/Athens');
header('Content-Type: text/html; charset=utf-8');
?>
<?php

// The coupon table

function htmlTableAdmin($sport, $source, $day, $champ) {
    $matches = GetMatches(strtolower($sport) . '_live_mix', $champ, $day);
    $table = '<table id="TableCoupon">';
    $table .= htmlTableHeadersAdmin();
    $row_count = 0;
    foreach ($matches as $key => $sources) {
        switch ($source) {
            case 'All-Opap':
                if (isset($sources['Opap'])) {
                    $table .= htmlTableRowAdminSingle($sport, $sources['Opap'], $row_count);
                    $row_count++;
                } else if ($sources['Source'] == 'Opap') {
                    $table .= htmlTableRowAdminSingle($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'All-FlashScore':
                if (isset($sources['FlashScore'])) {
                    $table .= htmlTableRowAdminSingle($sport, $sources['FlashScore'], $row_count);
                    $row_count++;
                } else if ($sources['Source'] == 'FlashScore') {
                    $table .= htmlTableRowAdminSingle($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'All-NowGoal':
                if (isset($sources['NowGoal'])) {
                    $table .= htmlTableRowAdminSingle($sport, $sources['NowGoal'], $row_count);
                    $row_count++;
                } else if ($sources['Source'] == 'NowGoal') {
                    $table .= htmlTableRowAdminSingle($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'All-Futbol24':
                if (isset($sources['Futbol24'])) {
                    $table .= htmlTableRowAdminSingle($sport, $sources['Futbol24'], $row_count);
                    $row_count++;
                } else if ($sources['Source'] == 'Futbol24') {
                    $table .= htmlTableRowAdminSingle($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'Matched-Opap':
                if (isset($sources['Opap'])) {
                    $table .= htmlTableRowAdminMulti($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'Matched-FlashScore':
                if (isset($sources['FlashScore'])) {
                    $table .= htmlTableRowAdminMulti($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'Matched-NowGoal':
                if (isset($sources['NowGoal'])) {
                    $table .= htmlTableRowAdminMulti($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'Matched-Futbol24':
                if (isset($sources['Futbol24'])) {
                    $table .= htmlTableRowAdminMulti($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'Unmatched-Opap':
                if ($sources['Source'] == 'Opap') {
                    $table .= htmlTableRowAdminSingle($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'Unmatched-FlashScore':
                if ($sources['Source'] == 'FlashScore') {
                    $table .= htmlTableRowAdminSingle($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'Unmatched-NowGoal':
                if ($sources['Source'] == 'NowGoal') {
                    $table .= htmlTableRowAdminSingle($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
            case 'Unmatched-Futbol24':
                if ($sources['Source'] == 'Futbol24') {
                    $table .= htmlTableRowAdminSingle($sport, $sources, $row_count);
                    $row_count++;
                }
                break;
        }
    }
    $table .= '</table>';
    return $table;
}

function htmlTableHeadersAdmin() {
    return '<tr class="row0">'
            . '<th>PairId</th>'
            . '<th>Source</th>'
            . '<th>Champ</th>'
            . '<th>StartTime</th>'
            . '<th>Code</th>'
            . '<th>HomeTeam</th>'
            . '<th>AwayTeam</th>'
            . '<th>ScoreHT</th>'
            . '<th>Score</th>'
            . '<th>Minute</th>'
            . '<th>Status</th>'
            . '<th>Modified</th>'
            . '</tr>';
}

function htmlTableRowAdminMulti($sport, $sources, $row_count) {
    if (($row_count % 2) == 0) {
        $bg = 'row1';
    } else {
        $bg = 'row2';
    }
    $i = 0;
    foreach ($sources as $name => $source) {
        $row .= '<tr class="' . $bg . '">';
        if ($i == 0) {
            $s = strval(count($sources) + 1);
            $row .= "<td class='{$sport}PairId{$s}' rowspan='{$s}'>{$source['PairId']}</td>";
        }
        $row .= '<td>' . $name . '</td>';
        $row .= writeTdChamp($sport, $source['Champ'], $source['ChampId'], $bg);
        $row .= '<td>' . $source['StartTime'] . '</td>';
        $row .= '<td>' . $source['WebId'] . '</td>';
        $row .= writeTdTeam($sport, $source['HomeTeam'], $source['HomeTeamId'], $bg);
        $row .= writeTdTeam($sport, $source['AwayTeam'], $source['AwayTeamId'], $bg);
        $row .= '<td>' . $source['ScoreHT'] . '</td>';
        $row .= '<td>' . $source['Score'] . '</td>';
        $row .= '<td>' . $source['Minute'] . '</td>';
        $row .= "<td>{$source['Status']} [{$source['StatusId']}]</td>";
        $row .= '<td>' . $source['Modified'] . '</td>';
        $row .= deleteCell($sport, $source['Source'], $source['WebId']);
        $row .= '</tr>';
        $i++;
    }
    if ($sport == 'basket') {
        $mixed = GetMixedMatchBasket($sources['Opap'], $sources['NowGoal']);
    } else {
        $mixed = GetMixedMatchFootball($Opap, $sources['FlashScore'], $sources['Futbol24'], $sources['NowGoal']);
    }
    $row .= '<tr class="' . $bg . ' mixed-row">';
    $row .= '<td>' . '[Mixed]' . '</td>';
    $row .= writeTdChamp($sport, $source['Champ'], $source['ChampId'], $bg);
    $row .= '<td>' . $mixed['StartTime'] . '</td>';
    $row .= '<td>' . $mixed['WebId'] . '</td>';
    $row .= writeTdTeam($sport, $mixed['HomeTeam'], $mixed['HomeTeamId'], $bg);
    $row .= writeTdTeam($sport, $mixed['AwayTeam'], $mixed['AwayTeamId'], $bg);
    $row .= '<td>' . $mixed['ScoreHT'] . '</td>';
    $row .= '<td>' . $mixed['Score'] . '</td>';
    $row .= '<td>' . $mixed['Minute'] . '</td>';
    $row .= '<td>' . $mixed['StatusId'] . '</td>';
    $row .= '<td>' . $mixed['Modified'] . '</td>';
    $row .= '</tr>';
    return $row;
}

function htmlTableRowAdminSingle($sport, $source, $row_count) {
    if (($row_count % 2) == 0) {
        $bg = 'row1';
    } else {
        $bg = 'row2';
    }
    $row .= "<tr class='{$bg}'>";
    if (empty($source['PairId'])) {
        $row .= "<td class='{$sport}PairId1'>Unmatched</td>";
    } else {
        $row .= "<td class='{$sport}PairId'>{$source['PairId']}</td>";
    }
    $row .= '<td>' . $source['Source'] . '</td>';
    $row .= writeTdChamp($sport, $source['Champ'], $source['ChampId'], $bg);
    $row .= '<td>' . $source['StartTime'] . '</td>';
    $row .= '<td>' . $source['WebId'] . '</td>';
    $row .= writeTdTeam($sport, $source['HomeTeam'], $source['HomeTeamId'], $bg);
    $row .= writeTdTeam($sport, $source['AwayTeam'], $source['AwayTeamId'], $bg);
    $row .= '<td>' . $source['ScoreHT'] . '</td>';
    $row .= '<td>' . $source['Score'] . '</td>';
    $row .= '<td>' . $source['Minute'] . '</td>';
    $row .= "<td>{$source['Status']} [{$source['StatusId']}]</td>";
    $row .= '<td>' . $source['Modified'] . '</td>';
    $row .= deleteCell($sport, $source['Source'], $source['WebId']);
    $row .= '</tr>';
    return $row;
}

function writeTdChamp($sport, $name, $id, $bg) {
    //return "<td><input type='text' class='{$bg}' value='{$name}'></td>";
    $td .= '<td>';
    $td .= champLink($sport, $name);
    $td .= inputBox($name, $bg);
    if (isset($id))
        $td .= champIdLink($sport, $id);
    $td .= '</td>';
    return $td;
}

function writeTdTeam($sport, $name, $id, $bg) {
    $td .= '<td>';
    $td .= teamLink($sport, $name);
    $td .= inputBox($name, $bg);
    if (isset($id))
        $td .= teamIdLink($sport, $id);
    $td .= '</td>';
    return $td;
}

function teamLink($sport, $name) {
    $name_enc = urlencode($name);
    return "<a href=\"table_teams.php?Sport={$sport}&Name={$name_enc}\" target=\"teams\">&</a>";
}

function champLink($sport, $name) {
    return "<a href=\"table_champs.php?Sport={$sport}&Name={$name}\" target=\"teams\">&</a>";
}

function inputBox($name, $bg) {
    return "<input type=\"text\" class=\"{$bg}\" value=\"{$name}\">";
}

function teamIdLink($sport, $id) {
    return "<a href=\"table_teams.php?Sport={$sport}&Id={$id}\" target=\"teams\">{$id}</a>";
}

function champIdLink($sport, $id) {
    return "<a href=\"table_champs.php?Sport={$sport}&Id={$id}\" target=\"teams\">{$id}</a>";
}

function deleteCell($sport, $source, $webId) {
    return "<td><button class=\"deleteRow\" onclick=\"deleteRecord('{$sport}','{$source}','{$webId}')\">X</button></td>";
}

?>
<?php

$sport = $_GET['sport'];
$source = $_GET['source'];
$day = $_GET['day'];
$champ = $_GET['champ'];
if (isset($sport) && isset($source)) {
    echo htmlTableAdmin($sport, $source, $day, $champ);
}
?>
<?php

// end gzip compressing
while (@ob_end_flush());
?>