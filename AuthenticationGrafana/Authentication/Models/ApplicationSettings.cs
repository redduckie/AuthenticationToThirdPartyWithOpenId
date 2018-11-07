using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models
{
    public class ApplicationSettings
    {
        public static string GrafanaApiUrl { get; set; }
        public static string GrafanaAdminUserName { get; set; }
        public static string GrafanaAdminUserPass { get; set; }
    }
}
