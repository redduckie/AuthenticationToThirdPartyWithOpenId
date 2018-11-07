using System;
using System.Collections.Generic;
using System.Text;

namespace GrafanaApiClient
{
    public class DashboardPermissionsItemModel
    {
        public int UserId { get; set; }
        public int Permission { get; set; }
        public string Role { get; set; }
        public string PermissionName { get; set; }
    }
}

