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
    /// <see cref="TextEditorDefaultBinder.DiagnosticsBag"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorDefaultBinder.SymbolsBag"/>
	/// <see cref="TextEditorDefaultBinder.GetDefinition(TextEditorTextSpan)"/>
	/// <see cref="TextEditorDefaultBinder.GetBoundScope(TextEditorTextSpan)"/>
    /// </summary>
    [Fact]
	public void DiagnosticsBag()
	{
        var defaultBinder = new TextEditorDefaultBinder();

        Assert.Equal(ImmutableArray<TextEditorDiagnostic>.Empty, defaultBinder.DiagnosticsBag);
        Assert.Equal(ImmutableArray<ITextEditorSymbol>.Empty, defaultBinder.SymbolsBag);
        Assert.Null(defaultBinder.GetDefinition(TextEditorTextSpan.FabricateTextSpan("unit-test")));
        Assert.Null(defaultBinder.GetBoundScope(TextEditorTextSpan.FabricateTextSpan("unit-test")));
    }
}