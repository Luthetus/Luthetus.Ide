using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="EmptyExpressionNode"/>
/// </summary>
public class EmptyExpressionNodeTests
{
    /// <summary>
    /// <see cref="EmptyExpressionNode(TypeClauseNode)"/>
    /// <br/>----<br/>
    /// <see cref="EmptyExpressionNode.ResultTypeClauseNode"/>
    /// <see cref="EmptyExpressionNode.ChildList"/>
    /// <see cref="EmptyExpressionNode.IsFabricated"/>
    /// <see cref="EmptyExpressionNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = "()";
        _ = sourceText; // Suppress unused variable

        var voidTypeIdentifier = new KeywordToken(
            TextEditorTextSpan.FabricateTextSpan("void"),
                RazorLib.CompilerServices.Syntax.SyntaxKind.VoidTokenKeyword);

        var voidTypeClauseNode = new TypeClauseNode(
            voidTypeIdentifier,
            typeof(void),
            null);

        var emptyExpressionNode = new EmptyExpressionNode(voidTypeClauseNode);

        Assert.Equal(voidTypeClauseNode, emptyExpressionNode.ResultTypeClauseNode);

        Assert.Single(emptyExpressionNode.ChildList);
        Assert.Equal(voidTypeClauseNode, emptyExpressionNode.ChildList.Single());

        Assert.False(emptyExpressionNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.EmptyExpressionNode,
            emptyExpressionNode.SyntaxKind);
	}
}