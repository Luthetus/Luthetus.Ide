using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseTokens
{
    public static void ParseNumericLiteralToken(
        NumericLiteralToken consumedNumericLiteralToken,
        CSharpParserModel model)
    {
        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

	public static void ParseCharLiteralToken(
        CharLiteralToken consumedCharLiteralToken,
        CSharpParserModel model)
    {
        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParseStringLiteralToken(
        StringLiteralToken consumedStringLiteralToken,
        CSharpParserModel model)
    {
        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParsePreprocessorDirectiveToken(
        PreprocessorDirectiveToken consumedPreprocessorDirectiveToken,
        CSharpParserModel model)
    {
        var consumedToken = model.TokenWalker.Consume();

        if (consumedToken.SyntaxKind == SyntaxKind.LibraryReferenceToken)
        {
            var preprocessorLibraryReferenceStatement = new PreprocessorLibraryReferenceStatementNode(
                consumedPreprocessorDirectiveToken,
                consumedToken);

            model.CurrentCodeBlockBuilder.ChildList.Add(preprocessorLibraryReferenceStatement);
            return;
        }
        else
        {
            model.DiagnosticBag.ReportTodoException(
                consumedToken.TextSpan,
                $"Implement {nameof(ParsePreprocessorDirectiveToken)}");
        }
    }

    public static void ParseIdentifierToken(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax is AmbiguousIdentifierNode)
            ResolveAmbiguousIdentifier((AmbiguousIdentifierNode)model.SyntaxStack.Pop(), model);

        if (TryParseTypedIdentifier(consumedIdentifierToken, model))
            return;

        if (TryParseConstructorDefinition(consumedIdentifierToken, model))
            return;

        if (TryParseVariableAssignment(consumedIdentifierToken, model))
            return;

        if (TryParseGenericTypeOrFunctionInvocation(consumedIdentifierToken, model))
            return;

        if (TryParseReference(consumedIdentifierToken, model))
            return;

        return;
    }

    private static bool TryParseGenericArguments(CSharpParserModel model)
    {
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            ParseTypes.HandleGenericArguments(
                (OpenAngleBracketToken)model.TokenWalker.Consume(),
                model);

            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool TryParseGenericParameters(
        CSharpParserModel model,
        out GenericParametersListingNode? genericParametersListingNode)
    {
        if (SyntaxKind.OpenAngleBracketToken == model.TokenWalker.Current.SyntaxKind)
        {
            ParseTypes.HandleGenericParameters(
                (OpenAngleBracketToken)model.TokenWalker.Consume(),
                model);

            genericParametersListingNode = (GenericParametersListingNode?)model.SyntaxStack.Pop();
            return true;
        }
        else
        {
            genericParametersListingNode = null;
            return false;
        }
    }

    private static bool TryParseConstructorDefinition(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken &&
            model.CurrentCodeBlockBuilder.CodeBlockOwner is not null &&
            model.CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind == SyntaxKind.TypeDefinitionNode)
        {
            ParseFunctions.HandleConstructorDefinition(consumedIdentifierToken, model);
            return true;
        }

        return false;
    }

    private static bool TryParseTypedIdentifier(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax is TypeClauseNode typeClauseNode)
        {
            // The variable 'genericArgumentsListingNode' is here for
            // when the syntax is determined to be a function definition.
            // In this case, the typeClauseNode would be the function's return type.
            // Yet, there may still be generic arguments to the function.
            var genericArgumentsListingNode = (GenericArgumentsListingNode?)null;

            if (TryParseGenericArguments(model) &&
                model.SyntaxStack.Peek().SyntaxKind == SyntaxKind.GenericArgumentsListingNode)
            {
                genericArgumentsListingNode = (GenericArgumentsListingNode)model.SyntaxStack.Pop();
            }

            if (TryParseFunctionDefinition(
                consumedIdentifierToken,
                typeClauseNode,
                genericArgumentsListingNode,
                model))
            {
                return true;
            }

            if (TryParseVariableDeclaration(
                typeClauseNode,
                consumedIdentifierToken,
                model))
            {
                return true;
            }
        }
        
        return false;
    }

    private static bool TryParseReference(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        var text = consumedIdentifierToken.TextSpan.GetText();

        if (model.Binder.NamespaceGroupNodes.TryGetValue(text, out var namespaceGroupNode) &&
            namespaceGroupNode is not null)
        {
            ParseOthers.HandleNamespaceReference(consumedIdentifierToken, namespaceGroupNode, model);
            return true;
        }
        else
        {
            if (model.Binder.TryGetVariableDeclarationHierarchically(
            		model,
                    model.BinderSession.ResourceUri,
                    model.BinderSession.CurrentScopeIndexKey,
                    text,
                    out var variableDeclarationStatementNode) &&
                variableDeclarationStatementNode is not null)
            {
                ParseVariables.HandleVariableReference(consumedIdentifierToken, model);
                return true;
            }
            else
            {
                // 'static class identifier' OR 'undeclared-variable reference'
                if (model.Binder.TryGetTypeDefinitionHierarchically(
                		model,
                        model.BinderSession.ResourceUri,
                        model.BinderSession.CurrentScopeIndexKey,
                        text,
                        out var typeDefinitionNode) &&
                    typeDefinitionNode is not null)
                {
                    ParseTypes.HandleStaticClassIdentifier(consumedIdentifierToken, model);
                    return true;
                }
                else
                {
                    ParseTypes.HandleUndefinedTypeOrNamespaceReference(consumedIdentifierToken, model);
                    return true;
                }
            }
        }
    }

    private static bool TryParseVariableAssignment(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
        {
            ParseVariables.HandleVariableAssignment(
                consumedIdentifierToken,
                (EqualsToken)model.TokenWalker.Consume(),
                model);
            return true;
        }

        return false;
    }

    private static bool TryParseGenericTypeOrFunctionInvocation(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        if (model.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken &&
            model.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenAngleBracketToken)
        {
            return false;
        }

        if (TryParseGenericParameters(model, out var genericParametersListingNode))
        {
            if (model.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            {
                // Generic type
                var typeClauseNode = new TypeClauseNode(
                    consumedIdentifierToken,
                    null,
                    genericParametersListingNode);

                model.Binder.BindTypeClauseNode(typeClauseNode, model);
                model.SyntaxStack.Push(typeClauseNode);
                return true;
            }
        }

        // Function invocation
        ParseFunctions.HandleFunctionInvocation(
            consumedIdentifierToken,
            genericParametersListingNode,
            model);

        return true;
    }

    private static bool TryParseFunctionDefinition(
        IdentifierToken consumedIdentifierToken,
        TypeClauseNode consumedTypeClauseNode,
        GenericArgumentsListingNode? consumedGenericArgumentsListingNode,
        CSharpParserModel model)
    {
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
        {
            ParseFunctions.HandleFunctionDefinition(
                consumedIdentifierToken,
                consumedTypeClauseNode,
                consumedGenericArgumentsListingNode,
                model);

            return true;
        }

        return false;
    }

    private static bool TryParseVariableDeclaration(
        TypeClauseNode consumedTypeClauseNode,
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        var isLocalOrField = model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
                             model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken;

        var isLambda = model.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken;

        var variableKind = (VariableKind?)null;

        if (isLocalOrField && !isLambda)
        {
            if (model.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode)
                variableKind = VariableKind.Field;
            else
                variableKind = VariableKind.Local;
        }
        else if (isLambda)
        {
            // Property (expression bound)
            variableKind = VariableKind.Property;
        }
        else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
        {
            // Property
            variableKind = VariableKind.Property;
        }

        if (variableKind is not null)
        {
            ParseVariables.HandleVariableDeclarationStatement(
                consumedTypeClauseNode,
                consumedIdentifierToken,
                variableKind.Value,
                model);

            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ResolveAmbiguousIdentifier(
        AmbiguousIdentifierNode consumedAmbiguousIdentifierNode,
        CSharpParserModel model)
    {
        var expectingTypeClause = false;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            expectingTypeClause = true;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            expectingTypeClause = true;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken ||
            model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
            model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
        {
            expectingTypeClause = true;
        }

        if (expectingTypeClause)
        {
            if (!model.Binder.TryGetTypeDefinitionHierarchically(
            		model,
                    model.BinderSession.ResourceUri,
                    model.BinderSession.CurrentScopeIndexKey,
                    consumedAmbiguousIdentifierNode.IdentifierToken.TextSpan.GetText(),
                    out var typeDefinitionNode)
                || typeDefinitionNode is null)
            {
                var fabricateTypeDefinition = new TypeDefinitionNode(
                    AccessModifierKind.Public,
                    false,
                    StorageModifierKind.Class,
                    consumedAmbiguousIdentifierNode.IdentifierToken,
                    null,
                    null,
                    null,
                    null,
                    openBraceToken: default,
                    null)
                {
                    IsFabricated = true
                };

                model.Binder.BindTypeDefinitionNode(fabricateTypeDefinition, model);
                typeDefinitionNode = fabricateTypeDefinition;
            }

            model.SyntaxStack.Push(typeDefinitionNode.ToTypeClause());
        }
    }

    public static void ParsePlusToken(
        PlusToken consumedPlusToken,
        CSharpParserModel model)
    {
        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParsePlusPlusToken(
        PlusPlusToken consumedPlusPlusToken,
        CSharpParserModel model)
    {
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.VariableReferenceNode)
        {
            var variableReferenceNode = (VariableReferenceNode)model.SyntaxStack.Pop();

            var unaryOperatorNode = new UnaryOperatorNode(
                variableReferenceNode.ResultTypeClauseNode,
                consumedPlusPlusToken,
                variableReferenceNode.ResultTypeClauseNode);

            var unaryExpressionNode = new UnaryExpressionNode(
                variableReferenceNode,
                unaryOperatorNode);

            model.CurrentCodeBlockBuilder.ChildList.Add(unaryExpressionNode);
        }
    }

    public static void ParseMinusToken(
        MinusToken consumedMinusToken,
        CSharpParserModel model)
    {
        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParseStarToken(
        StarToken consumedStarToken,
        CSharpParserModel model)
    {
        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParseDollarSignToken(
        DollarSignToken consumedDollarSignToken,
        CSharpParserModel model)
    {
        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }
    
    public static void ParseAtToken(
        AtToken consumedAtToken,
        CSharpParserModel model)
    {
        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParseColonToken(
        ColonToken consumedColonToken,
        CSharpParserModel model)
    {
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
        {
            var typeDefinitionNode = (TypeDefinitionNode)model.SyntaxStack.Pop();
            var inheritedTypeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

            model.Binder.BindTypeClauseNode(inheritedTypeClauseNode, model);

			typeDefinitionNode.SetInheritedTypeClauseNode(inheritedTypeClauseNode);

            model.SyntaxStack.Push(typeDefinitionNode);
            model.CurrentCodeBlockBuilder.PendingChild = typeDefinitionNode;
            
            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.WhereTokenContextualKeyword)
	        {
	        	while (!model.TokenWalker.IsEof)
	        	{
	        		if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
	        			break;
	        		
	        		_ = model.TokenWalker.Consume();
	        	}
	        }
        }
        else
        {
            model.DiagnosticBag.ReportTodoException(consumedColonToken.TextSpan, "Colon is in unexpected place.");
        }
    }

    public static void ParseOpenBraceToken(
        OpenBraceToken consumedOpenBraceToken,
        CSharpParserModel model)
    {
		var closureCurrentCodeBlockBuilder = model.CurrentCodeBlockBuilder;
        ICodeBlockOwner? nextCodeBlockOwner = null;
        TypeClauseNode? scopeReturnTypeClauseNode = null;

		if (closureCurrentCodeBlockBuilder.PendingChild is null)
		{
			var arbitraryCodeBlockNode = new ArbitraryCodeBlockNode(
				closureCurrentCodeBlockBuilder.CodeBlockOwner);
		
			model.SyntaxStack.Push(arbitraryCodeBlockNode);
        	model.CurrentCodeBlockBuilder.PendingChild = arbitraryCodeBlockNode;
		}

		var parentScopeDirection = model.CurrentCodeBlockBuilder?.CodeBlockOwner?.ScopeDirectionKind
			?? ScopeDirectionKind.Both;
			
		bool wasDeferred = false;

		if (parentScopeDirection == ScopeDirectionKind.Both)
		{
			// Retrospective: ??? How would two consecutive defers get enqueued if doing '== 0'.
			//
			// Response: This seems to be a flag that says a child scope is allowed to be parsed (rather than infinitely enqueueing).
			//           If so, rename the variable and make it a bool because it being a 'counter' is extremely confusing.
			if (model.CurrentCodeBlockBuilder.DequeueChildScopeCounter == 0)
			{
				model.TokenWalker.DeferParsingOfChildScope(consumedOpenBraceToken, model);
				return;
			}

			model.CurrentCodeBlockBuilder.DequeueChildScopeCounter--;
			wasDeferred = true;
		}

		var indexToUpdateAfterDequeue = model.CurrentCodeBlockBuilder.DequeuedIndexForChildList
			?? model.CurrentCodeBlockBuilder.ChildList.Count;
		
		if (closureCurrentCodeBlockBuilder.PendingChild is null)
			return;
		
		nextCodeBlockOwner = closureCurrentCodeBlockBuilder.PendingChild;
		
		var returnTypeClauseNode = nextCodeBlockOwner.GetReturnTypeClauseNode();
		if (returnTypeClauseNode is not null)
			scopeReturnTypeClauseNode = returnTypeClauseNode;
		
		// Awkwardly capturing this variable so the closure on 'FinalizeCodeBlockNodeActionStack'
		// Reads better. Because it will be used AFTER the code block was read so 'next' is nonsense.
		var selfCodeBlockOwner = nextCodeBlockOwner;
		
		model.FinalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
        {
            selfCodeBlockOwner = selfCodeBlockOwner.SetCodeBlockNode(consumedOpenBraceToken, codeBlockNode);
			
			/*if (wasDeferred)
				closureCurrentCodeBlockBuilder.ChildList[indexToUpdateAfterDequeue] = selfCodeBlockOwner;
			else
				closureCurrentCodeBlockBuilder.ChildList.Add(selfCodeBlockOwner);*/
			
			if (selfCodeBlockOwner.SyntaxKind != SyntaxKind.TryStatementTryNode &&
				selfCodeBlockOwner.SyntaxKind != SyntaxKind.TryStatementCatchNode &&
				selfCodeBlockOwner.SyntaxKind != SyntaxKind.TryStatementFinallyNode)
			{
				closureCurrentCodeBlockBuilder.ChildList.Add(selfCodeBlockOwner);
			}
			
			closureCurrentCodeBlockBuilder.PendingChild = null;
				
			if (selfCodeBlockOwner.SyntaxKind == SyntaxKind.NamespaceStatementNode)
				model.Binder.BindNamespaceStatementNode((NamespaceStatementNode)selfCodeBlockOwner, model);
			else if (selfCodeBlockOwner.SyntaxKind == SyntaxKind.TypeDefinitionNode)
				model.Binder.BindTypeDefinitionNode((TypeDefinitionNode)selfCodeBlockOwner, model, true);
        });

        model.Binder.RegisterScope(scopeReturnTypeClauseNode, consumedOpenBraceToken.TextSpan, model);
		model.CurrentCodeBlockBuilder = new(model.CurrentCodeBlockBuilder, nextCodeBlockOwner);
		nextCodeBlockOwner.OnBoundScopeCreatedAndSetAsCurrent(model);
    }

    public static void ParseCloseBraceToken(
        CloseBraceToken consumedCloseBraceToken,
        CSharpParserModel model)
    {
		if (model.CurrentCodeBlockBuilder.ParseChildScopeQueue.TryDequeue(out var action))
		{
			action.Invoke(model.TokenWalker.Index - 1);
			model.CurrentCodeBlockBuilder.DequeueChildScopeCounter++;
			return;
		}

        model.Binder.DisposeScope(consumedCloseBraceToken.TextSpan, model);

        if (model.CurrentCodeBlockBuilder.Parent is not null && model.FinalizeCodeBlockNodeActionStack.Any())
        {
            model.FinalizeCodeBlockNodeActionStack
                .Pop()
                .Invoke(model.CurrentCodeBlockBuilder.Build());

            model.CurrentCodeBlockBuilder = model.CurrentCodeBlockBuilder.Parent;
        }
    }

    public static void ParseOpenParenthesisToken(
        OpenParenthesisToken consumedOpenParenthesisToken,
        CSharpParserModel model)
    {
        if (model.SyntaxStack.TryPeek(out var syntax) &&
            syntax is TypeDefinitionNode typeDefinitionNode)
        {
            _ = model.SyntaxStack.Pop();

            ParseTypes.HandlePrimaryConstructorDefinition(
                typeDefinitionNode,
                consumedOpenParenthesisToken,
                model);

            return;
        }

		_ = model.TokenWalker.Backtrack();
		var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParseCloseParenthesisToken(
        CloseParenthesisToken consumedCloseParenthesisToken,
        CSharpParserModel model)
    {
    }

    public static void ParseOpenAngleBracketToken(
        OpenAngleBracketToken consumedOpenAngleBracketToken,
        CSharpParserModel model)
    {
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
            model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
            model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.BinaryExpressionNode ||
            /* Prefer the enum comparison. Will short circuit. This "is" cast is for fallback in case someone in the future adds for expression syntax kinds but does not update this if statement TODO: Check if node ends with "ExpressionNode"? */
            model.SyntaxStack.TryPeek(out syntax) && syntax is IExpressionNode)
        {
            // Mathematical angle bracket
            model.DiagnosticBag.ReportTodoException(
                consumedOpenAngleBracketToken.TextSpan,
                $"Implement mathematical angle bracket");
        }
        else
        {
            // Generic Arguments
            ParseTypes.HandleGenericArguments(consumedOpenAngleBracketToken, model);

            if (model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
            {
                var typeDefinitionNode = (TypeDefinitionNode)model.SyntaxStack.Pop();

                // TODO: Fix boundClassDefinitionNode, it broke on (2023-07-26)
                //
                // _cSharpParser._nodeRecent = boundClassDefinitionNode with
                // {
                //     BoundGenericArgumentsNode = boundGenericArguments
                // };
            }
        }
    }

    public static void ParseCloseAngleBracketToken(
        CloseAngleBracketToken consumedCloseAngleBracketToken,
        CSharpParserModel model)
    {
        model.DiagnosticBag.ReportTodoException(
            consumedCloseAngleBracketToken.TextSpan,
            $"Implement {nameof(ParseCloseAngleBracketToken)}");
    }

    public static void ParseOpenSquareBracketToken(
        OpenSquareBracketToken consumedOpenSquareBracketToken,
        CSharpParserModel model)
    {
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
            model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
            model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.BinaryExpressionNode ||
            /* Prefer the enum comparison. Will short circuit. This "is" cast is for fallback in case someone in the future adds for expression syntax kinds but does not update this if statement TODO: Check if node ends with "ExpressionNode"? */
            model.SyntaxStack.TryPeek(out syntax) && syntax is IExpressionNode)
        {
            // Mathematical square bracket
            model.DiagnosticBag.ReportTodoException(
                consumedOpenSquareBracketToken.TextSpan,
                $"Implement mathematical square bracket");
        }
        else
        {
            // Attribute
            model.SyntaxStack.Push(consumedOpenSquareBracketToken);
            ParseTypes.HandleAttribute(consumedOpenSquareBracketToken, model);
        }
    }

    public static void ParseCloseSquareBracketToken(
        CloseSquareBracketToken consumedCloseSquareBracketToken,
        CSharpParserModel model)
    {
    }

    public static void ParseEqualsToken(
        EqualsToken consumedEqualsToken,
        CSharpParserModel model)
    {
        if (model.SyntaxStack.TryPeek(out var syntax) &&
            syntax is FunctionDefinitionNode functionDefinitionNode)
        {
            if (functionDefinitionNode.CodeBlockNode is null &&
                model.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
            {
                var closeAngleBracketToken = model.TokenWalker.Consume();

				var expression = ParseOthers.ParseExpression(model);
                var codeBlockNode = new CodeBlockNode(new ISyntax[] 
                {
                    expression
                }.ToImmutableArray());

                functionDefinitionNode = (FunctionDefinitionNode)model.SyntaxStack.Pop();
                functionDefinitionNode.SetExpressionBody(codeBlockNode);

                model.CurrentCodeBlockBuilder.ChildList.Add(functionDefinitionNode);
                model.CurrentCodeBlockBuilder.PendingChild = null;
            }
        }
    }

    public static void ParseMemberAccessToken(
        MemberAccessToken consumedMemberAccessToken,
        CSharpParserModel model)
    {
    	// Backtrack so that model.TokenWalker.Current is MemberAccessToken
    	model.TokenWalker.Backtrack();
    	
    	/*if (model.TokenWalker.Previous.SyntaxKind == SyntaxKind.IdentifierToken)
    	{
    		// Backtrack so that model.TokenWalker.Current is the IdentifierToken
    		// This is presumed to be the full expression, albeit quite naive.
    		//
    		// Ex: 'myVariable.SomeProperty'
    		//
    		// But, this might not work with method invocations
    		//
    		// Ex: 'myMethod().SomeProperty'
    		model.TokenWalker.Backtrack();
    	}*/
    	
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParseStatementDelimiterToken(
        StatementDelimiterToken consumedStatementDelimiterToken,
        CSharpParserModel model)
    {
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.NamespaceStatementNode)
        {
            var closureCurrentCompilationUnitBuilder = model.CurrentCodeBlockBuilder;
            ICodeBlockOwner? nextCodeBlockOwner = null;
            TypeClauseNode? scopeReturnTypeClauseNode = null;

            var namespaceStatementNode = (NamespaceStatementNode)model.SyntaxStack.Pop();
            nextCodeBlockOwner = namespaceStatementNode;

            model.FinalizeNamespaceFileScopeCodeBlockNodeAction = codeBlockNode =>
            {
                namespaceStatementNode.SetFileScoped(codeBlockNode);

				model.CurrentCodeBlockBuilder.PendingChild = null;
                closureCurrentCompilationUnitBuilder.ChildList.Add(namespaceStatementNode);
                model.Binder.BindNamespaceStatementNode(namespaceStatementNode, model);
            };

            model.Binder.RegisterScope(
                scopeReturnTypeClauseNode,
                consumedStatementDelimiterToken.TextSpan,
                model);

            model.Binder.AddNamespaceToCurrentScope(
                namespaceStatementNode.IdentifierToken.TextSpan.GetText(),
                model);

            model.CurrentCodeBlockBuilder = new(model.CurrentCodeBlockBuilder, nextCodeBlockOwner);
        }
        else if (model.CurrentCodeBlockBuilder.PendingChild is not null)
        {
        	var pendingChild = model.CurrentCodeBlockBuilder.PendingChild;
        
        	model.Binder.RegisterScope(CSharpFacts.Types.Void.ToTypeClause(), consumedStatementDelimiterToken.TextSpan, model);
			model.CurrentCodeBlockBuilder = new(model.CurrentCodeBlockBuilder, pendingChild);
			pendingChild.OnBoundScopeCreatedAndSetAsCurrent(model);
			
	        model.Binder.DisposeScope(consumedStatementDelimiterToken.TextSpan, model);
	
	        if (model.CurrentCodeBlockBuilder.Parent is not null)
	            model.CurrentCodeBlockBuilder = model.CurrentCodeBlockBuilder.Parent;
	            
	        model.CurrentCodeBlockBuilder.PendingChild = null;
        }
    }

    public static void ParseKeywordToken(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // 'return', 'if', 'get', etc...
        switch (consumedKeywordToken.SyntaxKind)
        {
            case SyntaxKind.AsTokenKeyword:
                ParseDefaultKeywords.HandleAsTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.BaseTokenKeyword:
                ParseDefaultKeywords.HandleBaseTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.BoolTokenKeyword:
                ParseDefaultKeywords.HandleBoolTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.BreakTokenKeyword:
                ParseDefaultKeywords.HandleBreakTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ByteTokenKeyword:
                ParseDefaultKeywords.HandleByteTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.CaseTokenKeyword:
                ParseDefaultKeywords.HandleCaseTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.CatchTokenKeyword:
                ParseDefaultKeywords.HandleCatchTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.CharTokenKeyword:
                ParseDefaultKeywords.HandleCharTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.CheckedTokenKeyword:
                ParseDefaultKeywords.HandleCheckedTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ConstTokenKeyword:
                ParseDefaultKeywords.HandleConstTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ContinueTokenKeyword:
                ParseDefaultKeywords.HandleContinueTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.DecimalTokenKeyword:
                ParseDefaultKeywords.HandleDecimalTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.DefaultTokenKeyword:
                ParseDefaultKeywords.HandleDefaultTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.DelegateTokenKeyword:
                ParseDefaultKeywords.HandleDelegateTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.DoTokenKeyword:
                ParseDefaultKeywords.HandleDoTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.DoubleTokenKeyword:
                ParseDefaultKeywords.HandleDoubleTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ElseTokenKeyword:
                ParseDefaultKeywords.HandleElseTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.EnumTokenKeyword:
                ParseDefaultKeywords.HandleEnumTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.EventTokenKeyword:
                ParseDefaultKeywords.HandleEventTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ExplicitTokenKeyword:
                ParseDefaultKeywords.HandleExplicitTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ExternTokenKeyword:
                ParseDefaultKeywords.HandleExternTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.FalseTokenKeyword:
                ParseDefaultKeywords.HandleFalseTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.FinallyTokenKeyword:
                ParseDefaultKeywords.HandleFinallyTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.FixedTokenKeyword:
                ParseDefaultKeywords.HandleFixedTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.FloatTokenKeyword:
                ParseDefaultKeywords.HandleFloatTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ForTokenKeyword:
                ParseDefaultKeywords.HandleForTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ForeachTokenKeyword:
                ParseDefaultKeywords.HandleForeachTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.GotoTokenKeyword:
                ParseDefaultKeywords.HandleGotoTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ImplicitTokenKeyword:
                ParseDefaultKeywords.HandleImplicitTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.InTokenKeyword:
                ParseDefaultKeywords.HandleInTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.IntTokenKeyword:
                ParseDefaultKeywords.HandleIntTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.IsTokenKeyword:
                ParseDefaultKeywords.HandleIsTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.LockTokenKeyword:
                ParseDefaultKeywords.HandleLockTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.LongTokenKeyword:
                ParseDefaultKeywords.HandleLongTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.NullTokenKeyword:
                ParseDefaultKeywords.HandleNullTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ObjectTokenKeyword:
                ParseDefaultKeywords.HandleObjectTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.OperatorTokenKeyword:
                ParseDefaultKeywords.HandleOperatorTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.OutTokenKeyword:
                ParseDefaultKeywords.HandleOutTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ParamsTokenKeyword:
                ParseDefaultKeywords.HandleParamsTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ProtectedTokenKeyword:
                ParseDefaultKeywords.HandleProtectedTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ReadonlyTokenKeyword:
                ParseDefaultKeywords.HandleReadonlyTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.RefTokenKeyword:
                ParseDefaultKeywords.HandleRefTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.SbyteTokenKeyword:
                ParseDefaultKeywords.HandleSbyteTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ShortTokenKeyword:
                ParseDefaultKeywords.HandleShortTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.SizeofTokenKeyword:
                ParseDefaultKeywords.HandleSizeofTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.StackallocTokenKeyword:
                ParseDefaultKeywords.HandleStackallocTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.StringTokenKeyword:
                ParseDefaultKeywords.HandleStringTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.StructTokenKeyword:
                ParseDefaultKeywords.HandleStructTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.SwitchTokenKeyword:
                ParseDefaultKeywords.HandleSwitchTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ThisTokenKeyword:
                ParseDefaultKeywords.HandleThisTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ThrowTokenKeyword:
                ParseDefaultKeywords.HandleThrowTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.TrueTokenKeyword:
                ParseDefaultKeywords.HandleTrueTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.TryTokenKeyword:
                ParseDefaultKeywords.HandleTryTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.TypeofTokenKeyword:
                ParseDefaultKeywords.HandleTypeofTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.UintTokenKeyword:
                ParseDefaultKeywords.HandleUintTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.UlongTokenKeyword:
                ParseDefaultKeywords.HandleUlongTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.UncheckedTokenKeyword:
                ParseDefaultKeywords.HandleUncheckedTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.UnsafeTokenKeyword:
                ParseDefaultKeywords.HandleUnsafeTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.UshortTokenKeyword:
                ParseDefaultKeywords.HandleUshortTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.VoidTokenKeyword:
                ParseDefaultKeywords.HandleVoidTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.VolatileTokenKeyword:
                ParseDefaultKeywords.HandleVolatileTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.WhileTokenKeyword:
                ParseDefaultKeywords.HandleWhileTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.UnrecognizedTokenKeyword:
                ParseDefaultKeywords.HandleUnrecognizedTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ReturnTokenKeyword:
                ParseDefaultKeywords.HandleReturnTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.NamespaceTokenKeyword:
                ParseDefaultKeywords.HandleNamespaceTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.ClassTokenKeyword:
                ParseDefaultKeywords.HandleClassTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.InterfaceTokenKeyword:
                ParseDefaultKeywords.HandleInterfaceTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.UsingTokenKeyword:
                ParseDefaultKeywords.HandleUsingTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.PublicTokenKeyword:
                ParseDefaultKeywords.HandlePublicTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.InternalTokenKeyword:
                ParseDefaultKeywords.HandleInternalTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.PrivateTokenKeyword:
                ParseDefaultKeywords.HandlePrivateTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.StaticTokenKeyword:
                ParseDefaultKeywords.HandleStaticTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.OverrideTokenKeyword:
                ParseDefaultKeywords.HandleOverrideTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.VirtualTokenKeyword:
                ParseDefaultKeywords.HandleVirtualTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.AbstractTokenKeyword:
                ParseDefaultKeywords.HandleAbstractTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.SealedTokenKeyword:
                ParseDefaultKeywords.HandleSealedTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.IfTokenKeyword:
                ParseDefaultKeywords.HandleIfTokenKeyword(consumedKeywordToken, model);
                break;
            case SyntaxKind.NewTokenKeyword:
                ParseDefaultKeywords.HandleNewTokenKeyword(consumedKeywordToken, model);
                break;
            default:
                ParseDefaultKeywords.HandleDefault(consumedKeywordToken, model);
                break;
        }
    }

    public static void ParseKeywordContextualToken(
        KeywordContextualToken consumedKeywordContextualToken,
        CSharpParserModel model)
    {
        switch (consumedKeywordContextualToken.SyntaxKind)
        {
            case SyntaxKind.VarTokenContextualKeyword:
                ParseContextualKeywords.HandleVarTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.PartialTokenContextualKeyword:
                ParseContextualKeywords.HandlePartialTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.AddTokenContextualKeyword:
                ParseContextualKeywords.HandleAddTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.AndTokenContextualKeyword:
                ParseContextualKeywords.HandleAndTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.AliasTokenContextualKeyword:
                ParseContextualKeywords.HandleAliasTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.AscendingTokenContextualKeyword:
                ParseContextualKeywords.HandleAscendingTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.ArgsTokenContextualKeyword:
                ParseContextualKeywords.HandleArgsTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.AsyncTokenContextualKeyword:
                ParseContextualKeywords.HandleAsyncTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.AwaitTokenContextualKeyword:
                ParseContextualKeywords.HandleAwaitTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.ByTokenContextualKeyword:
                ParseContextualKeywords.HandleByTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.DescendingTokenContextualKeyword:
                ParseContextualKeywords.HandleDescendingTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.DynamicTokenContextualKeyword:
                ParseContextualKeywords.HandleDynamicTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.EqualsTokenContextualKeyword:
                ParseContextualKeywords.HandleEqualsTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.FileTokenContextualKeyword:
                ParseContextualKeywords.HandleFileTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.FromTokenContextualKeyword:
                ParseContextualKeywords.HandleFromTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.GetTokenContextualKeyword:
                ParseContextualKeywords.HandleGetTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.GlobalTokenContextualKeyword:
                ParseContextualKeywords.HandleGlobalTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.GroupTokenContextualKeyword:
                ParseContextualKeywords.HandleGroupTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.InitTokenContextualKeyword:
                ParseContextualKeywords.HandleInitTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.IntoTokenContextualKeyword:
                ParseContextualKeywords.HandleIntoTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.JoinTokenContextualKeyword:
                ParseContextualKeywords.HandleJoinTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.LetTokenContextualKeyword:
                ParseContextualKeywords.HandleLetTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.ManagedTokenContextualKeyword:
                ParseContextualKeywords.HandleManagedTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.NameofTokenContextualKeyword:
                ParseContextualKeywords.HandleNameofTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.NintTokenContextualKeyword:
                ParseContextualKeywords.HandleNintTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.NotTokenContextualKeyword:
                ParseContextualKeywords.HandleNotTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.NotnullTokenContextualKeyword:
                ParseContextualKeywords.HandleNotnullTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.NuintTokenContextualKeyword:
                ParseContextualKeywords.HandleNuintTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.OnTokenContextualKeyword:
                ParseContextualKeywords.HandleOnTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.OrTokenContextualKeyword:
                ParseContextualKeywords.HandleOrTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.OrderbyTokenContextualKeyword:
                ParseContextualKeywords.HandleOrderbyTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.RecordTokenContextualKeyword:
                ParseContextualKeywords.HandleRecordTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.RemoveTokenContextualKeyword:
                ParseContextualKeywords.HandleRemoveTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.RequiredTokenContextualKeyword:
                ParseContextualKeywords.HandleRequiredTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.ScopedTokenContextualKeyword:
                ParseContextualKeywords.HandleScopedTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.SelectTokenContextualKeyword:
                ParseContextualKeywords.HandleSelectTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.SetTokenContextualKeyword:
                ParseContextualKeywords.HandleSetTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.UnmanagedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnmanagedTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.ValueTokenContextualKeyword:
                ParseContextualKeywords.HandleValueTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.WhenTokenContextualKeyword:
                ParseContextualKeywords.HandleWhenTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.WhereTokenContextualKeyword:
                ParseContextualKeywords.HandleWhereTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.WithTokenContextualKeyword:
                ParseContextualKeywords.HandleWithTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.YieldTokenContextualKeyword:
                ParseContextualKeywords.HandleYieldTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            case SyntaxKind.UnrecognizedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnrecognizedTokenContextualKeyword(consumedKeywordContextualToken, model);
                break;
            default:
            	model.DiagnosticBag.ReportTodoException(consumedKeywordContextualToken.TextSpan, $"Implement the {consumedKeywordContextualToken.SyntaxKind.ToString()} contextual keyword.");
            	break;
        }
    }
}