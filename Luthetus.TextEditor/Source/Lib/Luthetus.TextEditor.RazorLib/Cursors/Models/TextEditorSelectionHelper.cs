using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.Cursors.Models;

public static class TextEditorSelectionHelper
{
    public static bool HasSelectedText(TextEditorSelection textEditorSelection)
    {
        return HasSelectedText(
            textEditorSelection.AnchorPositionIndex,
            textEditorSelection.EndingPositionIndex);
    }

    public static bool HasSelectedText(TextEditorCursorModifier cursorModifier)
    {
        return HasSelectedText(
            cursorModifier.SelectionAnchorPositionIndex,
            cursorModifier.SelectionEndingPositionIndex);
    }

    public static bool HasSelectedText(int? anchorPositionIndex, int endingPositionIndex)
    {
        if (anchorPositionIndex.HasValue && anchorPositionIndex.Value != endingPositionIndex)
            return true;

        return false;
    }

    public static string? GetSelectedText(
        TextEditorSelection textEditorSelection,
        TextEditorModel textEditorModel)
    {
        return GetSelectedText(
            textEditorSelection.AnchorPositionIndex,
            textEditorSelection.EndingPositionIndex,
            textEditorModel);
    }
    
    public static string? GetSelectedText(
        TextEditorCursorModifier cursorModifier,
        TextEditorModel textEditorModel)
    {
        return GetSelectedText(
            cursorModifier.SelectionAnchorPositionIndex,
            cursorModifier.SelectionEndingPositionIndex,
            textEditorModel);
    }

    public static string? GetSelectedText(
        int? anchorPositionIndex,
        int endingPositionIndex,
        TextEditorModel textEditorModel)
    {
        if (HasSelectedText(anchorPositionIndex, endingPositionIndex))
        {
            var selectionBounds = GetSelectionBounds(anchorPositionIndex, endingPositionIndex);

            var result = textEditorModel.GetTextRange(
                selectionBounds.lowerPositionIndexInclusive,
                selectionBounds.upperPositionIndexExclusive - selectionBounds.lowerPositionIndexInclusive);

            return result.Length != 0 ? result : null;
        }

        return null;
    }

    public static TextEditorCursor SelectLinesRange(
        TextEditorModel textEditorModel,
        int startingRowIndex,
        int count)
    {
        var startingPositionIndexInclusive = textEditorModel.GetPositionIndex(startingRowIndex, 0);

        var lastRowIndexExclusive = startingRowIndex + count;

        var endingPositionIndexExclusive = textEditorModel.GetPositionIndex(lastRowIndexExclusive, 0);

        var columnIndex = 0;
        var textEditorCursor = new TextEditorCursor(
            startingRowIndex,
            columnIndex,
            columnIndex,
            false,
            TextEditorSelection.Empty);

        textEditorCursor.Selection.AnchorPositionIndex = startingPositionIndexInclusive;
        textEditorCursor.Selection.EndingPositionIndex = endingPositionIndexExclusive;

        return textEditorCursor;
    }

    public static (int lowerPositionIndexInclusive, int upperPositionIndexExclusive) GetSelectionBounds(
        TextEditorSelection textEditorSelection)
    {
        return GetSelectionBounds(
            textEditorSelection.AnchorPositionIndex,
            textEditorSelection.EndingPositionIndex);
    }
    
    public static (int lowerPositionIndexInclusive, int upperPositionIndexExclusive) GetSelectionBounds(
        TextEditorCursorModifier cursorModifier)
    {
        return GetSelectionBounds(
            cursorModifier.SelectionAnchorPositionIndex,
            cursorModifier.SelectionEndingPositionIndex);
    }

    public static (int lowerPositionIndexInclusive, int upperPositionIndexExclusive) GetSelectionBounds(
        int? anchorPositionIndex,
        int endingPositionIndex)
    {
        if (anchorPositionIndex is null)
        {
            throw new ApplicationException(
                $"{nameof(anchorPositionIndex)} was null.");
        }

        var lowerPositionIndexInclusive = anchorPositionIndex.Value;
        var upperPositionIndexExclusive = endingPositionIndex;

        if (lowerPositionIndexInclusive > upperPositionIndexExclusive) // Swap the values around
            (lowerPositionIndexInclusive, upperPositionIndexExclusive) = (upperPositionIndexExclusive, lowerPositionIndexInclusive);

        return (lowerPositionIndexInclusive, upperPositionIndexExclusive);
    }

    public static (int lowerRowIndexInclusive, int upperRowIndexExclusive) ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
        TextEditorModel textEditorModel,
        (int lowerPositionIndexInclusive, int upperPositionIndexExclusive) positionIndexBounds)
    {
        var firstRowToSelectDataInclusive = textEditorModel.FindRowInformation(
                positionIndexBounds.lowerPositionIndexInclusive)
            .rowIndex;

        var lastRowToSelectDataExclusive = textEditorModel.FindRowInformation(
                positionIndexBounds.upperPositionIndexExclusive)
            .rowIndex +
            1;

        return (firstRowToSelectDataInclusive, lastRowToSelectDataExclusive);
    }
}