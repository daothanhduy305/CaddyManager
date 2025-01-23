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
builder.Services.RegisterAssemblyPublicNonGenericClasses()
    .Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("Repository"))
    .AsPublicImplementedInterfaces();

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