using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="VariableExpressionNode"/>
/// </summary>
public class VariableExpressionNodeTests
{
    /// <summary>
    /// <see cref="VariableExpressionNode(TypeClauseNode)"/>
    /// <br/>----<br/>
    /// <see cref="VariableExpressionNode.ResultTypeClauseNode"/>
    /// <see cref="VariableExpressionNode.ChildList"/>
    /// <see cref="VariableExpressionNode.IsFabricated"/>
    /// <see cref="VariableExpressionNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = @"int x = 2;
MyMethod(x)";
        _ = sourceText; // Suppress unused variable warning

        TypeClauseNode intTypeClauseNode;
        {
            var intTypeIdentifier = new KeywordToken(
                TextEditorTextSpan.FabricateTextSpan("int"),
                RazorLib.CompilerServices.Syntax.SyntaxKind.IntTokenKeyword);

            intTypeClauseNode = new TypeClauseNode(
                intTypeIdentifier,
                typeof(int),
                null);
        }

        var variableExpressionNode = new VariableExpressionNode(
            intTypeClauseNode);

        Assert.Equal(intTypeClauseNode, variableExpressionNode.ResultTypeClauseNode);

        Assert.Single(variableExpressionNode.ChildList);
        Assert.Equal(intTypeClauseNode, variableExpressionNode.ChildList.Single());

        Assert.False(variableExpressionNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.VariableExpressionNode,
            variableExpressionNode.SyntaxKind);
	}
}
