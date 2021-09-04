<?php // Start gzip compressing
ob_start('ob_gzhandler');
header('Content-Encoding: gzip');
?>
<?php
// Dependencies
require_once('database.php');
// Classes
class MatchStatistics {
    public $match = NULL;
    public $homePrevMatch = NULL;
    public $awayPrevMatch = NULL;
    public $home = NULL;
    public $away = NULL;
    public $history = NULL;
    public $stats = array();
    public $names = NULL;
    // Constructor
    public function __construct($matchId) {
        $this->match = mysqlQuerySelectFirst("SELECT * FROM `football_opap_matches` WHERE `Id` = {$matchId}");
        if ($this->match != NULL) {
            $this->home = $this->queryHomeTeam($this->match);
            $this->away = $this->queryAwayTeam($this->match);
            if (count($this->home) > 0) $this->homePrevMatch = $this->home[0];
            if (count($this->away) > 0) $this->awayPrevMatch = $this->away[0];
            $this->history = $this->queryMatchHistory($this->match);
            $this->names = $this->queryNames();
            // Home Statistincs
            $this->stats['HomeMatches'] = 0;
            $this->stats['HomeWins'] = 0;
            $this->stats['HomeDraws'] = 0;
            $this->stats['HomeLosts'] = 0;
            $this->stats['HomeGoalsScored'] = 0;
            $this->stats['HomeGoalsAgianst'] = 0;
            $this->stats['HomeUnderCount'] = 0;
            $this->stats['HomeOverCount'] = 0;
            $this->stats['HomeNoGoalCount'] = 0;
            $this->stats['HomeGoalGoalCount'] = 0;
            $this->stats['HomeGoals0to1'] = 0;
            $this->stats['HomeGoals2to3'] = 0;
            $this->stats['HomeGoals4plus'] = 0;
            foreach ($this->home as $match) {
                $goalsHost = intval($match['ScoreHomeFT']);
                $goalsGuest = intval($match['ScoreAwayFT']);
                $goalsTotal = $goalsHost + $goalsGuest;
                $this->stats['HomeMatches'] += 1;
                if ($goalsHost > $goalsGuest) { $this->stats['HomeWins'] += 1; }
                if ($goalsHost == $goalsGuest) { $this->stats['HomeDraws'] += 1; }
                if ($goalsHost < $goalsGuest) { $this->stats['HomeLosts'] += 1; }
                $this->stats['HomeGoalsScored'] += $goalsHost;
                $this->stats['HomeGoalsAgianst'] += $goalsGuest;
                if ($goalsTotal > 2.5) $this->stats['HomeOverCount'] += 1; else $this->stats['HomeUnderCount'] += 1;
                if ($goalsHost > 0 && $goalsGuest > 0) $this->stats['HomeGoalGoalCount'] += 1; else $this->stats['HomeNoGoalCount'] += 1;
                if ($goalsTotal >= 0 && $goalsTotal <= 1) $this->stats['HomeGoals0to1'] += 1;
                if ($goalsTotal >= 2 && $goalsTotal <= 3) $this->stats['HomeGoals2to3'] += 1;
                if ($goalsTotal >= 4) $this->stats['HomeGoals4plus'] += 1;
            }
            // Away Statistincs
            $this->stats['AwayMatches'] = 0;
            $this->stats['AwayWins'] = 0;
            $this->stats['AwayDraws'] = 0;
            $this->stats['AwayLosts'] = 0;
            $this->stats['AwayGoalsScored'] = 0;
            $this->stats['AwayGoalsAgianst'] = 0;
            $this->stats['AwayUnderCount'] = 0;
            $this->stats['AwayOverCount'] = 0;
            $this->stats['AwayNoGoalCount'] = 0;
            $this->stats['AwayGoalGoalCount'] = 0;
            $this->stats['AwayGoals0to1'] = 0;
            $this->stats['AwayGoals2to3'] = 0;
            $this->stats['AwayGoals4plus'] = 0;
            foreach ($this->away as $match) {
                $goalsHost = intval($match['ScoreHomeFT']);
                $goalsGuest = intval($match['ScoreAwayFT']);
                $goalsTotal = $goalsHost + $goalsGuest;
                $this->stats['AwayMatches'] += 1;
                if ($goalsHost < $goalsGuest) { $this->stats['AwayWins'] += 1; }
                if ($goalsHost == $goalsGuest) { $this->stats['AwayDraws'] += 1; }
                if ($goalsHost > $goalsGuest) { $this->stats['AwayLosts'] += 1; }
                $this->stats['AwayGoalsScored'] += $goalsGuest;
                $this->stats['AwayGoalsAgianst'] += $goalsHost;
                if ($goalsTotal > 2.5) $this->stats['AwayOverCount'] += 1; else $this->stats['AwayUnderCount'] += 1;
                if ($goalsHost > 0 && $goalsGuest > 0) $this->stats['AwayGoalGoalCount'] += 1; else $this->stats['AwayNoGoalCount'] += 1;
                if ($goalsTotal >= 0 && $goalsTotal <= 1) $this->stats['AwayGoals0to1'] += 1;
                if ($goalsTotal >= 2 && $goalsTotal <= 3) $this->stats['AwayGoals2to3'] += 1;
                if ($goalsTotal >= 4) $this->stats['AwayGoals4plus'] += 1;
            }
            // Home Seri
            $this->stats['HomeWinsSeri'] = 0;
            $this->stats['HomeDrawsSeri'] = 0;
            $this->stats['HomeLostsSeri'] = 0;
            $this->stats['HomeUnderSeri'] = 0;
            $this->stats['HomeOverSeri'] = 0;
            $this->stats['HomeNoGoalSeri'] = 0;
            $this->stats['HomeGoalGoalSeri'] = 0;
            $this->stats['HomeGoalsSeri0to1'] = 0;
            $this->stats['HomeGoalsSeri2to3'] = 0;
            $this->stats['HomeGoalsSeri4plus'] = 0;
            $this->stats['HomeOver2GoalScoresSeri'] = 0;
            $this->stats['HomeOver2GoalAgainstSeri'] = 0;
            $this->stats['HomeWinsHistorySeri'] = 0;
            $this->stats['HomeDrawsHistorySeri'] = 0;
            $this->stats['HomeLostsHistorySeri'] = 0;
            foreach ($this->home as $match) {
                if (intval($match['ScoreHomeFT']) > intval($match['ScoreAwayFT'])) $this->stats['HomeWinsSeri'] += 1; else break;
            }
            foreach ($this->home as $match) {
                if (intval($match['ScoreHomeFT']) == intval($match['ScoreAwayFT'])) $this->stats['HomeDrawsSeri'] += 1; else break;
            }
            foreach ($this->home as $match) {
                if (intval($match['ScoreHomeFT']) < intval($match['ScoreAwayFT'])) $this->stats['HomeLostsSeri'] += 1; else break;
            }
            foreach ($this->home as $match) {
                if (intval($match['ScoreHomeFT']) + intval($match['ScoreAwayFT']) <= 2) $this->stats['HomeUnderSeri'] += 1; else break;
            }
            foreach ($this->home as $match) {
                if (intval($match['ScoreHomeFT']) + intval($match['ScoreAwayFT']) >= 3) $this->stats['HomeOverSeri'] += 1; else break;
            }
            foreach ($this->home as $match) {
                if (intval($match['ScoreHomeFT']) == 0 || intval($match['ScoreAwayFT']) == 0) $this->stats['HomeNoGoalSeri'] += 1; else break;
            }
            foreach ($this->home as $match) {
                if (intval($match['ScoreHomeFT']) > 0 && intval($match['ScoreAwayFT']) > 0) $this->stats['HomeGoalGoalSeri'] += 1; else break;
            }
            foreach ($this->home as $match) {
                $goalsTotal = intval($match['ScoreHomeFT']) + intval($match['ScoreAwayFT']);
                if ($goalsTotal == 0 || $goalsTotal == 1) $this->stats['HomeGoalsSeri0to1'] += 1; else break;
            }
            foreach ($this->home as $match) {
                $goalsTotal = intval($match['ScoreHomeFT']) + intval($match['ScoreAwayFT']);
                if ($goalsTotal == 2 || $goalsTotal == 3) $this->stats['HomeGoalsSeri2to3'] += 1; else break;
            }
            foreach ($this->home as $match) {
                $goalsTotal = intval($match['ScoreHomeFT']) + intval($match['ScoreAwayFT']);
                if ($goalsTotal >= 4) $this->stats['HomeGoalsSeri4plus'] += 1; else break;
            }
            foreach ($this->home as $match) {
                if (intval($match['ScoreHomeFT']) >= 2) $this->stats['HomeOver2GoalScoresSeri'] += 1; else break;
            }
            foreach ($this->home as $match) {
                if (intval($match['ScoreAwayFT']) >= 2) $this->stats['HomeOver2GoalAgainstSeri'] += 1; else break;
            }
            foreach ($this->history as $match) {
                if (intval($match['ScoreHomeFT']) > intval($match['ScoreAwayFT'])) $this->stats['HomeWinsHistorySeri'] += 1; else break;
            }
            foreach ($this->history as $match) {
                if (intval($match['ScoreHomeFT']) == intval($match['ScoreAwayFT'])) $this->stats['HomeDrawsHistorySeri'] += 1; else break;
            }
            foreach ($this->history as $match) {
                if (intval($match['ScoreHomeFT']) < intval($match['ScoreAwayFT'])) $this->stats['HomeLostsHistorySeri'] += 1; else break;
            }
            // Away Seri
            $this->stats['AwayWinsSeri'] = 0;
            $this->stats['AwayDrawsSeri'] = 0;
            $this->stats['AwayLostsSeri'] = 0;
            $this->stats['AwayUnderSeri'] = 0;
            $this->stats['AwayOverSeri'] = 0;
            $this->stats['AwayNoGoalSeri'] = 0;
            $this->stats['AwayGoalGoalSeri'] = 0;
            $this->stats['AwayGoalsSeri0to1'] = 0;
            $this->stats['AwayGoalsSeri2to3'] = 0;
            $this->stats['AwayGoalsSeri4plus'] = 0;
            $this->stats['AwayOver2GoalScoresSeri'] = 0;
            $this->stats['AwayOver2GoalAgainstSeri'] = 0;
            $this->stats['AwayWinsHistorySeri'] = 0;
            $this->stats['AwayDrawsHistorySeri'] = 0;
            $this->stats['AwayLostsHistorySeri'] = 0;
            foreach ($this->away as $match) {
                if (intval($match['ScoreHomeFT']) > intval($match['ScoreAwayFT'])) $this->stats['AwayWinsSeri'] += 1; else break;
            }
            foreach ($this->away as $match) {
                if (intval($match['ScoreHomeFT']) == intval($match['ScoreAwayFT'])) $this->stats['AwayDrawsSeri'] += 1; else break;
            }
            foreach ($this->away as $match) {
                if (intval($match['ScoreHomeFT']) < intval($match['ScoreAwayFT'])) $this->stats['AwayLostsSeri'] += 1; else break;
            }
            foreach ($this->away as $match) {
                if (intval($match['ScoreHomeFT']) + intval($match['ScoreAwayFT']) <= 2) $this->stats['AwayUnderSeri'] += 1; else break;
            }
            foreach ($this->away as $match) {
                if (intval($match['ScoreHomeFT']) + intval($match['ScoreAwayFT']) >= 3) $this->stats['AwayOverSeri'] += 1; else break;
            }
            foreach ($this->away as $match) {
                if (intval($match['ScoreHomeFT']) == 0 || intval($match['ScoreAwayFT']) == 0) $this->stats['AwayNoGoalSeri'] += 1; else break;
            }
            foreach ($this->away as $match) {
                if (intval($match['ScoreHomeFT']) > 0 && intval($match['ScoreAwayFT']) > 0) $this->stats['AwayGoalGoalSeri'] += 1; else break;
            }
            foreach ($this->away as $match) {
                $goalsTotal = intval($match['ScoreHomeFT']) + intval($match['ScoreAwayFT']);
                if ($goalsTotal == 0 || $goalsTotal == 1) $this->stats['AwayGoalsSeri0to1'] += 1; else break;
            }
            foreach ($this->away as $match) {
                $goalsTotal = intval($match['ScoreHomeFT']) + intval($match['ScoreAwayFT']);
                if ($goalsTotal == 2 || $goalsTotal == 3) $this->stats['AwayGoalsSeri2to3'] += 1; else break;
            }
            foreach ($this->away as $match) {
                $goalsTotal = intval($match['ScoreHomeFT']) + intval($match['ScoreAwayFT']);
                if ($goalsTotal >= 4) $this->stats['AwayGoalsSeri4plus'] += 1; else break;
            }
            foreach ($this->away as $match) {
                if (intval($match['ScoreHomeFT']) >= 2) $this->stats['AwayOver2GoalScoresSeri'] += 1; else break;
            }
            foreach ($this->away as $match) {
                if (intval($match['ScoreAwayFT']) >= 2) $this->stats['AwayOver2GoalAgainstSeri'] += 1; else break;
            }
            foreach ($this->history as $match) {
                if (intval($match['ScoreHomeFT']) > intval($match['ScoreAwayFT'])) $this->stats['AwayWinsHistorySeri'] += 1; else break;
            }
            foreach ($this->history as $match) {
                if (intval($match['ScoreHomeFT']) == intval($match['ScoreAwayFT'])) $this->stats['AwayDrawsHistorySeri'] += 1; else break;
            }
            foreach ($this->history as $match) {
                if (intval($match['ScoreHomeFT']) < intval($match['ScoreAwayFT'])) $this->stats['AwayLostsHistorySeri'] += 1; else break;
            }
            // History Statistincs
            $this->stats['MatchesHistory'] = 0;
            $this->stats['HomeWinsHistory'] = 0;
            $this->stats['HomeDrawsHistory'] = 0;
            $this->stats['HomeLostsHistory'] = 0;
            $this->stats['HomeGoalsHistory'] = 0;
            $this->stats['AwayWinsHistory'] = 0;
            $this->stats['AwayDrawsHistory'] = 0;
            $this->stats['AwayLostsHistory'] = 0;
            $this->stats['AwayGoalsHistory'] = 0;
            foreach ($this->history as $match) {
                $this->stats['MatchesHistory'] += 1;
                if (intval($match['ScoreHomeFT']) > intval($match['ScoreAwayFT'])) { $this->stats['HomeWinsHistory'] += 1; $this->stats['AwayLostsHistory'] += 1; }
                if (intval($match['ScoreHomeFT']) == intval($match['ScoreAwayFT'])) { $this->stats['HomeDrawsHistory'] += 1; $this->stats['AwayDrawsHistory'] += 1; }
                if (intval($match['ScoreHomeFT']) < intval($match['ScoreAwayFT'])) { $this->stats['HomeLostsHistory'] += 1; $this->stats['AwayWinsHistory'] += 1; }
                $this->stats['HomeGoalsHistory'] += intval($match['ScoreHomeFT']);
                $this->stats['AwayGoalsHistory'] += intval($match['ScoreAwayFT']);
            }
        }
    }
    // Functions
    public function queryResult() {
        $result = array();
        // MatchId
        $result['MatchId'] = $this->match['Id'];
        // Home 80%
        $result['101'] = $this->Over80PerCent($this->stats['HomeWins'], $this->stats['HomeMatches']);
        $result['102'] = $this->Over80PerCent($this->stats['HomeDraws'], $this->stats['HomeMatches']);
        $result['103'] = $this->Over80PerCent($this->stats['HomeLosts'], $this->stats['HomeMatches']);
        $result['104'] = $this->Over80PerCent($this->stats['HomeUnderCount'], $this->stats['HomeMatches']);
        $result['105'] = $this->Over80PerCent($this->stats['HomeOverCount'], $this->stats['HomeMatches']);
        $result['106'] = $this->Over80PerCent($this->stats['HomeGoalGoalCount'], $this->stats['HomeMatches']);
        $result['107'] = $this->Over80PerCent($this->stats['HomeNoGoalCount'], $this->stats['HomeMatches']);
        $result['108'] = $this->Over80PerCent($this->stats['HomeGoals0to1'], $this->stats['HomeMatches']);
        $result['109'] = $this->Over80PerCent($this->stats['HomeGoals2to3'], $this->stats['HomeMatches']);
        $result['110'] = $this->Over80PerCent($this->stats['HomeGoals4plus'], $this->stats['HomeMatches']);
        $result['111'] = $this->OverGoalMO($this->stats['HomeGoalsScored'], $this->stats['HomeMatches']);
        $result['112'] = $this->OverGoalMO($this->stats['HomeGoalsAgianst'], $this->stats['HomeMatches']);
        $result['113'] = $this->Over80PerCent($this->stats['HomeWinsHistory'], $this->stats['MatchesHistory']);
        $result['114'] = $this->Over80PerCent($this->stats['HomeDrawsHistory'], $this->stats['MatchesHistory']);
        $result['115'] = $this->Over80PerCent($this->stats['HomeLostsHistory'], $this->stats['MatchesHistory']);
        // Home 20%
        $result['201'] = $this->Under20PerCent($this->stats['HomeWins'], $this->stats['HomeMatches']);
        $result['202'] = $this->Under20PerCent($this->stats['HomeDraws'], $this->stats['HomeMatches']);
        $result['203'] = $this->Under20PerCent($this->stats['HomeLosts'], $this->stats['HomeMatches']);
        $result['204'] = $this->Under20PerCent($this->stats['HomeUnderCount'], $this->stats['HomeMatches']);
        $result['205'] = $this->Under20PerCent($this->stats['HomeOverCount'], $this->stats['HomeMatches']);
        $result['206'] = $this->Under20PerCent($this->stats['HomeGoalGoalCount'], $this->stats['HomeMatches']);
        $result['207'] = $this->Under20PerCent($this->stats['HomeNoGoalCount'], $this->stats['HomeMatches']);
        $result['208'] = $this->Under20PerCent($this->stats['HomeGoals0to1'], $this->stats['HomeMatches']);
        $result['209'] = $this->Under20PerCent($this->stats['HomeGoals2to3'], $this->stats['HomeMatches']);
        $result['210'] = $this->Under20PerCent($this->stats['HomeGoals4plus'], $this->stats['HomeMatches']);
        $result['211'] = $this->UnderGoalMO($this->stats['HomeGoalsScored'], $this->stats['HomeMatches']);
        $result['212'] = $this->UnderGoalMO($this->stats['HomeGoalsAgianst'], $this->stats['HomeMatches']);
        $result['213'] = $this->Under20PerCent($this->stats['HomeWinsHistory'], $this->stats['MatchesHistory']);
        $result['214'] = $this->Under20PerCent($this->stats['HomeDrawsHistory'], $this->stats['MatchesHistory']);
        $result['215'] = $this->Under20PerCent($this->stats['HomeLostsHistory'], $this->stats['MatchesHistory']);
        // Away 80%
        $result['301'] = $this->Over80PerCent($this->stats['AwayWins'], $this->stats['AwayMatches']);
        $result['302'] = $this->Over80PerCent($this->stats['AwayDraws'], $this->stats['AwayMatches']);
        $result['303'] = $this->Over80PerCent($this->stats['AwayLosts'], $this->stats['AwayMatches']);
        $result['304'] = $this->Over80PerCent($this->stats['AwayUnderCount'], $this->stats['AwayMatches']);
        $result['305'] = $this->Over80PerCent($this->stats['AwayOverCount'], $this->stats['AwayMatches']);
        $result['306'] = $this->Over80PerCent($this->stats['AwayGoalGoalCount'], $this->stats['AwayMatches']);
        $result['307'] = $this->Over80PerCent($this->stats['AwayNoGoalCount'], $this->stats['AwayMatches']);
        $result['308'] = $this->Over80PerCent($this->stats['AwayGoals0to1'], $this->stats['AwayMatches']);
        $result['309'] = $this->Over80PerCent($this->stats['AwayGoals2to3'], $this->stats['AwayMatches']);
        $result['310'] = $this->Over80PerCent($this->stats['AwayGoals4plus'], $this->stats['AwayMatches']);
        $result['311'] = $this->OverGoalMO($this->stats['AwayGoalsScored'], $this->stats['AwayMatches']);
        $result['312'] = $this->OverGoalMO($this->stats['AwayGoalsAgianst'], $this->stats['AwayMatches']);
        $result['313'] = $this->Over80PerCent($this->stats['AwayWinsHistory'], $this->stats['MatchesHistory']);
        $result['314'] = $this->Over80PerCent($this->stats['AwayDrawsHistory'], $this->stats['MatchesHistory']);
        $result['315'] = $this->Over80PerCent($this->stats['AwayLostsHistory'], $this->stats['MatchesHistory']);
        // Away 20%
        $result['401'] = $this->Under20PerCent($this->stats['AwayWins'], $this->stats['AwayMatches']);
        $result['402'] = $this->Under20PerCent($this->stats['AwayDraws'], $this->stats['AwayMatches']);
        $result['403'] = $this->Under20PerCent($this->stats['AwayLosts'], $this->stats['AwayMatches']);
        $result['404'] = $this->Under20PerCent($this->stats['AwayUnderCount'], $this->stats['AwayMatches']);
        $result['405'] = $this->Under20PerCent($this->stats['AwayOverCount'], $this->stats['AwayMatches']);
        $result['406'] = $this->Under20PerCent($this->stats['AwayGoalGoalCount'], $this->stats['AwayMatches']);
        $result['407'] = $this->Under20PerCent($this->stats['AwayNoGoalCount'], $this->stats['AwayMatches']);
        $result['408'] = $this->Under20PerCent($this->stats['AwayGoals0to1'], $this->stats['AwayMatches']);
        $result['409'] = $this->Under20PerCent($this->stats['AwayGoals2to3'], $this->stats['AwayMatches']);
        $result['410'] = $this->Under20PerCent($this->stats['AwayGoals4plus'], $this->stats['AwayMatches']);
        $result['411'] = $this->UnderGoalMO($this->stats['AwayGoalsScored'], $this->stats['AwayMatches']);
        $result['412'] = $this->UnderGoalMO($this->stats['AwayGoalsAgianst'], $this->stats['AwayMatches']);
        $result['413'] = $this->Under20PerCent($this->stats['AwayWinsHistory'], $this->stats['MatchesHistory']);
        $result['414'] = $this->Under20PerCent($this->stats['AwayDrawsHistory'], $this->stats['MatchesHistory']);
        $result['415'] = $this->Under20PerCent($this->stats['AwayLostsHistory'], $this->stats['MatchesHistory']);
        // Home Seri
        $result['501'] = $this->Over4Seri($this->stats['HomeWinsSeri']);
        $result['502'] = $this->Over4Seri($this->stats['HomeDrawsSeri']);
        $result['503'] = $this->Over4Seri($this->stats['HomeLostsSeri']);
        $result['504'] = $this->Over4Seri($this->stats['HomeUnderSeri']);
        $result['505'] = $this->Over4Seri($this->stats['HomeOverSeri']);
        $result['506'] = $this->Over4Seri($this->stats['HomeGoalGoalSeri']);
        $result['507'] = $this->Over4Seri($this->stats['HomeNoGoalSeri']);
        $result['508'] = $this->Over4Seri($this->stats['HomeGoalsSeri0to1']);
        $result['509'] = $this->Over4Seri($this->stats['HomeGoalsSeri2to3']);
        $result['510'] = $this->Over4Seri($this->stats['HomeGoalsSeri4plus']);
        $result['511'] = $this->Over4Seri($this->stats['HomeOver2GoalScoresSeri']);
        $result['512'] = $this->Over4Seri($this->stats['HomeOver2GoalAgainstSeri']);
        $result['513'] = $this->Over4Seri($this->stats['HomeWinsHistorySeri']);
        $result['514'] = $this->Over4Seri($this->stats['HomeDrawsHistorySeri']);
        $result['515'] = $this->Over4Seri($this->stats['HomeLostsHistorySeri']);
        // Away Seri
        $result['601'] = $this->Over4Seri($this->stats['AwayWinsSeri']);
        $result['602'] = $this->Over4Seri($this->stats['AwayDrawsSeri']);
        $result['603'] = $this->Over4Seri($this->stats['AwayLostsSeri']);
        $result['604'] = $this->Over4Seri($this->stats['AwayUnderSeri']);
        $result['605'] = $this->Over4Seri($this->stats['AwayOverSeri']);
        $result['606'] = $this->Over4Seri($this->stats['AwayGoalGoalSeri']);
        $result['607'] = $this->Over4Seri($this->stats['AwayNoGoalSeri']);
        $result['608'] = $this->Over4Seri($this->stats['AwayGoalsSeri0to1']);
        $result['609'] = $this->Over4Seri($this->stats['AwayGoalsSeri2to3']);
        $result['610'] = $this->Over4Seri($this->stats['AwayGoalsSeri4plus']);
        $result['611'] = $this->Over4Seri($this->stats['AwayOver2GoalScoresSeri']);
        $result['612'] = $this->Over4Seri($this->stats['AwayOver2GoalAgainstSeri']);
        $result['613'] = $this->Over4Seri($this->stats['AwayWinsHistorySeri']);
        $result['614'] = $this->Over4Seri($this->stats['AwayDrawsHistorySeri']);
        $result['615'] = $this->Over4Seri($this->stats['AwayLostsHistorySeri']);
        // Ο γηπεδούχος ήταν φαβορί και έχασε
        $result['701'] = FALSE;
        if ($this->homePrevMatch != NULL) {
            $odd = floatval($this->homePrevMatch['FT_1']);
            // ήταν φαβορί;
            if ($odd > 1 && $odd <= 1.65 ) {
                // κερδισέ;
                if (intval($this->homePrevMatch['ScoreHomeFT']) < intval($this->homePrevMatch['ScoreAwayFT'])) {
                    $result['701'] = TRUE;
                }
            }
        }
        // Ο φιλοξενούμενος ήταν φαβορί και έχασε
        $result['901'] = FALSE;
        if ($this->awayPrevMatch != NULL) {
            $odd = floatval($this->awayPrevMatch['FT_2']);
            // ήταν φαβορί;
            if ($odd > 1 && $odd <= 1.65 ) {
                // κερδισέ;
                if (intval($this->awayPrevMatch['ScoreHomeFT']) > intval($this->awayPrevMatch['ScoreAwayFT'])) {
                    $result['901'] = TRUE;
                }
            }
        }
        // Αν υπάρχει σκορ ημιχρόνου γραψε και τα αποτελεσματα στα γεγονότα
        if ($this->hasScoreHT()) {
            $goalsHost = intval($this->match['ScoreHomeHT']);
            $goalsGuest = intval($this->match['ScoreAwayHT']);
            $goalsTotal = $goalsHost + $goalsGuest;
            $result['R_1_HT'] = ($goalsHost > $goalsGuest) ? TRUE : FALSE;
            $result['R_X_HT'] = ($goalsHost == $goalsGuest) ? TRUE : FALSE;
            $result['R_2_HT'] = ($goalsHost < $goalsGuest) ? TRUE : FALSE;
            $result['R_1orX_HT'] = ($result['R_1_HT'] || $result['R_X_HT']) ? TRUE : FALSE;
            $result['R_Xor2_HT'] = ($result['R_X_HT'] || $result['R_2_HT']) ? TRUE : FALSE;
            $result['R_1or2_HT'] = ($result['R_1_HT'] || $result['R_2_HT']) ? TRUE : FALSE;
            $result['R_NoGoal_HT'] = ($goalsHost == 0 || $goalsGuest == 0);
            $result['R_GoalGoal_HT'] = ($goalsHost > 0 && $goalsGuest > 0);
            $result['R_Under0.5_HT'] = ($goalsTotal < 0.5);
            $result['R_Under1.5_HT'] = ($goalsTotal < 1.5);
            $result['R_Under2.5_HT'] = ($goalsTotal < 2.5);
            $result['R_Under3.5_HT'] = ($goalsTotal < 3.5);
            $result['R_Over0.5_HT'] = ($goalsTotal > 0.5);
            $result['R_Over1.5_HT'] = ($goalsTotal > 1.5);
            $result['R_Over2.5_HT'] = ($goalsTotal > 2.5);
            $result['R_Over3.5_HT'] = ($goalsTotal > 3.5);
        }
        // Αν υπάρχει τελικό σκορ γραψε και τα αποτελεσματα στα γεγονότα
        if ($this->hasScoreFT()) {
            $goalsHost = intval($this->match['ScoreHomeFT']);
            $goalsGuest = intval($this->match['ScoreAwayFT']);
            $goalsTotal = $goalsHost + $goalsGuest;
            $result['R_1_FT'] = ($goalsHost > $goalsGuest) ? TRUE : FALSE;
            $result['R_X_FT'] = ($goalsHost == $goalsGuest) ? TRUE : FALSE;
            $result['R_2_FT'] = ($goalsHost < $goalsGuest) ? TRUE : FALSE;
            $result['R_1orX_FT'] = ($result['R_1_FT'] || $result['R_X_FT']) ? TRUE : FALSE;
            $result['R_Xor2_FT'] = ($result['R_X_FT'] || $result['R_2_FT']) ? TRUE : FALSE;
            $result['R_1or2_FT'] = ($result['R_1_FT'] || $result['R_2_FT']) ? TRUE : FALSE;
            $result['R_NoGoal_FT'] = ($goalsHost == 0 || $goalsGuest == 0);
            $result['R_GoalGoal_FT'] = ($goalsHost > 0 && $goalsGuest > 0);
            $result['R_Under0.5_FT'] = ($goalsTotal < 0.5);
            $result['R_Under1.5_FT'] = ($goalsTotal < 1.5);
            $result['R_Under2.5_FT'] = ($goalsTotal < 2.5);
            $result['R_Under3.5_FT'] = ($goalsTotal < 3.5);
            $result['R_Over0.5_FT'] = ($goalsTotal > 0.5);
            $result['R_Over1.5_FT'] = ($goalsTotal > 1.5);
            $result['R_Over2.5_FT'] = ($goalsTotal > 2.5);
            $result['R_Over3.5_FT'] = ($goalsTotal > 3.5);
        }
        return $result;
    }
    public function hasScoreHT() {
        return (isset($this->match['ScoreHomeHT']) && isset($this->match['ScoreAwayHT']));
    }
    public function hasScoreFT() {
        return (isset($this->match['ScoreHomeFT']) && isset($this->match['ScoreAwayFT']));
    }
    public function queryName($Id) {
        return $this->names[intval($Id)];
    }
    private function Over80PerCent($num, $total) {
        if ($total >= 3) {
            $c = $num / $total;
            if ($c >= 0.8) return TRUE;
        }
        return FALSE;
    }
    private function Under20PerCent($num, $total) {
        if ($total >= 3) {
            $c = $num / $total;
            if ($c <= 0.2) return TRUE;
        }
        return FALSE;
    }
    private function OverGoalMO($num, $total) {
        if ($total >= 3) {
            $c = $num / $total;
            if ($c >= 1.5) return TRUE;
        }
        return FALSE;
    }
    private function UnderGoalMO($num, $total) {
        if ($total >= 3) {
            $c = $num / $total;
            if ($c < 0.99) return TRUE;
        }
        return FALSE;
    }
    private function Over4Seri($num) {
        if ($num >= 4) return TRUE;
        return FALSE;
    }
    // Static Functions
    static function queryHomeTeam($match) {
        $beginStartTime = date('Y-m-d', strtotime($match['StartTime'].' - 5 years'));
        $sql = "SELECT * FROM `football_opap_matches` ";
        $sql .= "WHERE `HomeTeamId`='".$match['HomeTeamId']."' ";
        $sql .= "AND `ChampId`='".$match['ChampId']."' ";
        $sql .= "AND `ScoreHomeFT` IS NOT NULL ";
        $sql .= "AND `ScoreAwayFT` IS NOT NULL ";
        $sql .= "AND `StartTime`>'".$beginStartTime."' ";
        $sql .= "AND `StartTime`<'".$match['StartTime']."' ";
        $sql .= "ORDER BY `StartTime` DESC LIMIT 0, 6;";
        return mysqlQuerySelect($sql);
    }
    static function queryAwayTeam($match) {
        $beginStartTime = date('Y-m-d', strtotime($match['StartTime'].' - 5 years'));
        $sql = "SELECT * FROM `football_opap_matches` ";
        $sql .= "WHERE `AwayTeamId`='".$match['AwayTeamId']."' ";
        $sql .= "AND `ChampId`='".$match['ChampId']."' ";
        $sql .= "AND `ScoreHomeFT` IS NOT NULL ";
        $sql .= "AND `ScoreAwayFT` IS NOT NULL ";
        $sql .= "AND `StartTime`>'".$beginStartTime."' ";
        $sql .= "AND `StartTime`<'".$match['StartTime']."' ";
        $sql .= "ORDER BY `StartTime` DESC LIMIT 0, 6;";
        return mysqlQuerySelect($sql);
    }
    static function queryMatchHistory($match) {
        $beginStartTime = date('Y-m-d', strtotime($match['StartTime'].' - 5 years'));
        $sql = "SELECT * FROM `football_opap_matches` ";
        $sql .= "WHERE `HomeTeamId`='".$match['HomeTeamId']."' ";
        $sql .= "AND `AwayTeamId`='".$match['AwayTeamId']."' ";
        $sql .= "AND `ChampId`='".$match['ChampId']."' ";
        $sql .= "AND `ScoreHomeFT` IS NOT NULL ";
        $sql .= "AND `ScoreAwayFT` IS NOT NULL ";
        $sql .= "AND `StartTime`>'".$beginStartTime."' ";
        $sql .= "AND `StartTime`<'".$match['StartTime']."' ";
        $sql .= "ORDER BY `StartTime` DESC LIMIT 0, 6;";
        return mysqlQuerySelect($sql);
    }
    static function queryNames() {
        $sql = "SELECT * FROM `football_queries`;";
        $names = array();
        $records = mysqlQuerySelect($sql);
        foreach ($records as $record) {
            $names[$record['Id']] = $record['Name'];
        }
        return $names;
    }
}
// Functions
function writeMatchesTable($title, $match, $matches) {
?>
<table style="width: 740px;">
    <tr><th colspan="16"><?php echo $title; ?></th></tr>
    <tr>
        <?php if (isset($matches)) { echo '<th>#</th>'; } ?>
        <th>Champ</th>
        <th>StartTime</th>
        <th>1</th>
        <th>+</th>
        <th>HomeTeam</th>
        <th>X</th>
        <th>AwayTeam</th>
        <th>+</th>
        <th>2</th>
        <th>U</th>
        <th>O</th>
        <th>GG</th>
        <th>NG</th>
        <th>Half</th>
        <th>Score</th>
    </tr>
    <?php
        if (isset($match)) {
             writeMatchRow(0, $match);
        } else if (isset($matches)) {
            foreach ($matches as $i => $m) { writeMatchRow($i + 1, $m); }
        }
    ?>
</table>
<?php
}
function writeMatchRow($i, $match) {
    echo '<tr>';
    if (!empty($i)) echo '<td>'.$i.'</td>';
    echo '<td>'.$match['Champ'].'</td>';
    echo '<td>'.$match['StartTime'].'</td>';
    echo '<td>'.formatOdd($match['FT_1']).'</td>';
    echo '<td class="adv">'.formatAdv($match['AdvHome']).'</td>';
    echo '<td class="left">'.$match['HomeTeam'].'</td>';
    echo '<td>'.formatOdd($match['FT_X']).'</td>';
    echo '<td class="right">'.$match['AwayTeam'].'</td>';
    echo '<td class="adv">'.formatAdv($match['AdvAway']).'</td>';
    echo '<td>'.formatOdd($match['FT_2']).'</td>';
    echo '<td>'.formatOdd($match['Under']).'</td>';
    echo '<td>'.formatOdd($match['Over']).'</td>';
    echo '<td>'.formatOdd($match['Goal']).'</td>';
    echo '<td>'.formatOdd($match['NoGoal']).'</td>';
    echo '<td>'.$match['ScoreHomeHT'].'-'.$match['ScoreAwayHT'].'</td>';
    echo '<td>'.$match['ScoreHomeFT'].'-'.$match['ScoreAwayFT'].'</td>';
echo '</tr>';
}
function formatOdd($odd) {
    if (empty($odd)) return '-';
    return number_format($odd, 2);
}
function formatAdv($adv) {
    if (empty($adv)) return '';
    return '+'.$adv;
}
function writeTdTrueFalse($value) {
    echo ($value == 1) ? '<td class="true">True</td>' : '<td class="false">False</td>';
}
// Ececute
switch ($_GET['action']) {
    case 'update-match':
        header('Content-Type: application/json; charset=utf-8');
        $s = new MatchStatistics($_GET['matchid']);
        mysqlInsertOrUpdateFromArray('football_queries_results', $s->queryResult());
        echo json_encode($s->match);
        break;
    case 'view-match':
        header('Content-Type: text/html; charset=utf-8');
        $s = new MatchStatistics($_GET['matchid']);
        if (empty($s->match)) die("Not Found");
        $result = $s->queryResult();
?>
<!DOCTYPE html>
<html>
<head>
	<title>Query - details</title>
    <style>
        body {
            font-family: Verdana;
            font-size: 12px;
            margin: 0px;
            padding: 0px;
        }
        table, td, th, tr {
            white-space: nowrap;
            border-collapse: collapse;
            border: 1px solid black;
        }
        th {
            background-color: DarkGray;
        }
        tr {
            background-color: LightGray;
            text-align: center;
        }
        tr:hover {
            background-color: khaki;
        }
        .true {
            color: green;
            font-weight: bold;
        }
        .false {
            color: red;
        }
        .left {
            text-align: left;
        }
        .right {
            text-align: right;
        }
        .adv {
            color: blue;
        }
        .body-td {
            background-color: white;
            border-color: white;
            vertical-align: top;
        }
    </style>
</head>
<body>
    <table>
        <tr>
            <td class="body-td">
                <?php writeMatchesTable('Match Info', $s->match, NULL); ?>
                <br />
                <?php writeMatchesTable('HomeTeam Matches', NULL, $s->home); ?>
                <br />
                <?php writeMatchesTable('AwayTeam Matches', NULL, $s->away); ?>
                <br />
                <?php writeMatchesTable('History', NULL, $s->history); ?>
            </td>
            <td class="body-td">
                <table>
                    <tr><th>Home</th><th></th><th>Away</th></tr>
                    <tr><td><?php echo $s->stats['HomeWins']; ?></td><th>Wins</th><td><?php echo $s->stats['AwayWins']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeDraws']; ?></td><th>Draws</th><td><?php echo $s->stats['AwayDraws']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeLosts']; ?></td><th>Losts</th><td><?php echo $s->stats['AwayLosts']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoalsScored']; ?></td><th>Goals Scored</th><td><?php echo $s->stats['AwayGoalsScored']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoalsAgianst']; ?></td><th>Goals Agianst</th><td><?php echo $s->stats['AwayGoalsAgianst']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeUnderCount']; ?></td><th>Under</th><td><?php echo $s->stats['AwayUnderCount']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeOverCount']; ?></td><th>Over</th><td><?php echo $s->stats['AwayOverCount']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeNoGoalCount']; ?></td><th>No-Goal</th><td><?php echo $s->stats['AwayNoGoalCount']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoalGoalCount']; ?></td><th>Goal-Goal</th><td><?php echo $s->stats['AwayGoalGoalCount']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoals0to1']; ?></td><th>Goals 0 to 1</th><td><?php echo $s->stats['AwayGoals0to1']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoals2to3']; ?></td><th>Goals 2 to 3</th><td><?php echo $s->stats['AwayGoals2to3']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoals4plus']; ?></td><th>Goals 4+</th><td><?php echo $s->stats['AwayGoals4plus']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeWinsSeri']; ?></td><th>WinsSeri</th><td><?php echo $s->stats['AwayWinsSeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeDrawsSeri']; ?></td><th>DrawsSeri</th><td><?php echo $s->stats['AwayDrawsSeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeLostsSeri']; ?></td><th>LostsSeri</th><td><?php echo $s->stats['AwayLostsSeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeUnderSeri']; ?></td><th>UnderSeri</th><td><?php echo $s->stats['AwayUnderSeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeOverSeri']; ?></td><th>OverSeri</th><td><?php echo $s->stats['AwayOverSeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeNoGoalSeri']; ?></td><th>NoGoalSeri</th><td><?php echo $s->stats['AwayNoGoalSeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoalGoalSeri']; ?></td><th>GoalGoalSeri</th><td><?php echo $s->stats['AwayGoalGoalSeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoalsSeri0to1']; ?></td><th>GoalsSeri0to1</th><td><?php echo $s->stats['AwayGoalsSeri0to1']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoalsSeri2to3']; ?></td><th>GoalsSeri2to3</th><td><?php echo $s->stats['AwayGoalsSeri2to3']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoalsSeri4plus']; ?></td><th>GoalsSeri4plus</th><td><?php echo $s->stats['AwayGoalsSeri4plus']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeOver2GoalScoresSeri']; ?></td><th>Over2GoalScoresSeri</th><td><?php echo $s->stats['AwayOver2GoalScoresSeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeOver2GoalAgainstSeri']; ?></td><th>Over2GoalAgainstSeri</th><td><?php echo $s->stats['AwayOver2GoalAgainstSeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeWinsHistorySeri']; ?></td><th>WinsHistorySeri</th><td><?php echo $s->stats['AwayWinsHistorySeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeDrawsHistorySeri']; ?></td><th>DrawsHistorySeri</th><td><?php echo $s->stats['AwayDrawsHistorySeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeLostsHistorySeri']; ?></td><th>LostsHistorySeri</th><td><?php echo $s->stats['AwayLostsHistorySeri']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeWinsHistory']; ?></td><th>WinsHistory</th><td><?php echo $s->stats['AwayWinsHistory']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeDrawsHistory']; ?></td><th>DrawsHistory</th><td><?php echo $s->stats['AwayDrawsHistory']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeLostsHistory']; ?></td><th>LostsHistory</th><td><?php echo $s->stats['AwayLostsHistory']; ?></td></tr>
                    <tr><td><?php echo $s->stats['HomeGoalsHistory']; ?></td><th>GoalsHistory</th><td><?php echo $s->stats['AwayGoalsHistory']; ?></td></tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="body-td">
                <table style="width: 740px;">
                    <tr>
                        <th>Code</th>
                        <th>Name</th>
                        <th>Value</th>
                    </tr>
                    <?php foreach ($result as $key => $value) { if ($key != 'MatchId' && $key[0] != 'R') { ?>
                    <tr>
                        <td><?php echo $key; ?></td>
                        <td class="left"><?php echo $s->queryName($key); ?></td>
                        <?php writeTdTrueFalse($value); ?>
                    </tr>
                    <?php }} ?>
                </table>
            </td>
            <td class="body-td">
                <table>
                    <tr><?php if ($s->hasScoreHT()) echo '<th>HT</th>'; ?><th>Result</th><?php if ($s->hasScoreFT()) echo '<th>FT</th>'; ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_1_HT']); ?><th>1</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_1_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_X_HT']); ?><th>X</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_X_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_2_HT']); ?><th>2</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_2_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_1orX_HT']); ?><th>1 or X</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_1orX_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_Xor2_HT']); ?><th>X or 2</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_Xor2_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_1or2_HT']); ?><th>1 or 2</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_1or2_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_NoGoal_HT']); ?><th>No-Goal</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_NoGoal_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_GoalGoal_HT']); ?><th>Goal-Goal</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_GoalGoal_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_Under0.5_HT']); ?><th>Under 0.5</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_Under0.5_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_Under1.5_HT']); ?><th>Under 1.5</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_Under1.5_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_Under2.5_HT']); ?><th>Under 2.5</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_Under2.5_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_Under3.5_HT']); ?><th>Under 3.5</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_Under3.5_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_Over0.5_HT']); ?><th>Over 0.5</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_Over0.5_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_Over1.5_HT']); ?><th>Over 1.5</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_Over1.5_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_Over2.5_HT']); ?><th>Over 2.5</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_Over2.5_FT']); ?></tr>
                    <tr><?php if ($s->hasScoreHT()) writeTdTrueFalse($result['R_Over3.5_HT']); ?><th>Over 3.5</th><?php if ($s->hasScoreFT()) writeTdTrueFalse($result['R_Over3.5_FT']); ?></tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
