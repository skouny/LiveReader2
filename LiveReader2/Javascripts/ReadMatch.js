(function () {
    var csv = '';
    var step = 'Begin...';
    try {
        // Read Teams
        step = 'Reading Teams...';
        var teams_container = document.getElementsByClassName("page-title");
        if (teams_container.length > 0) {
            var Teams = teams_container[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').split('-');
            if (Teams.length == 2) {
                csv += 'HomeTeam' + '\t' + Teams[0].trim() + '\n';
                csv += 'AwayTeam' + '\t' + Teams[1].trim() + '\n';
            }
        }
        // Read Score
        step = 'Reading Score...';
        var score_container = document.getElementsByClassName("cell__section--main u-tC");
        if (score_container.length > 0) {
            var score = score_container[0].children;
            if (score.length >= 2) {
                step = 'Reading ScoreFT...';
                var scoreFT = score[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').split('-');
                if (scoreFT.length == 2) {
                    csv += 'HomeScoreFT' + '\t' + scoreFT[0].trim() + '\n';
                    csv += 'AwayScoreFT' + '\t' + scoreFT[1].trim() + '\n';
                }
                step = 'Reading ScoreHT...';
                var scoreHT = score[1].textContent.replace(/\(|\)/g, '').replace(/\s/g, ' ').replace(/ +/g, ' ').split('-');
                if (scoreHT.length == 2) {
                    csv += 'HomeScoreHT' + '\t' + scoreHT[0].trim() + '\n';
                    csv += 'AwayScoreHT' + '\t' + scoreHT[1].trim() + '\n';
                }
            }
        }
        // Read Incidents
        step = 'Reading Incidents...';
        var incidents_container = document.getElementsByClassName("incidents-container");
        if (incidents_container.length > 0) {
            var incidents = incidents_container[0].children;
            if (incidents.length > 1) {
                step = 'Found ' + incidents.length + ' incidents...';
                for (var e = 1; e < incidents.length; e++) {
                    step = 'Reading incident ' + e;
                    var incident = incidents[e];
                    step = 'Reading incident ' + incident.textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                    var incident_class = incident.getAttribute("class");
                    var incident_side = '-';
                    var incident_title = '-';
                    var incident_minute = '-';
                    var incident_score = '-';
                    var incident_player = '-';
                    var incident_description = '-';
                    if (incident_class == 'cell cell--incident') {
                        incident_side = 'Home';
                        incident_title = incident.children[0].getAttribute("title");
                        incident_minute = incident.children[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                        var incident_content = incident.children[2].children[0].children;
                        if (incident_title == 'Substitution') {
                            incident_player = incident_content[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                            incident_description = incident_content[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                        } else if (incident_title == 'Goal' || incident_title == 'Own goal') {
                            for (var i = 0; i < incident_content.length; i++) {
                                var text = incident_content[i].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                                switch (i) {
                                    case 0: incident_score = text; break;
                                    case 1: incident_player = text; break;
                                    case 2: incident_description = text; break;
                                }
                            }
                        } else { // is card
                            incident_player = incident.children[2].children[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                        }
                    } else if (incident_class == 'cell cell--right cell--incident') {
                        incident_side = 'Away';
                        incident_title = incident.children[2].getAttribute("title");
                        incident_minute = incident.children[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                        var incident_content = incident.children[0].children[0].children;
                        if (incident_title == 'Substitution') {
                            incident_player = incident_content[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                            incident_description = incident_content[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                        } else if (incident_title == 'Goal' || incident_title == 'Own goal') {
                            for (var i = incident_content.length - 1; i >= 0; i--) {
                                var text = incident_content[i].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                                switch (incident_content.length - 1 - i) {
                                    case 0: incident_score = text; break;
                                    case 1: incident_player = text; break;
                                    case 2: incident_description = text; break;
                                }
                            }
                        } else { // is card
                            incident_player = incident.children[0].children[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                        }
                    } else if (incident_class == 'cell cell--center cell--incident') {
                        var incident_description = incident.textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                    }
                    csv += 'Incident' + '\t' + incident_side + '\t' + incident_title + '\t' + incident_minute + '\t' + incident_score + '\t' + incident_player + '\t' + incident_description + '\n';
                }
            }
        }
        // Statistics
        step = 'Reading Statistics...';
        var statistics_container = document.getElementsByClassName("statistics-container");
        if (statistics_container.length > 0) {
            var statistics = statistics_container[0].children[1].children;
            if (statistics.length > 0) {
                step = 'Found ' + statistics.length + ' statistics...';
                for (var s = 0; s < statistics.length; s++) {
                    var stat_group = statistics[s].children;
                    for (var g = 0; g < stat_group.length; g++) {
                        var stat_incident = stat_group[g].children;
                        if (stat_incident.length == 3) {
                            var val_home = stat_incident[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                            var val_name = stat_incident[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                            var val_away = stat_incident[2].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                            csv += 'Stat' + '\t' + val_name + '\t' + val_home + '\t' + val_away + '\n';
                        }
                    }
                }
            }
        }
        // Votes
        step = 'Reading Votes...';
        var votes_container = document.getElementsByClassName("cell vote__stats js-vote-stats-container");
        if (statistics_container.length > 0) {
            var votes = votes_container[0].children;
            var votes1 = votes[0].children;
            var votes1count = votes1[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
            var votes1percent = votes1[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
            csv += 'Votes' + '\t' + '1' + '\t' + votes1count + '\t' + votes1percent + '\n';
            var votesX = votes[1].children;
            var votesXcount = votesX[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
            var votesXpercent = votesX[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
            csv += 'Votes' + '\t' + 'X' + '\t' + votesXcount + '\t' + votesXpercent + '\n';
            var votes2 = votes[2].children;
            var votes2count = votes2[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
            var votes2percent = votes2[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
            csv += 'Votes' + '\t' + '2' + '\t' + votes2count + '\t' + votes2percent + '\n';
        }
        // End
        step = 'End';
    } catch (err) {
        return 'ERROR!! [ReadMatch.js] => Step: ' + step + ', Message: ' + err.Message;
    }
    return csv;
})();