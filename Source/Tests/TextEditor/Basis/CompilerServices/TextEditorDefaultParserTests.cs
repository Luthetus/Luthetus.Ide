using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="TextEditorDefaultParser"/>
/// </summary>
public class TextEditorDefaultParserTests
{
	/// <summary>
	/// <see cref="TextEditorDefaultParser.DiagnosticsList"/>
	/// </summary>
	[Fact]
	public void DiagnosticsList()
	{
		var defaultLexer = new Lexer(
			new ResourceUri(string.Empty),
            string.Empty,
			LexerKeywords.Empty);

        var defaultParser = new Parser(defaultLexer);

        Assert.Equal(
            ImmutableArray<TextEditorDiagnostic>.Empty,
            defaultParser.DiagnosticsList);
	}
}