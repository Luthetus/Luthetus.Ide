using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="IfStatementNode"/>
/// </summary>
public class IfStatementNodeTests
{
    /// <summary>
    /// <see cref="IfStatementNode(KeywordToken, IExpressionNode, RazorLib.CompilerServices.CodeBlockNode?)"/>
    /// <br/>----<br/>
    /// <see cref="IfStatementNode.KeywordToken"/>
    /// <see cref="IfStatementNode.ExpressionNode"/>
    /// <see cref="IfStatementNode.IfStatementBodyCodeBlockNode"/>
    /// <see cref="IfStatementNode.ChildList"/>
    /// <see cref="IfStatementNode.IsFabricated"/>
    /// <see cref="IfStatementNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = @"if (true)
{
}";

        KeywordToken ifKeywordToken;
        {
            var ifKeywordText = "if";
            int indexOfIfKeywordText = sourceText.IndexOf(ifKeywordText);

            ifKeywordToken = new KeywordToken(new TextEditorTextSpan(
                indexOfIfKeywordText,
                indexOfIfKeywordText + ifKeywordText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText),
                SyntaxKind.IfTokenKeyword);
        }

        IExpressionNode expressionNode;
        {
            var trueKeywordText = "true";
            int indexOfTrueKeywordText = sourceText.IndexOf(trueKeywordText);
            var trueKeyword = new KeywordToken(new TextEditorTextSpan(
                indexOfTrueKeywordText,
                indexOfTrueKeywordText + trueKeywordText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText),
                SyntaxKind.TrueTokenKeyword);

            TypeClauseNode boolTypeClauseNode;
            {
                var boolTypeIdentifier = new KeywordToken(
                    TextEditorTextSpan.FabricateTextSpan("bool"),
                    SyntaxKind.BoolTokenKeyword);

                boolTypeClauseNode = new TypeClauseNode(
                    boolTypeIdentifier,
                    typeof(bool),
                    null);
            }

            expressionNode = new LiteralExpressionNode(
                trueKeyword,
                boolTypeClauseNode);
        }

        CodeBlockNode ifStatementBodyCodeBlockNode = new(ImmutableArray<ISyntax>.Empty);

        var ifStatementNode = new IfStatementNode(
            ifKeywordToken,
            expressionNode,
            ifStatementBodyCodeBlockNode);

        Assert.Equal(ifKeywordToken, ifStatementNode.KeywordToken);
        Assert.Equal(expressionNode, ifStatementNode.ExpressionNode);
        Assert.Equal(ifStatementBodyCodeBlockNode, ifStatementNode.IfStatementBodyCodeBlockNode);

        Assert.Equal(3, ifStatementNode.ChildList.Length);
        Assert.Equal(ifKeywordToken, ifStatementNode.ChildList[0]);
        Assert.Equal(expressionNode, ifStatementNode.ChildList[1]);
        Assert.Equal(ifStatementBodyCodeBlockNode, ifStatementNode.ChildList[2]);

        Assert.False(ifStatementNode.IsFabricated);

        Assert.Equal(
            SyntaxKind.IfStatementNode,
            ifStatementNode.SyntaxKind);
	}
}