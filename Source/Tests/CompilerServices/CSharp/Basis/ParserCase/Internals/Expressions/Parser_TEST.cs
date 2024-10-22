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
    	Console.WriteLine();
    	
    	var binder = new Binder_TEST();
    	IExpressionNode expressionPrimary = new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
    	session.Position = 0;
    	
    	while (session.Position < session.TokenList.Count)
    	{
    		var tokenCurrent = session.TokenList[session.Position];
    		if (tokenCurrent.SyntaxKind == SyntaxKind.StatementDelimiterToken)
    			break;
    			
    		var indentation = session.ShortCircuitList.Count;
    		
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
		    			
		    			WriteMergeBefore(indentation, $"E_{session.Position}: ", expressionPrimary, expressionSecondary);
		    			expressionPrimary = binder.AnyMergeExpression(expressionPrimary, expressionSecondary, session);
		    			WriteMergeAfter(indentation, expressionPrimary);
		    			
		    			break;
	    			}
	    		}
    		//}
    		
    		WriteMergeBefore(session.ShortCircuitList.Count, $"T_{session.Position}: ", expressionPrimary, tokenCurrent);
    		expressionPrimary = binder.AnyMergeToken(expressionPrimary, tokenCurrent, session);
    		WriteMergeAfter(session.ShortCircuitList.Count, expressionPrimary);
    		
    		session.Position++;
    		Console.WriteLine();
    	}
    	
    	return expressionPrimary;
    }
    
    private static void WriteMergeBefore(
    	int indentation,
    	string message,
    	IExpressionNode expressionPrimary,
    	ISyntax syntax)
	{
		for (int i = 0; i < indentation; i++)
			Console.Write("====");
			
		Console.Write(message);
		
		Console.Write("'");
		WriteSyntax(expressionPrimary);
		Console.Write("'");
		
		Console.Write(" + ");
		
		Console.Write("'");
		WriteSyntax(syntax);
		Console.Write("'");
		
		Console.WriteLine();
	}
	
	private static void WriteMergeAfter(int indentation, IExpressionNode expressionResult)
	{
		for (int i = 0; i < indentation; i++)
			Console.Write("====");
			
		Console.Write($"\t-> ");
		
		Console.Write("'");
		WriteSyntax(expressionResult);
		Console.Write("'");
		
		Console.WriteLine();
	}
	
	private static void WriteSyntax(ISyntax syntax)
	{
		if (syntax.SyntaxKind == SyntaxKind.BadExpressionNode)
		{
			var badExpressionNode = (BadExpressionNode)syntax;
			
			Console.Write("b{");
			for (var i = 0; i < badExpressionNode.SyntaxList.Count; i++)
			{
				var child = badExpressionNode.SyntaxList[i];
				WriteSyntax(child);
				
				if (i < badExpressionNode.SyntaxList.Count - 1)
					Console.Write(",");
			}
			Console.Write("}");
		}
		else if (syntax.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)syntax;
			Console.Write($"a{ambiguousIdentifierExpressionNode.Token.TextSpan.GetText()}");
			
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
		else if (syntax.SyntaxKind == SyntaxKind.ConstructorInvocationExpressionNode)
		{
			var constructorInvocationNode = (ConstructorInvocationExpressionNode)syntax;
			
			if (!constructorInvocationNode.NewKeywordToken.ConstructorWasInvoked)
				Console.Write($"new");
			else
				Console.Write($"badnew ");
			
	        if (constructorInvocationNode.ResultTypeClauseNode is not null)
				WriteSyntax(constructorInvocationNode.ResultTypeClauseNode);
	        if (constructorInvocationNode.FunctionParametersListingNode is not null)
				WriteSyntax(constructorInvocationNode.FunctionParametersListingNode);
	        if (constructorInvocationNode.ObjectInitializationParametersListingNode is not null)
				WriteSyntax(constructorInvocationNode.ObjectInitializationParametersListingNode);
		}
		else if (syntax.SyntaxKind == SyntaxKind.TypeClauseNode)
		{
			var typeClauseNode = (TypeClauseNode)syntax;
			WriteSyntax(typeClauseNode.TypeIdentifierToken);
			WriteSyntax(typeClauseNode.GenericParametersListingNode);
		}
		else if (syntax.SyntaxKind == SyntaxKind.IdentifierToken)
		{
			var identifierToken = (IdentifierToken)syntax;
			Console.Write($"{identifierToken.TextSpan.GetText()}");
		}
		else if (syntax.SyntaxKind == SyntaxKind.LiteralExpressionNode)
		{
			var literalExpressionNode = (LiteralExpressionNode)syntax;
			Console.Write($"{literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText()}");
		}
		else if (syntax.SyntaxKind == SyntaxKind.BinaryExpressionNode)
		{
			var binaryExpressionNode = (BinaryExpressionNode)syntax;
			
			WriteSyntax(binaryExpressionNode.LeftExpressionNode);
	        WriteSyntax(binaryExpressionNode.BinaryOperatorNode);
	        WriteSyntax(binaryExpressionNode.RightExpressionNode);
		}
		else if (syntax.SyntaxKind == SyntaxKind.BinaryOperatorNode)
		{
			var binaryOperatorNode = (BinaryOperatorNode)syntax;
			WriteSyntax(binaryOperatorNode.OperatorToken);
		}
		else if (syntax.SyntaxKind == SyntaxKind.EmptyExpressionNode)
		{
			Console.Write("e");
		}
		else if (syntax.SyntaxKind == SyntaxKind.ParenthesizedExpressionNode)
		{
			var parenthesizedExpressionNode = (ParenthesizedExpressionNode)syntax;
			
			
			WriteSyntax(parenthesizedExpressionNode.OpenParenthesisToken);
	  	  WriteSyntax(parenthesizedExpressionNode.InnerExpression);
	        WriteSyntax(parenthesizedExpressionNode.CloseParenthesisToken);
		}
		else
		{
			if (syntax is ISyntaxToken token)
			{
				if (token.ConstructorWasInvoked)
				{
					Console.Write(token.TextSpan.GetText());
				}
				else
				{
					Console.Write("_");
					
					/*if (syntax.SyntaxKind == SyntaxKind.CloseParenthesisToken)
						Console.Write(")");
					else
						Console.Write(syntax.SyntaxKind);*/
				}
			}
			else
			{
				Console.Write($"{syntax.SyntaxKind}");
			}
		}
	}
}
