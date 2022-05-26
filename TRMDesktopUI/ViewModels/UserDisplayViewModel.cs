using System.ComponentModel;
using System.Dynamic;
using System.Windows;
using Caliburn.Micro;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels;

public class UserDisplayViewModel : Screen
{
    private readonly StatusInfoViewModel _status;
    private readonly IWindowManager _window;
    private readonly IUserEndpoint _userEndpoint;

    BindingList<UserModel> _users;

    public BindingList<UserModel> Users
    {
        get { return _users; }
        set
        {
            _users = value;
            NotifyOfPropertyChange(() => Users);
        }
    }

    private UserModel _selectedUser;

    public UserModel SelectedUser
    {
        get { return _selectedUser; }
        set
        {
            _selectedUser = value;
            SelectedUserName = value.Email;
            UserRoles.Clear();
            UserRoles = new(value.Roles.Select(x => x.Value).ToList());
            //TODO - Pull up this to a function.
            LoadRoles();
            NotifyOfPropertyChange(() => SelectedUser);
            NotifyOfPropertyChange(() => CanAddSelectedRole);
        }
    }

    private string _selectedUserRole;

    public string SelectedUserRole
    {
        get { return _selectedUserRole; }
        set
        {
            _selectedUserRole = value;
            NotifyOfPropertyChange(() => SelectedUserRole);
            NotifyOfPropertyChange(() => CanRemoveSelectedRole);
        }
    }

    private string _selectedAvailableRole;

    public string SelectedAvailableRole
    {
        get { return _selectedAvailableRole; }
        set
        {
            _selectedAvailableRole = value;
            NotifyOfPropertyChange(() => SelectedAvailableRole);
            NotifyOfPropertyChange(() => CanAddSelectedRole);
        }
    }


    private string _selectedUserName;

    public string SelectedUserName
    {
        get { return _selectedUserName; }
        set
        {
            _selectedUserName = value;
            NotifyOfPropertyChange(() => SelectedUserName);
        }
    }

    private BindingList<string> _userRoles = new();

    public BindingList<string> UserRoles
    {
        get { return _userRoles; }
        set
        {
            _userRoles = value;
            NotifyOfPropertyChange(() => UserRoles);
        }
    }

    private BindingList<string> _availableRoles = new();

    public BindingList<string> AvailableRoles
    {
        get { return _availableRoles; }
        set
        {
            _availableRoles = value;
            NotifyOfPropertyChange(() => AvailableRoles);
        }
    }

    public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IUserEndpoint userEndpoint)
    {
        _status = status;
        _window = window;
        _userEndpoint = userEndpoint;
    }

    protected override async void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        try
        {
            await LoadUsers();
        }
        catch (Exception ex)
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.ResizeMode = ResizeMode.NoResize;
            settings.Title = "System Error";

            if (ex.Message == "Unauthorized")
            {
                _status.UpdateMessage("Unauthorized Access",
                    "You do not have permission to interact with the Users Form.");
                await _window.ShowDialogAsync(_status, null, settings);
            }
            else
            {
                _status.UpdateMessage("Fatal Exception", ex.Message);
                await _window.ShowDialogAsync(_status, null, settings);
            }

            await TryCloseAsync();
        }
    }

    private async Task LoadUsers()
    {
        var userList = await _userEndpoint.GetAll();
        Users = new(userList);
    }

    private async Task LoadRoles()
    {
        var roles = await _userEndpoint.GetAllRoles();

        AvailableRoles.Clear();

        foreach (var role in roles)
        {
            if (UserRoles.IndexOf(role.Value) < 0)
            {
                AvailableRoles.Add(role.Value);
            }
        }
    }

    public bool CanAddSelectedRole
    {
        get
        {
            if (SelectedUser is null || SelectedAvailableRole is null)
            {
                return false;
            }
            else
            {
                return true;    
            }
        }
    }

    public async Task AddSelectedRole()
    {
        await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);

        UserRoles.Add(SelectedAvailableRole);
        AvailableRoles.Remove(SelectedAvailableRole);
    }

    public bool CanRemoveSelectedRole
    {
        get
        {
            if (SelectedUser is null || SelectedUserRole is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public async Task RemoveSelectedRole()
    {
        await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);

        AvailableRoles.Add(SelectedUserRole);
        UserRoles.Remove(SelectedUserRole);
    }
}