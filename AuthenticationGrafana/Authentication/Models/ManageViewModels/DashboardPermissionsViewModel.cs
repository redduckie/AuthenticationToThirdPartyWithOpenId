using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models.ManageViewModels
{
    public class DashboardPermissionsViewModel
    {
        public string UserName { get; set; }

        public string SelectedFolder { get; set; }

        public SelectList Folders { get; set; }

        [Display(Name ="Dashboard Title")]
        public string SelectedDashboardName { get; set; }

        public SelectList DashboardNames { get; set; }

        [Display(Name = "Dashboard Role")]
        public string UserRole { get; set; }

        public SelectList DashboardRoles { get; set; }

        public List<UserDashboardRoleViewModel> UserDashboardRoles { get; set; }
    }
}
