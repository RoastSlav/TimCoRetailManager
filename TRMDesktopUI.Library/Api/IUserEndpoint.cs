﻿using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api;

public interface IUserEndpoint
{
    Task<List<UserModel>> GetAll();

    Task<Dictionary<string, string>> GetAllRoles();

    Task AddUserToRole(string userId, string roleName);

    Task RemoveUserFromRole(string userId, string roleName);
    Task CreateUser(CreateUserModel model);
}