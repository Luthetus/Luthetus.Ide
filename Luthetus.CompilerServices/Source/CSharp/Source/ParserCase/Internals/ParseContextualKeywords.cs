using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

public class ParseContextualKeywords
{
    public static void HandleVarTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // Check if previous statement is finished, and a new one is starting.
        // TODO: 'Peek(-2)' is horribly confusing. The reason for using -2 is that one consumed the 'var' keyword and moved their position forward by 1. So to read the token behind 'var' one must go back 2 tokens. It feels natural to put '-1' and then this evaluates to the wrong token. Should an expression bound property be made for 'Peek(-2)'?
        var previousToken = model.TokenWalker.Peek(-2);

        if (previousToken.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
            previousToken.SyntaxKind == SyntaxKind.CloseBraceToken ||
            previousToken.SyntaxKind == SyntaxKind.BadToken)
        {
            // Check if the next token is a second 'var keyword' or an IdentifierToken. Two IdentifierTokens is invalid, and therefore one can contextually take this 'var' as a keyword.
            bool nextTokenIsVarKeyword = SyntaxKind.VarTokenContextualKeyword == model.TokenWalker.Current.SyntaxKind;
            bool nextTokenIsIdentifierToken = SyntaxKind.IdentifierToken == model.TokenWalker.Current.SyntaxKind;

            if (nextTokenIsVarKeyword || nextTokenIsIdentifierToken)
            {
                var varTypeClauseNode = new TypeClauseNode(
                    consumedKeywordContextualToken,
                    null,
                    null);

                if (model.Binder.TryGetTypeDefinitionHierarchically(
                        consumedKeywordContextualToken.TextSpan.GetText(),
                        out var varTypeDefinitionNode) &&
                    varTypeDefinitionNode is not null)
                {
                    varTypeClauseNode = varTypeDefinitionNode.ToTypeClause();
                }

                model.SyntaxStack.Push(varTypeClauseNode);
                return;
            }
        }

        // Take 'var' as an identifier
        IdentifierToken varIdentifierToken = new(consumedKeywordContextualToken.TextSpan);
        ParseTokens.ParseIdentifierToken(varIdentifierToken, model);
    }

    public static void HandlePartialTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleAddTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleAndTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleAliasTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleAscendingTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleArgsTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleAsyncTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleAwaitTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleByTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDescendingTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDynamicTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleEqualsTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleFileTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleFromTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleGetTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleGlobalTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleGroupTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleInitTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleIntoTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleJoinTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleLetTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleManagedTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleNameofTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleNintTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleNotTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleNotnullTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleNuintTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleOnTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleOrTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleOrderbyTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleRecordTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        ParseDefaultKeywords.HandleStorageModifierTokenKeyword(
            consumedKeywordContextualToken,
            model);
    }

    public static void HandleRemoveTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleRequiredTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleScopedTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleSelectTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleSetTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUnmanagedTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleValueTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleWhenTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleWhereTokenContextualKeyword(
        KeywordContextualToken consumedWhereKeywordContextualToken,
        ParserModel model)
    {
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
        {
            var functionDefinitionNode = (FunctionDefinitionNode)model.SyntaxStack.Pop();

            /*
             Examples:

             public static T Clone<T>(T item) where T : class
             {
                 return item;
             }

             public static T Clone<T>(T item) where T : class => item;
            */

            // TODO: Implement generic constraints, until then just read until the generic...
            // ...constraint is finished.

            var constraintNodeInnerTokens = new List<ISyntaxToken>
            {
                consumedWhereKeywordContextualToken
            };

            while (!model.TokenWalker.IsEof)
            {
                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
                    model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
                {
                    break;
                }

                constraintNodeInnerTokens.Add(model.TokenWalker.Consume());
            }

            var constraintNode = new ConstraintNode(constraintNodeInnerTokens.ToImmutableArray());

            model.SyntaxStack.Push(new FunctionDefinitionNode(
                AccessModifierKind.Public,
                functionDefinitionNode.ReturnTypeClauseNode,
                functionDefinitionNode.FunctionIdentifierToken,
                functionDefinitionNode.GenericArgumentsListingNode,
                functionDefinitionNode.FunctionArgumentsListingNode,
                functionDefinitionNode.FunctionBodyCodeBlockNode,
                constraintNode));
        }
        else
        {
            model.DiagnosticBag.ReportTodoException(consumedWhereKeywordContextualToken.TextSpan, nameof(HandleWhereTokenContextualKeyword));
        }
    }

    public static void HandleWithTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleYieldTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUnrecognizedTokenContextualKeyword(
        KeywordContextualToken consumedKeywordContextualToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }
}
