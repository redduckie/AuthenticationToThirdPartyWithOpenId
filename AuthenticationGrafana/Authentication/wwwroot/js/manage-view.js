var userName = "";

$(document).ready(function () {

    $(".user-roles").click(function () {
        var row = $(this).closest('tr');
        var cell = row.find('td:nth-child(1)');
        userName = cell.html();
        $("#manageViews").load("/Account/UserRolesAsync", { userName: userName });
    });

    $(".user-dashboard-roles").click(function () {
        var row = $(this).closest('tr');
        var cell = row.find('td:nth-child(1)');
        userName = cell.html();
        $("#manageViews").load("/Account/UserDashboardRolesAsync", { userName: userName });
    });
});


$(document).ajaxComplete(function (event, xhr, settings) {

    $("#btnAdd").click(function () {
        var roleName = $("#IdentityRole_Name option:selected").text();
        var model = {
            UserName: userName,
            RoleName: roleName
        };
        $.ajax({
            url: '/Account/AddUserRoleAsync',
            type: 'POST',
            data: JSON.stringify(model),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $("#manageViews").load("/Account/UserRolesAsync", { userName: userName });
            }
        });

    });

    $(".user-role-delete").click(function () {
        var row = $(this).closest('tr');
        var cell = row.find('td:nth-child(1)');
        var userRole = cell.html();
        var model = { UserName: userName, RoleName: userRole };
        $.ajax({
            url: '/Account/RemoveUserRoleAsync',
            type: 'POST',
            data: JSON.stringify(model),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $("#manageViews").load("/Account/UserRolesAsync", { userName: userName });
            }
        });
    });

    $("#btnUpdateDashboardRole").click(function () {
        userDashboardRole = $("#UserGrafanaRole option:selected").text();
        var model = {
            UserName: userName,
            UserGrafanaRole: userDashboardRole
        }
        $.ajax({
            url: '/Account/UpdateUserDashboardRole',
            type: 'POST',
            data: JSON.stringify(model),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $("#manageViews").load("/Account/UserRolesAsync", { userName: userName });
            }
        });
    });

    $("#btnAddDashboardRole").click(function () {
        var roleId = $("#UserRole option:selected").val();
        var dashboardId = $("#SelectedDashboardName option:selected").val();

        var model = {
            UserName: userName,
            RoleId: roleId,
            DashboardId: dashboardId
        };

        $.ajax({
            url: '/Account/AddUserToDashboardPermissions',
            type: 'POST',
            data: JSON.stringify(model),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $("#manageViews").load("/Account/UserDashboardRolesAsync", { userName: userName });
            }
        });

    });

    $('.user-dashboard-permission-delete').click(function () {
        var row = $(this).closest('tr');
        var dashboardId = row.find('td:nth-child(1)').find('input').val();
        var roleId = row.find('td:nth-child(2)').find('input').val();
        var model = {
            UserName: userName,
            DashboardId: dashboardId,
            RoleId: roleId
        };

        $.ajax({
            url: '/Account/DeleteUserDashboardPermissions',
            type: 'POST',
            data: JSON.stringify(model),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $("#manageViews").load("/Account/UserDashboardRolesAsync", { userName: userName });
            }
        });

    });

});