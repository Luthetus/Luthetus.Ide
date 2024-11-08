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
