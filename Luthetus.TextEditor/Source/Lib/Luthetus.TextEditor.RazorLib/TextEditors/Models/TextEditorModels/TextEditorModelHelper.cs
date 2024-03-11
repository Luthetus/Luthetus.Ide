using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public static class TextEditorModelHelper
{
	/// <summary>
	/// This returns the position index of the line-ending character which created the row in question.
	/// To get the first character on the row, add 1 to the position index.
	/// <br/><br/>
	/// The cursor is a separate Blazor Component and at times will try to access out of bounds locations.
	/// <br/><br/>
	/// When cursor accesses out of bounds location return the final RowIndex, and that row's final ColumnIndex
	/// </summary>
	public static RowEnding GetRowEndingThatCreatedRow(
		this ITextEditorModel model, int rowIndex)
	{
		if (rowIndex > model.RowEndingPositionsList.Count - 1)
			rowIndex = model.RowEndingPositionsList.Count - 1;

		if (rowIndex > 0)
			return model.RowEndingPositionsList[rowIndex - 1];

		return new(0, 0, RowEndingKind.StartOfFile);
	}

	/// <summary>Returns the Length of a row however it does not include the line ending characters by default. To include line ending characters the parameter <see cref="includeLineEndingCharacters" /> must be true.</summary>
	public static int GetLengthOfRow(
		this ITextEditorModel model, int rowIndex, bool includeLineEndingCharacters = false)
	{
		if (!model.RowEndingPositionsList.Any())
			return 0;

		if (rowIndex > model.RowEndingPositionsList.Count - 1)
			rowIndex = model.RowEndingPositionsList.Count - 1;

		if (rowIndex < 0)
			rowIndex = 0;

		var startOfRowTupleInclusive = model.GetRowEndingThatCreatedRow(rowIndex);

		// TODO: Index was out of range exception on 2023-04-18
		var endOfRowTupleExclusive = model.RowEndingPositionsList[rowIndex];

		var lengthOfRowWithLineEndings = endOfRowTupleExclusive.EndPositionIndexExclusive - startOfRowTupleInclusive.EndPositionIndexExclusive;

		if (includeLineEndingCharacters)
			return lengthOfRowWithLineEndings;

		return lengthOfRowWithLineEndings - endOfRowTupleExclusive.RowEndingKind.AsCharacters().Length;
	}

    /// <summary>
	/// Line endings are included in the individual rows which get returned.
	/// </summary>
	/// <param name="startingRowIndex">The starting index of the rows to return</param>
    /// <param name="count">
    /// A count of 0 returns 0 rows.<br/>
    /// A count of 1 returns rows[startingRowIndex] only.<br/>
    /// A count of 2 returns rows[startingRowIndex] and rows[startingRowIndex + 1].<br/>
    /// </param>
    public static List<List<RichCharacter>> GetRows(
		this ITextEditorModel model, int startingRowIndex, int count)
	{
		var rowCountAvailable = model.RowEndingPositionsList.Count - startingRowIndex;

		var rowCountToReturn = count < rowCountAvailable
			? count
			: rowCountAvailable;

		var endingRowIndexExclusive = startingRowIndex + rowCountToReturn;

		var rowsList = new List<List<RichCharacter>>();

		if (rowCountToReturn < 0 || startingRowIndex < 0 || endingRowIndexExclusive < 0)
			return rowsList;

		for (var i = startingRowIndex; i < endingRowIndexExclusive; i++)
		{
			// Previous row's line ending position is this row's start.
			var startOfRowInclusive = model.GetRowEndingThatCreatedRow(i).EndPositionIndexExclusive;

			var endOfRowExclusive = model.RowEndingPositionsList[i].EndPositionIndexExclusive;

            // (2024-02-29) Plan to add text editor partitioning #Step 400:
            // --------------------------------------------------
            // After changing 'ContentList' to an ImmutableList<ImmutableList<RichCharacter>>,
            // a lot of the code broke, understandably.
            //
            // I'm thinking that 'ContentList' should be the an expression bound property
            // that performs a 'SelectMany' on the partitions to pull out all the RichCharacter(s).
            //
            // If I go down that route, that means I need a new name for what the list of partitions will be called.
            // I want to make 'PartitionList' a datatype of ImmutableList<ImmutableList<RichCharacter>>.
			// Then make 'ContentList' a List<RichCharacter>;
            var row = model.ContentList
				.Skip(startOfRowInclusive)
				.Take(endOfRowExclusive - startOfRowInclusive)
				.ToList();

			rowsList.Add(row);
		}

		return rowsList;
	}

	public static int GetTabsCountOnSameRowBeforeCursor(
		this ITextEditorModel model, int rowIndex, int columnIndex)
	{
		var startOfRowPositionIndex = model.GetRowEndingThatCreatedRow(rowIndex).EndPositionIndexExclusive;

		var tabs = model.TabKeyPositionsList
			.SkipWhile(positionIndex => positionIndex < startOfRowPositionIndex)
			.TakeWhile(positionIndex => positionIndex < startOfRowPositionIndex + columnIndex);

		return tabs.Count();
	}

	/// <summary>If applying syntax highlighting it may be preferred to use <see cref="ApplySyntaxHighlightingAsync" />. It is effectively just invoking the lexer and then <see cref="ApplyDecorationRange" /></summary>
	public static void ApplyDecorationRange(
		this ITextEditorModel model, 
		IEnumerable<TextEditorTextSpan> textEditorTextSpans)
	{
		var localContent = model.ContentList;

		var positionsPainted = new HashSet<int>();

		foreach (var textEditorTextSpan in textEditorTextSpans)
		{
			for (var i = textEditorTextSpan.StartingIndexInclusive; i < textEditorTextSpan.EndingIndexExclusive; i++)
			{
				if (i >= localContent.Count)
					continue;

				localContent[i].DecorationByte = textEditorTextSpan.DecorationByte;

				positionsPainted.Add(i);
			}
		}

		for (var i = 0; i < localContent.Count - 1; i++)
		{
			if (!positionsPainted.Contains(i))
			{
				// DecorationByte of 0 is to be 'None'
				localContent[i].DecorationByte = 0;
			}
		}
	}

	public static Task ApplySyntaxHighlightingAsync(this ITextEditorModel model)
	{
		var syntacticTextSpansList = model.CompilerService.GetTokenTextSpansFor(model.ResourceUri);
		var symbolsList = model.CompilerService.GetSymbolsFor(model.ResourceUri);

		var symbolTextSpansList = symbolsList.Select(s => s.TextSpan);

		model.ApplyDecorationRange(syntacticTextSpansList.Union(symbolTextSpansList));

		return Task.CompletedTask;
	}

	/// <summary>
	/// TODO: Given that the text editor is now immutable (2023-12-17), when this is invoked...
	/// ...it should be cached.
	/// </summary>
	public static string GetAllText(this ITextEditorModel model)
	{
		return new string(model.ContentList.Select(rc => rc.Value).ToArray());
	}

	public static int GetPositionIndex(
		this ITextEditorModel model, TextEditorCursor textEditorCursor)
	{
		return model.GetPositionIndex(textEditorCursor.RowIndex, textEditorCursor.ColumnIndex);
	}
	
	public static int GetPositionIndex(
		this ITextEditorModel model, TextEditorCursorModifier textEditorCursorModifier)
	{
		return model.GetPositionIndex(textEditorCursorModifier.RowIndex, textEditorCursorModifier.ColumnIndex);
	}

	/// <summary>
	/// TODO: How should this method handle input which is out of bounds?
	/// </summary>
	public static int GetPositionIndex(
		this ITextEditorModel model, int rowIndex, int columnIndex)
	{
		var startOfRowPositionIndex = model.GetRowEndingThatCreatedRow(rowIndex).EndPositionIndexExclusive;
		return startOfRowPositionIndex + columnIndex;
	}

	public static (int rowIndex, int columnIndex) GetRowAndColumnIndicesFromPositionIndex(
		this ITextEditorModel model, int positionIndex)
	{
        var rowInformation = model.GetRowInformationFromPositionIndex(positionIndex);

		return (
			rowInformation.RowIndex,
			positionIndex - rowInformation.RowStartPositionIndexInclusive);
    }

    /// <summary>
    /// To receive a <see cref="string"/> value, one can use <see cref="GetString"/> instead.
    /// </summary>
    public static char GetCharacter(
		this ITextEditorModel model,
		int positionIndex)
	{
		if (positionIndex < 0 || positionIndex >= model.ContentList.Count)
			return ParserFacts.END_OF_FILE;

		return model.ContentList[positionIndex].Value;
	}

    /// <summary>
    /// To receive a <see cref="char"/> value, one can use <see cref="GetCharacter"/> instead.
    /// </summary>
    public static string GetString(
		this ITextEditorModel model,
		int startingPositionIndex,
		int count)
	{
		return new string(model.ContentList
			.Skip(startingPositionIndex)
			.Take(count)
			.Select(rc => rc.Value)
			.ToArray());
	}

	public static string GetLineRange(this ITextEditorModel model, int startingRowIndex, int count)
	{
		if (startingRowIndex > model.RowCount - 1)
			return string.Empty;
        
		var startingPositionIndexInclusive = model.GetPositionIndex(startingRowIndex, 0);

        var lastRowIndexExclusive = startingRowIndex + count;
		int endingPositionIndexExclusive;

		if (lastRowIndexExclusive > model.RowCount - 1)
		{
			endingPositionIndexExclusive = model.DocumentLength;
        }
		else
		{
			endingPositionIndexExclusive = model.GetPositionIndex(lastRowIndexExclusive, 0);
        }

		return model.GetString(
			startingPositionIndexInclusive,
			endingPositionIndexExclusive - startingPositionIndexInclusive);
	}

	/// <summary>Given a <see cref="TextEditorModel"/> with a preference for the right side of the cursor, the following conditional branch will play out.<br/><br/>-IF the cursor is amongst a word, that word will be returned.<br/><br/>-ELSE IF the start of a word is to the right of the cursor that word will be returned.<br/><br/>-ELSE IF the end of a word is to the left of the cursor that word will be returned.</summary>
	public static TextEditorTextSpan? GetWordTextSpan(this ITextEditorModel model, int positionIndex)
	{
		var previousCharacter = model.GetCharacter(positionIndex - 1);
		var currentCharacter = model.GetCharacter(positionIndex);

		var previousCharacterKind = CharacterKindHelper.CharToCharacterKind(previousCharacter);
		var currentCharacterKind = CharacterKindHelper.CharToCharacterKind(currentCharacter);

		var rowInformation = model.GetRowInformationFromPositionIndex(positionIndex);

		var columnIndex = positionIndex - rowInformation.RowStartPositionIndexInclusive;

		if (previousCharacterKind == CharacterKind.LetterOrDigit && currentCharacterKind == CharacterKind.LetterOrDigit)
		{
			var wordColumnIndexStartInclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
				rowInformation.RowIndex,
				columnIndex,
				true);

			if (wordColumnIndexStartInclusive == -1)
				wordColumnIndexStartInclusive = 0;

			var wordColumnIndexEndExclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
				rowInformation.RowIndex,
				columnIndex,
				false);

			if (wordColumnIndexEndExclusive == -1)
				wordColumnIndexEndExclusive = model.GetLengthOfRow(rowInformation.RowIndex);

			return new TextEditorTextSpan(
				wordColumnIndexStartInclusive + rowInformation.RowStartPositionIndexInclusive,
				wordColumnIndexEndExclusive + rowInformation.RowStartPositionIndexInclusive,
				0,
				model.ResourceUri,
				model.GetAllText());
		}
		else if (currentCharacterKind == CharacterKind.LetterOrDigit)
		{
			var wordColumnIndexEndExclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
				rowInformation.RowIndex,
				columnIndex,
				false);

			if (wordColumnIndexEndExclusive == -1)
				wordColumnIndexEndExclusive = model.GetLengthOfRow(rowInformation.RowIndex);

			return new TextEditorTextSpan(
				columnIndex + rowInformation.RowStartPositionIndexInclusive,
				wordColumnIndexEndExclusive + rowInformation.RowStartPositionIndexInclusive,
				0,
				model.ResourceUri,
				model.GetAllText());
		}
		else if (previousCharacterKind == CharacterKind.LetterOrDigit)
		{
			var wordColumnIndexStartInclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
				rowInformation.RowIndex,
				columnIndex,
				true);

			if (wordColumnIndexStartInclusive == -1)
				wordColumnIndexStartInclusive = 0;

			return new TextEditorTextSpan(
				wordColumnIndexStartInclusive + rowInformation.RowStartPositionIndexInclusive,
				columnIndex + rowInformation.RowStartPositionIndexInclusive,
				0,
				model.ResourceUri,
				model.GetAllText());
		}

		return null;
	}

	public static ImmutableArray<TextEditorTextSpan> FindMatches(
		this ITextEditorModel model,
		string query)
	{
		var text = model.GetAllText();
		var matchedTextSpans = new List<TextEditorTextSpan>();

		for (int outerI = 0; outerI < text.Length; outerI++)
		{
            if (outerI + query.Length <= text.Length)
			{
				int innerI = 0;
                for (; innerI < query.Length; innerI++)
				{
					if (text[outerI + innerI] != query[innerI])
						break;
				}

				if (innerI == query.Length)
				{
					// Then the entire query was matched
					matchedTextSpans.Add(new TextEditorTextSpan(
                        outerI,
                        outerI + innerI,
                        (byte)TextEditorFindOverlayDecorationKind.LongestCommonSubsequence,
                        model.ResourceUri,
                        text));
                }
			}
		}

		return matchedTextSpans.ToImmutableArray();
	}

    public static RowInformation GetRowInformationFromPositionIndex(
		this ITextEditorModel model,
		int positionIndex)
	{
		for (var i = model.RowEndingPositionsList.Count - 1; i >= 0; i--)
		{
			var rowEndingTuple = model.RowEndingPositionsList[i];

			if (positionIndex >= rowEndingTuple.EndPositionIndexExclusive)
			{
				return new(
					i + 1,
					rowEndingTuple.EndPositionIndexExclusive,
					i == model.RowEndingPositionsList.Count - 1
						? rowEndingTuple
						: model.RowEndingPositionsList[i + 1]);
			}
		}

		return new(0, 0, model.RowEndingPositionsList[0]);
	}

	/// <summary>
	/// <see cref="moveBackwards"/> is to mean earlier in the document
	/// (lower column index or lower row index depending on position) 
	/// </summary>
	/// <returns>Will return -1 if no valid result was found.</returns>
	public static int GetColumnIndexOfCharacterWithDifferingKind(
		this ITextEditorModel model, int rowIndex, int columnIndex, bool moveBackwards)
	{
		var iterateBy = moveBackwards
			? -1
			: 1;

		var startOfRowPositionIndex = model.GetRowEndingThatCreatedRow(rowIndex).EndPositionIndexExclusive;

		if (rowIndex > model.RowEndingPositionsList.Count - 1)
			return -1;

		var lastPositionIndexOnRow = model.RowEndingPositionsList[rowIndex].EndPositionIndexExclusive - 1;

		var positionIndex = model.GetPositionIndex(rowIndex, columnIndex);

		if (moveBackwards)
		{
			if (positionIndex <= startOfRowPositionIndex)
				return -1;

			positionIndex -= 1;
		}

		if (positionIndex < 0 || positionIndex >= model.ContentList.Count)
			return -1;

		var startingCharacterKind = model.ContentList[positionIndex].GetCharacterKind();

		while (true)
		{
			if (positionIndex >= model.ContentList.Count ||
				positionIndex > lastPositionIndexOnRow ||
				positionIndex < startOfRowPositionIndex)
			{
				return -1;
			}

			var currentCharacterKind = model.ContentList[positionIndex].GetCharacterKind();

			if (currentCharacterKind != startingCharacterKind)
				break;

			positionIndex += iterateBy;
		}

		if (moveBackwards)
			positionIndex += 1;

		return positionIndex - startOfRowPositionIndex;
	}

	public static ImmutableArray<RichCharacter> GetAllRichCharacters(this ITextEditorModel model)
	{
		return model.ContentList.ToImmutableArray();
	}

	public static bool CanUndoEdit(this ITextEditorModel model)
	{
		return model.EditBlockIndex > 0;
	}

	public static bool CanRedoEdit(this ITextEditorModel model)
	{
		return model.EditBlockIndex < model.EditBlocksList.Count - 1;
	}

	public static CharacterKind GetCharacterKind(this ITextEditorModel model, int positionIndex)
	{
		try
		{
			return model.ContentList[positionIndex].GetCharacterKind();
		}
		catch (ArgumentOutOfRangeException)
		{
			// The text editor's cursor is is likely
			// to have this occur at times
		}

		return CharacterKind.Bad;
	}

	public static string? ReadPreviousWordOrDefault(
		this ITextEditorModel model, int rowIndex, int columnIndex, bool isRecursiveCall = false)
	{
		var wordPositionIndexEndExclusive = model.GetPositionIndex(rowIndex, columnIndex);
		var wordCharacterKind = model.GetCharacterKind(wordPositionIndexEndExclusive - 1);

		if (wordCharacterKind == CharacterKind.Punctuation && !isRecursiveCall)
		{
			// If previous previous word is a punctuation character, then perhaps
			// the user hit { 'Ctrl' + 'Space' } to trigger the autocomplete
			// and was at a MemberAccessToken (or a period '.')
			//
			// So, read the word previous to the punctuation.

			var anotherAttemptColumnIndex = columnIndex - 1;

			if (anotherAttemptColumnIndex >= 0)
				return model.ReadPreviousWordOrDefault(rowIndex, anotherAttemptColumnIndex, true);
		}

		if (wordCharacterKind == CharacterKind.LetterOrDigit)
		{
			var wordColumnIndexStartInclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
				rowIndex,
				columnIndex,
				true);

			if (wordColumnIndexStartInclusive == -1)
				wordColumnIndexStartInclusive = 0;

			var wordLength = columnIndex - wordColumnIndexStartInclusive;

			var wordPositionIndexStartInclusive = wordPositionIndexEndExclusive - wordLength;

			return model.GetString(wordPositionIndexStartInclusive, wordLength);
		}

		return null;
	}

	public static string? ReadNextWordOrDefault(
		this ITextEditorModel model, int rowIndex, int columnIndex)
	{
		var wordPositionIndexStartInclusive = model.GetPositionIndex(rowIndex, columnIndex);

		var wordCharacterKind = model.GetCharacterKind(wordPositionIndexStartInclusive);

		if (wordCharacterKind == CharacterKind.LetterOrDigit)
		{
			var wordColumnIndexEndExclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
				rowIndex,
				columnIndex,
				false);

			if (wordColumnIndexEndExclusive == -1)
				wordColumnIndexEndExclusive = model.GetLengthOfRow(rowIndex);

			var wordLength = wordColumnIndexEndExclusive - columnIndex;

			return model.GetString(wordPositionIndexStartInclusive, wordLength);
		}

		return null;
	}

	/// <summary>This method returns the text to the left of the cursor in most cases. The method name is as such because of right to left written texts.<br/><br/>One uses this method most often to measure the position of the cursor when rendering the UI for a font-family which is proportional (i.e. not monospace).</summary>
	public static string GetTextOffsettingCursor(
		this ITextEditorModel model, TextEditorCursor textEditorCursor)
	{
		var cursorPositionIndex = model.GetPositionIndex(textEditorCursor);
		var startOfRowTuple = model.GetRowEndingThatCreatedRow(textEditorCursor.RowIndex);

		return model.GetString(startOfRowTuple.EndPositionIndexExclusive, cursorPositionIndex - startOfRowTuple.EndPositionIndexExclusive);
	}

	public static string GetLine(
		this ITextEditorModel model, int rowIndex)
	{
		if (rowIndex < 0 || rowIndex > model.RowCount - 1)
			return string.Empty;

		var startOfRowTuple = model.GetRowEndingThatCreatedRow(rowIndex);
		var lengthOfRow = model.GetLengthOfRow(rowIndex, true);

		return model.GetString(startOfRowTuple.EndPositionIndexExclusive, lengthOfRow);
	}
}
