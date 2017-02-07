/**
 * This functiion allows convertion from a UTC date to the local date
 */

$(function () {
    $("[data-locationtime]").each(function () {
        var date = moment.utc($(this).attr("data-locationtime"), "DD/MM/YYYY HH:mm:ss A").toDate();

        var localtime = moment(date).format("DD/MM/YYYY HH:mm:ss A");
        $(this).text(localtime);
    });
});