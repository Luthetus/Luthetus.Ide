using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

internal static class TokenWalkerExtensionMethods
{
    public static TypeClauseNode MatchTypeClauseNode(this TokenWalker tokenWalker, CSharpCompilationUnit compilationUnit)
    {
        return ParseTypes.MatchTypeClause(model);
    }

	public static void DeferParsingOfChildScope(
		this TokenWalker tokenWalker,
		OpenBraceToken consumedOpenBraceToken,
		CSharpCompilationUnit compilationUnit)
    {
		// Pop off the 'TypeDefinitionNode', then push it back on when later dequeued.
		var pendingCodeBlockOwner = model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner;

		var openTokenIndex = tokenWalker.Index - 1;

		var openBraceCounter = 1;
		
		#if DEBUG
		model.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
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
}
