using System;
using System.Collections.Generic;
using System.Text;

namespace GrafanaApiClient
{
    public class DashboardModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

        public string Url { get; set; }
    }
}
