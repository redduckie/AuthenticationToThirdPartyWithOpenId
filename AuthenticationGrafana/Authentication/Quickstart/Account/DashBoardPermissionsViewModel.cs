using Microsoft.AspNetCore.Mvc.Rendering;
using Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Quickstart.Account
{
    public class DashBoardPermissionsViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public SelectList ListOfDashboards{ get; set; }
        public string selectedDashboard { get; set; }
        public string DashBoardPermission { get; set; }

    }
}
