using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="ConstraintNode"/>
/// </summary>
public class ConstraintNodeTests
{
    /// <summary>
    /// <see cref="ConstraintNode(ImmutableArray{ISyntaxToken})"/>
    /// <br/>----<br/>
    /// <see cref="ConstraintNode.InnerTokens"/>
    /// <see cref="ConstraintNode.ChildList"/>
    /// <see cref="ConstraintNode.IsFabricated"/>
    /// <see cref="ConstraintNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var constraintText = @"where T : notnull";
        var sourceText = $@"public abstract class TreeViewWithType<T> : TreeViewNoType {constraintText}
{{
}}";

        KeywordContextualToken whereKeywordToken;
        {
            var whereKeywordText = "where";
            int indexOfWhereKeywordText = sourceText.IndexOf(whereKeywordText);
            
            whereKeywordToken = new KeywordContextualToken(new TextEditorTextSpan(
                indexOfWhereKeywordText,
                indexOfWhereKeywordText + whereKeywordText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText),
                SyntaxKind.WhereTokenContextualKeyword);
        }
        
        IdentifierToken identifierToken;
        {
            var identifierText = "T";
            int indexOfIdentifierText = sourceText.IndexOf(identifierText);

            identifierToken = new IdentifierToken(new TextEditorTextSpan(
                indexOfIdentifierText,
                indexOfIdentifierText + identifierText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }
        
        ColonToken colonToken;
        {
            var colonText = "T";
            int indexOfColonText = sourceText.IndexOf(colonText);

            colonToken = new ColonToken(new TextEditorTextSpan(
                indexOfColonText,
                indexOfColonText + colonText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        KeywordContextualToken notnullKeywordToken;
        {
            var notnullKeywordText = "notnull";
            int indexOfNotnullKeywordText = sourceText.IndexOf(notnullKeywordText);

            notnullKeywordToken = new KeywordContextualToken(new TextEditorTextSpan(
                indexOfNotnullKeywordText,
                indexOfNotnullKeywordText + notnullKeywordText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText),
                SyntaxKind.NotnullTokenContextualKeyword);
        }

        ImmutableArray<ISyntaxToken> innerTokens = new List<ISyntaxToken>
        {
            whereKeywordToken,
            identifierToken,
            colonToken,
            notnullKeywordToken,
        }.ToImmutableArray();

        var constraintNode = new ConstraintNode(innerTokens);

        Assert.Equal(innerTokens, constraintNode.InnerTokens);

        Assert.Equal(innerTokens.Length, constraintNode.ChildList.Length);
        Assert.Equal(whereKeywordToken, constraintNode.ChildList[0]);
        Assert.Equal(identifierToken, constraintNode.ChildList[1]);
        Assert.Equal(colonToken, constraintNode.ChildList[2]);
        Assert.Equal(notnullKeywordToken, constraintNode.ChildList[3]);

        Assert.False(constraintNode.IsFabricated);

        Assert.Equal(
            SyntaxKind.ConstraintNode,
            constraintNode.SyntaxKind);
    }
}