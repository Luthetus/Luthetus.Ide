using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.Facts;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.BinderCase;

public partial class CSharpBinder
{
	/// <summary>
	/// Returns the new primary expression which will be 'BadExpressionNode'
	/// if the parameters were not mergeable.
	/// </summary>
	public IExpressionNode AnyMergeToken(
		IExpressionNode expressionPrimary, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		/*#if DEBUG
		if (expressionPrimary.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			var variableReferenceNode = (VariableReferenceNode)expressionPrimary;
			Console.Write($"{variableReferenceNode.VariableIdentifierToken.TextSpan.GetText()}____");
		}
		Console.WriteLine($"{expressionPrimary.SyntaxKind} + {token.SyntaxKind}:{parserModel.TokenWalker.Index}");
		#else
		Console.WriteLine($"{nameof(AnyMergeToken)} has debug 'Console.Write...' that needs commented out.");
		#endif*/
		
		if (parserModel.ParserContextKind != CSharpParserContextKind.ForceParseGenericParameters &&
			UtilityApi.IsBinaryOperatorSyntaxKind(token.SyntaxKind))
		{
			return HandleBinaryOperator(expressionPrimary, ref token, compilationUnit, ref parserModel);
		}
		
		switch (expressionPrimary.SyntaxKind)
		{
			case SyntaxKind.EmptyExpressionNode:
				return EmptyMergeToken((EmptyExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.LiteralExpressionNode:
				return LiteralMergeToken((LiteralExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.InterpolatedStringNode:
				return InterpolatedStringMergeToken((InterpolatedStringNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.FunctionDefinitionNode:
				var functionDefinitionNode = (FunctionDefinitionNode)expressionPrimary;
				if (functionDefinitionNode.IsParsingGenericParameters)
					return GenericParametersListingMergeToken(functionDefinitionNode, ref token, compilationUnit, ref parserModel);
				else
					return FunctionDefinitionMergeToken(functionDefinitionNode, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.ConstructorDefinitionNode:
			case SyntaxKind.TypeDefinitionNode:
				return FunctionDefinitionMergeToken((IFunctionDefinitionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.BinaryExpressionNode:
				return BinaryMergeToken((BinaryExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.ParenthesizedExpressionNode:
				return ParenthesizedMergeToken((ParenthesizedExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.FunctionInvocationNode:
				return FunctionInvocationMergeToken((FunctionInvocationNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.LambdaExpressionNode:
				return LambdaMergeToken((LambdaExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.ConstructorInvocationExpressionNode:
				return ConstructorInvocationMergeToken((ConstructorInvocationExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.WithExpressionNode:
				return WithMergeToken((WithExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.ExplicitCastNode:
				return ExplicitCastMergeToken((ExplicitCastNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.TupleExpressionNode:
				return TupleMergeToken((TupleExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.AmbiguousParenthesizedExpressionNode:
				return AmbiguousParenthesizedMergeToken((AmbiguousParenthesizedExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.AmbiguousIdentifierExpressionNode:
				return AmbiguousIdentifierMergeToken((AmbiguousIdentifierExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.TypeClauseNode:
				return TypeClauseMergeToken((TypeClauseNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.ReturnStatementNode:
				return ReturnStatementMergeToken((ReturnStatementNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.BadExpressionNode:
				return BadMergeToken((BadExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), expressionPrimary, token);
		};
	}
	
	/// <summary>
	/// Returns the new primary expression which will be 'BadExpressionNode'
	/// if the parameters were not mergeable.
	/// </summary>
	public IExpressionNode AnyMergeExpression(
		IExpressionNode expressionPrimary, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		/*#if DEBUG
		Console.WriteLine($"{expressionPrimary.SyntaxKind} + {expressionSecondary.SyntaxKind}");
		#else
		Console.WriteLine($"{nameof(AnyMergeExpression)} has debug 'Console.Write...' that needs commented out.");
		#endif*/
	
		switch (expressionPrimary.SyntaxKind)
		{
			case SyntaxKind.BinaryExpressionNode:
				return BinaryMergeExpression((BinaryExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.InterpolatedStringNode:
				return InterpolatedStringMergeExpression((InterpolatedStringNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.FunctionDefinitionNode:
				var functionDefinitionNode = (FunctionDefinitionNode)expressionPrimary;
				if (functionDefinitionNode.IsParsingGenericParameters)
				{
					return GenericParametersListingMergeExpression(
						functionDefinitionNode, expressionSecondary, compilationUnit, ref parserModel);
				}
				return FunctionDefinitionMergeExpression(functionDefinitionNode, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.ConstructorDefinitionNode:
			case SyntaxKind.TypeDefinitionNode:
				return FunctionDefinitionMergeExpression((IFunctionDefinitionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.ParenthesizedExpressionNode:
				return ParenthesizedMergeExpression((ParenthesizedExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.FunctionInvocationNode:
				return FunctionInvocationMergeExpression((FunctionInvocationNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.LambdaExpressionNode:
				return LambdaMergeExpression((LambdaExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.ConstructorInvocationExpressionNode:
				return ConstructorInvocationMergeExpression((ConstructorInvocationExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.WithExpressionNode:
				return WithMergeExpression((WithExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.TupleExpressionNode:
				return TupleMergeExpression((TupleExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.AmbiguousParenthesizedExpressionNode:
				return AmbiguousParenthesizedMergeExpression((AmbiguousParenthesizedExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.AmbiguousIdentifierExpressionNode:
				return AmbiguousIdentifierMergeExpression((AmbiguousIdentifierExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.TypeClauseNode:
				return TypeClauseMergeExpression((TypeClauseNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.ReturnStatementNode:
				return ReturnStatementMergeExpression((ReturnStatementNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			case SyntaxKind.BadExpressionNode:
				return BadMergeExpression((BadExpressionNode)expressionPrimary, expressionSecondary, compilationUnit, ref parserModel);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), expressionPrimary, expressionSecondary);
		};
	}
	
	public IExpressionNode HandleBinaryOperator(
		IExpressionNode expressionPrimary, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		// TODO: MemberAccessToken should be treated the same as any other operator.
		//       This feels very "special case" the way it is written.
		//       This seems similar to the most precedence being assigned to it.
		if (token.SyntaxKind == SyntaxKind.MemberAccessToken)
			return ParseMemberAccessToken(expressionPrimary, ref token, compilationUnit, ref parserModel);
	
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
			return TypeClauseMergeToken((TypeClauseNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
		}
		
		// TODO: This isn't great. The ConstructorInvocationExpressionNode after reading 'new'...
		// if then has to read the TypeClauseNode, it actually does this inside of the 'ConstructorInvocationExpressionNode'.
		// It is sort of duplicated logic, and this case for the 'TypeClauseNode' needs repeating too.
		if (expressionPrimary.SyntaxKind == SyntaxKind.ConstructorInvocationExpressionNode &&
			(token.SyntaxKind == SyntaxKind.OpenAngleBracketToken || token.SyntaxKind == SyntaxKind.CloseAngleBracketToken))
		{
			return ConstructorInvocationMergeToken((ConstructorInvocationExpressionNode)expressionPrimary, ref token, compilationUnit, ref parserModel);
		}
		
		var expressionAntecedent = GetParentNode(expressionPrimary, compilationUnit, ref parserModel);
		if (expressionAntecedent.SyntaxKind == SyntaxKind.BinaryExpressionNode)
		{
			var binaryExpressionAntecedent = (BinaryExpressionNode)expressionAntecedent;
			
			var precedenceAntecedent = UtilityApi.GetOperatorPrecedence(binaryExpressionAntecedent.OperatorToken.SyntaxKind);
			var precedencePrecedent = UtilityApi.GetOperatorPrecedence(token.SyntaxKind);
			
			if (binaryExpressionAntecedent.RightExpressionNode.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			{
				if (precedenceAntecedent >= precedencePrecedent)
				{
					// Antecedent takes 'primaryExpression' as its right node.
		            // Precedent takes antecedent as its left node.
		            // Precedent becomes "subtree-root".
					binaryExpressionAntecedent.RightExpressionNode = expressionPrimary;
					
					var typeClauseNode = expressionPrimary.ResultTypeReference;
					var binaryExpressionPrecedent = new BinaryExpressionNode(binaryExpressionAntecedent, typeClauseNode, token, typeClauseNode, typeClauseNode);
					
					// It is important that the primitive recursion does not
		            // set 'binaryExpressionAntecedent' as the primaryExpression in the future
		            // because it is now the left node of 'binaryExpressionPrecedent'.
					ClearFromExpressionList(binaryExpressionAntecedent, compilationUnit, ref parserModel);
					
					parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, binaryExpressionPrecedent));
					
					return EmptyExpressionNode.Empty;
				}
				else
				{
					// Precedent takes 'primaryExpression' as its left node.
	            	// Antecedent takes precedent as its right node.
					var typeClauseNode = expressionPrimary.ResultTypeReference;
					var binaryExpressionNodePrecedent = new BinaryExpressionNode(expressionPrimary, typeClauseNode, token, typeClauseNode, typeClauseNode);
					
					binaryExpressionAntecedent.RightExpressionNode = binaryExpressionNodePrecedent;
					
					parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, binaryExpressionNodePrecedent));
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
				ClearFromExpressionList(expressionPrimary, compilationUnit, ref parserModel);
				ClearFromExpressionList(binaryExpressionAntecedent, compilationUnit, ref parserModel);
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), binaryExpressionAntecedent, expressionPrimary)
				{
					ClobberCount = 1
				};
				
				// TODO: The new constructor for 'BadExpressionNode' doesn't take a List, I can only choose 2 syntax to provide now...
				// ...and yet this previous constructor was giving the List 3 syntax.
				// I'm going to take the first two for now, but if it ever feels like information is missing here.
				// It might be due to the token having disappeared from existence.
				//
				// return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), new List<ISyntax> { binaryExpressionAntecedent, expressionPrimary, token });
			}
		}
		else
		{
			var typeClauseNode = expressionPrimary.ResultTypeReference;
			var binaryExpressionNode = new BinaryExpressionNode(expressionPrimary, typeClauseNode, token, typeClauseNode, typeClauseNode);
			
			parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, binaryExpressionNode));
			return EmptyExpressionNode.Empty;
		}
	}

	public IExpressionNode AmbiguousParenthesizedMergeToken(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
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
			parserModel.ParserContextKind = CSharpParserContextKind.None;
		
			if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
			{
				return AmbiguousParenthesizedExpressionTransformTo_LambdaExpressionNode(ambiguousParenthesizedExpressionNode, compilationUnit, ref parserModel);
			}
			else if (ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes is null)
			{
				var parenthesizedExpressionNode = new ParenthesizedExpressionNode(
					ambiguousParenthesizedExpressionNode.OpenParenthesisToken,
					CSharpFacts.Types.Void.ToTypeReference());
					
				parserModel.NoLongerRelevantExpressionNode = ambiguousParenthesizedExpressionNode;
					
				return parenthesizedExpressionNode;
			}
			else if (ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes.Value &&
					 ambiguousParenthesizedExpressionNode.NodeList.Count >= 1)
			{
				return AmbiguousParenthesizedExpressionTransformTo_TypeClauseNode(ambiguousParenthesizedExpressionNode, ref token, compilationUnit, ref parserModel);
			}
			else if (!ambiguousParenthesizedExpressionNode.ShouldMatchVariableDeclarationNodes.Value)
			{
				if (ambiguousParenthesizedExpressionNode.NodeList.Count > 1)
				{
					if (ambiguousParenthesizedExpressionNode.IsParserContextKindForceStatementExpression ||
						ambiguousParenthesizedExpressionNode.NodeList.All(node => node.SyntaxKind == SyntaxKind.TypeClauseNode))
					{
						return AmbiguousParenthesizedExpressionTransformTo_TypeClauseNode(ambiguousParenthesizedExpressionNode, ref token, compilationUnit, ref parserModel);
					}
					
					return AmbiguousParenthesizedExpressionTransformTo_TupleExpressionNode(ambiguousParenthesizedExpressionNode, expressionSecondary: null, compilationUnit, ref parserModel);
				}
				else if (ambiguousParenthesizedExpressionNode.NodeList.Count == 1 &&
						 UtilityApi.IsConvertibleToTypeClauseNode(ambiguousParenthesizedExpressionNode.NodeList[0].SyntaxKind) ||
						 ambiguousParenthesizedExpressionNode.NodeList[0].SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode ||
						 ambiguousParenthesizedExpressionNode.NodeList[0].SyntaxKind == SyntaxKind.VariableReferenceNode)
				{
					return AmbiguousParenthesizedExpressionTransformTo_ExplicitCastNode(
						ambiguousParenthesizedExpressionNode, (IExpressionNode)ambiguousParenthesizedExpressionNode.NodeList[0], ref token, compilationUnit, ref parserModel);
				}
			}
		}
		
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), ambiguousParenthesizedExpressionNode, token);
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
				
				if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
				{
					var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)expressionSecondary;
					expressionSecondary = new AmbiguousIdentifierExpressionNode(
						ambiguousIdentifierExpressionNode.Token,
						ambiguousIdentifierExpressionNode.GenericParameterListing,
						ambiguousIdentifierExpressionNode.ResultTypeReference)
					{
						FollowsMemberAccessToken = ambiguousIdentifierExpressionNode.FollowsMemberAccessToken
					};
				}
				
				ambiguousParenthesizedExpressionNode.NodeList.Add(expressionSecondary);
				return ambiguousParenthesizedExpressionNode;
			case SyntaxKind.AmbiguousParenthesizedExpressionNode:
				// The 'AmbiguousParenthesizedExpressionNode' merging with 'SyntaxToken' method will
				// return the existing 'AmbiguousParenthesizedExpressionNode' in various situations.
				//
				// One of which is to signify the closing brace token.
				return ambiguousParenthesizedExpressionNode;
		}
		
		// 'ambiguousParenthesizedExpressionNode.NodeList.Count > 0' because the current was never added,
		// so if there already is 1, then there'd be many expressions.
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken || ambiguousParenthesizedExpressionNode.NodeList.Count > 0)
		{
			return AmbiguousParenthesizedExpressionTransformTo_TupleExpressionNode(ambiguousParenthesizedExpressionNode, expressionSecondary, compilationUnit, ref parserModel);
		}
		else
		{
			if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
				return ambiguousParenthesizedExpressionNode; // '() => ...;
			else
				return AmbiguousParenthesizedExpressionTransformTo_ParenthesizedExpressionNode(ambiguousParenthesizedExpressionNode, expressionSecondary, compilationUnit, ref parserModel);
		}
	}
	
	public IExpressionNode AmbiguousIdentifierMergeToken(
		AmbiguousIdentifierExpressionNode ambiguousIdentifierExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (ambiguousIdentifierExpressionNode.IsParsingGenericParameters)
		{
			return GenericParametersListingMergeToken(
				ambiguousIdentifierExpressionNode, ref token, compilationUnit, ref parserModel);
		}
	
		switch (token.SyntaxKind)
		{
			case SyntaxKind.OpenParenthesisToken:
			{
				if (ambiguousIdentifierExpressionNode.Token.SyntaxKind == SyntaxKind.IdentifierToken)
				{
					// TODO: ContextualKeywords as the function identifier?
					var functionInvocationNode = new FunctionInvocationNode(
						ambiguousIdentifierExpressionNode.Token,
				        functionDefinitionNode: null,
				        ambiguousIdentifierExpressionNode.GenericParameterListing,
				        new FunctionParameterListing(
							token,
					        new List<FunctionParameterEntry>(),
					        closeParenthesisToken: default),
				        CSharpFacts.Types.Void.ToTypeReference());
				    
				    BindFunctionInvocationNode(
				        functionInvocationNode,
				        compilationUnit,
				        ref parserModel);
					
					return ParseFunctionParameterListing_Start(
						functionInvocationNode, compilationUnit, ref parserModel);
				}
				
				goto default;
			}
			case SyntaxKind.OpenAngleBracketToken:
			{
				// TODO: As of (2024-12-21) is this conditional branch no longer hit?...
				//       ...The 'AmbiguousIdentifierExpressionNode' merging with 'OpenAngleBracketToken'
				//       code was moved to 'HandleBinaryOperator(...)'
				return ParseGenericParameterNode_Start(
					ambiguousIdentifierExpressionNode, ref token, compilationUnit, ref parserModel);
			}
			case SyntaxKind.CloseAngleBracketToken:
			{
				if (ambiguousIdentifierExpressionNode.GenericParameterListing.ConstructorWasInvoked)
				{
					ambiguousIdentifierExpressionNode.GenericParameterListing.SetCloseAngleBracketToken(token);
					return ambiguousIdentifierExpressionNode;
				}
			
				goto default;
			}
			case SyntaxKind.EqualsToken:
			{
				// TODO: Is this code ever hit?
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, ambiguousIdentifierExpressionNode));
				return EmptyExpressionNode.Empty;
			}
			case SyntaxKind.EqualsCloseAngleBracketToken:
			{
				var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeReference());
				SetLambdaExpressionNodeVariableDeclarationNodeList(lambdaExpressionNode, ambiguousIdentifierExpressionNode, compilationUnit, ref parserModel);
				
				SyntaxToken openBraceToken;
				
				if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenBraceToken)
					openBraceToken = parserModel.TokenWalker.Next;
				else
					openBraceToken = new SyntaxToken(SyntaxKind.OpenBraceToken, token.TextSpan);
				
				return ParseLambdaExpressionNode(lambdaExpressionNode, ref openBraceToken, compilationUnit, ref parserModel);
			}
			case SyntaxKind.IsTokenKeyword:
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
			case SyntaxKind.WithTokenContextualKeyword:
			{
				var decidedNode = ForceDecisionAmbiguousIdentifier(
					EmptyExpressionNode.Empty,
					ambiguousIdentifierExpressionNode,
					compilationUnit,
					ref parserModel);
					
				if (decidedNode.SyntaxKind != SyntaxKind.VariableReferenceNode)
					goto default;
				
				return new WithExpressionNode(
					new VariableReference((VariableReferenceNode)decidedNode));
			}
			case SyntaxKind.PlusPlusToken:
			{
				var decidedExpression = ForceDecisionAmbiguousIdentifier(
					EmptyExpressionNode.Empty,
					ambiguousIdentifierExpressionNode,
					compilationUnit,
					ref parserModel);
				
				goto default;
			}
			case SyntaxKind.MinusMinusToken:
			{
				var decidedExpression = ForceDecisionAmbiguousIdentifier(
					EmptyExpressionNode.Empty,
					ambiguousIdentifierExpressionNode,
					compilationUnit,
					ref parserModel);
					
				goto default;
			}
			case SyntaxKind.BangToken:
			case SyntaxKind.QuestionMarkToken:
			{
				if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.MemberAccessToken)
					return ambiguousIdentifierExpressionNode;
				
				if (token.SyntaxKind == SyntaxKind.QuestionMarkToken)
				{
					ambiguousIdentifierExpressionNode.HasQuestionMark = true;
					return ambiguousIdentifierExpressionNode;
				}
				
				goto default;
			}
			case SyntaxKind.IdentifierToken:
			{
				var decidedExpression = ForceDecisionAmbiguousIdentifier(
					EmptyExpressionNode.Empty,
					ambiguousIdentifierExpressionNode,
					compilationUnit,
					ref parserModel);
				
				if (decidedExpression.SyntaxKind != SyntaxKind.TypeClauseNode)
				{
					/*parserModel.DiagnosticBag.ReportTodoException(
			    		parserModel.TokenWalker.Current.TextSpan,
			    		"if (decidedExpression.SyntaxKind != SyntaxKind.TypeClauseNode)");*/
					return decidedExpression;
				}
			
				var identifierToken = parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
				
				var variableDeclarationNode = ParseVariables.HandleVariableDeclarationExpression(
			        (TypeClauseNode)decidedExpression,
			        identifierToken,
			        VariableKind.Local,
			        compilationUnit,
			        ref parserModel);
			        
			    return variableDeclarationNode;
			}
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), ambiguousIdentifierExpressionNode, token);
		}
	}
		
	public IExpressionNode AmbiguousIdentifierMergeExpression(
		AmbiguousIdentifierExpressionNode ambiguousIdentifierExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (ambiguousIdentifierExpressionNode.IsParsingGenericParameters)
		{
			return GenericParametersListingMergeExpression(
				ambiguousIdentifierExpressionNode, expressionSecondary, compilationUnit, ref parserModel);
		}
	
		if (ambiguousIdentifierExpressionNode.GenericParameterListing.ConstructorWasInvoked &&
			!ambiguousIdentifierExpressionNode.GenericParameterListing.CloseAngleBracketToken.ConstructorWasInvoked)
		{
			return ambiguousIdentifierExpressionNode;
		}
		
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), ambiguousIdentifierExpressionNode, expressionSecondary);
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
			    	compilationUnit.ResourceUri,
			    	parserModel.CurrentScopeIndexKey,
			        ambiguousIdentifierExpressionNode.Token.TextSpan.GetText(),
			        out var existingVariableDeclarationNode))
			{
				var token = ambiguousIdentifierExpressionNode.Token;
				var identifierToken = UtilityApi.ConvertToIdentifierToken(ref token, compilationUnit, ref parserModel);
				
				var variableReferenceNode = ConstructAndBindVariableReferenceNode(
					identifierToken,
					compilationUnit,
					ref parserModel);
    			
    			return variableReferenceNode;
			}
		}
		
		if (!forceVariableReferenceNode && UtilityApi.IsConvertibleToTypeClauseNode(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
		{
			if (TryGetTypeDefinitionHierarchically(
	        		compilationUnit,
	        		compilationUnit.ResourceUri,
	                parserModel.CurrentScopeIndexKey,
	                ambiguousIdentifierExpressionNode.Token.TextSpan.GetText(),
	                out var typeDefinitionNode))
	        {
	        	var token = ambiguousIdentifierExpressionNode.Token;
	        	
	        	TypeClauseNode typeClauseNode;
	        	
	        	if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
	        		typeClauseNode = UtilityApi.ConvertTokenToTypeClauseNode(ref token, compilationUnit, ref parserModel);
	        	else
	        		typeClauseNode = UtilityApi.ConvertTokenToTypeClauseNode(ref token, compilationUnit, ref parserModel);
	        	
	            
	            typeClauseNode.HasQuestionMark = ambiguousIdentifierExpressionNode.HasQuestionMark;
				BindTypeClauseNode(typeClauseNode, compilationUnit, ref parserModel);
				
				// FindAllReferences
				// BindTypeClauseNodeSuccessfully(typeClauseNode, typeDefinitionNode, compilationUnit, ref parserModel);
				
			    return typeClauseNode;
	        }
		}
		
		if (ambiguousIdentifierExpressionNode.Token.SyntaxKind == SyntaxKind.IdentifierToken &&
			ambiguousIdentifierExpressionNode.Token.TextSpan.Length == 1 &&
    		ambiguousIdentifierExpressionNode.Token.TextSpan.GetText() == "_")
    	{
    		if (!parserModel.Binder.TryGetVariableDeclarationHierarchically(
			    	compilationUnit,
			    	compilationUnit.ResourceUri,
			    	parserModel.CurrentScopeIndexKey,
			        ambiguousIdentifierExpressionNode.Token.TextSpan.GetText(),
			        out _))
			{
				parserModel.Binder.BindDiscard(ambiguousIdentifierExpressionNode.Token, compilationUnit, ref parserModel);
	    		return ambiguousIdentifierExpressionNode;
			}
    	}
    	
    	if (!forceVariableReferenceNode &&
    	    parserModel.ParserContextKind != CSharpParserContextKind.ForceParseNextIdentifierAsTypeClauseNode &&
    	    UtilityApi.IsConvertibleToIdentifierToken(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
		{
			if (TryGetFunctionHierarchically(
			    	compilationUnit,
			        compilationUnit.ResourceUri,
			    	parserModel.CurrentScopeIndexKey,
			        ambiguousIdentifierExpressionNode.Token.TextSpan.GetText(),
			        out var functionDefinitionNode))
	        {
	        	var token = ambiguousIdentifierExpressionNode.Token;
				var identifierToken = UtilityApi.ConvertToIdentifierToken(ref token, compilationUnit, ref parserModel);
				
				var functionInvocationNode = new FunctionInvocationNode(
					ambiguousIdentifierExpressionNode.Token,
			        functionDefinitionNode: null,
			        genericParameterListing: default,
			        functionParameterListing: default,
			        CSharpFacts.Types.Void.ToTypeReference());
				
				// TODO: Method groups
				BindFunctionInvocationNode(
			        functionInvocationNode,
			        compilationUnit,
			        ref parserModel);
    			
    			return functionInvocationNode;
	        }
		}
		
		if (allowFabricatedUndefinedNode)
		{
			// Bind an undefined-TypeClauseNode
			if (!forceVariableReferenceNode ||
				UtilityApi.IsConvertibleToTypeClauseNode(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
			{
				var token = ambiguousIdentifierExpressionNode.Token;
	            
	            TypeClauseNode typeClauseNode;
	            
	            if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
	        		typeClauseNode = UtilityApi.ConvertTokenToTypeClauseNode(ref token, compilationUnit, ref parserModel);
	        	else
	        		typeClauseNode = UtilityApi.ConvertTokenToTypeClauseNode(ref token, compilationUnit, ref parserModel);
	            
	            typeClauseNode.HasQuestionMark = ambiguousIdentifierExpressionNode.HasQuestionMark;
				BindTypeClauseNode(typeClauseNode, compilationUnit, ref parserModel);
			    return typeClauseNode;
			}
			
			// Bind an undefined-variable
			if (UtilityApi.IsConvertibleToIdentifierToken(ambiguousIdentifierExpressionNode.Token.SyntaxKind))
			{
				var token = ambiguousIdentifierExpressionNode.Token;
				var identifierToken = UtilityApi.ConvertToIdentifierToken(ref token, compilationUnit, ref parserModel);
				
				var variableReferenceNode = ConstructAndBindVariableReferenceNode(
					identifierToken,
					compilationUnit,
					ref parserModel);
				
				return variableReferenceNode;
			}
		}
		
		return ambiguousIdentifierExpressionNode;
	}
		
	public IExpressionNode BadMergeToken(
		BadExpressionNode badExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		// (2025-01-31)
		// ============
		// 'if (typeof(string))' is breaking any text parsed after it in 'CSharpBinder.Main.cs'.
		//
		// There is more to it than just 'if (typeof(string))',
		// the issue actually occurs due to two consecutive 'if (typeof(string))'.
		//
		// Because the parser can recover from this under certain conditions,
		// but the nested 'if (typeof(string))' in 'CSharpBinder.Main.cs' results
		// in plain text syntax highlighting for any code that appears in the remaining methods.
		//
		// The issue is that 'if (...)' adds to 'parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		//
		// This results in the expression loop returning (back to the statement loop) upon encountering an unmatched 'CloseParenthesisToken'.
		// But, 'typeof(string)' is not understood by the expression loop.
		//
		// It only will create a FunctionInvocationNode if the "function name is an IdentifierToken / convertible to an IdentifierToken".
		//
		// And the keyword 'typeof' cannot be converted to an IdentifierToken, so it makes a bad expression node.
		//
		// Following that, the bad expression node goes to merge with an 'OpenParenthesisToken',
		// and under normal circumstances an 'OpenParenthesisToken' would 'parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, ambiguousParenthesizedExpressionNode));'
		//
		// But, when merging with the 'bad expression node' the 'OpenParenthesisToken' does not do this.
		//
		// Thus, the statement loop picks back up at the first 'CloseParenthesisToken' of 'if (typeof(string))'
		// when it should've picked back up at the second 'CloseParenthesisToken'.
		//
		// The statement loop then goes on to presume that the first 'CloseParenthesisToken'
		// was the closing delimiter of the if statement's predicate.
		//
		// So it Matches a 'CloseParenthesisToken', then sets the next token to be the start of the if statement's code block.
		// But, that next token is another 'CloseParenthesisToken'.
		//
		// From here it is presumed that errors start to cascade, and therefore the details are only relevant if wanting to
		// add 'recovery' logic.
		//
		// I think the best 'recovery' logic for this would that an unmatched 'CloseBraceToken' should
		// return to the statement loop.
		//
		// But, as for a fix, the bad expression node needs to 'match' the Parenthesis tokens so that
		// the statement loop picks back up at the second 'CloseParenthesisToken'.
		// 
		if (token.SyntaxKind == SyntaxKind.OpenParenthesisToken)
			parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, badExpressionNode));
		
		badExpressionNode.ClobberCount++;
		return badExpressionNode;
	}

	public IExpressionNode BadMergeExpression(
		BadExpressionNode badExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		badExpressionNode.ClobberCount++;
		return badExpressionNode;
	}

	public IExpressionNode BinaryMergeToken(
		BinaryExpressionNode binaryExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.NumericLiteralToken:
			case SyntaxKind.StringLiteralToken:
			case SyntaxKind.StringInterpolatedStartToken:
			case SyntaxKind.CharLiteralToken:
			case SyntaxKind.FalseTokenKeyword:
			case SyntaxKind.TrueTokenKeyword:
				TypeReference tokenTypeReference;
				
				if (token.SyntaxKind == SyntaxKind.NumericLiteralToken)
					tokenTypeReference = CSharpFacts.Types.Int.ToTypeReference();
				else if (token.SyntaxKind == SyntaxKind.StringLiteralToken || token.SyntaxKind == SyntaxKind.StringInterpolatedStartToken)
					tokenTypeReference = CSharpFacts.Types.String.ToTypeReference();
				else if (token.SyntaxKind == SyntaxKind.CharLiteralToken)
					tokenTypeReference = CSharpFacts.Types.Char.ToTypeReference();
				else if (token.SyntaxKind == SyntaxKind.FalseTokenKeyword || token.SyntaxKind == SyntaxKind.TrueTokenKeyword)
					tokenTypeReference = CSharpFacts.Types.Bool.ToTypeReference();
				else
					goto default;
					
				var tokenTypeReferenceText = tokenTypeReference.TypeIdentifierToken.TextSpan.GetText();
			
				var leftExpressionTypeClauseNodeText = binaryExpressionNode.LeftExpressionNode.ResultTypeReference.TypeIdentifierToken.TextSpan.GetText();
				if (leftExpressionTypeClauseNodeText != tokenTypeReferenceText)
					goto default;
				
				IExpressionNode rightExpressionNode;
					
				if (token.SyntaxKind == SyntaxKind.StringInterpolatedStartToken)
				{
					rightExpressionNode = new InterpolatedStringNode(
						token,
				    	stringInterpolatedEndToken: default,
				    	toBeExpressionPrimary: binaryExpressionNode,
				    	resultTypeReference: CSharpFacts.Types.String.ToTypeReference());
				}
				else
				{
					rightExpressionNode = new LiteralExpressionNode(token, tokenTypeReference);
				}
				
				binaryExpressionNode.RightExpressionNode = rightExpressionNode;
				
				if (token.SyntaxKind == SyntaxKind.StringInterpolatedStartToken)
				{
					// Awkwardly double checking the 'token.SyntaxKind' here to avoid duplicating 'binaryExpressionNode.SetRightExpressionNode(rightExpressionNode);'
					return ParseInterpolatedStringNode((InterpolatedStringNode)rightExpressionNode, compilationUnit, ref parserModel);
				}
				
				return binaryExpressionNode;
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.StarToken:
		    case SyntaxKind.DivisionToken:
		    case SyntaxKind.EqualsEqualsToken:
				// TODO: More generally, the result will be a number, so all that matters is what operators a number can interact with instead of duplicating this code.
				// RETROSPECTIVE: This code reads like nonsense to me. Shouldn't you check '==' not '!='? This 'if' is backwards?
				if (binaryExpressionNode.RightExpressionNode.SyntaxKind != SyntaxKind.EmptyExpressionNode)
	    		{
	    			var typeClauseNode = binaryExpressionNode.ResultTypeReference;
    				return new BinaryExpressionNode(binaryExpressionNode, typeClauseNode, token, typeClauseNode, typeClauseNode, EmptyExpressionNode.Empty);
	    		}
	    		else
	    		{
	    			goto default;
	    		}
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), binaryExpressionNode, token);
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
				
			binaryExpressionNode.RightExpressionNode = expressionSecondary;
			
			return binaryExpressionNode;
		}
	
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), binaryExpressionNode, expressionSecondary);
	}
	
	public IExpressionNode ConstructorInvocationMergeToken(
		ConstructorInvocationExpressionNode constructorInvocationExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (constructorInvocationExpressionNode.IsParsingFunctionParameters)
		{
			return ParseFunctionParameterListing_Token(
				constructorInvocationExpressionNode, ref token, compilationUnit, ref parserModel);
		}
		
		switch (token.SyntaxKind)
		{
			// TODO: This is wrong, it doesn't account for contextual keywords.
			case SyntaxKind.IdentifierToken:
				if (constructorInvocationExpressionNode.ResultTypeReference == default)
				{
					if (UtilityApi.IsConvertibleToTypeClauseNode(token.SyntaxKind))
					{
						_ = parserModel.TokenWalker.Consume(); // Consume the IdentifierToken
						var typeClauseNode = UtilityApi.ConvertTokenToTypeClauseNode(ref token, compilationUnit, ref parserModel);
						
						BindTypeClauseNode(
					        typeClauseNode,
					        compilationUnit,
					        ref parserModel);
						
						if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenAngleBracketToken)
						{
							constructorInvocationExpressionNode.ResultTypeReference = new TypeReference(typeClauseNode);
							return constructorInvocationExpressionNode;
						}
						
						constructorInvocationExpressionNode.ConstructorInvocationStageKind = ConstructorInvocationStageKind.GenericParameters;
						var openAngleBracketToken = parserModel.TokenWalker.Consume();
						
						return ParseGenericParameterNode_Start(
							typeClauseNode, ref openAngleBracketToken, compilationUnit, ref parserModel, nodeToRestoreAtCloseAngleBracketToken: constructorInvocationExpressionNode);
					}
				}
				
				goto default;
			case SyntaxKind.OpenParenthesisToken:
			    constructorInvocationExpressionNode.FunctionParameterListing = new FunctionParameterListing(
					token,
			        new List<FunctionParameterEntry>(),
			        closeParenthesisToken: default);
				
				constructorInvocationExpressionNode.ConstructorInvocationStageKind = ConstructorInvocationStageKind.FunctionParameters;
				
				return ParseFunctionParameterListing_Start(constructorInvocationExpressionNode, compilationUnit, ref parserModel);
			case SyntaxKind.CloseParenthesisToken:
				if (constructorInvocationExpressionNode.FunctionParameterListing.ConstructorWasInvoked)
				{
					constructorInvocationExpressionNode.FunctionParameterListing.SetCloseParenthesisToken(token);
					return constructorInvocationExpressionNode;
				}
				else
				{
					goto default;
				}
			case SyntaxKind.CloseAngleBracketToken:
				constructorInvocationExpressionNode.ConstructorInvocationStageKind = ConstructorInvocationStageKind.Unset;
				return constructorInvocationExpressionNode;
			case SyntaxKind.OpenBraceToken:				
				constructorInvocationExpressionNode.ConstructorInvocationStageKind = ConstructorInvocationStageKind.ObjectInitializationParameters;
				parserModel.ExpressionList.Add((SyntaxKind.CloseBraceToken, constructorInvocationExpressionNode));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode));
				return ParseObjectInitialization(constructorInvocationExpressionNode, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.CloseBraceToken:
				if (constructorInvocationExpressionNode.ConstructorInvocationStageKind == ConstructorInvocationStageKind.ObjectInitializationParameters)
				{
					constructorInvocationExpressionNode.ConstructorInvocationStageKind = ConstructorInvocationStageKind.Unset;
					return constructorInvocationExpressionNode;
				}
				
				goto default;
			case SyntaxKind.CommaToken:
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, constructorInvocationExpressionNode));
				return ParseObjectInitialization(constructorInvocationExpressionNode, ref token, compilationUnit, ref parserModel);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), constructorInvocationExpressionNode, token);
		}
	}
	
	public IExpressionNode ConstructorInvocationMergeExpression(
		ConstructorInvocationExpressionNode constructorInvocationExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (constructorInvocationExpressionNode.IsParsingFunctionParameters)
		{
			return ParseFunctionParameterListing_Expression(
				constructorInvocationExpressionNode, expressionSecondary, compilationUnit, ref parserModel);
		}
	
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionSecondary = ForceDecisionAmbiguousIdentifier(
				constructorInvocationExpressionNode,
				(AmbiguousIdentifierExpressionNode)expressionSecondary,
				compilationUnit,
				ref parserModel);
		}
	
		if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			return constructorInvocationExpressionNode;
			
		switch (constructorInvocationExpressionNode.ConstructorInvocationStageKind)
		{
			case ConstructorInvocationStageKind.GenericParameters:
				if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseAngleBracketToken &&
					expressionSecondary is TypeClauseNode typeClauseNode)
				{
					typeClauseNode.GenericParameterListing.SetCloseAngleBracketToken(parserModel.TokenWalker.Current);
					constructorInvocationExpressionNode.ResultTypeReference = new TypeReference(typeClauseNode);
					return constructorInvocationExpressionNode;
				}
				
				goto default;
			case ConstructorInvocationStageKind.FunctionParameters:
				if (constructorInvocationExpressionNode.FunctionParameterListing.ConstructorWasInvoked)
					return constructorInvocationExpressionNode;
				goto default;
			case ConstructorInvocationStageKind.ObjectInitializationParameters:
				return constructorInvocationExpressionNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), constructorInvocationExpressionNode, expressionSecondary);
		}
	}
	
	/// <summary>
	/// CurrentToken is to either be 'OpenBraceToken', or 'CommaToken' when invoking this method.
	/// </summary>
	public IExpressionNode ParseObjectInitialization(
		ConstructorInvocationExpressionNode constructorInvocationExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{	
		// Consume either 'OpenBraceToken', or 'CommaToken'
		_ = parserModel.TokenWalker.Consume();
		
		if (constructorInvocationExpressionNode.ResultTypeReference == default)
			constructorInvocationExpressionNode.ResultTypeReference = parserModel.MostRecentLeftHandSideAssignmentExpressionTypeClauseNode;
		
		if (UtilityApi.IsConvertibleToIdentifierToken(parserModel.TokenWalker.Current.SyntaxKind) &&
		    parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.EqualsToken)
		{
			var memberAccessToken = new SyntaxToken(
				SyntaxKind.MemberAccessToken,
				new TextEditorTextSpan(
					0,
				    0,
				    0,
				    token.TextSpan.ResourceUri,
				    string.Empty,
				    string.Empty))
				{
					IsFabricated = true
				};
		
			return ParseMemberAccessToken(new TypeClauseNode(constructorInvocationExpressionNode.ResultTypeReference), ref memberAccessToken, compilationUnit, ref parserModel);
		}
	
		return EmptyExpressionNode.Empty;
	}
	
	public IExpressionNode WithMergeToken(
		WithExpressionNode withExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.OpenBraceToken:
				parserModel.ExpressionList.Add((SyntaxKind.CloseBraceToken, withExpressionNode));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, withExpressionNode));
				return ParseWithExpressionNode(withExpressionNode, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.CloseBraceToken:				
				return withExpressionNode;
			case SyntaxKind.CommaToken:
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, withExpressionNode));
				return ParseWithExpressionNode(withExpressionNode, ref token, compilationUnit, ref parserModel);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), withExpressionNode, token);
		}
	}
	
	public IExpressionNode WithMergeExpression(
		WithExpressionNode withExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		return withExpressionNode;
	}
	
	public IExpressionNode ParseWithExpressionNode(
		WithExpressionNode withExpressionNode,
		ref SyntaxToken token,
		CSharpCompilationUnit compilationUnit,
		ref CSharpParserModel parserModel)
	{
		// Consume either 'OpenBraceToken', or 'CommaToken'
		_ = parserModel.TokenWalker.Consume();
		
		if (UtilityApi.IsConvertibleToIdentifierToken(parserModel.TokenWalker.Current.SyntaxKind) &&
		    parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.EqualsToken)
		{
			var memberAccessToken = new SyntaxToken(
				SyntaxKind.MemberAccessToken,
				new TextEditorTextSpan(
					0,
				    0,
				    0,
				    token.TextSpan.ResourceUri,
				    string.Empty,
				    string.Empty))
				{
					IsFabricated = true
				};
		
			return ParseMemberAccessToken(new TypeClauseNode(withExpressionNode.ResultTypeReference), ref memberAccessToken, compilationUnit, ref parserModel);
		}
	
		return EmptyExpressionNode.Empty;
	}
	
	public IExpressionNode EmptyMergeToken(
		EmptyExpressionNode emptyExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (UtilityApi.IsConvertibleToTypeClauseNode(token.SyntaxKind))
		{
			parserModel.AmbiguousIdentifierExpressionNode.SetSharedInstance(
				token,
				genericParameterListing: default,
				CSharpFacts.Types.Void.ToTypeReference(),
				emptyExpressionNode.FollowsMemberAccessToken);
			var ambiguousExpressionNode = parserModel.AmbiguousIdentifierExpressionNode;
		    
		    if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.StatementDelimiterToken && !ambiguousExpressionNode.FollowsMemberAccessToken ||
		    	parserModel.TryParseExpressionSyntaxKindList.Contains(SyntaxKind.TypeClauseNode) && parserModel.TokenWalker.Next.SyntaxKind != SyntaxKind.WithTokenContextualKeyword &&
		    	parserModel.TokenWalker.Next.SyntaxKind != SyntaxKind.EqualsCloseAngleBracketToken)
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
			case SyntaxKind.StringInterpolatedStartToken:
			case SyntaxKind.CharLiteralToken:
			case SyntaxKind.FalseTokenKeyword:
			case SyntaxKind.TrueTokenKeyword:
				TypeReference tokenTypeReference;
				
				if (token.SyntaxKind == SyntaxKind.NumericLiteralToken)
					tokenTypeReference = CSharpFacts.Types.Int.ToTypeReference();
				else if (token.SyntaxKind == SyntaxKind.StringLiteralToken || token.SyntaxKind == SyntaxKind.StringInterpolatedStartToken)
					tokenTypeReference = CSharpFacts.Types.String.ToTypeReference();
				else if (token.SyntaxKind == SyntaxKind.CharLiteralToken)
					tokenTypeReference = CSharpFacts.Types.Char.ToTypeReference();
				else if (token.SyntaxKind == SyntaxKind.FalseTokenKeyword || token.SyntaxKind == SyntaxKind.TrueTokenKeyword)
					tokenTypeReference = CSharpFacts.Types.Bool.ToTypeReference();
				else
					goto default;
				
				if (token.SyntaxKind == SyntaxKind.StringInterpolatedStartToken)
				{
					var interpolatedStringNode = new InterpolatedStringNode(
						token,
				    	stringInterpolatedEndToken: default,
				    	toBeExpressionPrimary: null,
				    	resultTypeReference: CSharpFacts.Types.String.ToTypeReference());
					
					return ParseInterpolatedStringNode(interpolatedStringNode, compilationUnit, ref parserModel);
				}
					
				return new LiteralExpressionNode(token, tokenTypeReference);
			case SyntaxKind.OpenParenthesisToken:
				return ShareEmptyExpressionNodeIntoOpenParenthesisTokenCase(ref token, compilationUnit, ref parserModel);
			case SyntaxKind.NewTokenKeyword:
				return new ConstructorInvocationExpressionNode(
					token,
			        typeReference: default,
			        functionParameterListing: default);
			case SyntaxKind.AwaitTokenContextualKeyword:
				return emptyExpressionNode;
			case SyntaxKind.AsyncTokenContextualKeyword:
				return emptyExpressionNode;
				// return new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeReference());
			case SyntaxKind.DollarSignToken:
			case SyntaxKind.AtToken:
				return emptyExpressionNode;
			case SyntaxKind.OutTokenKeyword:
				return emptyExpressionNode;
			case SyntaxKind.InTokenKeyword:
			case SyntaxKind.RefTokenKeyword:
			case SyntaxKind.ParamsTokenKeyword:
			case SyntaxKind.ThisTokenKeyword:
				return emptyExpressionNode;
			case SyntaxKind.OpenAngleBracketToken:
				// TODO: If text is "<Apple>" it no longer parses as generic parameters...
				// ...now there needs to be something prior to the OpenAngleBracketToken that opens the possibility
				// for generic parameters. (2025-03-16)
				//
				/*var genericParameterListing = new GenericParameterListing(
					token,
			        new List<GenericParameterEntry>(),
				    closeAngleBracketToken: default);
				
				parserModel.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, genericParameterListing));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, genericParameterListing));
				
				return genericParameterListing;*/
				goto default;
			case SyntaxKind.ReturnTokenKeyword:
				var returnStatementNode = new ReturnStatementNode(token, EmptyExpressionNode.Empty);
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
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), emptyExpressionNode, token);
		}
	}
	
	public IExpressionNode ShareEmptyExpressionNodeIntoOpenParenthesisTokenCase(
		ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		// This conditional branch is meant for '(2)' where the parenthesized expression node is
		// wrapping a numeric literal node / etc...
		//
		// First check if for NOT equaling '()' due to empty parameters for a lambda expression.
		if (parserModel.TokenWalker.Next.SyntaxKind != SyntaxKind.CloseParenthesisToken &&
			!UtilityApi.IsConvertibleToTypeClauseNode(parserModel.TokenWalker.Next.SyntaxKind))
		{
			var parenthesizedExpressionNode = new ParenthesizedExpressionNode(
				token,
				CSharpFacts.Types.Void.ToTypeReference());
			
			parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, parenthesizedExpressionNode));
			parserModel.ExpressionList.Add((SyntaxKind.CommaToken, parenthesizedExpressionNode));
			
			return EmptyExpressionNode.Empty;
		}
	
		var ambiguousParenthesizedExpressionNode = new AmbiguousParenthesizedExpressionNode(
			token,
			isParserContextKindForceStatementExpression: parserModel.ParserContextKind == CSharpParserContextKind.ForceStatementExpression ||
				// '(List<(int, bool)>)' required the following hack because the CSharpParserContextKind.ForceStatementExpression enum
				// is reset after the first TypeClauseNode in a statement is made, and there was no clear way to set it back again in this situation.;
				// TODO: Don't do this '(List<(int, bool)>)', instead figure out how to have CSharpParserContextKind.ForceStatementExpression live longer in a statement that has many TypeClauseNode(s).
				parserModel.ExpressionList.Any(x => x.ExpressionNode is IGenericParameterNode genericParameterNode && genericParameterNode.IsParsingGenericParameters));
			
		parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, ambiguousParenthesizedExpressionNode));
		parserModel.ExpressionList.Add((SyntaxKind.CommaToken, ambiguousParenthesizedExpressionNode));
		return EmptyExpressionNode.Empty;
	}
	
	public IExpressionNode ExplicitCastMergeToken(
		ExplicitCastNode explicitCastNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CloseParenthesisToken:
				explicitCastNode.CloseParenthesisToken = token;
				return explicitCastNode;
			case SyntaxKind.IdentifierToken:
				var ambiguousExpressionNode = new AmbiguousIdentifierExpressionNode(
					token,
			        genericParameterListing: default,
			        CSharpFacts.Types.Void.ToTypeReference());
			    return ambiguousExpressionNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), explicitCastNode, token);
		}
	}
	
	public IExpressionNode ReturnStatementMergeToken(
		ReturnStatementNode returnStatementNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), returnStatementNode, token);
		}
	}
	
	public IExpressionNode ReturnStatementMergeExpression(
		ReturnStatementNode returnStatementNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), returnStatementNode, expressionSecondary);
	}
	
	public IExpressionNode LambdaMergeToken(
		LambdaExpressionNode lambdaExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (token.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
		{
			var textSpan = new TextEditorTextSpan(
				token.TextSpan.StartInclusiveIndex,
			    token.TextSpan.EndExclusiveIndex,
			    (byte)GenericDecorationKind.None,
			    token.TextSpan.ResourceUri,
			    token.TextSpan.SourceText);
		
			compilationUnit.__SymbolList.Add(new Symbol(SyntaxKind.LambdaSymbol, parserModel.GetNextSymbolId(), textSpan));
		
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
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), lambdaExpressionNode, token);
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
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), lambdaExpressionNode, token);
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
			
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), lambdaExpressionNode, token);
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
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), lambdaExpressionNode, token);
			}
			else
			{
				lambdaExpressionNode.HasReadParameters = true;
				return lambdaExpressionNode;
			}
		}
		else
		{
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), lambdaExpressionNode, token);
		}
	}
	
	public IExpressionNode LambdaMergeExpression(
		LambdaExpressionNode lambdaExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (expressionSecondary.SyntaxKind)
		{
			default:
				if (!lambdaExpressionNode.CloseCodeBlockTextSpan.ConstructorWasInvoked)
					CloseLambdaExpressionScope(lambdaExpressionNode, compilationUnit, ref parserModel);
				
				return lambdaExpressionNode;
		}
	}

	public IExpressionNode LiteralMergeToken(
		LiteralExpressionNode literalExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), literalExpressionNode, token);
	}
	
	public IExpressionNode InterpolatedStringMergeToken(
		InterpolatedStringNode interpolatedStringNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (token.SyntaxKind == SyntaxKind.StringInterpolatedEndToken)
		{
			return interpolatedStringNode;
		}
		else if (token.SyntaxKind == SyntaxKind.StringInterpolatedContinueToken)
		{
			parserModel.ExpressionList.Add((SyntaxKind.StringInterpolatedContinueToken, interpolatedStringNode));
			return EmptyExpressionNode.Empty;
		}

		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), interpolatedStringNode, token);
	}
	
	public IExpressionNode InterpolatedStringMergeExpression(
		InterpolatedStringNode interpolatedStringNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StringInterpolatedEndToken)
		{
			if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
				ForceDecisionAmbiguousIdentifier(EmptyExpressionNode.Empty, (AmbiguousIdentifierExpressionNode)expressionSecondary, compilationUnit, ref parserModel);

			interpolatedStringNode.StringInterpolatedEndToken = parserModel.TokenWalker.Current;

			// Interpolated strings have their interpolated expressions inserted into the syntax token list
			// immediately following the StringInterpolatedStartToken itself.
			//
			// They are deliminated by StringInterpolatedEndToken,
			// upon which this 'LiteralMergeExpression' will be invoked.
			//
			// Just return back the 'interpolatedStringNode.ToBeExpressionPrimary'.
			return interpolatedStringNode.ToBeExpressionPrimary ?? interpolatedStringNode;
		}
		else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StringInterpolatedContinueToken)
		{
			if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
				ForceDecisionAmbiguousIdentifier(EmptyExpressionNode.Empty, (AmbiguousIdentifierExpressionNode)expressionSecondary, compilationUnit, ref parserModel);

			return interpolatedStringNode;
		}
		else
		{
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), interpolatedStringNode, expressionSecondary);
		}
	}
	
	public IExpressionNode ParenthesizedMergeToken(
		ParenthesizedExpressionNode parenthesizedExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CloseParenthesisToken:
				if (parenthesizedExpressionNode.InnerExpression.SyntaxKind == SyntaxKind.TypeClauseNode)
				{
					var typeClauseNode = (TypeClauseNode)parenthesizedExpressionNode.InnerExpression;
					var explicitCastNode = new ExplicitCastNode(parenthesizedExpressionNode.OpenParenthesisToken, new TypeReference(typeClauseNode));
					return ExplicitCastMergeToken(explicitCastNode, ref token, compilationUnit, ref parserModel);
				}
				
				parenthesizedExpressionNode.CloseParenthesisToken = token;
				return parenthesizedExpressionNode;
			case SyntaxKind.EqualsCloseAngleBracketToken:
				// TODO: I think this switch case needs to be removed. With the addition of the AmbiguousParenthesizedExpressionNode code...
				// ...(what is about to be said needs confirmation) the parser now only creates the parenthesized expression in the
				// absence of the 'EqualsCloseAngleBracketToken'?
				var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeReference());
				SetLambdaExpressionNodeVariableDeclarationNodeList(lambdaExpressionNode, parenthesizedExpressionNode.InnerExpression, compilationUnit, ref parserModel);
				return lambdaExpressionNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), parenthesizedExpressionNode, token);
		}
	}
	
	public IExpressionNode ParenthesizedMergeExpression(
		ParenthesizedExpressionNode parenthesizedExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
		{
			// TODO: I think this conditional branch needs to be removed. With the addition of the AmbiguousParenthesizedExpressionNode code...
			// ...(what is about to be said needs confirmation) the parser now only creates the parenthesized expression in the
			// absence of the 'EqualsCloseAngleBracketToken'?
			var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeReference());
			return SetLambdaExpressionNodeVariableDeclarationNodeList(lambdaExpressionNode, expressionSecondary, compilationUnit, ref parserModel);
		}
	
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
			expressionSecondary = ForceDecisionAmbiguousIdentifier(parenthesizedExpressionNode, (AmbiguousIdentifierExpressionNode)expressionSecondary, compilationUnit, ref parserModel);
	
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
		{
			parserModel.NoLongerRelevantExpressionNode = parenthesizedExpressionNode;
			var tupleExpressionNode = new TupleExpressionNode();
			tupleExpressionNode.InnerExpressionList.Add(expressionSecondary);
			// tupleExpressionNode never saw the 'OpenParenthesisToken' so the 'ParenthesizedExpressionNode'
			// has to create the ExpressionList entry on behalf of the 'TupleExpressionNode'.
			parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, tupleExpressionNode));
			return tupleExpressionNode;
		}
	
		if (parenthesizedExpressionNode.InnerExpression.SyntaxKind != SyntaxKind.EmptyExpressionNode)
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), parenthesizedExpressionNode, expressionSecondary);
		
		// TODO: This seems like a bad idea?
		if (expressionSecondary.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			 var variableReferenceNode = (VariableReferenceNode)expressionSecondary;
			 
			 if (variableReferenceNode.IsFabricated)
			 {
			 	var typeClauseNode = parserModel.ConstructOrRecycleTypeClauseNode(
			 		variableReferenceNode.VariableIdentifierToken, valueType: null, genericParameterListing: default, isKeywordType: false);
				
				BindTypeClauseNode(
			        typeClauseNode,
			        compilationUnit,
			        ref parserModel);
			        
				return new ExplicitCastNode(parenthesizedExpressionNode.OpenParenthesisToken, new TypeReference(typeClauseNode));
			 }
		}

		parenthesizedExpressionNode.InnerExpression = expressionSecondary;
		return parenthesizedExpressionNode;
	}
	
	public IExpressionNode TupleMergeToken(
		TupleExpressionNode tupleExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
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
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), tupleExpressionNode, token);
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
					tupleExpressionNode.InnerExpressionList.Add(expressionSecondary);
					return tupleExpressionNode;
				}
			
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), tupleExpressionNode, expressionSecondary);
		}
	}
	
	public IExpressionNode TypeClauseMergeToken(
		TypeClauseNode typeClauseNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (typeClauseNode.IsParsingGenericParameters)
		{
			return GenericParametersListingMergeToken(
				typeClauseNode, ref token, compilationUnit, ref parserModel);
		}
	
		switch (token.SyntaxKind)
		{
			case SyntaxKind.OpenAngleBracketToken:
				return ParseGenericParameterNode_Start(
					typeClauseNode, ref token, compilationUnit, ref parserModel);
			case SyntaxKind.CloseAngleBracketToken:
				if (typeClauseNode.GenericParameterListing.ConstructorWasInvoked)
				{
					typeClauseNode.GenericParameterListing.SetCloseAngleBracketToken(token);
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
					var typeClauseToken = typeClauseNode.TypeIdentifierToken;
					var functionInvocationNode = new FunctionInvocationNode(
						UtilityApi.ConvertToIdentifierToken(ref typeClauseToken, compilationUnit, ref parserModel),
				        functionDefinitionNode: null,
				        typeClauseNode.GenericParameterListing,
				        new FunctionParameterListing(
							token,
					        new List<FunctionParameterEntry>(),
					        closeParenthesisToken: default),
				        CSharpFacts.Types.Void.ToTypeReference());
				        
				    BindFunctionInvocationNode(
				        functionInvocationNode,
				        compilationUnit,
				        ref parserModel);
		
					return ParseFunctionParameterListing_Start(functionInvocationNode, compilationUnit, ref parserModel);
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
					var identifierToken = UtilityApi.ConvertToIdentifierToken(ref token, compilationUnit, ref parserModel);
					var isRootExpression = true;
					
					foreach (var tuple in parserModel.ExpressionList)
					{
						if (tuple.ExpressionNode is null)
							continue;
						
						isRootExpression = false;
						break;
					}
					
					VariableDeclarationNode variableDeclarationNode;
					
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
					        new TypeReference(typeClauseNode),
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
				
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), typeClauseNode, token);
		}
	}
	
	public IExpressionNode TypeClauseMergeExpression(
		TypeClauseNode typeClauseNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (typeClauseNode.IsParsingGenericParameters)
		{
			return GenericParametersListingMergeExpression(
				typeClauseNode, expressionSecondary, compilationUnit, ref parserModel);
		}
	
		switch (expressionSecondary.SyntaxKind)
		{
			case SyntaxKind.GenericParametersListingNode:
				if (typeClauseNode.GenericParameterListing.ConstructorWasInvoked &&
					!typeClauseNode.GenericParameterListing.CloseAngleBracketToken.ConstructorWasInvoked)
				{
					return typeClauseNode;
				}
				
				goto default;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), typeClauseNode, expressionSecondary);
		}
	}
	
	public IExpressionNode FunctionInvocationMergeToken(
		FunctionInvocationNode functionInvocationNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (functionInvocationNode.IsParsingFunctionParameters)
		{
			return ParseFunctionParameterListing_Token(
				functionInvocationNode, ref token, compilationUnit, ref parserModel);
		}
		else if (functionInvocationNode.IsParsingGenericParameters)
		{
			return GenericParametersListingMergeToken(
				functionInvocationNode, ref token, compilationUnit, ref parserModel);
		}

		switch (token.SyntaxKind)
		{
			case SyntaxKind.OpenAngleBracketToken:
				if (!functionInvocationNode.FunctionParameterListing.ConstructorWasInvoked)
				{
					// Note: non member access function invocation takes the path:
					//       AmbiguousIdentifierExpressionNode -> FunctionInvocationNode
					//
					// ('AmbiguousIdentifierExpressionNode' converts when it sees 'OpenParenthesisToken')
					//
					//
					// But, member access will determine that an identifier is a function
					// prior to seeing the 'OpenAngleBracketToken' or the 'OpenParenthesisToken'.
					//
					// These paths would preferably be combined into a less "hacky" two way path.
					// Until then these 'if (functionInvocationNode.FunctionParametersListingNode is null)'
					// statements will be here.
					
					if (functionInvocationNode.GenericParameterListing.ConstructorWasInvoked)
						goto default;
					
					functionInvocationNode.GenericParameterListing =
						new GenericParameterListing(
							token,
					        new List<GenericParameterEntry>(),
					        closeAngleBracketToken: default);
					
				    parserModel.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, functionInvocationNode));
					parserModel.ExpressionList.Add((SyntaxKind.CommaToken, functionInvocationNode));
					return functionInvocationNode;
				}
				
				goto default;
			case SyntaxKind.OpenParenthesisToken:
				if (!functionInvocationNode.FunctionParameterListing.ConstructorWasInvoked)
				{
					// Note: non member access function invocation takes the path:
					//       AmbiguousIdentifierExpressionNode -> FunctionInvocationNode
					//
					// ('AmbiguousIdentifierExpressionNode' converts when it sees 'OpenParenthesisToken')
					//
					//
					// But, member access will determine that an identifier is a function
					// prior to seeing the 'OpenAngleBracketToken' or the 'OpenParenthesisToken'.
					//
					// These paths would preferably be combined into a less "hacky" two way path.
					// Until then these 'if (functionInvocationNode.FunctionParametersListingNode is null)'
					// statements will be here.
					
					functionInvocationNode.FunctionParameterListing = new FunctionParameterListing(
						token,
				        new List<FunctionParameterEntry>(),
				        closeParenthesisToken: default);
					
					return ParseFunctionParameterListing_Start(
						functionInvocationNode, compilationUnit, ref parserModel);
				}

				goto default;
			case SyntaxKind.CloseParenthesisToken:
				functionInvocationNode.FunctionParameterListing.SetCloseParenthesisToken(token);
				return functionInvocationNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), functionInvocationNode, token);
		}
	}
	
	public IExpressionNode FunctionInvocationMergeExpression(
		FunctionInvocationNode functionInvocationNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (functionInvocationNode.IsParsingFunctionParameters)
		{
			return ParseFunctionParameterListing_Expression(
				functionInvocationNode, expressionSecondary, compilationUnit, ref parserModel);
		}
		else if (functionInvocationNode.IsParsingGenericParameters)
		{
			return GenericParametersListingMergeExpression(
				functionInvocationNode, expressionSecondary, compilationUnit, ref parserModel);
		}
	
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionSecondary = ForceDecisionAmbiguousIdentifier(functionInvocationNode, (AmbiguousIdentifierExpressionNode)expressionSecondary, compilationUnit, ref parserModel);
		}
	
		switch (expressionSecondary.SyntaxKind)
		{
			case SyntaxKind.EmptyExpressionNode:
				return functionInvocationNode;
			case SyntaxKind.FunctionInvocationNode:
				return functionInvocationNode;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), functionInvocationNode, expressionSecondary);
		}
	}
	
	public IExpressionNode SetLambdaExpressionNodeVariableDeclarationNodeList(
		LambdaExpressionNode lambdaExpressionNode, IExpressionNode expressionNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (expressionNode.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
    	{
    		var token = ((AmbiguousIdentifierExpressionNode)expressionNode).Token;
    		
    		if (token.SyntaxKind != SyntaxKind.IdentifierToken)
    			return lambdaExpressionNode;
    	
    		var variableDeclarationNode = new VariableDeclarationNode(
		        TypeFacts.Empty.ToTypeReference(),
		        token,
		        VariableKind.Local,
		        isInitialized: false);
		        
    		lambdaExpressionNode.VariableDeclarationNodeList.Add(variableDeclarationNode);
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
	
	public IExpressionNode ParseLambdaExpressionNode(LambdaExpressionNode lambdaExpressionNode, ref SyntaxToken openBraceToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
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
		
		if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenBraceToken)
		{
			OpenLambdaExpressionScope(lambdaExpressionNode, ref openBraceToken, compilationUnit, ref parserModel);
			return SkipLambdaExpressionStatements(lambdaExpressionNode, compilationUnit, ref parserModel);
		}
		else
		{
			parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, lambdaExpressionNode));
			OpenLambdaExpressionScope(lambdaExpressionNode, ref openBraceToken, compilationUnit, ref parserModel);
			return EmptyExpressionNode.Empty;
		}
	}
	
	public void OpenLambdaExpressionScope(LambdaExpressionNode lambdaExpressionNode, ref SyntaxToken openBraceToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		parserModel.Binder.NewScopeAndBuilderFromOwner(
        	lambdaExpressionNode,
        	openBraceToken.TextSpan,
        	compilationUnit,
	        ref parserModel);
	}
	
	public void CloseLambdaExpressionScope(LambdaExpressionNode lambdaExpressionNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		var closeBraceToken = new SyntaxToken(SyntaxKind.CloseBraceToken, parserModel.TokenWalker.Current.TextSpan);		
        parserModel.Binder.CloseScope(closeBraceToken.TextSpan, compilationUnit, ref parserModel);
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
	public IExpressionNode SkipLambdaExpressionStatements(LambdaExpressionNode lambdaExpressionNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		parserModel.TokenWalker.Consume(); // Skip the EqualsCloseAngleBracketToken
		
		var openTokenIndex = parserModel.TokenWalker.Index;
		var openBraceToken = parserModel.TokenWalker.Consume();
    	
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
		
		var lambdaCodeBlockBuilder = parserModel.CurrentCodeBlockBuilder;
		CloseLambdaExpressionScope(lambdaExpressionNode, compilationUnit, ref parserModel);
	
		var closeTokenIndex = parserModel.TokenWalker.Index;
		var closeBraceToken = parserModel.TokenWalker.Match(SyntaxKind.CloseBraceToken);
		
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif
		
		parserModel.StatementBuilder.ParseChildScopeStack.Push(
			(
				parserModel.CurrentCodeBlockBuilder.CodeBlockOwner,
				new CSharpDeferredChildScope(
					openTokenIndex,
					closeTokenIndex,
					lambdaCodeBlockBuilder)
			));
			
		return lambdaExpressionNode;
	}

	public IExpressionNode ParseMemberAccessToken(
		IExpressionNode expressionPrimary, ref SyntaxToken tokenIn, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		/*
		(2025-01-26)
		============
		
		````public class Aaa
		````{
		````    public Aaa(int number)
		````    {
		````    }
		````
		````    public class Bbb
		````    {
		````    }
		````}
		
		// Type definition contains type definition, invoke inner type definition's constructor.
		{
			new Aaa.Bbb();
			
			// Current scenario:
			// -----------------
			// empty expression + new -> constructor invocation
			// constructor invocation + Aaa -> constructor invocation typeof(Aaa)
			
			// Next scenario
			// -------------
			// empty expression + new -> constructor invocation
			// constructor invocation + Aaa
			//     | HandleAmbiguousIdentifier(Aaa)
			//     | 
			//     | ````HandleAmbiguousIdentifier(AmbiguousIdentifierExpressionNode node)
			//     | ````{
			//     | ````    // TODO: Static reference to a type where there exists a variable with the same identifier.
			//     | ````    // TODO: Explicit namespace qualification.
			//     | ````    // 
			//     | ````    var boundNode = Binder.Bind(ambiguousIdentifierExpressionNode: node);
			//     | ````    
			//     | ````    while (TokenNext.SyntaxKind == SyntaxKind.MemberAccerAccessToken)
			//     | ````        boundNode = boundNode.GetMember(TokenWalker.Peek(2));
			//     | ````    
			//     | ````    return boundNode;
			//     | ````}
			// constructor invocation + boundNode -> constructor invocation typeof(Bbb)
		}
		*/
		
		var token = tokenIn;
		var loopIteration = 0;
		
		while (!parserModel.TokenWalker.IsEof)
		{
			if (loopIteration++ >= 1)
			{
				// The object initialization / record 'with' keyword
				// provide a fabricated initial token.
				//
				// So the 0th iteration needs to use this function's SyntaxToken parameter.
				token = parserModel.TokenWalker.Current;
				
				if (token.SyntaxKind != SyntaxKind.MemberAccessToken)
					break;
			}
		
			if (!token.IsFabricated && !UtilityApi.IsConvertibleToIdentifierToken(parserModel.TokenWalker.Next.SyntaxKind))
				break; // TODO: Consume and return the MemberAccessToken here?
	
			if (!token.IsFabricated)
				_ = parserModel.TokenWalker.Consume(); // Consume the 'MemberAccessToken'
			
			// Consume the 'NameableToken'
			var nameableToken = parserModel.TokenWalker.Consume();
			var memberIdentifierToken = UtilityApi.ConvertToIdentifierToken(
				ref nameableToken,
				compilationUnit,
				ref parserModel);
				
			if (!memberIdentifierToken.ConstructorWasInvoked || expressionPrimary is null)
				break;
			
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
		
			TypeReference typeReference = default;
		
			if (expressionPrimary.SyntaxKind == SyntaxKind.VariableReferenceNode)
			{
				var variableReferenceNode = (VariableReferenceNode)expressionPrimary;
				if (variableReferenceNode.VariableDeclarationNode is not null)
					typeReference = variableReferenceNode.VariableDeclarationNode.TypeReference;
			}
			else if (expressionPrimary.SyntaxKind == SyntaxKind.FunctionInvocationNode)
			{
				typeReference = ((FunctionInvocationNode)expressionPrimary).ResultTypeReference;
			}
			else if (expressionPrimary.SyntaxKind == SyntaxKind.TypeClauseNode)
			{
				typeReference = new TypeReference((TypeClauseNode)expressionPrimary);
			}
				
			if (typeReference == default)
			{
				expressionPrimary = Aaa(memberIdentifierToken, compilationUnit, ref parserModel);
				continue;
			}
			
			var maybeTypeDefinitionNode = GetDefinitionNode(compilationUnit, typeReference.TypeIdentifierToken.TextSpan, SyntaxKind.TypeClauseNode);
			if (maybeTypeDefinitionNode is null || maybeTypeDefinitionNode.SyntaxKind != SyntaxKind.TypeDefinitionNode)
			{
				expressionPrimary = Aaa(memberIdentifierToken, compilationUnit, ref parserModel);
				continue;
			}
				
			var typeDefinitionNode = (TypeDefinitionNode)maybeTypeDefinitionNode;
			var memberList = typeDefinitionNode.GetMemberList();
			ISyntaxNode? foundDefinitionNode = null;
			
			foreach (var node in memberList)
			{
				if (node.SyntaxKind == SyntaxKind.VariableDeclarationNode)
				{
					var variableDeclarationNode = (VariableDeclarationNode)node;
					if (!variableDeclarationNode.IdentifierToken.ConstructorWasInvoked)
						continue;
					
					if (variableDeclarationNode.IdentifierToken.TextSpan.GetText() == memberIdentifierToken.TextSpan.GetText())
					{
						foundDefinitionNode = variableDeclarationNode;
						break;
					}
				}
				else if (node.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
				{
					// TODO: Create a Binder.Main method that takes a node and returns its identifier?
					var functionDefinitionNode = (FunctionDefinitionNode)node;
					if (!functionDefinitionNode.FunctionIdentifierToken.ConstructorWasInvoked)
						continue;
					
					if (functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText() == memberIdentifierToken.TextSpan.GetText())
					{
						foundDefinitionNode = functionDefinitionNode;
						break;
					}
				}
			}
			
			if (foundDefinitionNode is null)
			{
				expressionPrimary = Aaa(memberIdentifierToken, compilationUnit, ref parserModel);
				continue;
			}
				 
			if (foundDefinitionNode.SyntaxKind == SyntaxKind.VariableDeclarationNode)
			{
				var variableDeclarationNode = (VariableDeclarationNode)foundDefinitionNode;
				
				var variableReferenceNode = parserModel.ConstructOrRecycleVariableReferenceNode(
		            memberIdentifierToken,
		            variableDeclarationNode);
		        var symbolId = CreateVariableSymbol(variableReferenceNode.VariableIdentifierToken, variableDeclarationNode.VariableKind, compilationUnit, ref parserModel);
		        
		        compilationUnit.SymbolIdToExternalTextSpanMap.TryAdd(
		        	symbolId,
		        	(variableDeclarationNode.IdentifierToken.TextSpan.ResourceUri, variableDeclarationNode.IdentifierToken.TextSpan.StartInclusiveIndex));
		        
		    	expressionPrimary = variableReferenceNode;
			}
			else if (foundDefinitionNode.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
			{
				var functionDefinitionNode = (FunctionDefinitionNode)foundDefinitionNode;
				
				// TODO: Method group node?
				var functionInvocationNode = new FunctionInvocationNode(
		            memberIdentifierToken,
		            // TODO: Don't store a reference to definitons.
		            // TODO: Type -> "<...>" -> "(" -> FunctionInvocationNode, but will FunctionInvocationNode -> "<...>"?
			        functionDefinitionNode,
			        // TODO: Bind the named arguments to their declaration within the definition.
			        genericParameterListing: default,
			        functionParameterListing: default,
			        functionDefinitionNode.ReturnTypeReference);
		        
		        var functionSymbol = new Symbol(
		        	SyntaxKind.FunctionSymbol,
		        	parserModel.GetNextSymbolId(),
		        	functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan with
			        {
			            DecorationByte = (byte)GenericDecorationKind.Function
			        });
		        compilationUnit.__SymbolList.Add(functionSymbol);
		        var symbolId = functionSymbol.SymbolId;
		        
		        compilationUnit.SymbolIdToExternalTextSpanMap.TryAdd(
		        	symbolId,
		        	(functionDefinitionNode.FunctionIdentifierToken.TextSpan.ResourceUri, functionDefinitionNode.FunctionIdentifierToken.TextSpan.StartInclusiveIndex));
		        
		        // TODO: Transition from 'FunctionInvocationNode' to GenericParameters / FunctionParameters
		        // TODO: Method group if next token is not '<' or '('
		    	expressionPrimary = functionInvocationNode;
			}
		}
		
		// TODO: Transition from 'FunctionInvocationNode' to GenericParameters / FunctionParameters
		// TODO: Transition from 'ConstructorInvocationNode' to GenericParameters / FunctionParameters
		// TODO: Method group if next token is not '<' or '('
		// TODO: return new Aaa.Bbb(); // is a very good test case.
		
		
		
		return expressionPrimary;
	}
	
	private IExpressionNode Aaa(SyntaxToken memberIdentifierToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
			parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
		{
			var functionInvocationNode = new FunctionInvocationNode(
	            memberIdentifierToken,
		        functionDefinitionNode: null,
		        genericParameterListing: default,
		        functionParameterListing: default,
		        TypeFacts.Empty.ToTypeReference());
	        var functionSymbol = new Symbol(
	        	SyntaxKind.FunctionSymbol,
	        	parserModel.GetNextSymbolId(),
	        	functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan with
		        {
		            DecorationByte = (byte)GenericDecorationKind.Function
		        });
	        compilationUnit.__SymbolList.Add(functionSymbol);
			return functionInvocationNode;
		}
		else
		{
			var variableReferenceNode = parserModel.ConstructOrRecycleVariableReferenceNode(
	            memberIdentifierToken,
	            variableDeclarationNode: null);
	        _ = CreateVariableSymbol(variableReferenceNode.VariableIdentifierToken, VariableKind.Property, compilationUnit, ref parserModel);
			return variableReferenceNode;
		}
	}
	
	private IExpressionNode AmbiguousParenthesizedExpressionTransformTo_ParenthesizedExpressionNode(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		var parenthesizedExpressionNode = new ParenthesizedExpressionNode(
			ambiguousParenthesizedExpressionNode.OpenParenthesisToken,
			CSharpFacts.Types.Void.ToTypeReference());
			
		parenthesizedExpressionNode.InnerExpression = expressionSecondary;
			
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
				
				tupleExpressionNode.InnerExpressionList.Add(expressionNode);
			}
		}
		
		if (expressionSecondary is not null)
			tupleExpressionNode.InnerExpressionList.Add(expressionSecondary);
		
		parserModel.NoLongerRelevantExpressionNode = ambiguousParenthesizedExpressionNode;
		
		if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.CloseParenthesisToken)
		{
			parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, tupleExpressionNode));
			parserModel.ExpressionList.Add((SyntaxKind.CommaToken, tupleExpressionNode));
		}
			
		return tupleExpressionNode;
	}
	
	private IExpressionNode AmbiguousParenthesizedExpressionTransformTo_ExplicitCastNode(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, IExpressionNode expressionNode, ref SyntaxToken closeParenthesisToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		TypeClauseNode typeClauseNode;
	
		if (expressionNode.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			var token = ((AmbiguousIdentifierExpressionNode)expressionNode).Token;
			typeClauseNode = UtilityApi.ConvertTokenToTypeClauseNode(
				ref token,
				compilationUnit,
				ref parserModel);
		}
		else if (expressionNode.SyntaxKind == SyntaxKind.TypeClauseNode)
		{
			typeClauseNode = (TypeClauseNode)expressionNode;
		}
		else if (expressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			var token = ((VariableReferenceNode)expressionNode).VariableIdentifierToken;
			typeClauseNode = UtilityApi.ConvertTokenToTypeClauseNode(
				ref token,
				compilationUnit,
				ref parserModel);
		}
		else
		{
			return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), ambiguousParenthesizedExpressionNode, expressionNode);
		}
			
		BindTypeClauseNode(typeClauseNode, compilationUnit, ref parserModel);
		
		var explicitCastNode = new ExplicitCastNode(ambiguousParenthesizedExpressionNode.OpenParenthesisToken, new TypeReference(typeClauseNode), closeParenthesisToken);
		return explicitCastNode;
	}
	
	private IExpressionNode AmbiguousParenthesizedExpressionTransformTo_TypeClauseNode(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		var identifierToken = new SyntaxToken(
			SyntaxKind.IdentifierToken,
			new TextEditorTextSpan(
			    ambiguousParenthesizedExpressionNode.OpenParenthesisToken.TextSpan.StartInclusiveIndex,
			    token.TextSpan.EndExclusiveIndex,
			    default(byte),
			    token.TextSpan.ResourceUri,
			    token.TextSpan.SourceText));
		
		return parserModel.ConstructOrRecycleTypeClauseNode(
			identifierToken,
	        valueType: null,
	        genericParameterListing: default,
	        isKeywordType: false);
	}
	
	private IExpressionNode AmbiguousParenthesizedExpressionTransformTo_LambdaExpressionNode(
		AmbiguousParenthesizedExpressionNode ambiguousParenthesizedExpressionNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{	
		var lambdaExpressionNode = new LambdaExpressionNode(CSharpFacts.Types.Void.ToTypeReference());
					
		if (ambiguousParenthesizedExpressionNode.NodeList is not null)
		{
			if (ambiguousParenthesizedExpressionNode.NodeList.Count >= 1)
			{
				foreach (var node in ambiguousParenthesizedExpressionNode.NodeList)
				{
					if (node.SyntaxKind == SyntaxKind.VariableDeclarationNode)
					{
						lambdaExpressionNode.VariableDeclarationNodeList.Add((VariableDeclarationNode)node);
					}
					else
					{
						SyntaxToken identifierToken;
					
						if (node.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
						{
							var token = ((AmbiguousIdentifierExpressionNode)node).Token;
							identifierToken = UtilityApi.ConvertToIdentifierToken(ref token, compilationUnit, ref parserModel);
						}
						else if (node.SyntaxKind == SyntaxKind.TypeClauseNode)
						{
							var token = ((TypeClauseNode)node).TypeIdentifierToken;
							identifierToken = identifierToken = UtilityApi.ConvertToIdentifierToken(ref token, compilationUnit, ref parserModel);
						}
						else if (node.SyntaxKind == SyntaxKind.VariableReferenceNode)
						{
							var token = ((VariableReferenceNode)node).VariableIdentifierToken;
							identifierToken = identifierToken = UtilityApi.ConvertToIdentifierToken(ref token, compilationUnit, ref parserModel);
						}
						else
						{
							return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), ambiguousParenthesizedExpressionNode, node);
						}
					
						var variableDeclarationNode = new VariableDeclarationNode(
					        TypeFacts.Empty.ToTypeReference(),
					        identifierToken,
					        VariableKind.Local,
					        false);
					        
			    		lambdaExpressionNode.VariableDeclarationNodeList.Add(variableDeclarationNode);
					}
				}
			}
		}
		
		// CONFUSING: the 'AmbiguousIdentifierExpressionNode' when merging with 'EqualsCloseAngleBracketToken'...
		// ...will invoke the 'ParseLambdaExpressionNode(...)' method.
		//
		// But, the loop entered in on 'EqualsCloseAngleBracketToken' for the 'AmbiguousIdentifierExpressionNode'.
		// Whereas this code block's loop entered in on 'CloseParenthesisToken'.
		//
		// This "desync" means you cannot synchronize them, while sharing the code for 'ParseLambdaExpressionNode(...)'
		// in its current state.
		// 
		// So, this code block needs to do some odd 'parserModel.TokenWalker.Consume();' before and after
		// the 'ParseLambdaExpressionNode(...)' invocation in order to "sync" the token walker
		// with the 'AmbiguousIdentifierExpressionNode' path.
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken &&
			parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
		{
			_ = parserModel.TokenWalker.Consume(); // CloseParenthesisToken
		}
		
		SyntaxToken openBraceToken;
		
		if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenBraceToken)
			openBraceToken = parserModel.TokenWalker.Next;
		else
			openBraceToken = new SyntaxToken(SyntaxKind.OpenBraceToken, parserModel.TokenWalker.Current.TextSpan);
		
		var resultExpression = ParseLambdaExpressionNode(lambdaExpressionNode, ref openBraceToken, compilationUnit, ref parserModel);
		
		_ = parserModel.TokenWalker.Consume(); // EqualsCloseAngleBracketToken
		
		return resultExpression;
	}
	
	/// <summary>
	/// Am working on the first implementation of parsing interpolated strings.
	/// Need a way for a 'StringInterpolatedToken' to trigger the new code.
	///
	/// Currently there are 2 'LiteralExpressionNode' constructor invocations,
	/// so under each of them I've invoked this method.
	///
	/// Will see where things go from here, TODO: don't do this long term.
	///
	/// --------------------------------
	///
	/// Interpolated strings might not actually be "literal expressions"
	/// but I think this is a good path to investigate that will lead to understanding the correct answer.
	/// </summary>
	public IExpressionNode ParseInterpolatedStringNode(
		InterpolatedStringNode interpolatedStringNode,
		CSharpCompilationUnit compilationUnit,
		ref CSharpParserModel parserModel)
	{
		parserModel.ExpressionList.Add((SyntaxKind.StringInterpolatedEndToken, interpolatedStringNode));
		parserModel.ExpressionList.Add((SyntaxKind.StringInterpolatedContinueToken, interpolatedStringNode));
		return EmptyExpressionNode.Empty;
	}
	
	public IExpressionNode ParseFunctionParameterListing_Token(
		IInvocationNode invocationNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CommaToken:
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, invocationNode));
				parserModel.ExpressionList.Add((SyntaxKind.ColonToken, invocationNode));
				return ParseNamedParameterSyntaxAndReturnEmptyExpressionNode(compilationUnit, ref parserModel);
			case SyntaxKind.ColonToken:
				parserModel.ExpressionList.Add((SyntaxKind.ColonToken, invocationNode));
				return EmptyExpressionNode.Empty;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), invocationNode, token);
		}
	}
	
	public IExpressionNode ParseFunctionParameterListing_Expression(
		IInvocationNode invocationNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
			invocationNode.IsParsingFunctionParameters = false;
	
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
			return invocationNode;
		
		if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			return invocationNode;
			
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionSecondary = ForceDecisionAmbiguousIdentifier(
				EmptyExpressionNode.Empty,
				(AmbiguousIdentifierExpressionNode)expressionSecondary,
				compilationUnit,
				ref parserModel);
		}
		
		invocationNode.FunctionParameterListing.FunctionParameterEntryList.Add(
			new FunctionParameterEntry(
		        expressionSecondary,
		        hasOutKeyword: false,
		        hasInKeyword: false,
		        hasRefKeyword: false));
		
		return invocationNode;
	}
	
	/// <summary>
	/// Careful if changing this method:
	/// ================================
	/// Constructor secondary syntax with 'base' or 'this':
	/// ````public MyClass() : base() { }
	///
	/// 'ParseFunctions.HandleConstructorDefinition(...)' has this logic repeated
	/// because it exists outside the expression loop.
	///
	/// You may want to change both locations.
	/// </summary>
	public IExpressionNode ParseFunctionParameterListing_Start(IInvocationNode invocationNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		invocationNode.IsParsingFunctionParameters = true;
	
		parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, invocationNode));
		parserModel.ExpressionList.Add((SyntaxKind.CommaToken, invocationNode));
		parserModel.ExpressionList.Add((SyntaxKind.ColonToken, invocationNode));
		return ParseNamedParameterSyntaxAndReturnEmptyExpressionNode(compilationUnit, ref parserModel);
	}
	
	/// <summary>
	/// Careful if changing this method:
	/// ================================
	/// Constructor secondary syntax with 'base' or 'this':
	/// ````public MyClass() : base() { }
	///
	/// 'ParseFunctions.HandleConstructorDefinition(...)' 
	/// will invoke this method from outside of the expression loop.
	///
	/// You may want to change both locations.
	///
	/// 'guaranteeOpenParenthesisConsume' is used for the constructor definitions
	/// because it doesn't have the expression loop to guarantee the consumption (with proper timing).
	/// </summary>
	public IExpressionNode ParseNamedParameterSyntaxAndReturnEmptyExpressionNode(
		CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel, bool guaranteeConsume = false)
	{
		if (UtilityApi.IsConvertibleToIdentifierToken(parserModel.TokenWalker.Peek(1).SyntaxKind) &&
			parserModel.TokenWalker.Peek(2).SyntaxKind == SyntaxKind.ColonToken)
		{
			// Consume the 'open parenthesis' / 'comma'
			_ = parserModel.TokenWalker.Consume();
			
			// Consume the identifierToken
			var token = parserModel.TokenWalker.Consume();
			parserModel.Binder.CreateVariableSymbol(
		        UtilityApi.ConvertToIdentifierToken(ref token, compilationUnit, ref parserModel),
		        VariableKind.Local,
		        compilationUnit,
		        ref parserModel);
			
			// Consume the ColonToken
			_ = parserModel.TokenWalker.Consume();
		}
		else
		{
			if (guaranteeConsume)
			{
				// Consume the 'open parenthesis'
				// (this is a hack for constructor definition secondary syntax)
				_ = parserModel.TokenWalker.Consume();
			}
		}
	
		return EmptyExpressionNode.Empty;
	}
	
	public IExpressionNode FunctionDefinitionMergeToken(
		IFunctionDefinitionNode functionDefinitionNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CommaToken:
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, functionDefinitionNode));
				return EmptyExpressionNode.Empty;
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), functionDefinitionNode, token);
		}
	}
	
	public IExpressionNode FunctionDefinitionMergeExpression(
		IFunctionDefinitionNode functionDefinitionNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			return functionDefinitionNode;
			
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			expressionSecondary = ForceDecisionAmbiguousIdentifier(
				functionDefinitionNode,
				(AmbiguousIdentifierExpressionNode)expressionSecondary,
				compilationUnit,
				ref parserModel);
		}
			
		functionDefinitionNode.FunctionArgumentListing.FunctionArgumentEntryList.Add(
			new FunctionArgumentEntry(
		        variableDeclarationNode: null,
		        optionalCompileTimeConstantToken: null,
		        isOptional: false,
		        hasParamsKeyword: false,
		        hasOutKeyword: false,
		        hasInKeyword: false,
		        hasRefKeyword: false));
		
		return functionDefinitionNode;
	}
	
	public IExpressionNode GenericParametersListingMergeToken(
		IGenericParameterNode genericParameterNode, ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (UtilityApi.IsConvertibleToTypeClauseNode(token.SyntaxKind))
		{
			var typeClauseNode = UtilityApi.ConvertTokenToTypeClauseNode(ref token, compilationUnit, ref parserModel);
			
			BindTypeClauseNode(
		        typeClauseNode,
		        compilationUnit,
		        ref parserModel);
			
			// TODO: Does typeClauseNode -> Generic params?
			return typeClauseNode;
		}
	
		switch (token.SyntaxKind)
		{
			case SyntaxKind.CommaToken:
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, genericParameterNode));
				return EmptyExpressionNode.Empty;
			case SyntaxKind.CloseAngleBracketToken:
				// This case only occurs when the text won't compile.
				// i.e.: "<int>" rather than "MyClass<int>".
				// The case is for when the user types just the generic parameter listing text without an identifier before it.
				//
				// In the case of "SomeMethod<int>()", the FunctionInvocationNode
				// is expected to have ran 'parserModel.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, functionInvocationNode));'
				// to receive the genericParametersListingNode.
				return genericParameterNode;
			case SyntaxKind.OpenParenthesisToken:
				return ShareEmptyExpressionNodeIntoOpenParenthesisTokenCase(ref token, compilationUnit, ref parserModel);
			default:
				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), genericParameterNode, token);
		}
	}
	
	public IExpressionNode GenericParametersListingMergeExpression(
		IGenericParameterNode genericParameterNode, IExpressionNode expressionSecondary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
	{
		if (expressionSecondary.SyntaxKind == SyntaxKind.EmptyExpressionNode)
			return genericParameterNode;
			
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
			genericParameterNode.IsParsingGenericParameters = false;
	
		if (expressionSecondary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
		{
			var expressionSecondaryTyped = (AmbiguousIdentifierExpressionNode)expressionSecondary;
			
			var token = expressionSecondaryTyped.Token;
			var typeClauseNode = UtilityApi.ConvertTokenToTypeClauseNode(ref token, compilationUnit, ref parserModel);
			
			// TODO: Is this running everytime a parameter is added???...
			// ...only do this at the end?
			BindTypeClauseNode(
		        typeClauseNode,
		        compilationUnit,
		        ref parserModel);
			
			genericParameterNode.GenericParameterListing.GenericParameterEntryList.Add(
				new GenericParameterEntry(new TypeReference(typeClauseNode)));
			
			return genericParameterNode;
		}
		else if (expressionSecondary.SyntaxKind == SyntaxKind.TypeClauseNode)
		{
			var typeClauseNode = (TypeClauseNode)expressionSecondary;
		
			genericParameterNode.GenericParameterListing.GenericParameterEntryList.Add(
				new GenericParameterEntry(new TypeReference(typeClauseNode)));
			
			return genericParameterNode;
		}
		
		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeReference(), genericParameterNode, expressionSecondary);
	}
	
	public IExpressionNode ParseGenericParameterNode_Start(
		IGenericParameterNode genericParameterNode, ref SyntaxToken openAngleBracketToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel, IExpressionNode nodeToRestoreAtCloseAngleBracketToken = null)
	{
		nodeToRestoreAtCloseAngleBracketToken ??= genericParameterNode;
	
		if (!genericParameterNode.GenericParameterListing.ConstructorWasInvoked)
		{
			genericParameterNode.GenericParameterListing =
				new GenericParameterListing(
					openAngleBracketToken,
					// Idea: 1 listing for the entire file and store the indices at which your parameters lie?
			        new List<GenericParameterEntry>(),
			        closeAngleBracketToken: default);

			genericParameterNode.IsParsingGenericParameters = true;
		}
		
	    parserModel.ExpressionList.Add((SyntaxKind.CloseAngleBracketToken, nodeToRestoreAtCloseAngleBracketToken));
		parserModel.ExpressionList.Add((SyntaxKind.CommaToken, genericParameterNode));
		return genericParameterNode;
	}
}
