using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.CompilerServices.CSharp.Facts;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.LexerCase;

namespace Luthetus.CompilerServices.CSharp.BinderCase;

public partial class CSharpBinder
{
	/// <summary>
	/// Returns the new primary expression which will be the passed in 'expressionPrimary'
	/// if the parameters were not mergeable.
	/// </summary>
	public IExpressionNode AnyMergeToken(
		IExpressionNode expressionPrimary, ISyntaxToken token, IParserModel model)
	{
		if (token.SyntaxKind == SyntaxKind.MemberAccessToken)
		{
			if (expressionPrimary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
			{
				var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)expressionPrimary;
				
				if (!ambiguousIdentifierExpressionNode.FollowsMemberAccessToken)
				{
					ForceDecisionAmbiguousIdentifier(
						EmptyExpressionNode.Empty,
						ambiguousIdentifierExpressionNode,
						model);
				}
			}
			
			return EmptyExpressionNode.EmptyFollowsMemberAccessToken;
		}
		
		switch (expressionPrimary.SyntaxKind)
		{
			case SyntaxKind.EmptyExpressionNode:
				return EmptyMergeToken((EmptyExpressionNode)expressionPrimary, token, model);
			case SyntaxKind.LiteralExpressionNode:
				return LiteralMergeToken((LiteralExpressionNode)expressionPrimary, token, model);
			case SyntaxKind.BinaryExpressionNode:
				return BinaryMergeToken((BinaryExpressionNode)expressionPrimary, token, model);
			case SyntaxKind.ParenthesizedExpressionNode:
				return ParenthesizedMergeToken((ParenthesizedExpressionNode)expressionPrimary, token, model);
			case SyntaxKind.FunctionInvocationNode:
				return FunctionInvocationMergeToken((FunctionInvocationNode)expressionPrimary, token, model);
			case SyntaxKind.LambdaExpressionNode:
				return LambdaMergeToken((LambdaExpressionNode)expressionPrimary, token, model);
			case SyntaxKind.ConstructorInvocationExpressionNode:
				return ConstructorInvocationMergeToken((ConstructorInvocationExpressionNode)expressionPrimary, token, model);
			case SyntaxKind.ExplicitCastNode:
				return ExplicitCastMergeToken((ExplicitCastNode)expressionPrimary, token, model);
			case SyntaxKind.AmbiguousIdentifierExpressionNode:
				return AmbiguousIdentifierMergeToken((AmbiguousIdentifierExpressionNode)expressionPrimary, token, model);
			case SyntaxKind.GenericParametersListingNode:
				return GenericParametersListingMergeToken((GenericParametersListingNode)expressionPrimary, token, model);
			case SyntaxKind.FunctionParametersListingNode:
				return FunctionParametersListingMergeToken((FunctionParametersListingNode)expressionPrimary, token, model);
			case SyntaxKind.BadExpressionNode:
				return BadMergeToken((BadExpressionNode)expressionPrimary, token, model);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), expressionPrimary, token);
		};
	}
	
	/// <summary>
	/// Returns the new primary expression which will be the passed in 'expressionPrimary'
	/// if the parameters were not mergeable.
	/// </summary>
	public IExpressionNode AnyMergeExpression(
		IExpressionNode expressionPrimary, IExpressionNode expressionSecondary, IParserModel model)
	{
		switch (expressionPrimary.SyntaxKind)
		{
			case SyntaxKind.ParenthesizedExpressionNode:
				return ParenthesizedMergeExpression((ParenthesizedExpressionNode)expressionPrimary, expressionSecondary, model);
			case SyntaxKind.FunctionInvocationNode:
				return FunctionInvocationMergeExpression((FunctionInvocationNode)expressionPrimary, expressionSecondary, model);
			case SyntaxKind.LambdaExpressionNode:
				return LambdaMergeExpression((LambdaExpressionNode)expressionPrimary, expressionSecondary, model);
			case SyntaxKind.ConstructorInvocationExpressionNode:
				return ConstructorInvocationMergeExpression((ConstructorInvocationExpressionNode)expressionPrimary, expressionSecondary, model);
			case SyntaxKind.AmbiguousIdentifierExpressionNode:
				return AmbiguousIdentifierMergeExpression((AmbiguousIdentifierExpressionNode)expressionPrimary, expressionSecondary, model);
			case SyntaxKind.GenericParametersListingNode:
				return GenericParametersListingMergeExpression((GenericParametersListingNode)expressionPrimary, expressionSecondary, model);
			case SyntaxKind.FunctionParametersListingNode:
				return FunctionParametersListingMergeExpression((FunctionParametersListingNode)expressionPrimary, expressionSecondary, model);
			case SyntaxKind.BadExpressionNode:
				return BadMergeExpression((BadExpressionNode)expressionPrimary, expressionSecondary, model);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), expressionPrimary, expressionSecondary);
		};
	}

	public IExpressionNode AmbiguousIdentifierMergeToken(
		AmbiguousIdentifierExpressionNode ambiguousIdentifierExpressionNode, ISyntaxToken token, IParserModel model)
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
		        
		    BindFunctionInvocationNode(
		        functionInvocationNode,
		        (CSharpParserModel)model);

			model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, functionInvocationNode));
			model.ExpressionList.Add((SyntaxKind.CommaToken, functionInvocationNode.FunctionParametersListingNode));
			return EmptyExpressionNode.Empty;
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
			
		    model.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, ambiguousIdentifierExpressionNode));
			model.ExpressionList.Add((SyntaxKind.CommaToken, ambiguousIdentifierExpressionNode.GenericParametersListingNode));
			return EmptyExpressionNode.Empty;
		}
		else if (token.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
		{
			if (ambiguousIdentifierExpressionNode.GenericParametersListingNode is not null)
			{
				ambiguousIdentifierExpressionNode.GenericParametersListingNode.SetCloseAngleBracketToken((CloseAngleBracketToken)token);
				return ambiguousIdentifierExpressionNode;
			}
		}
		else if (token.SyntaxKind == SyntaxKind.EqualsToken)
		{
			if (model.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
			{
				var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
				SetLambdaExpressionNodeVariableDeclarationNodeList(lambdaExpressionNode, ambiguousIdentifierExpressionNode, model);
				return lambdaExpressionNode;
			}
			
			model.ExpressionList.Add((SyntaxKind.CommaToken, ambiguousIdentifierExpressionNode));
			return EmptyExpressionNode.Empty;
		}
		else if (token.SyntaxKind == SyntaxKind.IsTokenKeyword)
		{
			ForceDecisionAmbiguousIdentifier(
				EmptyExpressionNode.Empty,
				ambiguousIdentifierExpressionNode,
				model);
				
			_ = model.TokenWalker.Consume(); // Consume the IsTokenKeyword
			
			if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.NotTokenContextualKeyword)
				_ = model.TokenWalker.Consume(); // Consume the NotTokenKeyword
			
			var typeClauseNode = ParseTypes.MatchTypeClause((CSharpParserModel)model);
			
			if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.IdentifierToken)
			{
				var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
				
				_ = ParseVariables.HandleVariableDeclarationExpression(
			        typeClauseNode,
			        identifierToken,
			        VariableKind.Local,
			        model);
			}
			
			// Guaranteed to consume 1 further than the secondary loop so have to backtrack 1 time as well.
			_ = model.TokenWalker.Backtrack();
			
			return ambiguousIdentifierExpressionNode;
		}
	
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), ambiguousIdentifierExpressionNode, token);
	}
		
	public IExpressionNode AmbiguousIdentifierMergeExpression(
		AmbiguousIdentifierExpressionNode ambiguousIdentifierExpressionNode, IExpressionNode expressionSecondary, IParserModel model)
	{
		if (ambiguousIdentifierExpressionNode.GenericParametersListingNode is not null &&
			!ambiguousIdentifierExpressionNode.GenericParametersListingNode.CloseAngleBracketToken.ConstructorWasInvoked)
		{
			return ambiguousIdentifierExpressionNode;
		}
		
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), ambiguousIdentifierExpressionNode, expressionSecondary);
	}
	
	public IExpressionNode ForceDecisionAmbiguousIdentifier(
		IExpressionNode expressionPrimary,
		AmbiguousIdentifierExpressionNode ambiguousIdentifierExpressionNode,
		IParserModel model,
		bool forceTypeClauseNode = false)
	{
		if (ambiguousIdentifierExpressionNode.FollowsMemberAccessToken)
			return ambiguousIdentifierExpressionNode;
	
		if (expressionPrimary.SyntaxKind == SyntaxKind.ParenthesizedExpressionNode)
		{
			forceTypeClauseNode = true;
		}
	
		if (!forceTypeClauseNode && ambiguousIdentifierExpressionNode.Token.SyntaxKind == SyntaxKind.IdentifierToken)
		{
			if (TryGetVariableDeclarationNodeByScope(
        		model,
        		model.BinderSession.ResourceUri,
        		model.BinderSession.CurrentScopeIndexKey,
        		ambiguousIdentifierExpressionNode.Token.TextSpan.GetText(),
        		out var existingVariableDeclarationNode))
			{
				var variableReferenceNode = ConstructAndBindVariableReferenceNode(
					(IdentifierToken)ambiguousIdentifierExpressionNode.Token,
					(CSharpParserModel)model);
    			
    			return variableReferenceNode;
			}
		}
		
		if (ambiguousIdentifierExpressionNode.Token.SyntaxKind == SyntaxKind.IdentifierToken ||
			UtilityApi.IsTypeIdentifierKeywordSyntaxKind(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
		{
			if (TryGetTypeDefinitionHierarchically(
        		model,
        		model.BinderSession.ResourceUri,
                model.BinderSession.CurrentScopeIndexKey,
                ambiguousIdentifierExpressionNode.Token.TextSpan.GetText(),
                out var typeDefinitionNode))
	        {
	            var typeClauseNode = new TypeClauseNode(
					(ISyntaxToken)ambiguousIdentifierExpressionNode.Token,
					valueType: null,
					genericParametersListingNode: null);
				
				BindTypeClauseNode(
			        typeClauseNode,
			        (CSharpParserModel)model);
			        
			    return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), EmptyExpressionNode.Empty, typeClauseNode);
	        }
		}
		
		// Bind an undefined-variable
		if (!forceTypeClauseNode && ambiguousIdentifierExpressionNode.Token.SyntaxKind == SyntaxKind.IdentifierToken)
		{
			var variableReferenceNode = ConstructAndBindVariableReferenceNode(
				(IdentifierToken)ambiguousIdentifierExpressionNode.Token,
				(CSharpParserModel)model);
			
			return variableReferenceNode;
		}
		
		// Bind an undefined-TypeClauseNode
		if (ambiguousIdentifierExpressionNode.Token.SyntaxKind == SyntaxKind.IdentifierToken ||
			UtilityApi.IsTypeIdentifierKeywordSyntaxKind(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
		{
            var typeClauseNode = new TypeClauseNode(
				(ISyntaxToken)ambiguousIdentifierExpressionNode.Token,
				valueType: null,
				genericParametersListingNode: null);
			
			BindTypeClauseNode(
		        typeClauseNode,
		        (CSharpParserModel)model);
		        
		    return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), EmptyExpressionNode.Empty, typeClauseNode);
		}
		
		return ambiguousIdentifierExpressionNode;
	}
		
	public IExpressionNode BadMergeToken(
		BadExpressionNode badExpressionNode, ISyntaxToken token, IParserModel model)
	{
		badExpressionNode.SyntaxList.Add(token);
		return badExpressionNode;
	}

	public IExpressionNode BadMergeExpression(
		BadExpressionNode badExpressionNode, IExpressionNode expressionSecondary, IParserModel model)
	{
		badExpressionNode.SyntaxList.Add(expressionSecondary);
		return badExpressionNode;
	}

	public IExpressionNode BinaryMergeToken(
		BinaryExpressionNode binaryExpressionNode, ISyntaxToken token, IParserModel model)
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
		ConstructorInvocationExpressionNode constructorInvocationExpressionNode, ISyntaxToken token, IParserModel model)
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
						
						BindTypeClauseNode(
					        typeClauseNode,
					        (CSharpParserModel)model);
						
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
				
				constructorInvocationExpressionNode.ConstructorInvocationStageKind = ConstructorInvocationStageKind.FunctionParameters;
				model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, constructorInvocationExpressionNode));
				model.ExpressionList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode.FunctionParametersListingNode));
				return EmptyExpressionNode.Empty;
			case SyntaxKind.CloseParenthesisToken:
				if (constructorInvocationExpressionNode.FunctionParametersListingNode is not null)
				{
					constructorInvocationExpressionNode.FunctionParametersListingNode.SetCloseParenthesisToken((CloseParenthesisToken)token);
					return constructorInvocationExpressionNode;
				}
				else
				{
					goto default;
				}
			case SyntaxKind.OpenAngleBracketToken:
				if (constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode is null)
				{
					constructorInvocationExpressionNode.ResultTypeClauseNode.SetGenericParametersListingNode(
						new GenericParametersListingNode(
							(OpenAngleBracketToken)token,
					        new List<GenericParameterEntryNode>(),
					        closeAngleBracketToken: default));
				}
				
				constructorInvocationExpressionNode.ConstructorInvocationStageKind = ConstructorInvocationStageKind.GenericParameters;
			    model.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, constructorInvocationExpressionNode));
				model.ExpressionList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode));
				return EmptyExpressionNode.Empty;
			case SyntaxKind.CloseAngleBracketToken:
				constructorInvocationExpressionNode.ConstructorInvocationStageKind = ConstructorInvocationStageKind.Unset;
				constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode.SetCloseAngleBracketToken((CloseAngleBracketToken)token);
				return constructorInvocationExpressionNode;
			case SyntaxKind.OpenBraceToken:
				var objectInitializationParametersListingNode = new ObjectInitializationParametersListingNode(
					(OpenBraceToken)token,
			        new List<ObjectInitializationParameterEntryNode>(),
			        closeBraceToken: default);
			        
			    constructorInvocationExpressionNode.SetObjectInitializationParametersListingNode(objectInitializationParametersListingNode);
				
				constructorInvocationExpressionNode.ConstructorInvocationStageKind = ConstructorInvocationStageKind.ObjectInitializationParameters;
				model.ExpressionList.Add((SyntaxKind.CloseBraceToken, constructorInvocationExpressionNode));
				model.ExpressionList.Add((SyntaxKind.EqualsToken, constructorInvocationExpressionNode));
				model.ExpressionList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode));
				return EmptyExpressionNode.Empty;
			case SyntaxKind.CloseBraceToken:
				constructorInvocationExpressionNode.ConstructorInvocationStageKind = ConstructorInvocationStageKind.Unset;
				
				if (constructorInvocationExpressionNode.ObjectInitializationParametersListingNode is not null)
				{
					constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.SetCloseBraceToken((CloseBraceToken)token);
					return constructorInvocationExpressionNode;
				}
				else
				{
					goto default;
				}
			case SyntaxKind.EqualsToken:
				model.ExpressionList.Add((SyntaxKind.EqualsToken, constructorInvocationExpressionNode));
				model.ExpressionList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode));
				
				if (constructorInvocationExpressionNode.ConstructorInvocationStageKind == ConstructorInvocationStageKind.ObjectInitializationParameters &&
					constructorInvocationExpressionNode.ObjectInitializationParametersListingNode is not null)
				{
					if (constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList.Count > 0)
					{
						var lastParameter = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList.Last();
						
						if (!lastParameter.EqualsToken.ConstructorWasInvoked)
						{
							lastParameter.EqualsToken = (EqualsToken)token;
							return EmptyExpressionNode.Empty;
						}
					}
				}
				
				goto default;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), constructorInvocationExpressionNode, token);
		}
	}
	
	public IExpressionNode ConstructorInvocationMergeExpression(
		ConstructorInvocationExpressionNode constructorInvocationExpressionNode, IExpressionNode expressionSecondary, IParserModel model)
	{
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			var forceTypeClauseNode = false;
			
			if (constructorInvocationExpressionNode.ConstructorInvocationStageKind == ConstructorInvocationStageKind.GenericParameters)
			{
				forceTypeClauseNode = true;
			}
			
			expressionSecondary = ForceDecisionAmbiguousIdentifier(
				constructorInvocationExpressionNode,
				(AmbiguousIdentifierExpressionNode)expressionSecondary,
				model,
				forceTypeClauseNode: forceTypeClauseNode);
		}
	
		if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
		{
			return constructorInvocationExpressionNode;
		}
		else if (constructorInvocationExpressionNode.ConstructorInvocationStageKind == ConstructorInvocationStageKind.GenericParameters &&
				 constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode is not null)
		{
			return constructorInvocationExpressionNode;
		}
		else if (constructorInvocationExpressionNode.ConstructorInvocationStageKind == ConstructorInvocationStageKind.FunctionParameters &&
				 constructorInvocationExpressionNode.FunctionParametersListingNode is not null)
		{
			return constructorInvocationExpressionNode;
		}
		else if (constructorInvocationExpressionNode.ConstructorInvocationStageKind == ConstructorInvocationStageKind.ObjectInitializationParameters &&
				 constructorInvocationExpressionNode.ObjectInitializationParametersListingNode is not null)
		{
			var needNewNode = true;
			ObjectInitializationParameterEntryNode? objectInitializationParameterEntryNode = null;
			
			if (constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList.Count > 0)
			{
				objectInitializationParameterEntryNode = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList.Last();
				
				if (!objectInitializationParameterEntryNode.PropertyIdentifierToken.ConstructorWasInvoked ||
					(objectInitializationParameterEntryNode.ExpressionNode.SyntaxKind == SyntaxKind.EmptyExpressionNode))
				{
					needNewNode = false;
				}
			}
			
			if (needNewNode)
			{
				objectInitializationParameterEntryNode = new ObjectInitializationParameterEntryNode(
			        propertyIdentifierToken: default,
			        equalsToken: default,
			        expressionNode: EmptyExpressionNode.Empty);
			    
			    constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList.Add(objectInitializationParameterEntryNode);
			}

			var success = true;
			
			// I'm tired and feel like I'm about to pass out.
			// This feels like hacky nonsense.
			// It allows for ObjectInitialization and CollectionInitialization
			// to use the same node but why not just use separate nodes?
			var currentTokenIsComma = model.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken;
			var currentTokenIsBrace = model.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken;

			if (!objectInitializationParameterEntryNode.PropertyIdentifierToken.ConstructorWasInvoked &&
				(!currentTokenIsComma && !currentTokenIsBrace))
			{
				if (expressionSecondary.SyntaxKind != SyntaxKind.VariableReferenceNode &&
					expressionSecondary.SyntaxKind != SyntaxKind.AmbiguousIdentifierExpressionNode)
				{
					success = false;
				}
				else if (expressionSecondary.SyntaxKind == SyntaxKind.VariableReferenceNode)
				{
					objectInitializationParameterEntryNode.PropertyIdentifierToken = ((VariableReferenceNode)expressionSecondary).VariableIdentifierToken;
				}
				else if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
				{
					var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)expressionSecondary;
					if (ambiguousIdentifierExpressionNode.Token.SyntaxKind == SyntaxKind.IdentifierToken)
					{
						objectInitializationParameterEntryNode.PropertyIdentifierToken = (IdentifierToken)ambiguousIdentifierExpressionNode.Token;
					}
					else
					{
						success = false;
					}
				}
				else
				{
					success = false;
				}
			}
			else if (!objectInitializationParameterEntryNode.EqualsToken.ConstructorWasInvoked &&
					 (!currentTokenIsComma && !currentTokenIsBrace))
			{
				success = false;
			}
			else if (objectInitializationParameterEntryNode.ExpressionNode.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			{
				objectInitializationParameterEntryNode.ExpressionNode = expressionSecondary;
				
				if (!objectInitializationParameterEntryNode.EqualsToken.ConstructorWasInvoked && (currentTokenIsComma || currentTokenIsBrace))
				{
					var nextObjectInitializationParameterEntryNode = new ObjectInitializationParameterEntryNode(
				        propertyIdentifierToken: default,
				        equalsToken: default,
				        expressionNode: EmptyExpressionNode.Empty);
				    
				    constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList.Add(nextObjectInitializationParameterEntryNode);
				}
			}
			else
			{
				success = false;
			}
			
			if (success)
			{
				return constructorInvocationExpressionNode;
			}
			else
			{
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), constructorInvocationExpressionNode, expressionSecondary);
			}
		}
		
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), constructorInvocationExpressionNode, expressionSecondary);
	}
	
	public IExpressionNode EmptyMergeToken(
		EmptyExpressionNode emptyExpressionNode, ISyntaxToken token, IParserModel model)
	{
		if (token.SyntaxKind == SyntaxKind.IdentifierToken ||
			UtilityApi.IsTypeIdentifierKeywordSyntaxKind(token.SyntaxKind))
		{
			var ambiguousExpressionNode = new AmbiguousIdentifierExpressionNode(
				token,
		        genericParametersListingNode: null,
		        CSharpFacts.Types.Void.ToTypeClause())
		    {
		    	FollowsMemberAccessToken = emptyExpressionNode.FollowsMemberAccessToken
		    };
		    
		    if (model.TokenWalker.Next.SyntaxKind == SyntaxKind.StatementDelimiterToken &&
		    	!ambiguousExpressionNode.FollowsMemberAccessToken)
		    {
				ForceDecisionAmbiguousIdentifier(
					emptyExpressionNode,
					ambiguousExpressionNode,
					model);
		    }
		    
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
				model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, parenthesizedExpressionNode));
				model.ExpressionList.Add((SyntaxKind.CommaToken, parenthesizedExpressionNode));
				return EmptyExpressionNode.Empty;
			case SyntaxKind.NewTokenKeyword:
				return new ConstructorInvocationExpressionNode(
					(KeywordToken)token,
			        typeClauseNode: null,
			        functionParametersListingNode: null,
			        objectInitializationParametersListingNode: null);
			case SyntaxKind.AsyncTokenContextualKeyword:
				return new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
			case SyntaxKind.DollarSignToken:
				BindStringInterpolationExpression((DollarSignToken)token, (CSharpParserModel)model);
				return emptyExpressionNode;
			case SyntaxKind.AtToken:
				BindStringVerbatimExpression((AtToken)token, (CSharpParserModel)model);
				return emptyExpressionNode;
			case SyntaxKind.OutTokenKeyword:
				if (ParseTypes.IsPossibleTypeClause(model.TokenWalker.Next, (CSharpParserModel)model))
				{
					// The code in this block is very confusing due to the Consume() and Backtrack() usages.
					//
					// The issue is that the statement loop (the main parser loop) invokes
					// 'Consume()' at the start of the while loop.
					//
					// The expression loop (the secondary parser loop) invokes
					// 'Consume()' at the end of the while loop.
					//
					// TODO: The statement loop (the main parser loop) needs to invoke 'Consume()' at the end of the while loop...
					//       ...(or vice versa, whichever is deemed a better fit, just be consistent in both loops).
				
					_ = model.TokenWalker.Consume();
					var typeClauseNode = ParseTypes.MatchTypeClause((CSharpParserModel)model);
					
					var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
					
					// Guaranteed to consume 1 further than the secondary loop so have to backtrack 1 time as well.
					_ = model.TokenWalker.Backtrack();
					
					_ = ParseVariables.HandleVariableDeclarationExpression(
				        typeClauseNode,
				        identifierToken,
				        VariableKind.Local,
				        model);
				}
				
				return emptyExpressionNode;
			case SyntaxKind.InTokenKeyword:
			case SyntaxKind.RefTokenKeyword:
				return emptyExpressionNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), emptyExpressionNode, token);
		}
	}
	
	public IExpressionNode ExplicitCastMergeToken(
		ExplicitCastNode explicitCastNode, ISyntaxToken token, IParserModel model)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CloseParenthesisToken:
				return explicitCastNode.SetCloseParenthesisToken((CloseParenthesisToken)token);
			case SyntaxKind.IdentifierToken:
				var ambiguousExpressionNode = new AmbiguousIdentifierExpressionNode(
					token,
			        genericParametersListingNode: null,
			        CSharpFacts.Types.Void.ToTypeClause());
			    return ambiguousExpressionNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), explicitCastNode, token);
		}
	}
	
	public IExpressionNode GenericParametersListingMergeToken(
		GenericParametersListingNode genericParametersListingNode, ISyntaxToken token, IParserModel model)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CommaToken:
				model.ExpressionList.Add((SyntaxKind.CommaToken, genericParametersListingNode));
				return EmptyExpressionNode.Empty;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), genericParametersListingNode, token);
		}
	}
	
	public IExpressionNode GenericParametersListingMergeExpression(
		GenericParametersListingNode genericParametersListingNode, IExpressionNode expressionSecondary, IParserModel model)
	{
		if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			return genericParametersListingNode;
	
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			var expressionSecondaryTyped = (AmbiguousIdentifierExpressionNode)expressionSecondary;
			
			var typeClauseNode = new TypeClauseNode(
				expressionSecondaryTyped.Token,
		        valueType: null,
		        genericParametersListingNode: null);
			
			BindTypeClauseNode(
		        typeClauseNode,
		        (CSharpParserModel)model);
			
			genericParametersListingNode.GenericParameterEntryNodeList.Add(
				new GenericParameterEntryNode(typeClauseNode));
			
			return genericParametersListingNode;
		}
		else if (expressionSecondary.SyntaxKind == SyntaxKind.BadExpressionNode)
		{
			var badExpressionNode = (BadExpressionNode)expressionSecondary;
		
			if (badExpressionNode.SyntaxList.Count == 2 &&
					(badExpressionNode.SyntaxList[1].SyntaxKind == SyntaxKind.TypeClauseNode ||
					 UtilityApi.IsTypeIdentifierKeywordSyntaxKind(badExpressionNode.SyntaxList[1].SyntaxKind)))
			{
				var typeClauseNode = (TypeClauseNode)badExpressionNode.SyntaxList[1];
				
				BindTypeClauseNode(
			        typeClauseNode,
			        (CSharpParserModel)model);
			    
				genericParametersListingNode.GenericParameterEntryNodeList.Add(
					new GenericParameterEntryNode(typeClauseNode));
				
				return genericParametersListingNode;
			}
		}
		
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), genericParametersListingNode, expressionSecondary);
	}
	
	public IExpressionNode FunctionParametersListingMergeToken(
		FunctionParametersListingNode functionParametersListingNode, ISyntaxToken token, IParserModel model)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CommaToken:
				model.ExpressionList.Add((SyntaxKind.CommaToken, functionParametersListingNode));
				return EmptyExpressionNode.Empty;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), functionParametersListingNode, token);
		}
	}
	
	public IExpressionNode FunctionParametersListingMergeExpression(
		FunctionParametersListingNode functionParametersListingNode, IExpressionNode expressionSecondary, IParserModel model)
	{
		if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			return functionParametersListingNode;
			
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionSecondary = ForceDecisionAmbiguousIdentifier(
				functionParametersListingNode,
				(AmbiguousIdentifierExpressionNode)expressionSecondary,
				model);
		}
			
		var functionParameterEntryNode = new FunctionParameterEntryNode(
	        expressionSecondary,
	        hasOutKeyword: false,
	        hasInKeyword: false,
	        hasRefKeyword: false);
	        
		functionParametersListingNode.FunctionParameterEntryNodeList.Add(functionParameterEntryNode);
		
		return functionParametersListingNode;
	}
	
	public IExpressionNode LambdaMergeToken(
		LambdaExpressionNode lambdaExpressionNode, ISyntaxToken token, IParserModel model)
	{
		if (token.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
		{
			int startInclusiveIndex;
			
			if (model.TokenWalker.Previous.SyntaxKind == SyntaxKind.EqualsToken)
				startInclusiveIndex = model.TokenWalker.Previous.TextSpan.StartingIndexInclusive;
			else
				startInclusiveIndex = token.TextSpan.StartingIndexInclusive;
			
			var endExclusiveIndex = token.TextSpan.EndingIndexExclusive;
			
			var textSpan = new TextEditorTextSpan(
				startInclusiveIndex,
			    endExclusiveIndex,
			    (byte)GenericDecorationKind.None,
			    token.TextSpan.ResourceUri,
			    token.TextSpan.SourceText);
		
			((CSharpBinder)model.Binder).AddSymbolDefinition(
				new LambdaSymbol(textSpan, lambdaExpressionNode), (CSharpParserModel)model);
		
			if (model.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenBraceToken)
			{
				lambdaExpressionNode.CodeBlockNodeIsExpression = false;
			
				model.ExpressionList.Add((SyntaxKind.CloseBraceToken, lambdaExpressionNode));
				model.ExpressionList.Add((SyntaxKind.StatementDelimiterToken, lambdaExpressionNode));
				return EmptyExpressionNode.Empty;
			}
			
			model.ExpressionList.Add((SyntaxKind.StatementDelimiterToken, lambdaExpressionNode));
			return EmptyExpressionNode.Empty;
		}
		else if (token.SyntaxKind == SyntaxKind.StatementDelimiterToken)
		{
			if (lambdaExpressionNode.CodeBlockNodeIsExpression)
			{
				return lambdaExpressionNode;
			}
			else
			{
				model.ExpressionList.Add((SyntaxKind.StatementDelimiterToken, lambdaExpressionNode));
				return EmptyExpressionNode.Empty;
			}
		}
		else if (token.SyntaxKind == SyntaxKind.CloseBraceToken)
		{
			if (lambdaExpressionNode.CodeBlockNodeIsExpression)
			{
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), lambdaExpressionNode, token);
			}
			else
			{
				return lambdaExpressionNode;
			}
		}
		else if (token.SyntaxKind == SyntaxKind.OpenParenthesisToken)
		{
			if (lambdaExpressionNode.HasReadParameters)
			{
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), lambdaExpressionNode, token);
			}
			else
			{
				lambdaExpressionNode.HasReadParameters = true;
				model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, lambdaExpressionNode));
				model.ExpressionList.Add((SyntaxKind.CommaToken, lambdaExpressionNode));
				return EmptyExpressionNode.Empty;
			}
		}
		else if (token.SyntaxKind == SyntaxKind.CloseParenthesisToken)
		{
			return lambdaExpressionNode;
		}
		else if (token.SyntaxKind == SyntaxKind.EqualsToken)
		{
			if (model.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
				return lambdaExpressionNode;
			
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), lambdaExpressionNode, token);
		}
		else if (token.SyntaxKind == SyntaxKind.CommaToken)
		{
			model.ExpressionList.Add((SyntaxKind.CommaToken, lambdaExpressionNode));
			return EmptyExpressionNode.Empty;
		}
		else if (token.SyntaxKind == SyntaxKind.IdentifierToken)
		{
			if (lambdaExpressionNode.HasReadParameters)
			{
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), lambdaExpressionNode, token);
			}
			else
			{
				lambdaExpressionNode.HasReadParameters = true;
				return lambdaExpressionNode;
			}
		}
		else
		{
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), lambdaExpressionNode, token);
		}
	}
	
	public IExpressionNode LambdaMergeExpression(
		LambdaExpressionNode lambdaExpressionNode, IExpressionNode expressionSecondary, IParserModel model)
	{
		switch (expressionSecondary.SyntaxKind)
		{
			default:
				return lambdaExpressionNode;
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
		LiteralExpressionNode literalExpressionNode, ISyntaxToken token, IParserModel model)
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
		ParenthesizedExpressionNode parenthesizedExpressionNode, ISyntaxToken token, IParserModel model)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CloseParenthesisToken:
				return parenthesizedExpressionNode.SetCloseParenthesisToken((CloseParenthesisToken)token);
			case SyntaxKind.EqualsToken:
				if (model.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
				{
					var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
					SetLambdaExpressionNodeVariableDeclarationNodeList(lambdaExpressionNode, parenthesizedExpressionNode.InnerExpression, model);
					return lambdaExpressionNode;
				}
				
				return parenthesizedExpressionNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), parenthesizedExpressionNode, token);
		}
	}
	
	public IExpressionNode ParenthesizedMergeExpression(
		ParenthesizedExpressionNode parenthesizedExpressionNode, IExpressionNode expressionSecondary, IParserModel model)
	{
		if (model.TokenWalker.Peek(1).SyntaxKind == SyntaxKind.EqualsToken &&
			model.TokenWalker.Peek(2).SyntaxKind == SyntaxKind.CloseAngleBracketToken)
		{
			var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
			return SetLambdaExpressionNodeVariableDeclarationNodeList(lambdaExpressionNode, expressionSecondary, model);
		}
	
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
			expressionSecondary = ForceDecisionAmbiguousIdentifier(parenthesizedExpressionNode, (AmbiguousIdentifierExpressionNode)expressionSecondary, model);
	
		if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
		{
			var commaSeparatedExpressionNode = new CommaSeparatedExpressionNode();
			commaSeparatedExpressionNode.AddInnerExpressionNode(parenthesizedExpressionNode);
			return commaSeparatedExpressionNode; 
		}
	
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
				
				BindTypeClauseNode(
			        typeClauseNode,
			        (CSharpParserModel)model);
			    
				return new ExplicitCastNode(parenthesizedExpressionNode.OpenParenthesisToken, typeClauseNode);
			}
			else if (badExpressionNode.SyntaxList[1].SyntaxKind == SyntaxKind.TypeClauseNode)
			{
				var typeClauseNode = (TypeClauseNode)badExpressionNode.SyntaxList[1];
				return new ExplicitCastNode(parenthesizedExpressionNode.OpenParenthesisToken, typeClauseNode);
			}
		}
		else if (expressionSecondary.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			 var variableReferenceNode = (VariableReferenceNode)expressionSecondary;
			 
			 if (variableReferenceNode.IsFabricated)
			 {
			 	var typeClauseNode = new TypeClauseNode(variableReferenceNode.VariableIdentifierToken, valueType: null, genericParametersListingNode: null);
				
				BindTypeClauseNode(
			        typeClauseNode,
			        (CSharpParserModel)model);
			        
				return new ExplicitCastNode(parenthesizedExpressionNode.OpenParenthesisToken, typeClauseNode);
			 }
		}

		return parenthesizedExpressionNode.SetInnerExpression(expressionSecondary);
	}
	
	public IExpressionNode FunctionInvocationMergeToken(
		FunctionInvocationNode functionInvocationNode, ISyntaxToken token, IParserModel model)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CloseParenthesisToken:
				functionInvocationNode.FunctionParametersListingNode.SetCloseParenthesisToken((CloseParenthesisToken)token);
				return functionInvocationNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), functionInvocationNode, token);
		}
	}
	
	public IExpressionNode FunctionInvocationMergeExpression(
		FunctionInvocationNode functionInvocationNode, IExpressionNode expressionSecondary, IParserModel model)
	{
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionSecondary = ForceDecisionAmbiguousIdentifier(functionInvocationNode, (AmbiguousIdentifierExpressionNode)expressionSecondary, model);
		}
	
		switch (expressionSecondary.SyntaxKind)
		{
			case SyntaxKind.EmptyExpressionNode:
				return functionInvocationNode;
			case SyntaxKind.FunctionParametersListingNode:
				return functionInvocationNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), functionInvocationNode, expressionSecondary);
		}
	}
	
	public IExpressionNode SetLambdaExpressionNodeVariableDeclarationNodeList(
		LambdaExpressionNode lambdaExpressionNode, IExpressionNode expressionNode, IParserModel model)
	{
		if (expressionNode.SyntaxKind == SyntaxKind.BadExpressionNode)
		{
			var badExpressionNode = (BadExpressionNode)expressionNode;
		
			if (badExpressionNode.SyntaxList.Count == 2 &&
	    		badExpressionNode.SyntaxList[0].SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode &&
	    		badExpressionNode.SyntaxList[1].SyntaxKind == SyntaxKind.IdentifierToken)
	    	{
	    		var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)badExpressionNode.SyntaxList[0];
	    		var typeClauseNode = new TypeClauseNode(ambiguousIdentifierExpressionNode.Token, valueType: null, genericParametersListingNode: null);
					
				BindTypeClauseNode(
			        typeClauseNode,
			        (CSharpParserModel)model);
	    		
	    		var identifierToken = (IdentifierToken)badExpressionNode.SyntaxList[1];
	    		
	    		var variableDeclarationNode = ParseVariables.HandleVariableDeclarationExpression(
			        typeClauseNode,
			        identifierToken,
			        VariableKind.Local,
			        model);
			        
	    		lambdaExpressionNode.AddVariableDeclarationNode(variableDeclarationNode);
	    	}
	    	else
	    	{
	    		var typeClauseNode = TypeFacts.Empty.ToTypeClause();
	    		ISyntaxToken variableIdentifier = default(IdentifierToken);
	    	
	    		for (int i = 0; i < badExpressionNode.SyntaxList.Count; i++)
	    		{
	    			var firstSyntax = badExpressionNode.SyntaxList[i];
	    			
	    			var wasTyped = false;
	    			
	    			if (firstSyntax.SyntaxKind != SyntaxKind.AmbiguousIdentifierExpressionNode &&
	    				firstSyntax.SyntaxKind != SyntaxKind.IdentifierToken)
	    			{
	    				continue;
	    			}
	    			
	    			if (i < badExpressionNode.SyntaxList.Count - 1 &&
		    				(badExpressionNode.SyntaxList[i + 1].SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode ||
		    				badExpressionNode.SyntaxList[i + 1].SyntaxKind == SyntaxKind.IdentifierToken))
	    			{
    					wasTyped = true;
    					
    					if (firstSyntax.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
    					{
    						var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)firstSyntax;
    						typeClauseNode = new TypeClauseNode(ambiguousIdentifierExpressionNode.Token, valueType: null, genericParametersListingNode: null);
					
							BindTypeClauseNode(
						        typeClauseNode,
						        (CSharpParserModel)model);
    					}
    					else if (firstSyntax.SyntaxKind == SyntaxKind.IdentifierToken)
    					{
    						var identifierToken = (IdentifierToken)firstSyntax;
    						typeClauseNode = new TypeClauseNode(identifierToken, valueType: null, genericParametersListingNode: null);
					
							BindTypeClauseNode(
						        typeClauseNode,
						        (CSharpParserModel)model);
    					}
    					
    					var secondSyntax = badExpressionNode.SyntaxList[++i];
    					
    					if (secondSyntax.SyntaxKind == SyntaxKind.IdentifierToken)
    					{
    						variableIdentifier = (IdentifierToken)secondSyntax;
    					}
    					else if (secondSyntax.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
    					{
    						var token = ((AmbiguousIdentifierExpressionNode)secondSyntax).Token;
		
				    		if (token.SyntaxKind != SyntaxKind.IdentifierToken)
				    			continue;
    					}
	    			}
	    			
	    			if (!wasTyped)
	    			{
	    				typeClauseNode = TypeFacts.Empty.ToTypeClause();
	    				
	    				if (firstSyntax.SyntaxKind == SyntaxKind.IdentifierToken)
    					{
    						variableIdentifier = (IdentifierToken)firstSyntax;
    					}
    					else if (firstSyntax.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
    					{
    						var token = ((AmbiguousIdentifierExpressionNode)firstSyntax).Token;
		
				    		if (token.SyntaxKind != SyntaxKind.IdentifierToken)
				    			continue;
    					}
	    			}
	    			
	    			if (variableIdentifier.SyntaxKind != SyntaxKind.IdentifierToken)
				    	continue;
	    			
	    			var variableDeclarationNode = ParseVariables.HandleVariableDeclarationExpression(
				        typeClauseNode,
				        (IdentifierToken)variableIdentifier,
				        VariableKind.Local,
				        model);
				        
		    		lambdaExpressionNode.AddVariableDeclarationNode(variableDeclarationNode);
	    		}
	    	}
    	}
    	else if (expressionNode.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
    	{
    		var token = ((AmbiguousIdentifierExpressionNode)expressionNode).Token;
    		
    		if (token.SyntaxKind != SyntaxKind.IdentifierToken)
    			return lambdaExpressionNode;
    	
    		var variableDeclarationNode = ParseVariables.HandleVariableDeclarationExpression(
		        TypeFacts.Empty.ToTypeClause(),
		        (IdentifierToken)token,
		        VariableKind.Local,
		        model);
		        
    		lambdaExpressionNode.AddVariableDeclarationNode(variableDeclarationNode);
    	}
    	
    	return lambdaExpressionNode;
	}
}
