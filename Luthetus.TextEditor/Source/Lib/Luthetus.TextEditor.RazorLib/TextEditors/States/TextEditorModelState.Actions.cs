using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

public partial class TextEditorModelState
{
    public record RegisterAction(TextEditorModel Model);
    public record DisposeAction(ResourceUri ResourceUri);
    public record UndoEditAction(ResourceUri ResourceUri);
    public record RedoEditAction(ResourceUri ResourceUri);
    public record ForceRerenderAction(ResourceUri ResourceUri);
    public record RegisterPresentationModelAction(ResourceUri ResourceUri, TextEditorPresentationModel PresentationModel);
    public record CalculatePresentationModelAction(ResourceUri ResourceUri, Key<TextEditorPresentationModel> PresentationKey);
    public record ReloadAction(ResourceUri ResourceUri, string Content, DateTime ResourceLastWriteTime);
    public record SetResourceDataAction(ResourceUri ResourceUri, DateTime ResourceLastWriteTime);
    public record SetUsingRowEndingKindAction(ResourceUri ResourceUri, RowEndingKind RowEndingKind);

    public record KeyboardEventAction(
        ResourceUri ResourceUri,
        Key<TextEditorViewModel> ViewModelKey,
        ImmutableArray<TextEditorCursor> CursorBag,
        ImmutableArray<TextEditorCursorModifier> CursorModifierBag,
        KeyboardEventArgs KeyboardEventArgs,
        CancellationToken CancellationToken);

    public record InsertTextAction(
        ResourceUri ResourceUri,
        Key<TextEditorViewModel> ViewModelKey,
        ImmutableArray<TextEditorCursor> CursorBag,
        ImmutableArray<TextEditorCursorModifier> CursorModifierBag,
        string Content,
        CancellationToken CancellationToken);

    public record DeleteTextByMotionAction(
        ResourceUri ResourceUri,
        Key<TextEditorViewModel> ViewModelKey,
        ImmutableArray<TextEditorCursor> CursorBag,
        ImmutableArray<TextEditorCursorModifier> CursorModifierBag,
        MotionKind MotionKind,
        CancellationToken CancellationToken);

    public record DeleteTextByRangeAction(
        ResourceUri ResourceUri,
        Key<TextEditorViewModel> ViewModelKey,
        ImmutableArray<TextEditorCursor> CursorBag,
        ImmutableArray<TextEditorCursorModifier> CursorModifierBag,
        int Count,
        CancellationToken CancellationToken);
}