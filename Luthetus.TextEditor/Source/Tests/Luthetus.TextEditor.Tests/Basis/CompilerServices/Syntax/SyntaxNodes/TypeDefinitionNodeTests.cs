using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="TypeDefinitionNode"/>
/// </summary>
public class TypeDefinitionNodeTests
{
    /// <summary>
    /// <see cref="TypeDefinitionNode(IdentifierToken, Type?, RazorLib.CompilerServices.Syntax.SyntaxNodes.GenericArgumentsListingNode?, TypeClauseNode?, RazorLib.CompilerServices.CodeBlockNode?)"/>
    /// <br/>----<br/>
    /// <see cref="TypeDefinitionNode.TypeIdentifier"/>
    /// <see cref="TypeDefinitionNode.ValueType"/>
    /// <see cref="TypeDefinitionNode.GenericArgumentsListingNode"/>
    /// <see cref="TypeDefinitionNode.InheritedTypeClauseNode"/>
    /// <see cref="TypeDefinitionNode.TypeBodyCodeBlockNode"/>
    /// <see cref="TypeDefinitionNode.IsInterface"/>
    /// <see cref="TypeDefinitionNode.ChildList"/>
    /// <see cref="TypeDefinitionNode.IsFabricated"/>
    /// <see cref="TypeDefinitionNode.SyntaxKind"/>
    /// <see cref="TypeDefinitionNode.GetFunctionDefinitionNodes()"/>
    /// <see cref="TypeDefinitionNode.ToTypeClause()"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = @"public class MyClass
{
}";
        IdentifierToken typeIdentifier;
        {
            var typeIdentifierText = "MyClass";
            int indexOfTypeIdentifierText = sourceText.IndexOf(typeIdentifierText);
            
            typeIdentifier = new IdentifierToken(new TextEditorTextSpan(
                indexOfTypeIdentifierText,
                indexOfTypeIdentifierText + typeIdentifierText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        Type? valueType = null;

        GenericArgumentsListingNode? genericArgumentsListingNode = null;

        TypeClauseNode? inheritedTypeClauseNode = null;

        CodeBlockNode? typeBodyCodeBlockNode = new(ImmutableArray<ISyntax>.Empty);

        var typeDefinitionNode = new TypeDefinitionNode(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            typeIdentifier,
            valueType,
            genericArgumentsListingNode,
            null,
            inheritedTypeClauseNode,
            typeBodyCodeBlockNode);

        Assert.Equal(typeIdentifier, typeDefinitionNode.TypeIdentifier);
        Assert.Equal(valueType, typeDefinitionNode.ValueType);
        Assert.Equal(genericArgumentsListingNode, typeDefinitionNode.GenericArgumentsListingNode);
        Assert.Equal(inheritedTypeClauseNode, typeDefinitionNode.InheritedTypeClauseNode);
        Assert.Equal(typeBodyCodeBlockNode, typeDefinitionNode.TypeBodyCodeBlockNode);
        Assert.False(typeDefinitionNode.IsInterface);
        Assert.Empty(typeDefinitionNode.GetFunctionDefinitionNodes());
        Assert.Equal(typeIdentifier, typeDefinitionNode.ToTypeClause().TypeIdentifierToken);
        Assert.Equal(valueType, typeDefinitionNode.ToTypeClause().ValueType);
        Assert.Null(typeDefinitionNode.ToTypeClause().GenericParametersListingNode);

        Assert.Equal(2, typeDefinitionNode.ChildList.Length);
        Assert.Equal(typeIdentifier, typeDefinitionNode.ChildList[0]);
        Assert.Equal(typeBodyCodeBlockNode, typeDefinitionNode.ChildList[1]);

        Assert.False(typeDefinitionNode.IsFabricated);

        Assert.Equal(
            SyntaxKind.TypeDefinitionNode,
            typeDefinitionNode.SyntaxKind);
	}
}