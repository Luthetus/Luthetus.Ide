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
        ITextEditorModel textEditorModel)
    {
        return GetSelectedText(
            textEditorSelection.AnchorPositionIndex,
            textEditorSelection.EndingPositionIndex,
            textEditorModel);
    }
    
    public static string? GetSelectedText(
        TextEditorCursorModifier cursorModifier,
        ITextEditorModel textEditorModel)
    {
        return GetSelectedText(
            cursorModifier.SelectionAnchorPositionIndex,
            cursorModifier.SelectionEndingPositionIndex,
            textEditorModel);
    }

    public static string? GetSelectedText(
        int? anchorPositionIndex,
        int endingPositionIndex,
        ITextEditorModel textEditorModel)
    {
        if (HasSelectedText(anchorPositionIndex, endingPositionIndex))
        {
            var selectionBounds = GetSelectionBounds(anchorPositionIndex, endingPositionIndex);

            var result = textEditorModel.GetString(
                selectionBounds.lowerPositionIndexInclusive,
                selectionBounds.upperPositionIndexExclusive - selectionBounds.lowerPositionIndexInclusive);

            return result.Length != 0 ? result : null;
        }

        return null;
    }

    public static TextEditorCursor SelectLinesRange(
        ITextEditorModel textEditorModel,
        int startingRowIndex,
        int count)
    {
        var startingPositionIndexInclusive = textEditorModel.GetPositionIndex(startingRowIndex, 0);

        var lastRowIndexExclusive = startingRowIndex + count;

        var endingPositionIndexExclusive = textEditorModel.GetPositionIndex(lastRowIndexExclusive, 0);

        var columnIndex = 0;
        // TODO: (2023-12-13) Writing immutability for text editor
        //
        //var textEditorCursor = new TextEditorCursor(
        //    startingRowIndex,
        //    columnIndex,
        //    columnIndex,
        //    false,
        //    TextEditorSelection.Empty);

        //textEditorCursor.Selection.AnchorPositionIndex = startingPositionIndexInclusive;
        //textEditorCursor.Selection.EndingPositionIndex = endingPositionIndexExclusive;

        return TextEditorCursor.Empty; // TODO: (2023-12-13) Writing immutability for text editor
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
        ITextEditorModel textEditorModel,
        (int lowerPositionIndexInclusive, int upperPositionIndexExclusive) positionIndexBounds)
    {
        var firstRowToSelectDataInclusive = textEditorModel.GetRowInformationFromPositionIndex(
            positionIndexBounds.lowerPositionIndexInclusive);

        var lastRowToSelectDataExclusive = textEditorModel.GetRowInformationFromPositionIndex(
            positionIndexBounds.upperPositionIndexExclusive);

        var upperRowIndexExclusive = lastRowToSelectDataExclusive.RowIndex + 1;

        if (lastRowToSelectDataExclusive.RowStartPositionIndexInclusive ==
                positionIndexBounds.upperPositionIndexExclusive)
        {
            // If the selection ends at the start of a row, then an extra row
            // will be erroneously indended. This occurs because the '+1' logic
            // is used to ensure partially selected rows still get indented.
            upperRowIndexExclusive--;
        }

        return (
            firstRowToSelectDataInclusive.RowIndex,
            upperRowIndexExclusive);
    }
}