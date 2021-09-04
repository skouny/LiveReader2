<?php
require_once('livereader2/database.php');
?>
<!DOCTYPE HTML>
<html>
    <head>
        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
        <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/jquery-ui.min.js"></script>
        <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
        <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/canvasjs/1.7.0/canvasjs.min.js"></script>
        <script src="./xml/livechart.js"></script>
    </head>
    <body>
        <pre>
        <?php
        function testtt($s){
            $s['pp']='rrrrr';
            return $s;
        }
        $t=array();
        $s=array();
        $t[16]=$s;
        $s['d']='sfddfg';
        $s['r']='ddddd';
        $t[16]=testtt($s);
        
        
        print_r($t);
        ?>
        </pre>
    </body>
</html>
