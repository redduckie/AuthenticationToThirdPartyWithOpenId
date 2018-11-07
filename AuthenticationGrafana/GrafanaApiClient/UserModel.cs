namespace GrafanaApiClient
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string OrgId { get; set; }
        public string IsGrafanaAdmin { get; set; }
    }
}
