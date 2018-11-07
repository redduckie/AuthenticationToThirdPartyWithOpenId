// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using GrafanaApiClient;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Authentication.Data;
using Authentication.Factory;
using Authentication.Interfaces;
using Authentication.Models;
using Authentication.Models.AccountViewModels;
using Authentication.Models.ManageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    public class AccountController : Controller
    {
        #region local variables
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IGrafanaHelper _grafHelper;
        private readonly ApplicationDbContext _context;
        private readonly IGrafRole _grafRole;
        #endregion

        #region constructor
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IOptions<MySettingsModel> appSettings,
            IGrafanaHelper grafHelper,
            ApplicationDbContext context,
            IGrafRole grafRole)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _grafHelper = grafHelper;
            _context = context;
            _grafRole = grafRole;
        }
        #endregion

        #region login
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (await _clientStore.IsPkceClientAsync(context.ClientId))
                    {
                        // if the client is PKCE then we assume it's native, so this change in how to
                        // return the response is for better UX for the end user.
                        return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

                    if (context != null)
                    {
                        if (await _clientStore.IsPkceClientAsync(context.ClientId))
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));
                ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }
        #endregion

        #region logout
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        public IActionResult RemoveGrafCookie()
        {
            foreach (var cookieKey in Request.Cookies.Keys)
            {
                if (cookieKey == "grafana_sess")
                    Response.Cookies.Delete(cookieKey);
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                //so that when logged out of the app also grafana auth cookie will be removed hence logging out of grafana.
                RemoveGrafCookie();
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            return View("LoggedOut", vm);
        }
        #endregion

        #region register
        [Authorize(Roles = "SecurityAdmin")]
        public ActionResult Register()
        {
            var roles = _roleManager.Roles.Select(role => new { role.Id, role.Name }).ToList();
            var grafRoles = _grafRole.GetGrafRoles();
            RegisterViewModel model = new RegisterViewModel();
            model.ApplicationRoles = new SelectList(roles, "Name", "Name");
            model.GrafRoles = new SelectList(grafRoles, "RoleName", "RoleName");
            return View(model);
            //return PartialView("_RegisterPartial",model);
        }

        [HttpPost]
        [Authorize(Roles = "SecurityAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.EmailAddress,
                    GrafRoleName = model.GrafRole.RoleName,
                    FirstName =model.FirstName,
                    LastName = model.LastName
                };
                var identityResult = await _userManager.CreateAsync(user, model.Password);
                var roleResult = await _userManager.AddToRoleAsync(user, model.AppUserRole.Name);

                if (identityResult.Succeeded && roleResult.Succeeded)
                {

                    var grafanaUser = await _grafHelper.GetUserAsync(model.EmailAddress);
                    //if the grafana user exists but there is no application user. update the role and the password of the grafana user
                    if (grafanaUser.Email != null)
                    {
                        //todo update the password and role of the user
                        var grafanaUpdateRoleResponse = await _grafHelper.UpdateGrafanaUserRoleAsync(grafanaUser, model.GrafRole.RoleName);
                    }

                    else
                    {
                        //save the user to grafana also. 
                        var grafanaSaveUserResponse = await _grafHelper.SaveGrafanaUser(model);
                        //get saved user info
                        grafanaUser = await _grafHelper.GetUserAsync(model.EmailAddress);
                        //update the grafanauser organization roles. 
                        var grafanaUpdateRoleResponse = await _grafHelper.UpdateGrafanaUserRoleAsync(grafanaUser, model.GrafRole.RoleName);
                        //AddGrafanaErrors(grafanaResponse);
                    }

                    var claimResult = await _userManager.AddClaimsAsync(user,
                        new List<Claim>
                        {
                            new Claim("email", user.Email),
                            new Claim("login", user.UserName),
                            new Claim("name", user.UserName),
                            new Claim("FullName", string.Format("{0} {1}",user.FirstName, user.LastName))
                        });
                    //if (claimResult.Succeeded)
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //}
                    return RedirectToAction("Manage", "Account");
                }
                AddErrors(identityResult);
            }

            var roles = _roleManager.Roles.Select(role => new { role.Id, role.Name }).ToList();
            model.ApplicationRoles = new SelectList(roles, "Name", "Name");
            return View(model);
            //return PartialView("_RegisterPartial",model);
        }
        #endregion

        #region Manage
        [Authorize(Roles = "SecurityAdmin")]
        public ActionResult Manage()
        {
            ManageViewModel model = new ManageViewModel();
            var users = _userManager.Users.ToList();
            model.Users = users;
            return View(model);
        }

        [Authorize(Roles = "SecurityAdmin")]
        public async Task<ActionResult> UserRolesAsync(string userName)
        {
            var roles = _roleManager.Roles.ToList();
            var user = await _userManager.FindByNameAsync(userName);
            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            var grafanaUser = await _grafHelper.GetUserAsync(user.Email);
            var grafRoles = _grafRole.GetGrafRoles();

            var model = new RoleInputViewModel();
            model.RolesList = new SelectList(roles, "Name", "Name");
            model.UserRoles = userRoles;
            model.UserName = userName;
            model.GrafanaRoles = new SelectList(grafRoles, "RoleName", "RoleName");
            model.UserGrafanaRole = user.GrafRoleName;
            return PartialView("_UserRolesPartial", model);
        }

        [HttpPost]
        [Authorize(Roles = "SecurityAdmin")]
        public async Task<IActionResult> AddUserRoleAsync([FromBody]DeleteUserRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var roleResult = await _userManager.AddToRoleAsync(user, model.RoleName);
                if (!roleResult.Succeeded)
                {
                    AddErrors(roleResult);
                }
            }
            //return PartialView("_UserRolesPartial",roleInputViewModel);
            return RedirectToAction("Manage", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "SecurityAdmin")]
        public async Task<IActionResult> RemoveUserRoleAsync([FromBody]DeleteUserRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var roleResult = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                if (!roleResult.Succeeded)
                {
                    AddErrors(roleResult);
                }
            }
            return RedirectToAction("Manage", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "SecurityAdmin")]
        public async Task<IActionResult> UpdateUserDashboardRole([FromBody]RoleInputViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var grafanaUser = await _grafHelper.GetUserAsync(user.Email);
                await _grafRole.UpdateInAppGrafRoleAsync(model.UserName, model.UserGrafanaRole);
                var grafanaUpdateRoleResponse = await _grafHelper.UpdateGrafanaUserRoleAsync(grafanaUser, model.UserGrafanaRole);
                AddGrafanaErrors(grafanaUpdateRoleResponse);
            }
            return RedirectToAction("Manage", "Account");
        }
        #endregion

        #region DashBoard
        [Authorize(Roles = "SecurityAdmin")]
        public async Task<ActionResult> UserDashboardRolesAsync(string userName)
        {

            var model = new DashboardPermissionsViewModel();
            var user = await _userManager.FindByNameAsync(userName);
            var grafanaUser = await _grafHelper.GetUserAsync(user.Email);
            var grafRoles = _grafRole.GetGrafRoles();
            var folders = await _grafHelper.GetDashboardFoldersAsync();
            var allDashboards = await _grafHelper.GetAllDashboardsAsync();
            var userDashBoardPermissions = await _grafHelper.GetUserDashboardsAsync(grafanaUser.Id);

            model.UserName = userName;
            model.Folders = new SelectList(folders, "Id", "Title");
            model.DashboardNames = new SelectList(allDashboards, "Id", "Title");
            model.DashboardRoles = new SelectList(grafRoles.ToList(), "RoleCode", "RoleName");
            model.UserDashboardRoles = userDashBoardPermissions; 
            return PartialView("_DashboardPermissionsPartial", model);
        }

        [HttpPost]
        [Authorize(Roles ="SecurityAdmin")]
        public async Task<ActionResult> AddUserToDashboardPermissions([FromBody] DashboardPermissionsSaveModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var grafanaUser = await _grafHelper.GetUserAsync(user.Email);
                model.UserId = grafanaUser.Id;
                model.DashboardId = model.DashboardId;
                model.RoleId = model.RoleId;
                await _grafHelper.AddOrUpdateUserDashboardPermissionAsync(model);
            }
            return RedirectToAction("Manage", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "SecurityAdmin")]
        public async Task<ActionResult> DeleteUserDashboardPermissions([FromBody] DashboardPermissionsSaveModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var grafanaUser = await _grafHelper.GetUserAsync(user.Email);
                model.UserId = grafanaUser.Id;
                model.DashboardId = model.DashboardId;
                model.RoleId = model.RoleId;
                await _grafHelper.DeleteUserDashboardPermissionAsync(model);
            }
            return RedirectToAction("Manage", "Account");
        }
        #endregion

        #region show Errors
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description.ToString());
            }
        }
        private void AddGrafanaErrors<T>(MessageModel<T> result)
        {
            if (result.Error != null)
                ModelState.AddModelError("", result.Error);
        }
        #endregion

        #region helper APIs for the AccountController
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                // this is meant to short circuit the UI and only trigger the one external IdP
                return new LoginViewModel
                {
                    EnableLocalLogin = false,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint
                };
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }
                    }
                }
            }

            return vm;
        }
        #endregion
    }
}