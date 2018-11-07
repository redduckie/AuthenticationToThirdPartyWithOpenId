var dashboardName = "";
$(document).ready(function () {

    $(".dashboard-user-roles").click(function () {
        var row = $(this).closest('tr');
        var dashboardId = row.find('td:nth-child(1)').find('input').val();
        dashboardName = row.find('td:nth-child(2)').html();
        $("#dashboard-options").load("/DashboardPermission/GetDashboardUserPermissionsAsync", { dashboardId: dashboardId, dashboardTitle: dashboardName });
    });
});

$(document).ajaxComplete(function (event, xhr, settings) {

});