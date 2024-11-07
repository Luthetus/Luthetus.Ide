using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseContextualKeywords
{
    public static void HandleVarTokenContextualKeyword(CSharpParserModel model)
    {
    	var varContextualKeywordToken = model.TokenWalker.Consume();
    
        // Check if previous statement is finished, and a new one is starting.
        var peekNumber = -1;
        
        while (model.TokenWalker.Peek(peekNumber).SyntaxKind != SyntaxKind.BadToken && !model.TokenWalker.IsEof)
        {
        	if (model.TokenWalker.Peek(peekNumber).SyntaxKind == SyntaxKind.CommentSingleLineToken ||
        		model.TokenWalker.Peek(peekNumber).SyntaxKind == SyntaxKind.CommentMultiLineToken)
        	{
        		peekNumber--;
        	}
        	else
        	{
        		break;
        	}
        }
        
        var previousToken = model.TokenWalker.Peek(peekNumber);

		switch (previousToken.SyntaxKind)
		{
			case SyntaxKind.StatementDelimiterToken:
            case SyntaxKind.CloseBraceToken:
            case SyntaxKind.OpenBraceToken:
            case SyntaxKind.CommaToken:
            case SyntaxKind.OpenParenthesisToken:
            case SyntaxKind.ColonToken:
            case SyntaxKind.BadToken:
            {
	            // Check if the next token is a second 'var keyword' or an IdentifierToken.
	            // Two IdentifierTokens is invalid, and therefore one can contextually take this 'var' as a keyword.
	            bool nextTokenIsVarKeyword = SyntaxKind.VarTokenContextualKeyword == model.TokenWalker.Next.SyntaxKind;
	            bool nextTokenIsIdentifierToken = SyntaxKind.IdentifierToken == model.TokenWalker.Next.SyntaxKind;
	
	            if (nextTokenIsVarKeyword || nextTokenIsIdentifierToken)
	            {
	                var varTypeClauseNode = new TypeClauseNode(
	                    varContextualKeywordToken,
	                    null,
	                    null);
	
	                if (model.Binder.TryGetTypeDefinitionHierarchically(
	                		model,
	                		model.BinderSession.ResourceUri,
	                		model.BinderSession.CurrentScopeIndexKey,
	                        varContextualKeywordToken.TextSpan.GetText(),
	                        out var varTypeDefinitionNode) &&
	                    varTypeDefinitionNode is not null)
	                {
	                    varTypeClauseNode = varTypeDefinitionNode.ToTypeClause();
	                }
	
	                model.SyntaxStack.Push(varTypeClauseNode);
	                return;
	            }
	            
	            break;
            }
		}
        
        // Take 'var' as an identifier
        IdentifierToken varIdentifierToken = new(varContextualKeywordToken.TextSpan);
        _ = model.TokenWalker.Backtrack();
        ParseTokens.ParseIdentifierToken(model);
    }

    public static void HandlePartialTokenContextualKeyword(CSharpParserModel model)
    {
    	var partialContextualKeywordToken = model.TokenWalker.Consume();
        model.SyntaxStack.Push(partialContextualKeywordToken);
    }

    public static void HandleAddTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleAndTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleAliasTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleAscendingTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleArgsTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleAsyncTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleAwaitTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleByTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleDescendingTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleDynamicTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleEqualsTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleFileTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleFromTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleGetTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleGlobalTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleGroupTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleInitTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleIntoTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleJoinTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleLetTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleManagedTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleNameofTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleNintTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleNotTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleNotnullTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleNuintTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleOnTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleOrTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleOrderbyTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleRecordTokenContextualKeyword(CSharpParserModel model)
    {
        ParseDefaultKeywords.HandleStorageModifierTokenKeyword(model);
    }

    public static void HandleRemoveTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleRequiredTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleScopedTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleSelectTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleSetTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleUnmanagedTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleValueTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleWhenTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleWhereTokenContextualKeyword(CSharpParserModel model)
    {
    	var whereContextualKeywordToken = model.TokenWalker.Consume();
    	
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
            	whereContextualKeywordToken
            };

            while (!model.TokenWalker.IsEof)
            {
                if (model.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenBraceToken ||
                    model.TokenWalker.Next.SyntaxKind == SyntaxKind.EqualsToken)
                {
                    break;
                }

                constraintNodeInnerTokens.Add(model.TokenWalker.Consume());
            }

            var constraintNode = new ConstraintNode(constraintNodeInnerTokens.ToImmutableArray());

			functionDefinitionNode.SetConstraintNode(constraintNode);
                
            model.SyntaxStack.Push(functionDefinitionNode);
            model.CurrentCodeBlockBuilder.PendingChild = functionDefinitionNode;
        }
        else
        {
            model.DiagnosticBag.ReportTodoException(whereContextualKeywordToken.TextSpan, nameof(HandleWhereTokenContextualKeyword));
        }
    }

    public static void HandleWithTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleYieldTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }

    public static void HandleUnrecognizedTokenContextualKeyword(CSharpParserModel model)
    {
    	var contextualKeywordToken = model.TokenWalker.Consume();
        // TODO: Implement this method
    }
}
