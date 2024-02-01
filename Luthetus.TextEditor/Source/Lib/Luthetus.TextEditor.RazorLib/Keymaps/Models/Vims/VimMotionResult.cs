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
        ITextEditorModel model,
        TextEditorCursorModifier cursorModifier,
        Func<Task> motionCommand)
    {
        var beforeMotionCursor = cursorModifier.ToCursor();

        var beforeMotionPositionIndex = model.GetPositionIndex(
            beforeMotionCursor.RowIndex,
            beforeMotionCursor.ColumnIndex);

        await motionCommand.Invoke().ConfigureAwait(false);

        var afterMotionCursor = cursorModifier.ToCursor();

        var afterMotionPositionIndex = model.GetPositionIndex(
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