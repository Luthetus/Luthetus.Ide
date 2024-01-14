using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using System.Collections.Immutable;

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
        var defaultParser = new TextEditorDefaultParser();

        Assert.Equal(
            ImmutableArray<TextEditorDiagnostic>.Empty,
            defaultParser.DiagnosticsList);
	}
}