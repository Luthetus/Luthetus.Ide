using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
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
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.BinderCase;

public partial class CSharpBinder
{
	/// <summary>
	/// Returns the new primary expression which will be 'BadExpressionNode'
	/// if the parameters were not mergeable.
	/// </summary>
	public IExpressionNode AnyMergeToken(
		IExpressionNode expressionPrimary, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		#if DEBUG
		Console.WriteLine($"{expressionPrimary.SyntaxKind} + {token.SyntaxKind}");
		#endif
		
		if (parserModel.ParserContextKind != CSharpParserContextKind.ForceParseGenericParameters &&
			UtilityApi.IsBinaryOperatorSyntaxKind(token.SyntaxKind))
		{
			return HandleBinaryOperator(expressionPrimary, token, compilationUnit, ref parserModel);
		}
		
		switch (expressionPrimary.SyntaxKind)
		{
			case SyntaxKind.EmptyExpressionNode:
				return EmptyMergeToken((EmptyExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.LiteralExpressionNode:
				return LiteralMergeToken((LiteralExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.BinaryExpressionNode:
				return BinaryMergeToken((BinaryExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.ParenthesizedExpressionNode:
				return ParenthesizedMergeToken((ParenthesizedExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			//case SyntaxKind.CommaSeparatedExpressionNode:
			//	return CommaSeparatedMergeToken((CommaSeparatedExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.FunctionInvocationNode:
				return FunctionInvocationMergeToken((FunctionInvocationNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.LambdaExpressionNode:
				return LambdaMergeToken((LambdaExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.ConstructorInvocationExpressionNode:
				return ConstructorInvocationMergeToken((ConstructorInvocationExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.ExplicitCastNode:
				return ExplicitCastMergeToken((ExplicitCastNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.TupleExpressionNode:
				return TupleMergeToken((TupleExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.AmbiguousParenthesizedExpressionNode:
				return AmbiguousParenthesizedMergeToken((AmbiguousParenthesizedExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.AmbiguousIdentifierExpressionNode:
				return AmbiguousIdentifierMergeToken((AmbiguousIdentifierExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.TypeClauseNode:
				return TypeClauseMergeToken((TypeClauseNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.VariableAssignmentExpressionNode:
				return VariableAssignmentMergeToken((VariableAssignmentExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.GenericParametersListingNode:
				return GenericParametersListingMergeToken((GenericParametersListingNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.FunctionParametersListingNode:
				return FunctionParametersListingMergeToken((FunctionParametersListingNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.FunctionArgumentsListingNode:
				return FunctionArgumentsListingMergeToken((FunctionArgumentsListingNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.ReturnStatementNode:
				return ReturnStatementMergeToken((ReturnStatementNode)expressionPrimary, token, compilationUnit, ref parserModel);
			case SyntaxKind.BadExpressionNode:
				return BadMergeToken((BadExpressionNode)expressionPrimary, token, compilationUnit, ref parserModel);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), expressionPrimary, token);
		};
	}
	
	/// <summary>
	/// Returns the new primary expression which will be 'BadExpressionNode'
	/// if the parameters were not mergeable.
	/// </summary>
	public IExpressionNode AnyMergeExpression(
		IExpressionNode expressionPrimary, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		#if DEBUG
		Console.WriteLine($"{expressionPrimary.SyntaxKind} + {expressionSecondary.SyntaxKind}");
		#endif
	
		switch (expressionPrimary.SyntaxKind)
		{
			case SyntaxKind.BinaryExpressionNode:
				return BinaryMergeExpression((BinaryExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.ParenthesizedExpressionNode:
				return ParenthesizedMergeExpression((ParenthesizedExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			//case SyntaxKind.CommaSeparatedExpressionNode:
			//	return CommaSeparatedMergeExpression((CommaSeparatedExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.FunctionInvocationNode:
				return FunctionInvocationMergeExpression((FunctionInvocationNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.LambdaExpressionNode:
				return LambdaMergeExpression((LambdaExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.ConstructorInvocationExpressionNode:
				return ConstructorInvocationMergeExpression((ConstructorInvocationExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.TupleExpressionNode:
				return TupleMergeExpression((TupleExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.AmbiguousParenthesizedExpressionNode:
				return AmbiguousParenthesizedMergeExpression((AmbiguousParenthesizedExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.AmbiguousIdentifierExpressionNode:
				return AmbiguousIdentifierMergeExpression((AmbiguousIdentifierExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.TypeClauseNode:
				return TypeClauseMergeExpression((TypeClauseNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.VariableAssignmentExpressionNode:
				return VariableAssignmentMergeExpression((VariableAssignmentExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.GenericParametersListingNode:
				return GenericParametersListingMergeExpression((GenericParametersListingNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.FunctionParametersListingNode:
				return FunctionParametersListingMergeExpression((FunctionParametersListingNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.FunctionArgumentsListingNode:
				return FunctionArgumentsListingMergeExpression((FunctionArgumentsListingNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.ReturnStatementNode:
				return ReturnStatementMergeExpression((ReturnStatementNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.BadExpressionNode:
				return BadMergeExpression((BadExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), expressionPrimary, expressionSecondary);
		};
	}
	
	public IExpressionNode HandleBinaryOperator(
		IExpressionNode expressionPrimary, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (token.SyntaxKind == SyntaxKind.MemberAccessToken)
			return ParseMemberAccessToken(expressionPrimary, token, compilationUnit, ref parserModel);
	
		// In order to disambiguate '<' between when the 'expressionPrimary' is an 'AmbiguousIdentifierExpressionNode'
		//     - Less than operator
		//     - GenericParametersListingNode
		// 
		// Invoke 'ForceDecisionAmbiguousIdentifier(...)' to determine what the true SyntaxKind is.
		//
		// If its true SyntaxKind is a TypeClauseNode, then parse the '<'
		// to be the start of a 'GenericParametersListingNode'.
		//
		// Otherwise presume that the '<' is the 'less than operator'.
		if (expressionPrimary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionPrimary = ForceDecisionAmbiguousIdentifier(
				EmptyExpressionNode.Empty,
				(AmbiguousIdentifierExpressionNode)expressionPrimary,
				compilationUnit,
				ref parserModel);
		}
		
		if (expressionPrimary.SyntaxKind == SyntaxKind.TypeClauseNode &&
			(token.SyntaxKind == SyntaxKind.OpenAngleBracketToken || token.SyntaxKind == SyntaxKind.CloseAngleBracketToken))
		{
			return TypeClauseMergeToken((TypeClauseNode)expressionPrimary, token, compilationUnit, ref parserModel);
		}
		
		/*if (expressionPrimary.SyntaxKind == SyntaxKind.VariableReferenceNode &&
			token.SyntaxKind == SyntaxKind.EqualsToken)
		{
			return VariableA
		}*/
	
		var parentExpressionNode = GetParentNode(expressionPrimary, compilationUnit, ref parserModel);
		
		if (parentExpressionNode.SyntaxKind == SyntaxKind.BinaryExpressionNode)
		{			
			var parentBinaryExpressionNode = (BinaryExpressionNode)parentExpressionNode;
			var precedenceParent = UtilityApi.GetOperatorPrecedence(parentBinaryExpressionNode.BinaryOperatorNode.OperatorToken.SyntaxKind);
			
			var precedenceChild = UtilityApi.GetOperatorPrecedence(token.SyntaxKind);
			
			if (parentBinaryExpressionNode.RightExpressionNode.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			{				
				if (precedenceParent >= precedenceChild)
				{
					// Child's left expression becomes the parent.
					
					parentBinaryExpressionNode.SetRightExpressionNode(expressionPrimary);
					
					var typeClauseNode = expressionPrimary.ResultTypeClauseNode;
					var binaryOperatorNode = new BinaryOperatorNode(typeClauseNode, token, typeClauseNode, typeClauseNode);
					var binaryExpressionNode = new BinaryExpressionNode(parentBinaryExpressionNode, binaryOperatorNode);
					
					ClearFromExpressionList(parentBinaryExpressionNode, compilationUnit, ref parserModel);
					parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, binaryExpressionNode));
					
					return EmptyExpressionNode.Empty;
				}
				else
				{
					// Parent's right expression becomes the child.
					
					var typeClauseNode = expressionPrimary.ResultTypeClauseNode;
					var binaryOperatorNode = new BinaryOperatorNode(typeClauseNode, token, typeClauseNode, typeClauseNode);
					var binaryExpressionNode = new BinaryExpressionNode(expressionPrimary, binaryOperatorNode);
					
					parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, binaryExpressionNode));
					
					//parentBinaryExpressionNode.SetRightExpressionNode(binaryExpressionNode);
					
					return EmptyExpressionNode.Empty;
				}
			}
			else
			{
				// Weird situation?
				// This sounds like it just wouldn't compile.
				//
				// Something like:
				//     1 + 2 3 + 4
				//
				// NOTE: There is no operator between the '2' and the '3'.
				//       It is just two binary expressions side by side.
				//
				// I think you'd want to pretend that the parent binary expression didn't exist
				// for the sake of parser recovery.
				
				var typeClauseNode = expressionPrimary.ResultTypeClauseNode;
				var binaryOperatorNode = new BinaryOperatorNode(typeClauseNode, token, typeClauseNode, typeClauseNode);
				var binaryExpressionNode = new BinaryExpressionNode(expressionPrimary, binaryOperatorNode);
				
				ClearFromExpressionList(parentBinaryExpressionNode, compilationUnit, ref parserModel);
				parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, binaryExpressionNode));
				return EmptyExpressionNode.Empty;
			}
		}
		
		// Scope to avoid variable name collision.
		{
			var typeClauseNode = expressionPrimary.ResultTypeClauseNode;
			var binaryOperatorNode = new BinaryOperatorNode(typeClauseNode, token, typeClauseNode, typeClauseNode);
			var binaryExpressionNode = new BinaryExpressionNode(expressionPrimary, binaryOperatorNode);
			parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, binaryExpressionNode));
			return EmptyExpressionNode.Empty;
		}
	}

	public IExpressionNode AmbiguousParenthesizedMergeToken(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (token.SyntaxKind == SyntaxKind.CommaToken)
		{
			if (ambiguousParenthesizedExpressionNode.IsParserContextKindForceStatementExpression)
				parserModel.ParserContextKind = CSharpParserContextKind.ForceStatementExpression;
		
			parserModel.ExpressionList.Add((SyntaxKind.CommaToken, ambiguousParenthesizedExpressionNode));
			return EmptyExpressionNode.Empty;
		}
		else if (token.SyntaxKind == SyntaxKind.CloseParenthesisToken)
		{
			if (ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes is null)
			{
				var parenthesizedExpressionNode = new ParenthesizedExpressionNode(
					ambiguousParenthesizedExpressionNode.OpenParenthesisToken,
					CSharpFacts.Types.Void.ToTypeClause());
					
				parserModel.NoLongerRelevantExpressionNode = ambiguousParenthesizedExpressionNode;
					
				return parenthesizedExpressionNode;
			}
			else if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
			{
				return AmbiguousParenthesizedExpressionTransformTo_LambdaExpressionNode(ambiguousParenthesizedExpressionNode, compilationUnit, ref parserModel);
			}
			else if (ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes.Value &&
					 ambiguousParenthesizedExpressionNode.NodeList.Count >= 1)
			{
				return AmbiguousParenthesizedExpressionTransformTo_TypeClauseNode(ambiguousParenthesizedExpressionNode, token, compilationUnit, ref parserModel);
			}
			else if (!ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes.Value)
			{
				if (ambiguousParenthesizedExpressionNode.NodeList.Count > 1)
				{
					if (ambiguousParenthesizedExpressionNode.IsParserContextKindForceStatementExpression ||
						ambiguousParenthesizedExpressionNode.NodeList.All(node => node.SyntaxKind == SyntaxKind.TypeClauseNode))
					{
						return AmbiguousParenthesizedExpressionTransformTo_TypeClauseNode(ambiguousParenthesizedExpressionNode, token, compilationUnit, ref parserModel);
					}
					
					return AmbiguousParenthesizedExpressionTransformTo_TupleExpressionNode(ambiguousParenthesizedExpressionNode, expressionSecondary: null, compilationUnit, ref parserModel);
				}
				else if (ambiguousParenthesizedExpressionNode.NodeList.Count == 1 &&
						 UtilityApi.IsConvertibleToTypeClauseNode(ambiguousParenthesizedExpressionNode.NodeList[0].SyntaxKind) ||
						 ambiguousParenthesizedExpressionNode.NodeList[0].SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode ||
						 ambiguousParenthesizedExpressionNode.NodeList[0].SyntaxKind == SyntaxKind.VariableReferenceNode)
				{
					return AmbiguousParenthesizedExpressionTransformTo_ExplicitCastNode(
						ambiguousParenthesizedExpressionNode, (IExpressionNode)ambiguousParenthesizedExpressionNode.NodeList[0], (CloseParenthesisToken)token, compilationUnit, ref parserModel);
				}
			}
		}
		
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), ambiguousParenthesizedExpressionNode, token);
	}
	
	public IExpressionNode AmbiguousParenthesizedMergeExpression(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (expressionSecondary.SyntaxKind)
		{
			case SyntaxKind.VariableDeclarationNode:
				if (ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes is null)
					ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes = true;
				if (!ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes.Value)
					break;
			
				if (ambiguousParenthesizedExpressionNode.IsParserContextKindForceStatementExpression)
					parserModel.ParserContextKind = CSharpParserContextKind.ForceStatementExpression;
				
				ambiguousParenthesizedExpressionNode.NodeList.Add(expressionSecondary);
				return ambiguousParenthesizedExpressionNode;
			case SyntaxKind.AmbiguousIdentifierExpressionNode:
			case SyntaxKind.TypeClauseNode:
			case SyntaxKind.VariableReferenceNode:
				if (ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes is null)
					ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes = false;
				if (ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes.Value)
					break;
			
				if (ambiguousParenthesizedExpressionNode.IsParserContextKindForceStatementExpression)
					parserModel.ParserContextKind = CSharpParserContextKind.ForceStatementExpression;
				
				ambiguousParenthesizedExpressionNode.NodeList.Add(expressionSecondary);
				return ambiguousParenthesizedExpressionNode;
			case SyntaxKind.AmbiguousParenthesizedExpressionNode:
				// The 'AmbiguousParenthesizedExpressionNode' merging with 'ISyntaxToken' method will
				// return the existing 'AmbiguousParenthesizedExpressionNode' in various situations.
				//
				// One of which is to signify the closing brace token.
				return ambiguousParenthesizedExpressionNode;
		}
		
		// 'ambiguousParenthesizedExpressionNode.NodeList.Count > 0' because the current was never added,
		// so if there already is 1, then there'd be many expressions.
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken || ambiguousParenthesizedExpressionNode.NodeList.Count > 0)
			return AmbiguousParenthesizedExpressionTransformTo_TupleExpressionNode(ambiguousParenthesizedExpressionNode, expressionSecondary, compilationUnit, ref parserModel);
		else
			return AmbiguousParenthesizedExpressionTransformTo_ParenthesizedExpressionNode(ambiguousParenthesizedExpressionNode, expressionSecondary, compilationUnit, ref parserModel);
	}
	
	public IExpressionNode AmbiguousIdentifierMergeToken(
		AmbiguousIdentifierExpressionNode ambiguousIdentifierExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
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
		        compilationUnit);
			
			return ParseFunctionParametersListingNode(
				functionInvocationNode, functionInvocationNode.FunctionParametersListingNode, compilationUnit, ref parserModel);
		}
		else if (token.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
		{
			// TODO: As of (2024-12-21) is this conditional branch no longer hit?...
			//       ...The 'AmbiguousIdentifierExpressionNode' merging with 'OpenAngleBracketToken'
			//       code was moved to 'HandleBinaryOperator(...)'
			return ParseAmbiguousIdentifierGenericParametersListingNode(
				ambiguousIdentifierExpressionNode, (OpenAngleBracketToken)token, compilationUnit, ref parserModel);
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
			// Thinking about: Variable Assignment, Optional Parameters, and other unknowns.
			if (UtilityApi.IsConvertibleToIdentifierToken(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
			{
				var variableAssignmentNode = new VariableAssignmentExpressionNode(
					UtilityApi.ConvertToIdentifierToken(ambiguousIdentifierExpressionNode.Token, compilationUnit, ref parserModel),
			        (EqualsToken)token,
			        EmptyExpressionNode.Empty);
			 
			    parserModel.ExpressionList.Add((SyntaxKind.CommaToken, variableAssignmentNode));
				parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, variableAssignmentNode));
				
				return EmptyExpressionNode.Empty;
			}
			else
			{
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, ambiguousIdentifierExpressionNode));
				return EmptyExpressionNode.Empty;
			}
		}
		else if (token.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
		{
			var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
			SetLambdaExpressionNodeVariableDeclarationNodeList(lambdaExpressionNode, ambiguousIdentifierExpressionNode, compilationUnit, ref parserModel);
			
			// If the lambda expression's code block is a single expression then there is no end delimiter.
			// Instead, it is the parent expression's delimiter that causes the lambda expression's code block to short circuit.
			// At this moment, the lambda expression is given whatever expression was able to be parsed and can take it as its "code block".
			// And then restore the parent expression as the expressionPrimary.
			//
			// -----------------------------------------------------------------------------------------------------------------------------
			//
			// If the lambda expression's code block is deliminated by braces
			// then the end delimiter is the CloseBraceToken.
			// But, we can only add a "short circuit" for 'CloseBraceToken and lambdaExpressionNode'
			// if we have seen the 'OpenBraceToken'.
			
			parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, lambdaExpressionNode));
			
			if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenBraceToken)
			{
				parserModel.ExpressionList.Add((SyntaxKind.CloseBraceToken, lambdaExpressionNode));
				OpenLambdaExpressionScope(lambdaExpressionNode, (OpenBraceToken)parserModel.TokenWalker.Next, compilationUnit, ref parserModel);
				SkipLambdaExpressionStatements(lambdaExpressionNode, compilationUnit, ref parserModel);
			}
			else
			{
				OpenLambdaExpressionScope(lambdaExpressionNode, new OpenBraceToken(token.TextSpan), compilationUnit, ref parserModel);
			}
			
			return EmptyExpressionNode.Empty;
		}
		else if (token.SyntaxKind == SyntaxKind.IsTokenKeyword)
		{
			ForceDecisionAmbiguousIdentifier(
				EmptyExpressionNode.Empty,
				ambiguousIdentifierExpressionNode,
				compilationUnit,
				ref parserModel);
				
			_ = parserModel.TokenWalker.Consume(); // Consume the IsTokenKeyword
			
			if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.NotTokenContextualKeyword)
				_ = parserModel.TokenWalker.Consume(); // Consume the NotTokenKeyword
				
			if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.NullTokenKeyword)
			{
				_ = parserModel.TokenWalker.Consume(); // Consume the NullTokenKeyword
			}
			
			return EmptyExpressionNode.Empty;
		}
		else if (token.SyntaxKind == SyntaxKind.WithTokenContextualKeyword)
		{
			ForceDecisionAmbiguousIdentifier(
				EmptyExpressionNode.Empty,
				ambiguousIdentifierExpressionNode,
				compilationUnit,
				ref parserModel);
				
			if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenBraceToken)
			{
				var withKeywordContextualToken = parserModel.TokenWalker.Consume();
			
				#if DEBUG
				parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
				#endif
				
				var openBraceToken = (OpenBraceToken)parserModel.TokenWalker.Consume();
		    	
		    	var openBraceCounter = 1;
				
				while (true)
				{
					if (parserModel.TokenWalker.IsEof)
						break;
		
					if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
					{
						++openBraceCounter;
					}
					else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
					{
						if (--openBraceCounter <= 0)
							break;
					}
		
					_ = parserModel.TokenWalker.Consume();
				}
		
				var closeBraceToken = (CloseBraceToken)parserModel.TokenWalker.Match(SyntaxKind.CloseBraceToken);
				
				#if DEBUG
				parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
				#endif
			}
			
			return new WithExpressionNode(
				variableIdentifierToken: default,
				openBraceToken: default,
				closeBraceToken: default,
				CSharpFacts.Types.Void.ToTypeClause());
		}
		else if (token.SyntaxKind == SyntaxKind.IdentifierToken)
		{
			var decidedExpression = ForceDecisionAmbiguousIdentifier(
				EmptyExpressionNode.Empty,
				ambiguousIdentifierExpressionNode,
				compilationUnit,
				ref parserModel);
			
			if (decidedExpression.SyntaxKind != SyntaxKind.TypeClauseNode)
			{
				parserModel.DiagnosticBag.ReportTodoException(
		    		parserModel.TokenWalker.Current.TextSpan,
		    		"if (decidedExpression.SyntaxKind != SyntaxKind.TypeClauseNode)");
				return decidedExpression;
			}
		
			var identifierToken = (IdentifierToken)parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
			
			var variableDeclarationNode = ParseVariables.HandleVariableDeclarationExpression(
		        (TypeClauseNode)decidedExpression,
		        identifierToken,
		        VariableKind.Local,
		        compilationUnit,
		        ref parserModel);
		        
		    return variableDeclarationNode;
		}
	
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), ambiguousIdentifierExpressionNode, token);
	}
		
	public IExpressionNode AmbiguousIdentifierMergeExpression(
		AmbiguousIdentifierExpressionNode ambiguousIdentifierExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
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
		CSharpCompilationUnit compilationUnit,
		ref CSharpParserModel parserModel,
		bool forceVariableReferenceNode = false,
		bool allowFabricatedUndefinedNode = true)
	{
		if (parserModel.ParserContextKind == CSharpParserContextKind.ForceStatementExpression)
		{
			parserModel.ParserContextKind = CSharpParserContextKind.None;
			
			if ((parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenAngleBracketToken ||
			 		UtilityApi.IsConvertibleToIdentifierToken(parserModel.TokenWalker.Next.SyntaxKind)) &&
				 parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.MemberAccessToken)
			{
				parserModel.ParserContextKind = CSharpParserContextKind.ForceParseNextIdentifierAsTypeClauseNode;
			}
		}
	
		if (parserModel.ParserContextKind != CSharpParserContextKind.ForceParseNextIdentifierAsTypeClauseNode &&
			UtilityApi.IsConvertibleToIdentifierToken(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
		{
			if (TryGetVariableDeclarationHierarchically(
			    	compilationUnit,
			    	compilationUnit.BinderSession.ResourceUri,
			    	compilationUnit.BinderSession.CurrentScopeIndexKey,
			        ambiguousIdentifierExpressionNode.Token.TextSpan.GetText(),
			        out var existingVariableDeclarationNode))
			{
				var identifierToken = UtilityApi.ConvertToIdentifierToken(ambiguousIdentifierExpressionNode.Token, compilationUnit, ref parserModel);
				
				var variableReferenceNode = ConstructAndBindVariableReferenceNode(
					identifierToken,
					compilationUnit);
    			
    			return variableReferenceNode;
			}
		}
		
		if (!forceVariableReferenceNode && UtilityApi.IsConvertibleToTypeClauseNode(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
		{
			if (TryGetTypeDefinitionHierarchically(
	        		compilationUnit,
	        		compilationUnit.BinderSession.ResourceUri,
	                compilationUnit.BinderSession.CurrentScopeIndexKey,
	                ambiguousIdentifierExpressionNode.Token.TextSpan.GetText(),
	                out var typeDefinitionNode))
	        {
	            var typeClauseNode = UtilityApi.ConvertToTypeClauseNode(ambiguousIdentifierExpressionNode.Token, compilationUnit, ref parserModel);
				BindTypeClauseNode(typeClauseNode, compilationUnit);
			    return typeClauseNode;
	        }
		}
		
		if (ambiguousIdentifierExpressionNode.Token.SyntaxKind == SyntaxKind.IdentifierToken &&
			ambiguousIdentifierExpressionNode.Token.TextSpan.Length == 1 &&
    		ambiguousIdentifierExpressionNode.Token.TextSpan.GetText() == "_")
    	{
    		if (!compilationUnit.Binder.TryGetVariableDeclarationHierarchically(
			    	compilationUnit,
			    	compilationUnit.BinderSession.ResourceUri,
			    	compilationUnit.BinderSession.CurrentScopeIndexKey,
			        ambiguousIdentifierExpressionNode.Token.TextSpan.GetText(),
			        out _))
			{
				compilationUnit.Binder.BindDiscard((IdentifierToken)ambiguousIdentifierExpressionNode.Token, compilationUnit);
	    		return ambiguousIdentifierExpressionNode;
			}
    	}
		
		if (allowFabricatedUndefinedNode)
		{
			// Bind an undefined-TypeClauseNode
			if (!forceVariableReferenceNode ||
				UtilityApi.IsConvertibleToTypeClauseNode(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
			{
	            var typeClauseNode = UtilityApi.ConvertToTypeClauseNode(ambiguousIdentifierExpressionNode.Token, compilationUnit, ref parserModel);
				BindTypeClauseNode(typeClauseNode, compilationUnit);
			    return typeClauseNode;
			}
			
			// Bind an undefined-variable
			if (UtilityApi.IsConvertibleToIdentifierToken(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
			{
				var identifierToken = UtilityApi.ConvertToIdentifierToken(ambiguousIdentifierExpressionNode.Token, compilationUnit, ref parserModel);
				
				var variableReferenceNode = ConstructAndBindVariableReferenceNode(
					identifierToken,
					compilationUnit);
				
				return variableReferenceNode;
			}
		}
		
		return ambiguousIdentifierExpressionNode;
	}
		
	public IExpressionNode BadMergeToken(
		BadExpressionNode badExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		badExpressionNode.SyntaxList.Add(token);
		return badExpressionNode;
	}

	public IExpressionNode BadMergeExpression(
		BadExpressionNode badExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		badExpressionNode.SyntaxList.Add(expressionSecondary);
		return badExpressionNode;
	}

	public IExpressionNode BinaryMergeToken(
		BinaryExpressionNode binaryExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
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
	
	public IExpressionNode BinaryMergeExpression(
		BinaryExpressionNode binaryExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (binaryExpressionNode.RightExpressionNode.SyntaxKind == SyntaxKind.EmptyExpressionNode)
		{
			if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
			{
				expressionSecondary = ForceDecisionAmbiguousIdentifier(
					EmptyExpressionNode.Empty,
					(AmbiguousIdentifierExpressionNode)expressionSecondary,
					compilationUnit,
					ref parserModel);
			}
				
			binaryExpressionNode.SetRightExpressionNode(expressionSecondary);
			return binaryExpressionNode;
		}
	
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), binaryExpressionNode, expressionSecondary);
	}
	
	public IExpressionNode CommaSeparatedMergeToken(
		CommaSeparatedExpressionNode commaSeparatedExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (token.SyntaxKind == SyntaxKind.CommaToken)
		{
			if (!commaSeparatedExpressionNode.CloseParenthesisToken.ConstructorWasInvoked)
			{
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, commaSeparatedExpressionNode));
				return EmptyExpressionNode.Empty;
			}
		}
		else if (token.SyntaxKind == SyntaxKind.CloseParenthesisToken)
		{
			return commaSeparatedExpressionNode;
		}
		
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), commaSeparatedExpressionNode, token);
	}
	
	public IExpressionNode CommaSeparatedMergeExpression(
		CommaSeparatedExpressionNode commaSeparatedExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken || parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
		{
			commaSeparatedExpressionNode.AddInnerExpressionNode(expressionSecondary);
			return commaSeparatedExpressionNode;
		}
	
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), commaSeparatedExpressionNode, expressionSecondary);
	}
	
	public IExpressionNode ConstructorInvocationMergeToken(
		ConstructorInvocationExpressionNode constructorInvocationExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.IdentifierToken:
				if (constructorInvocationExpressionNode.ResultTypeClauseNode is null)
				{
					if (UtilityApi.IsConvertibleToTypeClauseNode(token.SyntaxKind))
					{
						var typeClauseNode = UtilityApi.ConvertToTypeClauseNode(token, compilationUnit, ref parserModel);
						
						BindTypeClauseNode(
					        typeClauseNode,
					        compilationUnit);
						
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
				
				return ParseFunctionParametersListingNode(constructorInvocationExpressionNode, constructorInvocationExpressionNode.FunctionParametersListingNode, compilationUnit, ref parserModel);
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
			    parserModel.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, constructorInvocationExpressionNode));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode));
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
				parserModel.ExpressionList.Add((SyntaxKind.CloseBraceToken, constructorInvocationExpressionNode));
				parserModel.ExpressionList.Add((SyntaxKind.EqualsToken, constructorInvocationExpressionNode));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode));
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
				parserModel.ExpressionList.Add((SyntaxKind.EqualsToken, constructorInvocationExpressionNode));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode));
				
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
			case SyntaxKind.CommaToken:
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode));
				return EmptyExpressionNode.Empty;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), constructorInvocationExpressionNode, token);
		}
	}
	
	public IExpressionNode ConstructorInvocationMergeExpression(
		ConstructorInvocationExpressionNode constructorInvocationExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (constructorInvocationExpressionNode.ConstructorInvocationStageKind != ConstructorInvocationStageKind.ObjectInitializationParameters &&
			expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionSecondary = ForceDecisionAmbiguousIdentifier(
				constructorInvocationExpressionNode,
				(AmbiguousIdentifierExpressionNode)expressionSecondary,
				compilationUnit,
				ref parserModel);
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
			var currentTokenIsComma = parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken;
			var currentTokenIsBrace = parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken;

			if (!objectInitializationParameterEntryNode.PropertyIdentifierToken.ConstructorWasInvoked &&
				(!currentTokenIsComma && !currentTokenIsBrace))
			{
				if (expressionSecondary.SyntaxKind == SyntaxKind.VariableReferenceNode)
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
				else if (expressionSecondary.SyntaxKind == SyntaxKind.TypeClauseNode)
				{
					var typeClauseNode = (TypeClauseNode)expressionSecondary;
					objectInitializationParameterEntryNode.PropertyIdentifierToken = (IdentifierToken)typeClauseNode.TypeIdentifierToken;
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
				if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
				{
					expressionSecondary = ForceDecisionAmbiguousIdentifier(
						constructorInvocationExpressionNode,
						(AmbiguousIdentifierExpressionNode)expressionSecondary,
						compilationUnit,
						ref parserModel);
				}
			
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
		EmptyExpressionNode emptyExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (UtilityApi.IsConvertibleToTypeClauseNode(token.SyntaxKind))
		{
			var ambiguousExpressionNode = new AmbiguousIdentifierExpressionNode(
				token,
		        genericParametersListingNode: null,
		        CSharpFacts.Types.Void.ToTypeClause())
		    {
		    	FollowsMemberAccessToken = emptyExpressionNode.FollowsMemberAccessToken
		    };
		    
		    if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.StatementDelimiterToken && !ambiguousExpressionNode.FollowsMemberAccessToken ||
		    	parserModel.TryParseExpressionSyntaxKindList.Contains(SyntaxKind.TypeClauseNode) && parserModel.TokenWalker.Next.SyntaxKind != SyntaxKind.WithTokenContextualKeyword)
		    {
				return ForceDecisionAmbiguousIdentifier(
					emptyExpressionNode,
					ambiguousExpressionNode,
					compilationUnit,
					ref parserModel);
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
			
				if (!UtilityApi.IsConvertibleToTypeClauseNode(parserModel.TokenWalker.Next.SyntaxKind))
				{
					var parenthesizedExpressionNode = new ParenthesizedExpressionNode(
						(OpenParenthesisToken)token,
						CSharpFacts.Types.Void.ToTypeClause());
					
					parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, parenthesizedExpressionNode));
					parserModel.ExpressionList.Add((SyntaxKind.CommaToken, parenthesizedExpressionNode));
					
					return EmptyExpressionNode.Empty;
				}
			
				var ambiguousParenthesizedExpressionNode = new AmbiguousParenthesizedExpressionNode(
					(OpenParenthesisToken)token,
					isParserContextKindForceStatementExpression: parserModel.ParserContextKind == CSharpParserContextKind.ForceStatementExpression ||
					// '(List<(int, bool)>)' required the following hack because the CSharpParserContextKind.ForceStatementExpression enum
					// is reset after the first TypeClauseNode in a statement is made, and there was no clear way to set it back again in this situation.;
					// TODO: Don't do this '(List<(int, bool)>)', instead figure out how to have CSharpParserContextKind.ForceStatementExpression live longer in a statement that has many TypeClauseNode(s).
					parserModel.ExpressionList.Any(x => x.ExpressionNode is not null && x.ExpressionNode.SyntaxKind == SyntaxKind.GenericParametersListingNode));
					
				parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, ambiguousParenthesizedExpressionNode));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, ambiguousParenthesizedExpressionNode));
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
				BindStringInterpolationExpression((DollarSignToken)token, compilationUnit);
				return emptyExpressionNode;
			case SyntaxKind.AtToken:
				BindStringVerbatimExpression((AtToken)token, compilationUnit);
				return emptyExpressionNode;
			case SyntaxKind.OutTokenKeyword:
				return emptyExpressionNode;
			case SyntaxKind.InTokenKeyword:
			case SyntaxKind.RefTokenKeyword:
			case SyntaxKind.ParamsTokenKeyword:
			case SyntaxKind.ThisTokenKeyword:
				return emptyExpressionNode;
			case SyntaxKind.OpenAngleBracketToken:
				var genericParametersListingNode = new GenericParametersListingNode(
					(OpenAngleBracketToken)token,
			        new List<GenericParameterEntryNode>(),
				    closeAngleBracketToken: default);
				
				parserModel.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, genericParametersListingNode));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, genericParametersListingNode));
				
				return EmptyExpressionNode.Empty;
			case SyntaxKind.ReturnTokenKeyword:
				var returnStatementNode = new ReturnStatementNode((KeywordToken)token, EmptyExpressionNode.Empty);
				parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, returnStatementNode));
				return EmptyExpressionNode.Empty;
			case SyntaxKind.BangToken:
			case SyntaxKind.PipeToken:
			case SyntaxKind.PipePipeToken:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.AmpersandAmpersandToken:
			case SyntaxKind.PlusPlusToken:
			case SyntaxKind.MinusMinusToken:
				return emptyExpressionNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), emptyExpressionNode, token);
		}
	}
	
	public IExpressionNode ExplicitCastMergeToken(
		ExplicitCastNode explicitCastNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
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
		GenericParametersListingNode genericParametersListingNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CommaToken:
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, genericParametersListingNode));
				return EmptyExpressionNode.Empty;
			case SyntaxKind.CloseAngleBracketToken:
				// This case only occurs when the text won't compile.
				// i.e.: "<int>" rather than "MyClass<int>".
				// The case is for when the user types just the generic parameter listing text without an identifier before it.
				//
				// In the case of "SomeMethod<int>()", the FunctionInvocationNode
				// is expected to have ran 'parserModel.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, functionInvocationNode));'
				// to receive the genericParametersListingNode.
				return genericParametersListingNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), genericParametersListingNode, token);
		}
	}
	
	public IExpressionNode GenericParametersListingMergeExpression(
		GenericParametersListingNode genericParametersListingNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			return genericParametersListingNode;
	
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			var expressionSecondaryTyped = (AmbiguousIdentifierExpressionNode)expressionSecondary;
			
			var typeClauseNode = UtilityApi.ConvertToTypeClauseNode(expressionSecondaryTyped.Token, compilationUnit, ref parserModel);
			
			BindTypeClauseNode(
		        typeClauseNode,
		        compilationUnit);
			
			genericParametersListingNode.GenericParameterEntryNodeList.Add(
				new GenericParameterEntryNode(typeClauseNode));
			
			return genericParametersListingNode;
		}
		else if (expressionSecondary.SyntaxKind == SyntaxKind.TypeClauseNode)
		{
			var typeClauseNode = (TypeClauseNode)expressionSecondary;
		
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
			        compilationUnit);
			    
				genericParametersListingNode.GenericParameterEntryNodeList.Add(
					new GenericParameterEntryNode(typeClauseNode));
				
				return genericParametersListingNode;
			}
		}
		
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), genericParametersListingNode, expressionSecondary);
	}
	
	public IExpressionNode FunctionParametersListingMergeToken(
		FunctionParametersListingNode functionParametersListingNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CommaToken:
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, functionParametersListingNode));
				parserModel.ExpressionList.Add((SyntaxKind.ColonToken, functionParametersListingNode));
				return ParseNamedParameterSyntaxAndReturnEmptyExpressionNode(compilationUnit, ref parserModel);
			case SyntaxKind.ColonToken:
				parserModel.ExpressionList.Add((SyntaxKind.ColonToken, functionParametersListingNode));
				return EmptyExpressionNode.Empty;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), functionParametersListingNode, token);
		}
	}
	
	public IExpressionNode FunctionParametersListingMergeExpression(
		FunctionParametersListingNode functionParametersListingNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
		{
			return functionParametersListingNode;
		}
		
		if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			return functionParametersListingNode;
			
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionSecondary = ForceDecisionAmbiguousIdentifier(
				functionParametersListingNode,
				(AmbiguousIdentifierExpressionNode)expressionSecondary,
				compilationUnit,
				ref parserModel);
		}
			
		var functionParameterEntryNode = new FunctionParameterEntryNode(
	        expressionSecondary,
	        hasOutKeyword: false,
	        hasInKeyword: false,
	        hasRefKeyword: false);
	        
		functionParametersListingNode.FunctionParameterEntryNodeList.Add(functionParameterEntryNode);
		
		return functionParametersListingNode;
	}
	
	public IExpressionNode FunctionArgumentsListingMergeToken(
		FunctionArgumentsListingNode functionArgumentsListingNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CommaToken:
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, functionArgumentsListingNode));
				return EmptyExpressionNode.Empty;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), functionArgumentsListingNode, token);
		}
	}
	
	public IExpressionNode FunctionArgumentsListingMergeExpression(
		FunctionArgumentsListingNode functionArgumentsListingNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			return functionArgumentsListingNode;
			
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionSecondary = ForceDecisionAmbiguousIdentifier(
				functionArgumentsListingNode,
				(AmbiguousIdentifierExpressionNode)expressionSecondary,
				compilationUnit,
				ref parserModel);
		}
			
	    var functionArgumentEntryNode = new FunctionArgumentEntryNode(
	        variableDeclarationNode: null,
	        optionalCompileTimeConstantToken: null,
	        isOptional: false,
	        hasParamsKeyword: false,
	        hasOutKeyword: false,
	        hasInKeyword: false,
	        hasRefKeyword: false);
	        
		functionArgumentsListingNode.FunctionArgumentEntryNodeList.Add(functionArgumentEntryNode);
		
		return functionArgumentsListingNode;
	}
	
	public IExpressionNode ReturnStatementMergeToken(
		ReturnStatementNode returnStatementNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), returnStatementNode, token);
		}
	}
	
	public IExpressionNode ReturnStatementMergeExpression(
		ReturnStatementNode returnStatementNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), returnStatementNode, expressionSecondary);
	}
	
	public IExpressionNode LambdaMergeToken(
		LambdaExpressionNode lambdaExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (token.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
		{
			var textSpan = new TextEditorTextSpan(
				token.TextSpan.StartingIndexInclusive,
			    token.TextSpan.EndingIndexExclusive,
			    (byte)GenericDecorationKind.None,
			    token.TextSpan.ResourceUri,
			    token.TextSpan.SourceText);
		
			((CSharpBinder)compilationUnit.Binder).AddSymbolDefinition(
				new LambdaSymbol(compilationUnit.BinderSession.GetNextSymbolId(), textSpan, lambdaExpressionNode), compilationUnit);
		
			if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenBraceToken)
			{
				lambdaExpressionNode.CodeBlockNodeIsExpression = false;
			
				parserModel.ExpressionList.Add((SyntaxKind.CloseBraceToken, lambdaExpressionNode));
				parserModel.ExpressionList.Add((SyntaxKind.StatementDelimiterToken, lambdaExpressionNode));
				return EmptyExpressionNode.Empty;
			}
			
			parserModel.ExpressionList.Add((SyntaxKind.StatementDelimiterToken, lambdaExpressionNode));
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
				parserModel.ExpressionList.Add((SyntaxKind.StatementDelimiterToken, lambdaExpressionNode));
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
				parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, lambdaExpressionNode));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, lambdaExpressionNode));
				return EmptyExpressionNode.Empty;
			}
		}
		else if (token.SyntaxKind == SyntaxKind.CloseParenthesisToken)
		{
			return lambdaExpressionNode;
		}
		else if (token.SyntaxKind == SyntaxKind.EqualsToken)
		{
			if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
				return lambdaExpressionNode;
			
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), lambdaExpressionNode, token);
		}
		else if (token.SyntaxKind == SyntaxKind.CommaToken)
		{
			parserModel.ExpressionList.Add((SyntaxKind.CommaToken, lambdaExpressionNode));
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
		LambdaExpressionNode lambdaExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (expressionSecondary.SyntaxKind)
		{
			default:
				if (lambdaExpressionNode.CloseCodeBlockTextSpan is null)
					CloseLambdaExpressionScope(lambdaExpressionNode, compilationUnit, ref parserModel);
				
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
		LiteralExpressionNode literalExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), literalExpressionNode, token);
	}
	
	public IExpressionNode ParenthesizedMergeToken(
		ParenthesizedExpressionNode parenthesizedExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CloseParenthesisToken:
				if (parenthesizedExpressionNode.InnerExpression.SyntaxKind == SyntaxKind.TypeClauseNode)
				{
					var typeClauseNode = (TypeClauseNode)parenthesizedExpressionNode.InnerExpression;
					var explicitCastNode = new ExplicitCastNode(parenthesizedExpressionNode.OpenParenthesisToken, typeClauseNode);
					return ExplicitCastMergeToken(explicitCastNode, token, compilationUnit, ref parserModel);
				}
				
				return parenthesizedExpressionNode.SetCloseParenthesisToken((CloseParenthesisToken)token);
			case SyntaxKind.EqualsCloseAngleBracketToken:
				var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
				SetLambdaExpressionNodeVariableDeclarationNodeList(lambdaExpressionNode, parenthesizedExpressionNode.InnerExpression, compilationUnit, ref parserModel);
				return lambdaExpressionNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), parenthesizedExpressionNode, token);
		}
	}
	
	public IExpressionNode ParenthesizedMergeExpression(
		ParenthesizedExpressionNode parenthesizedExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
		{
			var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
			return SetLambdaExpressionNodeVariableDeclarationNodeList(lambdaExpressionNode, expressionSecondary, compilationUnit, ref parserModel);
		}
	
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
			expressionSecondary = ForceDecisionAmbiguousIdentifier(parenthesizedExpressionNode, (AmbiguousIdentifierExpressionNode)expressionSecondary, compilationUnit, ref parserModel);
	
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
		{
			parserModel.NoLongerRelevantExpressionNode = parenthesizedExpressionNode;
			var tupleExpressionNode = new TupleExpressionNode();
			tupleExpressionNode.AddInnerExpressionNode(expressionSecondary);
			// tupleExpressionNode never saw the 'OpenParenthesisToken' so the 'ParenthesizedExpressionNode'
			// has to create the ExpressionList entry on behalf of the 'TupleExpressionNode'.
			parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, tupleExpressionNode));
			return tupleExpressionNode;
		}
	
		if (parenthesizedExpressionNode.InnerExpression.SyntaxKind != SyntaxKind.EmptyExpressionNode)
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), parenthesizedExpressionNode, expressionSecondary);
		
		// TODO: This seems like a bad idea?
		if (expressionSecondary.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			 var variableReferenceNode = (VariableReferenceNode)expressionSecondary;
			 
			 if (variableReferenceNode.IsFabricated)
			 {
			 	var typeClauseNode = new TypeClauseNode(variableReferenceNode.VariableIdentifierToken, valueType: null, genericParametersListingNode: null);
				
				BindTypeClauseNode(
			        typeClauseNode,
			        compilationUnit);
			        
				return new ExplicitCastNode(parenthesizedExpressionNode.OpenParenthesisToken, typeClauseNode);
			 }
		}

		return parenthesizedExpressionNode.SetInnerExpression(expressionSecondary);
	}
	
	public IExpressionNode TupleMergeToken(
		TupleExpressionNode tupleExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CloseParenthesisToken:
				return tupleExpressionNode;
			case SyntaxKind.CommaToken:
				// TODO: Track the CloseParenthesisToken and ensure it isn't 'ConstructorWasInvoked'.
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, tupleExpressionNode));
				return EmptyExpressionNode.Empty;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), tupleExpressionNode, token);
		}
	}
	
	public IExpressionNode TupleMergeExpression(
		TupleExpressionNode tupleExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (expressionSecondary.SyntaxKind)
		{
			case SyntaxKind.TupleExpressionNode:
				return tupleExpressionNode;
			default:
				if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
				{
					expressionSecondary = ForceDecisionAmbiguousIdentifier(
						EmptyExpressionNode.Empty,
						(AmbiguousIdentifierExpressionNode)expressionSecondary,
						compilationUnit,
						ref parserModel);
				}
				
				if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken || parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
				{
					tupleExpressionNode.AddInnerExpressionNode(expressionSecondary);
					return tupleExpressionNode;
				}
			
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), tupleExpressionNode, expressionSecondary);
		}
	}
	
	public IExpressionNode TypeClauseMergeToken(
		TypeClauseNode typeClauseNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.OpenAngleBracketToken:
				return ParseTypeClauseNodeGenericParametersListingNode(
					typeClauseNode, (OpenAngleBracketToken)token, compilationUnit, ref parserModel);
			case SyntaxKind.CloseAngleBracketToken:
				if (typeClauseNode.GenericParametersListingNode is not null)
				{
					typeClauseNode.GenericParametersListingNode.SetCloseAngleBracketToken((CloseAngleBracketToken)token);
					return typeClauseNode;
				}
				
			    goto default;
			case SyntaxKind.QuestionMarkToken:
				if (!typeClauseNode.HasQuestionMark)
				{
					typeClauseNode.HasQuestionMark = true;
					return typeClauseNode;
				}
				
			    goto default;
			case SyntaxKind.OpenParenthesisToken:
				if (token.SyntaxKind == SyntaxKind.OpenParenthesisToken &&
					UtilityApi.IsConvertibleToIdentifierToken(typeClauseNode.TypeIdentifierToken.SyntaxKind))
				{
					var functionParametersListingNode = new FunctionParametersListingNode(
						(OpenParenthesisToken)token,
				        new List<FunctionParameterEntryNode>(),
				        closeParenthesisToken: default);
				
					var functionInvocationNode = new FunctionInvocationNode(
						UtilityApi.ConvertToIdentifierToken(typeClauseNode.TypeIdentifierToken, compilationUnit, ref parserModel),
				        functionDefinitionNode: null,
				        typeClauseNode.GenericParametersListingNode,
				        functionParametersListingNode,
				        CSharpFacts.Types.Void.ToTypeClause());
				        
				    BindFunctionInvocationNode(
				        functionInvocationNode,
				        compilationUnit);
		
					return ParseFunctionParametersListingNode(functionInvocationNode, functionInvocationNode.FunctionParametersListingNode, compilationUnit, ref parserModel);
				}
				
				goto default;
			case SyntaxKind.OpenSquareBracketToken:
				typeClauseNode.ArrayRank++;
				return typeClauseNode;
			case SyntaxKind.CloseSquareBracketToken:
				return typeClauseNode;
			default:
				if (UtilityApi.IsConvertibleToIdentifierToken(token.SyntaxKind))
				{
					var identifierToken = UtilityApi.ConvertToIdentifierToken(token, compilationUnit, ref parserModel);
					var isRootExpression = true;
					
					foreach (var tuple in parserModel.ExpressionList)
					{
						if (tuple.ExpressionNode is null)
							continue;
						
						isRootExpression = false;
						break;
					}
					
					IVariableDeclarationNode variableDeclarationNode;
					
					if (isRootExpression)
					{
						// If isRootExpression do not bind the VariableDeclarationNode
						// because it could in reality be a FunctionDefinitionNode.
						//
						// So, manually create the node, and then eventually return back to the
						// statement code so it can check for a FunctionDefinitionNode.
						//
						// If it truly is a VariableDeclarationNode,
						// then it is the responsibility of the statement code
						// to bind the VariableDeclarationNode, and add it to the current code block builder.
						variableDeclarationNode = new VariableDeclarationNode(
					        typeClauseNode,
					        identifierToken,
					        VariableKind.Local,
					        false);
					}
					else
					{
						variableDeclarationNode = ParseVariables.HandleVariableDeclarationExpression(
					        typeClauseNode,
					        identifierToken,
					        VariableKind.Local,
					        compilationUnit,
					        ref parserModel);
					}
				        
				    return variableDeclarationNode;
				}
				
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), typeClauseNode, token);
		}
	}
	
	public IExpressionNode TypeClauseMergeExpression(
		TypeClauseNode typeClauseNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (expressionSecondary.SyntaxKind)
		{
			case SyntaxKind.GenericParametersListingNode:
				if (typeClauseNode.GenericParametersListingNode is not null &&
					!typeClauseNode.GenericParametersListingNode.CloseAngleBracketToken.ConstructorWasInvoked)
				{
					return typeClauseNode;
				}
				
				goto default;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), typeClauseNode, expressionSecondary);
		}
	}
	
	public IExpressionNode VariableAssignmentMergeToken(
		VariableAssignmentExpressionNode variableAssignmentNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		return variableAssignmentNode;
	}
	
	public IExpressionNode VariableAssignmentMergeExpression(
		VariableAssignmentExpressionNode variableAssignmentNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (variableAssignmentNode.ExpressionNode == EmptyExpressionNode.Empty)
		{
			variableAssignmentNode.SetExpressionNode(expressionSecondary);
			return variableAssignmentNode;
		}
		else
		{
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), variableAssignmentNode, expressionSecondary);
		}
	}
	
	public IExpressionNode FunctionInvocationMergeToken(
		FunctionInvocationNode functionInvocationNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
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
		FunctionInvocationNode functionInvocationNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionSecondary = ForceDecisionAmbiguousIdentifier(functionInvocationNode, (AmbiguousIdentifierExpressionNode)expressionSecondary, compilationUnit, ref parserModel);
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
		LambdaExpressionNode lambdaExpressionNode, IExpressionNode expressionNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (expressionNode.SyntaxKind == SyntaxKind.BadExpressionNode)
		{
			var badExpressionNode = (BadExpressionNode)expressionNode;
		
			if (badExpressionNode.SyntaxList.Count == 2 &&
	    		badExpressionNode.SyntaxList[0].SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode &&
	    		badExpressionNode.SyntaxList[1].SyntaxKind == SyntaxKind.IdentifierToken)
	    	{
	    		var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)badExpressionNode.SyntaxList[0];
	    		
	    		var typeClauseNode = UtilityApi.ConvertToTypeClauseNode(ambiguousIdentifierExpressionNode.Token, compilationUnit, ref parserModel);
					
				BindTypeClauseNode(
			        typeClauseNode,
			        compilationUnit);
	    		
	    		var identifierToken = (IdentifierToken)badExpressionNode.SyntaxList[1];
	    		
	    		var variableDeclarationNode = ParseVariables.HandleVariableDeclarationExpression(
			        typeClauseNode,
			        identifierToken,
			        VariableKind.Local,
			        compilationUnit,
			        ref parserModel);
			        
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
    						typeClauseNode = UtilityApi.ConvertToTypeClauseNode(ambiguousIdentifierExpressionNode.Token, compilationUnit, ref parserModel);
					
							BindTypeClauseNode(
						        typeClauseNode,
						        compilationUnit);
    					}
    					else if (firstSyntax.SyntaxKind == SyntaxKind.IdentifierToken)
    					{
    						var identifierToken = (IdentifierToken)firstSyntax;
    						typeClauseNode = new TypeClauseNode(identifierToken, valueType: null, genericParametersListingNode: null);
					
							BindTypeClauseNode(
						        typeClauseNode,
						        compilationUnit);
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
				        compilationUnit,
				        ref parserModel);
				        
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
		        compilationUnit,
		        ref parserModel);
		        
    		lambdaExpressionNode.AddVariableDeclarationNode(variableDeclarationNode);
    	}
    	
    	return lambdaExpressionNode;
	}
	
	/// <summary>
	/// A ParenthesizedExpressionNode expression will "become" a CommaSeparatedExpressionNode
	/// upon encounter a CommaToken within its parentheses.
	///
	/// An issue arises however, because the parserModel.ExpressionList still says to
	/// "short circuit" when the CloseParenthesisToken is encountered,
	/// and to at this point make the ParenthesizedExpressionNode the primary expression.
	///
	/// Well, the ParenthesizedExpressionNode should no longer exist, it was deemed
	/// to be more accurately described by a CommaSeparatedExpressionNode.
	///
	/// So, this method will remove any entries in the parserModel.ExpressionList
	/// that have the 'ParenthesizedExpressionNode' as the to-be primary expression.
	/// </summary>
	public void ClearFromExpressionList(IExpressionNode expressionNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		for (int i = parserModel.ExpressionList.Count - 1; i > -1; i--)
		{
			var delimiterExpressionTuple = parserModel.ExpressionList[i];
			
			if (Object.ReferenceEquals(expressionNode, delimiterExpressionTuple.ExpressionNode))
				parserModel.ExpressionList.RemoveAt(i);
		}
	}
	
	/// <summary>
	/// 'bool foundChild' usage:
	/// If the child is NOT in the ExpressionList then this is true,
	///
	/// But, if the child is in the ExpressionList, and is not the final entry in the ExpressionList,
	/// then this needs to be set to 'false', otherwise a descendent node of 'childExpressionNode'
	/// will be thought to be the parent node due to the list being traversed end to front order.
	/// </summary>
	public IExpressionNode GetParentNode(
		IExpressionNode childExpressionNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel, bool foundChild = true)
	{
		for (int i = parserModel.ExpressionList.Count - 1; i > -1; i--)
		{
			var delimiterExpressionTuple = parserModel.ExpressionList[i];
			
			if (foundChild)
			{
				if (delimiterExpressionTuple.ExpressionNode is null)
					break;
					
				if (!Object.ReferenceEquals(childExpressionNode, delimiterExpressionTuple.ExpressionNode))
					return delimiterExpressionTuple.ExpressionNode;
			}
			else
			{
				if (Object.ReferenceEquals(childExpressionNode, delimiterExpressionTuple.ExpressionNode))
					foundChild = true;
			}
		}
		
		return EmptyExpressionNode.Empty;
	}
	
	/// <summary>
	/// The argument 'IExpressionNode functionInvocationNode' is not of type 'FunctionInvocationNode'
	/// in order to permit this method to be invoked for 'ConstructorInvocationExpressionNode' as well.
	/// </summary>
	public IExpressionNode ParseFunctionParametersListingNode(IExpressionNode functionInvocationNode, FunctionParametersListingNode functionParametersListingNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, functionInvocationNode));
		parserModel.ExpressionList.Add((SyntaxKind.CommaToken, functionParametersListingNode));
		parserModel.ExpressionList.Add((SyntaxKind.ColonToken, functionParametersListingNode));
		return ParseNamedParameterSyntaxAndReturnEmptyExpressionNode(compilationUnit, ref parserModel);
	}
	
	public IExpressionNode ParseNamedParameterSyntaxAndReturnEmptyExpressionNode(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (UtilityApi.IsConvertibleToIdentifierToken(parserModel.TokenWalker.Peek(1).SyntaxKind) &&
			parserModel.TokenWalker.Peek(2).SyntaxKind == SyntaxKind.ColonToken)
		{
			// Consume the open parenthesis
			_ = parserModel.TokenWalker.Consume();
			
			// Consume the identifierToken
			compilationUnit.Binder.CreateVariableSymbol(
		        UtilityApi.ConvertToIdentifierToken(parserModel.TokenWalker.Consume(), compilationUnit, ref parserModel),
		        VariableKind.Local,
		        compilationUnit);
			
			// Consume the ColonToken
			_ = parserModel.TokenWalker.Consume();
		}
	
		return EmptyExpressionNode.Empty;
	}
	
	public IExpressionNode ParseAmbiguousIdentifierGenericParametersListingNode(
		AmbiguousIdentifierExpressionNode ambiguousIdentifierExpressionNode, OpenAngleBracketToken openAngleBracketToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (ambiguousIdentifierExpressionNode.GenericParametersListingNode is null)
		{
			ambiguousIdentifierExpressionNode.SetGenericParametersListingNode(
				new GenericParametersListingNode(
					openAngleBracketToken,
			        new List<GenericParameterEntryNode>(),
			        closeAngleBracketToken: default));
		}
		
	    parserModel.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, ambiguousIdentifierExpressionNode));
		parserModel.ExpressionList.Add((SyntaxKind.CommaToken, ambiguousIdentifierExpressionNode.GenericParametersListingNode));
		return EmptyExpressionNode.Empty;
	}
	
	public IExpressionNode ParseTypeClauseNodeGenericParametersListingNode(
		TypeClauseNode typeClauseNode, OpenAngleBracketToken openAngleBracketToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (typeClauseNode.GenericParametersListingNode is null)
		{
			typeClauseNode.SetGenericParametersListingNode(
				new GenericParametersListingNode(
					openAngleBracketToken,
			        new List<GenericParameterEntryNode>(),
			        closeAngleBracketToken: default));
		}
		
	    parserModel.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, typeClauseNode));
		parserModel.ExpressionList.Add((SyntaxKind.CommaToken, typeClauseNode.GenericParametersListingNode));
		return EmptyExpressionNode.Empty;
	}
	
	public void OpenLambdaExpressionScope(LambdaExpressionNode lambdaExpressionNode, OpenBraceToken openBraceToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		lambdaExpressionNode.SetOpenCodeBlockTextSpan(openBraceToken.TextSpan, parserModel.DiagnosticBag, parserModel.TokenWalker);

		// (2025-01-13)
		// ========================================================
		// - 'SetActiveCodeBlockBuilder', 'SetActiveScope', and 'PermitInnerPendingCodeBlockOwnerToBeParsed'
		//   should all be handled by the same method.
		
		compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	lambdaExpressionNode,
        	lambdaExpressionNode.GetReturnTypeClauseNode(),
        	openBraceToken.TextSpan,
        	compilationUnit,
	        ref parserModel);
		
		compilationUnit.Binder.OnBoundScopeCreatedAndSetAsCurrent(lambdaExpressionNode, compilationUnit, ref parserModel);
	}
	
	public void CloseLambdaExpressionScope(LambdaExpressionNode lambdaExpressionNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		var closeBraceToken = new CloseBraceToken(parserModel.TokenWalker.Current.TextSpan);
	
		if (parserModel.CurrentCodeBlockBuilder.CodeBlockOwner is not null)
			parserModel.CurrentCodeBlockBuilder.CodeBlockOwner.SetCloseCodeBlockTextSpan(closeBraceToken.TextSpan, parserModel.DiagnosticBag, parserModel.TokenWalker);
		
        compilationUnit.Binder.CloseScope(closeBraceToken.TextSpan, compilationUnit, ref parserModel);
	}
	
	/// <summary>
	/// TODO: Parse the lambda expression's statements...
	///       ...This sounds quite complicated because we went
	///       from the statement-loop to the expression-loop
	///       and now have to run the statement-loop again
	///       but not lose the state of any active loops.
	///       For now skip tokens until the close brace token is matched in order to
	///       preserve the other features of the text editor.
	///       (rather than lambda expression statements clobbering the entire syntax highlighting of the file).
	/// </summary>
	public void SkipLambdaExpressionStatements(LambdaExpressionNode lambdaExpressionNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		parserModel.TokenWalker.Consume(); // Skip the EqualsCloseAngleBracketToken
		
		var openBraceToken = (OpenBraceToken)parserModel.TokenWalker.Consume();
    	
    	var openBraceCounter = 1;
		
		while (true)
		{
			if (parserModel.TokenWalker.IsEof)
				break;

			if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
			{
				++openBraceCounter;
			}
			else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
			{
				if (--openBraceCounter <= 0)
					break;
			}

			_ = parserModel.TokenWalker.Consume();
		}

		CloseLambdaExpressionScope(lambdaExpressionNode, compilationUnit, ref parserModel);
	
		var closeTokenIndex = parserModel.TokenWalker.Index;
		var closeBraceToken = (CloseBraceToken)parserModel.TokenWalker.Match(SyntaxKind.CloseBraceToken);
		
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif
	}
	
	public IExpressionNode ParseMemberAccessToken(IExpressionNode expressionPrimary, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		var rememberOriginalExpressionPrimary = expressionPrimary;
		var rememberOriginalTokenIndex = parserModel.TokenWalker.Index;
		
		if (!UtilityApi.IsConvertibleToIdentifierToken(parserModel.TokenWalker.Next.SyntaxKind))
			return ParseMemberAccessToken_Fallback(rememberOriginalTokenIndex, rememberOriginalExpressionPrimary, token, compilationUnit, ref parserModel);

		_ = parserModel.TokenWalker.Consume(); // Consume the 'MemberAccessToken'
		
		var memberIdentifierToken = UtilityApi.ConvertToIdentifierToken(
			parserModel.TokenWalker.Consume(),
			compilationUnit,
			ref parserModel);
			
		if (!memberIdentifierToken.ConstructorWasInvoked || memberIdentifierToken.TextSpan.SourceText is null)
			return ParseMemberAccessToken_Fallback(rememberOriginalTokenIndex, rememberOriginalExpressionPrimary, token, compilationUnit, ref parserModel);
		
		if (expressionPrimary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)expressionPrimary;
			if (!ambiguousIdentifierExpressionNode.FollowsMemberAccessToken)
			{
				expressionPrimary = ForceDecisionAmbiguousIdentifier(
					EmptyExpressionNode.Empty,
					ambiguousIdentifierExpressionNode,
					compilationUnit,
					ref parserModel);
			}
		}
	
		TypeClauseNode? typeClauseNode = null;
	
		if (expressionPrimary.SyntaxKind == SyntaxKind.VariableReferenceNode)
			typeClauseNode = ((VariableReferenceNode)expressionPrimary).VariableDeclarationNode?.TypeClauseNode;
		else if (expressionPrimary.SyntaxKind == SyntaxKind.TypeClauseNode)
			typeClauseNode = (TypeClauseNode)expressionPrimary;
		
		if (typeClauseNode is null)
			return ParseMemberAccessToken_Fallback(rememberOriginalTokenIndex, rememberOriginalExpressionPrimary, token, compilationUnit, ref parserModel);
		
		var maybeTypeDefinitionNode = GetDefinitionNode(compilationUnit, typeClauseNode.TypeIdentifierToken.TextSpan, SyntaxKind.TypeClauseNode);
		if (maybeTypeDefinitionNode is null || maybeTypeDefinitionNode.SyntaxKind != SyntaxKind.TypeDefinitionNode)
			return ParseMemberAccessToken_Fallback(rememberOriginalTokenIndex, rememberOriginalExpressionPrimary, token, compilationUnit, ref parserModel);
			
		var typeDefinitionNode = (TypeDefinitionNode)maybeTypeDefinitionNode;
		var memberList = typeDefinitionNode.GetMemberList();
		ISyntaxNode? foundDefinitionNode = null;
		
		foreach (var node in memberList)
		{
			if (node.SyntaxKind == SyntaxKind.VariableDeclarationNode)
			{
				var variableDeclarationNode = (VariableDeclarationNode)node;
				if (!variableDeclarationNode.IdentifierToken.ConstructorWasInvoked || variableDeclarationNode.IdentifierToken.TextSpan.SourceText is null)
					continue;
				
				if (variableDeclarationNode.IdentifierToken.TextSpan.GetText() == memberIdentifierToken.TextSpan.GetText())
				{
					foundDefinitionNode = variableDeclarationNode;
					break;
				}
			}
		}
		
		if (foundDefinitionNode is null)
			 return ParseMemberAccessToken_Fallback(rememberOriginalTokenIndex, rememberOriginalExpressionPrimary, token, compilationUnit, ref parserModel);
			 
		if (foundDefinitionNode.SyntaxKind == SyntaxKind.VariableDeclarationNode)
		{
			var variableDeclarationNode = (VariableDeclarationNode)foundDefinitionNode;
			
			var variableReferenceNode = new VariableReferenceNode(
	            memberIdentifierToken,
	            variableDeclarationNode);
	        var symbolId = CreateVariableSymbol(variableReferenceNode.VariableIdentifierToken, variableDeclarationNode.VariableKind, compilationUnit);
	        
	        compilationUnit.BinderSession.SymbolIdToExternalTextSpanMap.TryAdd(
	        	symbolId,
	        	(variableDeclarationNode.IdentifierToken.TextSpan.ResourceUri, variableDeclarationNode.IdentifierToken.TextSpan.StartingIndexInclusive));
	        
	    	return variableReferenceNode;
		}
	
		return ParseMemberAccessToken_Fallback(rememberOriginalTokenIndex, rememberOriginalExpressionPrimary, token, compilationUnit, ref parserModel);
	}
	
	/// <summary>
	/// New code is being written in <see cref="ParseMemberAccessToken"/>
	/// that will bind member access expressions like:
	/// 'person.FirstName'
	/// where 'person' is an instance of the type 'Person',
	/// and 'Person' is a type that has a property 'FirstName'.
	///
	/// In the example the 'person.FirstName' if the cursor hovers 'FirstName'
	/// a tooltip should indicate that it is a property and that it can be goto definitioned.
	///
	/// goto definition here would take the user to the property definition within the type definition.
	///
	/// -----------------------------------------------------------------------------------------------
	///
	/// While this code is being written, this method will contain the previous code, and any
	/// "bad cases" will "fallback" to the old code (i.e.: this method) instead of
	/// running the new code.
	///
	/// Then the easy cases can be wired to the new code, and the new code can build up
	/// to handle cases, rather than have to immediately deal with every edge case.
	/// </summary>
	public IExpressionNode ParseMemberAccessToken_Fallback(int rememberOriginalTokenIndex, IExpressionNode expressionPrimary, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken &&
			parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenAngleBracketToken)
		{
			return EmptyExpressionNode.EmptyFollowsMemberAccessToken;
		}
		
		if (parserModel.TokenWalker.Index == 2 + rememberOriginalTokenIndex)
		{
			// If the new code consumed, undo that.
			_ = parserModel.TokenWalker.Backtrack();
			_ = parserModel.TokenWalker.Backtrack();
		}
		
		return EmptyExpressionNode.Empty;
	}
	
	private IExpressionNode AmbiguousParenthesizedExpressionTransformTo_ParenthesizedExpressionNode(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		var parenthesizedExpressionNode = new ParenthesizedExpressionNode(
			ambiguousParenthesizedExpressionNode.OpenParenthesisToken,
			CSharpFacts.Types.Void.ToTypeClause());
			
		parenthesizedExpressionNode.SetInnerExpression(expressionSecondary);
			
		parserModel.NoLongerRelevantExpressionNode = ambiguousParenthesizedExpressionNode;
		
		if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.CloseParenthesisToken)
			parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, parenthesizedExpressionNode));
			
		return parenthesizedExpressionNode;
	}
	
	private IExpressionNode AmbiguousParenthesizedExpressionTransformTo_TupleExpressionNode(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, IExpressionNode? expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		var tupleExpressionNode = new TupleExpressionNode();
			
		foreach (var node in ambiguousParenthesizedExpressionNode.NodeList)
		{
			if (node is IExpressionNode expressionNode)
			{
				// (x, y) => 3; # Lambda expression node
				// (x, 2);      # At first appeared to be Lambda expression node, but is actually tuple expression node.
				if (expressionNode.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
				{
					expressionNode = ForceDecisionAmbiguousIdentifier(
						EmptyExpressionNode.Empty,
						(AmbiguousIdentifierExpressionNode)expressionNode,
						compilationUnit,
						ref parserModel);
				}
				
				tupleExpressionNode.AddInnerExpressionNode(expressionNode);
			}
		}
		
		if (expressionSecondary is not null)
			tupleExpressionNode.AddInnerExpressionNode(expressionSecondary);
		
		parserModel.NoLongerRelevantExpressionNode = ambiguousParenthesizedExpressionNode;
		
		if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.CloseParenthesisToken)
		{
			parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, tupleExpressionNode));
			parserModel.ExpressionList.Add((SyntaxKind.CommaToken, tupleExpressionNode));
		}
			
		return tupleExpressionNode;
	}
	
	private IExpressionNode AmbiguousParenthesizedExpressionTransformTo_ExplicitCastNode(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, IExpressionNode expressionNode, CloseParenthesisToken closeParenthesisToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		ISyntax syntax;
	
		if (expressionNode.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
			syntax = ((AmbiguousIdentifierExpressionNode)expressionNode).Token;
		else if (expressionNode.SyntaxKind == SyntaxKind.TypeClauseNode)
			syntax = expressionNode;
		else if (expressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
			syntax = ((VariableReferenceNode)expressionNode).VariableIdentifierToken;
		else
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), ambiguousParenthesizedExpressionNode, expressionNode);
	
		var typeClauseNode = UtilityApi.ConvertToTypeClauseNode(
			syntax,
			compilationUnit,
			ref parserModel);
			
		BindTypeClauseNode(typeClauseNode, compilationUnit);
		
		var explicitCastNode = new ExplicitCastNode(ambiguousParenthesizedExpressionNode.OpenParenthesisToken, typeClauseNode, closeParenthesisToken);
		return explicitCastNode;
	}
	
	private IExpressionNode AmbiguousParenthesizedExpressionTransformTo_TypeClauseNode(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, ISyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		var identifierToken = new IdentifierToken(
			new TextEditorTextSpan(
			    ambiguousParenthesizedExpressionNode.OpenParenthesisToken.TextSpan.StartingIndexInclusive,
			    token.TextSpan.EndingIndexExclusive,
			    default(byte),
			    token.TextSpan.ResourceUri,
			    token.TextSpan.SourceText));
		
		return new TypeClauseNode(
			identifierToken,
	        valueType: null,
	        genericParametersListingNode: null);
	}
	
	private IExpressionNode AmbiguousParenthesizedExpressionTransformTo_LambdaExpressionNode(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
					
		if (ambiguousParenthesizedExpressionNode.NodeList is not null)
		{
			if (ambiguousParenthesizedExpressionNode.NodeList.Count >= 1)
			{
				foreach (var node in ambiguousParenthesizedExpressionNode.NodeList)
				{
					ISyntax syntax;
					
					if (node.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
						syntax = ((AmbiguousIdentifierExpressionNode)node).Token;
					else if (node.SyntaxKind == SyntaxKind.TypeClauseNode)
						syntax = ((TypeClauseNode)node).TypeIdentifierToken;
					else if (node.SyntaxKind == SyntaxKind.VariableReferenceNode)
						syntax = ((VariableReferenceNode)node).VariableIdentifierToken;
					else
						return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), ambiguousParenthesizedExpressionNode, node);
				
					var variableDeclarationNode = new VariableDeclarationNode(
				        TypeFacts.Empty.ToTypeClause(),
				        UtilityApi.ConvertToIdentifierToken(syntax, compilationUnit, ref parserModel),
				        VariableKind.Local,
				        false);
				        
		    		lambdaExpressionNode.AddVariableDeclarationNode(variableDeclarationNode);
				}
			}
		}
		else if (ambiguousParenthesizedExpressionNode.NodeList is not null)
		{
			if (ambiguousParenthesizedExpressionNode.NodeList.Count >= 1)
			{
				foreach (var node in ambiguousParenthesizedExpressionNode.NodeList)
				{
					if (node.SyntaxKind == SyntaxKind.VariableDeclarationNode)
					{
						lambdaExpressionNode.AddVariableDeclarationNode((VariableDeclarationNode)node);
					}
					else
					{
						Console.WriteLine("aaa went else branch of -> if (node.SyntaxKind == SyntaxKind.VariableDeclarationNode)");
					}
				}
			}
		}
		
		// If the lambda expression's code block is a single expression then there is no end delimiter.
		// Instead, it is the parent expression's delimiter that causes the lambda expression's code block to short circuit.
		// At this moment, the lambda expression is given whatever expression was able to be parsed and can take it as its "code block".
		// And then restore the parent expression as the expressionPrimary.
		//
		// -----------------------------------------------------------------------------------------------------------------------------
		//
		// If the lambda expression's code block is deliminated by braces
		// then the end delimiter is the CloseBraceToken.
		// But, we can only add a "short circuit" for 'CloseBraceToken and lambdaExpressionNode'
		// if we have seen the 'OpenBraceToken'.
		
		parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, lambdaExpressionNode));
		
		if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenBraceToken)
		{
			parserModel.ExpressionList.Add((SyntaxKind.CloseBraceToken, lambdaExpressionNode));
			OpenLambdaExpressionScope(lambdaExpressionNode, (OpenBraceToken)parserModel.TokenWalker.Next, compilationUnit, ref parserModel);
			SkipLambdaExpressionStatements(lambdaExpressionNode, compilationUnit, ref parserModel);
		}
		else
		{
			OpenLambdaExpressionScope(lambdaExpressionNode, new OpenBraceToken(parserModel.TokenWalker.Next.TextSpan), compilationUnit, ref parserModel);
		}
		
		return EmptyExpressionNode.Empty;
	}
}
