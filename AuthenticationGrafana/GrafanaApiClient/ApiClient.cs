using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaApiClient
{
    public partial class ApiClient
    {
        private readonly HttpClient _httpClient;
        private string _grafanaAdminUserName;
        private string _grafanaAdminPassword;
        private Uri BaseEndpoint { get; set; }

        public ApiClient(Uri baseEndpoint, string GrafanaAdminUserName, string GrafanaAdminPassword)
        {
            if (baseEndpoint == null)
            {
                throw new ArgumentNullException("baseEndpoint");
            }
            BaseEndpoint = baseEndpoint;
            _httpClient = new HttpClient();
            _grafanaAdminUserName = GrafanaAdminUserName;
            _grafanaAdminPassword = GrafanaAdminPassword;
        }

        public async Task<T> GetAsync<T>(Uri requestUrl)
        {
            addHeaders();
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var response = await _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
            //response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(data);
        }

        public async Task<MessageModel<T>> PostAsync<T>(Uri requestUrl, T content)
        {
            addHeaders();
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var response = await _httpClient.PostAsync(requestUrl.ToString(), CreateHttpContent<T>(content));
            //response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MessageModel<T>>(data);
        }

        private async Task<MessageModel<T1>> PostAsync<T1, T2>(Uri requestUrl, T2 content)
        {
            addHeaders();
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var response = await _httpClient.PostAsync(requestUrl.ToString(), CreateHttpContent<T2>(content));
            //response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MessageModel<T1>>(data);
        }


        public async Task<MessageModel<T>> PatchAsync<T>(Uri requestUrl, T content)
        {
            addHeaders();
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var response = await _httpClient.PatchAsync(requestUrl, CreateHttpContent<T>(content));
            //response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MessageModel<T>>(data);
        }

        public Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
            uriBuilder.Query = queryString;
            return uriBuilder.Uri;
        }

        private HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static JsonSerializerSettings MicrosoftDateFormatSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
            }
        }

        private void addHeaders()
        {
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_grafanaAdminUserName + ":" + _grafanaAdminPassword));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", encoded);
        }
    }


}
