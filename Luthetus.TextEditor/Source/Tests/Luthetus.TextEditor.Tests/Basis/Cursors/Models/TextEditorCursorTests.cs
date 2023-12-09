using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.Tests.Basis.Cursors.Models;

public class TextEditorCursorTests
{
    public TextEditorCursor(bool isPrimaryCursor)
    {
        IsPrimaryCursor = isPrimaryCursor;
    }

    public TextEditorCursor((int rowIndex, int columnIndex) rowAndColumnIndex, bool isPrimaryCursor)
        : this(isPrimaryCursor)
    {
        IndexCoordinates = rowAndColumnIndex;
        PreferredColumnIndex = IndexCoordinates.columnIndex;
    }

    public TextEditorCursor(ImmutableTextEditorCursor immutableTextEditorCursor, bool isPrimaryCursor)
        : this(
            (immutableTextEditorCursor.RowIndex, immutableTextEditorCursor.ColumnIndex),
            isPrimaryCursor)
    {
    }

    public (int rowIndex, int columnIndex) IndexCoordinates { get; set; }
    public int PreferredColumnIndex { get; set; }
    public TextEditorSelection Selection { get; } = new();
    public bool ShouldRevealCursor { get; set; }
    public bool IsPrimaryCursor { get; }
    /// <summary>
    /// Relates to whether the cursor is within the viewable area of the Text Editor on the UI
    /// </summary>
    public bool IsIntersecting { get; set; }

