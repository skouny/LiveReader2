<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php // init and functions
require_once('authentication.php');
date_default_timezone_set('Europe/Athens');
header('Content-Type: text/html; charset=utf-8');
define('DB_HOST', 'localhost');
define('DB_USER', 'myroot');
define('DB_PASS', 'zJp4$81s');
function GetSportsRecords($ChampId, $ChampName, $CountryId, $SportId) {
	// Connect to database
	$mysqli = new mysqli(DB_HOST, DB_USER, DB_PASS, 'sports');
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
	// Create SQL string
	if (isset($ChampId)) {
		$sql = "SELECT * FROM `champ_names` WHERE `ChampId`='{$ChampId}'";
	}
	else if (isset($ChampName)) {
		$sql = "SELECT * FROM `champ_names` WHERE `Name` LIKE '{$mysqli->real_escape_string($ChampName)}'";
		if (isset($CountryId)) $sql .= " AND `CountryId` = '{$CountryId}'";
		else if (isset($SportId)) $sql .= " AND `SportId` = '{$SportId}'";
	}
	// Execute the command
	if (isset($sql)) {
		$query = $mysqli->query($sql);
		// If execution failed
		if ($mysqli->errno) {
			echo "Failed to execute query!\n\n";
			echo "ERROR ({$mysqli->errno}): {$mysqli->error}\n\n";
			echo "SQL COMMAND: {$sql}";
		}
		$rows = array();
		while($row = $query->fetch_assoc()) {
			$rows[] = $row;
		}
		// free query set
		$query->free();
	}
	// Close the connection
	$mysqli->close();
	// return
	if (isset($rows)) {
		return $rows;
	} else {
		return null;
	}
}
function CreateChamp($SportId, $CountryId) {
	// Connect to database
	$mysqli = new mysqli(DB_HOST, DB_USER, DB_PASS, 'sports');
	$ChampId = 0;
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
    // Create command
    $sql = "INSERT INTO `champs` (`SportId`,`CountryId`) VALUES ('{$SportId}', '{$CountryId}');";
	// Execute the command
	$query = $mysqli->query($sql);
	// If execution failed
	if ($mysqli->errno) {
        echo "Failed to execute query!\n\n";
        echo "ERROR ({$mysqli->errno}): {$mysqli->error}\n\n";
        echo "SQL COMMAND: {$sql}";
	} else {
		$ChampId = $mysqli->insert_id;
	}
	// Close the connection
	$mysqli->close();
	// Return last insert id
	return $ChampId;
}
function InsertRecord($array) {
	// Connect to database
	$mysqli = new mysqli(DB_HOST, DB_USER, DB_PASS, 'sports');
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
    // Create command
    $sql = "INSERT INTO `champ_names` (`SportId`,`ChampId`,`CountryId`,`Name`,`LanguageId`,`Flag`)";
	$SportId = empty($array['SportId']) ? '1' : $array['SportId'];
	$ChampId = empty($array['ChampId']) ? CreateChamp($SportId, '0') : $array['ChampId'];
	$CountryId = empty($array['CountryId']) ? '0' : $array['CountryId'];
	$Name = $mysqli->real_escape_string($array['Name']);
	$LanguageId = empty($array['LanguageId']) ? '1' : $array['LanguageId'];
	$Flag = $mysqli->real_escape_string($array['Flag']);
	$sql .= " VALUES ('{$SportId}','{$ChampId}','{$CountryId}','{$Name}','{$LanguageId}','{$Flag}');";
	// Execute the command
	$query = $mysqli->query($sql);
	// If execution failed
	if ($mysqli->errno) {
        echo "Failed to execute query!\n\n";
        echo "ERROR ({$mysqli->errno}): {$mysqli->error}\n\n";
        echo "SQL COMMAND: {$sql}";
	}
	// Close the connection
	$mysqli->close();
}
function UpdateRecord($array) {
	// Connect to database
	$mysqli = new mysqli(DB_HOST, DB_USER, DB_PASS, 'sports');
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
    // Create command
    $sql = "UPDATE `champ_names`";
	$Id = $array['Id'];
	$SportId = empty($array['SportId']) ? '1' : $array['SportId'];
	$ChampId = empty($array['ChampId']) ? '0' : $array['ChampId'];
	$CountryId = empty($array['CountryId']) ? '0' : $array['CountryId'];
	$Name = $mysqli->real_escape_string($array['Name']);
	$LanguageId = empty($array['LanguageId']) ? '1' : $array['LanguageId'];
	$Flag = $mysqli->real_escape_string($array['Flag']);
	$sql .= " SET `SportId` = '{$SportId}', `ChampId` = '{$ChampId}', `CountryId` = '{$CountryId}', `Name` = '{$Name}', `LanguageId` = '{$LanguageId}', `Flag` = '{$Flag}'";
	$sql .= " WHERE `Id` = {$Id};";
	// Execute the command
	$query = $mysqli->query($sql);
	// If execution failed
	if ($mysqli->errno) {
        echo "Failed to execute query!\n\n";
        echo "ERROR ({$mysqli->errno}): {$mysqli->error}\n\n";
        echo "SQL COMMAND: {$sql}";
	}
	// Close the connection
	$mysqli->close();
}
function DeleteRecord($id) {
	// Connect to database
	$mysqli = new mysqli(DB_HOST, DB_USER, DB_PASS, 'sports');
	// If connection failed die...
	if ($mysqli->connect_errno) {
		die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
	}
	// Change character set to utf8
	$mysqli->set_charset('utf8');
    // Create command
    $sql = "DELETE FROM `champ_names` WHERE `Id`='{$id}';";
	// Execute the command
	$query = $mysqli->query($sql);
	// If execution failed
	if ($mysqli->errno) {
        echo "Failed to execute query!\n\n";
        echo "ERROR ({$mysqli->errno}): {$mysqli->error}\n\n";
        echo "SQL COMMAND: {$sql}";
	}
	// Close the connection
	$mysqli->close();
}
?>
<?php
// Get params, sport is needed it both for post & get
$ChampId = isset($_GET['Id']) ? $_GET['Id'] : NULL;
$SportId = isset($_GET['SportId']) ? $_GET['SportId'] : NULL;
if (empty($SportId) && isset($_GET['Sport'])) {
	if (strtolower($_GET['Sport']) == 'basket') {
		$SportId = 2;
	} else {
		$SportId = 1;
	}
}
$CountryId = isset($_GET['CountryId']) ? $_GET['CountryId'] : NULL;
$ChampName = isset($_GET['Name']) ? urldecode($_GET['Name']) : NULL;
// POST Data
if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    if (!empty($_POST['Id']) && !empty($_POST['Delete']) && $_POST['Delete'] == 1){
		// Delete
		DeleteRecord($_POST['Id']);
	} else if (!empty($_POST['Id']) && !empty($_POST['Name'])) {
		// Update
		UpdateRecord($_POST);
	} else if (!empty($_POST['Name'])) {
		// Insert
		InsertRecord($_POST);
	} else {
		// Nothing to do, print the post array for debug
		print_r($_POST);
	}
	die();
}
// Get records
$records = GetSportsRecords($ChampId, $ChampName, $CountryId, $SportId);
// Update ChampId if empty
if (isset($records) && count($records) > 0) {
	if (empty($SportId)) $SportId = $records[0]['SportId'];
	if (empty($ChampId)) $ChampId = $records[0]['ChampId'];
}
?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
	<link rel="stylesheet" type="text/css" href="table_champs.css">
	<script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/jquery-ui.min.js"></script>
    <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
	<script>
		function postRecord(id) {
			$.post("", {
				'Id': id,
				'SportId': $('#SportId_' + id).val(),
				'ChampId': $('#ChampId_' + id).val(),
				'CountryId': $('#CountryId' + id).val(),
				'Name': $('#Name_' + id).val(),
				'LanguageId': $('#LanguageId_' + id).val(),
				'Flag': $('#Flag_' + id).val()
				},
			function(result){
			    if (result) alert(result);
				location.reload(true);
			});
		}
		function addRecord() {
			postRecord(0);
		}
		function saveRecord(id) {
			postRecord(id);
		}
		function deleteRecord(id) {
			$.post("", {
				'Id': id,
				'Delete': 1
				},
			function(result){
				if (result) alert(result);
				location.reload(true);
			});
		}
	</script>
	<style>
		body, input {
			font-size: 11px;
		}
		th {
			background-color: gray;
		}
		button {
			margin: 0px;
			padding: 0px;
		}
		button {
			margin: 0px;
			padding: 0px;
		}
		button.save {
			color: green;
			font-weight: bold;
		}
		button.delete {
			color: red;
			font-weight: bold;
		}
		input.iSportId { width: 25px; }
		input.iChampId { width: 40px; }
		input.iCountryId { width: 40px; }
		input.iName { width: 150px; }
		input.iLanguageId { width: 30px; }
		input.iFlag { width: 40px; }
	</style>
