using Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Interfaces
{
    public interface IGrafRole
    {
        List<GrafRole> GetGrafRoles();

        Task UpdateInAppGrafRoleAsync(string userName, string RoleName);
    }
}
