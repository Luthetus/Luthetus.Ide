using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Luthetus.TextEditor.Usage.RazorLib;
using Luthetus.TextEditor.Usage.Wasm;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddLuthetusTextEditorUsageServices();

builder.Services.AddSingleton<CommonQueuedHostedService>();
builder.Services.AddSingleton<TextEditorQueuedHostedService>();
builder.Services.AddSingleton<CompilerServiceQueuedHostedService>();

builder.Services.AddSingleton<ICommonBackgroundTaskQueue, CommonBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<ITextEditorBackgroundTaskQueue, TextEditorBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<ICompilerServiceBackgroundTaskQueue, CompilerServiceBackgroundTaskQueueSingleThreaded>();

await builder.Build().RunAsync();
