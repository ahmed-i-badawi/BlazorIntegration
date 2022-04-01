using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Net.Http.Json;
using WorkerService.Extensions;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public HttpClient _httpClient;
        public string _token="test";

        HubConnection connection = null;
        private string _hubUrl;
        private string _username;
        private bool IsConnected => HubConnectionState.Connected == connection?.State;
        public string Status { get; set; }

        public Worker(IHttpClientFactory httpClient, ILogger<Worker> logger)
        {
            _httpClient = httpClient.CreateClient();
            _httpClient.BaseAddress = new Uri($"https://localhost:7032/");
            _logger = logger;
        }

        Task MachineIsAdded(string arg)
        {
            _logger.LogInformation($"Worker running at: {DateTimeOffset.Now} - This Machine Added Successfully");
            connection.StopAsync();
            return Task.CompletedTask;
        }

        private async Task RegisterCurrentMachine()
        {
            // start connection and send connId, sysInfo to server;
            // if exist => log in
            // if not => waiting registeration event from server Interface
            // on server register on current systeminfo if machine status pending


            SystemInfo sysInfoObj = new SystemInfo();
            var sysInfo = sysInfoObj.ValueAsync();

            _hubUrl = $"https://localhost:7032/MessagingHub?sysInfo={sysInfo}";
            connection = new HubConnectionBuilder().WithUrl(_hubUrl).Build();
            await connection.StartAsync();

            connection.Closed += async (e) =>
            {
                await connection.StartAsync();
            };

            connection.On<string>("MachineIsAdded", this.MachineIsAdded);

            return;
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