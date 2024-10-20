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
		    			
		    			void WriteBadExpressionNode(BadExpressionNode badExpressionNode)
		    			{
		    				Console.Write($"{expressionPrimary.SyntaxKind} {{");
		    				
		    				foreach (var child in badExpressionNode.SyntaxList)
		    				{
		    					Console.Write($"{child.SyntaxKind}, ");
		    				}
		    				
		    				Console.Write(" }, ");
		    			}
		    			
		    			Console.Write("MERGE: ");
		    			
		    			if (expressionPrimary.SyntaxKind == SyntaxKind.BadExpressionNode)
		    			{
		    				WriteBadExpressionNode((BadExpressionNode)expressionPrimary);
		    			}
		    			else
		    			{
		    				Console.Write("{expressionPrimary.SyntaxKind}, ");
		    			}
		    			
		    			if (expressionSecondary.SyntaxKind == SyntaxKind.BadExpressionNode)
		    			{
		    				WriteBadExpressionNode((BadExpressionNode)expressionSecondary);
		    			}
		    			else
		    			{
		    				Console.Write("{expressionSecondary.SyntaxKind}, ");
		    			}
		    			Console.WriteLine();
		    			
		    			
		    			expressionPrimary = binder.AnyMergeExpression(expressionPrimary, expressionSecondary, session);
		    			
		    			Console.Write($"\t-> ");
		    			if (expressionPrimary.SyntaxKind == SyntaxKind.BadExpressionNode)
		    			{
		    				WriteBadExpressionNode((BadExpressionNode)expressionPrimary);
		    			}
		    			else
		    			{
		    				Console.Write("{expressionPrimary.SyntaxKind}, ");
		    			}
		    			Console.WriteLine();
		    			break;
	    			}
	    		}
    		//}
    		
    		expressionPrimary = binder.AnyMergeToken(expressionPrimary, tokenCurrent, session);

    		position++;
    	}
    	
    	return expressionPrimary;
    }
}
