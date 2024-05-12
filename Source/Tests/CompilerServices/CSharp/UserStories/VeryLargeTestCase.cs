namespace Luthetus.CompilerServices.Lang.CSharp.Tests.UserStories;

internal class VeryLargeTestCase
{
    public static readonly string Value = @"using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.UserStories;

/// <summary>
/// Many tests in this project are pre-written string inputs.
/// I want to have this test write to a file using the text editor,
/// character by character.
/// </summary>
public class USER_TYPES_OUT_CODE
{
    [Fact]
    public void BLAZOR_WASM_TEMPLATE_PROGRAM_CS()
    {
        Initialize_USER_TYPES_OUT_CODE(
            string.Empty,
            out var textEditorService,
            out var cSharpCompilerService,
            out var textEditorModel,
            out var textEditorViewModel,
            out var serviceProvider);

        var content = @""using BlazorCrudApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>(""""#app"""");
builder.RootComponents.Add<HeadOutlet>(""""head::after"""");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
"".ReplaceLineEndings(""\n"");

        foreach (var character in content)
        {
            textEditorService.Post(
                nameof(USER_TYPES_OUT_CODE),
                async editContext =>
                {
                    await textEditorService.ModelApi.InsertTextFactory(
                            textEditorModel.ResourceUri,
                            textEditorViewModel.ViewModelKey,
                            character.ToString(),
                            CancellationToken.None)
                        .Invoke(editContext);
                });

            cSharpCompilerService.ResourceWasModified(
                textEditorModel.ResourceUri,
                ImmutableArray<TextEditorTextSpan>.Empty);
        }

        var resultText = textEditorService.ModelApi.GetAllText(textEditorModel.ResourceUri);
        Assert.Equal(content, resultText);

        var cSharpResource = (CSharpResource?)cSharpCompilerService.GetCompilerServiceResourceFor(textEditorModel.ResourceUri);
        Assert.NotNull(cSharpResource);
    }

    private static void Initialize_USER_TYPES_OUT_CODE(
        string initialContent,
        out ITextEditorService textEditorService,
        out CSharpCompilerService cSharpCompilerService,
        out TextEditorModel model,
        out TextEditorViewModel viewModel,
        out IServiceProvider serviceProvider)
    {
        var services = new ServiceCollection()
            .AddSingleton<LuthetusCommonConfig>()
            .AddSingleton<LuthetusTextEditorConfig>()
            .AddScoped<IStorageService, DoNothingStorageService>()
            .AddScoped<IJSRuntime, DoNothingJsRuntime>()
            .AddScoped<StorageSync>()
            .AddScoped<IBackgroundTaskService>(_ => new BackgroundTaskServiceSynchronous())
            .AddScoped<ITextEditorRegistryWrap, TextEditorRegistryWrap>()
            .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistryDefault>()
            .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistryDefault>()
            .AddScoped<ITextEditorService, TextEditorService>()
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonConfig).Assembly,
                typeof(LuthetusTextEditorConfig).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        var backgroundTaskService = serviceProvider.GetRequiredService<IBackgroundTaskService>();

        var continuousQueue = new BackgroundTaskQueue(
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(continuousQueue);

        var blockingQueue = new BackgroundTaskQueue(
            BlockingBackgroundTaskWorker.GetQueueKey(),
            BlockingBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(blockingQueue);

        var textEditorRegistryWrap = serviceProvider.GetRequiredService<ITextEditorRegistryWrap>();

        textEditorRegistryWrap.DecorationMapperRegistry = serviceProvider
            .GetRequiredService<IDecorationMapperRegistry>();

        textEditorRegistryWrap.CompilerServiceRegistry = serviceProvider
            .GetRequiredService<ICompilerServiceRegistry>();

        textEditorService = serviceProvider.GetRequiredService<ITextEditorService>();

        cSharpCompilerService = new CSharpCompilerService(
            textEditorService,
            backgroundTaskService,
            serviceProvider.GetRequiredService<IDispatcher>());

        var fileExtension = ExtensionNoPeriodFacts.C_SHARP_CLASS;
        var resourceUri = new ResourceUri(""/unitTesting.txt"");
        var resourceLastWriteTime = DateTime.UtcNow;

        model = new TextEditorModel(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            new GenericDecorationMapper(),
            cSharpCompilerService);

        textEditorService.ModelApi.RegisterCustom(model);

        cSharpCompilerService.RegisterResource(model.ResourceUri);

        var viewModelKey = Key<TextEditorViewModel>.NewKey();

        textEditorService.ViewModelApi.Register(
            viewModelKey,
            resourceUri);

        viewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey)
           ?? throw new ArgumentNullException();
    }
}
".ReplaceLineEndings("\n");
}
