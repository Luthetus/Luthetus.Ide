using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public record VimMotionResult(
    TextEditorCursor LowerPositionIndexCursor,
    int LowerPositionIndex,
    TextEditorCursor HigherPositionIndexCursor,
    int HigherPositionIndex,
    int PositionIndexDisplacement)
{
    public static async Task<VimMotionResult> GetResultAsync(
        TextEditorCommandArgs textEditorCommandArgs,
        TextEditorCursor textEditorCursorForMotion,
        Func<Task> motionCommand)
    {
        await motionCommand.Invoke();

        var beforeMotionCursor = textEditorCommandArgs.PrimaryCursor;

        var beforeMotionPositionIndex = textEditorCommandArgs.ModelResourceUri.GetPositionIndex(
            beforeMotionCursor.RowIndex,
            beforeMotionCursor.ColumnIndex);

        var afterMotionCursor = textEditorCursorForMotion;

        var afterMotionPositionIndex = textEditorCommandArgs.ModelResourceUri.GetPositionIndex(
            afterMotionCursor.RowIndex,
            afterMotionCursor.ColumnIndex);

        if (beforeMotionPositionIndex > afterMotionPositionIndex)
        {
            return new VimMotionResult(
                afterMotionCursor,
                afterMotionPositionIndex,
                beforeMotionCursor,
                beforeMotionPositionIndex,
                beforeMotionPositionIndex - afterMotionPositionIndex);
        }

        return new VimMotionResult(
            beforeMotionCursor,
            beforeMotionPositionIndex,
            afterMotionCursor,
            afterMotionPositionIndex,
            afterMotionPositionIndex - beforeMotionPositionIndex);
    }
}