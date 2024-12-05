using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.CompilerServices.CSharp.LexerCase;

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
		
		var indexBracePair = model.Lexer?.GetIndexBracePairByOpenBraceTokenIndex(openTokenIndex, 0);
		
		// Console.WriteLine(indexBracePair?.ToString() ?? "null");
		
		BracePairMetadata? bracePair;
		
		if (indexBracePair is null)
			bracePair = null;
		else
			bracePair = model.Lexer.BracePairList[indexBracePair.Value];
		
		var closeBraceTokenIndex = bracePair?.CloseBraceTokenIndex ?? -1;
		
		if (closeBraceTokenIndex == -1)
		{
			#if DEBUG
			model.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
			#endif
		
			Console.WriteLine("FallbackFindCloseBraceToken(tokenWalker);");
			FallbackFindCloseBraceToken(tokenWalker);
			
			closeBraceTokenIndex = tokenWalker.Index;
			_ = (CloseBraceToken)tokenWalker.Match(SyntaxKind.CloseBraceToken);
			
			#if DEBUG
			model.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
			#endif
		}
		else
		{
			model.LastUsedIndexBracePair = indexBracePair.Value;
			tokenWalker.GotoIndex(closeBraceTokenIndex + 1);
		}
		
		model.CurrentCodeBlockBuilder.ParseChildScopeQueue.Enqueue(new DeferredChildScope(
			openTokenIndex,
			closeBraceTokenIndex,
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
