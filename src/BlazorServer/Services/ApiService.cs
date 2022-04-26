using Microsoft.AspNetCore.Components;

namespace BlazorServer.Services
{
    public class ApiService
    {
        public ApiService(HttpClient httpClient, NavigationManager navigationManager)
        {
            HttpClient = httpClient;
            NavigationManager = navigationManager;
            HttpClient.BaseAddress = new Uri(NavigationManager.BaseUri);
            HubUrl = $"{NavigationManager.BaseUri}MessagingHub";
        }

        public HttpClient HttpClient { get; }
        public NavigationManager NavigationManager { get; }
        public string HubUrl { get; }
    }
}
