using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

public static class UtilityApi
{
    public static TypeClauseNode MatchTypeClause(ParserModel model)
    {
        ISyntaxToken syntaxToken;

        if (IsKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind) &&
                (IsTypeIdentifierKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind) ||
                IsVarContextualKeyword(model, model.TokenWalker.Current.SyntaxKind)))
        {
            syntaxToken = model.TokenWalker.Consume();
        }
        else
        {
            syntaxToken = model.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

        var typeClauseNode = new TypeClauseNode(
            syntaxToken,
            null,
            null);

        typeClauseNode = model.Binder.BindTypeClauseNode(typeClauseNode);

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            var openAngleBracketToken = (OpenAngleBracketToken)model.TokenWalker.Consume();

            model.SyntaxStack.Push(openAngleBracketToken);
            SyntaxApi.HandleGenericParameters(model);

            var genericParametersListingNode = (GenericParametersListingNode)model.SyntaxStack.Pop();

            typeClauseNode = new TypeClauseNode(
                typeClauseNode.TypeIdentifier,
                null,
                genericParametersListingNode);
        }

        while (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenSquareBracketToken)
        {
            var openSquareBracketToken = model.TokenWalker.Consume();
            var closeSquareBracketToken = model.TokenWalker.Match(SyntaxKind.CloseSquareBracketToken);

            var arraySyntaxTokenTextSpan = syntaxToken.TextSpan with
            {
                EndingIndexExclusive = closeSquareBracketToken.TextSpan.EndingIndexExclusive
            };

            var arraySyntaxToken = new ArraySyntaxToken(arraySyntaxTokenTextSpan);
            var genericParameterEntryNode = new GenericParameterEntryNode(typeClauseNode);

            var genericParametersListingNode = new GenericParametersListingNode(
                new OpenAngleBracketToken(openSquareBracketToken.TextSpan)
                {
                    IsFabricated = true
                },
                new GenericParameterEntryNode[] { genericParameterEntryNode }.ToImmutableArray(),
                new CloseAngleBracketToken(closeSquareBracketToken.TextSpan)
                {
                    IsFabricated = true
                });

            return new TypeClauseNode(
                arraySyntaxToken,
                null,
                genericParametersListingNode);

            // TODO: Implement multidimensional arrays. This array logic always returns after finding the first array syntax.
        }

        return typeClauseNode;
    }

    public static bool IsContextualKeywordSyntaxKind(SyntaxKind syntaxKind)
    {
        return syntaxKind.ToString().EndsWith("ContextualKeyword");
    }

    public static bool IsKeywordSyntaxKind(SyntaxKind syntaxKind)
    {
        return syntaxKind.ToString().EndsWith("Keyword");
    }

    /// <summary>
    /// The keywords: 'string', 'bool' 'int' and etc... are keywords, but identify a type.
    /// </summary>
    public static bool IsTypeIdentifierKeywordSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.BoolTokenKeyword:
            case SyntaxKind.ByteTokenKeyword:
            case SyntaxKind.CharTokenKeyword:
            case SyntaxKind.DecimalTokenKeyword:
            case SyntaxKind.DelegateTokenKeyword:
            case SyntaxKind.DoubleTokenKeyword:
            case SyntaxKind.EnumTokenKeyword:
            case SyntaxKind.FalseTokenKeyword:
            case SyntaxKind.FloatTokenKeyword:
            case SyntaxKind.IntTokenKeyword:
            case SyntaxKind.LongTokenKeyword:
            case SyntaxKind.NullTokenKeyword:
            case SyntaxKind.ObjectTokenKeyword:
            case SyntaxKind.SbyteTokenKeyword:
            case SyntaxKind.ShortTokenKeyword:
            case SyntaxKind.StringTokenKeyword:
            case SyntaxKind.TrueTokenKeyword:
            case SyntaxKind.UintTokenKeyword:
            case SyntaxKind.UlongTokenKeyword:
            case SyntaxKind.UshortTokenKeyword:
            case SyntaxKind.VoidTokenKeyword:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// TODO: What is described in this summary has not yet been implemented.
    /// The text 'var' can be an identifier to any syntax.
    /// For example, 'var' can be the name of a class, a variable, etc...
    /// Therefore, this method will check if the <see cref="SyntaxKind"/> is
    /// <see cref="SyntaxKind.VarTokenContextualKeyword"/> AND check if
    /// any 'var' identified definitions are in scope.
    /// </summary>
    public static bool IsVarContextualKeyword(ParserModel parserModel, SyntaxKind syntaxKind)
    {
        if (syntaxKind != SyntaxKind.VarTokenContextualKeyword)
            return false;

        return true;
    }

    public static bool IsBinaryOperatorSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
            case SyntaxKind.StarToken:
            case SyntaxKind.DivisionToken:
                return true;
            default:
                return false;
        }
    }

    public static bool IsUnaryOperatorSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PlusPlusToken:
            case SyntaxKind.MinusMinusToken:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// public, private, protected, internal, ....
    /// </summary>
    public static bool IsAccessibilitySyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PublicTokenKeyword:
            case SyntaxKind.PrivateTokenKeyword:
            case SyntaxKind.ProtectedTokenKeyword:
            case SyntaxKind.InternalTokenKeyword:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// The value '0' is returned if the provided <see cref="SyntaxKind"/> was not an operator.
    /// </summary>
    public static int GetOperatorPrecedence(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
                return 1;
            case SyntaxKind.StarToken:
            case SyntaxKind.DivisionToken:
                return 2;
            case SyntaxKind.ParenthesizedExpressionNode:
                return 3;
            default:
                return 0;
        }
    }
}