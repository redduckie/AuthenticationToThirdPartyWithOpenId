using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models
{
    public class MySettingsModel
    {
        public string GrafanaApiBaseUrl { get; set; }
        public string GrafanaAdminUserName { get; set; }
        public string GrafanaAdminUserPass { get; set; }
    }
}
