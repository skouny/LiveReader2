<?php // start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php // functions
function GetNowDayId() {
    $diff = abs(time() - strtotime("2015-08-20"));
    return floor($diff/(60*60*24));
}
?>
<?php
date_default_timezone_set("Europe/Athens"); // <= SOS!!! It is for correct datetime translation!!
if (isset($_GET['sport']) && $_GET['sport'] == 'basket') {
    $sport = 'basket';
} else {
    $sport = 'soccer';
}
if (empty($_GET['dayid'])) {
    $dayId = GetNowDayId();
} else {
    $dayId = $_GET['dayid'];
}
$dayBackId = $dayId - 1;
$dayNextId = $dayId + 1;
if ($sport == 'basket') {
    $url = "https://pamestoixima.opap.gr/forward/web/services/rs/iFlexBetting/retail/games/15104/0.json?shortTourn=true&startDrawNumber={$dayId}&endDrawNumber={$dayId}&sportId=s-442&locale=gr&brandId=defaultBrand&channelId=0";
} else {
    $url = "https://pamestoixima.opap.gr/forward/web/services/rs/iFlexBetting/retail/games/15104/0.json?shortTourn=true&startDrawNumber={$dayId}&endDrawNumber={$dayId}&sportId=s-441&marketIds=0&marketIds=0A&marketIds=1&marketIds=69&marketIds=68&marketIds=20&marketIds=21&marketIds=8&locale=gr&brandId=defaultBrand&channelId=0";
}
$data = file_get_contents($url);
$matches = json_decode($data);
usort($matches, function ($a, $b) { 
    return ($a->kdt < $b->kdt) ? -1 : 1;
});
//header('Content-Type: text/plain; charset=utf-8');print_r($matches[0]);die();
?>
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8" />
        <title>OPAP Coupon New</title>
        <script src="//ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
        <link href="//cdn.datatables.net/1.10.16/css/jquery.dataTables.min.css" rel="stylesheet" />
        <script src="//cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js"></script>
        <style>
            body {
                font-size: 11px;
                font-family: Verdana;
                margin: 0px;
            }
            table {
                border-collapse: collapse;
                border-spacing: 0px;
                margin: 4px;
                display: inline-block;
                vertical-align: top;
            }
            table, td, th {
                border: 1px solid black;
            }
            tr {
                background-color: lightgray;
            }
            tr:hover {
                background-color: khaki;
            }
            th {
                background-color: gray;
            }
            td {
                text-align: center;
                padding-left: 4px;
                padding-right: 4px;
            }
        </style>
        <script>
            $(document).ready(function(){
                $('#tableMain').DataTable();
            });
        </script>
    </head>
    <body>
        <a href="<?php echo "?sport=soccer&dayid={$dayId}"; ?>">Soccer</a>
        <a href="<?php echo "?sport=basket&dayid={$dayId}"; ?>">Basket</a>
        <span style="">|</span>
        <a href="<?php echo "?sport={$sport}&dayid={$dayBackId}"; ?>">< Back</a>
        <a href="<?php echo "?sport={$sport}"; ?>">Today</a>
        <a href="<?php echo "?sport={$sport}&dayid={$dayNextId}"; ?>">Next ></a>
        </br>
        <table id="tableMain">
            <thead>
                <tr>
                    <th>Champ</th>
                    <th>StartTime</th>
                    <th>Code</th>
                    <?php if ($sport == 'basket') { ?>
                        <th>EE</th>
                        <th>1</th>
                        <th>H</th>
                        <th>HomeTeam</th>
                        <th>AwayTeam</th>
                        <th>H</th>
                        <th>2</th>
                        <th>U</th>
                        <th>Spread</th>
                        <th>O</th>
                        <th>ScoreHT</th>
                        <th>ScoreFT</th>
                    <?php } else { ?>
                        <th>EE</th>
                        <th>1</th>
                        <th>H</th>
                        <th>HomeTeam</th>
                        <th>X</th>
                        <th>AwayTeam</th>
                        <th>H</th>
                        <th>2</th>
                        <th>U</th>
                        <th>O</th>
                        <th>GG</th>
                        <th>NG</th>
                        <th>ScoreHT</th>
                        <th>ScoreFT</th>
                    <?php } ?>
                    <th>TV</th>
                </tr>
            </thead>
            <tbody>
            <?php
                foreach ($matches as $match) {
                    foreach ($match->markets as $market) {
                        switch ($market->i) {
                            case '0': $market1X2 = $market; break; // 1-X-2 football
                            case '10': $marketML = $market; break; // 1-2 basket
                            case '20': $marketUO25 = $market; break; // Under/Over 2.5
                            case '69': $marketGGNG = $market; break; // GG/NG
                        }
                    }
                    if ($sport == 'basket') {
                        
                    } else {
                        if ($match->hpp->{1} >= 0 && $match->vsp->{1} >= 0) {
                            $goalsHomeHT = $match->hpp->{1};
                            $goalsAwayHT = $match->vsp->{1};
                            if ($match->hpp->{2} >= 0 && $match->vsp->{2} >= 0) {
                                $goalsHomeFT = $goalsHomeHT + $match->hpp->{2};
                                $goalsAwayFT = $goalsAwayHT + $match->vsp->{2};
                            } else {
                                $goalsHomeFT = '';
                                $goalsAwayFT = '';
                            }
                        } else {
                            $goalsHomeHT = '';
                            $goalsAwayHT = '';
                            $goalsHomeFT = '';
                            $goalsAwayFT = '';
                        }
                    }
            ?>
            <tr>
                <td><?php echo $match->lexicon->resources->{$match->ti.'_sh'}; ?></td>
                <td><?php echo date('Y-m-d H:i', $match->kdt / 1000); ?></td>
                <td><?php echo $match->code; ?></td>
                <?php if ($sport == 'basket') { ?>
                    <td><?php echo $marketML->mins; ?></td>
                    <td><?php echo $marketML->codes[0]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo $marketML->v1; ?></td>
                    <td><?php echo $match->lexicon->resources->{$match->hi}; ?></td>
                    <td><?php echo $match->lexicon->resources->{$match->ai}; ?></td>
                    <td><?php echo $marketML->v1; ?></td>
                    <td><?php echo $marketML->codes[1]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo $marketUO25->codes[0]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo $marketUO25->v1; ?></td>
                    <td><?php echo $marketUO25->codes[1]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo ""; ?></td>
                    <td><?php echo ""; ?></td>
                <?php } else { ?>
                    <td><?php echo $market1X2->mins; ?></td>
                    <td><?php echo $market1X2->codes[0]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo $market1X2->v1; ?></td>
                    <td><?php echo $match->lexicon->resources->{$match->hi}; ?></td>
                    <td><?php echo $market1X2->codes[1]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo $match->lexicon->resources->{$match->ai}; ?></td>
                    <td><?php echo $market1X2->v1; ?></td>
                    <td><?php echo $market1X2->codes[2]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo $marketUO25->codes[0]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo $marketUO25->codes[1]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo $marketGGNG->codes[0]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo $marketGGNG->codes[1]->oddPerSet->{'1'}; ?></td>
                    <td><?php echo "{$goalsHomeHT}-{$goalsAwayHT}"; ?></td>
                    <td><?php echo "{$goalsHomeFT}-{$goalsAwayFT}"; ?></td>
                <?php } ?>
                <td><?php if (isset($match->tvId)) echo $match->tvId; ?></td>
            </tr>
            <?php } ?>
            </tbody>
        </table>
    </body>
</html>
<?php // end gzip compressing
while (@ob_end_flush());
?>