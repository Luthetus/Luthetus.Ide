using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.CSharp.BinderCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public struct CSharpStatementBuilder
{
	public CSharpStatementBuilder(CSharpBinder binder)
	{
		ChildList = binder.CSharpStatementBuilder_ChildList;
		ChildList.Clear();
		
		ParseChildScopeStack = binder.CSharpStatementBuilder_ParseChildScopeStack;
		ParseChildScopeStack.Clear();
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
	/// has been added to the parserModel.CurrentCodeBlockBuilder.ChildList.
	///
	/// If it was not yet added, then add it.
	///
	/// Lastly, clear the StatementBuilder.ChildList.
	///
	/// Returns the result of 'ParseChildScopeStack.TryPop(out var deferredChildScope)'.
	/// </summary>
	public bool FinishStatement(int finishTokenIndex, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		parserModel.SafeToUseSharedTypeClauseNode = true;
		
		if (ChildList.Count != 0)
		{
			var statementSyntax = ChildList[^1];
			
			ISyntax codeBlockBuilderSyntax;
			
			if (parserModel.CurrentCodeBlockBuilder.ChildList.Count == 0)
				codeBlockBuilderSyntax = EmptyExpressionNode.Empty;
			else
				codeBlockBuilderSyntax = parserModel.CurrentCodeBlockBuilder.ChildList[^1];

			if (!Object.ReferenceEquals(statementSyntax, codeBlockBuilderSyntax) &&
				!Object.ReferenceEquals(statementSyntax, parserModel.CurrentCodeBlockBuilder.CodeBlockOwner))
			{
				parserModel.CurrentCodeBlockBuilder.ChildList.Add(statementSyntax);
			}
			
			ChildList.Clear();
		}
		
		// Recur
		
		if (ParseChildScopeStack.Count > 0)
		{
			var tuple = ParseChildScopeStack.Peek();
			
			if (Object.ReferenceEquals(tuple.CodeBlockOwner, parserModel.CurrentCodeBlockBuilder.CodeBlockOwner))
			{
				tuple = ParseChildScopeStack.Pop();
				tuple.DeferredChildScope.PrepareMainParserLoop(finishTokenIndex, compilationUnit, ref parserModel);
				return true;
			}
		}
		
		return false;
	}
	
	/*private void Recur()
	{
		if (binaryExpressionNode.LeftExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode &&
		    binaryExpressionNode.RightExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			return new BinaryExpressionLeftAndRightVariableReference(binaryExpressionNode);
		}
		else if (binaryExpressionNode.LeftExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			return new BinaryExpressionLeftVariableReference(binaryExpressionNode);
		}
		else if (binaryExpressionNode.RightExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			return new BinaryExpressionRightVariableReference(binaryExpressionNode);
		}
	}*/
	
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

