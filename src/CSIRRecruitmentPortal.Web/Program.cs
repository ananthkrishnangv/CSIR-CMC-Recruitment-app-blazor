using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CSIRRecruitmentPortal.Web;
using CSIRRecruitmentPortal.Web.Services;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register HttpClient with base address from the host environment (the API server)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add MudBlazor services
builder.Services.AddMudServices();

// Add local storage
builder.Services.AddBlazoredLocalStorage();

// Add Authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ClientAuthService>();
builder.Services.AddScoped<IAuthService>(provider => provider.GetRequiredService<AuthenticationStateProvider>() as IAuthService);

// Add Application services
builder.Services.AddScoped<IConfigService, ClientConfigService>();
builder.Services.AddScoped<IPostService, ClientPostService>();
builder.Services.AddScoped<IApplicationService, ClientApplicationService>();

await builder.Build().RunAsync();
