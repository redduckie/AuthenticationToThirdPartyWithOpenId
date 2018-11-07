using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models.ManageViewModels
{
    public class ManageViewModel
    {
        public SelectList UserList{ get; set; }
        public ApplicationUser User { get; set; }

        public List<ApplicationUser> Users { get; set; }
    }
}
