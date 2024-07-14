using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="LuthBinder"/>
/// </summary>
public class TextEditorDefaultBinderTests
{
    /// <summary>
    /// <see cref="LuthBinder.DiagnosticsList"/>
    /// <br/>----<br/>
    /// <see cref="LuthBinder.SymbolsList"/>
	/// <see cref="LuthBinder.GetDefinition(TextEditorTextSpan)"/>
	/// <see cref="LuthBinder.GetBoundScope(TextEditorTextSpan)"/>
    /// </summary>
    [Fact]
	public void DiagnosticsList()
	{
        var defaultBinder = new Binder();

        Assert.Equal(ImmutableArray<TextEditorDiagnostic>.Empty, defaultBinder.DiagnosticsList);
        Assert.Equal(ImmutableArray<ITextEditorSymbol>.Empty, defaultBinder.SymbolsList);
        Assert.Null(defaultBinder.GetDefinition(TextEditorTextSpan.FabricateTextSpan("unit-test")));
        Assert.Null(defaultBinder.GetBoundScope(TextEditorTextSpan.FabricateTextSpan("unit-test")));
    }
}