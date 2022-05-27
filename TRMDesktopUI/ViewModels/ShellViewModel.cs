using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels;

public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
{
    private readonly IEventAggregator _events;
    private readonly ILoggedInUserModel _user;
    private readonly IAPIHelper _apiHelper;

    public ShellViewModel(IEventAggregator events, ILoggedInUserModel user, IAPIHelper apiHelper)
    {
        _events = events;
        _user = user;
        _apiHelper = apiHelper;

        _events.SubscribeOnPublishedThread(this);

        ActivateItemAsync(IoC.Get<LoginViewModel>(), new());
    }

    public bool IsLoggedIn
    {
        get
        {
            bool output = false;

            if (string.IsNullOrWhiteSpace(_user.Token) == false)
            {
                output = true;
            }

            return output;
        }
    }

    public bool IsLoggedOut
    {
        get
        {
            return !IsLoggedIn;
        }
    }
    public async Task ExitApplication()
    {
        await TryCloseAsync();
    }

    public async Task UserManagement()
    {
        await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(), new());
    }

    public async Task LogIn()
    {
        await ActivateItemAsync(IoC.Get<LoginViewModel>(), new());
    }

    public async Task LogOut()
    {
        _user.ResetUserModel();
        _apiHelper.LogOffUser();
        await ActivateItemAsync(IoC.Get<LoginViewModel>(), new());
        NotifyOfPropertyChange(() => IsLoggedIn);
        NotifyOfPropertyChange(() => IsLoggedOut);
    }

    public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
    {
        await ActivateItemAsync(IoC.Get<SalesViewModel>(), cancellationToken);
        NotifyOfPropertyChange(() => IsLoggedIn);
        NotifyOfPropertyChange(() => IsLoggedOut);
    }
}