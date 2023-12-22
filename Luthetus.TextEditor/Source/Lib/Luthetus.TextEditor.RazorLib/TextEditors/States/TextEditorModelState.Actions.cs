using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

public partial class TextEditorModelState
{
    public record RegisterAction(TextEditorModel Model);
    public record DisposeAction(ResourceUri ResourceUri);
    public record ForceRerenderAction(ResourceUri ResourceUri);
    
    public record RegisterPresentationModelAction(
            ResourceUri ResourceUri,
            TextEditorPresentationModel PresentationModel);
    
    public record UndoEditAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    public record RedoEditAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    public record CalculatePresentationModelAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            Key<TextEditorPresentationModel> PresentationKey)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    public record ReloadAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            string Content,
            DateTime ResourceLastWriteTime)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    public record SetResourceDataAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            DateTime ResourceLastWriteTime)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    public record SetUsingRowEndingKindAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            RowEndingKind RowEndingKind)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    public record KeyboardEventAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            Key<TextEditorViewModel> ViewModelKey,
            KeyboardEventArgs KeyboardEventArgs,
            CancellationToken CancellationToken)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="KeyboardEventAction"/>
    /// instead of this action. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public record KeyboardEventUnsafeAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            KeyboardEventArgs KeyboardEventArgs,
            CancellationToken CancellationToken)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    public record InsertTextAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            Key<TextEditorViewModel> ViewModelKey,
            string Content,
            CancellationToken CancellationToken)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="InsertTextAction"/>
    /// instead of this action. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public record InsertTextUnsafeAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            string Content,
            CancellationToken CancellationToken)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    public record DeleteTextByMotionAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            Key<TextEditorViewModel> ViewModelKey,
            MotionKind MotionKind,
            CancellationToken CancellationToken)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="DeleteTextByMotionAction"/>
    /// instead of this action. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public record DeleteTextByMotionUnsafeAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            MotionKind MotionKind,
            CancellationToken CancellationToken)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    public record DeleteTextByRangeAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            Key<TextEditorViewModel> ViewModelKey,
            int Count,
            CancellationToken CancellationToken)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="DeleteTextByRangeAction"/>
    /// instead of this action. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public record DeleteTextByRangeUnsafeAction(
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            int Count,
            CancellationToken CancellationToken)
        : AuthenticatedAction(EditContext.AuthenticatedActionKey);

    public record SetModelAction(TextEditorModelModifier ModelModifier);
}