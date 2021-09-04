<?php // Start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
// Dependencies
require_once('database.php');
// Functions
function getYears() {
    
}
function getCoupons() {
    $coupons = array();
    $sql = "SELECT DATE(`StartTime`) AS `StartDate` FROM `football_opap_matches`";
    $sql .= " WHERE `Code`='100'";
    $sql .= " ORDER BY `StartTime` DESC";
    $startDates = mysqlQuerySelect($sql);
    $count = count($startDates);
    for ($i = 0; $i < $count; $i++) {
        $StartDate = $startDates[$i]['StartDate'];
        if ($i > 0) {
            $EndDate = date('Y-m-d', strtotime($startDates[$i - 1]['StartDate'] . ' - 1 day'));
        } else {
            $EndDate = getCouponsLastDay();
        }
        $coupons[] = array('StartDate' => $StartDate, 'EndDate' => $EndDate);
    }
    return $coupons;
}
function getCouponsLastDay() {
    $sql = "SELECT MAX(`StartTime`) AS `LastDate` FROM `football_opap_matches`";
    list($get_date, $get_time) = explode(" ", mysqlQuerySelectFirst($sql)['LastDate']);
    if (strtotime($get_time) < strtotime('08:00:00')) {
       $LastDay = date('Y-m-d', strtotime($get_date . ' - 1 day')); 
    } else {
        $LastDay = $get_date;
    }
    return $LastDay;
}
function getChamps() {
    $sql = "SELECT * FROM `football_champs` ORDER BY `GrNameShort` ASC";
    return mysqlQuerySelect($sql);
}
function getSeasons($champId) {
    $sql = "SELECT * FROM `football_champs_seasons` WHERE `ChampId`='{$champId}' ORDER BY `DateStart` DESC";
    return mysqlQuerySelect($sql);
}
function getMatchList($dateStart, $dateEnd, $champId) {
    $sql = "SELECT * FROM `football_opap_matches` WHERE `Id` > '0'";
    if (!empty($dateStart)) $sql .= " AND DATE(`StartTime`) >= '{$dateStart}'";
    if (!empty($dateEnd)) $sql .= " AND DATE(`StartTime`) <= '{$dateEnd}'";
    if (!empty($champId)) $sql .= " AND `ChampId` = '{$champId}'";
    $sql .= " ORDER BY `StartTime` DESC";
    return mysqlQuerySelect($sql);
}
function getCouponMatches($dateStart, $dateEnd) {
    $sql = "SELECT * FROM `football_opap_matches`";
    $sql .= " WHERE `Code` IS NOT NULL";
    $sql .= " AND `StartTime` >= '{$dateStart}'";
    $sql .= " AND `StartTime` <= '{$dateEnd}'";
    $sql .= " ORDER BY `StartTime` ASC";
    return mysqlQuerySelect($sql);
}
$action = (isset($_GET['action'])) ? $_GET['action'] : '';
switch ($action) {
    case 'get-years':
        header('Content-Type: application/json; charset=utf-8');

        break;
    case 'get-coupons':
        header('Content-Type: application/json; charset=utf-8');
        $coupons = array();
        foreach (getCoupons() as $coupon) {
            $coupons[] = $coupon['StartDate'] . ' > ' . $coupon['EndDate'];
        }
        echo json_encode($coupons);
        break;
    case 'get-matches':
        header('Content-Type: application/json; charset=utf-8');
        $dateStart = date('Y-m-d', strtotime($_GET['date_start'])) . ' 08:00:00';
        $dateEnd = date('Y-m-d', strtotime($_GET['date_end'] . ' + 1 day')) . ' 07:59:59';
        $matches = getCouponMatches($dateStart, $dateEnd);
        echo json_encode($matches);
        //echo $dateStart . ' => ' . $dateEnd . "\n";
        //echo print_r($matches);
        break;
    default:
        header('Content-Type: text/html; charset=utf-8');
?>
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8" />
        <title>Opap Seasons</title>
        <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
        <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/jquery-ui.min.js"></script>
        <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
        <script>
            $(function () {
                $(document).tooltip({
                    track: true
                });
            });
            function doReload(code) {
                switch (code) {
                    case 0:
                        window.location.href = "?couponId=" + couponId();
                        break;
                    case 1:
                        window.location.href = "?champId=" + champId();
                        break;
                    case 2:
                        window.location.href = "?champId=" + champId() + "&seasonId=" + seasonId();
                        break;
                    default:
                        window.location.href = "?champId=" + champId() + "&seasonId=" + seasonId() + "&groupId=" + groupId();
                        break;
                }
            }
            function couponId() {
                var couponId = $("#selectCoupon").val();
                if (couponId == null) return ""; else return couponId;
            }
            function champId() {
                var champId = $("#selectChamp").val();
                if (champId == null) return ""; else return champId;
            }
            function seasonId() {
                var seasonId = $("#selectSeason").val();
                if (seasonId == null) return ""; else return seasonId;
            }
            function groupId() {
                var groupId = $("#selectGroup").val();
                if (groupId == null) return ""; else return groupId;
            }
            function showDialog(id, width) {
                $(id).dialog({
                    autoOpen: true
                    , width: width
                });
            }
        </script>
        <style>
            body {
                font-size: 12px;
                font-family: Verdana;
            }
            table {
                border-collapse: collapse;
                border-spacing: 0px;
                margin-top: 6px;
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
            }
            .hidden {
                display: none;
            }
            .scroll {
                overflow: scroll;
            }
            .click {
                cursor: pointer;
            }
            .red {
                color: red;
            }
            .green {
                color: green;
                font-weight: bold;
            }
        </style>
    </head>
    <body>
        <!-- Coupon -->
        <label for="selectCoupon">Coupon:</label>
        <select id="selectCoupon" onchange="doReload(0)">
            <?php
                $couponId = $_GET['couponId'];
                $coupons = getCoupons();
                foreach($coupons as $coupon) {
                    $couponValue = "{$coupon['StartDate']}:{$coupon['EndDate']}";
                    if ($couponValue == $couponId) {
                        echo "<option value=\"{$couponValue}\" selected=\"selected\">{$coupon['StartDate']} => {$coupon['EndDate']}</option>";
                    }
                    else {
                        echo "<option value=\"{$couponValue}\">{$coupon['StartDate']} => {$coupon['EndDate']}</option>";
                    }
                }
            ?>
        </select>
        <!-- Champ -->
        <label for="selectChamp">Champ:</label>
        <select id="selectChamp" onchange="doReload(1)">
            <?php
                $champId = $_GET['champId'];
                $champs = getChamps();
                foreach ($champs as $champ) {
                    if (empty($champId)) $champId = $champ['Id'];
                    if ($champ['Id'] == $champId) {
                        echo "<option value=\"{$champ['Id']}\" selected=\"selected\">{$champ['GrNameShort']}</option>";
                    }
                    else {
                        echo "<option value=\"{$champ['Id']}\">{$champ['GrNameShort']}</option>";
                    }
                }
            ?>
        </select>
        <!-- Season -->
        <label for="selectSeason">Season:</label>
        <select id="selectSeason" onchange="doReload(2)">
            <?php
                $seasonId = $_GET['seasonId'];
                $seasons = getSeasons($champId);
                foreach ($seasons as $season) {
                    if (empty($seasonId)) { $seasonId = $season['Id']; }
                    if ($season['Id'] == $seasonId) {
                        $dateStart = $season['DateStart'];
                        $dateEnd = $season['DateEnd'];
                        echo "<option value=\"{$season['Id']}\" selected=\"selected\">{$season['Name']}</option>";
                    }
                    else {
                        echo "<option value=\"{$season['Id']}\">{$season['Name']}</option>";
                    }
                }
            ?>
        </select>
        <!-- Group -->
        <label for="selectGroup">Group:</label>
        <select id="selectGroup" onchange="doReload(3)">

        </select>
        <!-- Team -->
        <label for="selectTeam">Team:</label>
        <select id="selectTeam" onchange="doReload(4)">

        </select>
        <!-- Ground -->
        <label for="selectGround">Ground:</label>
        <select id="selectGround" onchange="doReload(5)">

        </select>
        <!-- Refresh Button -->
        <button onclick="doReload()">Refresh</button>
        <!-- Info Button -->
        <button onclick="showDialog('#dialogInfo', 820)">Info</button>
        <!-- Matches Table -->
        <table id="matches">
            <tr>
                <th>#</th>
                <th>StartTime</th>
                <th>Champ</th>
                <th>HomeTeam</th>
                <th>AwayTeam</th>
                <th>Half</th>
                <th>Final</th>
                <th>Status</th>
            </tr>
            <?php
                $seasonMatches = getMatchList($dateStart, $dateEnd, $champId);
                $i = 1;
                foreach ($seasonMatches as $seasonMatch) {
                    echo "<tr>";
                    echo "<th>{$i}</th>";
                    echo "<td>" . substr_replace($seasonMatch['StartTime'], "", -3) . "</td>";
                    echo "<td>{$seasonMatch['Champ']}</td>";
                    echo "<td>{$seasonMatch['HomeTeam']}</td>";
                    echo "<td>{$seasonMatch['AwayTeam']}</td>";
                    echo "<td>{$seasonMatch['ScoreHomeHT']}-{$seasonMatch['ScoreAwayHT']}</td>";
                    echo "<td>{$seasonMatch['ScoreHomeFT']}-{$seasonMatch['ScoreAwayFT']}</td>";
                    echo "<td>{$seasonMatch['Status']}</td>";
                    echo "</tr>";
                    $i++;
                }
            ?>
        </table>
        <!-- Dialog Info -->
        <div id="dialogInfo" class="hidden" title="Payments Info">
            <table>
                <tr>
                    <th rowspan="2">#</th>
                    <th rowspan="2">TeamName</th>
                    <th rowspan="1" colspan="3">Matches</th>
                    <th rowspan="100%"></th>
                    <th rowspan="1" colspan="3">Payments Home</th>
                    <th rowspan="100%"></th>
                    <th rowspan="1" colspan="3">Payments Away</th>
                    <th rowspan="100%"></th>
                    <th rowspan="1" colspan="3">Payments Total</th>
                    <th rowspan="100%"></th>
                    <th rowspan="2">Total</th>
                </tr>
                <tr>
                    <th>Home</th>
                    <th>Away</th>
                    <th>Total</th>
                    <th>FT_1</th>
                    <th>FT_X</th>
                    <th>FT_2</th>
                    <th>FT_1</th>
                    <th>FT_X</th>
                    <th>FT_2</th>
                    <th title="FT_1 Home + FT_2 Away">Win</th>
                    <th title="FT_X Home + FT_X Away">Draw</th>
                    <th title="FT_2 Home + FT_1 Away">Lost</th>
                </tr>
                <?php
                    function newTeamInfo() {
                        return array('HomeMatches' => 0
                        , 'AwayMatches' => 0
                        , 'MatchesHome' => array()
                        , 'MatchesAway' => array()
                        , 'HomePay_FT_1' => 0
                        , 'HomePay_FT_X' => 0
                        , 'HomePay_FT_2' => 0
                        , 'AwayPay_FT_1' => 0
                        , 'AwayPay_FT_X' => 0
                        , 'AwayPay_FT_2' => 0
                        );
                    }
                    function addTeamInfo(&$infoHome, &$infoAway, $match) {
                        // If match has no FT score return
                        if (!isset($match['ScoreHomeFT']) || !isset($match['ScoreAwayFT'])) return;
                        // Init infoHome, infoAway
                        if (!isset($infoHome)) $infoHome = newTeamInfo();
                        if (!isset($infoAway)) $infoAway = newTeamInfo();
                        // Set name
                        if (empty($infoHome['Name'])) {
                            $infoHome['Name'] = $match['HomeTeam'];
                            $infoHome['TeamId'] = $match['HomeTeamId'];
                        }
                        if (empty($infoAway['Name'])) {
                            $infoAway['Name'] = $match['AwayTeam'];
                            $infoAway['TeamId'] = $match['AwayTeamId'];
                        }
                        // Match count
                        $infoHome['HomeMatches'] += 1;
                        $infoAway['AwayMatches'] += 1;
                        // List the matches
                        $infoHome['MatchesHome'][] = $match;
                        $infoAway['MatchesAway'][] = $match;
                        // Score FT with advance
                        $scoreHomeFT = intval($match['ScoreHomeFT']) + intval($match['AdvHome']);
                        $scoreAwayFT = intval($match['ScoreAwayFT']) + intval($match['AdvAway']);
                        // Calc FT_1-X-2 payments
                        if ($scoreHomeFT > $scoreAwayFT) {
                            $infoHome['HomePay_FT_1'] += $match['FT_1'] - 1;
                            $infoHome['HomePay_FT_X'] -= 1;
                            $infoHome['HomePay_FT_2'] -= 1;
                            $infoAway['AwayPay_FT_1'] += $match['FT_1'] - 1;
                            $infoAway['AwayPay_FT_X'] -= 1;
                            $infoAway['AwayPay_FT_2'] -= 1;
                        } else if ($scoreHomeFT == $scoreAwayFT) {
                            $infoHome['HomePay_FT_1'] -= 1;
                            $infoHome['HomePay_FT_X'] += $match['FT_X'] - 1;
                            $infoHome['HomePay_FT_2'] -= 1;
                            $infoAway['AwayPay_FT_1'] -= 1;
                            $infoAway['AwayPay_FT_X'] += $match['FT_X'] - 1;
                            $infoAway['AwayPay_FT_2'] -= 1;
                        } else if ($scoreHomeFT < $scoreAwayFT) {
                            $infoHome['HomePay_FT_1'] -= 1;
                            $infoHome['HomePay_FT_X'] -= 1;
                            $infoHome['HomePay_FT_2'] += $match['FT_2'] - 1;
                            $infoAway['AwayPay_FT_1'] -= 1;
                            $infoAway['AwayPay_FT_X'] -= 1;
                            $infoAway['AwayPay_FT_2'] += $match['FT_2'] - 1;
                        }
                    }
                    function writePayOddTd($odd, $onclick) {
                        if ($odd > 0) {
                            writeOddTd($odd, "green", $onclick);
                        }
                        else {
                            writeOddTd($odd, "red", $onclick);
                        }
                    }
                    function writePayOddTh($odd, $onclick) {
                        if ($odd > 0) {
                            writeOddTh($odd, "green", $onclick);
                        }
                        else {
                            writeOddTh($odd, "red", $onclick);
                        }
                    }
                    function writeOddTd($odd, $color, $onclick) {
                        if (empty($onclick)) {
                            echo "<td class=\"{$color}\">" . number_format($odd, 2) . "</td>";
                        } else {
                            echo "<td class=\"{$color} click\" onclick=\"{$onclick}\">" . number_format($odd, 2) . "</td>";
                        }
                    }
                    function writeOddTh($odd, $color, $onclick) {
                        if (empty($onclick)) {
                            echo "<th class=\"{$color}\">" . number_format($odd, 2) . "</th>";
                        } else {
                            echo "<th class=\"{$color} click\" onclick=\"{$onclick}\">" . number_format($odd, 2) . "</th>";
                        }
                    }
                    function writePayOddMatches($matches, $dialogId, $dialogTitle) {
                        echo "<div id=\"{$dialogId}\" class=\"hidden\" title=\"{$dialogTitle}\">";
                        echo "<table>";
                        echo "<tr>";
                        echo "<th>#</th>";
                        echo "<th>StartTime</th>";
                        echo "<th>Champ</th>";
                        echo "<th>Adv</th>";
                        echo "<th>HomeTeam</th>";
                        echo "<th>AwayTeam</th>";
                        echo "<th>Adv</th>";
                        echo "<th>Half</th>";
                        echo "<th>Final</th>";
                        echo "<th>Status</th>";
                        echo "<th>FT_1</th>";
                        echo "<th>FT_X</th>";
                        echo "<th>FT_2</th>";
                        echo "</tr>";
                        $i = 1;
                        $totalFT_1 = 0;
                        $totalFT_X = 0;
                        $totalFT_2 = 0;
                        foreach ($matches as $match) {
                            // Score FT with advance
                            $scoreHomeFT = intval($match['ScoreHomeFT']) + intval($match['AdvHome']);
                            $scoreAwayFT = intval($match['ScoreAwayFT']) + intval($match['AdvAway']);
                            echo "<tr>";
                            echo "<th>{$i}</th>";
                            echo "<td>" . substr_replace($match['StartTime'], "", -3) . "</td>";
                            echo "<td>{$match['Champ']}</td>";
                            echo "<td>{$match['AdvHome']}</td>";
                            echo "<td>{$match['HomeTeam']}</td>";
                            echo "<td>{$match['AwayTeam']}</td>";
                            echo "<td>{$match['AdvAway']}</td>";
                            echo "<td>{$match['ScoreHomeHT']}-{$match['ScoreAwayHT']}</td>";
                            echo "<td>{$match['ScoreHomeFT']}-{$match['ScoreAwayFT']}</td>";
                            echo "<td>{$match['Status']}</td>";
                            if ($scoreHomeFT > $scoreAwayFT) {
                                writeOddTd($match['FT_1'], "green", "");
                                writeOddTd($match['FT_X'], "red", "");
                                writeOddTd($match['FT_2'], "red", "");
                                $totalFT_1 += $match['FT_1'] - 1;
                                $totalFT_X -= 1;
                                $totalFT_2 -= 1;
                            } else if ($scoreHomeFT == $scoreAwayFT) {
                                writeOddTd($match['FT_1'], "red", "");
                                writeOddTd($match['FT_X'], "green", "");
                                writeOddTd($match['FT_2'], "red", "");
                                $totalFT_1 -= 1;
                                $totalFT_X += $match['FT_X'] - 1;
                                $totalFT_2 -= 1;
                            } else {
                                writeOddTd($match['FT_1'], "red", "");
                                writeOddTd($match['FT_X'], "red", "");
                                writeOddTd($match['FT_2'], "green", "");
                                $totalFT_1 -= 1;
                                $totalFT_X -= 1;
                                $totalFT_2 += $match['FT_2'] - 1;
                            }
                            echo "</tr>";
                            $i++;
                        }
                        echo "<tr>";
                        echo "<th colspan=\"10\" style=\"text-align:right;\">TOTAL:</th>";
                        writePayOddTh($totalFT_1, "");
                        writePayOddTh($totalFT_X, "");
                        writePayOddTh($totalFT_2, "");
                        echo "</tr>";
                        echo "</table>";
                        echo "</div>";
                    }
                    $seasonInfo = array();
                    foreach ($seasonMatches as $seasonMatch) {
                        if (isset($seasonMatch['ScoreHomeFT']) && isset($seasonMatch['ScoreAwayFT'])) {
                            addTeamInfo($seasonInfo[$seasonMatch['HomeTeamId']], $seasonInfo[$seasonMatch['AwayTeamId']], $seasonMatch);
                        }
                    }
                    $i = 1;
                    foreach ($seasonInfo as $seasonTeam) {
                        $dialogHomeTeamId = "dialogHome" . $seasonTeam['TeamId'];
                        $dialogAwayTeamId = "dialogAway" . $seasonTeam['TeamId'];
                        $dialogHomeTitle = $seasonTeam['Name'] . " - Home";
                        $dialogAwayTitle = $seasonTeam['Name'] . " - Away";
                        $onclickHome = "showDialog('#{$dialogHomeTeamId}', 800)";
                        $onclickAway = "showDialog('#{$dialogAwayTeamId}', 800)";
                        echo "<tr>";
                        echo "<th>" . $i;
                        writePayOddMatches($seasonTeam['MatchesHome'], $dialogHomeTeamId, $dialogHomeTitle);
                        writePayOddMatches($seasonTeam['MatchesAway'], $dialogAwayTeamId, $dialogAwayTitle);
                        echo "</th>";
                        echo "<td>" . $seasonTeam['Name'] . "</td>";
                        echo "<td>" . intval($seasonTeam['HomeMatches']) . "</td>";
                        echo "<td>" . intval($seasonTeam['AwayMatches']) . "</td>";
                        echo "<td>" . (intval($seasonTeam['HomeMatches']) + intval($seasonTeam['AwayMatches'])) . "</td>";
                        writePayOddTd($seasonTeam['HomePay_FT_1'], $onclickHome);
                        writePayOddTd($seasonTeam['HomePay_FT_X'], $onclickHome);
                        writePayOddTd($seasonTeam['HomePay_FT_2'], $onclickHome);
                        writePayOddTd($seasonTeam['AwayPay_FT_1'], $onclickAway);
                        writePayOddTd($seasonTeam['AwayPay_FT_X'], $onclickAway);
                        writePayOddTd($seasonTeam['AwayPay_FT_2'], $onclickAway);
                        $totalPayWin = $seasonTeam['HomePay_FT_1'] + $seasonTeam['AwayPay_FT_2'];
                        $totalPayDraw = $seasonTeam['HomePay_FT_X'] + $seasonTeam['AwayPay_FT_X'];
                        $totalPayLost = $seasonTeam['HomePay_FT_2'] + $seasonTeam['AwayPay_FT_1'];
                        writePayOddTd($totalPayWin, "");
                        writePayOddTd($totalPayDraw, "");
                        writePayOddTd($totalPayLost, "");
                        writePayOddTd($totalPayWin + $totalPayDraw + $totalPayLost, "");
                        echo "</tr>";
                        $i++;
                    }
                ?>
            </table>
        </div>
    </body>
</html>
<?php
        break;
}
?>
<?php // End gzip compressing
while (@ob_end_flush());
?>