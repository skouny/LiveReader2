<?php
// force https
if ($_SERVER["HTTPS"] != "on") {
    header("Location: https://" . $_SERVER["HTTP_HOST"] . $_SERVER["REQUEST_URI"]);
    exit();
}
require_once('database.php');
session_start(); // start or resume session

function login($username, $password) {
    if ($username && $password) {
        $sql = "SELECT * FROM `users` WHERE `Username` = '{$username}' AND `Level` >= 9;";
        $record = mysqlQuerySelectFirst($sql);
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
    <html xmlns="http://www.w3.org/1999/xhtml">
        <head>
            <title>Authentication required</title>
            <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.0/jquery.min.js"></script>
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