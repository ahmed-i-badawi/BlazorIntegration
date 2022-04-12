using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Net.Http.Json;
using SimpleTouchWorker.Extensions;
using Microsoft.Extensions.Configuration;

namespace SimpleTouchWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public HttpClient _httpClient;
    HubConnection connection = null;
    private string _hubBaseUrl;
    private string _hubUrl;
    private string _username;
    private bool IsConnected => HubConnectionState.Connected == connection?.State;
    public string Status { get; set; }
    public IConfiguration _config { get; }

    public Worker(IHttpClientFactory httpClient, ILogger<Worker> logger, IConfiguration config)
    {
        _httpClient = httpClient.CreateClient();
        _config = config;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_config["API"]);
        _hubBaseUrl = $"{_config["API"]}/{_config["HubName"]}";
    }

    Task MachineIsAdded(string arg)
    {
        _logger.LogInformation($"Worker running at: {DateTimeOffset.Now} - This Machine Added Successfully");
        //connection.StopAsync();
        return Task.CompletedTask;
    }

    Task NewOrder(string arg)
    {
        _logger.LogInformation($"at: {DateTimeOffset.Now} - This Machine Added New Order");
        //connection.StopAsync();
        return Task.CompletedTask;
    }

    private async Task OpenConnectionToServer()
    {
        // start connection and send connId, sysInfo to server;
        // if exist => log in
        // if not => waiting registeration event from server Interface
        // on server register on current systeminfo if machine status pending

        SystemInfo sysInfoObj = new SystemInfo();
        var sysInfo = sysInfoObj.ValueAsync();

        _hubUrl = $"{_hubBaseUrl}?sysInfo={sysInfo}";
        connection = new HubConnectionBuilder().WithUrl(_hubUrl).Build();
        await connection.StartAsync();

        connection.Closed += async (e) =>
        {
            await connection.StartAsync();
        };

        connection.On<string>("MachineIsAdded", this.MachineIsAdded);
        connection.On<string>("NewOrder", this.NewOrder);

        return;
    }

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        await OpenConnectionToServer();
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
