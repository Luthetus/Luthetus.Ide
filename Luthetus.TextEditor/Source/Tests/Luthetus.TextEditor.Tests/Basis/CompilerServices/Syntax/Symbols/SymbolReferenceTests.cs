using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="SymbolReference"/>
/// </summary>
public class SymbolReferenceTests
{
    /// <summary>
    /// <see cref="SymbolReference(ISymbol, BoundScopeKey)"/>
    /// <br/>----<br/>
    /// <see cref="SymbolReference.Symbol"/>
    /// <see cref="SymbolReference.BoundScopeKey"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var type = "int";
        var name = "myVariable";
        var sourceText = $@"{type} {name} = 2;
{name} = 7;";

        // The first occurrence of 'name' variable will be returned, thus we get the definition considering the hardcoded 'sourceText'
        var startingIndexOfSymbolDefinitionInclusive = sourceText.IndexOf(name);
        var endingIndexOfSymbolDefinitionExclusive = startingIndexOfSymbolDefinitionInclusive + name.Length;

        var boundScopeKey = BoundScopeKey.NewKey();
        var resourceUri = new ResourceUri("/unitTesting.txt");

        var remainingSourceText = sourceText[endingIndexOfSymbolDefinitionExclusive..];

        var startingIndexOfSymbolReferenceInclusive = remainingSourceText.IndexOf(name);
        var endingIndexOfSymbolReferenceExclusive = startingIndexOfSymbolReferenceInclusive + name.Length;

        var variableSymbolReference = new VariableSymbol(new TextEditorTextSpan(
            startingIndexOfSymbolReferenceInclusive,
            endingIndexOfSymbolReferenceExclusive,
            0,
            resourceUri,
            sourceText));

        var symbolReference = new SymbolReference(variableSymbolReference, boundScopeKey);

        Assert.Equal(boundScopeKey, symbolReference.BoundScopeKey);
        Assert.Equal(variableSymbolReference, symbolReference.Symbol);
    }
}