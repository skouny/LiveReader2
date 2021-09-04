<?php // Start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
// Dependencies
require_once('database.php');
?>
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8" />
        <title></title>
        <style>
            body {
                font-family: Verdana;
                font-size: 12px;
                margin: 0px;
                padding: 0px;
                border: 0px;
                width: 100%;
                height: 100%;
                overflow: hidden;
            }
            iframe {
                position: absolute;
                top: 0px;
                left: 0px;
                right: 0px;
                bottom: 0px;
            }
        </style>
    </head>
    <body>
        <div style="position: fixed;top: 0px; left: 0px;right: 0px;height: 90px; overflow-x: hidden;overflow-y: auto;">
            Matches: 
<?php
    $iframe_url = "";
    foreach (explode(',', $_GET['matches']) as $match) {
        $url = "football_queries_results.php?action=view-match&matchid={$match}";
        echo "<a href=\"{$url}\" target=\"match_form\">{$match}</a>, ";
        if (empty($iframe_url)) $iframe_url = $url;
    }
?>
            <br />
            Combinations: <?php echo $_GET['combinations']; ?>
            <br />
            History: 
<?php
    foreach (explode(',', $_GET['history']) as $match) {
        $url = "football_queries_results.php?action=view-match&matchid={$match}";
        echo "<a href=\"{$url}\" target=\"match_form\">{$match}</a>, ";
    }
?>
        </div>
        <div style="position: fixed;top: 90px; left: 0px;right: 0px;bottom: 0px;">
            <iframe name="match_form" width="100%" height="100%" src="<?php echo $iframe_url; ?>"></iframe>
        </div>
    </body>
</html>
<?php // End gzip compressing
while (@ob_end_flush());
?>