using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models.DashboardViewModels
{
    public class DashboardPermissionViewModel
    {
        public int DashboardId { get; set; }
        public string DashboardTitle { get; set; }
        public List<DashboardAccessorPermissionViewModel> DashboardAccessors { get; set; }
    }
}
