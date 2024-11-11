using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

public class StatementBuilder
{
	public List<ISyntax> ChildList { get; } = new();
	
	/// <summary>Invokes the other overload with index: ^1</summary>
	public bool TryPeek(out ISyntax syntax)
	{
		return TryPeek(^1, out syntax);
	}
	
	/// <summary>^1 gives the last entry</summary>
	public bool TryPeek(Index index, out ISyntax syntax)
	{
		if (ChildList.Count > 0)
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
	/// has been added to the model.CurrentCodeBlockBuilder.ChildList.
	///
	/// If it was not yet added, then add it.
	///
	/// Lastly, clear the StatementBuilder.ChildList.
	/// </summary>
	public void FinishStatement(IParserModel model)
	{
		if (ChildList.Count == 0)
			return;
		
		var statementSyntax = ChildList[^1];
		
		ISyntax codeBlockBuilderSyntax;
		
		if (model.CurrentCodeBlockBuilder.ChildList.Count == 0)
			codeBlockBuilderSyntax = EmptyExpressionNode.Empty;
		else
			codeBlockBuilderSyntax = model.CurrentCodeBlockBuilder.ChildList[^1];
			
		if (!Object.ReferenceEquals(statementSyntax, codeBlockBuilderSyntax))
			model.CurrentCodeBlockBuilder.ChildList.Add(statementSyntax);
		
		ChildList.Clear();
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
