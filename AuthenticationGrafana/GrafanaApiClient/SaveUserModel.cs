using System;
using System.Collections.Generic;
using System.Text;

namespace GrafanaApiClient
{
    public class SaveUserModel
    {
        public string Name { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
