using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.CompilerServices.Lang.FSharp.FSharp.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer;

/// <summary>
/// <see cref="GenericLanguageDefinition"/>
/// </summary>
public class GenericLanguageDefinitionTests
{
    /// <summary>
    /// <see cref="GenericLanguageDefinition(string, string, string, string, string, string, ImmutableArray{string}, string, string, ImmutableArray{string}, GenericPreprocessorDefinition)"/>
	/// <br/>----<br/>
    /// <see cref="GenericLanguageDefinition.StringStart"/>
    /// <see cref="GenericLanguageDefinition.StringEnd"/>
	/// <see cref="GenericLanguageDefinition.FunctionInvocationStart"/>
    /// <see cref="GenericLanguageDefinition.FunctionInvocationEnd"/>
    /// <see cref="GenericLanguageDefinition.MemberAccessToken"/>
    /// <see cref="GenericLanguageDefinition.CommentSingleLineStart"/>
    /// <see cref="GenericLanguageDefinition.CommentSingleLineEndingsBag"/>
    /// <see cref="GenericLanguageDefinition.CommentMultiLineStart"/>
    /// <see cref="GenericLanguageDefinition.CommentMultiLineEnd"/>
    /// <see cref="GenericLanguageDefinition.KeywordsBag"/>
    /// <see cref="GenericLanguageDefinition.PreprocessorDefinition"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var fSharpPreprocessorDefinition = new GenericPreprocessorDefinition(
			"#",
			ImmutableArray<DeliminationExtendedSyntaxDefinition>.Empty);

        var stringStart = "\"";
		var stringEnd = "\"";
		var functionInvocationStart = "(";
		var functionInvocationEnd = ")";
		var memberAccessToken = ".";
		var commentSingleLineStart = "//";

		var commentSingleLineEndingsBag = new[]
            {
                WhitespaceFacts.CARRIAGE_RETURN.ToString(),
                WhitespaceFacts.LINE_FEED.ToString()
            }.ToImmutableArray();

		var commentMultiLineStart = "(*";
		var commentMultiLineEnd = "*)";
		var keywordsBag = FSharpKeywords.ALL;
		var preprocessorDefinition = fSharpPreprocessorDefinition;

        GenericLanguageDefinition fSharpLanguageDefinition = new(
            stringStart,
			stringEnd,
			functionInvocationStart,
			functionInvocationEnd,
			memberAccessToken,
			commentSingleLineStart,
            commentSingleLineEndingsBag,
            commentMultiLineStart,
			commentMultiLineEnd,
			keywordsBag,
			preprocessorDefinition);

		Assert.Equal(stringStart, fSharpLanguageDefinition.StringStart);
		Assert.Equal(stringEnd, fSharpLanguageDefinition.StringEnd);
		Assert.Equal(functionInvocationStart, fSharpLanguageDefinition.FunctionInvocationStart);
        Assert.Equal(functionInvocationEnd, fSharpLanguageDefinition.FunctionInvocationEnd);
        Assert.Equal(memberAccessToken, fSharpLanguageDefinition.MemberAccessToken);
        Assert.Equal(commentSingleLineStart, fSharpLanguageDefinition.CommentSingleLineStart);
        Assert.Equal(commentSingleLineEndingsBag, fSharpLanguageDefinition.CommentSingleLineEndingsBag);
        Assert.Equal(commentMultiLineStart, fSharpLanguageDefinition.CommentMultiLineStart);
        Assert.Equal(commentMultiLineEnd, fSharpLanguageDefinition.CommentMultiLineEnd);
        Assert.Equal(keywordsBag, fSharpLanguageDefinition.KeywordsBag);
        Assert.Equal(preprocessorDefinition, fSharpLanguageDefinition.PreprocessorDefinition);
	}
}