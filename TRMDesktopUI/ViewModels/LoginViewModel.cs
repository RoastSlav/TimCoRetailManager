using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;

namespace TRMDesktopUI.ViewModels;

public class LoginViewModel : Screen
{
    private string _userName = "raikov0411@gmail.com";
    private string _password = "Psw12!";
    private readonly IAPIHelper _apiHelper;
    private string _errorMessage;
    private readonly IEventAggregator _events;

    public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
    {
        _apiHelper = apiHelper;
        _events = events;
    }
    public string UserName
    {
        get { return _userName; }
        set 
        {
            _userName = value;
            NotifyOfPropertyChange(() => UserName);
            NotifyOfPropertyChange(() => CanLogIn);
        }
    }

    public string Password
    {
        get { return _password; }
        set
        {
            _password = value;
            NotifyOfPropertyChange(() => Password);
            NotifyOfPropertyChange(() => CanLogIn);
        }
    }

    public bool IsErrorVisible
    {
        get 
        {
            bool output = false;
            if(ErrorMessage?.Length > 0)
            {
                output = true;
            }
            return output; 
        }        
    }   

    public string ErrorMessage
    {
        get { return _errorMessage; }
        set 
        { 
            _errorMessage = value; 
            NotifyOfPropertyChange(() => ErrorMessage);
            NotifyOfPropertyChange(() => IsErrorVisible);
        }
    }


    public bool CanLogIn
    {
        get
        {
            bool output = false;

            if (UserName?.Length > 0 && Password?.Length > 0)
            {
                output = true;
            }

            return output;
        }
    }

    public async Task LogIn()
    {
        try
        {
            ErrorMessage = "";
            var result = await _apiHelper.Authenticate(UserName, Password);

            //Capture more information about the user
            await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

            await _events.PublishOnUIThreadAsync(new LogOnEvent(), new());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}