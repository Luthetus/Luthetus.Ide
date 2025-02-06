using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
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
    
    /*#if DEBUG
    private static readonly HashSet<int> _seenOpenTokenIndices = new();
    #endif*/

	public static void DeferParsingOfChildScope(
		this TokenWalker tokenWalker,
		CSharpCompilationUnit compilationUnit,
		ref CSharpParserModel parserModel)
    {    
		// Pop off the 'TypeDefinitionNode', then push it back on when later dequeued.
		var deferredCodeBlockBuilder = parserModel.CurrentCodeBlockBuilder;
		
		compilationUnit.Binder.SetCurrentScopeAndBuilder(
			deferredCodeBlockBuilder.Parent,
			compilationUnit,
			ref parserModel);

		var openTokenIndex = tokenWalker.Index - 1;
		
		/*#if DEBUG
		if (!_seenOpenTokenIndices.Add(openTokenIndex))
		{
			throw new NotImplementedException("aaa Infinite loop?");
		}
		#endif*/

		var openBraceCounter = 1;
		
		int closeTokenIndex;
		
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		if (deferredCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan)
		{
			while (true)
			{
				if (tokenWalker.IsEof)
					break;
	
				if (tokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
					break;
	
				_ = tokenWalker.Consume();
			}
	
			closeTokenIndex = tokenWalker.Index;
			var statementDelimiterToken = tokenWalker.Match(SyntaxKind.StatementDelimiterToken);
		}
		else
		{
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
	
			closeTokenIndex = tokenWalker.Index;
			var closeBraceToken = tokenWalker.Match(SyntaxKind.CloseBraceToken);
		}
		
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif

		parserModel.ParseChildScopeStack.Push(
			(
				parserModel.CurrentCodeBlockBuilder.CodeBlockOwner,
				new CSharpDeferredChildScope(
					openTokenIndex,
					closeTokenIndex,
					deferredCodeBlockBuilder)
			));
    }
}
