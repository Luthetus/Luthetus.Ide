using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.Diffs.Models;

public class TextEditorDiffResultTests
{
	[Fact]
	public void TextEditorDiffResult()
	{
		//private TextEditorDiffResult(
	 //       string inText,
	 //       string outText,
	 //       TextEditorDiffCell[,] diffMatrix,
	 //       (int sourceWeight, int inIndex, int outIndex) highestSourceWeightTuple,
	 //       string longestCommonSubsequence,
	 //       ImmutableList<TextEditorTextSpan> inLongestCommonSubsequenceTextSpanBag,
	 //       ImmutableList<TextEditorTextSpan> outLongestCommonSubsequenceTextSpanBag)
		throw new NotImplementedException();
	}

	[Fact]
	public void InText()
	{
		//public string InText { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void OutText()
	{
		//public string OutText { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void DiffMatrix()
	{
		//public TextEditorDiffCell[,] DiffMatrix { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void HighestSourceWeightTuple()
	{
		//public (int sourceWeight, int beforeIndex, int afterIndex) HighestSourceWeightTuple { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void LongestCommonSubsequence()
	{
		//public string LongestCommonSubsequence { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void InResultTextSpanBag()
	{
		//public ImmutableList<TextEditorTextSpan> InResultTextSpanBag { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void OutResultTextSpanBag()
	{
		//public ImmutableList<TextEditorTextSpan> OutResultTextSpanBag { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void Calculate()
	{
		//public static TextEditorDiffResult Calculate(
	 //       ResourceUri inResourceUri,
	 //       string inText,
	 //       ResourceUri outResourceUri,
	 //       string outText)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetLargestWeightPriorToCurrentPosition()
	{
		//private static int GetLargestWeightPriorToCurrentPosition(
	 //       TextEditorDiffCell[,] diffMatrix,
	 //       int inIndex,
	 //       int outIndex)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetTextSpans()
	{
		//private static List<TextEditorTextSpan> GetTextSpans(
	 //       ResourceUri resourceUri,
	 //       string sourceText,
	 //       HashSet<int> positionIndicesHashSet,
	 //       byte decorationByte)
		throw new NotImplementedException();
	}
}