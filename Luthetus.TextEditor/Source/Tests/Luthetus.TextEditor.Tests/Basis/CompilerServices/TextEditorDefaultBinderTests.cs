using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="TextEditorDefaultBinder"/>
/// </summary>
public class TextEditorDefaultBinderTests
{
    /// <summary>
    /// <see cref="TextEditorDefaultBinder.DiagnosticsList"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorDefaultBinder.SymbolsList"/>
	/// <see cref="TextEditorDefaultBinder.GetDefinition(TextEditorTextSpan)"/>
	/// <see cref="TextEditorDefaultBinder.GetBoundScope(TextEditorTextSpan)"/>
    /// </summary>
    [Fact]
	public void DiagnosticsList()
	{
        var defaultBinder = new TextEditorDefaultBinder();

        Assert.Equal(ImmutableArray<TextEditorDiagnostic>.Empty, defaultBinder.DiagnosticsList);
        Assert.Equal(ImmutableArray<ITextEditorSymbol>.Empty, defaultBinder.SymbolsList);
        Assert.Null(defaultBinder.GetDefinition(TextEditorTextSpan.FabricateTextSpan("unit-test")));
        Assert.Null(defaultBinder.GetBoundScope(TextEditorTextSpan.FabricateTextSpan("unit-test")));
    }
}