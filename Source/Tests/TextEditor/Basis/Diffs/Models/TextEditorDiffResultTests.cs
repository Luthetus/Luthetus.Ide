using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;

namespace Luthetus.TextEditor.Tests.Basis.Diffs.Models;

/// <summary>
/// <see cref="TextEditorDiffResult"/>
/// </summary>
public class TextEditorDiffResultTests
{
    /// <summary>
    /// <see cref="TextEditorDiffResult(string, string, TextEditorDiffCell[,], ValueTuple{int, int, int}, string, ImmutableList{TextEditorTextSpan}, ImmutableList{TextEditorTextSpan})"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorDiffResult.InText"/>
    /// <see cref="TextEditorDiffResult.OutText"/>
    /// <see cref="TextEditorDiffResult.DiffMatrix"/>
    /// <see cref="TextEditorDiffResult.HighestSourceWeightTuple"/>
    /// <see cref="TextEditorDiffResult.LongestCommonSubsequence"/>
    /// <see cref="TextEditorDiffResult.InResultTextSpanList"/>
    /// <see cref="TextEditorDiffResult.OutResultTextSpanList"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorDiffResult.Calculate(ResourceUri, string, ResourceUri, string)"/>
	/// </summary>
	[Fact]
	public void Calculate()
	{
		throw new NotImplementedException();
	}
}