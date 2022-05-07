using Infrastructure.ApplicationDatabase.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models;
using SharedLibrary.Entities;

namespace Infrastructure.ApplicationDatabase.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    public async Task<string> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }

    public async Task<bool> IsUserNameOrMailExist(string userName, string mail)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName || u.Email == mail);

        return user != null;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, bool isEmailConfirmed = true, string mail = default)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = !string.IsNullOrWhiteSpace(mail) ? userName + "@mail.com" : mail,
            EmailConfirmed = isEmailConfirmed
        };
        try
        {
            //var result = await _userManager.CreateAsync(user, password);
            var result = _userManager.CreateAsync(user, password);

            return (result.Result.ToApplicationResult(), user.Id);
            //return (result.ToApplicationResult(), user.Id);
        }
        catch (Exception ex)
        {

            throw;
        }

    }

    public async Task<bool> AddUserToRole(string userId, string role)
    {

        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }
        var result = await _userManager.AddToRolesAsync(user, new[] { role });
        return result.Succeeded;
    }


    public async Task<bool> SetUserPasswrod(string userId, string newPassword)
    {

        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var result = await _userManager.AddPasswordAsync(user, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
}
