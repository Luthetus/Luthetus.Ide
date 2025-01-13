using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public class CSharpDeferredChildScope
{
	public CSharpDeferredChildScope(
		int openTokenIndex,
		int closeTokenIndex,
		ICodeBlockOwner pendingCodeBlockOwner)
	{
		OpenTokenIndex = openTokenIndex;
		CloseTokenIndex = closeTokenIndex;
		PendingCodeBlockOwner = pendingCodeBlockOwner;
	}
	
	public int OpenTokenIndex { get; }
	public int CloseTokenIndex { get; }
	public ICodeBlockOwner PendingCodeBlockOwner { get; }
	
	public int TokenIndexToRestore { get; private set; }
	
	public void PrepareMainParserLoop(int tokenIndexToRestore, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		TokenIndexToRestore = tokenIndexToRestore;
		parserModel.CurrentCodeBlockBuilder.PermitInnerPendingCodeBlockOwnerToBeParsed = true;
		
		parserModel.CurrentCodeBlockBuilder.DequeuedIndexForChildList = null;
		
		parserModel.TokenWalker.DeferredParsing(
			OpenTokenIndex,
			CloseTokenIndex,
			TokenIndexToRestore);
		
		parserModel.SyntaxStack.Push(PendingCodeBlockOwner);
		parserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = PendingCodeBlockOwner;
		
		// (2025-01-13)
		// ========================================================
		// 
		// - 'SetActiveCodeBlockBuilder', 'SetActiveScope', and 'PermitInnerPendingCodeBlockOwnerToBeParsed'
		//   should all be handled by the same method.
		//
		// - PermitInnerPendingCodeBlockOwnerToBeParsed needs to move
		//   to the ICodeBlockOwner itself.
		// 
		// - 'parserModel.SyntaxStack.Push(PendingCodeBlockOwner);' is unnecessary because
		//   the CodeBlockBuilder and Scope will be active.
		//
		// - '...InnerPendingCodeBlockOwner = PendingCodeBlockOwner;' needs to change
		//   to 'set active code block builder' and 'set active scope'.
	}
}
