using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Ide.Wasm;
using Luthetus.Website.RazorLib;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var hostingInformation = new LuthetusHostingInformation(
    LuthetusHostingKind.Wasm,
    new BackgroundTaskService());

builder.Services.AddLuthetusWebsiteServices(hostingInformation);

var host = builder.Build();

await host.RunAsync();