using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

public class DeferredChildScope
{
	public DeferredChildScope(
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
	
	public void PrepareMainParserLoop(int tokenIndexToRestore, IParserModel model)
	{
		TokenIndexToRestore = tokenIndexToRestore;
		model.CurrentCodeBlockBuilder.PermitInnerPendingCodeBlockOwnerToBeParsed = true;
		
		model.CurrentCodeBlockBuilder.DequeuedIndexForChildList = null;
		
		model.TokenWalker.DeferredParsing(
			OpenTokenIndex,
			CloseTokenIndex,
			TokenIndexToRestore);
		
		model.SyntaxStack.Push(PendingCodeBlockOwner);
		model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = PendingCodeBlockOwner;
	}
}
