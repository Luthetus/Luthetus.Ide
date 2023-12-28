using Microsoft.AspNetCore.Components.Web;
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
    public record RegisterAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            TextEditorModel Model)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record DisposeAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ResourceUri ResourceUri)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record ForceRerenderAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ResourceUri ResourceUri)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record RegisterPresentationModelAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ResourceUri ResourceUri,
            TextEditorPresentationModel PresentationModel)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record CalculatePresentationModelAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            Key<TextEditorPresentationModel> PresentationKey)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record KeyboardEventAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            Key<TextEditorViewModel> ViewModelKey,
            KeyboardEventArgs KeyboardEventArgs,
            CancellationToken CancellationToken)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record InsertTextAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            Key<TextEditorViewModel> ViewModelKey,
            string Content,
            CancellationToken CancellationToken)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

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
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            string Content,
            CancellationToken CancellationToken)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record DeleteTextByMotionAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            Key<TextEditorViewModel> ViewModelKey,
            MotionKind MotionKind,
            CancellationToken CancellationToken)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

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
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            MotionKind MotionKind,
            CancellationToken CancellationToken)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record DeleteTextByRangeAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            Key<TextEditorViewModel> ViewModelKey,
            int Count,
            CancellationToken CancellationToken)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

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
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            ResourceUri ResourceUri,
            TextEditorCursorModifierBag CursorModifierBag,
            int Count,
            CancellationToken CancellationToken)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record SetModelAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            TextEditorModelModifier ModelModifier)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);
}