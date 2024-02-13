using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="TextEditorDefaultLexer"/>
/// </summary>
public class TextEditorDefaultLexerTests
{
	/// <summary>
	/// <see cref="TextEditorDefaultLexer.DiagnosticList"/>
	/// </summary>
	[Fact]
	public void DiagnosticsList()
	{
		var defaultLexer = new LuthLexer(null, null, null);

		Assert.Equal(
			ImmutableArray<TextEditorDiagnostic>.Empty,
			defaultLexer.DiagnosticList);
	}
}