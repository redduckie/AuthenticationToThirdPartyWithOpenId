using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models.ManageViewModels
{
    public class RoleInputViewModel
    {
        [Display(Name = "Application Roles")]
        public SelectList RolesList { get; set; }

        [Display(Name = "User Roles")]
        public IList<string> UserRoles { get; set; }

        [Display(Name = "User Roles")]
        public IdentityRole IdentityRole { get; set; }

        public string UserName { get; set; }

        [Display(Name ="User Dashboard Role")]
        public string UserGrafanaRole { get; set; }

        public SelectList GrafanaRoles { get; set; }

    }
}
