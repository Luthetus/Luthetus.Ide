using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Luthetus.TextEditor.RazorLib.Commands.Models;

public class TextEditorCommandParameter : ICommandParameter
{
    public TextEditorCommandParameter(
        TextEditorModel textEditor,
        ImmutableArray<TextEditorCursorSnapshot> cursorSnapshotsBag,
        bool hasTextSelection,
        IClipboardService clipboardService,
        ITextEditorService textEditorService,
        TextEditorViewModel textEditorViewModel,
        Func<MouseEventArgs, Task>? handleMouseStoppedMovingEventAsyncFunc,
        IJSRuntime? jsRuntime)
    {
        Model = textEditor;
        CursorSnapshotsBag = cursorSnapshotsBag;
        HasTextSelection = hasTextSelection;
        ClipboardService = clipboardService;
        TextEditorService = textEditorService;
        ViewModel = textEditorViewModel;
        HandleMouseStoppedMovingEventAsyncFunc = handleMouseStoppedMovingEventAsyncFunc;
        JsRuntime = jsRuntime;
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
}