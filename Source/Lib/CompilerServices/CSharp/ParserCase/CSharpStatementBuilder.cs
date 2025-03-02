using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public struct CSharpStatementBuilder
{
	public CSharpStatementBuilder()
	{
		ChildList = new();
		ParseChildScopeStack = new();
	}

	public List<ISyntax> ChildList { get; }
	
	/// <summary>
	/// TODO: Measure the cost of 'Peek(...)', 'TryPeek(...)' since now...
	/// ...this is a value tuple and the dequeue alone does not mean success,
	/// you have to peek first to see if the object references are equal.
	/// </summary>
	public Stack<(ICodeBlockOwner CodeBlockOwner, CSharpDeferredChildScope DeferredChildScope)> ParseChildScopeStack { get; }
	
	/// <summary>Invokes the other overload with index: ^1</summary>
	public bool TryPeek(out ISyntax syntax)
	{
		return TryPeek(^1, out syntax);
	}
	
	/// <summary>^1 gives the last entry</summary>
	public bool TryPeek(Index index, out ISyntax syntax)
	{
		if (ChildList.Count - index.Value > -1)
		{
			syntax = ChildList[index];
			return true;
		}
		
		syntax = null;
		return false;
	}
	
	public ISyntax Pop()
	{
		var syntax = ChildList[^1];
		ChildList.RemoveAt(ChildList.Count - 1);
		return syntax;
	}

	/// <summary>
	/// If 'StatementDelimiterToken', 'OpenBraceToken', or 'CloseBraceToken'
	/// are parsed by the main loop,
	///
	/// Then check that the last item in the StatementBuilder.ChildList
	/// has been added to the parserComputation.CurrentCodeBlockBuilder.ChildList.
	///
	/// If it was not yet added, then add it.
	///
	/// Lastly, clear the StatementBuilder.ChildList.
	///
	/// Returns the result of 'ParseChildScopeStack.TryPop(out var deferredChildScope)'.
	/// </summary>
	public bool FinishStatement(int finishTokenIndex, CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
	{
		if (ChildList.Count != 0)
		{
			var statementSyntax = ChildList[^1];
			
			ISyntax codeBlockBuilderSyntax;
			
			if (parserComputation.CurrentCodeBlockBuilder.ChildList.Count == 0)
				codeBlockBuilderSyntax = EmptyExpressionNode.Empty;
			else
				codeBlockBuilderSyntax = parserComputation.CurrentCodeBlockBuilder.ChildList[^1];

			if (!Object.ReferenceEquals(statementSyntax, codeBlockBuilderSyntax) &&
				!Object.ReferenceEquals(statementSyntax, parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner))
			{
				parserComputation.CurrentCodeBlockBuilder.ChildList.Add(statementSyntax);
			}
			
			ChildList.Clear();
		}
		
		if (ParseChildScopeStack.Count > 0)
		{
			var tuple = ParseChildScopeStack.Peek();
			
			if (Object.ReferenceEquals(tuple.CodeBlockOwner, parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner))
			{
				tuple = ParseChildScopeStack.Pop();
				tuple.DeferredChildScope.PrepareMainParserLoop(finishTokenIndex, compilationUnit, ref parserComputation);
				return true;
			}
		}
		
		return false;
	}
	
	public void WriteToConsole()
	{
		Console.Write("StatementBuilder: ");
	
		foreach (var child in ChildList)
		{
			Console.Write($"{child.SyntaxKind}, ");
		}
		
		Console.WriteLine();
	}
}

