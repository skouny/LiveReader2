(function () {
    let data = {};
    let table_live = document.querySelector("#f24com_tablelive");
    let table_results = document.querySelector("#f24com_tableresults");
    // Selected Date
    let s = document.querySelector("#f24com_date > span").textContent.split(".");
    let day = parseInt(s[0]);
    let month = parseInt(s[1]) - 1;
    let year = parseInt(s[2]);
    let today_date = new Date(year, month, day);
    let tomorrow_date = new Date(year, month, day + 1);
    let today = today_date.getFullYear() + "-" + (today_date.getMonth() + 1) + "-" + today_date.getDate();
    let tomorrow = tomorrow_date.getFullYear() + "-" + (tomorrow_date.getMonth() + 1) + "-" + tomorrow_date.getDate();
    console.log(today);
    console.log(tomorrow);
    // Live
    if (table_live) {
        table_live.querySelectorAll("tr.match").forEach(function (tr) {
            let matchId = tr.id;
            data[matchId] = {};
            data[matchId]["Champ"] = tr.querySelector("td.league").textContent.trim();
            data[matchId]["HomeTeam"] = tr.querySelector("td.home").textContent.trim();
            data[matchId]["AwayTeam"] = tr.querySelector("td.guest").textContent.trim();
            let status = tr.querySelector("td.status").textContent.trim();
            if (status.includes(":")) {
                let t = status.split(":");
                let hour = parseInt(t[0]);
                let minute = parseInt(t[1]);
                if (hour > 7) {
                    data[matchId]["StartTime"] = today + " " + status;
                } else {
                    data[matchId]["StartTime"] = tomorrow + " " + status;
                }
            } else {
                data[matchId]["Status"] = status;
            }
            let result = tr.querySelector("td.result");
            let result1 = result.querySelector("span.result1");
            let result2 = result.querySelector("span.result2");
            if (result1 && result2) {
                let s1 = result1.textContent.split("-");
                data[matchId]["HomeScore"] = s1[0];
                data[matchId]["AwayScore"] = s1[1];
                let s2 = result2.textContent.replace("(", "").replace(")", "").split("-");
                data[matchId]["HomeScoreHT"] = s2[0];
                data[matchId]["AwayScoreHT"] = s2[1];
            }
        });
    }
    // Result
    if (table_results) {
        table_results.querySelectorAll("tr.match").forEach(function (tr) {
            let matchId = tr.id;
            data[matchId] = {};
            data[matchId]["Champ"] = tr.querySelector("td.league").textContent.trim();
            data[matchId]["HomeTeam"] = tr.querySelector("td.home").textContent.trim();
            data[matchId]["AwayTeam"] = tr.querySelector("td.guest").textContent.trim();
            data[matchId]["Status"] = tr.querySelector("td.status").textContent.trim();
            let result = tr.querySelector("td.result");
            let result1 = result.querySelector("span.result1");
            let result2 = result.querySelector("span.result2");
            if (result1 && result2) {
                let s1 = result1.textContent.split("-");
                data[matchId]["HomeScore"] = s1[0];
                data[matchId]["AwayScore"] = s1[1];
                let s2 = result2.textContent.replace("(", "").replace(")", "").split("-");
                data[matchId]["HomeScoreHT"] = s2[0];
                data[matchId]["AwayScoreHT"] = s2[1];
            }
        });
    }
    return JSON.stringify(data);
})();