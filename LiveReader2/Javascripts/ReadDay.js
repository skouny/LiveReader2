(function () {
    var csv = '';
    var log = 'Begin';
    try {
        var target = document.getElementsByClassName("event-list-table-wrapper js-event-list-table-wrapper")[0];
        var champs = target.getElementsByClassName("js-event-list-tournament tournament");
        var events_count = 0;
        if (champs.length > 0) {
            log = 'Found ' + champs.length + ' champs';
            for (var c = 0; c < champs.length; c++) {
                var header = champs[c].getElementsByClassName("cell cell--event-list js-event-list-tournament-header")[0];
                var events = champs[c].getElementsByClassName("js-event-list-tournament-events")[0].getElementsByTagName("a");
                var header_cells = header.getElementsByClassName("cell__section--main")[0].getElementsByClassName("cell__content");
                var country = header_cells[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                var champ = header_cells[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                log = 'Reading champ ' + c + ': ' + champ;
                for (var e = 0; e < events.length; e++) {
                    var event = events[e];
                    var cells = event.children;
                    if (cells.length == 7) {
                        events_count++;
                        var webid = event.getAttribute("data-id");
                        var url = document.location.origin + event.getAttribute("href");
                        var start_time = cells[0].children[0].textContent.replace(/\s/g, '').trim();
                        var status = cells[0].getAttribute("title").trim();
                        var home_team = cells[2].children[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                        var away_team = cells[2].children[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                        var home_score_ft = cells[4].children[0].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                        var away_score_ft = cells[4].children[1].textContent.replace(/\s/g, ' ').replace(/ +/g, ' ').trim();
                        var line = webid;
                        line += '\t' + country;
                        line += '\t' + champ;
                        line += '\t' + start_time;
                        line += '\t' + home_team;
                        line += '\t' + away_team;
                        line += '\t' + home_score_ft;
                        line += '\t' + away_score_ft;
                        line += '\t' + status;
                        line += '\t' + url;
                        csv += line + '\n';
                    }
                }
            }
        }
        log = 'End';
    } catch (err) {
        return 'ERROR!! [ReadDay.js] => Log: ' + log + ', Message: ' + err.Message;
    }
    return csv;
})();