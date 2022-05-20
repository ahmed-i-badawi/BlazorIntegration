using Infrastructure.ApplicationDatabase;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using Infrastructure.ApplicationDatabase.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Entities;

IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettingsApplicationDatabase.json")
                            .Build();

var builder = WebApplication.CreateBuilder(args);

var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");

// Add services to the container.

builder.Services.AddApplicationDatabase(defaultConnectionString);

var app = builder.Build();


var dbApplicationcontext = app.Services.GetRequiredService<ApplicationDbContext>();

dbApplicationcontext.Database.Migrate();

// Configure the HTTP request pipeline.


app.Run();

