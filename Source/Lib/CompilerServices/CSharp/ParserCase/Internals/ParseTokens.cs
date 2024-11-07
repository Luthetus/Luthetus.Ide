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
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

	public static void ParseCharLiteralToken(
        CharLiteralToken consumedCharLiteralToken,
        CSharpParserModel model)
    {
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParseStringLiteralToken(
        StringLiteralToken consumedStringLiteralToken,
        CSharpParserModel model)
    {
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

    public static void ParseIdentifierToken(CSharpParserModel model)
    {
    	var identifierToken = (IdentifierToken)model.TokenWalker.Consume();
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
        	Console.WriteLine("TryParseTypedIdentifier.is TypeClauseNode typeClauseNode");
        
            // The variable 'genericArgumentsListingNode' is here for
            // when the syntax is determined to be a function definition.
            // In this case, the typeClauseNode would be the function's return type.
            // Yet, there may still be generic arguments to the function.
            var genericArgumentsListingNode = (GenericArgumentsListingNode?)null;

            if (TryParseGenericArguments(model) &&
                model.SyntaxStack.Peek().SyntaxKind == SyntaxKind.GenericArgumentsListingNode)
            {
            	Console.WriteLine("TryParseTypedIdentifier.TryParseGenericArguments.Success");
                genericArgumentsListingNode = (GenericArgumentsListingNode)model.SyntaxStack.Pop();
            }

            if (TryParseFunctionDefinition(
                consumedIdentifierToken,
                typeClauseNode,
                genericArgumentsListingNode,
                model))
            {
            	Console.WriteLine("TryParseFunctionDefinition.ReturnTrue");
                return true;
            }

            if (TryParseVariableDeclaration(
                typeClauseNode,
                consumedIdentifierToken,
                model))
            {
            	Console.WriteLine("TryParseVariableDeclaration.ReturnTrue");
                return true;
            }
        }
        
        Console.WriteLine("TryParseTypedIdentifier.ReturnFalse");
        return false;
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
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParseStarToken(
        StarToken consumedStarToken,
        CSharpParserModel model)
    {
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }

    public static void ParseDollarSignToken(
        DollarSignToken consumedDollarSignToken,
        CSharpParserModel model)
    {
        var expression = ParseOthers.ParseExpression(model);
        model.CurrentCodeBlockBuilder.ChildList.Add(expression);
    }
    
    public static void ParseAtToken(
        AtToken consumedAtToken,
        CSharpParserModel model)
    {
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
        	if (model.CurrentCodeBlockBuilder.Parent is not null && // Not Global Scope
        		!model.CurrentCodeBlockBuilder.PendingChild.OpenBraceToken.ConstructorWasInvoked) // Not Already Deliminated By an OpenBrace.
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
    }

    public static void ParseKeywordToken(CSharpParserModel model)
    {
        // 'return', 'if', 'get', etc...
        switch (model.TokenWalker.Current.SyntaxKind)
        {
            case SyntaxKind.AsTokenKeyword:
                ParseDefaultKeywords.HandleAsTokenKeyword(model);
                break;
            case SyntaxKind.BaseTokenKeyword:
                ParseDefaultKeywords.HandleBaseTokenKeyword(model);
                break;
            case SyntaxKind.BoolTokenKeyword:
                ParseDefaultKeywords.HandleBoolTokenKeyword(model);
                break;
            case SyntaxKind.BreakTokenKeyword:
                ParseDefaultKeywords.HandleBreakTokenKeyword(model);
                break;
            case SyntaxKind.ByteTokenKeyword:
                ParseDefaultKeywords.HandleByteTokenKeyword(model);
                break;
            case SyntaxKind.CaseTokenKeyword:
                ParseDefaultKeywords.HandleCaseTokenKeyword(model);
                break;
            case SyntaxKind.CatchTokenKeyword:
                ParseDefaultKeywords.HandleCatchTokenKeyword(model);
                break;
            case SyntaxKind.CharTokenKeyword:
                ParseDefaultKeywords.HandleCharTokenKeyword(model);
                break;
            case SyntaxKind.CheckedTokenKeyword:
                ParseDefaultKeywords.HandleCheckedTokenKeyword(model);
                break;
            case SyntaxKind.ConstTokenKeyword:
                ParseDefaultKeywords.HandleConstTokenKeyword(model);
                break;
            case SyntaxKind.ContinueTokenKeyword:
                ParseDefaultKeywords.HandleContinueTokenKeyword(model);
                break;
            case SyntaxKind.DecimalTokenKeyword:
                ParseDefaultKeywords.HandleDecimalTokenKeyword(model);
                break;
            case SyntaxKind.DefaultTokenKeyword:
                ParseDefaultKeywords.HandleDefaultTokenKeyword(model);
                break;
            case SyntaxKind.DelegateTokenKeyword:
                ParseDefaultKeywords.HandleDelegateTokenKeyword(model);
                break;
            case SyntaxKind.DoTokenKeyword:
                ParseDefaultKeywords.HandleDoTokenKeyword(model);
                break;
            case SyntaxKind.DoubleTokenKeyword:
                ParseDefaultKeywords.HandleDoubleTokenKeyword(model);
                break;
            case SyntaxKind.ElseTokenKeyword:
                ParseDefaultKeywords.HandleElseTokenKeyword(model);
                break;
            case SyntaxKind.EnumTokenKeyword:
                ParseDefaultKeywords.HandleEnumTokenKeyword(model);
                break;
            case SyntaxKind.EventTokenKeyword:
                ParseDefaultKeywords.HandleEventTokenKeyword(model);
                break;
            case SyntaxKind.ExplicitTokenKeyword:
                ParseDefaultKeywords.HandleExplicitTokenKeyword(model);
                break;
            case SyntaxKind.ExternTokenKeyword:
                ParseDefaultKeywords.HandleExternTokenKeyword(model);
                break;
            case SyntaxKind.FalseTokenKeyword:
                ParseDefaultKeywords.HandleFalseTokenKeyword(model);
                break;
            case SyntaxKind.FinallyTokenKeyword:
                ParseDefaultKeywords.HandleFinallyTokenKeyword(model);
                break;
            case SyntaxKind.FixedTokenKeyword:
                ParseDefaultKeywords.HandleFixedTokenKeyword(model);
                break;
            case SyntaxKind.FloatTokenKeyword:
                ParseDefaultKeywords.HandleFloatTokenKeyword(model);
                break;
            case SyntaxKind.ForTokenKeyword:
                ParseDefaultKeywords.HandleForTokenKeyword(model);
                break;
            case SyntaxKind.ForeachTokenKeyword:
                ParseDefaultKeywords.HandleForeachTokenKeyword(model);
                break;
            case SyntaxKind.GotoTokenKeyword:
                ParseDefaultKeywords.HandleGotoTokenKeyword(model);
                break;
            case SyntaxKind.ImplicitTokenKeyword:
                ParseDefaultKeywords.HandleImplicitTokenKeyword(model);
                break;
            case SyntaxKind.InTokenKeyword:
                ParseDefaultKeywords.HandleInTokenKeyword(model);
                break;
            case SyntaxKind.IntTokenKeyword:
                ParseDefaultKeywords.HandleIntTokenKeyword(model);
                break;
            case SyntaxKind.IsTokenKeyword:
                ParseDefaultKeywords.HandleIsTokenKeyword(model);
                break;
            case SyntaxKind.LockTokenKeyword:
                ParseDefaultKeywords.HandleLockTokenKeyword(model);
                break;
            case SyntaxKind.LongTokenKeyword:
                ParseDefaultKeywords.HandleLongTokenKeyword(model);
                break;
            case SyntaxKind.NullTokenKeyword:
                ParseDefaultKeywords.HandleNullTokenKeyword(model);
                break;
            case SyntaxKind.ObjectTokenKeyword:
                ParseDefaultKeywords.HandleObjectTokenKeyword(model);
                break;
            case SyntaxKind.OperatorTokenKeyword:
                ParseDefaultKeywords.HandleOperatorTokenKeyword(model);
                break;
            case SyntaxKind.OutTokenKeyword:
                ParseDefaultKeywords.HandleOutTokenKeyword(model);
                break;
            case SyntaxKind.ParamsTokenKeyword:
                ParseDefaultKeywords.HandleParamsTokenKeyword(model);
                break;
            case SyntaxKind.ProtectedTokenKeyword:
                ParseDefaultKeywords.HandleProtectedTokenKeyword(model);
                break;
            case SyntaxKind.ReadonlyTokenKeyword:
                ParseDefaultKeywords.HandleReadonlyTokenKeyword(model);
                break;
            case SyntaxKind.RefTokenKeyword:
                ParseDefaultKeywords.HandleRefTokenKeyword(model);
                break;
            case SyntaxKind.SbyteTokenKeyword:
                ParseDefaultKeywords.HandleSbyteTokenKeyword(model);
                break;
            case SyntaxKind.ShortTokenKeyword:
                ParseDefaultKeywords.HandleShortTokenKeyword(model);
                break;
            case SyntaxKind.SizeofTokenKeyword:
                ParseDefaultKeywords.HandleSizeofTokenKeyword(model);
                break;
            case SyntaxKind.StackallocTokenKeyword:
                ParseDefaultKeywords.HandleStackallocTokenKeyword(model);
                break;
            case SyntaxKind.StringTokenKeyword:
                ParseDefaultKeywords.HandleStringTokenKeyword(model);
                break;
            case SyntaxKind.StructTokenKeyword:
                ParseDefaultKeywords.HandleStructTokenKeyword(model);
                break;
            case SyntaxKind.SwitchTokenKeyword:
                ParseDefaultKeywords.HandleSwitchTokenKeyword(model);
                break;
            case SyntaxKind.ThisTokenKeyword:
                ParseDefaultKeywords.HandleThisTokenKeyword(model);
                break;
            case SyntaxKind.ThrowTokenKeyword:
                ParseDefaultKeywords.HandleThrowTokenKeyword(model);
                break;
            case SyntaxKind.TrueTokenKeyword:
                ParseDefaultKeywords.HandleTrueTokenKeyword(model);
                break;
            case SyntaxKind.TryTokenKeyword:
                ParseDefaultKeywords.HandleTryTokenKeyword(model);
                break;
            case SyntaxKind.TypeofTokenKeyword:
                ParseDefaultKeywords.HandleTypeofTokenKeyword(model);
                break;
            case SyntaxKind.UintTokenKeyword:
                ParseDefaultKeywords.HandleUintTokenKeyword(model);
                break;
            case SyntaxKind.UlongTokenKeyword:
                ParseDefaultKeywords.HandleUlongTokenKeyword(model);
                break;
            case SyntaxKind.UncheckedTokenKeyword:
                ParseDefaultKeywords.HandleUncheckedTokenKeyword(model);
                break;
            case SyntaxKind.UnsafeTokenKeyword:
                ParseDefaultKeywords.HandleUnsafeTokenKeyword(model);
                break;
            case SyntaxKind.UshortTokenKeyword:
                ParseDefaultKeywords.HandleUshortTokenKeyword(model);
                break;
            case SyntaxKind.VoidTokenKeyword:
                ParseDefaultKeywords.HandleVoidTokenKeyword(model);
                break;
            case SyntaxKind.VolatileTokenKeyword:
                ParseDefaultKeywords.HandleVolatileTokenKeyword(model);
                break;
            case SyntaxKind.WhileTokenKeyword:
                ParseDefaultKeywords.HandleWhileTokenKeyword(model);
                break;
            case SyntaxKind.UnrecognizedTokenKeyword:
                ParseDefaultKeywords.HandleUnrecognizedTokenKeyword(model);
                break;
            case SyntaxKind.ReturnTokenKeyword:
                ParseDefaultKeywords.HandleReturnTokenKeyword(model);
                break;
            case SyntaxKind.NamespaceTokenKeyword:
                ParseDefaultKeywords.HandleNamespaceTokenKeyword(model);
                break;
            case SyntaxKind.ClassTokenKeyword:
                ParseDefaultKeywords.HandleClassTokenKeyword(model);
                break;
            case SyntaxKind.InterfaceTokenKeyword:
                ParseDefaultKeywords.HandleInterfaceTokenKeyword(model);
                break;
            case SyntaxKind.UsingTokenKeyword:
                ParseDefaultKeywords.HandleUsingTokenKeyword(model);
                break;
            case SyntaxKind.PublicTokenKeyword:
                ParseDefaultKeywords.HandlePublicTokenKeyword(model);
                break;
            case SyntaxKind.InternalTokenKeyword:
                ParseDefaultKeywords.HandleInternalTokenKeyword(model);
                break;
            case SyntaxKind.PrivateTokenKeyword:
                ParseDefaultKeywords.HandlePrivateTokenKeyword(model);
                break;
            case SyntaxKind.StaticTokenKeyword:
                ParseDefaultKeywords.HandleStaticTokenKeyword(model);
                break;
            case SyntaxKind.OverrideTokenKeyword:
                ParseDefaultKeywords.HandleOverrideTokenKeyword(model);
                break;
            case SyntaxKind.VirtualTokenKeyword:
                ParseDefaultKeywords.HandleVirtualTokenKeyword(model);
                break;
            case SyntaxKind.AbstractTokenKeyword:
                ParseDefaultKeywords.HandleAbstractTokenKeyword(model);
                break;
            case SyntaxKind.SealedTokenKeyword:
                ParseDefaultKeywords.HandleSealedTokenKeyword(model);
                break;
            case SyntaxKind.IfTokenKeyword:
                ParseDefaultKeywords.HandleIfTokenKeyword(model);
                break;
            case SyntaxKind.NewTokenKeyword:
                ParseDefaultKeywords.HandleNewTokenKeyword(model);
                break;
            default:
                ParseDefaultKeywords.HandleDefault(model);
                break;
        }
    }

    public static void ParseKeywordContextualToken(CSharpParserModel model)
    {
        switch (model.TokenWalker.Current.SyntaxKind)
        {
            case SyntaxKind.VarTokenContextualKeyword:
                ParseContextualKeywords.HandleVarTokenContextualKeyword(model);
                break;
            case SyntaxKind.PartialTokenContextualKeyword:
                ParseContextualKeywords.HandlePartialTokenContextualKeyword(model);
                break;
            case SyntaxKind.AddTokenContextualKeyword:
                ParseContextualKeywords.HandleAddTokenContextualKeyword(model);
                break;
            case SyntaxKind.AndTokenContextualKeyword:
                ParseContextualKeywords.HandleAndTokenContextualKeyword(model);
                break;
            case SyntaxKind.AliasTokenContextualKeyword:
                ParseContextualKeywords.HandleAliasTokenContextualKeyword(model);
                break;
            case SyntaxKind.AscendingTokenContextualKeyword:
                ParseContextualKeywords.HandleAscendingTokenContextualKeyword(model);
                break;
            case SyntaxKind.ArgsTokenContextualKeyword:
                ParseContextualKeywords.HandleArgsTokenContextualKeyword(model);
                break;
            case SyntaxKind.AsyncTokenContextualKeyword:
                ParseContextualKeywords.HandleAsyncTokenContextualKeyword(model);
                break;
            case SyntaxKind.AwaitTokenContextualKeyword:
                ParseContextualKeywords.HandleAwaitTokenContextualKeyword(model);
                break;
            case SyntaxKind.ByTokenContextualKeyword:
                ParseContextualKeywords.HandleByTokenContextualKeyword(model);
                break;
            case SyntaxKind.DescendingTokenContextualKeyword:
                ParseContextualKeywords.HandleDescendingTokenContextualKeyword(model);
                break;
            case SyntaxKind.DynamicTokenContextualKeyword:
                ParseContextualKeywords.HandleDynamicTokenContextualKeyword(model);
                break;
            case SyntaxKind.EqualsTokenContextualKeyword:
                ParseContextualKeywords.HandleEqualsTokenContextualKeyword(model);
                break;
            case SyntaxKind.FileTokenContextualKeyword:
                ParseContextualKeywords.HandleFileTokenContextualKeyword(model);
                break;
            case SyntaxKind.FromTokenContextualKeyword:
                ParseContextualKeywords.HandleFromTokenContextualKeyword(model);
                break;
            case SyntaxKind.GetTokenContextualKeyword:
                ParseContextualKeywords.HandleGetTokenContextualKeyword(model);
                break;
            case SyntaxKind.GlobalTokenContextualKeyword:
                ParseContextualKeywords.HandleGlobalTokenContextualKeyword(model);
                break;
            case SyntaxKind.GroupTokenContextualKeyword:
                ParseContextualKeywords.HandleGroupTokenContextualKeyword(model);
                break;
            case SyntaxKind.InitTokenContextualKeyword:
                ParseContextualKeywords.HandleInitTokenContextualKeyword(model);
                break;
            case SyntaxKind.IntoTokenContextualKeyword:
                ParseContextualKeywords.HandleIntoTokenContextualKeyword(model);
                break;
            case SyntaxKind.JoinTokenContextualKeyword:
                ParseContextualKeywords.HandleJoinTokenContextualKeyword(model);
                break;
            case SyntaxKind.LetTokenContextualKeyword:
                ParseContextualKeywords.HandleLetTokenContextualKeyword(model);
                break;
            case SyntaxKind.ManagedTokenContextualKeyword:
                ParseContextualKeywords.HandleManagedTokenContextualKeyword(model);
                break;
            case SyntaxKind.NameofTokenContextualKeyword:
                ParseContextualKeywords.HandleNameofTokenContextualKeyword(model);
                break;
            case SyntaxKind.NintTokenContextualKeyword:
                ParseContextualKeywords.HandleNintTokenContextualKeyword(model);
                break;
            case SyntaxKind.NotTokenContextualKeyword:
                ParseContextualKeywords.HandleNotTokenContextualKeyword(model);
                break;
            case SyntaxKind.NotnullTokenContextualKeyword:
                ParseContextualKeywords.HandleNotnullTokenContextualKeyword(model);
                break;
            case SyntaxKind.NuintTokenContextualKeyword:
                ParseContextualKeywords.HandleNuintTokenContextualKeyword(model);
                break;
            case SyntaxKind.OnTokenContextualKeyword:
                ParseContextualKeywords.HandleOnTokenContextualKeyword(model);
                break;
            case SyntaxKind.OrTokenContextualKeyword:
                ParseContextualKeywords.HandleOrTokenContextualKeyword(model);
                break;
            case SyntaxKind.OrderbyTokenContextualKeyword:
                ParseContextualKeywords.HandleOrderbyTokenContextualKeyword(model);
                break;
            case SyntaxKind.RecordTokenContextualKeyword:
                ParseContextualKeywords.HandleRecordTokenContextualKeyword(model);
                break;
            case SyntaxKind.RemoveTokenContextualKeyword:
                ParseContextualKeywords.HandleRemoveTokenContextualKeyword(model);
                break;
            case SyntaxKind.RequiredTokenContextualKeyword:
                ParseContextualKeywords.HandleRequiredTokenContextualKeyword(model);
                break;
            case SyntaxKind.ScopedTokenContextualKeyword:
                ParseContextualKeywords.HandleScopedTokenContextualKeyword(model);
                break;
            case SyntaxKind.SelectTokenContextualKeyword:
                ParseContextualKeywords.HandleSelectTokenContextualKeyword(model);
                break;
            case SyntaxKind.SetTokenContextualKeyword:
                ParseContextualKeywords.HandleSetTokenContextualKeyword(model);
                break;
            case SyntaxKind.UnmanagedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnmanagedTokenContextualKeyword(model);
                break;
            case SyntaxKind.ValueTokenContextualKeyword:
                ParseContextualKeywords.HandleValueTokenContextualKeyword(model);
                break;
            case SyntaxKind.WhenTokenContextualKeyword:
                ParseContextualKeywords.HandleWhenTokenContextualKeyword(model);
                break;
            case SyntaxKind.WhereTokenContextualKeyword:
                ParseContextualKeywords.HandleWhereTokenContextualKeyword(model);
                break;
            case SyntaxKind.WithTokenContextualKeyword:
                ParseContextualKeywords.HandleWithTokenContextualKeyword(model);
                break;
            case SyntaxKind.YieldTokenContextualKeyword:
                ParseContextualKeywords.HandleYieldTokenContextualKeyword(model);
                break;
            case SyntaxKind.UnrecognizedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnrecognizedTokenContextualKeyword(model);
                break;
            default:
            	model.DiagnosticBag.ReportTodoException(model.TokenWalker.Current.TextSpan, $"Implement the {model.TokenWalker.Current.SyntaxKind.ToString()} contextual keyword.");
            	break;
        }
    }
}