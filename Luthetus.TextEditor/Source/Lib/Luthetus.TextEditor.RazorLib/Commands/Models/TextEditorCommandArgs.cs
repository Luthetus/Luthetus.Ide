using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Fluxor;

namespace Luthetus.TextEditor.RazorLib.Commands.Models;

public class TextEditorCommandArgs : ICommandArgs
{
    public TextEditorCommandArgs(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        bool hasTextSelection,
        IClipboardService clipboardService,
        ITextEditorService textEditorService,
        Func<MouseEventArgs, Task>? handleMouseStoppedMovingEventAsyncFunc,
        IJSRuntime? jsRuntime,
        IDispatcher dispatcher,
        Action<ResourceUri>? registerModelAction,
        Action<ResourceUri>? registerViewModelAction,
        Action<Key<TextEditorViewModel>>? showViewModelAction)
    {
        ModelResourceUri = modelResourceUri;
        ViewModelKey = viewModelKey;
        HasTextSelection = hasTextSelection;
        ClipboardService = clipboardService;
        TextEditorService = textEditorService;
        HandleMouseStoppedMovingEventAsyncFunc = handleMouseStoppedMovingEventAsyncFunc;
        JsRuntime = jsRuntime;
        Dispatcher = dispatcher;
        RegisterModelAction = registerModelAction;
        RegisterViewModelAction = registerViewModelAction;
        ShowViewModelAction = showViewModelAction;
    }

    public ResourceUri ModelResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public IClipboardService ClipboardService { get; }
    public ITextEditorService TextEditorService { get; }
    /// <summary>
    /// This property is used so a keyboard event can trigger a tooltip at the cursor's position.
    /// </summary>
    public Func<MouseEventArgs, Task>? HandleMouseStoppedMovingEventAsyncFunc { get; }
    public IJSRuntime? JsRuntime { get; }
    public IDispatcher Dispatcher { get; }
    public bool HasTextSelection { get; set; }
    /// <summary>
    /// The go-to definition implementation makes use of <see cref="RegisterModelAction"/>.<br/>
    /// 
    /// In the case that a symbol's definition exists within a resource that does not have
    /// an already existing Model, then this is invoked to create that instance, so that
    /// go-to definition can then be performed.<br/>
    /// 
    /// The Func takes in the resource uri that needs a model.
    /// </summary>
    public Action<ResourceUri>? RegisterModelAction { get; set; }
    /// <summary>
    /// The go-to definition implementation makes use of <see cref="RegisterModelAction"/>.<br/>
    /// 
    /// In the case that a symbol's definition exists within a resource that does not have
    /// an already existing ViewModel, then this is invoked to create that instance, so that
    /// go-to definition can then be performed.<br/>
    /// 
    /// The Func takes in the resource uri that needs a ViewModel.
    /// </summary>
    public Action<ResourceUri>? RegisterViewModelAction { get; set; }
    public Action<Key<TextEditorViewModel>>? ShowViewModelAction { get; set; }

    /// <summary>
    /// Hack for <see cref="Defaults.TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(bool)"/>
    /// to be able to select text. (2023-12-15)
    /// </summary>
    public bool ShouldSelectText { get; set; }

    /// <summary>
    /// Hack for <see cref="Vims.TextEditorCommandVimFacts.Motions.GetVisual(TextEditorCommand, string)"/>
    /// to be able to select text. (2023-12-15)
    /// </summary>
    public TextEditorCommand InnerCommand { get; set; }
    /// <summary>
    /// Hack for <see cref="Vims.TextEditorCommandVimFacts.Motions.GetVisual(TextEditorCommand, string)"/>
    /// to be able to select text. (2023-12-15)
    /// </summary>
    public string DisplayName { get; set; }
}
