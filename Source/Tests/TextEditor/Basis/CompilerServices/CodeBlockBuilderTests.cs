using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="CodeBlockBuilder"/>
/// </summary>
public class CodeBlockBuilderTests
{
    /// <summary>
    /// <see cref="CodeBlockBuilder(CodeBlockBuilder?, RazorLib.CompilerServices.Syntax.ISyntaxNode?)"/>
    /// <br/>----<br/>
	/// <see cref="CodeBlockBuilder.ChildList"/>
    /// <see cref="CodeBlockBuilder.Parent"/>
    /// <see cref="CodeBlockBuilder.CodeBlockOwner"/>
    /// <see cref="CodeBlockBuilder.Build()"/>
    /// <see cref="CodeBlockBuilder.Build(ImmutableArray{TextEditorDiagnostic})"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var globalCodeBlockBuilder = new CodeBlockBuilder(null, null);

		Assert.Empty(globalCodeBlockBuilder.ChildList);
		Assert.Null(globalCodeBlockBuilder.Parent);
		Assert.Null(globalCodeBlockBuilder.CodeBlockOwner);

		var typeDefinitionNode = ConstructTypeDefinitionNode();
        globalCodeBlockBuilder.ChildList.Add(typeDefinitionNode);
        Assert.Single(globalCodeBlockBuilder.ChildList);

        // No Diagnostics
        {
            var codeBlockNoDiagnostics = globalCodeBlockBuilder.Build();

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

            var codeBlockWithDiagnostics = globalCodeBlockBuilder.Build(diagnosticList);

            Assert.Single(codeBlockWithDiagnostics.DiagnosticsList);
            Assert.Single(codeBlockWithDiagnostics.ChildList);
            Assert.Equal(typeDefinitionNode, codeBlockWithDiagnostics.ChildList.Single());
            Assert.Equal(SyntaxKind.CodeBlockNode, codeBlockWithDiagnostics.SyntaxKind);
            Assert.False(codeBlockWithDiagnostics.IsFabricated);
        }

		var typeDefinitionCodeBlockBuilder = new CodeBlockBuilder(
			globalCodeBlockBuilder,
			typeDefinitionNode);

		Assert.Equal(globalCodeBlockBuilder, typeDefinitionCodeBlockBuilder.Parent);
		Assert.Equal(typeDefinitionNode, typeDefinitionCodeBlockBuilder.CodeBlockOwner);
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
            openBraceToken: null,
            typeBodyCodeBlockNode);
    }
}