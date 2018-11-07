using GrafanaApiClient;
using Authentication.Models.AccountViewModels;
using Authentication.Models.ManageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Interfaces
{
    public interface IGrafanaHelper
    {
        Task<MessageModel<SaveUserModel>> SaveGrafanaUser(RegisterViewModel model);

        Task<MessageModel<SaveUserRoleModel>> UpdateGrafanaUserRoleAsync(UserModel model, string role);

        Task<UserModel> GetUserAsync(string userMail);

        Task<UserModel> GetSingleUserByIdAsync(int UserId);

        Task<List<FolderModel>> GetDashboardFoldersAsync(); 

        Task<List<DashboardModel>> GetDashboardsListsForFolderAsync(int folderId);

        Task<List<DashboardModel>> GetAllDashboardsAsync();

        Task<List<UserDashboardRoleViewModel>> GetUserDashboardsAsync(int userId);

        Task<List<DashboardPermissionsItemModel>> GetDashboardPermissionItemsAsync(int dashboardId);

        Task<DashboardPermissionsSaveModel> GetExistingDashboardPermissionItems(DashboardPermissionsSaveModel model);

        Task<MessageModel<DashboardPermissionsSaveModel>> AddOrUpdateUserDashboardPermissionAsync(DashboardPermissionsSaveModel model);

        Task<MessageModel<DashboardPermissionsSaveModel>> DeleteUserDashboardPermissionAsync(DashboardPermissionsSaveModel model);
    }

}