</head>
<body>
    <form action="" method="get" autocomplete="off">
        <table>
			<tr><th colspan="2">Search Info</th></tr>
			<tr><td class="colNames">SportId:</td><td><input id="TextSport" type="text" name="Sport" value="<?php echo $SportId; ?>" /></tr>
			<tr><td class="colNames">CountryId:</td><td><input id="TextSport" type="text" name="Champ" value="<?php echo $CountryId; ?>" /></tr>
			<tr>
				<td class="colNames">Name:</td>
				<td><input id="TextName" type="text" name="Name" value="<?php echo $ChampName; ?>" /></td>
				<td><a href="?Id=<?php echo $ChampId; ?>"><?php echo $ChampId; ?></a></td>
			</tr>
            <tr><td colspan="2"><input type="submit" value="Search" style="width:100%;"></td></tr>
        </table>
    </form>
	<?php if (isset($records)) { ?>
	<table>
		<tr>
			<th>Id</th>
			<th>SportId</th>
			<th>ChampId</th>
			<th>CountryId</th>
			<th>Name</th>
			<th>LangId</th>
			<th>Flag</th>
		</tr>
		<?php foreach ($records as $key => $record) { ?>
			<tr>
				<th><?php echo $record['Id']; ?></th>
				<td><input id="SportId_<?php echo $record['Id']; ?>" type="text" name="SportId" class="iSportId" value="<?php echo $record['SportId']; ?>" /></td>
				<td><input id="ChampId_<?php echo $record['Id']; ?>" type="text" name="ChampId" class="iChampId" value="<?php echo $record['ChampId']; ?>" /></td>
				<td><input id="CountryId_<?php echo $record['Id']; ?>" type="text" name="CountryId" class="iCountryId" value="<?php echo $record['CountryId']; ?>" /></td>
				<td><input id="Name_<?php echo $record['Id']; ?>" type="text" name="Name" class="iName" value="<?php echo $record['Name']; ?>" /></td>
				<td><input id="LanguageId_<?php echo $record['Id']; ?>" type="text" name="LanguageId" class="iLanguageId" value="<?php echo $record['LanguageId']; ?>" /></td>
				<td><input id="Flag_<?php echo $record['Id']; ?>" type="text" name="Flag" class="iFlag" value="<?php echo $record['Flag']; ?>" /></td>
				<td><button class="save" title="Save" onclick="saveRecord(<?php echo $record['Id']; ?>)" >V</button></td>
				<td><button class="delete" title="Delete" onclick="deleteRecord(<?php echo $record['Id']; ?>)" >X</button></td>
			</tr>
		<?php } ?>
		<tr>
			<th>=></th>
			<td><input id="SportId_0" type="text" name="SportId" class="iSportId" value="<?php echo $SportId; ?>" /></td>
			<td><input id="ChampId_0" type="text" name="ChampId" class="iChampId" value="<?php echo $ChampId; ?>" /></td>
			<td><input id="CountryId_0" type="text" name="CountryId" class="iCountryId" value="<?php echo $CountryId; ?>" /></td>
			<td><input id="Name_0" type="text" name="Name" class="iName" value="" /></td>
			<td><input id="LanguageId_0" type="text" name="LanguageId" class="iLanguageId" value="" /></td>
			<td><input id="Flag_0" type="text" name="Flag" class="iFlag" value="" /></td>
			<td><button class="save" title="Save" onclick="addRecord()" >V</button></td>
		</tr>
	</table>
	<?php } ?>
</body>
</html>
<?php // end gzip compressing
  while (@ob_end_flush());
?>