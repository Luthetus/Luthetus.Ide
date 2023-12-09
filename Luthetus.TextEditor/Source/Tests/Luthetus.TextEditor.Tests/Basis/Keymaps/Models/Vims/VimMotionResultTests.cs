using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public record VimMotionResult(
    ImmutableTextEditorCursor LowerPositionIndexImmutableCursor,
    int LowerPositionIndex,
    ImmutableTextEditorCursor HigherPositionIndexImmutableCursor,
    int HigherPositionIndex,
    int PositionIndexDisplacement)
{
    public static async Task<VimMotionResult> GetResultAsync(
        TextEditorCommandArgs textEditorCommandArgs,
        TextEditorCursor textEditorCursorForMotion,
        Func<Task> motionCommandArgs)
    {
        await motionCommandArgs.Invoke();

        var beforeMotionImmutableCursor = textEditorCommandArgs.PrimaryCursorSnapshot.ImmutableCursor;

        var beforeMotionPositionIndex = textEditorCommandArgs.Model.GetPositionIndex(
            beforeMotionImmutableCursor.RowIndex,
            beforeMotionImmutableCursor.ColumnIndex);

        var afterMotionImmutableCursor = new ImmutableTextEditorCursor(textEditorCursorForMotion);

        var afterMotionPositionIndex = textEditorCommandArgs.Model.GetPositionIndex(
            afterMotionImmutableCursor.RowIndex,
            afterMotionImmutableCursor.ColumnIndex);

        if (beforeMotionPositionIndex > afterMotionPositionIndex)
        {
            return new VimMotionResult(
                afterMotionImmutableCursor,
                afterMotionPositionIndex,
                beforeMotionImmutableCursor,
                beforeMotionPositionIndex,
                beforeMotionPositionIndex - afterMotionPositionIndex);
        }

        return new VimMotionResult(
            beforeMotionImmutableCursor,
            beforeMotionPositionIndex,
            afterMotionImmutableCursor,
            afterMotionPositionIndex,
            afterMotionPositionIndex - beforeMotionPositionIndex);
    }
}