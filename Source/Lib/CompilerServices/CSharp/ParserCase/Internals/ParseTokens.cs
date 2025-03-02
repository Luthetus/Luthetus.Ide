using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.Facts;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseTokens
{
    public static void ParseIdentifierToken(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	if (parserComputation.TokenWalker.Current.TextSpan.Length == 1 &&
    		parserComputation.TokenWalker.Current.TextSpan.GetText() == "_")
    	{
    		if (!parserComputation.Binder.TryGetVariableDeclarationHierarchically(
			    	compilationUnit,
			    	compilationUnit.ResourceUri,
			    	parserComputation.CurrentScopeIndexKey,
			        parserComputation.TokenWalker.Current.TextSpan.GetText(),
			        out _))
			{
				parserComputation.Binder.BindDiscard(parserComputation.TokenWalker.Current, compilationUnit, ref parserComputation);
	    		var identifierToken = parserComputation.TokenWalker.Consume();
	    		
	    		var variableReferenceNode = new VariableReferenceNode(
	    			identifierToken,
        			null);
        			
	    		parserComputation.StatementBuilder.ChildList.Add(variableReferenceNode);
	    		return;
			}
    	}
    	
    	var originalTokenIndex = parserComputation.TokenWalker.Index;
    	
    	parserComputation.TryParseExpressionSyntaxKindList.Add(SyntaxKind.TypeClauseNode);
    	parserComputation.TryParseExpressionSyntaxKindList.Add(SyntaxKind.VariableDeclarationNode);
    	parserComputation.TryParseExpressionSyntaxKindList.Add(SyntaxKind.VariableReferenceNode);
    	parserComputation.TryParseExpressionSyntaxKindList.Add(SyntaxKind.ConstructorInvocationExpressionNode);
    	
    	if (parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind != SyntaxKind.TypeDefinitionNode)
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
    		parserComputation.TryParseExpressionSyntaxKindList.Add(SyntaxKind.FunctionInvocationNode);
    	}
    	
    	parserComputation.ParserContextKind = CSharpParserContextKind.ForceStatementExpression;
    	
		var successParse = ParseOthers.TryParseExpression(null, compilationUnit, ref parserComputation, out var expressionNode);
		
		if (!successParse)
		{
			expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
			parserComputation.StatementBuilder.ChildList.Add(expressionNode);
	    	return;
		}
		
		switch (expressionNode.SyntaxKind)
		{
			case SyntaxKind.TypeClauseNode:
				MoveToHandleTypeClauseNode(originalTokenIndex, (TypeClauseNode)expressionNode, compilationUnit, ref parserComputation);
				return;
			case SyntaxKind.VariableDeclarationNode:
				if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
    				parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
    			{
    				MoveToHandleFunctionDefinition((VariableDeclarationNode)expressionNode, compilationUnit, ref parserComputation);
				    return;
    			}
    			
    			MoveToHandleVariableDeclarationNode((VariableDeclarationNode)expressionNode, compilationUnit, ref parserComputation);
				return;
	        case SyntaxKind.VariableReferenceNode:
	        
	        	var isQuestionMarkMemberAccessToken = parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.QuestionMarkToken &&
	        		parserComputation.TokenWalker.Next.SyntaxKind == SyntaxKind.MemberAccessToken;
	        
	        	if ((parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.MemberAccessToken || isQuestionMarkMemberAccessToken) &&
	        		originalTokenIndex == parserComputation.TokenWalker.Index - 1)
				{
					_ = parserComputation.TokenWalker.Backtrack();
					expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
					parserComputation.StatementBuilder.ChildList.Add(expressionNode);
					return;
				}
				
				parserComputation.StatementBuilder.ChildList.Add(expressionNode);
				return;
	        case SyntaxKind.FunctionInvocationNode:
			case SyntaxKind.ConstructorInvocationExpressionNode:
				parserComputation.StatementBuilder.ChildList.Add(expressionNode);
				return;
			default:
				// compilationUnit.DiagnosticBag.ReportTodoException(parserComputation.TokenWalker.Current.TextSpan, $"nameof(ParseIdentifierToken) default case");
				return;
		}
    }
    
    public static void MoveToHandleFunctionDefinition(VariableDeclarationNode variableDeclarationNode, CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	ParseFunctions.HandleFunctionDefinition(
			variableDeclarationNode.IdentifierToken,
	        variableDeclarationNode.TypeClauseNode,
	        consumedGenericArgumentsListingNode: null,
	        compilationUnit,
	        ref parserComputation);
    }
    
    public static void MoveToHandleVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode, CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var variableKind = VariableKind.Local;
    			
		if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
			parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
		{
			variableKind = VariableKind.Property;
		}
		else if (parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind == SyntaxKind.TypeDefinitionNode)
		{
			variableKind = VariableKind.Field;
		}
		
		((VariableDeclarationNode)variableDeclarationNode).VariableKind = variableKind;
		
		parserComputation.Binder.BindVariableDeclarationNode(variableDeclarationNode, compilationUnit, ref parserComputation);
        parserComputation.CurrentCodeBlockBuilder.ChildList.Add(variableDeclarationNode);
		parserComputation.StatementBuilder.ChildList.Add(variableDeclarationNode);
		
		if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
		{
			ParsePropertyDefinition_ExpressionBound(compilationUnit, ref parserComputation);
		}
		else
		{
			if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
				ParsePropertyDefinition(compilationUnit, variableDeclarationNode, ref parserComputation);
			
			if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
			{
				parserComputation.TokenWalker.Backtrack();
				var expression = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
				
				if (variableDeclarationNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText() ==
				        CSharpFacts.Types.Var.TypeIdentifierToken.TextSpan.GetText())
				{
					if (expression.SyntaxKind == SyntaxKind.BinaryExpressionNode)
					{
						var binaryExpressionNode = (BinaryExpressionNode)expression;
						
						if (binaryExpressionNode.BinaryOperatorNode.OperatorToken.SyntaxKind == SyntaxKind.EqualsToken)
							variableDeclarationNode.SetTypeClauseNode(binaryExpressionNode.RightExpressionNode.ResultTypeClauseNode);
					}
				}
				
				parserComputation.StatementBuilder.ChildList.Add(expression);
			}
		}
    }
    
    public static void MoveToHandleTypeClauseNode(int originalTokenIndex, TypeClauseNode typeClauseNode, CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
			parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.EndOfFileToken ||
			parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
			parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
		{
			parserComputation.StatementBuilder.ChildList.Add(typeClauseNode);
		}
		else if (parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
				 UtilityApi.IsConvertibleToIdentifierToken(typeClauseNode.TypeIdentifierToken.SyntaxKind) &&
				 parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken &&
			     typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText() == typeClauseNode.TypeIdentifierToken.TextSpan.GetText())
		{
			// ConstructorDefinitionNode
			
			var identifierToken = UtilityApi.ConvertToIdentifierToken(typeClauseNode.TypeIdentifierToken, compilationUnit, ref parserComputation);
			
			ParseFunctions.HandleConstructorDefinition(
				typeDefinitionNode,
		        identifierToken,
		        compilationUnit,
		        ref parserComputation);
		}
		else
		{
			parserComputation.StatementBuilder.ChildList.Add(typeClauseNode);
		}
		
		return;
    }
    
    public static void ParsePropertyDefinition(CSharpCompilationUnit compilationUnit, IVariableDeclarationNode variableDeclarationNode, ref CSharpParserComputation parserComputation)
    {
		#if DEBUG
		parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		var openBraceToken = parserComputation.TokenWalker.Consume();
    	
    	var openBraceCounter = 1;
		
		while (true)
		{
			if (parserComputation.TokenWalker.IsEof)
				break;

			if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
			{
				++openBraceCounter;
			}
			else if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
			{
				if (--openBraceCounter <= 0)
					break;
			}
			else if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.GetTokenContextualKeyword)
			{
				variableDeclarationNode.HasGetter = true;
			}
			else if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.SetTokenContextualKeyword)
			{
				variableDeclarationNode.HasSetter = true;
			}

			_ = parserComputation.TokenWalker.Consume();
		}

		var closeTokenIndex = parserComputation.TokenWalker.Index;
		var closeBraceToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseBraceToken);
		
		#if DEBUG
		parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif
    }
    
    public static void ParsePropertyDefinition_ExpressionBound(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
		var equalsCloseAngleBracketToken = parserComputation.TokenWalker.Consume();
		
		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
		var statementDelimiterToken = parserComputation.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
    }

	/// <summary>
	/// OpenBraceToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseOpenBraceToken(SyntaxToken openBraceToken, CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
		if (parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan ||
    		parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner.OpenCodeBlockTextSpan is not null)
		{
			var arbitraryCodeBlockNode = new ArbitraryCodeBlockNode(parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner);
			
			parserComputation.Binder.NewScopeAndBuilderFromOwner(
		    	arbitraryCodeBlockNode,
		        arbitraryCodeBlockNode.GetReturnTypeClauseNode(),
		        openBraceToken.TextSpan,
		        compilationUnit,
		        ref parserComputation);
		}
		
		parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = false;

		// Global scope has a null parent.
		var parentScopeDirection = parserComputation.CurrentCodeBlockBuilder.Parent?.CodeBlockOwner.ScopeDirectionKind ?? ScopeDirectionKind.Both;
		
		if (parentScopeDirection == ScopeDirectionKind.Both)
		{
			if (!parserComputation.CurrentCodeBlockBuilder.PermitCodeBlockParsing)
			{
				parserComputation.TokenWalker.DeferParsingOfChildScope(compilationUnit, ref parserComputation);
				return;
			}

			parserComputation.CurrentCodeBlockBuilder.PermitCodeBlockParsing = false;
		}
		
		// This has to come after the 'DeferParsingOfChildScope(...)'
		// or it makes an ArbitraryCodeBlockNode when it comes back around.
		parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner.SetOpenCodeBlockTextSpan(openBraceToken.TextSpan, compilationUnit.__DiagnosticList, parserComputation.TokenWalker);
    }

	/// <summary>
	/// CloseBraceToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseCloseBraceToken(SyntaxToken closeBraceToken, int closeBraceTokenIndex, CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	// while () if not CloseBraceToken accepting bubble up until someone takes it or null parent.
    	
    	/*if (parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan)
    	{
    		throw new NotImplementedException("ParseCloseBraceToken(...) -> if (parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan)");
    	}*/
    
    	if (parserComputation.ParseChildScopeStack.Count > 0)
		{
			var tuple = parserComputation.ParseChildScopeStack.Peek();
			
			if (Object.ReferenceEquals(tuple.CodeBlockOwner, parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner))
			{
				tuple = parserComputation.ParseChildScopeStack.Pop();
				tuple.DeferredChildScope.PrepareMainParserLoop(closeBraceTokenIndex, compilationUnit, ref parserComputation);
				return;
			}
		}

		if (parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind != SyntaxKind.GlobalCodeBlockNode)
			parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner.SetCloseCodeBlockTextSpan(closeBraceToken.TextSpan, compilationUnit.__DiagnosticList, parserComputation.TokenWalker);
        
        parserComputation.Binder.CloseScope(closeBraceToken.TextSpan, compilationUnit, ref parserComputation);
    }

    public static void ParseOpenParenthesisToken(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var originalTokenIndex = parserComputation.TokenWalker.Index;
    	
    	parserComputation.TryParseExpressionSyntaxKindList.Add(SyntaxKind.VariableDeclarationNode);
    	parserComputation.TryParseExpressionSyntaxKindList.Add(SyntaxKind.TypeClauseNode);
    	parserComputation.TryParseExpressionSyntaxKindList.Add(SyntaxKind.AmbiguousParenthesizedExpressionNode);
    	
    	parserComputation.ParserContextKind = CSharpParserContextKind.ForceStatementExpression;
    	
		var successParse = ParseOthers.TryParseExpression(null, compilationUnit, ref parserComputation, out var expressionNode);
		
		if (!successParse)
		{
			expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
			parserComputation.StatementBuilder.ChildList.Add(expressionNode);
	    	return;
		}
		
		if (expressionNode.SyntaxKind == SyntaxKind.VariableDeclarationNode)
		{
			if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
				parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
			{
				MoveToHandleFunctionDefinition((VariableDeclarationNode)expressionNode, compilationUnit, ref parserComputation);
			    return;
			}
			
			MoveToHandleVariableDeclarationNode((VariableDeclarationNode)expressionNode, compilationUnit, ref parserComputation);
		}
		
		//// I am catching the next two but not doing anything with them
		//// only so that the TryParseExpression won't return early due to those being the
		//// SyntaxKind(s) that will appear during the process of parsing the VariableDeclarationNode
		//// given that the TypeClauseNode is a tuple.
		//
		// else if (expressionNode.SyntaxKind == SyntaxKind.TypeClauseNode)
		// else if (expressionNode.SyntaxKind == SyntaxKind.AmbiguousParenthesizedExpressionNode)
    }

    public static void ParseOpenSquareBracketToken(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var openSquareBracketToken = parserComputation.TokenWalker.Consume();
    
    	if (parserComputation.StatementBuilder.ChildList.Count != 0)
    	{
    		/*compilationUnit.DiagnosticBag.ReportTodoException(
	    		openSquareBracketToken.TextSpan,
	    		$"Unexpected '{nameof(SyntaxKind.OpenSquareBracketToken)}'");*/
	    	return;
	    }
    	var openSquareBracketCounter = 1;
		var corruptState = false;
		
		#if DEBUG
		parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		while (!parserComputation.TokenWalker.IsEof)
		{
			if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenSquareBracketToken)
			{
				++openSquareBracketCounter;
			}
			else if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
			{
				if (--openSquareBracketCounter <= 0)
					break;
			}
			else if (!corruptState)
			{
				var tokenIndexOriginal = parserComputation.TokenWalker.Index;
				
				parserComputation.ExpressionList.Add((SyntaxKind.CloseSquareBracketToken, null));
				parserComputation.ExpressionList.Add((SyntaxKind.CommaToken, null));
				var expression = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
				
				if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
					_ = parserComputation.TokenWalker.Consume();
					
				if (tokenIndexOriginal < parserComputation.TokenWalker.Index)
					continue; // Already consumed so avoid the one at the end of the while loop
			}

			_ = parserComputation.TokenWalker.Consume();
		}

		var closeTokenIndex = parserComputation.TokenWalker.Index;
		var closeSquareBracketToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseSquareBracketToken);
		
		#if DEBUG
		parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif
    }

    public static void ParseEqualsToken(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	if (parserComputation.StatementBuilder.ChildList.Count == 1 &&
    		(parserComputation.StatementBuilder.ChildList[0].SyntaxKind == SyntaxKind.VariableReferenceNode ||
    		 	parserComputation.StatementBuilder.ChildList[0].SyntaxKind == SyntaxKind.TypeClauseNode))
    	{
    		_ = parserComputation.TokenWalker.Backtrack();
    	}
    	
    	ParseOthers.StartStatement_Expression(compilationUnit, ref parserComputation);
	}

	/// <summary>
	/// StatementDelimiterToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseStatementDelimiterToken(SyntaxToken statementDelimiterToken, CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	if (parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind == SyntaxKind.NamespaceStatementNode)
        {
        	var namespaceStatementNode = (NamespaceStatementNode)parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner;
        	
            ICodeBlockOwner nextCodeBlockOwner = namespaceStatementNode;
            TypeClauseNode? scopeReturnTypeClauseNode = null;
            
            namespaceStatementNode.SetCloseCodeBlockTextSpan(statementDelimiterToken.TextSpan, compilationUnit.__DiagnosticList, parserComputation.TokenWalker);

            parserComputation.Binder.AddNamespaceToCurrentScope(
                namespaceStatementNode.IdentifierToken.TextSpan.GetText(),
                compilationUnit,
                ref parserComputation);
        }
        else 
        {
        	while (parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind != SyntaxKind.GlobalCodeBlockNode &&
        		   parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan)
        	{
        		parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner.SetCloseCodeBlockTextSpan(statementDelimiterToken.TextSpan, compilationUnit.__DiagnosticList, parserComputation.TokenWalker);
	        	parserComputation.Binder.CloseScope(statementDelimiterToken.TextSpan, compilationUnit, ref parserComputation);
        	}
        }
    }

    public static void ParseKeywordToken(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        // 'return', 'if', 'get', etc...
        switch (parserComputation.TokenWalker.Current.SyntaxKind)
        {
            case SyntaxKind.AsTokenKeyword:
                ParseDefaultKeywords.HandleAsTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.BaseTokenKeyword:
                ParseDefaultKeywords.HandleBaseTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.BoolTokenKeyword:
                ParseDefaultKeywords.HandleBoolTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.BreakTokenKeyword:
                ParseDefaultKeywords.HandleBreakTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ByteTokenKeyword:
                ParseDefaultKeywords.HandleByteTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.CaseTokenKeyword:
                ParseDefaultKeywords.HandleCaseTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.CatchTokenKeyword:
                ParseDefaultKeywords.HandleCatchTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.CharTokenKeyword:
                ParseDefaultKeywords.HandleCharTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.CheckedTokenKeyword:
                ParseDefaultKeywords.HandleCheckedTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ConstTokenKeyword:
                ParseDefaultKeywords.HandleConstTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ContinueTokenKeyword:
                ParseDefaultKeywords.HandleContinueTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.DecimalTokenKeyword:
                ParseDefaultKeywords.HandleDecimalTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.DefaultTokenKeyword:
                ParseDefaultKeywords.HandleDefaultTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.DelegateTokenKeyword:
                ParseDefaultKeywords.HandleDelegateTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.DoTokenKeyword:
                ParseDefaultKeywords.HandleDoTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.DoubleTokenKeyword:
                ParseDefaultKeywords.HandleDoubleTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ElseTokenKeyword:
                ParseDefaultKeywords.HandleElseTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.EnumTokenKeyword:
                ParseDefaultKeywords.HandleEnumTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.EventTokenKeyword:
                ParseDefaultKeywords.HandleEventTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ExplicitTokenKeyword:
                ParseDefaultKeywords.HandleExplicitTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ExternTokenKeyword:
                ParseDefaultKeywords.HandleExternTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.FalseTokenKeyword:
                ParseDefaultKeywords.HandleFalseTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.FinallyTokenKeyword:
                ParseDefaultKeywords.HandleFinallyTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.FixedTokenKeyword:
                ParseDefaultKeywords.HandleFixedTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.FloatTokenKeyword:
                ParseDefaultKeywords.HandleFloatTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ForTokenKeyword:
                ParseDefaultKeywords.HandleForTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ForeachTokenKeyword:
                ParseDefaultKeywords.HandleForeachTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.GotoTokenKeyword:
                ParseDefaultKeywords.HandleGotoTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ImplicitTokenKeyword:
                ParseDefaultKeywords.HandleImplicitTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.InTokenKeyword:
                ParseDefaultKeywords.HandleInTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.IntTokenKeyword:
                ParseDefaultKeywords.HandleIntTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.IsTokenKeyword:
                ParseDefaultKeywords.HandleIsTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.LockTokenKeyword:
                ParseDefaultKeywords.HandleLockTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.LongTokenKeyword:
                ParseDefaultKeywords.HandleLongTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.NullTokenKeyword:
                ParseDefaultKeywords.HandleNullTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ObjectTokenKeyword:
                ParseDefaultKeywords.HandleObjectTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.OperatorTokenKeyword:
                ParseDefaultKeywords.HandleOperatorTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.OutTokenKeyword:
                ParseDefaultKeywords.HandleOutTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ParamsTokenKeyword:
                ParseDefaultKeywords.HandleParamsTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ProtectedTokenKeyword:
                ParseDefaultKeywords.HandleProtectedTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ReadonlyTokenKeyword:
                ParseDefaultKeywords.HandleReadonlyTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.RefTokenKeyword:
                ParseDefaultKeywords.HandleRefTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.SbyteTokenKeyword:
                ParseDefaultKeywords.HandleSbyteTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ShortTokenKeyword:
                ParseDefaultKeywords.HandleShortTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.SizeofTokenKeyword:
                ParseDefaultKeywords.HandleSizeofTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.StackallocTokenKeyword:
                ParseDefaultKeywords.HandleStackallocTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.StringTokenKeyword:
                ParseDefaultKeywords.HandleStringTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.StructTokenKeyword:
                ParseDefaultKeywords.HandleStructTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.SwitchTokenKeyword:
                ParseDefaultKeywords.HandleSwitchTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ThisTokenKeyword:
                ParseDefaultKeywords.HandleThisTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ThrowTokenKeyword:
                ParseDefaultKeywords.HandleThrowTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.TrueTokenKeyword:
                ParseDefaultKeywords.HandleTrueTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.TryTokenKeyword:
                ParseDefaultKeywords.HandleTryTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.TypeofTokenKeyword:
                ParseDefaultKeywords.HandleTypeofTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.UintTokenKeyword:
                ParseDefaultKeywords.HandleUintTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.UlongTokenKeyword:
                ParseDefaultKeywords.HandleUlongTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.UncheckedTokenKeyword:
                ParseDefaultKeywords.HandleUncheckedTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.UnsafeTokenKeyword:
                ParseDefaultKeywords.HandleUnsafeTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.UshortTokenKeyword:
                ParseDefaultKeywords.HandleUshortTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.VoidTokenKeyword:
                ParseDefaultKeywords.HandleVoidTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.VolatileTokenKeyword:
                ParseDefaultKeywords.HandleVolatileTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.WhileTokenKeyword:
                ParseDefaultKeywords.HandleWhileTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.UnrecognizedTokenKeyword:
                ParseDefaultKeywords.HandleUnrecognizedTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ReturnTokenKeyword:
                ParseDefaultKeywords.HandleReturnTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.NamespaceTokenKeyword:
                ParseDefaultKeywords.HandleNamespaceTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ClassTokenKeyword:
                ParseDefaultKeywords.HandleClassTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.InterfaceTokenKeyword:
                ParseDefaultKeywords.HandleInterfaceTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.UsingTokenKeyword:
                ParseDefaultKeywords.HandleUsingTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.PublicTokenKeyword:
                ParseDefaultKeywords.HandlePublicTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.InternalTokenKeyword:
                ParseDefaultKeywords.HandleInternalTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.PrivateTokenKeyword:
                ParseDefaultKeywords.HandlePrivateTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.StaticTokenKeyword:
                ParseDefaultKeywords.HandleStaticTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.OverrideTokenKeyword:
                ParseDefaultKeywords.HandleOverrideTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.VirtualTokenKeyword:
                ParseDefaultKeywords.HandleVirtualTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.AbstractTokenKeyword:
                ParseDefaultKeywords.HandleAbstractTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.SealedTokenKeyword:
                ParseDefaultKeywords.HandleSealedTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.IfTokenKeyword:
                ParseDefaultKeywords.HandleIfTokenKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.NewTokenKeyword:
                ParseDefaultKeywords.HandleNewTokenKeyword(compilationUnit, ref parserComputation);
                break;
            default:
                ParseDefaultKeywords.HandleDefault(compilationUnit, ref parserComputation);
                break;
        }
    }

    public static void ParseKeywordContextualToken(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        switch (parserComputation.TokenWalker.Current.SyntaxKind)
        {
            case SyntaxKind.VarTokenContextualKeyword:
                ParseContextualKeywords.HandleVarTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.PartialTokenContextualKeyword:
                ParseContextualKeywords.HandlePartialTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.AddTokenContextualKeyword:
                ParseContextualKeywords.HandleAddTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.AndTokenContextualKeyword:
                ParseContextualKeywords.HandleAndTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.AliasTokenContextualKeyword:
                ParseContextualKeywords.HandleAliasTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.AscendingTokenContextualKeyword:
                ParseContextualKeywords.HandleAscendingTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ArgsTokenContextualKeyword:
                ParseContextualKeywords.HandleArgsTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.AsyncTokenContextualKeyword:
                ParseContextualKeywords.HandleAsyncTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.AwaitTokenContextualKeyword:
                ParseContextualKeywords.HandleAwaitTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ByTokenContextualKeyword:
                ParseContextualKeywords.HandleByTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.DescendingTokenContextualKeyword:
                ParseContextualKeywords.HandleDescendingTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.DynamicTokenContextualKeyword:
                ParseContextualKeywords.HandleDynamicTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.EqualsTokenContextualKeyword:
                ParseContextualKeywords.HandleEqualsTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.FileTokenContextualKeyword:
                ParseContextualKeywords.HandleFileTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.FromTokenContextualKeyword:
                ParseContextualKeywords.HandleFromTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.GetTokenContextualKeyword:
                ParseContextualKeywords.HandleGetTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.GlobalTokenContextualKeyword:
                ParseContextualKeywords.HandleGlobalTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.GroupTokenContextualKeyword:
                ParseContextualKeywords.HandleGroupTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.InitTokenContextualKeyword:
                ParseContextualKeywords.HandleInitTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.IntoTokenContextualKeyword:
                ParseContextualKeywords.HandleIntoTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.JoinTokenContextualKeyword:
                ParseContextualKeywords.HandleJoinTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.LetTokenContextualKeyword:
                ParseContextualKeywords.HandleLetTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ManagedTokenContextualKeyword:
                ParseContextualKeywords.HandleManagedTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.NameofTokenContextualKeyword:
                ParseContextualKeywords.HandleNameofTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.NintTokenContextualKeyword:
                ParseContextualKeywords.HandleNintTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.NotTokenContextualKeyword:
                ParseContextualKeywords.HandleNotTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.NotnullTokenContextualKeyword:
                ParseContextualKeywords.HandleNotnullTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.NuintTokenContextualKeyword:
                ParseContextualKeywords.HandleNuintTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.OnTokenContextualKeyword:
                ParseContextualKeywords.HandleOnTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.OrTokenContextualKeyword:
                ParseContextualKeywords.HandleOrTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.OrderbyTokenContextualKeyword:
                ParseContextualKeywords.HandleOrderbyTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.RecordTokenContextualKeyword:
                ParseContextualKeywords.HandleRecordTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.RemoveTokenContextualKeyword:
                ParseContextualKeywords.HandleRemoveTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.RequiredTokenContextualKeyword:
                ParseContextualKeywords.HandleRequiredTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ScopedTokenContextualKeyword:
                ParseContextualKeywords.HandleScopedTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.SelectTokenContextualKeyword:
                ParseContextualKeywords.HandleSelectTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.SetTokenContextualKeyword:
                ParseContextualKeywords.HandleSetTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.UnmanagedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnmanagedTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.ValueTokenContextualKeyword:
                ParseContextualKeywords.HandleValueTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.WhenTokenContextualKeyword:
                ParseContextualKeywords.HandleWhenTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.WhereTokenContextualKeyword:
                ParseContextualKeywords.HandleWhereTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.WithTokenContextualKeyword:
                ParseContextualKeywords.HandleWithTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.YieldTokenContextualKeyword:
                ParseContextualKeywords.HandleYieldTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            case SyntaxKind.UnrecognizedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnrecognizedTokenContextualKeyword(compilationUnit, ref parserComputation);
                break;
            default:
            	// compilationUnit.DiagnosticBag.ReportTodoException(parserComputation.TokenWalker.Current.TextSpan, $"Implement the {parserComputation.TokenWalker.Current.SyntaxKind.ToString()} contextual keyword.");
            	break;
        }
    }
}
