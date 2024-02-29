using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorModels;

/// <summary>
/// <see cref="TextEditorModelHelper"/>
/// </summary>
public class TextEditorModelHelperTests
{
	/// <summary>
	/// <see cref="TextEditorModelHelper.GetRowEndingThatCreatedRow(ITextEditorModel, int)"/>
	/// </summary>
	[Fact]
	public void GetRowEndingThatCreatedRow()
	{
		// This test is broken down into code blocks, one for each subcase.
		// TODO: Perhaps separate the subcases as individual tests.

		// InBounds
		{
			{
                TextEditorServicesTestsHelper.InBounds_StartOfRow(
					out var model,
					out var cursor);

				var expected = new RowEnding(12, 13, RowEndingKind.Linefeed);

				// TextEditorModel
				{
					var actual = model.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }
				
				// TextEditorModelModifier
				{
					var modelModifier = new TextEditorModelModifier(model);
                    var actual = modelModifier.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }
            }

            // !start_of_row && !end_of_row
            {
                TextEditorServicesTestsHelper.InBounds_NOT_StartOfRow_AND_NOT_EndOfRow(
                    out var model,
                    out var cursor);

                var expected = new RowEnding(12, 13, RowEndingKind.Linefeed);

                // TextEditorModel
                {
                    var actual = model.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }

                // TextEditorModelModifier
                {
                    var modelModifier = new TextEditorModelModifier(model);
                    var actual = modelModifier.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }
            }
			
			// End of a row && !end_of_document
			{
                TextEditorServicesTestsHelper.InBounds_EndOfRow(
                    out var model,
                    out var cursor);

                var expected = new RowEnding(12, 13, RowEndingKind.Linefeed);

                // TextEditorModel
                {
                    var actual = model.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }

                // TextEditorModelModifier
                {
                    var modelModifier = new TextEditorModelModifier(model);
                    var actual = modelModifier.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }
            }
			
			// Start of document
			{
                TextEditorServicesTestsHelper.InBounds_StartOfDocument(
                    out var model,
                    out var cursor);

                var expected = new RowEnding(0, 0, RowEndingKind.StartOfFile);

                // TextEditorModel
                {
                    var actual = model.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }

                // TextEditorModelModifier
                {
                    var modelModifier = new TextEditorModelModifier(model);
                    var actual = modelModifier.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }
            }

            // End of document
			//
            // NOTE: end of document is to mean: position_index == document.Length...
			// ...the reason for this is that an insertion cursor is allowed to go 1 index...
			// ...beyond the length of the document.
            {
                TextEditorServicesTestsHelper.InBounds_EndOfDocument(
                    out var model,
                    out var cursor);

                var expected = new RowEnding(24, 25, RowEndingKind.Linefeed);

                // TextEditorModel
                {
                    var actual = model.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }

                // TextEditorModelModifier
                {
                    var modelModifier = new TextEditorModelModifier(model);
                    var actual = modelModifier.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }
            }
		}

		// OutOfBounds
		{
			// position_index < 0
			{
                TextEditorServicesTestsHelper.OutOfBounds_PositionIndex_LESS_THAN_Zero(
                    out var model,
                    out var cursor);

                var expected = new RowEnding(0, 0, RowEndingKind.StartOfFile);

                // TextEditorModel
                {
                    var actual = model.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }

                // TextEditorModelModifier
                {
                    var modelModifier = new TextEditorModelModifier(model);
                    var actual = modelModifier.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }
            }

            // position_index > document.Length + 1
            {
                TextEditorServicesTestsHelper.OutOfBounds_PositionIndex_GREATER_THAN_DocumentLength_PLUS_One(
                    out var model,
                    out var cursor);

                var expected = new RowEnding(24, 25, RowEndingKind.Linefeed);

                // TextEditorModel
                {
                    var actual = model.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }

                // TextEditorModelModifier
                {
                    var modelModifier = new TextEditorModelModifier(model);
                    var actual = modelModifier.GetRowEndingThatCreatedRow(cursor.RowIndex);
                    Assert.Equal(expected, actual);
                }
            }
        }
	}

