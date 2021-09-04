<?php
// <editor-fold defaultstate="collapsed" desc="Init">
error_reporting(0);
date_default_timezone_set('Europe/Athens');
header('Content-Type: text/html; charset=utf-8');
require_once 'authenticate.php';
// </editor-fold>
// <editor-fold defaultstate="collapsed" desc="Input">
if (!empty($_GET)) {
    $type = $_GET['type'];
    $id = $_GET['id'];
    $name = urldecode($_GET['name']);
    //echo '<pre>';print_r($_GET);echo '</pre>';
}
if (!empty($_POST)) {
    if (!empty($_POST['InsertChampName'])) {
        $array = $_POST['InsertChampName'];
        // Init fields
	$SportId = empty($array['SportId']) ? '1' : $array['SportId'];
	$ChampId = empty($array['ChampId']) ? CreateChamp($SportId, '0') : $array['ChampId'];
	$CountryId = empty($array['CountryId']) ? '0' : $array['CountryId'];
	$Name = db::$mysqli->real_escape_string($array['Name']);
	$LanguageId = empty($array['LanguageId']) ? '1' : $array['LanguageId'];
	$Flag = db::$mysqli->real_escape_string($array['Flag']);
        // Create command
        $sql = "INSERT INTO `sports`.`champ_names` (`SportId`,`ChampId`,`CountryId`,`Name`,`LanguageId`,`Flag`)";
	$sql .= " VALUES ('{$SportId}','{$ChampId}','{$CountryId}','{$Name}','{$LanguageId}','{$Flag}');";
    }
    else if (!empty($_POST['UpdateChampName'])) {
        $array = $_POST['UpdateChampName'];
        // Init fields
        $Id = $array['Id'];
        $SportId = empty($array['SportId']) ? '1' : $array['SportId'];
        $ChampId = empty($array['ChampId']) ? '0' : $array['ChampId'];
        $CountryId = empty($array['CountryId']) ? '0' : $array['CountryId'];
        $Name = db::$mysqli->real_escape_string($array['Name']);
        $LanguageId = empty($array['LanguageId']) ? '1' : $array['LanguageId'];
        $Flag = db::$mysqli->real_escape_string($array['Flag']);
        // Create command
        $sql = "UPDATE `sports`.`champ_names`";
        $sql .= " SET `SportId` = '{$SportId}', `ChampId` = '{$ChampId}', `CountryId` = '{$CountryId}', `Name` = '{$Name}', `LanguageId` = '{$LanguageId}', `Flag` = '{$Flag}'";
        $sql .= " WHERE `Id` = {$Id};";
    }
    else if (!empty($_POST['DeleteChampName'])) {
        $array = $_POST['DeleteChampName'];
        // Init fields
        $Id = $array['Id'];
        // Create command
        $sql = "DELETE FROM `sports`.`champ_names` WHERE `Id`='{$Id}';";
    }
    else if (!empty($_POST['InsertTeamName'])) {
        $array = $_POST['InsertTeamName'];
        // Init fields
	$sportId = empty($array['SportId']) ? '1' : $array['SportId'];
	$teamId = empty($array['TeamId']) ? CreateTeam($sportId, '0') : $array['TeamId'];
	$champId = empty($array['ChampId']) ? '0' : $array['ChampId'];
	$name = db::$mysqli->real_escape_string($array['Name']);
	$languageId = empty($array['LanguageId']) ? '1' : $array['LanguageId'];
	$flag = db::$mysqli->real_escape_string($array['Flag']);
        // Create command
        $sql = "INSERT INTO `sports`.`team_names` (`SportId`,`TeamId`,`ChampId`,`Name`,`LanguageId`,`Flag`)";
	$sql .= " VALUES ('{$sportId}','{$teamId}','{$champId}','{$name}','{$languageId}','{$flag}');";
    }
    else if (!empty($_POST['UpdateTeamName'])) {
        $array = $_POST['UpdateTeamName'];
        // Init fields
	$Id = $array['Id'];
	$SportId = empty($array['SportId']) ? '1' : $array['SportId'];
	$TeamId = empty($array['TeamId']) ? '0' : $array['TeamId'];
	$ChampId = empty($array['ChampId']) ? '0' : $array['ChampId'];
	$Name = db::$mysqli->real_escape_string($array['Name']);
	$LanguageId = empty($array['LanguageId']) ? '1' : $array['LanguageId'];
	$Flag = db::$mysqli->real_escape_string($array['Flag']);
        // Create command
        $sql = "UPDATE `sports`.`team_names`";
	$sql .= " SET `SportId` = '{$SportId}', `TeamId` = '{$TeamId}', `ChampId` = '{$ChampId}', `Name` = '{$Name}', `LanguageId` = '{$LanguageId}', `Flag` = '{$Flag}'";
	$sql .= " WHERE `Id` = {$Id};";
    }
    else if (!empty($_POST['DeleteTeamName'])) {
        $array = $_POST['DeleteTeamName'];
        // Init fields
        $Id = $array['Id'];
        // Create command
        $sql = "DELETE FROM `sports`.`team_names` WHERE `Id`='{$Id}';";
    }
    // If command exists
    if (!empty($sql)) {
        db::executeNonQuery($sql);
    }
    die();
}
function CreateChamp($sportId, $countryId) {
    $champId = 0;
    // Create command
    $sql = "INSERT INTO `sports`.`champs` (`SportId`,`CountryId`) VALUES ('{$sportId}', '{$countryId}');";
    // Execute
    $champId = db::executeNonQuery($sql);
    // Return last insert id
    return $champId;
}
function CreateTeam($sportId, $groundId) {
    $teamId = 0;
    // Create command
    $sql = "INSERT INTO `sports`.`teams` (`SportId`,`GroundId`) VALUES ('{$sportId}', '{$groundId}');";
    // Execute the command (query is not created for insert command)
    $teamId = db::executeNonQuery($sql);
    // Return last insert id
    return $teamId;
}
// </editor-fold>
// <editor-fold defaultstate="collapsed" desc="Teams/Names">
if (isset($type) && $type == 'team' && isset($id)) {
    $sql = "SELECT * FROM `sports`.`team_names` WHERE `TeamId`='{$id}'";
} else if (isset($type) && $type == 'champ' && isset($id)) {
    $sql = "SELECT * FROM `sports`.`champ_names` WHERE `ChampId`='{$id}'";
} else if (isset($type) && $type == 'team' && isset($name)) {
    $sql = "SELECT * FROM `sports`.`team_names` WHERE `Name` LIKE '" . db::$mysqli->real_escape_string($name) . "'";
} else if (isset($type) && $type == 'champ' && isset($name)) {
    $sql = "SELECT * FROM `sports`.`champ_names` WHERE `Name` LIKE '" . db::$mysqli->real_escape_string($name) . "'";
} else {
    die("Unknown Input");
}
// Execute
$records = db::getRecords($sql);
//echo "<pre>";print_r($records);echo "</pre>";
// </editor-fold>
// <editor-fold defaultstate="collapsed" desc="Functions (Html)">
function writeChampRecords($records) {
    echo '<table>';
    echo "<tr class=\"row0\">";
    echo "<th class=\"width:50px;\">Id</th>";
    echo "<th class=\"width:50px;\">SportId</th>";
    echo "<th class=\"width:50px;\">ChampId</th>";
    echo "<th class=\"width:50px;\">CountryId</th>";
    echo "<th class=\"width:150px;\">Name</th>";
    echo "<th class=\"width:50px;\">LangId</th>";
    echo "<th class=\"width:50px;\">Flag</th>";
    echo "</tr>";
    $lastSportId = '';
    $lastChampId = '';
    foreach ($records as $record) {
        echo "<tr class=\"row1\">";
        echo "<th class=\"row0\">" . $record['Id'] . "</th>";
        echo "<td>" . textBox("sportId_{$record['Id']}", $record['SportId'], 'center') . "</th>";
        echo "<td>" . writeChampId($record['Id'], $record['ChampId']) . "</th>";
        echo "<td>" . textBox("countryId_{$record['Id']}", $record['CountryId']) . "</th>";
        echo "<td>" . writeChampName($record['Id'], $record['Name']) . "</th>";
        echo "<td>" . textBox("langId_{$record['Id']}", $record['LanguageId'], 'center') . "</th>";
        echo "<td>" . textBox("flag_{$record['Id']}", $record['Flag'], 'center') . "</th>";
        echo "<td>";
        echo "<button class=\"updateRecord\" onclick=\"updateChampName({$record['Id']})\">V</button>";
        echo "<button class=\"deleteRecord\" onclick=\"deleteChampName({$record['Id']})\">X</button>";
        echo "</td>";
        echo "</tr>";
        $lastSportId = $record['SportId'];
        $lastChampId = $record['ChampId'];
    }
    echo "<tr class=\"row1\">";
    echo "<th class=\"row0\">=></th>";
    echo "<td>" . textBox("sportId_0", $lastSportId, 'center') . "</th>";
    echo "<td>" . writeChampId(0, $lastChampId) . "</th>";
    echo "<td>" . textBox("countryId_0", '') . "</th>";
    echo "<td>" . writeChampName(0, '') . "</th>";
    echo "<td>" . textBox("langId_0", '', 'center') . "</th>";
    echo "<td>" . textBox("flag_0", '', 'center') . "</th>";
    echo "<td>";
    echo "<button class=\"updateRecord\" onclick=\"insertChampName()\">V</button>";
    echo "</td>";
    echo "</tr>";
    echo '</table>';
}
function writeTeamRecords($records) {
    echo "<table>";
    echo "<tr class=\"row0\">";
    echo "<th class=\"width:50px;\">Id</th>";
    echo "<th style=\"width:50px;\">SportId</th>";
    echo "<th style=\"width:50px;\">TeamId</th>";
    echo "<th style=\"width:50px;\">ChampId</th>";
    echo "<th style=\"width:150px;\">Name</th>";
    echo "<th style=\"width:50px;\">LangId</th>";
    echo "<th style=\"width:50px;\">Flag</th>";
    echo "</tr>";
    $lastSportId = '';
    $lastTeamId = '';
    foreach ($records as $record) {
        echo "<tr class=\"row1\">";
        echo "<th class=\"row0\">" . $record['Id'] . "</th>";
        echo "<td>" . textBox("sportId_{$record['Id']}", $record['SportId'], 'center') . "</th>";
        echo "<td>" . writeTeamId($record['Id'], $record['TeamId']) . "</th>";
        echo "<td>" . writeChampId($record['Id'], $record['ChampId']) . "</th>";
        echo "<td>" . writeTeamName($record['Id'], $record['Name']) . "</th>";
        echo "<td>" . textBox("langId_{$record['Id']}", $record['LanguageId'], 'center') . "</th>";
        echo "<td>" . textBox("flag_{$record['Id']}", $record['Flag'], 'center') . "</th>";
        echo "<td>";
        echo "<button class=\"updateRecord\" onclick=\"updateTeamName({$record['Id']})\">V</button>";
        echo "<button class=\"deleteRecord\" onclick=\"deleteTeamName({$record['Id']})\">X</button>";
        echo "</td>";
        echo "</tr>";
        $lastSportId = $record['SportId'];
        $lastTeamId = $record['TeamId'];
    }
    echo "<tr class=\"row1\">";
    echo "<th class=\"row0\">=></th>";
    echo "<td>" . textBox("sportId_0", $lastSportId, 'center') . "</th>";
    echo "<td>" . writeTeamId(0, $lastTeamId) . "</th>";
    echo "<td>" . writeChampId(0, '') . "</th>";
    echo "<td>" . writeTeamName(0, '') . "</th>";
    echo "<td>" . textBox("langId_0", '', 'center') . "</th>";
    echo "<td>" . textBox("flag_0", '', 'center') . "</th>";
    echo "<td>";
    echo "<button class=\"updateRecord\" onclick=\"insertTeamName()\">V</button>";
    echo "</td>";
    echo "</tr>";
    echo "</table>";
}
function writeChampId($id, $champId) {
    return linkChampId($champId) . textBox("champId_{$id}", $champId);
}
function writeChampName($id, $champName) {
    return linkChampName($champName) . textBox("champName_{$id}", $champName);
}
function writeTeamId($id, $teamId) {
    return linkTeamId($teamId) . textBox("teamId_{$id}", $teamId);
}
function writeTeamName($id, $teamName) {
    return linkTeamName($teamName) . textBox("teamName_{$id}", $teamName);
}
function linkChampId($champId) {
    if (!empty($champId)) {
        return "<a href=\"?type=champ&id={$champId}\">$</a>";
    }
}
function linkTeamId($teamId) {
    if (!empty($teamId)) {
        return "<a href=\"?type=team&id={$teamId}\">$</a>";
    }
}
function linkChampName($champName) {
    if (!empty($champName)) {
        return "<a href=\"?type=champ&name={$champName}\">&</a>";
    }
}
function linkTeamName($teamName) {
    if (!empty($teamName)) {
        return "<a href=\"?type=team&name={$teamName}\">&</a>";
    }
}
function textBox($id, $text, $class){
    return "<input type=\"text\" id=\"{$id}\" value=\"{$text}\" class=\"{$class}\" />";
}
// </editor-fold>
?>
<!DOCTYPE html>
<html>
    <head>
        <title>Names</title>
        <meta charset="UTF-8">
        <script src="//code.jquery.com/jquery-3.2.1.min.js"></script>
        <script src="//code.jquery.com/ui/1.12.1/jquery-ui.min.js"></script>
        <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/cupertino/jquery-ui.css">
        <style>
            .ui-widget {
                font-size: 95%;
            }
        </style>
        <style>
            body {
                font-family: "Lucida Grande","Lucida Sans","Arial","sans-serif";
                font-size: 12px;
                margin: 0px;
            }
            table {
                table-layout: fixed;
                white-space: nowrap;
                border-collapse: collapse;
            }
            table, td, th, tr {
                border: 1px solid black;
                margin: 0px;
                padding: 0px;
            }
            button {
                margin: 0px;
                padding: 0px;
            }
            input {
                border-width: 0px;
                margin: 0px;
                padding: 0px;
                background-color: transparent;
                width: 100%;
            }
            a:link {
                color: blue;
            }
            a:visited {
                color: blue;
            }
            a:hover {
                color: indigo;
            }
            a:active {
                color: red;
            }
            .row0 {
                background-color: #B3B3B3;
            }
            .row1 {
                background-color: #E6E6E6;
            }
            .row2 {
                background-color: #CCCCCC;
            }
            .updateRecord {
                color: green;
                font-weight: bold;
            }
            .deleteRecord {
                color: red;
                font-weight: bold;
            }
            .center {
                text-align: center;
            }
        </style>
        <script>
            $(function () {
                //$("select").selectmenu();
                //$("button").button();
            });
            function insertChampName() {
                $.post("", {
                    'InsertChampName': {
                        'SportId': $('#sportId_0').val(),
                        'ChampId': $('#champId_0').val(),
                        'CountryId': $('#countryId_0').val(),
                        'Name': $('#champName_0').val(),
                        'LanguageId': $('#langId_0').val(),
                        'Flag': $('#flag_0').val()
                    }
                },
                function(result){
                    if (result) alert(result);
                    location.reload(true);
                });
            }
            function updateChampName(id) {
                $.post("", {
                    'UpdateChampName': {
                        'Id': id,
                        'SportId': $('#sportId_' + id).val(),
                        'ChampId': $('#champId_' + id).val(),
                        'CountryId': $('#countryId_' + id).val(),
                        'Name': $('#champName_' + id).val(),
                        'LanguageId': $('#langId_' + id).val(),
                        'Flag': $('#flag_' + id).val()
                    }
                },
                function(result){
                    if (result) alert(result);
                    location.reload(true);
                });
            }
            function deleteChampName(id) {
                $.post("", {
                    'DeleteChampName': {
                        'Id': id
                    }
                },
                function(result){
                    if (result) alert(result);
                    location.reload(true);
                });
            }
            function insertTeamName() {
                $.post("", {
                    'InsertTeamName': {
                        'SportId': $('#sportId_0').val(),
                        'TeamId': $('#teamId_0').val(),
                        'ChampId': $('#champId_0').val(),
                        'Name': $('#teamName_0').val(),
                        'LanguageId': $('#langId_0').val(),
                        'Flag': $('#flag_0').val()
                    }
                },
                function(result){
                    if (result) alert(result);
                    location.reload(true);
                });
            }
            function updateTeamName(id) {
                $.post("", {
                    'UpdateTeamName': {
                        'Id': id,
                        'SportId': $('#sportId_' + id).val(),
                        'TeamId': $('#teamId_' + id).val(),
                        'ChampId': $('#champId_' + id).val(),
                        'Name': $('#teamName_' + id).val(),
                        'LanguageId': $('#langId_' + id).val(),
                        'Flag': $('#flag_' + id).val()
                    }
                },
                function(result){
                    if (result) alert(result);
                    location.reload(true);
                });
            }
            function deleteTeamName(id) {
                $.post("", {
                    'DeleteTeamName': {
                        'Id': id
                    }
                },
                function(result){
                    if (result) alert(result);
                    location.reload(true);
                });
            }
        </script>
    </head>
    <body>
        <?php
        // <editor-fold defaultstate="collapsed" desc="Html">
        if (isset($type) && $type == 'team' && isset($id)) {
            writeTeamRecords($records);
        } else if (isset($type) && $type == 'champ' && isset($id)) {
            writeChampRecords($records);
        } else if (isset($type) && $type == 'team' && isset($name)) {
            writeTeamRecords($records);
        } else if (isset($type) && $type == 'champ' && isset($name)) {
            writeChampRecords($records);
        } else {
            die("Unknown Input");
        }
        //echo "<pre>";print_r($records);echo "</pre>";
        // </editor-fold>
        ?>
    </body>
</html>
