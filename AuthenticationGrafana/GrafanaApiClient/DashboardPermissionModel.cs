namespace GrafanaApiClient
{
    public class DashboardPermissionModel
    {
        public int DashboardId { get; set; }
        public int UserId { get; set; }
        public string UserLogin { get; set; }
        public string UserEmail { get; set; }
        public int TeamId  { get; set; }
        public string TeamName { get; set; }
        public string Role { get; set; }
        public int Permission { get; set; }
        public string PermissionName { get; set; }
        public string Title { get; set; }
        public AccessorType AccessorType {
            get
            {
                return UserId != 0 ? AccessorType.User : TeamId != 0 ? AccessorType.Team : !string.IsNullOrEmpty(Role) ? AccessorType.Role : AccessorType.NotAvailable;
            }
        }

    }
}
