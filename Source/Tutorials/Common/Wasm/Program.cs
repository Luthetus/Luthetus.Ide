using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Luthetus.Common.Usage.Wasm;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.Usage.RazorLib;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var luthetusHostingInformation = new LuthetusHostingInformation(
    LuthetusHostingKind.Wasm,
    new BackgroundTaskService());

builder.Services.AddLuthetusCommonUsageServices(
    luthetusHostingInformation);

await builder.Build().RunAsync();
