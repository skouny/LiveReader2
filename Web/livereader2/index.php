<?php
// start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
// Init
require_once('authentication.php');
error_reporting(0);
date_default_timezone_set('Europe/Athens');
header('Content-Type: text/html; charset=utf-8');
?>
<!DOCTYPE html>
<html>
    <head>
        <title>LiveReader2 - Admin</title>
        <!-- jquery / jquery-ui / jquery-ui style -->
        <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
        <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/jquery-ui.min.js"></script>
        <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
        <!-- self -->
        <script src="index.js?v=47"></script>
        <link rel="stylesheet" type="text/css" href="index.css?v=5">
    </head>
    <body>
        <div id="DivOptions">
            <!-- Sport -->
            <select id="SelectSport" onchange="initSelectedSport()">
                <option>Football</option>
                <option>Basket</option>
            </select>
            <!-- Day -->
            <select id="SelectDay" onchange="tableRefresh(true)">

            </select>
            <!-- Source -->
            <select id="SelectSource" onchange="tableRefresh(true)">
                <option>All-Opap</option>
                <option>All-FlashScore</option>
                <option>All-NowGoal</option>
                <option>All-Futbol24</option>
                <option>Matched-Opap</option>
                <option>Matched-FlashScore</option>
                <option>Matched-NowGoal</option>
                <option>Matched-Futbol24</option>
                <option>Unmatched-Opap</option>
                <option>Unmatched-FlashScore</option>
                <option>Unmatched-NowGoal</option>
                <option>Unmatched-Futbol24</option>
            </select>
            <!-- Champ -->
            <label>Champ:</label>
            <input type="text" id="TextChamp" style="width:120px;border:1px solid red;" onkeydown="if (event.keyCode == 13)
                            tableRefresh(true)" />
            <!-- Refresh -->
            <button onclick="tableRefresh(true)">Refresh</button>
            <input id="CheckboxAutoRefresh" type="checkbox" />
            <label for="CheckboxAutoRefresh">Auto</label>
            <label>[<span id="SpanMessage"></span>]</label>
            <!-- Old Records -->
            <button onclick="deleteOldRecords()">Delete Old Records</button>
            <!-- Menu -->
            <ul id="menu">
                <li>
                    <a href="#"><?php echo "Hello {$_SESSION['valid_user']}!"; ?></a>
                    <ul>
                        <li><a href="../" target="_blank">Home</a></li>
                        <li><a href="../phpmyadmin/" target="_blank">phpMyAdmin</a></li>
                        <li><a href="../backup/mysqldump.php?database=livereader2" target="_blank">Database Backup</a></li>
                        <li><a href="../xml/livescore.php?sport=football&day=today" target="_blank">XML Feed Football [Today]</a></li>
                        <li><a href="../xml/livescore.php?sport=basket&day=today" target="_blank">XML Feed Basket [Today]</a></li>
                        <li><a href="../xml/livescore.php?sport=football&day=yesterday" target="_blank">XML Feed Football [Yesterday]</a></li>
                        <li><a href="../xml/livescore.php?sport=basket&day=yesterday" target="_blank">XML Feed Basket [Yesterday]</a></li>
                        <li><a href="../xml/standings.php" target="_blank">XML Feed Standings</a></li>
                        <li><a href="../xml/weather.php?teams=ΠΛΑΤΑΝΙΑΣ,ΧΑΝΙΑ,ΚΙΣΣΑΜΙΚΟΣ" target="_blank">XML Feed Weather</a></li>
                        <li>
                            <a href="#">Sources Football</a>
                            <ul>
                                <li><a href="./table_live_mix_get.php?sport=Football&source=Opap" target="_blank">Opap</a></li>
                                <li><a href="./table_live_mix_get.php?sport=Football&source=FlashScore" target="_blank">FlashScore</a></li>
                                <li><a href="./table_live_mix_get.php?sport=Football&source=Futbol24" target="_blank">Futbol24</a></li>
                                <li><a href="./table_live_mix_get.php?sport=Football&source=NowGoal" target="_blank">NowGoal</a></li>
                            </ul>
                        </li>
                        <li>
                            <a href="#">Sources Basket</a>
                            <ul>
                                <li><a href="./table_live_mix_get.php?sport=Basket&source=Opap" target="_blank">Opap</a></li>
                                <li><a href="./table_live_mix_get.php?sport=Basket&source=NowGoal" target="_blank">NowGoal</a></li>
                            </ul>
                        </li>
                        <li><a href="http://livereader2.skouny.net/" target="_blank">Install Client</a></li>
                        <li>
                            <a href="#">
                                <form action="" method="post">
                                    <input type="submit" name="logout" value="Logout" style="width:100%;height:100%;font-weight:bold;border-width:thin;">
                                </form>
                            </a>
                        </li>
                    </ul>
                </li>
            </ul>
        </div>
        <div id="DivCoupon">

        </div>
        <div id="DivTeams">
            <iframe src="table_teams.php" name="teams"></iframe>
        </div>
    </body>
</html>
<?php
// end gzip compressing
while (@ob_end_flush());
?>