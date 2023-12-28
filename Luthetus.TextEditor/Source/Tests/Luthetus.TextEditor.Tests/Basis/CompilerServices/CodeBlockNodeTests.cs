using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

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
    /// <see cref="CodeBlockNode.DiagnosticsBag"/>
    /// <see cref="CodeBlockNode.ChildBag"/>
    /// <see cref="CodeBlockNode.IsFabricated"/>
    /// <see cref="CodeBlockNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var typeDefinitionNode = ConstructTypeDefinitionNode();
        var childBag = new List<ISyntax> { typeDefinitionNode }.ToImmutableArray();

        // No Diagnostics
        {
            var codeBlockNoDiagnostics = new CodeBlockNode(childBag);

            Assert.Empty(codeBlockNoDiagnostics.DiagnosticsBag);
            Assert.Single(codeBlockNoDiagnostics.ChildBag);
            Assert.Equal(typeDefinitionNode, codeBlockNoDiagnostics.ChildBag.Single());
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

            var diagnosticBag = new TextEditorDiagnostic[]
            {
                diagnostic
            }.ToImmutableArray();

            var codeBlockWithDiagnostics = new CodeBlockNode(childBag, diagnosticBag);

            Assert.Single(codeBlockWithDiagnostics.DiagnosticsBag);
            Assert.Single(codeBlockWithDiagnostics.ChildBag);
            Assert.Equal(typeDefinitionNode, codeBlockWithDiagnostics.ChildBag.Single());
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
            typeIdentifier,
            valueType,
            genericArgumentsListingNode,
            inheritedTypeClauseNode,
            typeBodyCodeBlockNode);
    }
}