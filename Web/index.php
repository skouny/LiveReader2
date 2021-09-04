<?php
// start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
// Init
error_reporting(0);
date_default_timezone_set('Europe/Athens');
// Headers
header('Content-Type: text/html; charset=utf-8');
?>
<html>
    <head>
        <title>LiveReader2</title>
        <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
        <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/jquery-ui.min.js"></script>
        <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
        <link rel="stylesheet" type="text/css" href="./index.css">
        <script src="./index.js"></script>
    </head>
    <body>
        <div id="DivOptions">
            <select id="SelectSport" onchange="tableRefresh()">
                <option>Football</option>
                <option>Basket</option>
            </select>
        </div>
        <div id="DivCoupon">

        </div>
        <div id="DivLabels">
            <label>Last Update: </label><label id="LabelMessage"></label>
        </div>
    </body>
</html>
<?php
// end gzip compressing
while (@ob_end_flush());
?>