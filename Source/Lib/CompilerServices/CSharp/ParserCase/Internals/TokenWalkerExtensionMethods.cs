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
		var indexToUpdateAfterDequeue = model.CurrentCodeBlockBuilder.ChildList.Count - 1;
		
		// Pop off the 'TypeDefinitionNode', then push it back on when later dequeued.
		_ = model.SyntaxStack.TryPop(out var syntax);
		var pendingChild = model.CurrentCodeBlockBuilder.PendingChild;

		// Im so confused I'll copy and paste a comment I put in DeferredChildScope.cs
		/*
			/// <summary>
			/// The parameter 'tokenIndexToRestore' to this method is confusing.
			/// It likely is the case that you'd use 'model.TokenWalker.Index - 1'.
			/// Note the '- 1'.
			///
			/// Because, after the TokenWalker changes...
			/// ...I thought I understood why but I confused myself again
			/// while trying to write this comment.
			///
			/// Maybe I never understood in the first place.
			/// I think it has to do with returning to the main while loop,
			/// and whether the automated 'model.TokenWalker.Consume()'
			/// messed with your perception of what index you were at.
			/// </summary>
			public void Run(int tokenIndexToRestore, IParserModel model)
			{
				...
			}
		*/
		var openTokenIndex = tokenWalker.Index - 1 - 1;

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
			syntax,
			pendingChild,
			indexToUpdateAfterDequeue));
    }
}
