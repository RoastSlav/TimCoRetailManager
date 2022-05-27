using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TRMApi.Data;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TRMApi.Controllers;

public class TokenController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public TokenController(ApplicationDbContext context, UserManager<IdentityUser> userManager,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    [Route("/token")]
    [HttpPost]
    public async Task<IActionResult> Create(string username, string password)
    {
        if (await IsValidUsernameAndPassword(username, password))
        {
            return new ObjectResult(await GenerateToken(username));
        }
        else
        {
            return BadRequest();
        }
    }

    private async Task<bool> IsValidUsernameAndPassword(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        return await _userManager.CheckPasswordAsync(user, password);

    }

    private async Task<dynamic> GenerateToken(string username)
    {
        var user = await _userManager.FindByEmailAsync(username);
        var roles = from ur in _context.UserRoles
            join r in _context.Roles on ur.RoleId equals r.Id
            where ur.UserId == user.Id
            select new { ur.UserId, ur.RoleId, r.Name };

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.Exp,
                new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
        };

        string key = _configuration.GetValue<string>("SecurityKey");

        foreach (var role in roles)
        {
            claims.Add(new(ClaimTypes.Role, role.Name));
        }

        var token = new JwtSecurityToken(
            new(
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256)),
            new(claims));

        var output = new
        {
            Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserName = username
        };

        return output;
    }
}