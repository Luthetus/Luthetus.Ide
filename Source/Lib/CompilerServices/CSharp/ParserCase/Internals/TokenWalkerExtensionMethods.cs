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

		var startIndexInclusive = tokenWalker.Index - 1;

		while (tokenWalker.Current.SyntaxKind != SyntaxKind.CloseBraceToken &&
			   !tokenWalker.IsEof)
		{
			_ = tokenWalker.Consume();
		}

		var endIndexExclusive = tokenWalker.Index + 1;
		var closeBraceToken = (CloseBraceToken)tokenWalker.Match(SyntaxKind.CloseBraceToken);

		Console.WriteLine($"TokenWalker::startIndexInclusive::{startIndexInclusive}::{tokenWalker.TokenList[startIndexInclusive].SyntaxKind}");
		Console.WriteLine($"TokenWalker::endIndexExclusive::{endIndexExclusive}::{tokenWalker.TokenList[endIndexExclusive].SyntaxKind}");
// 		Console.WriteLine($"TokenWalker::closeBraceToken::{closeBraceToken}::tokenWalker.TokenList[closeBraceToken]");

        model.ParseChildScopeQueue.Enqueue(tokenIndexToRestore =>
		{
			tokenWalker.DeferredParsing(startIndexInclusive, endIndexExclusive, tokenIndexToRestore);
			model.SyntaxStack.Push(syntax);
		});
    }
}
