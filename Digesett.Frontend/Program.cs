global using Digesett.Frontend.Services;
using Digesett.Frontend;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CurrieTechnologies.Razor.SweetAlert2;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Servicios de HttpClient
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

builder.Services.AddHttpClient("Digesett", option =>
{
    option.BaseAddress = new Uri("http://172.16.0.18:2050");
    option.Timeout = TimeSpan.FromSeconds(15);
    option.DefaultRequestHeaders.Add("User-Agent", "BlazorApp");
});

builder.Services.AddSweetAlert2();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IServiceCommon, ServiceCommon>();

await builder.Build().RunAsync();
