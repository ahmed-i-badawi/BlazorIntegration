using Infrastructure.LogDatabase;
using LogDatabase;
using Microsoft.EntityFrameworkCore;

IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettingsLogDatabase.json")
                            .Build();

var builder = WebApplication.CreateBuilder(args);

var logConnectionString = builder.Configuration.GetConnectionString("LogConnection");

// Add services to the container.

builder.Services.AddLogDatabase(logConnectionString);

var app = builder.Build();


var dbLogcontext = app.Services.GetRequiredService<LogDbContext>();

dbLogcontext.Database.Migrate();

// Configure the HTTP request pipeline.


app.Run();

