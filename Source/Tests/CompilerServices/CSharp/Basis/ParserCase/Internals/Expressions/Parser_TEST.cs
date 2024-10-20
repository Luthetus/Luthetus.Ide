using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals.Expressions;

public class Parser_TEST
{
	public static IExpressionNode ParseExpression(ExpressionSession session)
    {
    	var binder = new Binder_TEST();
    	IExpressionNode expressionPrimary = new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
    	int position = 0;
    	
    	while (position < session.TokenList.Count)
    	{
    		var tokenCurrent = session.TokenList[position];
    		if (tokenCurrent.SyntaxKind == SyntaxKind.StatementDelimiterToken)
    			break;
    		
    		// Check if the tokenCurrent is a token that is used as a end-delimiter before iterating the list?
    		//if (tokenCurrent.SyntaxKind == SyntaxKind.CloseParenthesisToken)
    		//{
    			for (int i = session.ShortCircuitList.Count - 1; i > -1; i--)
	    		{
	    			var tuple = session.ShortCircuitList[i];
	    			
	    			if (tuple.DelimiterSyntaxKind == tokenCurrent.SyntaxKind)
	    			{
	    				session.ShortCircuitList.RemoveRange(i, session.ShortCircuitList.Count - i);
	    				
		    			var expressionSecondary = expressionPrimary;
		    			expressionPrimary = tuple.ExpressionNode;
		    			
		    			var expressionResult = binder.AnyMergeExpression(expressionPrimary, expressionSecondary, session);
		    			WriteMerge(expressionPrimary, expressionSecondary, expressionResult);
		    			
		    			expressionPrimary = expressionResult;
		    			break;
	    			}
	    		}
    		//}
    		
    		expressionPrimary = binder.AnyMergeToken(expressionPrimary, tokenCurrent, session);

    		position++;
    	}
    	
    	return expressionPrimary;
    }
    
    /// <summary>This method is nasty I'm just logging to console for debug purposes.</summary>
    private static void WriteMerge(
    	IExpressionNode expressionPrimary,
    	IExpressionNode expressionSecondary,
    	IExpressionNode expressionResult)
	{
		Console.Write("MERGE: ");
		
		WriteSyntax(expressionPrimary);
		Console.Write(" + ");
		WriteSyntax(expressionSecondary);
		Console.WriteLine();
		
		Console.Write($"\t-> ");
		WriteSyntax(expressionResult);
		Console.WriteLine();
	}
	
	private static void WriteSyntax(ISyntax syntax)
	{
		if (syntax.SyntaxKind == SyntaxKind.BadExpressionNode)
		{
			var badExpressionNode = (BadExpressionNode)syntax;
			
			Console.Write($"{badExpressionNode.SyntaxKind} {{");
		
			foreach (var child in badExpressionNode.SyntaxList)
			{
				WriteSyntax(child);
			}
			
			Console.Write(" }, ");
		}
		else if (syntax.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)syntax;
			Console.Write($"AIEN_{ambiguousIdentifierExpressionNode.Token.TextSpan.GetText()}");
			
			if (ambiguousIdentifierExpressionNode.GenericParametersListingNode is not null)
			{
				WriteSyntax(ambiguousIdentifierExpressionNode.GenericParametersListingNode);
			}
		}
		else if (syntax.SyntaxKind == SyntaxKind.GenericParametersListingNode)
		{
			var genericParametersListingNode = (GenericParametersListingNode)syntax;
			
			Console.Write("<");
			for (int i = 0; i < genericParametersListingNode.GenericParameterEntryNodeList.Count; i++)
			{
				Console.Write($"{genericParametersListingNode.GenericParameterEntryNodeList[i].TypeClauseNode.TypeIdentifierToken.TextSpan.GetText()}");
				
				if (i < genericParametersListingNode.GenericParameterEntryNodeList.Count - 1)
					Console.Write(",");
			}
			Console.Write(">");
		}
		else
		{
			Console.Write($"{syntax.SyntaxKind}, ");
		}
	}
}
