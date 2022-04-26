using SharedLibrary.Dto;
using Syncfusion.Blazor.Notifications;
using System.Security.Claims;

namespace BlazorServer.Services;

public interface IClientOperations
{
    public string Title { get; set; }
    public string Content { get; set; }
    public SfToast ToastObj { get; set; }
    public Task ShowToast(string Title, string Content, string Type = "None");
    Task<UserDataDto> GetLoggedInUser();
    Task SetLoggedInUser(ClaimsPrincipal claimsPrincipal);

}

public class ClientOperations : IClientOperations
{
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public SfToast ToastObj { get; set; }
    private UserDataDto LoggedInUser { get; set; }


    public async Task SetLoggedInUser(ClaimsPrincipal claimsPrincipal)
    {
        var identity = (ClaimsIdentity)claimsPrincipal.Identity;

        LoggedInUser = new UserDataDto()
        {
            UserId = claimsPrincipal.Claims?.FirstOrDefault(e => e.Type.Contains("identity/claims/nameidentifier"))?.Value,
            UserName = claimsPrincipal.Claims?.FirstOrDefault(e => e.Type.Contains("identity/claims/name"))?.Value,
            FullName = claimsPrincipal.Claims?.FirstOrDefault(e => e.Type.Contains("identity/claims/name"))?.Value,
            Mail = claimsPrincipal.Claims?.FirstOrDefault(e => e.Type.Contains("identity/claims/emailaddress"))?.Value,
            Roles = claimsPrincipal.Claims?.FirstOrDefault(e => e.Type.Contains("identity/claims/role"))?.Value
        };
    }
    public async Task<UserDataDto> GetLoggedInUser()
    {
        return LoggedInUser;
    }

    public async Task ShowToast(string Title, string Content, string Type = "None")
    {
        ToastModel tModel = tModel = new ToastModel 
        { 
            Title = Title,
            Content = Content,
            Target = "#Control-Region",
            ShowProgressBar = true,
            Timeout = 5000,
            Icon = "e-meeting",
        };

        switch (Type)
        {
            case "warning":
                tModel.CssClass = "e-toast-warning"; tModel.Icon = "e-warning toast-icons";
                break;

            case "success":
                tModel.CssClass = "e-toast-success"; tModel.Icon = "e-success toast-icons";
                break;

            case "error":
                tModel.CssClass = "e-toast-danger"; tModel.Icon = "e-danger toast-icons";
                break;

            case "info":
                tModel.CssClass = "e-toast-warniinfong"; tModel.Icon = "e-info toast-icons";
                break;

            default:
                break;
        }

        await ToastObj.Show(tModel);

    }

}
