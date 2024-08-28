using System.Text;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// (2024-06-08) I've been dogfooding the IDE, and the 'TextEditorModelModifier.cs' file demonstrates some clear issues regarding text editor optimization.
///              Im breaking up the 80,000 character file a bit here into partial classes for now. TODO: merge the partial classes back?
/// </summary>
public partial class TextEditorModelModifier
{
	/// <param name="useLineEndKindPreference">
    /// If false, then the string will be inserted as is.
    /// If true, then the string will have its line endings replaced with the <see cref="LineEndKindPreference"/>
    /// </param>
    public void Insert(
        string value,
        CursorModifierBagTextEditor cursorModifierBag,
        bool useLineEndKindPreference = true,
        CancellationToken cancellationToken = default,
		bool shouldCreateEditHistory = true)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        for (var cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
        {
            var cursorModifier = cursorModifierBag.List[cursorIndex];

            if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
            {
                Delete(
					// TODO: 'cursorModifierBag' is not the correcy parameter here...
					//       ...one needs to create a new cursor modifier bag which contains the single cursor that is being looked at. 
                    cursorModifierBag,
                    1,
                    false,
                    DeleteKind.Delete,
                    CancellationToken.None);
            }

            {
                // TODO: If one inserts a carriage return character,
                //       meanwhile the text editor model happens to have a line feed character at the position
                //       you are inserting at.
                //       |
                //       Then, the '\r' takes the position of the '\n', and the '\n' is shifted further
                //       by 1 position in order to allow space the '\r'.
                //       |
                //       Well, now the text editor model sees its contents as "\r\n".
                //       What is to be done in this scenario?
                //       (2024-04-22)
            }

            // Remember the cursorPositionIndex
            var initialCursorPositionIndex = this.GetPositionIndex(cursorModifier);

            // Track metadata with the cursorModifier itself
            //
            // Metadata must be done prior to 'InsertValue'
            //
            // 'value' is replaced by the original with any line endings changed (based on 'useLineEndKindPreference').
            value = InsertMetadata(value, cursorModifier, useLineEndKindPreference, cancellationToken);

            // Now the text still needs to be inserted.
            // The cursorModifier is invalid, because the metadata step moved its position.
            // So, use the 'cursorPositionIndex' variable that was calculated prior to the metadata step.
            InsertValue(value, initialCursorPositionIndex, useLineEndKindPreference, cancellationToken);

			if (shouldCreateEditHistory)
				EnsureUndoPoint(new TextEditorEditInsert(initialCursorPositionIndex, value));

            // NOTE: One cannot obtain the 'MostCharactersOnASingleLineTuple' from within the 'InsertMetadata(...)'
            //       method because this specific metadata is being calculated by counting the characters, which
            //       in the case of 'InsertMetadata(...)' wouldn't have been inserted yet.
            //
            // TODO: Fix tracking the MostCharactersOnASingleRowTuple this way is possibly inefficient - should instead only check the rows that changed
            {
                (int rowIndex, int rowLength) localMostCharactersOnASingleRowTuple = (0, 0);

                for (var i = 0; i < LineEndList.Count; i++)
                {
                    var lengthOfRow = this.GetLineLength(i);

                    if (lengthOfRow > localMostCharactersOnASingleRowTuple.rowLength)
                        localMostCharactersOnASingleRowTuple = (i, lengthOfRow);
                }

                localMostCharactersOnASingleRowTuple = (localMostCharactersOnASingleRowTuple.rowIndex,
                    localMostCharactersOnASingleRowTuple.rowLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                _mostCharactersOnASingleLineTuple = localMostCharactersOnASingleRowTuple;
            }
        }

        SetIsDirtyTrue();
		ShouldReloadVirtualizationResult = true;
    }

	private string InsertMetadata(
        string value,
        TextEditorCursorModifier cursorModifier,
        bool useLineEndKindPreference,
        CancellationToken cancellationToken)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        var initialCursorPositionIndex = this.GetPositionIndex(cursorModifier);
        var initialCursorLineIndex = cursorModifier.LineIndex;

#if DEBUG
        this.AssertPositionIndex(initialCursorPositionIndex);
#endif

        bool isTab = false;
        bool isCarriageReturn = false;
        bool isLineFeed = false;
        bool isCarriageReturnLineFeed = false;

        (int? index, List<LineEnd> localLineEndList) lineEndPositionLazyInsertRange = (null, new());
        (int? index, List<int> localTabPositionList) tabPositionLazyInsertRange = (null, new());

        var lineEndingsChangedValueBuilder = new StringBuilder();

        for (int charIndex = 0; charIndex < value.Length; charIndex++)
        {
            var charValue = value[charIndex];

            isTab = charValue == '\t';
            isCarriageReturn = charValue == '\r';
            isLineFeed = charValue == '\n';
            // The CRLF boolean must be checked prior to CR, as one is a "substring" of the other
            isCarriageReturnLineFeed = isCarriageReturn && charIndex != value.Length - 1 && value[1 + charIndex] == '\n';

            {
                // TODO: If one inserts a carriage return character, meanwhile the text editor model
                //       happens to have a line feed character at the position you are inserting at.
                //       |
                //       Then, the '\r' takes the position of the '\n', and the '\n' is shifted further
                //       by 1 position in order to allow space the '\r'.
                //       |
                //       Well, now the text editor model sees its contents as "\r\n".
                //       What is to be done in this scenario?
                //       (2024-04-22)
            }

            if (isLineFeed || isCarriageReturn || isCarriageReturnLineFeed)
            {
                // Regardless of which line ending is used, since the source text
                // is CRLF, one must increment the for loop one character further.
                if (isCarriageReturnLineFeed)
                    charIndex++;

                LineEndKind lineEndKind;
                
                if (useLineEndKindPreference)
                {
                    lineEndKind = LineEndKindPreference;
                }
                else
                {
                    lineEndKind =
                        // CRLF must be checked prior to CR, as one is a "substring" of the other.
                        isCarriageReturnLineFeed ? LineEndKind.CarriageReturnLineFeed :
                        isCarriageReturn ? LineEndKind.CarriageReturn :
                        isLineFeed ? LineEndKind.LineFeed :
                        LineEndKindPreference;
                }

                // The LineEndKindPreference can invalidate the booleans
                //
                // Additionally, by clearing all the booleans and then setting only one of them,
                //
                //     -"CRLF must be checked prior to CR, as one is a "substring" of the other"
                //
                // can be avoided.
                {
                    isCarriageReturnLineFeed = false;
                    isCarriageReturn = false;
                    isLineFeed = false;

                    if (lineEndKind == LineEndKind.CarriageReturnLineFeed)
                        isCarriageReturnLineFeed = true;
                    else if (lineEndKind == LineEndKind.CarriageReturn)
                        isCarriageReturn = true;
                    else if (lineEndKind == LineEndKind.LineFeed)
                        isLineFeed = true;
                }

                lineEndPositionLazyInsertRange.index ??= cursorModifier.LineIndex;

                var lineEndCharacters = lineEndKind.AsCharacters();

                lineEndPositionLazyInsertRange.localLineEndList.Add(new LineEnd(
                    initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length,
                    lineEndCharacters.Length + initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length,
                    lineEndKind));

                lineEndingsChangedValueBuilder.Append(lineEndCharacters);

                MutateLineEndKindCount(lineEndKind, 1);

                cursorModifier.LineIndex++;
                cursorModifier.SetColumnIndexAndPreferred(0);
            }
            else
            {
                if (isTab)
                {
                    if (tabPositionLazyInsertRange.index is null)
                    {
                        tabPositionLazyInsertRange.index = _tabKeyPositionsList.FindIndex(x => x >= initialCursorPositionIndex);

                        if (tabPositionLazyInsertRange.index == -1)
                            tabPositionLazyInsertRange.index = _tabKeyPositionsList.Count;
                    }

                    tabPositionLazyInsertRange.localTabPositionList.Add(initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length);
                }

                lineEndingsChangedValueBuilder.Append(charValue);
                cursorModifier.SetColumnIndexAndPreferred(1 + cursorModifier.ColumnIndex);
            }
        }

        // Reposition the Row Endings
        {
            for (var i = initialCursorLineIndex; i < LineEndList.Count; i++)
            {
                var rowEndingTuple = LineEndList[i];

                LineEndList[i] = rowEndingTuple with
                {
                    StartPositionIndexInclusive = rowEndingTuple.StartPositionIndexInclusive + lineEndingsChangedValueBuilder.Length,
                    EndPositionIndexExclusive = rowEndingTuple.EndPositionIndexExclusive + lineEndingsChangedValueBuilder.Length,
                };
            }
        }

        // Reposition the Tabs
        {
            var firstTabKeyPositionIndexToModify = _tabKeyPositionsList.FindIndex(x => x >= initialCursorPositionIndex);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < TabKeyPositionList.Count; i++)
                {
                    TabKeyPositionList[i] += lineEndingsChangedValueBuilder.Length;
                }
            }
        }

        // Reposition the Diagnostic Squigglies
        {
            var textSpanForInsertion = new TextEditorTextSpan(
                initialCursorPositionIndex,
                initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length,
                0,
                ResourceUri.Empty,
                string.Empty);

            var textModification = new TextEditorTextModification(true, textSpanForInsertion);

            foreach (var presentationModel in PresentationModelList)
            {
                presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
            }
        }

        // Add in any new metadata
        {
            if (lineEndPositionLazyInsertRange.index is not null)
            {
                _lineEndList.InsertRange(
                    lineEndPositionLazyInsertRange.index.Value,
                    lineEndPositionLazyInsertRange.localLineEndList);
            }

            if (tabPositionLazyInsertRange.index is not null)
            {
                _tabKeyPositionsList.InsertRange(
                    tabPositionLazyInsertRange.index.Value,
                    tabPositionLazyInsertRange.localTabPositionList);
            }
        }

        return lineEndingsChangedValueBuilder.ToString();
    }

	private void InsertValue(
        string value,
        int cursorPositionIndex,
        bool useLineEndKindPreference,
        CancellationToken cancellationToken)
    {
        // If cursor is out of bounds then continue
        if (cursorPositionIndex > _richCharacterList.Length)
            return;

        __InsertRange(
            cursorPositionIndex,
            value.Select(character => new RichCharacter(character, 0)));
    }

	/// <summary>
    /// This method allows for a "RemoveRange" like operation on the text editor's contents.
    /// Any meta-data will automatically be updated (e.g. <see cref="ITextEditorModel.LineEndKindCountList"/>.
    /// </summary>
    /// <param name="cursorModifierBag">
    /// The list of cursors that indicate the positionIndex to start a "RemoveRange" operation.
    /// The cursors are iterated backwards, with each cursor being its own "RemoveRange" operation.
    /// </param>
    /// <param name="columnCount">
    /// The amount of columns to delete. If a the value of a column is of 2-char length (e.g. "\r\n"),
    /// then internally this columnCount will be converted to a 'charCount' of the corrected length.
    /// </param>
    /// <param name="expandWord">
    /// Applied after moving by the 'count' parameter.<br/>
    /// Ex:
    ///     count of 1, and expandWord of true;
    ///     will move 1 char-value from the initialPositionIndex.
    ///     Afterwards, if expandWord is true, then the cursor is checked to be within a word, or at the start or end of one.
    ///     If the cursor is at the start or end of one, then the selection to delete is expanded such that it contains
    ///     the entire word that the cursor ended at.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <param name="deleteKind">
    /// The default <see cref="DeleteKind.Delete"/> will do logic similar to a "RemoveRange" like
    /// operation on a collection type.<br/>
    /// If one has keyboard input from a user, one might want to have the user's backspace key
    /// pass in the <see cref="DeleteKind.Backspace"/> parameter.
    /// Then, for a user's delete key, pass in <see cref="DeleteKind.Delete"/>.
    /// </param>
	public void Delete(
        CursorModifierBagTextEditor cursorModifierBag,
        int columnCount,
        bool expandWord,
        DeleteKind deleteKind,
        CancellationToken cancellationToken = default,
		bool shouldCreateEditHistory = true)
	{
        if (columnCount < 0)
            throw new LuthetusTextEditorException($"{nameof(columnCount)} < 0");

        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
		}
	
	        for (var cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
	        {
	            var cursorModifier = cursorModifierBag.List[cursorIndex];

				var initialPositionIndex = this.GetPositionIndex(cursorModifier);
	
	            var tuple = DeleteMetadata(columnCount, cursorModifier, expandWord, deleteKind, cancellationToken);
	
	            if (tuple is null)
	            {
	                SetIsDirtyTrue();
	                return;
				}

            var (positionIndex, charCount) = tuple.Value;

			var textRemoved = this.GetString(positionIndex, charCount);

            DeleteValue(positionIndex, charCount, cancellationToken);

			if (shouldCreateEditHistory)
			{
				if (deleteKind == DeleteKind.Delete)
					EnsureUndoPoint(new TextEditorEditDelete(positionIndex, charCount) { TextRemoved = textRemoved });
				else if (deleteKind == DeleteKind.Backspace)
					EnsureUndoPoint(new TextEditorEditBackspace(initialPositionIndex, charCount) { TextRemoved = textRemoved });
				else
					throw new NotImplementedException($"The {nameof(DeleteKind)}: {deleteKind} was not recognized.");
			}

            // NOTE: One cannot obtain the 'MostCharactersOnASingleLineTuple' from within the 'DeleteMetadata(...)'
            //       method because this specific metadata is being calculated by counting the characters, which
            //       in the case of 'DeleteMetadata(...)' wouldn't have been deleted yet.
            //
            // TODO: Fix tracking the MostCharactersOnASingleLineTuple this way is possibly inefficient - should instead only check the rows that changed
            {
                (int lineIndex, int lineLength) localMostCharactersOnASingleLineTuple = (0, 0);

                for (var i = 0; i < LineEndList.Count; i++)
                {
                    var lengthOfLine = this.GetLineLength(i);

                    if (lengthOfLine > localMostCharactersOnASingleLineTuple.lineLength)
                    {
                        localMostCharactersOnASingleLineTuple = (i, lengthOfLine);
                    }
				
                }

                localMostCharactersOnASingleLineTuple = (
                    localMostCharactersOnASingleLineTuple.lineIndex,
                    localMostCharactersOnASingleLineTuple.lineLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                _mostCharactersOnASingleLineTuple = localMostCharactersOnASingleLineTuple;
            }
        }

        SetIsDirtyTrue();
		ShouldReloadVirtualizationResult = true;
    }

	/// <summary>
    /// The text editor sees "\r\n" as 1 character, even though that is made up of 2 char values.
    /// The <see cref="TextEditorPartition"/> however, sees "\r\n" as 2 char values.<br/><br/>
    /// 
    /// This different means, to delete "\r\n" one tells the text editor to delete 1 character,
    /// where as one tells the <see cref="TextEditorPartition"/> to delete 2 char values.<br/><br/>
    /// 
    /// This method returns the 'int charValueCount', so that it can be used
    /// in the <see cref="DeleteValue(int, int, CancellationToken)"/> method.
    /// </summary>
	private (int positionIndex, int charCount)? DeleteMetadata(
        int columnCount,
        TextEditorCursorModifier cursorModifier,
        bool expandWord,
        DeleteKind deleteKind,
        CancellationToken cancellationToken)
	{
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        var initiallyHadSelection = TextEditorSelectionHelper.HasSelectedText(cursorModifier);
        var initialLineIndex = cursorModifier.LineIndex;
        var positionIndex = this.GetPositionIndex(cursorModifier);

        if (initiallyHadSelection && cursorModifier.SelectionAnchorPositionIndex is not null)
        {
            // If user's cursor has a selection, then set the variables so the positionIndex is the
            // selection.AnchorPositionIndex and the count is selection.EndPositionIndex - selection.AnchorPositionIndex
            // and that the 'DeleteKind.Delete' logic runs.
            var (lowerPositionIndexInclusive, upperPositionIndexExclusive) = TextEditorSelectionHelper.GetSelectionBounds(cursorModifier);

            var lowerLineData = this.GetLineInformationFromPositionIndex(lowerPositionIndexInclusive);
            var lowerColumnIndex = lowerPositionIndexInclusive - lowerLineData.StartPositionIndexInclusive;

            cursorModifier.LineIndex = lowerLineData.Index;
            initialLineIndex = cursorModifier.LineIndex;
            cursorModifier.SetColumnIndexAndPreferred(lowerColumnIndex);
            positionIndex = lowerPositionIndexInclusive;

            // The deletion of a selection logic does not check for multibyte characters.
            // Therefore, later in this method, if a multibyte character is found, the columnCount must be reduced. (2024-05-01)
            columnCount = upperPositionIndexExclusive - lowerPositionIndexInclusive;
            deleteKind = DeleteKind.Delete;

            cursorModifier.SelectionAnchorPositionIndex = null;
            cursorModifier.SelectionEndingPositionIndex = 0;
		}

#if DEBUG
        this.AssertPositionIndex(positionIndex);
#endif

        (int? index, int count) lineEndPositionLazyRemoveRange = (null, 0);
        (int? index, int count) tabPositionLazyRemoveRange = (null, 0);

        var charCount = 0;

        if (deleteKind == DeleteKind.Delete)
        {
            if (expandWord && !initiallyHadSelection)
            {
                var columnIndexOfCharacterWithDifferingKind = this.GetColumnIndexOfCharacterWithDifferingKind(
                    cursorModifier.LineIndex,
                    cursorModifier.ColumnIndex,
                    false);

                // -1 implies that no differing kind was found on the current line.
                if (columnIndexOfCharacterWithDifferingKind == -1)
                {
                    var line = this.GetLineInformation(cursorModifier.LineIndex);
                    columnIndexOfCharacterWithDifferingKind = line.LastValidColumnIndex;
                }

                columnCount = columnIndexOfCharacterWithDifferingKind - cursorModifier.ColumnIndex;

                // Cursor is at the start of a row
                if (columnCount == 0)
                    columnCount = 1;
			}

            for (int i = 0; i < columnCount; i++)
            {
                var toDeletePositionIndex = positionIndex + charCount;
                if (toDeletePositionIndex < 0 || toDeletePositionIndex >= CharCount)
                    break;

                var richCharacterToDelete = _richCharacterList[toDeletePositionIndex];

                if (KeyboardKeyFacts.IsLineEndingCharacter(richCharacterToDelete.Value))
                {
                    // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndList
                    // is a starting index, and a count.
                    var indexLineEnd = _lineEndList.FindIndex(
                        x => x.StartPositionIndexInclusive == toDeletePositionIndex);

                    var lineEnd = LineEndList[indexLineEnd];

                    // Delete starts at the lowest index, therefore use '??=' to only assign once.
                    lineEndPositionLazyRemoveRange.index ??= indexLineEnd;
                    lineEndPositionLazyRemoveRange.count++;

                    var lengthOfLineEnd = LineEndList[indexLineEnd].LineEndKind.AsCharacters().Length;
                    charCount += lengthOfLineEnd;

                    MutateLineEndKindCount(lineEnd.LineEndKind, -1);

                    if (lineEnd.LineEndKind == LineEndKind.CarriageReturnLineFeed && initiallyHadSelection)
                    {
                        // The deletion of a selection logic does not check for multibyte characters.
                        // Therefore, if a multibyte character is found, the columnCount must be reduced. (2024-05-01)
                        columnCount--;
                    }
                }
				else
                {
                    charCount++;

                    if (richCharacterToDelete.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                    {
                        var indexTabKey = _tabKeyPositionsList.FindIndex(
                            x => x == toDeletePositionIndex);

                        // Delete starts at the lowest index, therefore use '??=' to only assign once.
                        tabPositionLazyRemoveRange.index ??= indexTabKey;
                        tabPositionLazyRemoveRange.count++;
                    }
                }
            }
        }
		else if (deleteKind == DeleteKind.Backspace)
        {
            if (expandWord && !initiallyHadSelection)
            {
                var columnIndexOfCharacterWithDifferingKind = this.GetColumnIndexOfCharacterWithDifferingKind(
                    cursorModifier.LineIndex,
                    cursorModifier.ColumnIndex,
                    true);

                // -1 implies that no differing kind was found on the current line.
                if (columnIndexOfCharacterWithDifferingKind == -1)
                    columnIndexOfCharacterWithDifferingKind = 0;

                columnCount = cursorModifier.ColumnIndex - columnIndexOfCharacterWithDifferingKind;

                // Cursor is at the start of a row
                if (columnCount == 0)
                    columnCount = 1;
			}

            for (int i = 0; i < columnCount; i++)
            {
                // Minus 1 here because 'Backspace' deletes the previous character.
                var toDeletePositionIndex = positionIndex - charCount - 1;
                if (toDeletePositionIndex < 0 || toDeletePositionIndex >= CharCount)
                    break;

                var richCharacterToDelete = _richCharacterList[toDeletePositionIndex];

                if (KeyboardKeyFacts.IsLineEndingCharacter(richCharacterToDelete.Value))
                {
                    // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndList
                    // is a starting index, and a count.
                    var indexLineEnd = _lineEndList.FindIndex(
                        // Check for '\n' or '\r'
                        x => x.EndPositionIndexExclusive == toDeletePositionIndex + 1 ||
                        // Check for "\r\n"
                        x.EndPositionIndexExclusive == toDeletePositionIndex + 2);

                    var lineEnd = LineEndList[indexLineEnd];

                    // Backspace starts at the highest index, therefore use '=' to only assign everytime.
                    lineEndPositionLazyRemoveRange.index = indexLineEnd;
                    lineEndPositionLazyRemoveRange.count++;

                    var lengthOfLineEnd = LineEndList[indexLineEnd].LineEndKind.AsCharacters().Length;
                    charCount += lengthOfLineEnd;

                    MutateLineEndKindCount(lineEnd.LineEndKind, -1);
                }
                else
                {
                    charCount++;

                    if (richCharacterToDelete.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                    {
                        var indexTabKey = _tabKeyPositionsList.FindIndex(
                            x => x == toDeletePositionIndex);

                        // Backspace starts at the highest index, therefore use '=' to only assign everytime.
                        tabPositionLazyRemoveRange.index = indexTabKey;
                        tabPositionLazyRemoveRange.count++;
                    }
                }
            }
		}
        else
        {
            throw new NotImplementedException();
		}

        // Reposition the LineEnd(s)
        {
            for (var i = initialLineIndex; i < LineEndList.Count; i++)
            {
                var lineEnd = LineEndList[i];

                LineEndList[i] = lineEnd with
                {
                    StartPositionIndexInclusive = lineEnd.StartPositionIndexInclusive - charCount,
                    EndPositionIndexExclusive = lineEnd.EndPositionIndexExclusive - charCount,
                };
            }
        }

        // Reposition the Tab(s)
        {
            var firstTabKeyPositionIndexToModify = _tabKeyPositionsList.FindIndex(x => x >= positionIndex);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < TabKeyPositionList.Count; i++)
                {
                    TabKeyPositionList[i] -= charCount;
                }
            }
        }

		// Reposition the PresentationModel(s)
        {
            var textSpanForInsertion = new TextEditorTextSpan(
                positionIndex,
                positionIndex + charCount,
                0,
                ResourceUri.Empty,
                string.Empty);

            var textModification = new TextEditorTextModification(false, textSpanForInsertion);

            foreach (var presentationModel in PresentationModelList)
            {
                presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
            }
        }

		// Delete metadata
        {
            if (lineEndPositionLazyRemoveRange.index is not null)
            {
                _lineEndList.RemoveRange(
                    lineEndPositionLazyRemoveRange.index.Value,
                    lineEndPositionLazyRemoveRange.count);
            }

            if (tabPositionLazyRemoveRange.index is not null)
            {
                _tabKeyPositionsList.RemoveRange(
                    tabPositionLazyRemoveRange.index.Value,
                    tabPositionLazyRemoveRange.count);
            }
        }

		if (deleteKind == DeleteKind.Delete)
        {
            // Reposition the cursor
            {
                var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(positionIndex);
                cursorModifier.LineIndex = lineIndex;
                cursorModifier.SetColumnIndexAndPreferred(columnIndex);
            }

            return (positionIndex, charCount);
        }
        else if (deleteKind == DeleteKind.Backspace)
        {
            var calculatedPositionIndex = positionIndex - charCount;

            // Reposition the cursor
            {
                var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(calculatedPositionIndex);
                cursorModifier.LineIndex = lineIndex;
                cursorModifier.SetColumnIndexAndPreferred(columnIndex);
            }

            return (calculatedPositionIndex, charCount);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

	private void DeleteValue(int positionIndex, int count, CancellationToken cancellationToken)
    {
        // If cursor is out of bounds then continue
        if (positionIndex >= _richCharacterList.Length)
            return;

        __RemoveRange(positionIndex, count);
	}
}
