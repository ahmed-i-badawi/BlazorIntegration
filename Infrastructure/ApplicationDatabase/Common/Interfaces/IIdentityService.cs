
using SharedLibrary.Models;

namespace Infrastructure.ApplicationDatabase.Common.Interfaces;

public interface IIdentityService
{
    Task<string> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, bool isEmailConfirmed = true, string mail = default);

    Task<Result> DeleteUserAsync(string userId);
    Task<bool> SetUserPasswrod(string userId, string newPassword);
    Task<bool> AddUserToRole(string userId, string role);
}
