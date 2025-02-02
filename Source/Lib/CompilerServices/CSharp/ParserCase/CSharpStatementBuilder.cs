using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public class CSharpStatementBuilder
{
	public List<ISyntax> ChildList { get; } = new();
	public Queue<CSharpDeferredChildScope> ParseChildScopeQueue { get; } = new();
	
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
	/// has been added to the parserModel.CurrentCodeBlockBuilder.ChildList.
	///
	/// If it was not yet added, then add it.
	///
	/// Lastly, clear the StatementBuilder.ChildList.
	///
	/// Returns the result of 'ParseChildScopeQueue.TryDequeue(out var deferredChildScope)'.
	/// </summary>
	public bool FinishStatement(int finishTokenIndex, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (ChildList.Count != 0)
		{
			var statementSyntax = ChildList[^1];
			
			ISyntax codeBlockBuilderSyntax;
			
			if (parserModel.CurrentCodeBlockBuilder.ChildList.Count == 0)
				codeBlockBuilderSyntax = EmptyExpressionNode.Empty;
			else
				codeBlockBuilderSyntax = parserModel.CurrentCodeBlockBuilder.ChildList[^1];
				
			if (!Object.ReferenceEquals(statementSyntax, codeBlockBuilderSyntax))
				parserModel.CurrentCodeBlockBuilder.ChildList.Add(statementSyntax);
			
			ChildList.Clear();
		}
		
		var success = ParseChildScopeQueue.TryDequeue(out var deferredChildScope);
		
		if (success)
			deferredChildScope.PrepareMainParserLoop(finishTokenIndex, compilationUnit, ref parserModel);
		
		return success;
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

