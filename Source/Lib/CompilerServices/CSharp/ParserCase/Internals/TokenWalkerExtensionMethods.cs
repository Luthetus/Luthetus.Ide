using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

internal static class TokenWalkerExtensionMethods
{
    public static TypeClauseNode MatchTypeClauseNode(this TokenWalker tokenWalker, CSharpParserModel model)
    {
        return ParseTypes.MatchTypeClause(model);
    }

	public static void DeferParsingOfChildScope(
		this TokenWalker tokenWalker,
		OpenBraceToken consumedOpenBraceToken,
		CSharpParserModel model)
    {
		// Pop off the 'TypeDefinitionNode', then push it back on when later dequeued.
		var pendingCodeBlockOwner = model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner;
		var openTokenIndex = tokenWalker.Index - 1;
		
		var bracePair = model.Lexer?.GetBracePairByOpenBraceTokenIndex(openTokenIndex);
		var closeBraceTokenIndex = bracePair?.CloseBraceTokenIndex ?? -1;
		
		#if DEBUG
		model.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		if (closeBraceTokenIndex == -1)
			FallbackFindCloseBraceToken(tokenWalker);
		else
			tokenWalker.GotoIndex(closeBraceTokenIndex);
		
		var closeTokenIndex = tokenWalker.Index;
		var closeBraceToken = (CloseBraceToken)tokenWalker.Match(SyntaxKind.CloseBraceToken);
		
		#if DEBUG
		model.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif

		model.CurrentCodeBlockBuilder.ParseChildScopeQueue.Enqueue(new DeferredChildScope(
			openTokenIndex,
			closeTokenIndex,
			pendingCodeBlockOwner));
    }
    
    private static void FallbackFindCloseBraceToken(TokenWalker tokenWalker)
    {
    	var openBraceCounter = 1;
    	
    	while (true)
		{
			if (tokenWalker.IsEof)
				break;

			if (tokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
			{
				++openBraceCounter;
			}
			else if (tokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
			{
				if (--openBraceCounter <= 0)
					break;
			}

			_ = tokenWalker.Consume();
		}
    }
}
