using System.Net.Http;
using System.Net.Http.Json;
using WorkerService.Extensions;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public HttpClient _httpClient;

        public Worker(IHttpClientFactory httpClient, ILogger<Worker> logger)
        {
            _httpClient = httpClient.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7032/");
            _logger = logger;
        }

        private async Task RegisterCurrentMachine()
        {
            SystemGuid systemGuid = new SystemGuid();
            var fingerPrint = systemGuid.ValueAsync();

            var myObject = new
            {
                FingerPrint = fingerPrint
            };

            var result = _httpClient.PostAsJsonAsync("api/MachineRegistration/SubmitMachine", myObject);
            var finalRes = result.Result.Content.ReadAsStringAsync();
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