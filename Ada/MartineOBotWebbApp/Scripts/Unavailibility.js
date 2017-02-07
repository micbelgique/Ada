window.onload = function () {
    $("#collapseDate").hide();
    $("#durationOptions .btn:not(:last-child)").click(function () {
        $("#collapseDate").hide();
    });
    $("#otherButton").click(function () {
        $("#collapseDate").show();
    });
}
