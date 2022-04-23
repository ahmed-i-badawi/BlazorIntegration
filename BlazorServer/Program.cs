using BlazorServer.Areas.Identity;
using BlazorServer.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using System.Globalization;
using BlazorServer.Shared;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Localization;
using Syncfusion.Licensing;
using BlazorServer.Services;
using System.Reflection;
using FluentValidation;
using BlazorServer;
using SharedLibrary;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using LogDatabase;
using Infrastructure.ApplicationDatabase;
using SharedLibrary.Entities;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using Infrastructure.LogDatabase.Common.Interfaces;
using Infrastructure.LogDatabase;
using System.Security.Principal;

IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

if (File.Exists(System.IO.Directory.GetCurrentDirectory() + "/SyncfusionLicense.txt"))
{
    string licenseKey = System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "/SyncfusionLicense.txt");
    SyncfusionLicenseProvider.RegisterLicense(licenseKey);
}

var builder = WebApplication.CreateBuilder(args);

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var logConnectionString = builder.Configuration.GetConnectionString("LogConnection");

// Add services to the container.




builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddApplicationDatabase(defaultConnectionString);
builder.Services.AddLogDatabase(logConnectionString);

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<ILogDbContext>(provider => provider.GetRequiredService<LogDbContext>());
//builder.Services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);
//builder.Services.AddTransient(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpClient("BlazorServer");
builder.Services.AddTransient<ApiService>();

builder.Services.AddSyncfusionBlazor();
builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    // Define the list of cultures your app will support
    var supportedCultures = new List<CultureInfo>()
    {
                    new CultureInfo("en-US"),
                    new CultureInfo("de"),
                    new CultureInfo("fr"),
                    new CultureInfo("ar"),
                    new CultureInfo("zh"),
    };
    // Set the default culture
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
builder.Services.AddSingleton<IClientOperations, ClientOperations>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());



// Supply HttpClient instances that include access tokens when making requests to the server project
//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorServer"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddResponseCompression(opt =>
{
    opt.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});
builder.Services.AddControllers();
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("MachineToMachine", policy => policy.RequireClaim("MachineToMachine"));
//});

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//              .AddJwtBearer(options =>
//              {
//                  options.TokenValidationParameters = new TokenValidationParameters
//                  {
//                      ValidateIssuer = true,
//                      ValidateAudience = true,
//                      ValidateLifetime = true,
//                      ValidateIssuerSigningKey = true,
//                      ValidIssuer = configuration["Jwt:Issuer"],
//                      ValidAudience = configuration["Jwt:Audience"],
//                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
//                  };
//              });
builder.Services
    .AddAuthentication()
        .AddJwtBearer("Bearer", options => { })
.AddJwtBearer("MachineToMachine", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        //ValidAudience = Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
    };
});
builder.Services
    .AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes("Bearer", "MachineToMachine")
            .Build();
        // options.AddPolicy("MyCustomPolicy",
        //policyBuilder => policyBuilder.RequireClaim("SomeClaim"));
    });
//builder.Services.AddAuthorization(auth =>
//{
//    auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
//        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
//        .RequireAuthenticatedUser().Build());

//    auth.AddPolicy("MachineToMachine", policy =>
//        policy.req.Add(new ApiKeyRequirement()));

//    auth.DefaultPolicy = auth.GetPolicy("Bearer");
//});
//builder.Services.AddAuthentication(x =>
//{
//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//             .AddJwtBearer(cfg =>
//             {
//                 cfg.RequireHttpsMetadata = false;
//                 cfg.SaveToken = true;
//                 cfg.TokenValidationParameters = new TokenValidationParameters()
//                 {
//                     //ValidateIssuerSigningKey = true,
//                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
//                     ValidateAudience = false,
//                     ValidateLifetime = true,
//                     ValidIssuer = configuration["Jwt:Issuer"],
//                     //ValidAudience = Configuration["Jwt:Audience"],
//                     //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
//                 };
//             });


builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<PlaceHolderClass>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRequestLocalization(app.Services.GetService<IOptions<RequestLocalizationOptions>>().Value);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");

app.MapDefaultControllerRoute();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
