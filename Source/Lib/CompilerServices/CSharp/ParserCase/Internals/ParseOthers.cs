using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseOthers
{
    public static void HandleNamespaceReference(
        IdentifierToken consumedIdentifierToken,
        NamespaceGroupNode resolvedNamespaceGroupNode,
        CSharpParserModel model)
    {
        model.Binder.BindNamespaceReference(consumedIdentifierToken, model);

        if (SyntaxKind.MemberAccessToken == model.TokenWalker.Current.SyntaxKind)
        {
            var memberAccessToken = model.TokenWalker.Consume();
            var memberIdentifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

            if (memberIdentifierToken.IsFabricated)
            {
                model.DiagnosticBag.ReportUnexpectedToken(
                    model.TokenWalker.Current.TextSpan,
                    model.TokenWalker.Current.SyntaxKind.ToString(),
                    SyntaxKind.IdentifierToken.ToString());
            }

            // Check all the TypeDefinitionNodes that are in the namespace
            var typeDefinitionNodes = resolvedNamespaceGroupNode.GetTopLevelTypeDefinitionNodes();

            var typeDefinitionNode = typeDefinitionNodes.SingleOrDefault(td =>
                td.TypeIdentifierToken.TextSpan.GetText() == memberIdentifierToken.TextSpan.GetText());

            if (typeDefinitionNode is null)
            {
                model.DiagnosticBag.ReportNotDefinedInContext(
                    model.TokenWalker.Current.TextSpan,
                    consumedIdentifierToken.TextSpan.GetText());
            }
            else
            {
                ParseTypes.HandleTypeReference(
                    memberIdentifierToken,
                    typeDefinitionNode,
                    model);
            }
        }
        else
        {
            // TODO: (2023-05-28) Report an error diagnostic for 'namespaces are not statements'. Something like this I'm not sure.
            model.TokenWalker.Consume();
        }
    }

    public static void HandleNamespaceIdentifier(CSharpParserModel model)
    {
        var combineNamespaceIdentifierIntoOne = new List<ISyntaxToken>();

        while (!model.TokenWalker.IsEof)
        {
            if (combineNamespaceIdentifierIntoOne.Count % 2 == 0)
            {
                var matchedToken = model.TokenWalker.Match(SyntaxKind.IdentifierToken);
                combineNamespaceIdentifierIntoOne.Add(matchedToken);

                if (matchedToken.IsFabricated)
                    break;
            }
            else
            {
                if (SyntaxKind.MemberAccessToken == model.TokenWalker.Current.SyntaxKind)
                    combineNamespaceIdentifierIntoOne.Add(model.TokenWalker.Consume());
                else
                    break;
            }
        }

        if (combineNamespaceIdentifierIntoOne.Count == 0)
        {
            model.SyntaxStack.Push(new EmptyNode());
            return;
        }

        var identifierTextSpan = combineNamespaceIdentifierIntoOne.First().TextSpan with
        {
            EndingIndexExclusive = combineNamespaceIdentifierIntoOne.Last().TextSpan.EndingIndexExclusive
        };

        model.SyntaxStack.Push(new IdentifierToken(identifierTextSpan));
    }

    public static void HandleExpression(
        IExpressionNode? topMostExpressionNode,
        IExpressionNode? previousInvocationExpressionNode,
        IExpressionNode? leftExpressionNode,
        ISyntaxToken? operatorToken,
        IExpressionNode? rightExpressionNode,
        ExpressionDelimiter[]? extraExpressionDeliminaters,
        CSharpParserModel model)
    {
        while (!model.TokenWalker.IsEof)
        {
            var tokenCurrent = model.TokenWalker.Consume();
            
            if (tokenCurrent.SyntaxKind == SyntaxKind.NewTokenKeyword) // Constructor Invocation
            	model.SyntaxStack.Push(tokenCurrent);

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken || tokenCurrent.SyntaxKind == SyntaxKind.StatementDelimiterToken)
            {
                model.TokenWalker.Backtrack();
                break;
            }

            ExpressionDelimiter? closeExtraExpressionDelimiterEncountered =
                extraExpressionDeliminaters?.FirstOrDefault(x => x.CloseSyntaxKind == tokenCurrent.SyntaxKind);

            if (closeExtraExpressionDelimiterEncountered is not null)
            {
                if (tokenCurrent.SyntaxKind == SyntaxKind.CloseParenthesisToken)
                {
                    if (closeExtraExpressionDelimiterEncountered?.OpenSyntaxToken is not null)
                    {
                        ParenthesizedExpressionNode parenthesizedExpression;

                        if (previousInvocationExpressionNode is not null)
                        {
                            parenthesizedExpression = new ParenthesizedExpressionNode(
                                (OpenParenthesisToken)closeExtraExpressionDelimiterEncountered.OpenSyntaxToken,
                                previousInvocationExpressionNode,
                                (CloseParenthesisToken)tokenCurrent);
                        }
                        else
                        {
                            parenthesizedExpression = new ParenthesizedExpressionNode(
                                (OpenParenthesisToken)closeExtraExpressionDelimiterEncountered.OpenSyntaxToken,
                                new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause()),
                                (CloseParenthesisToken)tokenCurrent);
                        }

                        model.SyntaxStack.Push(parenthesizedExpression);
                        return;
                    }
                    else
                    {
                        // If one provides 'CloseParenthesisToken' as a closing delimiter,
                        // but does not provide the corresponding open delimiter (it is null)
                        // then a function invocation started the initial invocation
                        // of this method.
                        model.TokenWalker.Backtrack();
                        break;
                    }
                }
                else if (tokenCurrent.SyntaxKind == SyntaxKind.CommaToken ||
                         tokenCurrent.SyntaxKind == SyntaxKind.CloseBraceToken)
                {
                    model.TokenWalker.Backtrack();
                    break;
                }
            }

            switch (tokenCurrent.SyntaxKind)
            {
                case SyntaxKind.TrueTokenKeyword:
                case SyntaxKind.FalseTokenKeyword:
                    var booleanLiteralExpressionNode = new LiteralExpressionNode(tokenCurrent, CSharpFacts.Types.Bool.ToTypeClause());
                    previousInvocationExpressionNode = booleanLiteralExpressionNode;
                    SetLiteralExpressionNode(booleanLiteralExpressionNode);
                    break;
                case SyntaxKind.NumericLiteralToken:
                    var numericLiteralExpressionNode = new LiteralExpressionNode(tokenCurrent, CSharpFacts.Types.Int.ToTypeClause());
                    previousInvocationExpressionNode = numericLiteralExpressionNode;
                    SetLiteralExpressionNode(numericLiteralExpressionNode);
                    break;
                case SyntaxKind.CharLiteralToken:
                    var charLiteralExpressionNode = new LiteralExpressionNode(tokenCurrent, CSharpFacts.Types.Char.ToTypeClause());
                    previousInvocationExpressionNode = charLiteralExpressionNode;
                    SetLiteralExpressionNode(charLiteralExpressionNode);
                    break;
                case SyntaxKind.StringLiteralToken:
                    var stringLiteralExpressionNode = new LiteralExpressionNode(tokenCurrent, CSharpFacts.Types.String.ToTypeClause());
                    previousInvocationExpressionNode = stringLiteralExpressionNode;
                    SetLiteralExpressionNode(stringLiteralExpressionNode);
                    break;
                case SyntaxKind.IdentifierToken:
                    // 'resultingExpression' given the identifier.
                    IExpressionNode resultingExpression;

                    if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
                        model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
                    {
                        var genericParametersListingNode = (GenericParametersListingNode?)null;
                        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
                        {
                            var openAngleBracketToken = (OpenAngleBracketToken)model.TokenWalker.Consume();
                            ParseTypes.HandleGenericParameters(openAngleBracketToken, model);
                            genericParametersListingNode = (GenericParametersListingNode)model.SyntaxStack.Pop();
                        }

                        FunctionParametersListingNode functionParametersListingNode;
                        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
                        {
                            var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Consume();
                            ParseFunctions.HandleFunctionParameters(openParenthesisToken, model);
                            functionParametersListingNode = (FunctionParametersListingNode)model.SyntaxStack.Pop();
                        }
                        else
                        {
                            functionParametersListingNode = new FunctionParametersListingNode(
                                (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken),
                                ImmutableArray<FunctionParameterEntryNode>.Empty,
                                (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken));
                        }
                        
                        if (model.SyntaxStack.TryPeek(out var syntax) &&
                        	syntax.SyntaxKind == SyntaxKind.NewTokenKeyword)
                        {
                        	// Constructor invocation
                        	var newKeywordToken = model.SyntaxStack.Pop();

					        var typeClauseNode = new TypeClauseNode(
					        	(IdentifierToken)tokenCurrent,
					        	valueType: null,
					        	genericParametersListingNode);
					        	
            				model.Binder.BindTypeClauseNode(typeClauseNode, model);
	
	                        var constructorInvocationNode = new ConstructorInvocationExpressionNode(
						        (KeywordToken)newKeywordToken,
						        typeClauseNode,
						        functionParametersListingNode,
						        objectInitializationParametersListingNode: null);
	
	                        resultingExpression = constructorInvocationNode;
                        }
                        else
                        {
                        	// Function invocation
                        	model.Binder.TryGetFunctionHierarchically(
	                            tokenCurrent.TextSpan.GetText(),
	                            model.BinderSession.CurrentScope,
	                            out var functionDefinitionNode);
	
	                        var functionInvocationNode = new FunctionInvocationNode(
	                            (IdentifierToken)tokenCurrent,
	                            functionDefinitionNode,
	                            genericParametersListingNode,
	                            functionParametersListingNode,
	                            functionDefinitionNode?.ReturnTypeClauseNode ?? CSharpFacts.Types.Void.ToTypeClause());
	
	                        model.Binder.BindFunctionInvocationNode(functionInvocationNode, model);
	
	                        resultingExpression = functionInvocationNode;
                        }
                    }
                    else
                    {
                        resultingExpression = model.Binder.ConstructAndBindVariableReferenceNode(
                            (IdentifierToken)tokenCurrent,
                            model);
                    }

                    if (topMostExpressionNode is null)
                        topMostExpressionNode = resultingExpression;
                    else if (leftExpressionNode is null)
                        leftExpressionNode = resultingExpression;
                    else if (rightExpressionNode is null)
                        rightExpressionNode = resultingExpression;
                    else
                        model.DiagnosticBag.ReportTodoException(resultingExpression.ConstructTextSpanRecursively(), $"{nameof(HandleExpression)} IdentifierToken issue text:{resultingExpression.ConstructTextSpanRecursively().GetText()}");

                    break;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.StarToken:
                case SyntaxKind.DivisionToken:
                    if (leftExpressionNode is null && previousInvocationExpressionNode is not null)
                        leftExpressionNode = previousInvocationExpressionNode;

                    if (previousInvocationExpressionNode is BinaryExpressionNode previousBinaryExpressionNode)
                    {
                        var previousOperatorPrecedence = UtilityApi.GetOperatorPrecedence(previousBinaryExpressionNode.BinaryOperatorNode.OperatorToken.SyntaxKind);
                        var currentOperatorPrecedence = UtilityApi.GetOperatorPrecedence(tokenCurrent.SyntaxKind);

                        if (currentOperatorPrecedence > previousOperatorPrecedence)
                        {
                            // Take the right node from the previous expression.
                            // Make it the new expression's left node.
                            //
                            // Then replace the previous expression's right node with the
                            // newly formed expression.

                            HandleExpression(
                                topMostExpressionNode,
                                null,
                                previousBinaryExpressionNode.RightExpressionNode,
                                tokenCurrent,
                                null,
                                extraExpressionDeliminaters,
                                model);

                            var modifiedRightExpressionNode = (IExpressionNode)model.SyntaxStack.Pop();

                            topMostExpressionNode = new BinaryExpressionNode(
                                previousBinaryExpressionNode.LeftExpressionNode,
                                previousBinaryExpressionNode.BinaryOperatorNode,
                                modifiedRightExpressionNode);
                        }
                    }

                    if (operatorToken is null)
                        operatorToken = tokenCurrent;
                    else
                        model.DiagnosticBag.ReportTodoException(tokenCurrent.TextSpan, $"{nameof(HandleExpression)} DivisionToken issue text:{tokenCurrent.TextSpan.GetText()}");

                    break;
                case SyntaxKind.OpenParenthesisToken:
                
                	// Goal: Start parsing 'ExplicitCastNode' (2024-10-04)
                	if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.IdentifierToken &&
                		model.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseParenthesisToken)
                	{
                		// Explicit Cast
                		
                		var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);
                		model.Binder.BindTypeClauseNode(typeClauseNode, model);
                		
                		var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
                	
                		var explicitCastNode = new ExplicitCastNode(
					        (OpenParenthesisToken)tokenCurrent,
					        typeClauseNode,
					        closeParenthesisToken,
					        new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause()));
                		break;
                	}
                	else
                	{
	                    var copyExtraExpressionDeliminaters = new List<ExpressionDelimiter>(extraExpressionDeliminaters ?? Array.Empty<ExpressionDelimiter>());
	
						// TODO: This doesn't add delimiters to the parent invocation of the method right? Because that seemingly would be very wrong?
	                    copyExtraExpressionDeliminaters.Insert(0, new ExpressionDelimiter(
	                        SyntaxKind.OpenParenthesisToken,
	                        SyntaxKind.CloseParenthesisToken,
	                        tokenCurrent,
	                        null));
	
	                    HandleExpression(
	                        null,
	                        null,
	                        null,
	                        null,
	                        null,
	                        copyExtraExpressionDeliminaters.ToArray(),
	                        model);
	
	                    var parenthesizedExpression = (IExpressionNode)model.SyntaxStack.Pop();
	
	                    previousInvocationExpressionNode = parenthesizedExpression;
	
	                    if (topMostExpressionNode is null)
	                        topMostExpressionNode = parenthesizedExpression;
	                    else if (leftExpressionNode is null)
	                        leftExpressionNode = parenthesizedExpression;
	                    else if (rightExpressionNode is null)
	                        rightExpressionNode = parenthesizedExpression;
	                    else
	                        model.DiagnosticBag.ReportTodoException(parenthesizedExpression.ConstructTextSpanRecursively(), $"{nameof(HandleExpression)} OpenParenthesisToken issue text:{parenthesizedExpression.ConstructTextSpanRecursively().GetText()}");
	                    break;
                    }
                default:
                    if (tokenCurrent.SyntaxKind == SyntaxKind.DollarSignToken)
                    {
                        // TODO: Convert DollarSignToken to a function signature...
                        // ...Then read in the parameters...
                        // ...Any function invocation logic also would be done here

                        model.Binder.BindStringInterpolationExpression(
                            (DollarSignToken)tokenCurrent,
                            model);
                    }
                    else if (tokenCurrent.SyntaxKind == SyntaxKind.AtToken)
                    {
                    	model.Binder.BindStringVerbatimExpression(
                            (AtToken)tokenCurrent,
                            model);
                    }

                    break;
            }

            if (leftExpressionNode is not null && operatorToken is not null && rightExpressionNode is not null)
            {
                var binaryOperatorNode = model.Binder.BindBinaryOperatorNode(
                    leftExpressionNode,
                    operatorToken,
                    rightExpressionNode,
                    model);

                var binaryExpressionNode = new BinaryExpressionNode(
                    leftExpressionNode,
                    binaryOperatorNode,
                    rightExpressionNode);

                topMostExpressionNode = binaryExpressionNode;
                previousInvocationExpressionNode = binaryExpressionNode;

                leftExpressionNode = null;
                operatorToken = null;
                rightExpressionNode = null;

                HandleExpression(
                    topMostExpressionNode,
                    previousInvocationExpressionNode,
                    leftExpressionNode,
                    operatorToken,
                    rightExpressionNode,
                    extraExpressionDeliminaters,
                    model);

                return;
            }
        }

        var fallbackExpressionNode = new LiteralExpressionNode(
            new EndOfFileToken(new(0, 0, (byte)GenericDecorationKind.None, ResourceUri.Empty, string.Empty)),
            CSharpFacts.Types.Void.ToTypeClause());

        model.SyntaxStack.Push(topMostExpressionNode ?? fallbackExpressionNode);

        void SetLiteralExpressionNode(IExpressionNode literalExpressionNode)
        {
            if (topMostExpressionNode is null)
            {
                topMostExpressionNode = literalExpressionNode;
            }
            else if (leftExpressionNode is null)
            {
                if (topMostExpressionNode.SyntaxKind != SyntaxKind.LiteralExpressionNode)
                    leftExpressionNode = literalExpressionNode;
            }
            else if (rightExpressionNode is null)
            {
                if (topMostExpressionNode.SyntaxKind != SyntaxKind.LiteralExpressionNode)
                    rightExpressionNode = literalExpressionNode;
            }
            else
            {
                model.DiagnosticBag.ReportTodoException(literalExpressionNode.ConstructTextSpanRecursively(), $"{nameof(HandleExpression)} LiteralExpressionNode issue text:{literalExpressionNode.ConstructTextSpanRecursively().GetText()}");
            }
        }
    }
}