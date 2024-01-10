using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="IdempotentExpressionNode"/>
/// </summary>
public class IdempotentExpressionNodeTests
{
    /// <summary>
    /// <see cref="IdempotentExpressionNode.IdempotentExpressionNode"/>
    /// <br/>----<br/>
    /// <see cref="IdempotentExpressionNode.ResultTypeClauseNode"/>
    /// <see cref="IdempotentExpressionNode.ChildList"/>
    /// <see cref="IdempotentExpressionNode.IsFabricated"/>
    /// <see cref="IdempotentExpressionNode.SyntaxKind"/>
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

        var idempotentExpressionNode = new IdempotentExpressionNode(voidTypeClauseNode);

        Assert.Equal(voidTypeClauseNode, idempotentExpressionNode.ResultTypeClauseNode);

        Assert.Single(idempotentExpressionNode.ChildList);
        Assert.Equal(voidTypeClauseNode, idempotentExpressionNode.ChildList.Single());

        Assert.False(idempotentExpressionNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.IdempotentExpressionNode,
            idempotentExpressionNode.SyntaxKind);
	}
}