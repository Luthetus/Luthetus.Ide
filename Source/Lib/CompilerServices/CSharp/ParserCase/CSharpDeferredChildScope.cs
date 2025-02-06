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
		
		compilationUnit.Binder.SetCurrentScopeAndBuilder(
			CodeBlockBuilder,
			compilationUnit,
			ref parserModel);
		
		parserModel.CurrentCodeBlockBuilder.PermitCodeBlockParsing = true;
		
		parserModel.CurrentCodeBlockBuilder.DequeuedIndexForChildList = null;
		
		parserModel.TokenWalker.DeferredParsing(
			OpenTokenIndex,
			CloseTokenIndex,
			TokenIndexToRestore);
	}
}
