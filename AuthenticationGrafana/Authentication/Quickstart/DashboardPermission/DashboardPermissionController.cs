using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Authentication.Interfaces;
using Authentication.Models;
using Authentication.Models.DashboardViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authentication.Quickstart.DashboardPermission
{
    public class DashboardPermissionController:Controller
    {
        private readonly IGrafanaHelper _grafanaHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashboardPermissionController(IGrafanaHelper grafanaHelper, UserManager<ApplicationUser> userManager)
        {
            _grafanaHelper = grafanaHelper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardIndexViewModel();

            var allDashboards = await _grafanaHelper.GetAllDashboardsAsync();
            //model.DashboardSelectList = new SelectList(allDashboards, "Id", "Title");
            model.DashboardsList = allDashboards;

            return View(model);
        }

        public async Task<IActionResult> GetDashboardUserPermissionsAsync(int dashboardId, string dashboardTitle)
        {
            var dashBoardUserRoles = await _grafanaHelper.GetDashboardPermissionItemsAsync(dashboardId);
            var dashboardAccessorsList = new List<DashboardAccessorPermissionViewModel>();

            foreach(var dashboardUserRole in dashBoardUserRoles)
            {
                var dashboardAccessor = new DashboardAccessorPermissionViewModel
                {
                    PermissionName = dashboardUserRole.PermissionName,
                    AccessorName = dashboardUserRole.UserId == 0
                        ? dashboardUserRole.Role
                        : await GetGrafanaUsersInAppName(dashboardUserRole.UserId)
                };
                dashboardAccessorsList.Add(dashboardAccessor);
            }

            var model = new DashboardPermissionViewModel
            {
                DashboardTitle = dashboardTitle,
                DashboardAccessors = dashboardAccessorsList
            };
            return PartialView("_DashboardUserPermissions", model);
        }

        public async Task<string> GetGrafanaUsersInAppName(int grafanaUserId)
        {
            var grafanaUser = await _grafanaHelper.GetSingleUserByIdAsync(grafanaUserId);
            var appUser = await _userManager.FindByEmailAsync(grafanaUser.Email);
            string fullName = string.Format("{0} {1}", appUser.FirstName, appUser.LastName);
            return fullName;
        }
    }
}
