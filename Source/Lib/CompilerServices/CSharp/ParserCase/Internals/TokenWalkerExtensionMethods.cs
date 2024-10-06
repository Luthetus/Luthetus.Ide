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

		var openTokenIndex = tokenWalker.Index - 1;

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

		var closeTokenIndex = tokenWalker.Index;
		var closeBraceToken = (CloseBraceToken)tokenWalker.Match(SyntaxKind.CloseBraceToken);

		model.CurrentCodeBlockBuilder.ParseChildScopeQueue.Enqueue(tokenIndexToRestore =>
		{
			tokenWalker.DeferredParsing(
				openTokenIndex,
				closeTokenIndex,
				tokenIndexToRestore,
				() => model.CurrentCodeBlockBuilder.DequeuedIndexForChildList = null);
			
			model.SyntaxStack.Push(syntax);
			model.CurrentCodeBlockBuilder.DequeuedIndexForChildList = indexToUpdateAfterDequeue;
		});
    }
}
