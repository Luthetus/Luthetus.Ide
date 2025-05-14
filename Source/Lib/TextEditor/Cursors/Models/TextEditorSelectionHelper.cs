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

    public static bool HasSelectedText(TextEditorViewModel viewModel)
    {
        return HasSelectedText(
            viewModel.SelectionAnchorPositionIndex,
            viewModel.SelectionEndingPositionIndex);
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
        TextEditorViewModel viewModel,
        TextEditorModel textEditorModel)
    {
        return GetSelectedText(
            viewModel.SelectionAnchorPositionIndex,
            viewModel.SelectionEndingPositionIndex,
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

    public static (int Position_LowerInclusiveIndex, int Position_UpperExclusiveIndex) GetSelectionBounds(TextEditorViewModel viewModel)
    {
    	return GetSelectionBounds(viewModel.SelectionAnchorPositionIndex, viewModel.SelectionEndingPositionIndex);
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

    public static (int Line_LowerInclusiveIndex, int Line_UpperExclusiveIndex) ConvertSelectionOfPositionIndexUnitsToLineIndexUnits(
        TextEditorModel textEditorModel,
        (int Position_LowerInclusiveIndex, int Position_UpperExclusiveIndex) positionIndexBounds)
    {
        var firstLineToSelectDataInclusive = textEditorModel.GetLineInformationFromPositionIndex(
            positionIndexBounds.Position_LowerInclusiveIndex);

        var lastLineToSelectDataExclusive = textEditorModel.GetLineInformationFromPositionIndex(
            positionIndexBounds.Position_UpperExclusiveIndex);

        var upperLineIndexExclusive = lastLineToSelectDataExclusive.Index + 1;

        if (lastLineToSelectDataExclusive.Position_StartInclusiveIndex == positionIndexBounds.Position_UpperExclusiveIndex)
        {
            // If the selection ends at the start of a row, then an extra row
            // will be erroneously indended. This occurs because the '+1' logic
            // is used to ensure partially selected rows still get indented.
            upperLineIndexExclusive--;
        }

        return (
            firstLineToSelectDataInclusive.Index,
            upperLineIndexExclusive);
    }
}