<?php
        break;
    case 'get-list-all':
        header('Content-Type: application/json; charset=utf-8');
        $sql = "SELECT `Id` FROM `football_opap_matches`";
        $sql .= " ORDER BY `StartTime` ASC;";
        $result = mysqlQuerySelect($sql);
        $list = array();
        foreach ($result as $value) { $list[] += $value['Id']; }
        echo json_encode($list);
        break;
    case 'get-list-existing':
        header('Content-Type: application/json; charset=utf-8');
        $sql = "SELECT `MatchId` AS `Id` FROM `football_queries_results`";
        $sql .= " ORDER BY `StartTime` ASC;";
        $result = mysqlQuerySelect($sql);
        $list = array();
        foreach ($result as $value) { $list[] += $value['Id']; }
        echo json_encode($list);
        break;
    case 'get-list-new':
        header('Content-Type: application/json; charset=utf-8');
        $sql = "SELECT `Id` FROM `football_opap_matches`";
        $sql .= " WHERE `Id` NOT IN (SELECT `MatchId` AS `Id` FROM `football_queries_results`)";
        $sql .= " ORDER BY `StartTime` ASC;";
        $result = mysqlQuerySelect($sql);
        $list = array();
        foreach ($result as $value) { $list[] += $value['Id']; }
        echo json_encode($list);
        break;
    case 'get-list-update':
        header('Content-Type: application/json; charset=utf-8');
        $dateStart = date('Y-m-d H:i:s', strtotime('-2 hours'));
        $dateEnd = date("Y-m-d", strtotime($dateStart . ' -1 days'));
        $sql = "SELECT `Id` FROM `football_opap_queries`";
        $sql .= " WHERE (`R_1_FT` IS NULL OR `R_X_FT` IS NULL OR `R_2_FT` IS NULL)";
        $sql .= " AND `StartTime` < '{$dateStart}'";
        $sql .= " AND `StartTime` > '{$dateEnd}'";
        $sql .= " ORDER BY `StartTime` ASC;";
        $result = mysqlQuerySelect($sql);
        $list = array();
        foreach ($result as $value) { $list[] += $value['Id']; }
        echo json_encode($list);
        break;
    case 'get-champ-history':
        header('Content-Type: application/json; charset=utf-8');
        $history = getChampHistory($_GET['champ_id'], $_GET['date_start'], $_GET['date_end']);
        echo json_encode($history);
        //echo print_r($history);
        break;
}
function getChampHistory($champ_id, $date_start, $date_end) {
    $sql = "SELECT *";
    $sql .= " FROM `football_opap_queries`";
    $sql .= " WHERE `ChampId` = '{$champ_id}'";
    $sql .= " AND `StartTime` <= '{$date_start}'";
    $sql .= " AND `StartTime` >= '{$date_end}'";
    $sql .= " AND `ScoreHomeFT` IS NOT NULL";
    $sql .= " AND `ScoreAwayFT` IS NOT NULL";
    $history = mysqlQuerySelect($sql);
    return $history;
}
?>
<?php // End gzip compressing
while (@ob_end_flush());
?>
