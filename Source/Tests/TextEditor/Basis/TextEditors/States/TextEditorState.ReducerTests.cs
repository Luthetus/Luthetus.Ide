using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.Tests.JsRuntimes;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.States;

/// <summary>
/// <see cref="TextEditorState"/>
/// </summary>
public class TextEditorStateReducerTests
{
	/// <summary>
    /// <see cref="Reducer.ReduceRegisterAction(TextEditorModelState, RegisterAction)"/>
    /// </summary>
    [Fact]
	public void ReduceRegisterAction()
	{
        InitializeTextEditorModelStateReducerTestsHelper(
            out var textEditorService,
            out var serviceProvider);

        var textEditorModelStateWrap = serviceProvider.GetRequiredService<IState<TextEditorState>>();
        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        var authenticatedActionKey = TextEditorService.AuthenticatedActionKey;

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var initialContent = "Hello World!";

        var model = new TextEditorModel(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            null,
            null);

        // Fact: ModelList is initially empty
        {
            Assert.Empty(textEditorModelStateWrap.Value.ModelList);
        }

        // Fact: new RegisterAction(...)
        TextEditorState.RegisterModelAction registerAction;
        {
            registerAction = new TextEditorState.RegisterModelAction(authenticatedActionKey, model);
            Assert.Equal(authenticatedActionKey, registerAction.AuthenticatedActionKey);
            Assert.Equal(model, registerAction.Model);
        }

        // Fact: Dispatching RegisterAction causes a TextEditorModel to be added to ModelList
        {
            dispatcher.Dispatch(registerAction);
            Assert.Single(textEditorModelStateWrap.Value.ModelList);
        }

        // Fact: ModelList can contain more than one TextEditorModel
        {
            var otherModel = new TextEditorModel(
                // Remove the first character to ensure a unique resource uri. (presuming it initially has length >= 1)
                new(resourceUri.Value.Remove(0, 1)),
                resourceLastWriteTime,
                fileExtension,
                initialContent,
                null,
                null);

            dispatcher.Dispatch(new TextEditorState.RegisterModelAction(authenticatedActionKey, otherModel));
            Assert.Equal(2, textEditorModelStateWrap.Value.ModelList.Count);
        }


        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="Reducer.ReduceDisposeAction(TextEditorModelState, DisposeAction)"/>
    /// </summary>
    [Fact]
	public void ReduceDisposeAction()
	{
        //InitializeTextEditorModelStateReducerTestsHelper(
        //    out var textEditorService,
        //    out var serviceProvider);

        //var authenticatedActionKey = TextEditorService.AuthenticatedActionKey;

        //var disposeAction = new DisposeAction(
        //    authenticatedActionKey,
        //    inModel.ResourceUri);

        //Assert.Equal(authenticatedActionKey, disposeAction.AuthenticatedActionKey);
        //Assert.Equal(inModel.ResourceUri, disposeAction.ResourceUri);

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="Reducer.ReduceSetAction(TextEditorModelState, SetAction)"/>
    /// </summary>
    [Fact]
	public void ReduceSetAction()
	{
		throw new NotImplementedException();
	}

    private static void InitializeTextEditorModelStateReducerTestsHelper(
        out ITextEditorService textEditorService,
        out IServiceProvider serviceProvider)
    {
        var services = new ServiceCollection()
            .AddSingleton<LuthetusCommonConfig>()
            .AddSingleton<LuthetusTextEditorConfig>()
            .AddScoped<IStorageService, DoNothingStorageService>()
            .AddScoped<IJSRuntime, TextEditorTestingJsRuntime>()
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
    }
}
