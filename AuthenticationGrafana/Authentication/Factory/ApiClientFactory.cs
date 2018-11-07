using GrafanaApiClient;
using Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Authentication.Factory
{
    public static class ApiClientFactory
    {
        private static Uri apiUri;

        private static Lazy<ApiClient> restClient = new Lazy<ApiClient>(
            () => new ApiClient(apiUri, ApplicationSettings.GrafanaAdminUserName, ApplicationSettings.GrafanaAdminUserPass),
            LazyThreadSafetyMode.ExecutionAndPublication);

        static ApiClientFactory()
        {
            apiUri = new Uri(ApplicationSettings.GrafanaApiUrl);
        }

        public static ApiClient Instance
        {
            get
            {
                return restClient.Value;
            }
        }
    }
}
