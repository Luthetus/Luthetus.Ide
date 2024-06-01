// This file was commented out because 'TextEditorModelState' and 'TextEditorViewModelState'
// have been combined into the same type 'TextEditorState'.
//
// TODO: Make tests for 'TextEditorState' and then delete the tests for ...
//       ...'TextEditorModelState' and 'TextEditorViewModelState'

/*
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorViewModelState;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.Tests.JsRuntimes;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.States;

/// <summary>
/// <see cref="TextEditorViewModelState"/>
/// </summary>
public class TextEditorViewModelStateReducerTests
{
	/// <summary>
	/// <see cref="Reducer.ReduceRegisterAction(TextEditorViewModelState, RegisterAction)"/>
	/// </summary>
	[Fact]
	public void ReduceRegisterAction()
	{
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //      InitializeTextEditorViewModelStateReducerTestsHelper(
        //          out var textEditorService,
        //          out var inModel,
        //          out var serviceProvider);

        //      var viewModelKey = Key<TextEditorViewModel>.NewKey();
        //      var category = new TextEditorCategory("UnitTesting");

        //      var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        //var textEditorViewModelStateWrap = serviceProvider.GetRequiredService<IState<TextEditorViewModelState>>();

        //      // Fact: TextEditorViewModelState initially has an empty ViewModelList
        //      {
        //          Assert.Empty(textEditorViewModelStateWrap.Value.ViewModelList);
        //      }

        //      RegisterAction registerAction;
        //      // Fact: RegisterAction constructor is working.
        //      {
        //          registerAction = new RegisterAction(
        //              viewModelKey,
        //              inModel.ResourceUri,
        //              category,
        //              textEditorService);

        //          Assert.Equal(viewModelKey, registerAction.ViewModelKey);
        //          Assert.Equal(inModel.ResourceUri, registerAction.ResourceUri);
        //          Assert.Equal(category, registerAction.Category);
        //          Assert.Equal(textEditorService, registerAction.TextEditorService);
        //      }

        //      // Fact: Dispatching a RegisterAction adds to ViewModelList
        //      {
        //          dispatcher.Dispatch(registerAction);
        //          Assert.Single(textEditorViewModelStateWrap.Value.ViewModelList);
        //      }

        //      // Fact: ViewModelList can contain more than one view model.
        //      {
        //          registerAction = registerAction with
        //          {
        //              ViewModelKey = Key<TextEditorViewModel>.NewKey(),
        //              // Remove the first character to ensure a unique resource uri. (presuming it initially has length >= 1)
        //              ResourceUri = new(registerAction.ResourceUri.Value.Remove(0, 1))
        //          };
        //          dispatcher.Dispatch(registerAction);
        //          Assert.Equal(2, textEditorViewModelStateWrap.Value.ViewModelList.Count);
        //      }

        //      // Fact: Attempting to register a view model with the 'ViewModelKey' of 'Empty' throws an exception.
        //      {
        //          Assert.Throws<InvalidOperationException>(() =>
        //          {
        //              dispatcher.Dispatch(new RegisterAction(
        //                  Key<TextEditorViewModel>.Empty,
        //                  inModel.ResourceUri,
        //                  category,
        //                  textEditorService,
        //                  dispatcher,
        //                  dialogService,
        //                  jsRuntime));
        //          });
        //      }

        //      // Fact: SetViewModelWithAction constructor is working.
        //      SetViewModelWithAction setViewModelWithAction;
        //      {
        //          var outCommandBarValue = "abc123";

        //          textEditorService.Post(
        //              nameof(SetViewModelWithAction),
        //              editContext =>
        //              {
        //                  var authenticatedActionKey = TextEditorService.AuthenticatedActionKey;
        //                  var withFunc = new Func<TextEditorViewModel, TextEditorViewModel>(inState =>
        //                  {
        //                      Assert.NotEqual(outCommandBarValue, inState.CommandBarValue);
        //                      return inState with
        //                      {
        //                          CommandBarValue = outCommandBarValue
        //                      };
        //                  });

        //                  setViewModelWithAction = new SetViewModelWithAction(
        //                      authenticatedActionKey,
        //                      editContext,
        //                      viewModelKey,
        //                      withFunc);

        //                  Assert.Equal(editContext, setViewModelWithAction.EditContext);
        //                  Assert.Equal(viewModelKey, setViewModelWithAction.ViewModelKey);
        //                  Assert.Equal(withFunc, setViewModelWithAction.WithFunc);

        //                  dispatcher.Dispatch(setViewModelWithAction);
        //                  return Task.CompletedTask;
        //              });

        //          var outViewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey);
        //          Assert.NotNull(outViewModel);
        //          Assert.Equal(outCommandBarValue, outViewModel!.CommandBarValue);
        //      }

        //      // Fact: DisposeAction constructor is working.
        //      DisposeAction disposeAction;
        //      {
        //          disposeAction = new DisposeAction(viewModelKey);
        //          Assert.Equal(viewModelKey, disposeAction.ViewModelKey);
        //      }

        //      // Fact: Dispatching a DisposeAction removes from ViewModelList
        //      {
        //          dispatcher.Dispatch(disposeAction);
        //          Assert.Single(textEditorViewModelStateWrap.Value.ViewModelList);
        //      }

        //      // Fact: ViewModelList can remove its only remaining entry
        //      {
        //          dispatcher.Dispatch(disposeAction with { ViewModelKey = registerAction.ViewModelKey });
        //          Assert.Empty(textEditorViewModelStateWrap.Value.ViewModelList);
        //      }
    }
    
    private static void InitializeTextEditorViewModelStateReducerTestsHelper(
        out ITextEditorService textEditorService,
        out TextEditorModel model,
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

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var initialContent = "Hello World!";

        textEditorService.ModelApi.RegisterTemplated(
            fileExtension,
            resourceUri,
            resourceLastWriteTime,
            initialContent);

        model = textEditorService.ModelApi.GetOrDefault(resourceUri)
           ?? throw new ArgumentNullException();
    }
}
*/