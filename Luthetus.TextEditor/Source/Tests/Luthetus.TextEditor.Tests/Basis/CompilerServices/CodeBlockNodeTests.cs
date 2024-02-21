using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Enums;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="CodeBlockNode"/>
/// </summary>
public class CodeBlockNodeTests
{
    /// <summary>
    /// <see cref="CodeBlockNode(ImmutableArray{ISyntax})"/>
    /// <br/>----<br/>
    /// <see cref="CodeBlockNode(ImmutableArray{ISyntax}, ImmutableArray{TextEditorDiagnostic})"/>
    /// <see cref="CodeBlockNode.DiagnosticsList"/>
    /// <see cref="CodeBlockNode.ChildList"/>
    /// <see cref="CodeBlockNode.IsFabricated"/>
    /// <see cref="CodeBlockNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var typeDefinitionNode = ConstructTypeDefinitionNode();
        var childList = new List<ISyntax> { typeDefinitionNode }.ToImmutableArray();

        // No Diagnostics
        {
            var codeBlockNoDiagnostics = new CodeBlockNode(childList);

            Assert.Empty(codeBlockNoDiagnostics.DiagnosticsList);
            Assert.Single(codeBlockNoDiagnostics.ChildList);
            Assert.Equal(typeDefinitionNode, codeBlockNoDiagnostics.ChildList.Single());
            Assert.Equal(SyntaxKind.CodeBlockNode, codeBlockNoDiagnostics.SyntaxKind);
            Assert.False(codeBlockNoDiagnostics.IsFabricated);
        }

        // With Diagnostics
        {
            var diagnostic = new TextEditorDiagnostic(
                TextEditorDiagnosticLevel.Error,
                "Error",
                TextEditorTextSpan.FabricateTextSpan("Hello World!"),
                Guid.NewGuid());

            var diagnosticList = new TextEditorDiagnostic[]
            {
                diagnostic
            }.ToImmutableArray();

            var codeBlockWithDiagnostics = new CodeBlockNode(childList, diagnosticList);

            Assert.Single(codeBlockWithDiagnostics.DiagnosticsList);
            Assert.Single(codeBlockWithDiagnostics.ChildList);
            Assert.Equal(typeDefinitionNode, codeBlockWithDiagnostics.ChildList.Single());
            Assert.Equal(SyntaxKind.CodeBlockNode, codeBlockWithDiagnostics.SyntaxKind);
            Assert.False(codeBlockWithDiagnostics.IsFabricated);
        }
    }

    private TypeDefinitionNode ConstructTypeDefinitionNode()
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

        return new TypeDefinitionNode(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            typeIdentifier,
            valueType,
            genericArgumentsListingNode,
            null,
            inheritedTypeClauseNode,
            typeBodyCodeBlockNode);
    }
}