using Luthetus.Ide.Wasm;
using Luthetus.Ide.RazorLib;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Repl.FileSystem;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;
using Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;
using Fluxor;
using Luthetus.Common.RazorLib;
using Luthetus.TextEditor.RazorLib;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddLuthetusIdeRazorLibServices(false);

builder.Services.AddScoped<IEnvironmentProvider, ReplEnvironmentProvider>();
builder.Services.AddScoped<IFileSystemProvider, ReplFileSystemProvider>();

builder.Services.AddSingleton<ICommonBackgroundTaskQueue, CommonBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<ITextEditorBackgroundTaskQueue, TextEditorBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<ICompilerServiceBackgroundTaskQueue, CompilerServiceBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<IFileSystemBackgroundTaskQueue, FileSystemBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<ITerminalBackgroundTaskQueue, TerminalBackgroundTaskQueueSingleThreaded>();

builder.Services.AddSingleton<CommonQueuedHostedService>();
builder.Services.AddSingleton<TextEditorQueuedHostedService>();
builder.Services.AddSingleton<CompilerServiceQueuedHostedService>();
builder.Services.AddSingleton<FileSystemQueuedHostedService>();
builder.Services.AddSingleton<TerminalQueuedHostedService>();

builder.Services.AddFluxor(options =>
    options.ScanAssemblies(
        typeof(Luthetus.Website.RazorLib.ServiceCollectionExtensions).Assembly,
        typeof(LuthetusCommonOptions).Assembly,
        typeof(LuthetusTextEditorOptions).Assembly,
        typeof(Luthetus.Ide.ClassLib.ServiceCollectionExtensions).Assembly));

await builder.Build().RunAsync();
