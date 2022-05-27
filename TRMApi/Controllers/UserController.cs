using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;
using ApplicationDbContext = TRMApi.Data.ApplicationDbContext;
using ApplicationUserModel = TRMApi.Models.ApplicationUserModel;
using UserRolePairModel = TRMApi.Models.UserRolePairModel;

namespace TRMApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserData _userData;
    private readonly ILogger _logger;

    public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IUserData userData,
        ILogger<UserController> logger)
    {
        _context = context;
        _userManager = userManager;
        _userData = userData;
        _logger = logger;
    }
        
    [HttpGet]
    public UserModel GetById()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return _userData.GetUserById(userId).First();
    }

    public record UserRegistrationModel(
        string FirstName, 
        string LastName, 
        string EmailAddress, 
        string Password);

    [HttpPost]
    [Route("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserRegistrationModel user)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);
            if (existingUser == null)
            {
                IdentityUser newUser = new()
                {
                    Email = user.EmailAddress,
                    EmailConfirmed = true,
                    UserName = user.EmailAddress
                };

                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);

                    if (existingUser == null)
                    {
                        return BadRequest();
                    }

                    UserModel userModel = new()
                    {
                        Id = existingUser.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        EmailAddress = user.EmailAddress,
                    };
                    _userData.CreateUser(userModel);
                    return Ok();
                }
            }
        }

        return BadRequest();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("Admin/GetAllUsers")]
    public List<ApplicationUserModel> GetAllUsers()
    {
        List<ApplicationUserModel> output = new();

        var users = _context.Users.ToList();
        var userRoles = from ur in _context.UserRoles
            join r in _context.Roles on ur.RoleId equals r.Id
            select new {ur.UserId, ur.RoleId, r.Name};

        foreach (var user in users)
        {
            ApplicationUserModel u = new()
            {
                Id = user.Id,
                Email = user.Email
            };

            u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(x => x.RoleId, x => x.Name);

            output.Add(u);
        }

        return output;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("Admin/GetAllRoles")]
    public Dictionary<string, string> GetAllRoles()
    {
        var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

        return roles;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("Admin/AddRole")]
    public async Task AddRole(UserRolePairModel pairing)
    {
        string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(pairing.UserId);

        _logger.LogInformation("Admin {Admin} added user {User} to role {Role}", loggedInUserId, user.Id,
            pairing.RoleName);

        await _userManager.AddToRoleAsync(user, pairing.RoleName);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("Admin/RemoveRole")]
    public async Task RemoveRole(UserRolePairModel pairing)
    {
        string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(pairing.UserId);

        _logger.LogInformation("Admin {Admin} removed user {User} from role {Role}", loggedInUserId, user.Id,
            pairing.RoleName);

        await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);
    }
}