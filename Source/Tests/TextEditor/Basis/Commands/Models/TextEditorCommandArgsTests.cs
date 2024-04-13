using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.Tests.Basis.Commands.Models;

/// <summary>
/// <see cref="TextEditorCommandArgs"/>
/// </summary>
public class TextEditorCommandArgsTests
{
    /// <summary>
    /// <see cref="TextEditorCommandArgs(ResourceUri, Key{TextEditorViewModel}, bool, IClipboardService, ITextEditorService, Func{MouseEventArgs, Task}?, IJSRuntime?, Fluxor.IDispatcher, Action{ResourceUri}?, Action{ResourceUri}?, Action{Key{TextEditorViewModel}}?)"/>
    /// <br/>----<br/>
	/// <see cref="TextEditorCommandArgs.ModelResourceUri"/>
    /// <see cref="TextEditorCommandArgs.PrimaryCursor"/>
    /// <see cref="TextEditorCommandArgs.CursorList"/>
    /// <see cref="TextEditorCommandArgs.ClipboardService"/>
    /// <see cref="TextEditorCommandArgs.TextEditorService"/>
    /// <see cref="TextEditorCommandArgs.ViewModelKey"/>
    /// <see cref="TextEditorCommandArgs.HandleMouseStoppedMovingEventAsyncFunc"/>
    /// <see cref="TextEditorCommandArgs.JsRuntime"/>
    /// <see cref="TextEditorCommandArgs.HasTextSelection"/>
    /// <see cref="TextEditorCommandArgs.RegisterModelAction"/>
    /// <see cref="TextEditorCommandArgs.RegisterViewModelAction"/>
    /// <see cref="TextEditorCommandArgs.ShowViewModelAction"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //      TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
        //          out var textEditorService,
        //          out var inModel,
        //          out var inViewModel,
        //          out var serviceProvider);

        //      ResourceUri modelResourceUri = new ResourceUri("/unitTesting.txt");
        //      Key<TextEditorViewModel> viewModelKey = Key<TextEditorViewModel>.NewKey();
        //      bool hasTextSelection = false;
        //      IClipboardService clipboardService = new InMemoryClipboardService();
        //      // ITextEditorService textEditorService; // defined via invoking 'InitializeTextEditorServicesTestsHelper'
        //      Func<MouseEventArgs, Task>? handleMouseStoppedMovingEventAsyncFunc = mouseEventArgs => Task.CompletedTask;
        //      IJSRuntime? jsRuntime = new DoNothingJsRuntime();
        //      IDispatcher dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        //      LuthetusTextEditorConfig textEditorConfig = serviceProvider.GetRequiredService<LuthetusTextEditorConfig>();
        //      Action<ResourceUri>? registerModelAction = resourceUri => { };
        //      Action<ResourceUri>? registerViewModelAction = resourceUri => { };
        //      Action<Key<TextEditorViewModel>>? showViewModelAction = viewModelKey => { };

        //      var commandArgs = new TextEditorCommandArgs(
        //          modelResourceUri,
        //          viewModelKey,
        //          hasTextSelection,
        //          clipboardService,
        //          textEditorService,
        //          TextEditorOptions,
        //          handleMouseStoppedMovingEventAsyncFunc,
        //          jsRuntime,
        //          dispatcher,
        //          serviceProvider,
        //          textEditorConfig);

        //Assert.Equal(modelResourceUri, commandArgs.ModelResourceUri);
        //Assert.Equal(viewModelKey, commandArgs.ViewModelKey);
        //Assert.Equal(hasTextSelection, commandArgs.HasTextSelection);
        //      Assert.Equal(clipboardService, commandArgs.ClipboardService);
        //      Assert.Equal(textEditorService, commandArgs.TextEditorService);
        //      Assert.Equal(handleMouseStoppedMovingEventAsyncFunc, commandArgs.HandleMouseStoppedMovingEventAsyncFunc);
        //      Assert.Equal(jsRuntime, commandArgs.JsRuntime);
        //      Assert.Equal(dispatcher, commandArgs.Dispatcher);
        //      Assert.Equal(serviceProvider, commandArgs.ServiceProvider);
        //      Assert.Equal(textEditorConfig, commandArgs.TextEditorConfig);
    }
}