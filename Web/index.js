window.onload = function () {
    tableRefresh();
    setInterval(function () {
        tableRefresh();
    }, 2000);
};
function tableRefresh() {
    var url = encodeURI('./live.php?sport=' + $('#SelectSport').val());
    $.ajax({
        type: 'GET',
        url: url,
        dataType: 'text',
        cache: false
    }).done(function (text) {
        $('#DivCoupon').html(text);
        $('#LabelMessage').text((new Date()).toString());
    }).fail(function () {
        $('#LabelMessage').text('Communication error!!');
    });
}