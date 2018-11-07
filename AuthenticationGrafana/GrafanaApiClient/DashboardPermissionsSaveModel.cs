using System;
using System.Collections.Generic;
using System.Text;

namespace GrafanaApiClient
{
    public class DashboardPermissionsSaveModel
    {
        public int DashboardId { get; set; }
        public int RoleId { get; set; }
        public List<DashboardPermissionsItemModel> Items { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
    }
}
