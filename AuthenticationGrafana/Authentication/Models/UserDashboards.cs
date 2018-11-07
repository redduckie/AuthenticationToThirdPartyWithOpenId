using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models
{
    public class UserDashboards
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DashboardName { get; set; }
        public string DashboardUrl { get; set; }

    }
}
