using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

internal static class TokenWalkerExtensionMethods
{
    public static TypeClauseNode MatchTypeClauseNode(this TokenWalker tokenWalker, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        return ParseTypes.MatchTypeClause(compilationUnit, ref parserModel);
    }

	public static void DeferParsingOfChildScope(
		this TokenWalker tokenWalker,
		OpenBraceToken consumedOpenBraceToken,
		CSharpCompilationUnit compilationUnit,
		ref CSharpParserModel parserModel)
    {
		// Pop off the 'TypeDefinitionNode', then push it back on when later dequeued.
		var pendingCodeBlockOwner = parserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner;
		parserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = null;

		var openTokenIndex = tokenWalker.Index - 1;

		var openBraceCounter = 1;
		
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
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
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif

		parserModel.CurrentCodeBlockBuilder.ParseChildScopeQueue.Enqueue(new CSharpDeferredChildScope(
			openTokenIndex,
			closeTokenIndex,
			pendingCodeBlockOwner));
    }
}
