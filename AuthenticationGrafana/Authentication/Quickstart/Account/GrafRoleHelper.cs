using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Authentication.Data;
using Authentication.Interfaces;
using Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Quickstart.Account
{
    public class GrafRoleHelper : IGrafRole
    {
        ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;
        public GrafRoleHelper(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public List<GrafRole> GetGrafRoles()
        {
            return _context.GrafRole.ToList();
        }

        public async Task UpdateInAppGrafRoleAsync(string userName, string grafRoleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            user.GrafRoleName = grafRoleName;
            await _userManager.UpdateAsync(user);
        }
    }
}
