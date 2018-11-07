using Microsoft.AspNetCore.Mvc.Rendering;

namespace Authentication.Models.ManageViewModels
{
    public class UserDashboardRoleViewModel
    {
        public string DashboardName { get; set; }
        public string UserDashBoardRole { get; set; }
        public int DashboardId { get; internal set; }
        public int UserDashboardRoleId { get; internal set; }
    }
}