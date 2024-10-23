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
    	
    	session.AddShortCircuit((SyntaxKind.StatementDelimiterToken, null));
    	
    	while (session.Position < session.TokenList.Count)
    	{
    		var tokenCurrent = session.TokenList[session.Position];
    			
    		var indentation = session.ShortCircuitList.Count;
    		
    		// Check if the tokenCurrent is a token that is used as a end-delimiter before iterating the list?
    		//if (tokenCurrent.SyntaxKind == SyntaxKind.CloseParenthesisToken)
    		//{
    			for (int i = session.ShortCircuitList.Count - 1; i > -1; i--)
	    		{
	    			var tuple = session.ShortCircuitList[i];
	    			
	    			if (tuple.DelimiterSyntaxKind == tokenCurrent.SyntaxKind)
	    			{
	    				session.RemoveRangeShortCircuit(i, session.ShortCircuitList.Count - i);
	    				
	    				// This is a hack to have SyntaxKind.StatementDelimiterToken break out of the expression.
	    				// The parser is adding as the 0th item that
	    				// 'SyntaxKind.StatementDelimiterToken' returns the primary expression to be 'null'.
	    				//
	    				// One isn't supposed to deal with nulls here, instead using EmptyExpressionNode.
	    				// So, if i==0 && tuple.ExpressionNode is null then
	    				// this special case to break out of the expresion logic exists.
	    				//
	    				// It needs to be part of the session.ShortCircuitList however,
	    				// because if an expression uses 'SyntaxKind.StatementDelimiterToken'
	    				// in their expression, they can override this 0th index entry
	    				// and have primary expression "short circuit" to their choosing
	    				// and the loop will continue parsing more expressions.
	    				//
	    				// LambdaExpressionNode for example, needs to override 'SyntaxKind.StatementDelimiterToken'.
	    				if (i == 0 && tuple.ExpressionNode is null)
	    					return expressionPrimary;
	    				
		    			var expressionSecondary = expressionPrimary;
		    			expressionPrimary = tuple.ExpressionNode;
		    			
		    			WriteMergeBefore(session.ShortCircuitListStringified, $"E_{session.Position}: ", expressionPrimary, expressionSecondary);
		    			expressionPrimary = binder.AnyMergeExpression(expressionPrimary, expressionSecondary, session);
		    			WriteMergeAfter(session.ShortCircuitListStringified, expressionPrimary);
		    			
		    			break;
	    			}
	    		}
    		//}
    		
    		WriteMergeBefore(session.ShortCircuitListStringified, $"T_{session.Position}: ", expressionPrimary, tokenCurrent);
    		expressionPrimary = binder.AnyMergeToken(expressionPrimary, tokenCurrent, session);
    		WriteMergeAfter(session.ShortCircuitListStringified, expressionPrimary);
    		
    		session.Position++;
    		Console.WriteLine();
    	}
    	
    	return expressionPrimary;
    }
    
    private static void WriteMergeBefore(
    	string indentation,
    	string message,
    	IExpressionNode expressionPrimary,
    	ISyntax syntax)
	{
		Console.Write(indentation);
			
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
	
	private static void WriteMergeAfter(string indentation, IExpressionNode expressionResult)
	{
		Console.Write(indentation);
			
		Console.Write($"\t-> ");
		
		Console.Write("'");
		WriteSyntax(expressionResult);
		Console.Write("'");
		
		Console.WriteLine();
	}
	
	private static void WriteSyntax(ISyntax syntax)
	{
		if (syntax is null)
			return;
	
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
			
			if (constructorInvocationNode.NewKeywordToken.ConstructorWasInvoked)
				Console.Write($"new ");
			else
				Console.Write($"badnew ");
			
	        if (constructorInvocationNode.ResultTypeClauseNode is not null)
				WriteSyntax(constructorInvocationNode.ResultTypeClauseNode);
	        if (constructorInvocationNode.FunctionParametersListingNode is not null)
				WriteSyntax(constructorInvocationNode.FunctionParametersListingNode);
	        if (constructorInvocationNode.ObjectInitializationParametersListingNode is not null)
				WriteSyntax(constructorInvocationNode.ObjectInitializationParametersListingNode);
		}
		else if (syntax.SyntaxKind == SyntaxKind.ObjectInitializationParametersListingNode)
		{
			var objectInitializationParametersListingNode = (ObjectInitializationParametersListingNode)syntax;
			
			WriteSyntax(objectInitializationParametersListingNode.OpenBraceToken);
			
			for (int i = 0; i < objectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList.Count; i++)
			{
				var objectInitializationParameterEntryNode = objectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[i];
				WriteSyntax(objectInitializationParameterEntryNode);
				
				if (i < objectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList.Count - 1)
				{
					Console.Write(',');
				}
			}
	        
	        WriteSyntax(objectInitializationParametersListingNode.CloseBraceToken);
		}
		else if (syntax.SyntaxKind == SyntaxKind.ObjectInitializationParameterEntryNode)
		{
			var objectInitializationParameterEntryNode = (ObjectInitializationParameterEntryNode)syntax;
			
			WriteSyntax(objectInitializationParameterEntryNode.PropertyIdentifierToken);
	        WriteSyntax(objectInitializationParameterEntryNode.EqualsToken);
	        WriteSyntax(objectInitializationParameterEntryNode.ExpressionNode);
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
			
			try
			{
				Console.Write($"{identifierToken.TextSpan.GetText()}");
			}
			catch (Exception e)
			{
				Console.Write($"EXCEPTIONidentifierToken.TextSpan.GetText()EXCEPTION");
			}
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
		else if (syntax.SyntaxKind == SyntaxKind.FunctionInvocationNode)
		{
			var functionInvocationNode = (FunctionInvocationNode)syntax;
			
			WriteSyntax(functionInvocationNode.ResultTypeClauseNode);
			
			Console.Write(' ');
			
			WriteSyntax(functionInvocationNode.FunctionInvocationIdentifierToken);
			WriteSyntax(functionInvocationNode.GenericParametersListingNode);
	  	  WriteSyntax(functionInvocationNode.FunctionParametersListingNode);
	  	  
	  	  // WriteSyntax(functionInvocationNode.FunctionDefinitionNode);
		}
		else if (syntax.SyntaxKind == SyntaxKind.FunctionParametersListingNode)
		{
			var functionParametersListingNode = (FunctionParametersListingNode)syntax;
			
			WriteSyntax(functionParametersListingNode.OpenParenthesisToken);
			for (int i = 0; i < functionParametersListingNode.FunctionParameterEntryNodeList.Count; i++)
			{
				var functionParameterEntryNode = functionParametersListingNode.FunctionParameterEntryNodeList[i];
				WriteSyntax(functionParameterEntryNode);
				if (i < functionParametersListingNode.FunctionParameterEntryNodeList.Count - 1)
					Console.Write(',');
			}
	        WriteSyntax(functionParametersListingNode.CloseParenthesisToken);
		}
		else if (syntax.SyntaxKind == SyntaxKind.FunctionParameterEntryNode)
		{
			var functionParameterEntryNode = (FunctionParameterEntryNode)syntax;
			
			WriteSyntax(functionParameterEntryNode.ExpressionNode);
			
			if (functionParameterEntryNode.HasOutKeyword)
	        	Console.Write("out");
	        if (functionParameterEntryNode.HasInKeyword)
	        	Console.Write("in");
	        if (functionParameterEntryNode.HasRefKeyword)
	        	Console.Write("ref");
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
					Console.Write("__" + syntax.SyntaxKind + "__");
					//Console.Write("_");
					
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
