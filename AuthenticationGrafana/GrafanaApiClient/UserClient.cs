using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaApiClient
{
    public partial class ApiClient
    {

        public async Task<UserModel> GetUser(string userLogin)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "users/lookup"), "?loginOrEmail=" + userLogin);
            var user = await GetAsync<UserModel>(requestUrl);
            return user;
        }

        public async Task<UserModel> GetSingleUserById(int userID)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "/api/users/{0}", userID));
            var user = await GetAsync<UserModel>(requestUrl);
            return user;
        }

        public async Task<MessageModel<SaveUserModel>> SaveUser(SaveUserModel model)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "admin/users"));
            return await PostAsync<SaveUserModel>(requestUrl, model);
        }

        public async Task<MessageModel<SaveUserRoleModel>> UpdateUserOrganizationRole(SaveUserRoleModel model)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "orgs/" + model.OrgId + "/users/" + model.UserId));
            return await PatchAsync<SaveUserRoleModel>(requestUrl, model);
        }

        public async Task<List<FolderModel>> GetFolders()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "folders"));
            return await GetAsync<List<FolderModel>>(requestUrl);
        }

        public async Task<List<DashboardModel>> GetFolderDashboards(int folderId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "search"), "?folderIds=" + folderId);
            var dashboards = await GetAsync<List<DashboardModel>>(requestUrl);
            return dashboards.Where(d=>d.Type =="dash-db").ToList();
        }

        public async Task<List<DashboardModel>> GetAllDashboardsAsync()
        {
            var dashBoards = new List<DashboardModel>();
            var folders = await GetFolders();
            folders.Add(new FolderModel { Id = 0, Title = "General" });
            foreach(var folder in folders)
            {
                 dashBoards.AddRange(await GetFolderDashboards(folder.Id));
            }
            return dashBoards;
        }

        public async Task<List<DashboardPermissionModel>> GetSingleDashboardPermissions(int dashboardId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
               "dashboards/id/{0}/permissions", dashboardId));
            return await GetAsync<List<DashboardPermissionModel>>(requestUrl);
        }

        public async Task<List<DashboardPermissionModel>> GetAllDashboardPermissionsPerUser(int userId)
        {
            var dashboards = await GetAllDashboardsAsync();
            List<DashboardPermissionModel> userDashboards = new List<DashboardPermissionModel>();
            foreach(var dashboard in dashboards)
            {
                var dashboardPermissions = await GetSingleDashboardPermissions(dashboard.Id);
                var userDashboardPermissions = dashboardPermissions.FirstOrDefault(d => d.UserId == userId);
                if(userDashboardPermissions != null)
                    userDashboards.Add(userDashboardPermissions);
            }
            return userDashboards;
        }

        public async Task<MessageModel<DashboardPermissionsSaveModel>> AddUserDashBoardPermission(DashboardPermissionsSaveModel model)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "dashboards/id/{0}/permissions", model.DashboardId));
            var jsonFormat = JsonConvert.SerializeObject(model);
            return await PostAsync<DashboardPermissionsSaveModel>(requestUrl, model);
        }

    }
}
