﻿using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api;

public class UserEndpoint : IUserEndpoint
{
    private readonly IAPIHelper _apiHelper;

    public UserEndpoint(IAPIHelper apiHelper)
    {
        _apiHelper = apiHelper;
    }

    public async Task<List<UserModel>> GetAll()
    {
        using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("api/User/Admin/GetAllUsers");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<List<UserModel>>();
            return result;
        }
        else
        {
            throw new(response.ReasonPhrase);
        }
    }

    public async Task CreateUser(CreateUserModel model)
    {
        var data = new {model.FirstName, model.LastName, model.EmailAddress, model.Password};

        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Register", data);
        if (response.IsSuccessStatusCode == false)
        {
            throw new(response.ReasonPhrase);
        }
    }

    public async Task<Dictionary<string, string>> GetAllRoles()
    {
        using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("api/User/Admin/GetAllRoles");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<Dictionary<string, string>>();
            return result;
        }
        else
        {
            throw new(response.ReasonPhrase);
        }
    }

    public async Task AddUserToRole(string userId, string roleName)
    {
        var data = new {userId, roleName};

        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Admin/AddRole", data);
        if (response.IsSuccessStatusCode == false)
        {
            throw new(response.ReasonPhrase);
        }
    }

    public async Task RemoveUserFromRole(string userId, string roleName)
    {
        var data = new { userId, roleName };

        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Admin/RemoveRole", data);
        if (response.IsSuccessStatusCode == false)
        {
            throw new(response.ReasonPhrase);
        }
    }
}