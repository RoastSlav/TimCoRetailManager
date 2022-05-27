using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api;

public interface IAPIHelper
{
    Task<AuthenticatedUser> Authenticate(string username, string password);
    HttpClient ApiClient { get; }

    Task GetLoggedInUserInfo(string token);

    void LogOffUser();
}