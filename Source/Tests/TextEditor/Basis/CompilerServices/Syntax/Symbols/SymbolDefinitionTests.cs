using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="SymbolDefinition"/>
/// </summary>
public class SymbolDefinitionTests
{
    /// <summary>
    /// <see cref="SymbolDefinition(BoundScopeKey, ISymbol)"/>
    /// <br/>----<br/>
    /// <see cref="SymbolDefinition.SymbolReferences"/>
    /// <see cref="SymbolDefinition.BoundScopeKey"/>
    /// <see cref="SymbolDefinition.Symbol"/>
    /// <see cref="SymbolDefinition.IsFabricated"/>
    /// <see cref="SymbolDefinition.GetSymbolReferences()"/>
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

        var variableSymbolDefinition = new VariableSymbol(new TextEditorTextSpan(
            startingIndexOfSymbolDefinitionInclusive,
            endingIndexOfSymbolDefinitionExclusive,
            0,
            resourceUri,
            sourceText));

        var symbolDefinition = new SymbolDefinition(boundScopeKey, variableSymbolDefinition);

        Assert.Equal(boundScopeKey, symbolDefinition.BoundScopeKey);
        Assert.Equal(variableSymbolDefinition, symbolDefinition.Symbol);
        Assert.Empty(symbolDefinition.SymbolReferences);
        Assert.False(symbolDefinition.IsFabricated);

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

        symbolDefinition.SymbolReferences.Add(symbolReference);
        Assert.Single(symbolDefinition.SymbolReferences);
        Assert.Equal(symbolReference, symbolDefinition.SymbolReferences.Single());
	}
}