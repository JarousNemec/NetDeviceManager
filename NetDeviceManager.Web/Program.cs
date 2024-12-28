using System.Net;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Identity;
using NetDeviceManager.Lib.Extensions;
using NetDeviceManager.Lib.Facades;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Services;
using NetDeviceManager.Web.Components;
using NetDeviceManager.Web.Components.Account;
using NetDeviceManager.Web.Components.Layout;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "dd.MM.yyyy HH:mm:ss ";
    options.ColorBehavior = LoggerColorBehavior.Enabled;
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSignalR(e => { e.MaximumReceiveMessageSize = 102400000; });

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(connectionString); }
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
builder.Services.AddBlazorBootstrap();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<ISnmpService, SnmpService>();
builder.Services.AddScoped<ISyslogService, SyslogService>();
builder.Services.AddSingleton<NavbarHelper>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<IPortService, PortService>();

builder.Services.AddScoped<ILoginProfileService, LoginProfileServiceFacade>();
builder.Services.AddScoped<LoginProfileService>();

builder.Services.AddScoped<IPortService, PortServiceFacade>();
builder.Services.AddScoped<PortService>();

builder.Services.AddScoped<IIpAddressesService, IpAddressService>();
// builder.Logging.SetMinimumLevel(GlobalSettings.MinimalLoggingLevel);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();