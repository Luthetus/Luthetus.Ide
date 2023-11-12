using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models;

public class TextEditorCommandArgs : ICommandArgs
{
    public TextEditorCommandArgs(
        TextEditorModel textEditor,
        ImmutableArray<TextEditorCursorSnapshot> cursorSnapshotsBag,
        bool hasTextSelection,
        IClipboardService clipboardService,
        ITextEditorService textEditorService,
        TextEditorViewModel textEditorViewModel,
        Func<MouseEventArgs, Task>? handleMouseStoppedMovingEventAsyncFunc,
        IJSRuntime? jsRuntime,
        Action<ResourceUri>? registerModelAction,
        Action<ResourceUri>? registerViewModelAction,
        Action<Key<TextEditorViewModel>>? showViewModelAction)
    {
        Model = textEditor;
        CursorSnapshotsBag = cursorSnapshotsBag;
        HasTextSelection = hasTextSelection;
        ClipboardService = clipboardService;
        TextEditorService = textEditorService;
        ViewModel = textEditorViewModel;
        HandleMouseStoppedMovingEventAsyncFunc = handleMouseStoppedMovingEventAsyncFunc;
        JsRuntime = jsRuntime;
        RegisterModelAction = registerModelAction;
        RegisterViewModelAction = registerViewModelAction;
        ShowViewModelAction = showViewModelAction;
    }

    public TextEditorModel Model { get; }

    public TextEditorCursorSnapshot PrimaryCursorSnapshot => CursorSnapshotsBag.First(
        x => x.UserCursor.IsPrimaryCursor);

    public ImmutableArray<TextEditorCursorSnapshot> CursorSnapshotsBag { get; }
    public IClipboardService ClipboardService { get; }
    public ITextEditorService TextEditorService { get; }
    public TextEditorViewModel ViewModel { get; }
    /// <summary>
    /// This property is used so a keyboard event can trigger a tooltip at the cursor's position.
    /// </summary>
    public Func<MouseEventArgs, Task>? HandleMouseStoppedMovingEventAsyncFunc { get; }
    public IJSRuntime? JsRuntime { get; }
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
}