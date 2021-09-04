<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
// Connect to database
$mysqli = new mysqli('localhost', 'myroot', 'zJp4$81s', 'sports');
// If connection failed die...
if ($mysqli->connect_errno) {
	die('Failed to connect to MySQL: (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
}
// Change character set to utf8
$mysqli->set_charset('utf8');
// One month back date
$interval = DateInterval::createFromDateString('-1 month');
//$interval = DateInterval::createFromDateString('-2 years');
$backDay = (new DateTime())->add($interval);
$backDate = $backDay->format('Y-m-d') . ' 08:00:00';
// Create command
$sql = "SELECT * FROM `football_matches`";
$sql .= " WHERE `Source` = 'Opap'";
$sql .= " AND `StartTime` >= '{$backDate}'";
$sql .= " AND `HomeScoreFT` IS NOT NULL";
$sql .= " AND `AwayScoreFT` IS NOT NULL";
$sql .= " ORDER BY `StartTime` ASC";
// Execute the command
$query = $mysqli->query($sql);
// If execution failed
if ($mysqli->errno) {
	echo "Failed to execute query!\n\n";
	echo "ERROR ({$mysqli->errno}): {$mysqli->error}\n\n";
	echo "SQL COMMAND: {$sql}";
}
if (isset($_GET["csv"])) {
	header('Content-Type: text/plain; charset=utf-8');
	echo "Id\tStartTime\tCode\tChamp\tHomeTeam\tAwayTeam\tHomeScoreHT\tAwayScoreHT\tHomeScoreFT\tAwayScoreFT\r\n";
	while($row = $query->fetch_assoc()) {
		echo "{$row['Id']}\t{$row['StartTime']}\t{$row['WebId']}\t{$row['Champ']}\t{$row['HomeTeam']}\t{$row['AwayTeam']}\t{$row['HomeScoreHT']}\t{$row['AwayScoreHT']}\t{$row['HomeScoreFT']}\t{$row['AwayScoreFT']}\r\n";
	}
} elseif (isset($_GET["xml"])) {
	header('Content-Type: text/xml; charset=utf-8');
	$writer = new XMLWriter();
	$writer->openMemory();
	$writer->startDocument('1.0','UTF-8');
	$writer->setIndent(4);
	$writer->startElement('Results');
	while($row = $query->fetch_assoc()) {
		$writer->startElement('Match');
		$writer->writeAttribute('Id', $row['Id']);
		$writer->writeAttribute('StartTime', $row['StartTime']);
		$writer->writeAttribute('Code', $row['WebId']);
		$writer->writeAttribute('Champ', $row['Champ']);
		$writer->writeAttribute('HomeTeam', $row['HomeTeam']);
		$writer->writeAttribute('AwayTeam', $row['AwayTeam']);
		$writer->writeAttribute('HomeScoreHT', $row['HomeScoreHT']);
		$writer->writeAttribute('AwayScoreHT', $row['AwayScoreHT']);
		$writer->writeAttribute('HomeScoreFT', $row['HomeScoreFT']);
		$writer->writeAttribute('AwayScoreFT', $row['AwayScoreFT']);
		$writer->endElement();
	}
	$writer->endElement();
	$writer->endDocument();
	$xml = $writer->outputMemory(TRUE);
	$writer->flush();
	echo $xml;
} else {
?>
<html>
<table>
<tr>
    <th>Id</th>
	<th>StartTime</th>
	<th>Code</th>
	<th>Champ</th>
	<th>HomeTeam</th>
	<th>AwayTeam</th>
	<th>HomeScoreHT</th>
	<th>AwayScoreHT</th>
	<th>HomeScoreFT</th>
	<th>AwayScoreFT</th>
</tr>
<?php while($row = $query->fetch_assoc()) { ?>
<tr>
	<td><?php echo $row['Id'] ?></td>
	<td><?php echo $row['StartTime'] ?></td>
	<td><?php echo $row['WebId'] ?></td>
	<td><?php echo $row['Champ'] ?></td>
	<td><?php echo $row['HomeTeam'] ?></td>
	<td><?php echo $row['AwayTeam'] ?></td>
	<td><?php echo $row['HomeScoreHT'] ?></td>
	<td><?php echo $row['AwayScoreHT'] ?></td>
	<td><?php echo $row['HomeScoreFT'] ?></td>
	<td><?php echo $row['AwayScoreFT'] ?></td>
</tr>
<?php } ?>
</table>
</html>
<?php
}
// free query set
$query->free();
// Close the connection
$mysqli->close();
?>
<?php // end gzip compressing
  while (@ob_end_flush());
?>