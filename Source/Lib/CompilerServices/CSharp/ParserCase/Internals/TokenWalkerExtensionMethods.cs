using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

internal static class TokenWalkerExtensionMethods
{
    public static TypeClauseNode MatchTypeClauseNode(this TokenWalker tokenWalker, ParserModel model)
    {
        return ParseTypes.MatchTypeClause(model);
    }

	public static void DeferParsingOfChildScope(
		this TokenWalker tokenWalker,
		OpenBraceToken consumedOpenBraceToken,
		ParserModel model)
    {
		_ = model.SyntaxStack.TryPeek(out var syntax);

		Console.WriteLine($"enqueued::{syntax.SyntaxKind}");

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

        model.ParseChildScopeQueue.Enqueue(tokenIndexToRestore =>
		{
			Console.WriteLine($"dequeued::{syntax.SyntaxKind}");
			tokenWalker.DeferredParsing(openTokenIndex, closeTokenIndex, tokenIndexToRestore);
			model.SyntaxStack.Push(syntax);
		});
    }
}
