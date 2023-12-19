using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.AspNetCore.Components.Web;
using static Luthetus.TextEditor.RazorLib.Commands.Models.TextEditorCommand;
using static Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorService;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorEdit
{
    public TextEditorCommandArgs CommandArgs { get; set; }
    public TextEditorModel Model { get; set; }
    public TextEditorViewModel ViewModel { get; set; }
    public RefreshCursorsRequest RefreshCursorsRequest { get; set; }
    public TextEditorCursorModifier PrimaryCursor { get; set; }
    public bool? IsCompleted { get; }

    public Task Model_UndoEdit(ResourceUri resourceUri);
    public Task Model_SetUsingRowEndingKind(ResourceUri resourceUri, RowEndingKind rowEndingKind);
    public Task Model_SetResourceData(ResourceUri resourceUri, DateTime resourceLastWriteTime);
    public Task Model_Reload(ResourceUri resourceUri, string content, DateTime resourceLastWriteTime);
    public Task Model_RedoEdit(ResourceUri resourceUri);
    public Task Model_InsertText(InsertTextAction insertTextAction, RefreshCursorsRequest refreshCursorsRequest);
    public Task Model_HandleKeyboardEvent(KeyboardEventAction keyboardEventAction, RefreshCursorsRequest refreshCursorsRequest);
    public Task Model_DeleteTextByRange(DeleteTextByRangeAction deleteTextByRangeAction, RefreshCursorsRequest refreshCursorsRequest);
    public Task Model_DeleteTextByMotion(DeleteTextByMotionAction deleteTextByMotionAction, RefreshCursorsRequest refreshCursorsRequest);

    public Task ViewModel_GetWithValueTask(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc);

    public Task ViewModel_GetWithTaskTask(
        TextEditorViewModel viewModel,
        Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap);

    /// <summary>
    /// If a parameter is null the JavaScript will not modify that value
    /// </summary>
    public Task ViewModel_GetSetScrollPositionTask(
        string bodyElementId,
        string gutterElementId,
        double? scrollLeftInPixels,
        double? scrollTopInPixels);

    public Task ViewModel_GetSetGutterScrollTopTask(
        string gutterElementId,
        double scrollTopInPixels);

    public Task ViewModel_GetMutateScrollVerticalPositionTask(
        string bodyElementId,
        string gutterElementId,
        double pixels);

    public Task ViewModel_GetMutateScrollHorizontalPositionTask(
        string bodyElementId,
        string gutterElementId,
        double pixels);

    public Task ViewModel_GetFocusPrimaryCursorTask(string primaryCursorContentId);

    public Task ViewModel_GetMoveCursorTask(
        KeyboardEventArgs keyboardEventArgs,
        TextEditorModel model,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCursorModifier primaryCursor);

    public Task ViewModel_GetCursorMovePageTopTask(
        ResourceUri modelResourceUri,
        TextEditorViewModel viewModel,
        TextEditorCursorModifier primaryCursor);

    public Task ViewModel_GetCursorMovePageBottomTask(
        TextEditorModel model,
        TextEditorViewModel viewModel,
        TextEditorCursorModifier primaryCursor);

    public Task ViewModel_GetCalculateVirtualizationResultTask(
        TextEditorModel model,
        TextEditorViewModel viewModel,
        TextEditorMeasurements? textEditorMeasurements,
        TextEditorCursorModifier primaryCursor,
        CancellationToken cancellationToken);

    public Task ViewModel_GetRemeasureTask(
        ResourceUri modelResourceUri,
        TextEditorViewModel viewModel,
        string measureCharacterWidthAndRowHeightElementId,
        int countOfTestCharacters,
        CancellationToken cancellationToken);
}

