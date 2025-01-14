using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.Facts;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseTokens
{
    public static void ParsePreprocessorDirectiveToken(
        PreprocessorDirectiveToken consumedPreprocessorDirectiveToken,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
        var consumedToken = parserModel.TokenWalker.Consume();
    }

    public static void ParseIdentifierToken(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (parserModel.TokenWalker.Current.TextSpan.Length == 1 &&
    		parserModel.TokenWalker.Current.TextSpan.GetText() == "_")
    	{
    		if (!compilationUnit.Binder.TryGetVariableDeclarationHierarchically(
			    	compilationUnit,
			    	compilationUnit.BinderSession.ResourceUri,
			    	compilationUnit.BinderSession.CurrentScopeIndexKey,
			        parserModel.TokenWalker.Current.TextSpan.GetText(),
			        out _))
			{
				compilationUnit.Binder.BindDiscard((IdentifierToken)parserModel.TokenWalker.Current, compilationUnit);
	    		_ = parserModel.TokenWalker.Consume();
	    		return;
			}
    	}
    	
    	var originalTokenIndex = parserModel.TokenWalker.Index;
    	
    	parserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.TypeClauseNode);
    	parserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.VariableDeclarationNode);
    	parserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.VariableReferenceNode);
    	parserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.ConstructorInvocationExpressionNode);
    	
    	if ((parserModel.CurrentCodeBlockBuilder.CodeBlockOwner?.SyntaxKind ?? SyntaxKind.EmptyNode) !=
    			SyntaxKind.TypeDefinitionNode)
    	{
    		// There is a syntax conflict between a ConstructorDefinitionNode and a FunctionInvocationNode.
    		//
    		// Disambiguation is done based on the 'CodeBlockOwner' until a better solution is found.
    		//
    		// If the supposed "ConstructorDefinitionNode" does not have the same name as
    		// the CodeBlockOwner.
    		//
    		// Then, it perhaps should be treated as a function invocation (or function definition).
    		// The main case for this being someone typing out pseudo code within a CodeBlockOwner
    		// that is a TypeDefinitionNode.
    		parserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.FunctionInvocationNode);
    	}
    	
    	parserModel.ParserContextKind = CSharpParserContextKind.ForceStatementExpression;
    	
		var successParse = ParseOthers.TryParseExpression(null, compilationUnit, ref parserModel, out var expressionNode);
		
		if (!successParse)
		{
			expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
			parserModel.StatementBuilder.ChildList.Add(expressionNode);
	    	return;
		}
		
		switch (expressionNode.SyntaxKind)
		{
			case SyntaxKind.TypeClauseNode:
				MoveToHandleTypeClauseNode(originalTokenIndex, (TypeClauseNode)expressionNode, compilationUnit, ref parserModel);
				return;
			case SyntaxKind.VariableDeclarationNode:
				if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
    				parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
    			{
    				MoveToHandleFunctionDefinition((VariableDeclarationNode)expressionNode, compilationUnit, ref parserModel);
				    return;
    			}
    			
    			MoveToHandleVariableDeclarationNode((VariableDeclarationNode)expressionNode, compilationUnit, ref parserModel);
				return;
	        case SyntaxKind.VariableReferenceNode:
	        
	        	var isQuestionMarkMemberAccessToken = parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.QuestionMarkToken &&
	        		parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.MemberAccessToken;
	        
	        	if ((parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.MemberAccessToken || isQuestionMarkMemberAccessToken) &&
	        		originalTokenIndex == parserModel.TokenWalker.Index - 1)
				{
					_ = parserModel.TokenWalker.Backtrack();
					expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
					parserModel.StatementBuilder.ChildList.Add(expressionNode);
					return;
				}
				
				parserModel.StatementBuilder.ChildList.Add(expressionNode);
				return;
	        case SyntaxKind.FunctionInvocationNode:
			case SyntaxKind.ConstructorInvocationExpressionNode:
				parserModel.StatementBuilder.ChildList.Add(expressionNode);
				return;
			default:
				parserModel.DiagnosticBag.ReportTodoException(parserModel.TokenWalker.Current.TextSpan, $"nameof(ParseIdentifierToken) default case");
				return;
		}
    }
    
    public static void MoveToHandleFunctionDefinition(VariableDeclarationNode variableDeclarationNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	ParseFunctions.HandleFunctionDefinition(
			variableDeclarationNode.IdentifierToken,
	        variableDeclarationNode.TypeClauseNode,
	        consumedGenericArgumentsListingNode: null,
	        compilationUnit,
	        ref parserModel);
    }
    
    public static void MoveToHandleVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var variableKind = VariableKind.Local;
    			
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
			parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
		{
			variableKind = VariableKind.Property;
		}
		else if (parserModel.CurrentCodeBlockBuilder.CodeBlockOwner is not null &&
				 parserModel.CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind == SyntaxKind.TypeDefinitionNode)
		{
			variableKind = VariableKind.Field;
		}
		
		((VariableDeclarationNode)variableDeclarationNode).VariableKind = variableKind;
		
		compilationUnit.Binder.BindVariableDeclarationNode(variableDeclarationNode, compilationUnit);
        parserModel.CurrentCodeBlockBuilder.ChildList.Add(variableDeclarationNode);
		parserModel.StatementBuilder.ChildList.Add(variableDeclarationNode);
		
		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
		{
			ParsePropertyDefinition(compilationUnit, variableDeclarationNode, ref parserModel);
		}
		else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
		{
			ParsePropertyDefinition_ExpressionBound(compilationUnit, ref parserModel);
		}
    }
    
    public static void MoveToHandleTypeClauseNode(int originalTokenIndex, TypeClauseNode typeClauseNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
			parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EndOfFileToken ||
			parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
			parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
		{
			parserModel.StatementBuilder.ChildList.Add(typeClauseNode);
		}
		else if (parserModel.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
				 UtilityApi.IsConvertibleToIdentifierToken(typeClauseNode.TypeIdentifierToken.SyntaxKind) &&
				 parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken &&
			     typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText() == typeClauseNode.TypeIdentifierToken.TextSpan.GetText())
		{
			// ConstructorDefinitionNode
			
			var identifierToken = UtilityApi.ConvertToIdentifierToken(typeClauseNode.TypeIdentifierToken, compilationUnit, ref parserModel);
			
			ParseFunctions.HandleConstructorDefinition(
				typeDefinitionNode,
		        identifierToken,
		        compilationUnit,
		        ref parserModel);
		}
		else
		{
			parserModel.StatementBuilder.ChildList.Add(typeClauseNode);
		}
		
		return;
    }
    
    public static void ParsePropertyDefinition(CSharpCompilationUnit compilationUnit, IVariableDeclarationNode variableDeclarationNode, ref CSharpParserModel parserModel)
    {
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
			else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.GetTokenContextualKeyword)
			{
				variableDeclarationNode.HasGetter = true;
			}
			else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.SetTokenContextualKeyword)
			{
				variableDeclarationNode.HasSetter = true;
			}

			_ = parserModel.TokenWalker.Consume();
		}

		var closeTokenIndex = parserModel.TokenWalker.Index;
		var closeBraceToken = (CloseBraceToken)parserModel.TokenWalker.Match(SyntaxKind.CloseBraceToken);
		
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif
    }
    
    public static void ParsePropertyDefinition_ExpressionBound(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
		var equalsCloseAngleBracketToken = (EqualsCloseAngleBracketToken)parserModel.TokenWalker.Consume();
		
		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		var statementDelimiterToken = (StatementDelimiterToken)parserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
    }

    public static void ParseColonToken(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var colonToken = (ColonToken)parserModel.TokenWalker.Consume();
    
        if (parserModel.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
        {
            var typeDefinitionNode = (TypeDefinitionNode)parserModel.SyntaxStack.Pop();
            var inheritedTypeClauseNode = parserModel.TokenWalker.MatchTypeClauseNode(compilationUnit, ref parserModel);

            compilationUnit.Binder.BindTypeClauseNode(inheritedTypeClauseNode, compilationUnit);

			typeDefinitionNode.SetInheritedTypeClauseNode(inheritedTypeClauseNode);

            parserModel.SyntaxStack.Push(typeDefinitionNode);
            
            compilationUnit.Binder.NewScopeAndBuilderFromOwner(
	        	typeDefinitionNode,
		        typeDefinitionNode.GetReturnTypeClauseNode(),
		        parserModel.TokenWalker.Current.TextSpan,
		        compilationUnit,
		        ref parserModel);
            
            // (2025-01-13)
			// ========================================================
			// - 'SetActiveCodeBlockBuilder', 'SetActiveScope', and 'PermitInnerPendingCodeBlockOwnerToBeParsed'
			//   should all be handled by the same method.
        }
        else
        {
            parserModel.DiagnosticBag.ReportTodoException(colonToken.TextSpan, "Colon is in unexpected place.");
        }
    }

	/// <summary>
	/// OpenBraceToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseOpenBraceToken(OpenBraceToken openBraceToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	// (2025-01-13)
		// ========================================================
		// - 'SetActiveCodeBlockBuilder', 'SetActiveScope', 'PermitInnerPendingCodeBlockOwnerToBeParsed',
		//   and '' should all be handled by the same method.
		//
		// With the new changes, you will come into this method
		// with the CurrentCodeBlockBuilder and CurrentScopeIndexKey already set
		//
		// You need to consider coming into this method and transitioning into deferred parsing.
		//
		// For the case of 'deferred parsing':
		// - The CurrentCodeBlockBuilder and CurrentScopeIndexKey
		//   will need to be set to the parent.
		// - After setting those to the parent,
		//   you need to enqueue the "previous code block builder"
		//   into the "current code block builder's deferred parsing queue".
		// 
		
		/*
		    (2025-01-13)
		    ========================================================
		    
		    public void SomeMethod(SomeRecord recordInstance) =>
		    	recordInstance with
			    {
			    	SomeProperty = "cat",
			    };
			
			-------------------------------------------------------
			
			SomeRecord recordInstance = new();
			
			if (false)
				return recordInstance with
			    {
			    	SomeProperty = "cat",
			    };
			    
			-------------------------------------------------------
			
			You know because an 'if' statement does not have "secondary syntax".
			Either the OpenBraceToken immediately follows the if statement's
			predicate, or there isn't one at all as far as the if statement is concerned.
			
			As for a FunctionDefinitionNode, (or any "secondary syntax" having ICodeBlockOwner),
			they will disambiguate with the EqualsCloseAngleBracketToken ('=>').
		*/
    	
    	// (2025-01-13)
		// ========================================================
		// - It is vital that the global scope has 'true' for 'IsImplicitOpenCodeBlockTextSpan'.
		// 
    	if (parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan)
		{
			var arbitraryCodeBlockNode = new ArbitraryCodeBlockNode(parserModel.CurrentCodeBlockBuilder.CodeBlockOwner);
			parserModel.SyntaxStack.Push(arbitraryCodeBlockNode);
			
			compilationUnit.Binder.NewScopeAndBuilderFromOwner(
		    	arbitraryCodeBlockNode,
		        arbitraryCodeBlockNode.GetReturnTypeClauseNode(),
		        openBraceToken.TextSpan,
		        compilationUnit,
		        ref parserModel);
		}
		
		parserModel.CurrentCodeBlockBuilder.CodeBlockOwner.SetOpenBraceToken(openBraceToken, parserModel.DiagnosticBag, parserModel.TokenWalker);

		// '?? ScopeDirectionKind.Both':
		// - global scope has a null parent.
		// - global scope itself has a null 'ICodeBlockOwner'
		var parentScopeDirection = parserModel.CurrentCodeBlockBuilder.Parent?.CodeBlockOwner?.ScopeDirectionKind ?? ScopeDirectionKind.Both;
		
		if (parentScopeDirection == ScopeDirectionKind.Both)
		{
			if (!parserModel.CurrentCodeBlockBuilder.PermitCodeBlockParsing)
			{
				parserModel.TokenWalker.DeferParsingOfChildScope(openBraceToken, compilationUnit, ref parserModel);
				return;
			}

			parserModel.CurrentCodeBlockBuilder.PermitCodeBlockParsing = false;
		}
    }

	/// <summary>
	/// CloseBraceToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseCloseBraceToken(CloseBraceToken closeBraceToken, int closeBraceTokenIndex, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (parserModel.CurrentCodeBlockBuilder.ParseChildScopeQueue.TryDequeue(out var deferredChildScope))
		{
			deferredChildScope.PrepareMainParserLoop(closeBraceTokenIndex, compilationUnit, ref parserModel);
			return;
		}

		if (parserModel.CurrentCodeBlockBuilder.CodeBlockOwner is not null)
			parserModel.CurrentCodeBlockBuilder.CodeBlockOwner.SetCloseBraceToken(closeBraceToken, parserModel.DiagnosticBag, parserModel.TokenWalker);
		
        compilationUnit.Binder.CloseScope(closeBraceToken.TextSpan, compilationUnit, ref parserModel);
    }

    public static void ParseOpenParenthesisToken(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (parserModel.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
    	{
    		var typeDefinitionNode = (TypeDefinitionNode)parserModel.SyntaxStack.Pop();
    		var functionArgumentsListingNode = ParseFunctions.HandleFunctionArguments(compilationUnit, ref parserModel);
    		typeDefinitionNode.SetPrimaryConstructorFunctionArgumentsListingNode(functionArgumentsListingNode);
    		return;
    	}
    	
    	var originalTokenIndex = parserModel.TokenWalker.Index;
    	
    	parserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.VariableDeclarationNode);
    	parserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.TypeClauseNode);
    	parserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.AmbiguousParenthesizedExpressionNode);
    	
    	parserModel.ParserContextKind = CSharpParserContextKind.ForceStatementExpression;
    	
		var successParse = ParseOthers.TryParseExpression(null, compilationUnit, ref parserModel, out var expressionNode);
		
		if (!successParse)
		{
			expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
			parserModel.StatementBuilder.ChildList.Add(expressionNode);
	    	return;
		}
		
		if (expressionNode.SyntaxKind == SyntaxKind.VariableDeclarationNode)
		{
			if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
				parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
			{
				MoveToHandleFunctionDefinition((VariableDeclarationNode)expressionNode, compilationUnit, ref parserModel);
			    return;
			}
			
			MoveToHandleVariableDeclarationNode((VariableDeclarationNode)expressionNode, compilationUnit, ref parserModel);
		}
		
		//// I am catching the next two but not doing anything with them
		//// only so that the TryParseExpression won't return early due to those being the
		//// SyntaxKind(s) that will appear during the process of parsing the VariableDeclarationNode
		//// given that the TypeClauseNode is a tuple.
		//
		// else if (expressionNode.SyntaxKind == SyntaxKind.TypeClauseNode)
		// else if (expressionNode.SyntaxKind == SyntaxKind.AmbiguousParenthesizedExpressionNode)
    }

    public static void ParseCloseParenthesisToken(
        CloseParenthesisToken consumedCloseParenthesisToken,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    	var closesParenthesisToken = (CloseParenthesisToken)parserModel.TokenWalker.Consume();
    }

    public static void ParseOpenAngleBracketToken(
        OpenAngleBracketToken consumedOpenAngleBracketToken,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    }

    public static void ParseCloseAngleBracketToken(
        CloseAngleBracketToken consumedCloseAngleBracketToken,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    }

    public static void ParseOpenSquareBracketToken(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var openSquareBracketToken = (OpenSquareBracketToken)parserModel.TokenWalker.Consume();
    
    	if (parserModel.StatementBuilder.ChildList.Count != 0)
    	{
    		parserModel.DiagnosticBag.ReportTodoException(
	    		openSquareBracketToken.TextSpan,
	    		$"Unexpected '{nameof(OpenSquareBracketToken)}'");
	    	return;
	    }
    	var openSquareBracketCounter = 1;
		var corruptState = false;
		
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		while (!parserModel.TokenWalker.IsEof)
		{
			if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenSquareBracketToken)
			{
				++openSquareBracketCounter;
			}
			else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
			{
				if (--openSquareBracketCounter <= 0)
					break;
			}
			else if (!corruptState)
			{
				var tokenIndexOriginal = parserModel.TokenWalker.Index;
				
				parserModel.ExpressionList.Add((SyntaxKind.CloseSquareBracketToken, null));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, null));
				var expression = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
				
				if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
					_ = parserModel.TokenWalker.Consume();
					
				if (tokenIndexOriginal < parserModel.TokenWalker.Index)
					continue; // Already consumed so avoid the one at the end of the while loop
			}

			_ = parserModel.TokenWalker.Consume();
		}

		var closeTokenIndex = parserModel.TokenWalker.Index;
		var closeSquareBracketToken = (CloseSquareBracketToken)parserModel.TokenWalker.Match(SyntaxKind.CloseSquareBracketToken);
		
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif
    }

    public static void ParseCloseSquareBracketToken(
        CloseSquareBracketToken consumedCloseSquareBracketToken,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    }

    public static void ParseEqualsToken(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (parserModel.StatementBuilder.ChildList.Count == 0)
    	{
    		ParseOthers.StartStatement_Expression(compilationUnit, ref parserModel);
    		return;
    	}
		
		if (parserModel.StatementBuilder.TryPeek(out var syntax) &&
			syntax.SyntaxKind == SyntaxKind.VariableDeclarationNode)
		{
			var variableDeclarationNode = (VariableDeclarationNode)syntax;
			
			parserModel.TokenWalker.Backtrack();
			var expression = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
			
			if (expression.SyntaxKind != SyntaxKind.VariableAssignmentExpressionNode)
			{
				// TODO: Report a diagnostic
				return;
			}
			
			parserModel.StatementBuilder.ChildList.Add(expression);
		}
	}

    public static void ParseMemberAccessToken(
        MemberAccessToken consumedMemberAccessToken,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    }

	/// <summary>
	/// StatementDelimiterToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseStatementDelimiterToken(StatementDelimiterToken statementDelimiterToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	// (2025-01-13)
		// ========================================================
		// - 'SetActiveCodeBlockBuilder', 'SetActiveScope', and 'PermitInnerPendingCodeBlockOwnerToBeParsed'
		//   should all be handled by the same method.
    
    	if (parserModel.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.NamespaceStatementNode)
        {
        	var namespaceStatementNode = (NamespaceStatementNode)parserModel.SyntaxStack.Pop();
        	
            ICodeBlockOwner? nextCodeBlockOwner = namespaceStatementNode;
            TypeClauseNode? scopeReturnTypeClauseNode = null;
            
            namespaceStatementNode.SetStatementDelimiterToken(statementDelimiterToken, parserModel.DiagnosticBag, parserModel.TokenWalker);

            compilationUnit.Binder.NewScopeAndBuilderFromOwner(
            	nextCodeBlockOwner,
                scopeReturnTypeClauseNode,
                statementDelimiterToken.TextSpan,
                compilationUnit);

            compilationUnit.Binder.AddNamespaceToCurrentScope(
                namespaceStatementNode.IdentifierToken.TextSpan.GetText(),
                compilationUnit);
        }
        else if (parserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner is not null &&
        		 !parserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner.OpenBraceToken.ConstructorWasInvoked)
        {
        	var pendingChild = parserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner;
        
        	compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        		pendingChild,
        		CSharpFacts.Types.Void.ToTypeClause(),
        		statementDelimiterToken.TextSpan,
        		compilationUnit);
        		
        	// (2025-01-13)
			// ========================================================
			// - You wouldn't invoke 'NewScopeAndBuilderFromOwner(...)' here,
			//   since it should have already been invoked with the new changes.

			pendingChild.SetStatementDelimiterToken(statementDelimiterToken, parserModel.DiagnosticBag, parserModel.TokenWalker);
			compilationUnit.Binder.OnBoundScopeCreatedAndSetAsCurrent(pendingChild, compilationUnit, ref parserModel);
			
	        compilationUnit.Binder.CloseScope(statementDelimiterToken.TextSpan, compilationUnit, ref parserModel);
        }
    }

    public static void ParseKeywordToken(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        // 'return', 'if', 'get', etc...
        switch (parserModel.TokenWalker.Current.SyntaxKind)
        {
            case SyntaxKind.AsTokenKeyword:
                ParseDefaultKeywords.HandleAsTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.BaseTokenKeyword:
                ParseDefaultKeywords.HandleBaseTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.BoolTokenKeyword:
                ParseDefaultKeywords.HandleBoolTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.BreakTokenKeyword:
                ParseDefaultKeywords.HandleBreakTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ByteTokenKeyword:
                ParseDefaultKeywords.HandleByteTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.CaseTokenKeyword:
                ParseDefaultKeywords.HandleCaseTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.CatchTokenKeyword:
                ParseDefaultKeywords.HandleCatchTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.CharTokenKeyword:
                ParseDefaultKeywords.HandleCharTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.CheckedTokenKeyword:
                ParseDefaultKeywords.HandleCheckedTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ConstTokenKeyword:
                ParseDefaultKeywords.HandleConstTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ContinueTokenKeyword:
                ParseDefaultKeywords.HandleContinueTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.DecimalTokenKeyword:
                ParseDefaultKeywords.HandleDecimalTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.DefaultTokenKeyword:
                ParseDefaultKeywords.HandleDefaultTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.DelegateTokenKeyword:
                ParseDefaultKeywords.HandleDelegateTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.DoTokenKeyword:
                ParseDefaultKeywords.HandleDoTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.DoubleTokenKeyword:
                ParseDefaultKeywords.HandleDoubleTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ElseTokenKeyword:
                ParseDefaultKeywords.HandleElseTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.EnumTokenKeyword:
                ParseDefaultKeywords.HandleEnumTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.EventTokenKeyword:
                ParseDefaultKeywords.HandleEventTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ExplicitTokenKeyword:
                ParseDefaultKeywords.HandleExplicitTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ExternTokenKeyword:
                ParseDefaultKeywords.HandleExternTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.FalseTokenKeyword:
                ParseDefaultKeywords.HandleFalseTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.FinallyTokenKeyword:
                ParseDefaultKeywords.HandleFinallyTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.FixedTokenKeyword:
                ParseDefaultKeywords.HandleFixedTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.FloatTokenKeyword:
                ParseDefaultKeywords.HandleFloatTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ForTokenKeyword:
                ParseDefaultKeywords.HandleForTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ForeachTokenKeyword:
                ParseDefaultKeywords.HandleForeachTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.GotoTokenKeyword:
                ParseDefaultKeywords.HandleGotoTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ImplicitTokenKeyword:
                ParseDefaultKeywords.HandleImplicitTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.InTokenKeyword:
                ParseDefaultKeywords.HandleInTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.IntTokenKeyword:
                ParseDefaultKeywords.HandleIntTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.IsTokenKeyword:
                ParseDefaultKeywords.HandleIsTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.LockTokenKeyword:
                ParseDefaultKeywords.HandleLockTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.LongTokenKeyword:
                ParseDefaultKeywords.HandleLongTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.NullTokenKeyword:
                ParseDefaultKeywords.HandleNullTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ObjectTokenKeyword:
                ParseDefaultKeywords.HandleObjectTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.OperatorTokenKeyword:
                ParseDefaultKeywords.HandleOperatorTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.OutTokenKeyword:
                ParseDefaultKeywords.HandleOutTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ParamsTokenKeyword:
                ParseDefaultKeywords.HandleParamsTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ProtectedTokenKeyword:
                ParseDefaultKeywords.HandleProtectedTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ReadonlyTokenKeyword:
                ParseDefaultKeywords.HandleReadonlyTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.RefTokenKeyword:
                ParseDefaultKeywords.HandleRefTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.SbyteTokenKeyword:
                ParseDefaultKeywords.HandleSbyteTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ShortTokenKeyword:
                ParseDefaultKeywords.HandleShortTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.SizeofTokenKeyword:
                ParseDefaultKeywords.HandleSizeofTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.StackallocTokenKeyword:
                ParseDefaultKeywords.HandleStackallocTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.StringTokenKeyword:
                ParseDefaultKeywords.HandleStringTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.StructTokenKeyword:
                ParseDefaultKeywords.HandleStructTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.SwitchTokenKeyword:
                ParseDefaultKeywords.HandleSwitchTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ThisTokenKeyword:
                ParseDefaultKeywords.HandleThisTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ThrowTokenKeyword:
                ParseDefaultKeywords.HandleThrowTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.TrueTokenKeyword:
                ParseDefaultKeywords.HandleTrueTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.TryTokenKeyword:
                ParseDefaultKeywords.HandleTryTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.TypeofTokenKeyword:
                ParseDefaultKeywords.HandleTypeofTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.UintTokenKeyword:
                ParseDefaultKeywords.HandleUintTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.UlongTokenKeyword:
                ParseDefaultKeywords.HandleUlongTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.UncheckedTokenKeyword:
                ParseDefaultKeywords.HandleUncheckedTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.UnsafeTokenKeyword:
                ParseDefaultKeywords.HandleUnsafeTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.UshortTokenKeyword:
                ParseDefaultKeywords.HandleUshortTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.VoidTokenKeyword:
                ParseDefaultKeywords.HandleVoidTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.VolatileTokenKeyword:
                ParseDefaultKeywords.HandleVolatileTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.WhileTokenKeyword:
                ParseDefaultKeywords.HandleWhileTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.UnrecognizedTokenKeyword:
                ParseDefaultKeywords.HandleUnrecognizedTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ReturnTokenKeyword:
                ParseDefaultKeywords.HandleReturnTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.NamespaceTokenKeyword:
                ParseDefaultKeywords.HandleNamespaceTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ClassTokenKeyword:
                ParseDefaultKeywords.HandleClassTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.InterfaceTokenKeyword:
                ParseDefaultKeywords.HandleInterfaceTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.UsingTokenKeyword:
                ParseDefaultKeywords.HandleUsingTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.PublicTokenKeyword:
                ParseDefaultKeywords.HandlePublicTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.InternalTokenKeyword:
                ParseDefaultKeywords.HandleInternalTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.PrivateTokenKeyword:
                ParseDefaultKeywords.HandlePrivateTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.StaticTokenKeyword:
                ParseDefaultKeywords.HandleStaticTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.OverrideTokenKeyword:
                ParseDefaultKeywords.HandleOverrideTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.VirtualTokenKeyword:
                ParseDefaultKeywords.HandleVirtualTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.AbstractTokenKeyword:
                ParseDefaultKeywords.HandleAbstractTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.SealedTokenKeyword:
                ParseDefaultKeywords.HandleSealedTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.IfTokenKeyword:
                ParseDefaultKeywords.HandleIfTokenKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.NewTokenKeyword:
                ParseDefaultKeywords.HandleNewTokenKeyword(compilationUnit, ref parserModel);
                break;
            default:
                ParseDefaultKeywords.HandleDefault(compilationUnit, ref parserModel);
                break;
        }
    }

    public static void ParseKeywordContextualToken(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        switch (parserModel.TokenWalker.Current.SyntaxKind)
        {
            case SyntaxKind.VarTokenContextualKeyword:
                ParseContextualKeywords.HandleVarTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.PartialTokenContextualKeyword:
                ParseContextualKeywords.HandlePartialTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.AddTokenContextualKeyword:
                ParseContextualKeywords.HandleAddTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.AndTokenContextualKeyword:
                ParseContextualKeywords.HandleAndTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.AliasTokenContextualKeyword:
                ParseContextualKeywords.HandleAliasTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.AscendingTokenContextualKeyword:
                ParseContextualKeywords.HandleAscendingTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ArgsTokenContextualKeyword:
                ParseContextualKeywords.HandleArgsTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.AsyncTokenContextualKeyword:
                ParseContextualKeywords.HandleAsyncTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.AwaitTokenContextualKeyword:
                ParseContextualKeywords.HandleAwaitTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ByTokenContextualKeyword:
                ParseContextualKeywords.HandleByTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.DescendingTokenContextualKeyword:
                ParseContextualKeywords.HandleDescendingTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.DynamicTokenContextualKeyword:
                ParseContextualKeywords.HandleDynamicTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.EqualsTokenContextualKeyword:
                ParseContextualKeywords.HandleEqualsTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.FileTokenContextualKeyword:
                ParseContextualKeywords.HandleFileTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.FromTokenContextualKeyword:
                ParseContextualKeywords.HandleFromTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.GetTokenContextualKeyword:
                ParseContextualKeywords.HandleGetTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.GlobalTokenContextualKeyword:
                ParseContextualKeywords.HandleGlobalTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.GroupTokenContextualKeyword:
                ParseContextualKeywords.HandleGroupTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.InitTokenContextualKeyword:
                ParseContextualKeywords.HandleInitTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.IntoTokenContextualKeyword:
                ParseContextualKeywords.HandleIntoTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.JoinTokenContextualKeyword:
                ParseContextualKeywords.HandleJoinTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.LetTokenContextualKeyword:
                ParseContextualKeywords.HandleLetTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ManagedTokenContextualKeyword:
                ParseContextualKeywords.HandleManagedTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.NameofTokenContextualKeyword:
                ParseContextualKeywords.HandleNameofTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.NintTokenContextualKeyword:
                ParseContextualKeywords.HandleNintTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.NotTokenContextualKeyword:
                ParseContextualKeywords.HandleNotTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.NotnullTokenContextualKeyword:
                ParseContextualKeywords.HandleNotnullTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.NuintTokenContextualKeyword:
                ParseContextualKeywords.HandleNuintTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.OnTokenContextualKeyword:
                ParseContextualKeywords.HandleOnTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.OrTokenContextualKeyword:
                ParseContextualKeywords.HandleOrTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.OrderbyTokenContextualKeyword:
                ParseContextualKeywords.HandleOrderbyTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.RecordTokenContextualKeyword:
                ParseContextualKeywords.HandleRecordTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.RemoveTokenContextualKeyword:
                ParseContextualKeywords.HandleRemoveTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.RequiredTokenContextualKeyword:
                ParseContextualKeywords.HandleRequiredTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ScopedTokenContextualKeyword:
                ParseContextualKeywords.HandleScopedTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.SelectTokenContextualKeyword:
                ParseContextualKeywords.HandleSelectTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.SetTokenContextualKeyword:
                ParseContextualKeywords.HandleSetTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.UnmanagedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnmanagedTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.ValueTokenContextualKeyword:
                ParseContextualKeywords.HandleValueTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.WhenTokenContextualKeyword:
                ParseContextualKeywords.HandleWhenTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.WhereTokenContextualKeyword:
                ParseContextualKeywords.HandleWhereTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.WithTokenContextualKeyword:
                ParseContextualKeywords.HandleWithTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.YieldTokenContextualKeyword:
                ParseContextualKeywords.HandleYieldTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            case SyntaxKind.UnrecognizedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnrecognizedTokenContextualKeyword(compilationUnit, ref parserModel);
                break;
            default:
            	parserModel.DiagnosticBag.ReportTodoException(parserModel.TokenWalker.Current.TextSpan, $"Implement the {parserModel.TokenWalker.Current.SyntaxKind.ToString()} contextual keyword.");
            	break;
        }
    }
}
