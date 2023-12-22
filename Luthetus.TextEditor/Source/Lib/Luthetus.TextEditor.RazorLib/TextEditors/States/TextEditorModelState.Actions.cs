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
            ResourceUri ResourceUri,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

    public record RedoEditAction(
            ResourceUri ResourceUri,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

    public record CalculatePresentationModelAction(
            ResourceUri ResourceUri,
            Key<TextEditorPresentationModel> PresentationKey,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

    public record ReloadAction(
            ResourceUri ResourceUri,
            string Content,
            DateTime ResourceLastWriteTime,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

    public record SetResourceDataAction(
            ResourceUri ResourceUri,
            DateTime ResourceLastWriteTime,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

    public record SetUsingRowEndingKindAction(
            ResourceUri ResourceUri,
            RowEndingKind RowEndingKind,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

    public record KeyboardEventAction(
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            KeyboardEventArgs KeyboardEventArgs,
            CancellationToken CancellationToken,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

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
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            KeyboardEventArgs KeyboardEventArgs,
            CancellationToken CancellationToken,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

    public record InsertTextAction(
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            string Content,
            CancellationToken CancellationToken,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

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
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            string Content,
            CancellationToken CancellationToken,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

    public record DeleteTextByMotionAction(
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            MotionKind MotionKind,
            CancellationToken CancellationToken,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

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
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            MotionKind MotionKind,
            CancellationToken CancellationToken,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

    public record DeleteTextByRangeAction(
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            int Count,
            CancellationToken CancellationToken,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);

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
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            int Count,
            CancellationToken CancellationToken,
            Key<AuthenticatedAction> AuthenticatedActionKey)
        : AuthenticatedAction(AuthenticatedActionKey);
}