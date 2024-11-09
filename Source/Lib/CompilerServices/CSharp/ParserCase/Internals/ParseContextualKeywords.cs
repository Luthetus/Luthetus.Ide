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
        var peekNumber = -1;
        
        while (model.TokenWalker.Peek(peekNumber).SyntaxKind != SyntaxKind.BadToken)
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
	            // Check if the next token is a second 'var keyword' or an IdentifierToken. Two IdentifierTokens is invalid, and therefore one can contextually take this 'var' as a keyword.
	            bool nextTokenIsVarKeyword = SyntaxKind.VarTokenContextualKeyword == model.TokenWalker.Current.SyntaxKind;
	            bool nextTokenIsIdentifierToken = SyntaxKind.IdentifierToken == model.TokenWalker.Current.SyntaxKind;
	
	            if (nextTokenIsVarKeyword || nextTokenIsIdentifierToken)
	            {
	            	var varContextualKeywordToken = model.TokenWalker.Consume();
	            	
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
        
        // Take 'var' as an 
        ParseTokens.ParseIdentifierToken(model);
    }

    public static void HandlePartialTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAddTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAndTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAliasTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAscendingTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleArgsTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAsyncTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAwaitTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleByTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleDescendingTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleDynamicTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleEqualsTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleFileTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleFromTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleGetTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleGlobalTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleGroupTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleInitTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleIntoTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleJoinTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleLetTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleManagedTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleNameofTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleNintTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleNotTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleNotnullTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleNuintTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleOnTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleOrTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleOrderbyTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleRecordTokenContextualKeyword(CSharpParserModel model)
    {
        ParseDefaultKeywords.HandleStorageModifierTokenKeyword(model);
    }

    public static void HandleRemoveTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleRequiredTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleScopedTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleSelectTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleSetTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleUnmanagedTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleValueTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleWhenTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleWhereTokenContextualKeyword(CSharpParserModel model)
    {
    	var whereTokenContextualKeyword = (KeywordContextualToken)model.TokenWalker.Consume();
    	
    	if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind != SyntaxKind.TypeDefinitionNode)
    		return;
    	
    	while (!model.TokenWalker.IsEof)
    	{
    		if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
    			model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
    		{
    			break;
    		}
    		
    		_ = model.TokenWalker.Consume();
    	}
    }

    public static void HandleWithTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleYieldTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleUnrecognizedTokenContextualKeyword(CSharpParserModel model)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }
}