	/// <summary>
	/// <see cref="TextEditorModelHelper.GetLengthOfRow(ITextEditorModel, int, bool)"/>
	/// </summary>
	[Fact]
	public void GetLengthOfRow()
	{
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(
            out var model);

        // Negative rowIndex
        {
            // Explanation of result: negative index gets changed to index of 0
            var lengthOfRow = model.GetLengthOfRow(TestConstants.NEGATIVE_ROW_INDEX);
            Assert.Equal(TestConstants.LENGTH_OF_FIRST_ROW, lengthOfRow);
        }

		// First row
		{
            var lengthOfRow = model.GetLengthOfRow(TestConstants.FIRST_ROW_INDEX);
            Assert.Equal(TestConstants.LENGTH_OF_FIRST_ROW, lengthOfRow);
        }

		// Row which is between first and last row.
		{
            var lengthOfRow = model.GetLengthOfRow(TestConstants.ROW_INDEX_WHICH_IS_BETWEEN_FIRST_AND_LAST_ROW);
            Assert.Equal(TestConstants.LENGTH_OF_ROW_WHICH_IS_BETWEEN_FIRST_AND_LAST_ROW, lengthOfRow);
        }

        // Last row
        {
            var lengthOfRow = model.GetLengthOfRow(TestConstants.LAST_ROW_INDEX);
            Assert.Equal(TestConstants.LENGTH_OF_LAST_ROW, lengthOfRow);
        }

        // rowIndex > document.Rows.Count
        {
            // Explanation of result: large out of bounds index gets changed to
            // index of 'model.RowEndingPositionsList.Count - 1'
            var lengthOfRow = model.GetLengthOfRow(TestConstants.LARGE_OUT_OF_BOUNDS_ROW_INDEX);
            Assert.Equal(TestConstants.LENGTH_OF_LAST_ROW, lengthOfRow);
        }
    }

	/// <summary>
	/// <see cref="TextEditorModelHelper.GetRows(ITextEditorModel, int, int)"/>
	/// </summary>
	[Fact]
	public void GetRows()
	{
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(
            out var model);

        // Negative rowIndex
        {
            // Negative count
            {
                var rows = model.GetRows(TestConstants.NEGATIVE_ROW_INDEX, -3);
                Assert.Equal(new List<List<RichCharacter>>(), rows);
            }

            // Positive count
            {
                var rows = model.GetRows(TestConstants.NEGATIVE_ROW_INDEX, 3);
                Assert.Equal(new List<List<RichCharacter>>(), rows);
			}

            // Zero count
            {
                var rows = model.GetRows(TestConstants.NEGATIVE_ROW_INDEX, 0);
                Assert.Equal(new List<List<RichCharacter>>(), rows);
            }

            // If the rowIndex is negative, but the row count to read results
            // in indices which are valid?
            {
                var countNeededToGoFromNegativeStartingIndexToAValidIndex =
                    1 + (-1 * TestConstants.NEGATIVE_ROW_INDEX);

                var rows = model.GetRows(
                    TestConstants.NEGATIVE_ROW_INDEX,
                    countNeededToGoFromNegativeStartingIndexToAValidIndex);

                Assert.Equal(new List<List<RichCharacter>>(), rows);
            }
        }

        // First row
        {
            // Negative count
            {
                var rows = model.GetRows(TestConstants.FIRST_ROW_INDEX, -3);
                Assert.Equal(new List<List<RichCharacter>>(), rows);
            }

            // Positive count
            {
                var rows = model.GetRows(TestConstants.FIRST_ROW_INDEX, 3);

                Assert.Equal(3, rows.Count);

                var text = new string(rows
                    .SelectMany(row => row.Select(richChar => richChar.Value))
                    .ToArray());

                Assert.Equal("Hello World!\n7 Pillows\n \n", text);
            }
            
            // Zero count
            {
                var rows = model.GetRows(TestConstants.FIRST_ROW_INDEX, 0);
                Assert.Equal(new List<List<RichCharacter>>(), rows);
            }
        }

        // Row which is between first and last row.
        {
            // Negative count
            {
                var rows = model.GetRows(TestConstants.ROW_INDEX_WHICH_IS_BETWEEN_FIRST_AND_LAST_ROW, -3);
                Assert.Equal(new List<List<RichCharacter>>(), rows);
            }

            // Positive count
            {
                var rows = model.GetRows(TestConstants.ROW_INDEX_WHICH_IS_BETWEEN_FIRST_AND_LAST_ROW, 3);

                Assert.Equal(3, rows.Count);

                var text = new string(rows
                    .SelectMany(row => row.Select(richChar => richChar.Value))
                    .ToArray());

                Assert.Equal("7 Pillows\n \n,abc123", text);
            }
        }

        // Last row
        {
            // Negative count
            {
                var rows = model.GetRows(TestConstants.LAST_ROW_INDEX, -3);
                Assert.Equal(new List<List<RichCharacter>>(), rows);
            }

            // Positive count
            {
                var rows = model.GetRows(TestConstants.LAST_ROW_INDEX, 3);

                Assert.Single(rows);

                var text = new string(rows
                    .SelectMany(row => row.Select(richChar => richChar.Value))
                    .ToArray());

                Assert.Equal(",abc123", text);
            }
        }

        // rowIndex > document.Rows.Count
        {
            // Negative count
            {
                var rows = model.GetRows(TestConstants.LARGE_OUT_OF_BOUNDS_ROW_INDEX, -3);
                Assert.Equal(new List<List<RichCharacter>>(), rows);
            }

            // Positive count
            {
                var rows = model.GetRows(TestConstants.LARGE_OUT_OF_BOUNDS_ROW_INDEX, 3);
                Assert.Equal(new List<List<RichCharacter>>(), rows);
            }
        }
    }

