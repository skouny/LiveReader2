<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php phpinfo(); ?>
<?php // end gzip compressing
  while (@ob_end_flush());
?>