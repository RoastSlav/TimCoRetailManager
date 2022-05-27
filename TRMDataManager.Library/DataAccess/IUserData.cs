using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess;

public interface IUserData
{
    List<UserModel> GetUserById(string Id);
    void CreateUser(UserModel user);
}