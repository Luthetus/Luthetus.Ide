using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="VariableSymbol"/>
/// </summary>
public class VariableSymbolTests
{
    /// <summary>
    /// <see cref="VariableSymbol(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="VariableSymbol.TextSpan"/>
    /// <see cref="VariableSymbol.SymbolKindString"/>
    /// <see cref="VariableSymbol.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var textSpan = TextEditorTextSpan.FabricateTextSpan("$");
        var variableSymbol = new VariableSymbol(textSpan);

        Assert.Equal(textSpan, variableSymbol.TextSpan);
        Assert.Equal(SyntaxKind.VariableSymbol, variableSymbol.SyntaxKind);
        Assert.Equal(variableSymbol.SyntaxKind.ToString(), variableSymbol.SymbolKindString);
    }
}