using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorModels;

public class TextEditorModelHelperTests
{
	[Fact]
	public void GetStartOfRowTuple()
	{
		//public static (int positionIndex, RowEndingKind rowEndingKind) GetStartOfRowTuple(
		//	this ITextEditorModel model, int rowIndex)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetLengthOfRow()
	{
		//public static int GetLengthOfRow(
		//	this ITextEditorModel model, int rowIndex, bool includeLineEndingCharacters = false)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetRows()
	{
		//public static List<List<RichCharacter>> GetRows(
		//	this ITextEditorModel model, int startingRowIndex, int count)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetTabsCountOnSameRowBeforeCursor()
	{
		//public static int GetTabsCountOnSameRowBeforeCursor(
		//	this ITextEditorModel model, int rowIndex, int columnIndex)
		throw new NotImplementedException();
	}
	
	[Fact]
	public void ApplyDecorationRange()
	{
		//public static void ApplyDecorationRange(
		//	this ITextEditorModel model, 
		//	IEnumerable<TextEditorTextSpan> textEditorTextSpans)
		throw new NotImplementedException();
	}

	[Fact]
	public void ApplySyntaxHighlightingAsync()
	{
		//public static Task ApplySyntaxHighlightingAsync(this ITextEditorModel model)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetAllText()
	{
		//public static string GetAllText(this ITextEditorModel model)
		throw new NotImplementedException();
	}
	
	[Fact]
	public void GetCursorPositionIndex_A()
	{
		//public static int GetCursorPositionIndex(
		//	this ITextEditorModel model, TextEditorCursor textEditorCursor)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetCursorPositionIndex_B()
	{
		//public static int GetCursorPositionIndex(
		//	this ITextEditorModel model, ImmutableTextEditorCursor immutableTextEditorCursor)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetPositionIndex()
	{
		//public static int GetPositionIndex(
		//	this ITextEditorModel model, int rowIndex, int columnIndex)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetTextAt()
	{
		//public static char GetTextAt(
		//	this ITextEditorModel model,
		//	int positionIndex)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetTextRange()
	{
		//public static string GetTextRange(
		//	this ITextEditorModel model,
		//	int startingPositionIndex,
		//	int count)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetLinesRange()
	{
		//public static string GetLinesRange(this ITextEditorModel model, int startingRowIndex, int count)
		throw new NotImplementedException();
	}


	[Fact]
	public void GetWordAt()
	{
		//public static TextEditorTextSpan? GetWordAt(this ITextEditorModel model, int positionIndex)
		throw new NotImplementedException();
	}

	[Fact]
	public void FindRowInformation()
	{
		//public static (int rowIndex, int rowStartPositionIndex, (int positionIndex, RowEndingKind rowEndingKind) rowEndingTuple)
		//	FindRowInformation(this ITextEditorModel model, int positionIndex)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetColumnIndexOfCharacterWithDifferingKind()
	{
		//public static int GetColumnIndexOfCharacterWithDifferingKind(
		//	this ITextEditorModel model, int rowIndex, int columnIndex, bool moveBackwards)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetAllRichCharacters()
	{
		//public static ImmutableArray<RichCharacter> GetAllRichCharacters(this ITextEditorModel model)
		throw new NotImplementedException();
	}

	[Fact]
	public void CanUndoEdit()
	{
		//public static bool CanUndoEdit(this ITextEditorModel model)
		throw new NotImplementedException();
	}

	[Fact]
	public void CanRedoEdit()
	{
		//public static bool CanRedoEdit(this ITextEditorModel model)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetCharacterKindAt()
	{
		//public static CharacterKind GetCharacterKindAt(this ITextEditorModel model, int positionIndex)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReadPreviousWordOrDefault()
	{
		//public static string? ReadPreviousWordOrDefault(
		//	this ITextEditorModel model, int rowIndex, int columnIndex, bool isRecursiveCall = false)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReadNextWordOrDefault()
	{
		//public static string? ReadNextWordOrDefault(
		//	this ITextEditorModel model, int rowIndex, int columnIndex)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetTextOffsettingCursor()
	{
		//public static string GetTextOffsettingCursor(
		//	this ITextEditorModel model, TextEditorCursor textEditorCursor)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetTextOnRow()
	{
		//public static string GetTextOnRow(
		//	this ITextEditorModel model, int rowIndex)
		throw new NotImplementedException();
	}
}
