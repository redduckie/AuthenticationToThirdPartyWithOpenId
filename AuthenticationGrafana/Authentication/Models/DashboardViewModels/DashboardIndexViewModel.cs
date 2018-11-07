using GrafanaApiClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models.DashboardViewModels
{
    public class DashboardIndexViewModel
    {
        [Display(Name = "Dashboards")]
        public string SelectedDashboard { get; set; }
        public SelectList DashboardSelectList { get; set; }
        public List<DashboardModel> DashboardsList{ get; set; }

    }
}