	/// <summary>
	/// <see cref="TextEditorModelHelper.GetTabsCountOnSameRowBeforeCursor(ITextEditorModel, int, int)"/>
	/// </summary>
	[Fact]
	public void GetTabsCountOnSameRowBeforeCursor()
	{
        // Negative rowIndex
        {
            // Negative columnIndex
            {
                throw new NotImplementedException();
            }

            // Positive columnIndex && WithinRange
            {
                throw new NotImplementedException();
            }

            // Positive columnIndex && OutOfRange
            {
                throw new NotImplementedException();
            }
        }

        // First row
        {
            // Negative columnIndex
            {
                throw new NotImplementedException();
            }

            // Positive columnIndex && WithinRange
            {
                throw new NotImplementedException();
            }
            
            // Positive columnIndex && OutOfRange
            {
                throw new NotImplementedException();
            }
        }

        // Row which is between first and last row.
        {
            // Negative columnIndex
            {
                throw new NotImplementedException();
            }

            // Positive columnIndex && WithinRange
            {
                throw new NotImplementedException();
            }

            // Positive columnIndex && OutOfRange
            {
                throw new NotImplementedException();
            }
        }

        // Last row
        {
            // Negative columnIndex
            {
                throw new NotImplementedException();
            }

            // Positive columnIndex && WithinRange
            {
                throw new NotImplementedException();
            }

            // Positive columnIndex && OutOfRange
            {
                throw new NotImplementedException();
            }
        }

        // rowIndex > document.Rows.Count
        {
            // Negative columnIndex
            {
                throw new NotImplementedException();
            }

            // Positive columnIndex && WithinRange
            {
                throw new NotImplementedException();
            }

            // Positive columnIndex && OutOfRange
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelHelper.ApplyDecorationRange(ITextEditorModel, IEnumerable{TextEditorTextSpan})"/>
    /// </summary>
    [Fact]
	public void ApplyDecorationRange()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelHelper.ApplySyntaxHighlightingAsync(ITextEditorModel)"/>
	/// </summary>
	[Fact]
	public void ApplySyntaxHighlightingAsync()
	{
		throw new NotImplementedException();
	}

    /// <summary>
	/// <see cref="TextEditorModelHelper.GetAllRichCharacters(ITextEditorModel)"/>
	/// </summary>
	[Fact]
    public void GetAllRichCharacters()
    {
        // Empty
        {
            var sourceText = string.Empty;

            var model = new TextEditorModel(
                new ResourceUri($"/{nameof(GetAllRichCharacters)}.txt"),
                DateTime.UtcNow,
                ExtensionNoPeriodFacts.TXT,
                sourceText,
                null,
                null);

            var richCharacterList = model.GetAllRichCharacters();

            Assert.Empty(sourceText);
            Assert.Empty(richCharacterList);
        }

        // NotEmpty
        {
            TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var model);

            var richCharacterList = model.GetAllRichCharacters();

            Assert.Equal(TestConstants.SOURCE_TEXT.Length, richCharacterList.Length);

            for (int i = 0; i < richCharacterList.Length; i++)
            {
                var character = TestConstants.SOURCE_TEXT[i];
                var richCharacter = richCharacterList[i];

                Assert.Equal(character, richCharacter.Value);
                Assert.Equal((byte)GenericDecorationKind.None, richCharacter.DecorationByte);
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelHelper.GetAllText(ITextEditorModel)"/>
    /// </summary>
    [Fact]
	public void GetAllText()
	{
        // Empty
        {
            var sourceText = string.Empty;

            var model = new TextEditorModel(
                new ResourceUri($"/{nameof(GetAllText)}.txt"),
                DateTime.UtcNow,
                ExtensionNoPeriodFacts.TXT,
                sourceText,
                null,
                null);

            var outputText = model.GetAllText();

            Assert.Empty(sourceText);
            Assert.Empty(outputText);
        }

        // NotEmpty
        {
            TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var model);

            var outputText = model.GetAllText();

            Assert.Equal(TestConstants.SOURCE_TEXT.Length, outputText.Length);

            for (int i = 0; i < outputText.Length; i++)
            {
                var sourceCharacter = TestConstants.SOURCE_TEXT[i];
                var outputCharacter = outputText[i];

                Assert.Equal(sourceCharacter, outputCharacter);
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelHelper.GetPositionIndex(ITextEditorModel, TextEditorCursor)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorModelHelper.GetPositionIndex(ITextEditorModel, TextEditorCursorModifier)"/>
	/// <see cref="TextEditorModelHelper.GetPositionIndex(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
	public void GetPositionIndex()
	{
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var model);

        // Cursor.RowIndex < 0
        {
            var expectedPositionIndex = -1;

            var rowIndex = -1;
            var columnIndex = -1;
            var pointPositionIndex = model.GetPositionIndex(rowIndex, columnIndex);
            Assert.Equal(expectedPositionIndex, pointPositionIndex);

            var cursor = new TextEditorCursor(rowIndex, columnIndex, true);
            var cursorPositionIndex = model.GetPositionIndex(cursor);
            Assert.Equal(expectedPositionIndex, cursorPositionIndex);

            var cursorModifier = new TextEditorCursorModifier(cursor);
            var modifierPositionIndex = model.GetPositionIndex(cursorModifier);
            Assert.Equal(expectedPositionIndex, modifierPositionIndex);
        }

        // Cursor.RowIndex > 0 && Cursor.RowIndex is within bounds
        {
            // FirstRow
            {
                var expectedPositionIndex = 1;

                var rowIndex = 0;
                var columnIndex = 1;
                var pointPositionIndex = model.GetPositionIndex(rowIndex, columnIndex);
                Assert.Equal(expectedPositionIndex, pointPositionIndex);

                var cursor = new TextEditorCursor(rowIndex, columnIndex, true);
                var cursorPositionIndex = model.GetPositionIndex(cursor);
                Assert.Equal(expectedPositionIndex, cursorPositionIndex);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var modifierPositionIndex = model.GetPositionIndex(cursorModifier);
                Assert.Equal(expectedPositionIndex, modifierPositionIndex);
            }

            // !FirstRow && !LastRow
            {
                var expectedPositionIndex = 14;

                var rowIndex = TestConstants.ROW_INDEX_WHICH_IS_BETWEEN_FIRST_AND_LAST_ROW;
                var columnIndex = 1;
                var pointPositionIndex = model.GetPositionIndex(rowIndex, columnIndex);
                Assert.Equal(expectedPositionIndex, pointPositionIndex);

                var cursor = new TextEditorCursor(rowIndex, columnIndex, true);
                var cursorPositionIndex = model.GetPositionIndex(cursor);
                Assert.Equal(expectedPositionIndex, cursorPositionIndex);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var modifierPositionIndex = model.GetPositionIndex(cursorModifier);
                Assert.Equal(expectedPositionIndex, modifierPositionIndex);
            }

            // LastRow
            {
                var expectedPositionIndex = 26;

                var rowIndex = TestConstants.LAST_ROW_INDEX;
                var columnIndex = 1;
                var pointPositionIndex = model.GetPositionIndex(rowIndex, columnIndex);
                Assert.Equal(expectedPositionIndex, pointPositionIndex);

                var cursor = new TextEditorCursor(rowIndex, columnIndex, true);
                var cursorPositionIndex = model.GetPositionIndex(cursor);
                Assert.Equal(expectedPositionIndex, cursorPositionIndex);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var modifierPositionIndex = model.GetPositionIndex(cursorModifier);
                Assert.Equal(expectedPositionIndex, modifierPositionIndex);
            }
        }

        // Cursor.RowIndex > 0 && Cursor.RowIndex is OUT of bounds
        {
            var expectedPositionIndex = 27;

            var rowIndex = TestConstants.LARGE_OUT_OF_BOUNDS_ROW_INDEX;
            var columnIndex = 2;
            var pointPositionIndex = model.GetPositionIndex(rowIndex, columnIndex);
            Assert.Equal(expectedPositionIndex, pointPositionIndex);

            var cursor = new TextEditorCursor(rowIndex, columnIndex, true);
            var cursorPositionIndex = model.GetPositionIndex(cursor);
            Assert.Equal(expectedPositionIndex, cursorPositionIndex);

            var cursorModifier = new TextEditorCursorModifier(cursor);
            var modifierPositionIndex = model.GetPositionIndex(cursorModifier);
            Assert.Equal(expectedPositionIndex, modifierPositionIndex);
        }
    }

	/// <summary>
	/// <see cref="TextEditorModelHelper.GetCharacter(ITextEditorModel, int)"/>
	/// </summary>
	[Fact]
	public void GetCharacter()
	{
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var model);

        // PositionIndex < 0
        {
            var expectedCharacter = ParserFacts.END_OF_FILE;
            var positionIndex = -1;
            var actualCharacter = model.GetCharacter(positionIndex);
            Assert.Equal(expectedCharacter, actualCharacter);
        }

        // PositionIndex > 0 && PositionIndex is within bounds
        {
            // FirstRow
            {
                var expectedCharacter = 'e';
                var positionIndex = 1;
                var actualCharacter = model.GetCharacter(positionIndex);
                Assert.Equal(expectedCharacter, actualCharacter);
            }

            // !FirstRow && !LastRow
            {
                var expectedCharacter = ' ';
                var positionIndex = 14;
                var actualCharacter = model.GetCharacter(positionIndex);
                Assert.Equal(expectedCharacter, actualCharacter);
            }

            // LastRow
            {
                var expectedCharacter = 'a';
                var positionIndex = 26;
                var actualCharacter = model.GetCharacter(positionIndex);
                Assert.Equal(expectedCharacter, actualCharacter);
            }
        }

        // PositionIndex > 0 && PositionIndex is OUT of bounds
        {
            var expectedCharacter = ParserFacts.END_OF_FILE;
            var positionIndex = 43;
            var actualCharacter = model.GetCharacter(positionIndex);
            Assert.Equal(expectedCharacter, actualCharacter);
        }
    }

    /// <summary>
	/// <see cref="TextEditorModelHelper.GetCharacterKind(ITextEditorModel, int)"/>
	/// </summary>
	[Fact]
    public void GetCharacterKind()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var model);

        // PositionIndex < 0
        {
            var expectedCharacterKind = CharacterKind.Bad;
            var positionIndex = -1;
            var actualCharacterKind = model.GetCharacterKind(positionIndex);
            Assert.Equal(expectedCharacterKind, actualCharacterKind);
        }

        // PositionIndex > 0 && PositionIndex is within bounds
        {
            // FirstRow
            {
                // CharacterKind.LetterOrDigit
                {
                    var expectedCharacterKind = CharacterKind.LetterOrDigit;
                    var positionIndex = 1;
                    var actualCharacterKind = model.GetCharacterKind(positionIndex);
                    Assert.Equal(expectedCharacterKind, actualCharacterKind);
                }
                
                // CharacterKind.Punctuation
                {
                    var expectedCharacterKind = CharacterKind.Punctuation;
                    var positionIndex = 11;
                    var actualCharacterKind = model.GetCharacterKind(positionIndex);
                    Assert.Equal(expectedCharacterKind, actualCharacterKind);
                }
            }

            // !FirstRow && !LastRow
            {
                var expectedCharacterKind = CharacterKind.Whitespace;
                var positionIndex = 14;
                var actualCharacterKind = model.GetCharacterKind(positionIndex);
                Assert.Equal(expectedCharacterKind, actualCharacterKind);
            }

            // LastRow
            {
                var expectedCharacterKind = CharacterKind.LetterOrDigit;
                var positionIndex = 26;
                var actualCharacterKind = model.GetCharacterKind(positionIndex);
                Assert.Equal(expectedCharacterKind, actualCharacterKind);
            }
        }

        // PositionIndex > 0 && PositionIndex is OUT of bounds
        {
            var expectedCharacterKind = CharacterKind.Bad;
            var positionIndex = 43;
            var actualCharacterKind = model.GetCharacterKind(positionIndex);
            Assert.Equal(expectedCharacterKind, actualCharacterKind);
        }
    }

    /// <summary>
	/// <see cref="TextEditorModelHelper.GetWordTextSpan(ITextEditorModel, int)"/>
	/// </summary>
	[Fact]
    public void GetWordTextSpan()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var model);

        // PositionIndex < 0
        {
            var expectedTextSpan = (TextEditorTextSpan?)null;

            var positionIndex = -1;
            var actualTextSpan = model.GetWordTextSpan(positionIndex);
            Assert.Equal(expectedTextSpan, actualTextSpan);
        }

        // PositionIndex > 0 && PositionIndex is within bounds
        {
            // FirstRow
            {
                // CharacterKind.LetterOrDigit
                {
                    var expectedTextSpan = new TextEditorTextSpan(
                        0, 5, 0, model.ResourceUri, model.GetAllText());

                    var positionIndex = 1;
                    var actualTextSpan = model.GetWordTextSpan(positionIndex);
                    Assert.Equal(expectedTextSpan, actualTextSpan);
                }

                // Position index lies between LetterOrDigit, and Punctuation
                {
                    var expectedTextSpan = new TextEditorTextSpan(
                        6, 11, 0, model.ResourceUri, model.GetAllText());

                    var positionIndex = 11;
                    var actualTextSpan = model.GetWordTextSpan(positionIndex);
                    Assert.Equal(expectedTextSpan, actualTextSpan);
                }
                
                // Position index lies between Punctuation, and Whitespace
                {
                    var expectedTextSpan = (TextEditorTextSpan?)null;

                    var positionIndex = 12;
                    var actualTextSpan = model.GetWordTextSpan(positionIndex);
                    Assert.Equal(expectedTextSpan, actualTextSpan);
                }
            }

            // !FirstRow && !LastRow
            //
            // Position index lies between LetterOrDigit and Whitespace
            {
                var expectedTextSpan = new TextEditorTextSpan(
                    13, 14, 0, model.ResourceUri, model.GetAllText());

                var positionIndex = 14;
                var actualTextSpan = model.GetWordTextSpan(positionIndex);
                Assert.Equal(expectedTextSpan, actualTextSpan);
            }

            // LastRow
            {
                var expectedTextSpan = new TextEditorTextSpan(
                    26, 32, 0, model.ResourceUri, model.GetAllText());

                var positionIndex = 26;
                var actualTextSpan = model.GetWordTextSpan(positionIndex);
                Assert.Equal(expectedTextSpan, actualTextSpan);
            }
        }

        // PositionIndex > 0 && PositionIndex is OUT of bounds
        {
            var expectedTextSpan = (TextEditorTextSpan?)null;

            var positionIndex = 43;
            var actualTextSpan = model.GetWordTextSpan(positionIndex);
            Assert.Equal(expectedTextSpan, actualTextSpan);
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelHelper.GetString(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
	public void GetString()
	{
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var model);

        // PositionIndex < 0
        {
            var positionIndex = -1;

            // Count < 0
            {
                var expectedString = string.Empty;
                var count = -1;
                var actualString = model.GetString(positionIndex, count);
                Assert.Equal(expectedString, actualString);
            }

            // Count == 0
            {
                var expectedString = string.Empty;
                var count = 0;
                var actualString = model.GetString(positionIndex, count);
                Assert.Equal(expectedString, actualString);
            }

            // Count > 0
            {
                var expectedString = "H";
                var count = 1;
                var actualString = model.GetString(positionIndex, count);
                Assert.Equal(expectedString, actualString);
            }

            // Count reads beyond the document length
            {
                var expectedString = TestConstants.SOURCE_TEXT;
                var count = 60;
                var actualString = model.GetString(positionIndex, count);
                Assert.Equal(expectedString, actualString);
            }
        }

        // PositionIndex is within bounds
        {
            // PositionIndex resides on the FirstRow
            {
                var positionIndex = 1;

                // Count < 0
                {
                    var expectedString = string.Empty;
                    var count = -1;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }

                // Count == 0
                {
                    var expectedString = string.Empty;
                    var count = 0;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }

                // Count > 0
                {
                    var expectedString = "e";
                    var count = 1;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }

                // Count reads beyond the document length
                {
                    var expectedString = TestConstants.SOURCE_TEXT[1..];
                    var count = 60;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }
            }

            //  PositionIndex does not reside on the FirstRow nor the LastRow
            {
                var positionIndex = 14;

                // Count < 0
                {
                    var expectedString = string.Empty;
                    var count = -1;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }

                // Count == 0
                {
                    var expectedString = string.Empty;
                    var count = 0;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }

                // Count > 0
                {
                    var expectedString = " ";
                    var count = 1;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }

                // Count reads beyond the document length
                {
                    var expectedString = TestConstants.SOURCE_TEXT[14..];
                    var count = 60;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }
            }

            // PositionIndex resides on the LastRow
            {
                var positionIndex = 26;

                // Count < 0
                {
                    var expectedString = string.Empty;
                    var count = -1;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }

                // Count == 0
                {
                    var expectedString = string.Empty;
                    var count = 0;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }

                // Count > 0
                {
                    var expectedString = "a";
                    var count = 1;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }

                // Count reads beyond the document length
                {
                    var expectedString = TestConstants.SOURCE_TEXT[26..];
                    var count = 60;
                    var actualString = model.GetString(positionIndex, count);
                    Assert.Equal(expectedString, actualString);
                }
            }
        }

        // PositionIndex > 0 && PositionIndex is OUT of bounds
        {
            var positionIndex = 43;

            // Count < 0
            {
                var expectedString = string.Empty;
                var count = -1;
                var actualString = model.GetString(positionIndex, count);
                Assert.Equal(expectedString, actualString);
            }

            // Count == 0
            {
                var expectedString = string.Empty;
                var count = 0;
                var actualString = model.GetString(positionIndex, count);
                Assert.Equal(expectedString, actualString);
            }

            // Count > 0
            {
                var expectedString = string.Empty;
                var count = 1;
                var actualString = model.GetString(positionIndex, count);
                Assert.Equal(expectedString, actualString);
            }

            // Count reads beyond the document length
            {
                var expectedString = string.Empty;
                var count = 60;
                var actualString = model.GetString(positionIndex, count);
                Assert.Equal(expectedString, actualString);
            }
        }
    }

    /// <summary>
	/// <see cref="TextEditorModelHelper.GetLine(ITextEditorModel, int)"/>
	/// </summary>
	[Fact]
    public void GetLine()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var model);

        // rowIndex < 0
        {
            var rowIndex = -1;
            var line = model.GetLine(rowIndex);
            Assert.Equal(string.Empty, line);
        }

        // rowIndex is within bounds
        {
            // rowIndex == 0
            {
                var rowIndex = 0;
                var line = model.GetLine(rowIndex);
                Assert.Equal("Hello World!\n", line);
            }

            //  rowIndex is not the FirstRow nor the LastRow
            {
                var rowIndex = TestConstants.ROW_INDEX_WHICH_IS_BETWEEN_FIRST_AND_LAST_ROW;
                var line = model.GetLine(rowIndex);
                Assert.Equal("7 Pillows\n", line);
            }

            // rowIndex resides on the LastRow
            {
                var rowIndex = TestConstants.LAST_ROW_INDEX;
                var line = model.GetLine(rowIndex);
                Assert.Equal(",abc123", line);
            }
        }

        // rowIndex > 0 && rowIndex is OUT of bounds
        {
            var rowIndex = TestConstants.LARGE_OUT_OF_BOUNDS_ROW_INDEX;
            var line = model.GetLine(rowIndex);
            Assert.Equal(string.Empty, line);
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelHelper.GetLineRange(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
	public void GetLineRange()
	{
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var model);

        // rowIndex < 0
        {
            var rowIndex = -1;

            // Count < 0
            {
                var lines = model.GetLineRange(rowIndex, -1);
                Assert.Equal(string.Empty, lines);
            }

            // Count == 0
            {
                var lines = model.GetLineRange(rowIndex, 0);
                Assert.Equal(string.Empty, lines);
            }

            // Count > 0
            {
                var lines = model.GetLineRange(rowIndex, 1);
                Assert.Equal(string.Empty, lines);
            }

            // Count reads beyond the document length
            {
                var lines = model.GetLineRange(rowIndex, TestConstants.ROW_COUNT + 5);
                Assert.Equal(TestConstants.SOURCE_TEXT, lines);
            }
        }

        // rowIndex is within bounds
        {
            // rowIndex == 0
            {
                var rowIndex = 0;

                // Count < 0
                {
                    var lines = model.GetLineRange(rowIndex, -1);
                    Assert.Equal(string.Empty, lines);
                }

                // Count == 0
                {
                    var lines = model.GetLineRange(rowIndex, 0);
                    Assert.Equal(string.Empty, lines);
                }

                // Count > 0
                {
                    var lines = model.GetLineRange(rowIndex, 1);
                    Assert.Equal("Hello World!\n", lines);
                }

                // Count reads beyond the document length
                {
                    var lines = model.GetLineRange(rowIndex, TestConstants.ROW_COUNT + 5);
                    Assert.Equal(TestConstants.SOURCE_TEXT, lines);
                }
            }

            //  rowIndex is not the FirstRow nor the LastRow
            {
                var rowIndex = TestConstants.ROW_INDEX_WHICH_IS_BETWEEN_FIRST_AND_LAST_ROW;

                // Count < 0
                {
                    var lines = model.GetLineRange(rowIndex, -1);
                    Assert.Equal(string.Empty, lines);
                }

                // Count == 0
                {
                    var lines = model.GetLineRange(rowIndex, 0);
                    Assert.Equal(string.Empty, lines);
                }

                // Count > 0
                {
                    var lines = model.GetLineRange(rowIndex, 1);
                    Assert.Equal("7 Pillows\n", lines);
                }

                // Count reads beyond the document length
                {
                    var lines = model.GetLineRange(rowIndex, TestConstants.ROW_COUNT + 5);
                    Assert.Equal("7 Pillows\n \n,abc123", lines);
                }
            }

            // rowIndex resides on the LastRow
            {
                var rowIndex = TestConstants.LAST_ROW_INDEX;

                // Count < 0
                {
                    var lines = model.GetLineRange(rowIndex, -1);
                    Assert.Equal(string.Empty, lines);
                }

                // Count == 0
                {
                    var lines = model.GetLineRange(rowIndex, 0);
                    Assert.Equal(string.Empty, lines);
                }

                // Count > 0
                {
                    var lines = model.GetLineRange(rowIndex, 1);
                    Assert.Equal(",abc123", lines);
                }

                // Count reads beyond the document length
                {
                    var lines = model.GetLineRange(rowIndex, 1);
                    Assert.Equal(",abc123", lines);
                }
            }
        }

        // rowIndex > 0 && rowIndex is OUT of bounds
        {
            var rowIndex = TestConstants.LARGE_OUT_OF_BOUNDS_ROW_INDEX;

            // Count < 0
            {
                var lines = model.GetLineRange(rowIndex, -1);
                Assert.Equal(string.Empty, lines);
            }

            // Count == 0
            {
                var lines = model.GetLineRange(rowIndex, 0);
                Assert.Equal(string.Empty, lines);
            }

            // Count > 0
            {
                var lines = model.GetLineRange(rowIndex, -1);
                Assert.Equal(string.Empty, lines);
            }

            // Count reads beyond the document length
            {
                var lines = model.GetLineRange(rowIndex, -1);
                Assert.Equal(string.Empty, lines);
            }
        }
	}

	/// <summary>
	/// <see cref="TextEditorModelHelper.GetRowInformationFromPositionIndex(ITextEditorModel, int)"/>
	/// </summary>
	[Fact]
	public void FindRowInformation()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelHelper.GetColumnIndexOfCharacterWithDifferingKind(ITextEditorModel, int, int, bool)"/>
	/// </summary>
	[Fact]
	public void GetColumnIndexOfCharacterWithDifferingKind()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelHelper.CanUndoEdit"/>
	/// </summary>
	[Fact]
	public void CanUndoEdit()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelHelper.CanRedoEdit"/>
	/// </summary>
	[Fact]
	public void CanRedoEdit()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelHelper.ReadPreviousWordOrDefault(ITextEditorModel, int, int, bool)"/>
	/// </summary>
	[Fact]
	public void ReadPreviousWordOrDefault()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelHelper.ReadNextWordOrDefault(ITextEditorModel, int, int)"/>
	/// </summary>
	[Fact]
	public void ReadNextWordOrDefault()
	{
		throw new NotImplementedException();
	}

    /// <summary>
    /// <see cref="TextEditorModelHelper.GetTextOffsettingCursor(ITextEditorModel, TextEditorCursor)"/>
    /// </summary>
    [Fact]
	public void GetTextOffsettingCursor()
	{
		throw new NotImplementedException();
	}
}
