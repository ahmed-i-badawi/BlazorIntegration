using System.Net.Http;
using System.Net.Http.Json;
using WorkerService.Extensions;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public HttpClient _httpClient;
        public string _token;

        public Worker(IHttpClientFactory httpClient, ILogger<Worker> logger)
        {
            _httpClient = httpClient.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7032/");
            _logger = logger;
        }

        private async Task RegisterCurrentMachine()
        {
            SystemInfo sysInfoObj = new SystemInfo(); 
            var sysInfo = sysInfoObj.ValueAsync();

            var myObject = new
            {
                SystemInfo = sysInfo
            };

            var result = _httpClient.PostAsJsonAsync("api/Machine/Login", myObject);
            var finalRes = result.Result.Content.ReadAsStringAsync();
            _token = finalRes.Result;
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            await RegisterCurrentMachine();
            //await base.StartAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }

        public override async void Dispose()
        {

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}