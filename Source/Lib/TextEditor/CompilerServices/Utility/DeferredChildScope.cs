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