    public static void MoveCursor(
        KeyboardEventArgs keyboardEventArgs,
        TextEditorCursor textEditorCursor,
        TextEditorModel textEditorModel)
    {
        var localIndexCoordinates = textEditorCursor.IndexCoordinates;
        var localPreferredColumnIndex = textEditorCursor.PreferredColumnIndex;

        var rememberTextEditorSelection = new TextEditorSelection
        {
            AnchorPositionIndex = textEditorCursor.Selection.AnchorPositionIndex,
            EndingPositionIndex = textEditorCursor.Selection.EndingPositionIndex,
        };

        void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
        {
            localIndexCoordinates.columnIndex = columnIndex;
            localPreferredColumnIndex = columnIndex;
        }

        if (keyboardEventArgs.ShiftKey)
        {
            if (textEditorCursor.Selection.AnchorPositionIndex is null ||
                textEditorCursor.Selection.EndingPositionIndex == textEditorCursor.Selection.AnchorPositionIndex)
            {
                var positionIndex = textEditorModel.GetPositionIndex(
                    localIndexCoordinates.rowIndex,
                    localIndexCoordinates.columnIndex);

                textEditorCursor.Selection.AnchorPositionIndex = positionIndex;
            }
        }
        else
        {
            textEditorCursor.Selection.AnchorPositionIndex = null;
        }

        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                {
                    if (TextEditorSelectionHelper.HasSelectedText(rememberTextEditorSelection) &&
                        !keyboardEventArgs.ShiftKey)
                    {
                        var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(rememberTextEditorSelection);

                        var lowerRowMetaData = textEditorModel.FindRowInformation(
                            selectionBounds.lowerPositionIndexInclusive);

                        localIndexCoordinates.rowIndex = lowerRowMetaData.rowIndex;

                        localIndexCoordinates.columnIndex =
                            selectionBounds.lowerPositionIndexInclusive - lowerRowMetaData.rowStartPositionIndex;
                    }
                    else
                    {
                        if (localIndexCoordinates.columnIndex == 0)
                        {
                            if (localIndexCoordinates.rowIndex != 0)
                            {
                                localIndexCoordinates.rowIndex--;

                                var lengthOfRow = textEditorModel.GetLengthOfRow(localIndexCoordinates.rowIndex);

                                MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                            }
                        }
                        else
                        {
                            if (keyboardEventArgs.CtrlKey)
                            {
                                var columnIndexOfCharacterWithDifferingKind = textEditorModel.GetColumnIndexOfCharacterWithDifferingKind(
                                    localIndexCoordinates.rowIndex,
                                    localIndexCoordinates.columnIndex,
                                    true);

                                if (columnIndexOfCharacterWithDifferingKind == -1)
                                    MutateIndexCoordinatesAndPreferredColumnIndex(0);
                                else
                                    MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);
                            }
                            else
                            {
                                MutateIndexCoordinatesAndPreferredColumnIndex(localIndexCoordinates.columnIndex - 1);
                            }
                        }
                    }

                    break;
                }
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                {
                    if (localIndexCoordinates.rowIndex < textEditorModel.RowCount - 1)
                    {
                        localIndexCoordinates.rowIndex++;

                        var lengthOfRow = textEditorModel.GetLengthOfRow(localIndexCoordinates.rowIndex);

                        localIndexCoordinates.columnIndex = lengthOfRow < localPreferredColumnIndex
                            ? lengthOfRow
                            : localPreferredColumnIndex;
                    }

                    break;
                }
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                {
                    if (localIndexCoordinates.rowIndex > 0)
                    {
                        localIndexCoordinates.rowIndex--;

                        var lengthOfRow = textEditorModel.GetLengthOfRow(localIndexCoordinates.rowIndex);

                        localIndexCoordinates.columnIndex = lengthOfRow < localPreferredColumnIndex
                            ? lengthOfRow
                            : localPreferredColumnIndex;
                    }

                    break;
                }
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                {
                    if (TextEditorSelectionHelper.HasSelectedText(rememberTextEditorSelection) && !keyboardEventArgs.ShiftKey)
                    {
                        var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(rememberTextEditorSelection);

                        var upperRowMetaData = textEditorModel.FindRowInformation(selectionBounds.upperPositionIndexExclusive);

                        localIndexCoordinates.rowIndex = upperRowMetaData.rowIndex;

                        if (localIndexCoordinates.rowIndex >= textEditorModel.RowCount)
                        {
                            localIndexCoordinates.rowIndex = textEditorModel.RowCount - 1;

                            var upperRowLength = textEditorModel.GetLengthOfRow(localIndexCoordinates.rowIndex);

                            localIndexCoordinates.columnIndex = upperRowLength;
                        }
                        else
                        {
                            localIndexCoordinates.columnIndex =
                                selectionBounds.upperPositionIndexExclusive - upperRowMetaData.rowStartPositionIndex;
                        }
                    }
                    else
                    {
                        var lengthOfRow = textEditorModel.GetLengthOfRow(localIndexCoordinates.rowIndex);

                        if (localIndexCoordinates.columnIndex == lengthOfRow &&
                            localIndexCoordinates.rowIndex < textEditorModel.RowCount - 1)
                        {
                            MutateIndexCoordinatesAndPreferredColumnIndex(0);
                            localIndexCoordinates.rowIndex++;
                        }
                        else if (localIndexCoordinates.columnIndex != lengthOfRow)
                        {
                            if (keyboardEventArgs.CtrlKey)
                            {
                                var columnIndexOfCharacterWithDifferingKind = textEditorModel.GetColumnIndexOfCharacterWithDifferingKind(
                                    localIndexCoordinates.rowIndex,
                                    localIndexCoordinates.columnIndex,
                                    false);

                                if (columnIndexOfCharacterWithDifferingKind == -1)
                                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                                else
                                {
                                    MutateIndexCoordinatesAndPreferredColumnIndex(
                                        columnIndexOfCharacterWithDifferingKind);
                                }
                            }
                            else
                            {
                                MutateIndexCoordinatesAndPreferredColumnIndex(localIndexCoordinates.columnIndex + 1);
                            }
                        }
                    }

                    break;
                }
            case KeyboardKeyFacts.MovementKeys.HOME:
                {
                    if (keyboardEventArgs.CtrlKey)
                        localIndexCoordinates.rowIndex = 0;

                    MutateIndexCoordinatesAndPreferredColumnIndex(0);

                    break;
                }
            case KeyboardKeyFacts.MovementKeys.END:
                {
                    if (keyboardEventArgs.CtrlKey)
                        localIndexCoordinates.rowIndex = textEditorModel.RowCount - 1;

                    var lengthOfRow = textEditorModel.GetLengthOfRow(localIndexCoordinates.rowIndex);

                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);

                    break;
                }
        }

        textEditorCursor.IndexCoordinates = localIndexCoordinates;
        textEditorCursor.PreferredColumnIndex = localPreferredColumnIndex;

        if (keyboardEventArgs.ShiftKey)
        {
            var positionIndex = textEditorModel.GetPositionIndex(
                localIndexCoordinates.rowIndex,
                localIndexCoordinates.columnIndex);

            textEditorCursor.Selection.EndingPositionIndex = positionIndex;
        }
    }
}