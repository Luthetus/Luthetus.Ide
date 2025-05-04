using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

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

    public static bool HasSelectedText(int anchorPositionIndex, int endingPositionIndex)
    {
        if (anchorPositionIndex != -1 && anchorPositionIndex != endingPositionIndex)
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
        int anchorPositionIndex,
        int endingPositionIndex,
        TextEditorModel textEditorModel)
    {
        if (HasSelectedText(anchorPositionIndex, endingPositionIndex))
        {
            var selectionBounds = GetSelectionBounds(anchorPositionIndex, endingPositionIndex);

            var result = textEditorModel.GetString(
                selectionBounds.Position_LowerInclusiveIndex,
                selectionBounds.Position_UpperExclusiveIndex - selectionBounds.Position_LowerInclusiveIndex);

            return result.Length != 0 ? result : null;
        }

        return null;
    }

    public static TextEditorCursor SelectLinesRange(
        TextEditorModel textEditorModel,
        int startingRowIndex,
        int count)
    {
        throw new NotImplementedException("TODO: (2023-12-13) Writing immutability for text editor");
    }

    public static (int Position_LowerInclusiveIndex, int Position_UpperExclusiveIndex) GetSelectionBounds(
        TextEditorSelection textEditorSelection)
    {
        return GetSelectionBounds(
            textEditorSelection.AnchorPositionIndex,
            textEditorSelection.EndingPositionIndex);
    }
    
    public static (int Position_LowerInclusiveIndex, int Position_UpperExclusiveIndex) GetSelectionBounds(
        TextEditorCursorModifier cursorModifier)
    {
        return GetSelectionBounds(
            cursorModifier.SelectionAnchorPositionIndex,
            cursorModifier.SelectionEndingPositionIndex);
    }

    public static (int Position_LowerInclusiveIndex, int Position_UpperExclusiveIndex) GetSelectionBounds(
        int anchorPositionIndex,
        int endingPositionIndex)
    {
        if (anchorPositionIndex == -1)
        {
            throw new LuthetusTextEditorException(
                $"{nameof(anchorPositionIndex)} was null.");
        }

        var positionLowerInclusiveIndex = anchorPositionIndex;
        var positionUpperExclusiveIndex = endingPositionIndex;

        if (positionLowerInclusiveIndex > positionUpperExclusiveIndex) // Swap the values around
            (positionLowerInclusiveIndex, positionUpperExclusiveIndex) = (positionUpperExclusiveIndex, positionLowerInclusiveIndex);

        return (positionLowerInclusiveIndex, positionUpperExclusiveIndex);
    }

    public static (int Row_LowerInclusiveIndex, int Row_UpperExclusiveIndex) ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
        TextEditorModel textEditorModel,
        (int Position_LowerInclusiveIndex, int Position_UpperExclusiveIndex) positionIndexBounds)
    {
        var firstRowToSelectDataInclusive = textEditorModel.GetLineInformationFromPositionIndex(
            positionIndexBounds.Position_LowerInclusiveIndex);

        var lastRowToSelectDataExclusive = textEditorModel.GetLineInformationFromPositionIndex(
            positionIndexBounds.Position_UpperExclusiveIndex);

        var upperRowIndexExclusive = lastRowToSelectDataExclusive.Index + 1;

        if (lastRowToSelectDataExclusive.Position_StartInclusiveIndex == positionIndexBounds.Position_UpperExclusiveIndex)
        {
            // If the selection ends at the start of a row, then an extra row
            // will be erroneously indended. This occurs because the '+1' logic
            // is used to ensure partially selected rows still get indented.
            upperRowIndexExclusive--;
        }

        return (
            firstRowToSelectDataInclusive.Index,
            upperRowIndexExclusive);
    }
}