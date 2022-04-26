using BlazorServerId4;
using BlazorServerId4.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading.Tasks;

IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient("BlazorServer");
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001") });

//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorServer"));


builder.Services.AddAuthentication(options =>
         {
             options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
             options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
         })
             .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
                 options =>
                 {
                     options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                     options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                     // Set Authority to setting in appsettings.json.  This is the URL of the IdentityServer4
                     options.Authority = configuration["OIDC:Authority"];
                     // Set ClientId to setting in appsettings.json.    This Client ID is set when registering the Blazor Server app in IdentityServer4
                     options.ClientId = configuration["OIDC:ClientId"];
                     // Set ClientSecret to setting in appsettings.json.  The secret value is set from the Client >  Basic tab in IdentityServer Admin UI
                     options.ClientSecret = configuration["OIDC:ClientSecret"];
                     // When set to code, the middleware will use PKCE protection
                     options.ResponseType = "code";
                     // Add request scopes.  The scopes are set in the Client >  Basic tab in IdentityServer Admin UI
                     options.Scope.Add("openid");
                     options.Scope.Add("profile");
                     options.Scope.Add("email");
                     options.Scope.Add("roles");
                     // Save access and refresh tokens to authentication cookie.  the default is false
                     options.SaveTokens = true;
                     // It's recommended to always get claims from the 
                     // UserInfoEndpoint during the flow. 
                     options.GetClaimsFromUserInfoEndpoint = true;
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         //map claim to name for display on the upper right corner after login.  Can be name, email, etc.
                         NameClaimType = "name",
                     };

                     options.Events = new OpenIdConnectEvents
                     {
                         OnAccessDenied = context =>
                            {
                                context.HandleResponse();
                                context.Response.Redirect("/");
                                return Task.CompletedTask;
                            }
                     };
                 });
//id4 integration end
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    //options.FallbackPolicy = options.DefaultPolicy;
});
//builder.Services.AddApiAuthorization().AddAccountClaimsPrincipalFactory<CustomUserFactory>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
