using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public class CSharpDeferredChildScope
{
	// (2025-01-13)
	// ========================================================
	// 
	// - 'CSharpDeferredChildScope' needs to accept the 'CSharpCodeBlockBuilder'
	//   in place of the 'ICodeBlockOwner'.
	//   (the 'CSharpCodeBlockBuilder' has a property which is the 'ICodeBlockOwner')
	
	public CSharpDeferredChildScope(
		int openTokenIndex,
		int closeTokenIndex,
		CSharpCodeBlockBuilder codeBlockBuilder)
	{
		OpenTokenIndex = openTokenIndex;
		CloseTokenIndex = closeTokenIndex;
		CodeBlockBuilder = codeBlockBuilder;
	}
	
	public int OpenTokenIndex { get; }
	public int CloseTokenIndex { get; }
	public CSharpCodeBlockBuilder CodeBlockBuilder { get; }
	
	public int TokenIndexToRestore { get; private set; }
	
	public void PrepareMainParserLoop(int tokenIndexToRestore, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		TokenIndexToRestore = tokenIndexToRestore;
		
		parserModel.CurrentCodeBlockBuilder = CodeBlockBuilder;
		
		parserModel.SyntaxStack.Push(CodeBlockBuilder.CodeBlockOwner);
		
		parserModel.CurrentCodeBlockBuilder.PermitCodeBlockParsing = true;
		
		compilationUnit.Binder.SetCurrentScopeAndBuilder(
			parserModel.CurrentCodeBlockBuilder,
			compilationUnit,
			ref parserModel);
		
		parserModel.CurrentCodeBlockBuilder.DequeuedIndexForChildList = null;
		
		parserModel.TokenWalker.DeferredParsing(
			OpenTokenIndex,
			CloseTokenIndex,
			TokenIndexToRestore);
		
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
