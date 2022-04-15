using Syncfusion.Blazor.Notifications;

namespace BlazorServer.Services;

public interface IClientOperations
{
    public string Title { get; set; }
    public string Content { get; set; }
    public SfToast ToastObj { get; set; }
    public Task ShowToast(string Title, string Content, string Type = "None");
}

public class ClientOperations : IClientOperations
{
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public SfToast ToastObj { get; set; }


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
