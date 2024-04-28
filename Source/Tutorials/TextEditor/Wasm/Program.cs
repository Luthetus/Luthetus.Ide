using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Luthetus.TextEditor.Usage.RazorLib;
using Luthetus.TextEditor.Usage.Wasm;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var luthetusHostingInformation = new LuthetusHostingInformation(
    LuthetusHostingKind.Wasm,
    new BackgroundTaskService());

builder.Services.AddLuthetusTextEditorUsageServices(luthetusHostingInformation);

await builder.Build().RunAsync();
