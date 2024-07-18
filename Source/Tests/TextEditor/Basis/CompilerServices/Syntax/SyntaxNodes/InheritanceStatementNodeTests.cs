using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="InheritanceStatementNode"/>
/// </summary>
public class InheritanceStatementNodeTests
{
    /// <summary>
    /// <see cref="InheritanceStatementNode(TypeClauseNode)"/>
    /// <br/>----<br/>
    /// <see cref="InheritanceStatementNode.ParentTypeClauseNode"/>
    /// <see cref="InheritanceStatementNode.ChildList"/>
    /// <see cref="InheritanceStatementNode.IsFabricated"/>
    /// <see cref="InheritanceStatementNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = @"public partial class ContextBoundary : ComponentBase
{
}";
        _ = sourceText; // Supress unused variable

        TypeClauseNode componentBaseTypeClauseNode;
        {
            var componentBaseTypeIdentifier = new IdentifierToken(
                TextEditorTextSpan.FabricateTextSpan("ComponentBase"));

            componentBaseTypeClauseNode = new TypeClauseNode(
                componentBaseTypeIdentifier,
                null,
                null);
        }

        var inheritanceStatementNode = new InheritanceStatementNode(
            componentBaseTypeClauseNode);

        Assert.Equal(componentBaseTypeClauseNode, inheritanceStatementNode.ParentTypeClauseNode);

        Assert.Single(inheritanceStatementNode.ChildList);
        Assert.Equal(componentBaseTypeClauseNode, inheritanceStatementNode.ChildList[0]);

        Assert.False(inheritanceStatementNode.IsFabricated);

        Assert.Equal(
            SyntaxKind.InheritanceStatementNode,
            inheritanceStatementNode.SyntaxKind);
	}
}