$(function () {
    $("#menu").menu();
	getLiveDays();
	setTimeout(function() { tableRefresh(true); }, 1000);
	setInterval(function () { tableRefresh(false); }, 60000);
});
function initSelectedSport() {
	getLiveDays();
    tableRefresh(true);
}
function getLiveDays() {
	var url = 'live_days.php?sport=' + $('#SelectSport').val();
	var url = encodeURI(url);
	$.ajax({
		type: 'GET',
		url: url,
		dataType: 'json',
		cache: false
	}).done(function (days) {
		$('#SelectDay').empty();
		var count = days.length;
		for (var i = 0; i < count; i++) {
			var day = days[i];
			$('#SelectDay').append('<option>' + day + '</option>');
		};
	}).fail(function () {
		$('#SpanMessage').text('Communication error!!');
	});
}
function tableRefresh(IsOrder) {
    if (IsOrder || document.getElementById('CheckboxAutoRefresh').checked) {
        var url = 'live.php?sport=' + $('#SelectSport').val() 
		  + '&source=' + $('#SelectSource').val() 
		  + '&day=' + $('#SelectDay').val() 
		  + '&champ=' + $('#TextChamp').val();
        var url = encodeURI(url);
        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'text',
            cache: false
        }).done(function (text) {
            $('#DivCoupon').html(text);
            $('#SpanMessage').text((new Date()).toLocaleTimeString());
        }).fail(function () {
            $('#SpanMessage').text('Communication error!!');
        });
    }
}
function deleteRecord(sport, source, webId) {
    var url = 'table_live_mix_delete.php?sport=' + sport + '&source=' + source + '&webid=' + webId;
    var url = encodeURI(url);
    $.ajax({
        type: 'GET',
        url: url,
        dataType: 'text',
        cache: false
    }).done(function (text) {
        tableRefresh(true);
    }).fail(function () {
        alert("Error");
    });
}
function deleteOldRecords() {
	var sport = $('#SelectSport').val();
    var url = encodeURI('table_live_mix_delete.php?sport=' + sport + '&delete-old-records');
    $.ajax({
        type: 'GET',
        url: url,
        dataType: 'text',
        cache: false
    }).done(function (message) {
        alert(message);
    }).fail(function () {
        alert("Error");
    });
}