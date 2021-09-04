<?php
error_reporting(0);
date_default_timezone_set('Europe/Athens');
header('Content-Type: text/html; charset=utf-8');
require_once 'authenticate.php';
?>
<!DOCTYPE html>
<html>
    <head>
        <title>LiveReader - Admin (New)</title>
        <meta charset="UTF-8">
        <script src="//code.jquery.com/jquery-3.2.1.min.js"></script>
        <script src="//code.jquery.com/ui/1.12.1/jquery-ui.min.js"></script>
        <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/cupertino/jquery-ui.css">
        <style>
            .ui-widget {
                font-size:95%;
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
            .aa {
                font-size: 24px;
                font-weight: bold;
                text-align: center;
            }
            .green {
                color: green;
            }
            .orange {
                color: orange;
            }
            .red, .red:link, .red:visited {
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
            .mixed-row {
                font-weight: bold;
            }
            .refreshRecord {
                color: green;
                font-weight: bold;
            }
            .deleteRecord {
                color: red;
                font-weight: bold;
            }
        </style>
        <script>
            $(function () {
                //$("select").selectmenu();
                //$("button").button();
                optionsChanged();
            });
            function optionsChanged() {
                var sport = $('#selectSport').val() || '';
                var day = $('#selectDay').val() || '';
                var source = $('#selectSource').val() || '';
                var champ = $('#selectChamp').val() || '';
                var url = encodeURI('options.php?sport=' + sport + '&day=' + day + '&source=' + source + '&champ=' + champ);
                optionsUpdate(url);
            }
            function matchesRefresh() {
                var sport = $('#selectSport').val() || '';
                var day = $('#selectDay').val() || '';
                var source = $('#selectSource').val() || '';
                var champ = $('#selectChamp').val() || '';
                var url = encodeURI('matches.php?sport=' + sport + '&day=' + day + '&source=' + source + '&champ=' + champ);
                matchesUpdate(url);
            }
            function matchesUpdate(url) {
                //alert(url);
                $.ajax({
                    type: 'GET',
                    url: url,
                    dataType: 'text',
                    cache: false
                }).done(function (text) {
                    $('#divMatches').html(text);
                    $('#labelMessage').text((new Date()).toLocaleTimeString());
                }).fail(function () {
                    $('#labelMessage').text('Communication error!!');
                });
            }
            function optionsUpdate(url) {
                //alert(url);
                //$("*").css("cursor", "progress");
                $.ajax({
                    type: 'GET',
                    url: url,
                    dataType: 'json',
                    contentType: 'application/json; charset=UTF-8',
                    cache: false
                }).done(function (data) {
                    //alert(JSON.stringify(data));
                    selectUpdate('#selectSport', data['sport'], $('#selectSport').val());
                    selectUpdate('#selectDay', data['day'], $('#selectDay').val());
                    selectUpdate('#selectSource', data['source'], $('#selectSource').val());
                    selectUpdate('#selectChamp', data['champ'], $('#selectChamp').val());
                    matchesRefresh();
                    //$("*").css("cursor", "default");
                }).fail(function () {
                    $('#labelMessage').text('Communication error!!');
                });
            }
            function selectUpdate(selector, values, selected) {
                try {
                    $(selector).empty();
                    for (key in values) {
                        var value = values[key];
                        if (selected && selected === value) {
                            $(selector).append('<option selected="selected">' + value + '</option>');
                        } else {
                            $(selector).append('<option>' + value + '</option>');
                        }
                    };
                } catch (err) {
                    alert(err);
                }
            }
            function openChampById(id){
                var url = encodeURI("names.php?type=champ&id=" + id);
                iframeDialog(url, 480, 360, "ChampId: " + id);
            }
            function openChampByName(name){
                var url = encodeURI("names.php?type=champ&name=" + name);
                iframeDialog(url, 480, 360, "Champ: " + decodeURIComponent(name));
            }
            function openTeamById(id){
                var url = encodeURI("names.php?type=team&id=" + id);
                iframeDialog(url, 480, 360, "TeamId: " + id);
            }
            function openTeamByName(name){
                var url = encodeURI("names.php?type=team&name=" + name);
                iframeDialog(url, 480, 360, "Team: " + decodeURIComponent(name));
            }
            function iframeDialog(url, width, height, title) {
                $("<div style='padding:0px;overflow:hidden;'><iframe src='" + url + "' style='width:100%;height:100%;border:none;overflow:hidden;'></iframe></div>").dialog({
                    autoOpen: true,
                    width: width,
                    height: height,
                    title: title || "Dialog",
                    open: function (event, ui) {
                        
                    },
                    close: function (event, ui) {
                        $(this).dialog("destroy");
                    },
                    modal: false
                });
            }
            function showDialog(base64, width, height, title) {
                let selector = atob(base64);
                $(selector).dialog({
                    autoOpen: true,
                    width: width,
                    height: height,
                    title: title || "Dialog",
                    open: function (event, ui) {
                        
                    },
                    close: function (event, ui) {
                        $(this).dialog("destroy");
                    },
                    modal: false
                });
            }
            function refreshRecord(sport, source, webId) {
                var data = {
                    'RefreshRecord': {
                        'Sport': sport,
                        'Source': source,
                        'WebId': webId
                    }
                };
                //alert(JSON.stringify(data));
                $.post("matches.php", data,
                function(result){
                    if (result) alert(result);
                    optionsChanged();
                });
            }
            function deleteRecord(sport, source, webId) {
                $.post("matches.php", {
                    'DeleteRecord': {
                        'Sport': sport,
                        'Source': source,
                        'WebId': webId
                    }
                },
                function(result){
                    if (result) alert(result);
                    optionsChanged();
                });
            }
        </script>
    </head>
    <body style="position:fixed;top:0px;left:0px;right:0px;bottom:0px;">
        <div id="divToolbar" style="position:fixed;top:0px;left:0px;right:0px;height:24px;">
            <select id="selectSport" onchange="optionsChanged()"></select>
            <select id="selectDay" onchange="optionsChanged()"></select>
            <select id="selectSource" onchange="optionsChanged()"></select>
            <select id="selectChamp" onchange="optionsChanged()"></select>
            <button id="buttonRefresh" onclick="optionsChanged()">Refresh</button>
            <label id="labelMessage"></label>
        </div>
        <div id="divMatches" style="position:fixed;top:24px;left:0px;right:0px;bottom:0px;overflow:auto;">
            
        </div>
    </body>
</html>
