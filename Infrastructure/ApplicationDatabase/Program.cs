using Infrastructure.ApplicationDatabase;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using Infrastructure.ApplicationDatabase.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Entities;

IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

var builder = WebApplication.CreateBuilder(args);

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.

builder.Services.AddApplicationDatabase(defaultConnectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.


app.Run();

