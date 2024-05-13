using HiLoGame.Components;
using HiLoGame.Hubs;
using static HiLoGame.Domain.Constants;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder
    .Services
    .AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHub<GameHub>(HubConstants.URL);

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(HiLoGame.Client._Imports).Assembly);

await app.RunAsync();