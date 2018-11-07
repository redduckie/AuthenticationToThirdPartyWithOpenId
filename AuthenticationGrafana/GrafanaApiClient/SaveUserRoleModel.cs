using System;
using System.Collections.Generic;
using System.Text;

namespace GrafanaApiClient
{
    public class SaveUserRoleModel
    {
        public int UserId { get; set; }

        public string OrgId { get; set; }

        public string Role { get; set; }
    }
}
