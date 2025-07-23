using CaddyManager.Components;
using MudBlazor.Services;
using NetCore.AutoRegisterDi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddMudServices()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Auto register all the Services, Repositories that we have had within the code base
builder.Services.RegisterAssemblyPublicNonGenericClasses(System.Reflection.Assembly.GetAssembly(typeof(CaddyManager.Services.Caddy.CaddyService)))
    .Where(t => t.Name.EndsWith("Service"))
    .AsPublicImplementedInterfaces();

builder.Services.AddSignalR(e => { e.MaximumReceiveMessageSize = 102400000; });

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
    config.SnackbarConfiguration.HideTransitionDuration = 100;
    config.SnackbarConfiguration.ShowTransitionDuration = 100;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
