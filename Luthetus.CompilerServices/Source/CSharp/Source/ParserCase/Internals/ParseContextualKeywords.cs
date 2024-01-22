using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

public class ParseContextualKeywords
{
    public static void HandleVarTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();

        // Check if previous statement is finished, and a new one is starting.
        // TODO: 'Peek(-2)' is horribly confusing. The reason for using -2 is that one consumed the 'var' keyword and moved their position forward by 1. So to read the token behind 'var' one must go back 2 tokens. It feels natural to put '-1' and then this evaluates to the wrong token. Should an expression bound property be made for 'Peek(-2)'?
        var previousToken = model.TokenWalker.Peek(-2);

        if (previousToken.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
            previousToken.SyntaxKind == SyntaxKind.BadToken)
        {
            // Check if the next token is a second 'var keyword' or an IdentifierToken. Two IdentifierTokens is invalid, and therefore one can contextually take this 'var' as a keyword.
            bool nextTokenIsVarKeyword = SyntaxKind.VarTokenContextualKeyword == model.TokenWalker.Current.SyntaxKind;
            bool nextTokenIsIdentifierToken = SyntaxKind.IdentifierToken == model.TokenWalker.Current.SyntaxKind;

            if (nextTokenIsVarKeyword || nextTokenIsIdentifierToken)
            {
                var varKeyword = new TypeClauseNode(
                    contextualKeywordToken,
                    null,
                    null);

                model.SyntaxStack.Push(varKeyword);
            }
        }
        else
        {
            // Take 'var' as an identifier
            IdentifierToken varIdentifierToken = new(contextualKeywordToken.TextSpan);
            model.SyntaxStack.Push(varIdentifierToken);
            ParseTokens.ParseIdentifierToken(model);
        }
    }

    public static void HandlePartialTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAddTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAndTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAliasTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAscendingTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleArgsTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAsyncTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAwaitTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleByTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDescendingTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDynamicTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleEqualsTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleFileTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleFromTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleGetTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleGlobalTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleGroupTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleInitTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleIntoTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleJoinTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleLetTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleManagedTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleNameofTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleNintTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleNotTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleNotnullTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleNuintTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleOnTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleOrTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleOrderbyTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleRecordTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleRemoveTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleRequiredTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleScopedTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleSelectTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleSetTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUnmanagedTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleValueTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleWhenTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleWhereTokenContextualKeyword(ParserModel model)
    {
        var whereKeywordContextualToken = (KeywordContextualToken)model.SyntaxStack.Pop();

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
                whereKeywordContextualToken
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
                functionDefinitionNode.ReturnTypeClauseNode,
                functionDefinitionNode.FunctionIdentifierToken,
                functionDefinitionNode.GenericArgumentsListingNode,
                functionDefinitionNode.FunctionArgumentsListingNode,
                functionDefinitionNode.FunctionBodyCodeBlockNode,
                constraintNode));
        }
        else
        {
            model.DiagnosticBag.ReportTodoException(whereKeywordContextualToken.TextSpan, nameof(HandleWhereTokenContextualKeyword));
        }
    }

    public static void HandleWithTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleYieldTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUnrecognizedTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }
}
