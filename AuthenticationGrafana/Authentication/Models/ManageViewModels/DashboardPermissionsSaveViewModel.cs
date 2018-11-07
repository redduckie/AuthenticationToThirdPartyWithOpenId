using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models.ManageViewModels
{
    public class DashboardPermissionsSaveViewModel
    {
        public List<DashboardPermissionsSaveItemViewModel> Items { get; set; }
    }

    public class DashboardPermissionsSaveItemViewModel
    {
        public int? UserId { get; set; }
        public string Role { get; set; }
        public string team { get; set; }
        public string Permission { get; set; }
    }
}
