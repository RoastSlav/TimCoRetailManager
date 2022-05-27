using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient _apiClient;
        private readonly ILoggedInUserModel _loggedInUserModel;
        private readonly IConfiguration _config;


        public APIHelper(ILoggedInUserModel loggedInUserModel, IConfiguration config)
        {
            _config = config;
            _loggedInUserModel = loggedInUserModel;
            InitializeClient();
        }

        public HttpClient ApiClient
        { 
            get { return _apiClient; }
        }

        private void InitializeClient()
        {
            string api =  _config.GetValue<string>("api");

            _apiClient = new();
            _apiClient.BaseAddress = new(api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
        }

        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            using HttpResponseMessage response = await _apiClient.PostAsync("/Token", data);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                return result;
            }
            else
            {
                throw new(response.ReasonPhrase);
            }
        }

        public void LogOffUser()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }

        public async Task GetLoggedInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token }");

            using HttpResponseMessage response = await _apiClient.GetAsync("/api/User");
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
                _loggedInUserModel.CreatedDate = result.CreatedDate;
                _loggedInUserModel.EmailAddress = result.EmailAddress;
                _loggedInUserModel.FirstName = result.FirstName;
                _loggedInUserModel.LastName = result.LastName;
                _loggedInUserModel.Id = result.Id;
                _loggedInUserModel.Token = token;
            }
            else
            {
                throw new(response.ReasonPhrase);
            }
        }
    }
}
