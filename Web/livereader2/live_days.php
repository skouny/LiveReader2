<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php // Init
require_once('authentication.php');
date_default_timezone_set('Europe/Athens');
header('Content-Type: application/json; charset=utf-8');
?>
<?php
if (isset($_GET['sport'])) {
	$sport = strtolower($_GET['sport']);
	$data = GetMatchesDays("{$sport}_live_mix");
	echo json_encode($data);
}
?>
<?php // end gzip compressing
  while (@ob_end_flush());
?>