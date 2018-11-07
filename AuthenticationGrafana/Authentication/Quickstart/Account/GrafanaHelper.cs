using GrafanaApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Authentication.Factory;
using Authentication.Interfaces;
using Authentication.Models;
using Authentication.Models.AccountViewModels;
using Authentication.Models.ManageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Quickstart.Account
{
    public class GrafanaHelper:IGrafanaHelper
    {
        private readonly IOptions<MySettingsModel> _appSettings;
        public GrafanaHelper(IOptions<MySettingsModel> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.GrafanaApiUrl = _appSettings.Value.GrafanaApiBaseUrl;
            ApplicationSettings.GrafanaAdminUserName = _appSettings.Value.GrafanaAdminUserName;
            ApplicationSettings.GrafanaAdminUserPass = _appSettings.Value.GrafanaAdminUserPass;
        }

        public async Task<List<DashboardModel>> GetAllDashboardsAsync()
        {
            return await ApiClientFactory.Instance.GetAllDashboardsAsync();
        }

        public async Task<List<FolderModel>> GetDashboardFoldersAsync()
        {
            var folders = await ApiClientFactory.Instance.GetFolders();
            return folders;
        }

        public async Task<List<DashboardModel>> GetDashboardsListsForFolderAsync(int folderId)
        {
            return await ApiClientFactory.Instance.GetFolderDashboards(folderId);
        }

        public async Task<UserModel> GetUserAsync(string userMail)
        {
            var user = await ApiClientFactory.Instance.GetUser(userMail);
            return user;
        }

        public async Task<List<UserDashboardRoleViewModel>> GetUserDashboardsAsync(int userId)
        {
            var userDashboardRoles =  await ApiClientFactory.Instance.GetAllDashboardPermissionsPerUser(userId);
            return userDashboardRoles.Select(udr => new UserDashboardRoleViewModel
            {
                DashboardName = udr.Title,
                DashboardId = udr.DashboardId,
                UserDashBoardRole = udr.PermissionName,
                UserDashboardRoleId = udr.Permission
            }).ToList();
        }

        public async Task<MessageModel<SaveUserModel>> SaveGrafanaUser(RegisterViewModel model)
        {
            var userModel = new SaveUserModel
            {
                Name = model.Username,
                Email = model.EmailAddress,
                Login = model.Username,
                Password = model.Password
            };
            var response = await ApiClientFactory.Instance.SaveUser(userModel);
            return response;
        }

        public async Task<MessageModel<SaveUserRoleModel>> UpdateGrafanaUserRoleAsync(UserModel model, string role)
        {
            var userOrgModel = new SaveUserRoleModel
            {
                OrgId = model.OrgId,
                UserId = model.Id,
                Role = role
            };
            var response = await ApiClientFactory.Instance.UpdateUserOrganizationRole(userOrgModel);
            return response;
        }

        public async Task<MessageModel<DashboardPermissionsSaveModel>> AddOrUpdateUserDashboardPermissionAsync(DashboardPermissionsSaveModel model)
        {
            var newSaveItem = new DashboardPermissionsItemModel
            {
                Permission = model.RoleId,
                UserId = model.UserId
            };
            var saveModel = await GetExistingDashboardPermissionItems(model);
            var ProvidedUserItem = saveModel.Items.FirstOrDefault(i => i.UserId == newSaveItem.UserId);
            if (ProvidedUserItem != null)
            {
                ProvidedUserItem.Permission = newSaveItem.Permission;
            }
            else
            {
                saveModel.Items.Add(newSaveItem);
            }
            return await ApiClientFactory.Instance.AddUserDashBoardPermission(saveModel);
        }
        public async Task<MessageModel<DashboardPermissionsSaveModel>> DeleteUserDashboardPermissionAsync(DashboardPermissionsSaveModel model)
        {
            var newSaveItem = new DashboardPermissionsItemModel
            {
                Permission = model.RoleId,
                UserId = model.UserId
            };
            var saveModel = await GetExistingDashboardPermissionItems(model);
            saveModel.Items = saveModel.Items.Where(i => i.UserId != newSaveItem.UserId).ToList();
            return await ApiClientFactory.Instance.AddUserDashBoardPermission(saveModel);
        }

        public async Task<DashboardPermissionsSaveModel> GetExistingDashboardPermissionItems(DashboardPermissionsSaveModel model)
        {
            var items = await GetDashboardPermissionItemsAsync(4);
            var newSaveModel = new DashboardPermissionsSaveModel();
            newSaveModel.DashboardId = model.DashboardId;
            newSaveModel.UserId = model.UserId;
            newSaveModel.UserName = model.UserName;
            newSaveModel.Items = new List<DashboardPermissionsItemModel>();
            newSaveModel.Items.AddRange(items);
            return newSaveModel;
        }

        public async Task<List<DashboardPermissionsItemModel>> GetDashboardPermissionItemsAsync(int dashboardId)
        {
            var dashboards = await ApiClientFactory.Instance.GetSingleDashboardPermissions(dashboardId);
            var items = dashboards.Select(x => new DashboardPermissionsItemModel
            {
                Permission = x.Permission,
                PermissionName = x.PermissionName,
                UserId = x.UserId,
                Role = x.Role
            }).ToList();
            return items;
        }

        public async Task<UserModel> GetSingleUserByIdAsync(int userId)
        {
            var user = await ApiClientFactory.Instance.GetSingleUserById(userId);
            return user;
        }
    }
}
