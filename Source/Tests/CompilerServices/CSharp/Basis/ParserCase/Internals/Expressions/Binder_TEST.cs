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

/// <summary>Methods are alphabetically sorted ONLY by the first word.</summary>
public class Binder_TEST
{
	/// <summary>
	/// Returns the new primary expression which will be the passed in 'expressionPrimary'
	/// if the parameters were not mergeable.
	/// </summary>
	public IExpressionNode AnyMergeToken(
		IExpressionNode expressionPrimary, ISyntaxToken token, ExpressionSession session)
	{
		switch (expressionPrimary.SyntaxKind)
		{
			case SyntaxKind.EmptyExpressionNode:
				return EmptyMergeToken((EmptyExpressionNode)expressionPrimary, token, session);
			case SyntaxKind.LiteralExpressionNode:
				return LiteralMergeToken((LiteralExpressionNode)expressionPrimary, token, session);
			case SyntaxKind.BinaryExpressionNode:
				return BinaryMergeToken((BinaryExpressionNode)expressionPrimary, token, session);
			case SyntaxKind.ParenthesizedExpressionNode:
				return ParenthesizedMergeToken((ParenthesizedExpressionNode)expressionPrimary, token, session);
			case SyntaxKind.FunctionInvocationNode:
				return FunctionInvocationMergeToken((FunctionInvocationNode)expressionPrimary, token, session);
			case SyntaxKind.ConstructorInvocationExpressionNode:
				return ConstructorInvocationMergeToken((ConstructorInvocationExpressionNode)expressionPrimary, token, session);
			case SyntaxKind.ExplicitCastNode:
				return ExplicitCastMergeToken((ExplicitCastNode)expressionPrimary, token, session);
			case SyntaxKind.AmbiguousIdentifierExpressionNode:
				return AmbiguousIdentifierMergeToken((AmbiguousIdentifierExpressionNode)expressionPrimary, token, session);
			case SyntaxKind.BadExpressionNode:
				return BadMergeToken((BadExpressionNode)expressionPrimary, token, session);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), expressionPrimary, token);
		};
	}
	
	/// <summary>
	/// Returns the new primary expression which will be the passed in 'expressionPrimary'
	/// if the parameters were not mergeable.
	/// </summary>
	public IExpressionNode AnyMergeExpression(
		IExpressionNode expressionPrimary, IExpressionNode expressionSecondary, ExpressionSession session)
	{
		switch (expressionPrimary.SyntaxKind)
		{
			case SyntaxKind.ParenthesizedExpressionNode:
				return ParenthesizedMergeExpression((ParenthesizedExpressionNode)expressionPrimary, expressionSecondary, session);
			case SyntaxKind.FunctionInvocationNode:
				return FunctionInvocationMergeExpression((FunctionInvocationNode)expressionPrimary, expressionSecondary, session);
			case SyntaxKind.ConstructorInvocationExpressionNode:
				return ConstructorInvocationMergeExpression((ConstructorInvocationExpressionNode)expressionPrimary, expressionSecondary, session);
			case SyntaxKind.AmbiguousIdentifierExpressionNode:
				return AmbiguousIdentifierMergeExpression((AmbiguousIdentifierExpressionNode)expressionPrimary, expressionSecondary, session);
			case SyntaxKind.BadExpressionNode:
				return BadMergeExpression((BadExpressionNode)expressionPrimary, expressionSecondary, session);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), expressionPrimary, expressionSecondary);
		};
	}

	public IExpressionNode AmbiguousIdentifierMergeToken(
		AmbiguousIdentifierExpressionNode ambiguousIdentifierExpressionNode, ISyntaxToken token, ExpressionSession session)
	{
		if (token.SyntaxKind == SyntaxKind.OpenParenthesisToken &&
			ambiguousIdentifierExpressionNode.Token.SyntaxKind == SyntaxKind.IdentifierToken)
		{
			var functionParametersListingNode = new FunctionParametersListingNode(
				(OpenParenthesisToken)token,
		        new List<FunctionParameterEntryNode>(),
		        closeParenthesisToken: default);
		
			// TODO: ContextualKeywords as the function identifier?
			var functionInvocationNode = new FunctionInvocationNode(
				(IdentifierToken)ambiguousIdentifierExpressionNode.Token,
		        functionDefinitionNode: null,
		        ambiguousIdentifierExpressionNode.GenericParametersListingNode,
		        functionParametersListingNode,
		        CSharpFacts.Types.Void.ToTypeClause());
			
			session.ShortCircuitList.Add((SyntaxKind.CloseParenthesisToken, functionInvocationNode));
			session.ShortCircuitList.Add((SyntaxKind.CommaToken, functionInvocationNode));
			return new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
		}
		else if (token.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
		{
			if (ambiguousIdentifierExpressionNode.GenericParametersListingNode is null)
			{
				ambiguousIdentifierExpressionNode.SetGenericParametersListingNode(
					new GenericParametersListingNode(
						(OpenAngleBracketToken)token,
				        new List<GenericParameterEntryNode>(),
				        closeAngleBracketToken: default));
			}
			
		    session.ShortCircuitList.Add((SyntaxKind.CloseAngleBracketToken, ambiguousIdentifierExpressionNode));
			session.ShortCircuitList.Add((SyntaxKind.CommaToken, ambiguousIdentifierExpressionNode));
			return new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
		}
		else if (token.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
		{
			ambiguousIdentifierExpressionNode.GenericParametersListingNode.SetCloseAngleBracketToken((CloseAngleBracketToken)token);
			return ambiguousIdentifierExpressionNode;
		}
		else if (token.SyntaxKind == SyntaxKind.CommaToken)
		{
			session.ShortCircuitList.Add((SyntaxKind.CommaToken, ambiguousIdentifierExpressionNode));
			return new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
		}
	
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), ambiguousIdentifierExpressionNode, token);
	}
		
	public IExpressionNode AmbiguousIdentifierMergeExpression(
		AmbiguousIdentifierExpressionNode ambiguousIdentifierExpressionNode, IExpressionNode expressionSecondary, ExpressionSession session)
	{
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			var expressionSecondaryTyped = (AmbiguousIdentifierExpressionNode)expressionSecondary;
			
			var typeClauseNode = new TypeClauseNode(
				expressionSecondaryTyped.Token,
		        valueType: null,
		        genericParametersListingNode: null);
			
			if (ambiguousIdentifierExpressionNode.GenericParametersListingNode is not null)
			{
				ambiguousIdentifierExpressionNode.GenericParametersListingNode.GenericParameterEntryNodeList.Add(
					new GenericParameterEntryNode(typeClauseNode));
				
				return ambiguousIdentifierExpressionNode;
			}
		}
		
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), ambiguousIdentifierExpressionNode, expressionSecondary);
	}
		
	public IExpressionNode BadMergeToken(
		BadExpressionNode badExpressionNode, ISyntaxToken token, ExpressionSession session)
	{
		badExpressionNode.SyntaxList.Add(token);
		return badExpressionNode;
	}

	public IExpressionNode BadMergeExpression(
		BadExpressionNode badExpressionNode, IExpressionNode expressionSecondary, ExpressionSession session)
	{
		badExpressionNode.SyntaxList.Add(expressionSecondary);
		return badExpressionNode;
	}

	public IExpressionNode BinaryMergeToken(
		BinaryExpressionNode binaryExpressionNode, ISyntaxToken token, ExpressionSession session)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.NumericLiteralToken:
			case SyntaxKind.StringLiteralToken:
			case SyntaxKind.CharLiteralToken:
			case SyntaxKind.FalseTokenKeyword:
			case SyntaxKind.TrueTokenKeyword:
				TypeClauseNode tokenTypeClauseNode;
				
				if (token.SyntaxKind == SyntaxKind.NumericLiteralToken)
					tokenTypeClauseNode = CSharpFacts.Types.Int.ToTypeClause();
				else if (token.SyntaxKind == SyntaxKind.StringLiteralToken)
					tokenTypeClauseNode = CSharpFacts.Types.String.ToTypeClause();
				else if (token.SyntaxKind == SyntaxKind.CharLiteralToken)
					tokenTypeClauseNode = CSharpFacts.Types.Char.ToTypeClause();
				else if (token.SyntaxKind == SyntaxKind.FalseTokenKeyword || token.SyntaxKind == SyntaxKind.TrueTokenKeyword)
					tokenTypeClauseNode = CSharpFacts.Types.Bool.ToTypeClause();
				else
					goto default;
					
				var tokenTypeClauseNodeText = tokenTypeClauseNode.TypeIdentifierToken.TextSpan.GetText();
			
				var leftExpressionTypeClauseNodeText = binaryExpressionNode.LeftExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText();
				if (leftExpressionTypeClauseNodeText != tokenTypeClauseNodeText)
					goto default;
			
				var rightExpressionNode = new LiteralExpressionNode(token, tokenTypeClauseNode);
				binaryExpressionNode.SetRightExpressionNode(rightExpressionNode);
				return binaryExpressionNode;
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.StarToken:
		    case SyntaxKind.DivisionToken:
		    case SyntaxKind.EqualsEqualsToken:
				// TODO: More generally, the result will be a number, so all that matters is what operators a number can interact with instead of duplicating this code.
				if (binaryExpressionNode.RightExpressionNode.SyntaxKind != SyntaxKind.EmptyExpressionNode)
	    		{
	    			var typeClauseNode = binaryExpressionNode.ResultTypeClauseNode;
    				var binaryOperatorNode = new BinaryOperatorNode(typeClauseNode, token, typeClauseNode, typeClauseNode);
    				return new BinaryExpressionNode(binaryExpressionNode, binaryOperatorNode, new EmptyExpressionNode(typeClauseNode));
	    		}
	    		else
	    		{
	    			goto default;
	    		}
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), binaryExpressionNode, token);
		}
	}
	
	public IExpressionNode ConstructorInvocationMergeToken(
		ConstructorInvocationExpressionNode constructorInvocationExpressionNode, ISyntaxToken token, ExpressionSession session)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.IdentifierToken:
				if (constructorInvocationExpressionNode.ResultTypeClauseNode is null)
				{
					if (token.SyntaxKind == SyntaxKind.IdentifierToken ||
					    UtilityApi.IsTypeIdentifierKeywordSyntaxKind(token.SyntaxKind))
					{
						var typeClauseNode = new TypeClauseNode(
							token,
        					valueType: null,
        					genericParametersListingNode: null);
						
						return constructorInvocationExpressionNode.SetTypeClauseNode(typeClauseNode);
					}
				}
				
				goto default;
			case SyntaxKind.OpenParenthesisToken:
				var functionParametersListingNode = new FunctionParametersListingNode(
					(OpenParenthesisToken)token,
			        new List<FunctionParameterEntryNode>(),
			        closeParenthesisToken: default);
			        
			    constructorInvocationExpressionNode.SetFunctionParametersListingNode(functionParametersListingNode);
				
				session.ShortCircuitList.Add((SyntaxKind.CloseParenthesisToken, constructorInvocationExpressionNode));
				session.ShortCircuitList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode));
				return new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
			case SyntaxKind.CloseParenthesisToken:
				constructorInvocationExpressionNode.FunctionParametersListingNode.SetCloseParenthesisToken((CloseParenthesisToken)token);
				return constructorInvocationExpressionNode;
			case SyntaxKind.CommaToken:
				session.ShortCircuitList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode));
				return new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), constructorInvocationExpressionNode, token);
		}
	}
	
	public IExpressionNode ConstructorInvocationMergeExpression(
		ConstructorInvocationExpressionNode constructorInvocationExpressionNode, IExpressionNode expressionSecondary, ExpressionSession session)
	{
		switch (expressionSecondary.SyntaxKind)
		{
			case SyntaxKind.EmptyExpressionNode:
				return constructorInvocationExpressionNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), constructorInvocationExpressionNode, expressionSecondary);
		}
	}
	
	public IExpressionNode EmptyMergeToken(
		EmptyExpressionNode emptyExpressionNode, ISyntaxToken token, ExpressionSession session)
	{
		if (token.SyntaxKind == SyntaxKind.IdentifierToken ||
			UtilityApi.IsTypeIdentifierKeywordSyntaxKind(token.SyntaxKind))
		{
			var ambiguousExpressionNode = new AmbiguousIdentifierExpressionNode(
				token,
		        genericParametersListingNode: null,
		        CSharpFacts.Types.Void.ToTypeClause());
		    
		    return ambiguousExpressionNode;
		}
	
		switch (token.SyntaxKind)
		{
			case SyntaxKind.NumericLiteralToken:
			case SyntaxKind.StringLiteralToken:
			case SyntaxKind.CharLiteralToken:
			case SyntaxKind.FalseTokenKeyword:
			case SyntaxKind.TrueTokenKeyword:
				TypeClauseNode tokenTypeClauseNode;
				
				if (token.SyntaxKind == SyntaxKind.NumericLiteralToken)
					tokenTypeClauseNode = CSharpFacts.Types.Int.ToTypeClause();
				else if (token.SyntaxKind == SyntaxKind.StringLiteralToken)
					tokenTypeClauseNode = CSharpFacts.Types.String.ToTypeClause();
				else if (token.SyntaxKind == SyntaxKind.CharLiteralToken)
					tokenTypeClauseNode = CSharpFacts.Types.Char.ToTypeClause();
				else if (token.SyntaxKind == SyntaxKind.FalseTokenKeyword || token.SyntaxKind == SyntaxKind.TrueTokenKeyword)
					tokenTypeClauseNode = CSharpFacts.Types.Bool.ToTypeClause();
				else
					goto default;
					
				return new LiteralExpressionNode(token, tokenTypeClauseNode);
			case SyntaxKind.OpenParenthesisToken:
				var parenthesizedExpressionNode = new ParenthesizedExpressionNode((OpenParenthesisToken)token, CSharpFacts.Types.Void.ToTypeClause());
				session.ShortCircuitList.Add((SyntaxKind.CloseParenthesisToken, parenthesizedExpressionNode));
				return new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
			case SyntaxKind.NewTokenKeyword:
				return new ConstructorInvocationExpressionNode(
					(KeywordToken)token,
			        typeClauseNode: null,
			        functionParametersListingNode: null,
			        objectInitializationParametersListingNode: null);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), emptyExpressionNode, token);
		}
	}
	
	public IExpressionNode ExplicitCastMergeToken(
		ExplicitCastNode explicitCastNode, ISyntaxToken token, ExpressionSession session)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CloseParenthesisToken:
				return explicitCastNode.SetCloseParenthesisToken((CloseParenthesisToken)token);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), explicitCastNode, token);
		}
	}

	/// <summary>
	/// I am not evaluating anything when parsing for the IDE, so for now I'm going to ignore the precedence,
	/// and just track the start and end of the expression more or less.
	///
	/// Reason for this being: object initialization and collection initialization
	/// currently will at times break the Parser for an entire file, and therefore
	/// they are much higher priority.
	/// </summary>
	public IExpressionNode LiteralMergeToken(
		LiteralExpressionNode literalExpressionNode, ISyntaxToken token, ExpressionSession session)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.StarToken:
		    case SyntaxKind.DivisionToken:
		    case SyntaxKind.EqualsEqualsToken:
				var typeClauseNode = literalExpressionNode.ResultTypeClauseNode;
				var binaryOperatorNode = new BinaryOperatorNode(typeClauseNode, token, typeClauseNode, typeClauseNode);
				return new BinaryExpressionNode(literalExpressionNode, binaryOperatorNode);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), literalExpressionNode, token);
		}
	}
	
	public IExpressionNode ParenthesizedMergeToken(
		ParenthesizedExpressionNode parenthesizedExpressionNode, ISyntaxToken token, ExpressionSession session)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CloseParenthesisToken:
				return parenthesizedExpressionNode.SetCloseParenthesisToken((CloseParenthesisToken)token);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), parenthesizedExpressionNode, token);
		}
	}
	
	public IExpressionNode ParenthesizedMergeExpression(
		ParenthesizedExpressionNode parenthesizedExpressionNode, IExpressionNode expressionSecondary, ExpressionSession session)
	{
		if (parenthesizedExpressionNode.InnerExpression.SyntaxKind != SyntaxKind.EmptyExpressionNode)
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), parenthesizedExpressionNode, expressionSecondary);
			
		if (expressionSecondary.SyntaxKind == SyntaxKind.BadExpressionNode)
		{
			var badExpressionNode = (BadExpressionNode)expressionSecondary;
			
			if (badExpressionNode.SyntaxList.Count == 2 &&
					(badExpressionNode.SyntaxList[1].SyntaxKind == SyntaxKind.IdentifierToken ||
					 UtilityApi.IsTypeIdentifierKeywordSyntaxKind(badExpressionNode.SyntaxList[1].SyntaxKind)))
			{
				var typeClauseNode = new TypeClauseNode((ISyntaxToken)badExpressionNode.SyntaxList[1], valueType: null, genericParametersListingNode: null);
				return new ExplicitCastNode(parenthesizedExpressionNode.OpenParenthesisToken, typeClauseNode);
			}
		}
			
		return parenthesizedExpressionNode.SetInnerExpression(expressionSecondary);
	}
	
	public IExpressionNode FunctionInvocationMergeToken(
		FunctionInvocationNode functionInvocationNode, ISyntaxToken token, ExpressionSession session)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CloseParenthesisToken:
				functionInvocationNode.FunctionParametersListingNode.SetCloseParenthesisToken((CloseParenthesisToken)token);
				return functionInvocationNode;
			case SyntaxKind.CommaToken:
				session.ShortCircuitList.Add((SyntaxKind.CommaToken, functionInvocationNode));
				return new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), functionInvocationNode, token);
		}
	}
	
	public IExpressionNode FunctionInvocationMergeExpression(
		FunctionInvocationNode functionInvocationNode, IExpressionNode expressionSecondary, ExpressionSession session)
	{
		switch (expressionSecondary.SyntaxKind)
		{
			case SyntaxKind.EmptyExpressionNode:
				return functionInvocationNode;
			default:
				var functionParameterEntryNode = new FunctionParameterEntryNode(
			        expressionSecondary,
			        hasOutKeyword: false,
			        hasInKeyword: false,
			        hasRefKeyword: false);
			        
				functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList.Add(functionParameterEntryNode);
				
				return functionInvocationNode;
		}
	}
}
