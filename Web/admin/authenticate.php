<?php
// force https
if ($_SERVER["HTTPS"] != "on") {
    header("Location: https://" . $_SERVER["HTTP_HOST"] . $_SERVER["REQUEST_URI"]);
    exit();
}
require_once('database.php');
// start or resume session
session_start();
function login($username, $password) {
    if (!empty($username) && !empty($password)) {
        $sql = "SELECT * FROM `users` WHERE `Username` = '{$username}' AND `Level` >= 9;";
        // Find user record
        $record = db::getRecord($sql);
        if ($record['Password'] == $password) {
            $_SESSION['valid_user'] = $username;
            return true;
        }
    }
    return false;
}
function logout() {
    unset($_SESSION['valid_user']);
    session_unset();
    session_destroy();
}
// If there is a login or logout post
if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    if (isset($_POST['username']) && isset($_POST['password'])) {
        login($_POST['username'], $_POST['password']);
    } else if (isset($_POST['logout'])) {
        logout();
    }
}
// If user not logged in, display the login form
if (!isset($_SESSION['valid_user'])) {
    ?>
    <!DOCTYPE html>
    <html>
        <head>
            <title>Authentication required</title>
            <meta charset="UTF-8">
            <script src="//code.jquery.com/jquery-3.2.1.min.js"></script>
            <script src="//code.jquery.com/ui/1.12.1/jquery-ui.min.js"></script>
            <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/cupertino/jquery-ui.css">
            <style>
                .ui-widget {
                    font-size:95%;
                }
            </style>
            <script>
                $(document).ready(function () {
                    var top = ($(window).height() - $('#TableLogin').height()) / 2;
                    var left = ($(window).width() - $('#TableLogin').width()) / 2;
                    $('#TableLogin').css("top", top + "px");
                    $('#TableLogin').css("left", left + "px");
                });
            </script>
            <style>
                .td1 {
                    text-align: right;
                }
                #ButtonLogin {
                    width: 100%;
                    font-weight: bold;
                    font-size: 105%;
                }
                #TableLogin {
                    position: absolute;
                    border: 2px solid gray;
                }
                table, tr, th, td {
                    padding: 4px;
                }
                th {
                    background-color: gray;
                }
            </style>
        </head>
        <body>
            <form action="" method="post">
                <table id="TableLogin">
                    <tr><th colspan="2">Authentication required</th></tr>
                    <tr><td class="td1">Username:</td><td><input id="TextUsername" type="text" name="username" value="" /></td></tr>
                    <tr><td class="td1">Password:</td><td><input id="TextPassword" type="password" name="password" value="" /></td></tr>
                    <tr><td colspan="2"><input id="ButtonLogin" type="submit" value="Login"></td></tr>
                </table>
            </form>
        </body>
    </html>
    <?php
    exit();
}
?>