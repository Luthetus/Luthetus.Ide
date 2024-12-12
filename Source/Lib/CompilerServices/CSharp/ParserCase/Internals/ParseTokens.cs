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
        CSharpCompilationUnit compilationUnit)
    {
        var consumedToken = compilationUnit.ParserModel.TokenWalker.Consume();
    }

    public static void ParseIdentifierToken(CSharpCompilationUnit compilationUnit)
    {
    	var originalTokenIndex = compilationUnit.ParserModel.TokenWalker.Index;
    	
    	compilationUnit.ParserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.TypeClauseNode);
    	compilationUnit.ParserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.VariableDeclarationNode);
    	compilationUnit.ParserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.VariableReferenceNode);
    	compilationUnit.ParserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.ConstructorInvocationExpressionNode);
    	
    	if (compilationUnit.ParserModel.CurrentCodeBlockBuilder.CodeBlockOwner is not null &&
			compilationUnit.ParserModel.CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind != SyntaxKind.TypeDefinitionNode)
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
    		compilationUnit.ParserModel.TryParseExpressionSyntaxKindList.Add(SyntaxKind.FunctionInvocationNode);
    	}
    	
		var successParse = ParseOthers.TryParseExpression(null, compilationUnit, out var expressionNode);
		
		if (!successParse)
		{
			expressionNode = ParseOthers.ParseExpression(compilationUnit);
			compilationUnit.ParserModel.StatementBuilder.ChildList.Add(expressionNode);
	    	return;
		}
		
		switch (expressionNode.SyntaxKind)
		{
			case SyntaxKind.TypeClauseNode:
				MoveToHandleTypeClauseNode(originalTokenIndex, (TypeClauseNode)expressionNode, compilationUnit);
				return;
			case SyntaxKind.VariableDeclarationNode:
				if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
    				compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
    			{
    				MoveToHandleFunctionDefinition((VariableDeclarationNode)expressionNode, compilationUnit);
				    return;
    			}
    			
    			MoveToHandleVariableDeclarationNode((VariableDeclarationNode)expressionNode, compilationUnit);
				return;
	        case SyntaxKind.VariableReferenceNode:
	        case SyntaxKind.FunctionInvocationNode:
			case SyntaxKind.ConstructorInvocationExpressionNode:
				compilationUnit.ParserModel.StatementBuilder.ChildList.Add(expressionNode);
				return;
			default:
				compilationUnit.ParserModel.DiagnosticBag.ReportTodoException(compilationUnit.ParserModel.TokenWalker.Current.TextSpan, $"nameof(ParseIdentifierToken) default case");
				return;
		}
    }
    
    public static void MoveToHandleFunctionDefinition(VariableDeclarationNode variableDeclarationNode, CSharpCompilationUnit compilationUnit)
    {
    	ParseFunctions.HandleFunctionDefinition(
			variableDeclarationNode.IdentifierToken,
	        variableDeclarationNode.TypeClauseNode,
	        consumedGenericArgumentsListingNode: null,
	        compilationUnit);
    }
    
    public static void MoveToHandleVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode, CSharpCompilationUnit compilationUnit)
    {
    	var variableKind = VariableKind.Local;
    			
		if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
			(compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken &&
				 compilationUnit.ParserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken))
		{
			variableKind = VariableKind.Property;
		}
		else if (compilationUnit.ParserModel.CurrentCodeBlockBuilder.CodeBlockOwner is not null &&
				 compilationUnit.ParserModel.CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind == SyntaxKind.TypeDefinitionNode)
		{
			variableKind = VariableKind.Field;
		}
		
		((VariableDeclarationNode)variableDeclarationNode).VariableKind = variableKind;
		
		compilationUnit.ParserModel.Binder.BindVariableDeclarationNode(variableDeclarationNode, compilationUnit);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.ChildList.Add(variableDeclarationNode);
		compilationUnit.ParserModel.StatementBuilder.ChildList.Add(variableDeclarationNode);
		
		if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
		{
			ParsePropertyDefinition(compilationUnit, variableDeclarationNode);
		}
		else if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken &&
				 compilationUnit.ParserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
		{
			ParsePropertyDefinition_ExpressionBound(compilationUnit);
		}
    }
    
    public static void MoveToHandleTypeClauseNode(int originalTokenIndex, TypeClauseNode typeClauseNode, CSharpCompilationUnit compilationUnit)
    {
    	if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
			compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EndOfFileToken ||
			compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
			compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
		{
			compilationUnit.ParserModel.StatementBuilder.ChildList.Add(typeClauseNode);
		}
		else if (compilationUnit.ParserModel.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
				 UtilityApi.IsConvertibleToIdentifierToken(typeClauseNode.TypeIdentifierToken.SyntaxKind) &&
				 compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken &&
			     typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText() == typeClauseNode.TypeIdentifierToken.TextSpan.GetText())
		{
			// ConstructorDefinitionNode
			
			var identifierToken = UtilityApi.ConvertToIdentifierToken(typeClauseNode.TypeIdentifierToken, compilationUnit);
			
			ParseFunctions.HandleConstructorDefinition(
				typeDefinitionNode,
		        identifierToken,
		        compilationUnit);
		}
		else
		{
			compilationUnit.ParserModel.StatementBuilder.ChildList.Add(typeClauseNode);
		}
		
		return;
    }
    
    public static void ParsePropertyDefinition(CSharpCompilationUnit compilationUnit, IVariableDeclarationNode variableDeclarationNode)
    {
		#if DEBUG
		compilationUnit.ParserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		var openBraceToken = (OpenBraceToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var openBraceCounter = 1;
		
		while (true)
		{
			if (compilationUnit.ParserModel.TokenWalker.IsEof)
				break;

			if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
			{
				++openBraceCounter;
			}
			else if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
			{
				if (--openBraceCounter <= 0)
					break;
			}
			else if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.GetTokenContextualKeyword)
			{
				variableDeclarationNode.HasGetter = true;
			}
			else if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.SetTokenContextualKeyword)
			{
				variableDeclarationNode.HasSetter = true;
			}

			_ = compilationUnit.ParserModel.TokenWalker.Consume();
		}

		var closeTokenIndex = compilationUnit.ParserModel.TokenWalker.Index;
		var closeBraceToken = (CloseBraceToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.CloseBraceToken);
		
		#if DEBUG
		compilationUnit.ParserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif
    }
    
    public static void ParsePropertyDefinition_ExpressionBound(CSharpCompilationUnit compilationUnit)
    {
		var equalsToken = (EqualsToken)compilationUnit.ParserModel.TokenWalker.Consume();
		var closeAngleBracketToken = (CloseAngleBracketToken)compilationUnit.ParserModel.TokenWalker.Consume();
		
		var expressionNode = ParseOthers.ParseExpression(compilationUnit);
		var statementDelimiterToken = (StatementDelimiterToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
    }

    public static void ParseColonToken(CSharpCompilationUnit compilationUnit)
    {
    	var colonToken = (ColonToken)compilationUnit.ParserModel.TokenWalker.Consume();
    
        if (compilationUnit.ParserModel.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
        {
            var typeDefinitionNode = (TypeDefinitionNode)compilationUnit.ParserModel.SyntaxStack.Pop();
            var inheritedTypeClauseNode = compilationUnit.ParserModel.TokenWalker.MatchTypeClauseNode(compilationUnit);

            compilationUnit.ParserModel.Binder.BindTypeClauseNode(inheritedTypeClauseNode, compilationUnit);

			typeDefinitionNode.SetInheritedTypeClauseNode(inheritedTypeClauseNode);

            compilationUnit.ParserModel.SyntaxStack.Push(typeDefinitionNode);
            compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = typeDefinitionNode;
        }
        else
        {
            compilationUnit.ParserModel.DiagnosticBag.ReportTodoException(colonToken.TextSpan, "Colon is in unexpected place.");
        }
    }

	/// <summary>
	/// OpenBraceToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseOpenBraceToken(OpenBraceToken openBraceToken, CSharpCompilationUnit compilationUnit)
    {    
		if (compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner is null)
		{
			var arbitraryCodeBlockNode = new ArbitraryCodeBlockNode(compilationUnit.ParserModel.CurrentCodeBlockBuilder.CodeBlockOwner);
			compilationUnit.ParserModel.SyntaxStack.Push(arbitraryCodeBlockNode);
        	compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = arbitraryCodeBlockNode;
		}
		
		compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner.SetOpenBraceToken(openBraceToken, compilationUnit);

		var parentScopeDirection = compilationUnit.ParserModel.CurrentCodeBlockBuilder?.CodeBlockOwner?.ScopeDirectionKind ?? ScopeDirectionKind.Both;
		if (parentScopeDirection == ScopeDirectionKind.Both)
		{
			if (!compilationUnit.ParserModel.CurrentCodeBlockBuilder.PermitInnerPendingCodeBlockOwnerToBeParsed)
			{
				compilationUnit.ParserModel.TokenWalker.DeferParsingOfChildScope(openBraceToken, compilationUnit);
				return;
			}

			compilationUnit.ParserModel.CurrentCodeBlockBuilder.PermitInnerPendingCodeBlockOwnerToBeParsed = false;
		}

		var nextCodeBlockOwner = compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner;
		var nextReturnTypeClauseNode = nextCodeBlockOwner.GetReturnTypeClauseNode();

        compilationUnit.ParserModel.Binder.OpenScope(nextCodeBlockOwner, nextReturnTypeClauseNode, openBraceToken.TextSpan, compilationUnit);
		compilationUnit.ParserModel.CurrentCodeBlockBuilder = new(parent: compilationUnit.ParserModel.CurrentCodeBlockBuilder, codeBlockOwner: nextCodeBlockOwner);
		nextCodeBlockOwner.OnBoundScopeCreatedAndSetAsCurrent(compilationUnit);
    }

	/// <summary>
	/// CloseBraceToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseCloseBraceToken(CloseBraceToken closeBraceToken, CSharpCompilationUnit compilationUnit)
    {
		if (compilationUnit.ParserModel.CurrentCodeBlockBuilder.ParseChildScopeQueue.TryDequeue(out var deferredChildScope))
		{
			deferredChildScope.PrepareMainParserLoop(compilationUnit.ParserModel.TokenWalker.Index - 1, compilationUnit);
			return;
		}

		if (compilationUnit.ParserModel.CurrentCodeBlockBuilder.CodeBlockOwner is not null)
			compilationUnit.ParserModel.CurrentCodeBlockBuilder.CodeBlockOwner.SetCloseBraceToken(closeBraceToken, compilationUnit);
		
        compilationUnit.ParserModel.Binder.CloseScope(closeBraceToken.TextSpan, compilationUnit);
    }

    public static void ParseOpenParenthesisToken(CSharpCompilationUnit compilationUnit)
    {
    }

    public static void ParseCloseParenthesisToken(
        CloseParenthesisToken consumedCloseParenthesisToken,
        CSharpCompilationUnit compilationUnit)
    {
    	var closesParenthesisToken = (CloseParenthesisToken)compilationUnit.ParserModel.TokenWalker.Consume();
    }

    public static void ParseOpenAngleBracketToken(
        OpenAngleBracketToken consumedOpenAngleBracketToken,
        CSharpCompilationUnit compilationUnit)
    {
    }

    public static void ParseCloseAngleBracketToken(
        CloseAngleBracketToken consumedCloseAngleBracketToken,
        CSharpCompilationUnit compilationUnit)
    {
    }

    public static void ParseOpenSquareBracketToken(CSharpCompilationUnit compilationUnit)
    {
    	var openSquareBracketToken = (OpenSquareBracketToken)compilationUnit.ParserModel.TokenWalker.Consume();
    
    	if (compilationUnit.ParserModel.StatementBuilder.ChildList.Count != 0)
    	{
    		compilationUnit.ParserModel.DiagnosticBag.ReportTodoException(
	    		openSquareBracketToken.TextSpan,
	    		$"Unexpected '{nameof(OpenSquareBracketToken)}'");
	    	return;
	    }
    	var openSquareBracketCounter = 1;
		var corruptState = false;
		
		#if DEBUG
		compilationUnit.ParserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		while (!compilationUnit.ParserModel.TokenWalker.IsEof)
		{
			if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenSquareBracketToken)
			{
				++openSquareBracketCounter;
			}
			else if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
			{
				if (--openSquareBracketCounter <= 0)
					break;
			}
			else if (!corruptState)
			{
				var tokenIndexOriginal = compilationUnit.ParserModel.TokenWalker.Index;
				
				compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.CloseSquareBracketToken, null));
				compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.CommaToken, null));
				var expression = ParseOthers.ParseExpression(compilationUnit);
				
				if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
					_ = compilationUnit.ParserModel.TokenWalker.Consume();
					
				if (tokenIndexOriginal < compilationUnit.ParserModel.TokenWalker.Index)
					continue; // Already consumed so avoid the one at the end of the while loop
			}

			_ = compilationUnit.ParserModel.TokenWalker.Consume();
		}

		var closeTokenIndex = compilationUnit.ParserModel.TokenWalker.Index;
		var closeSquareBracketToken = (CloseSquareBracketToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.CloseSquareBracketToken);
		
		#if DEBUG
		compilationUnit.ParserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif
    }

    public static void ParseCloseSquareBracketToken(
        CloseSquareBracketToken consumedCloseSquareBracketToken,
        CSharpCompilationUnit compilationUnit)
    {
    }

    public static void ParseEqualsToken(CSharpCompilationUnit compilationUnit)
    {
    	if (compilationUnit.ParserModel.StatementBuilder.ChildList.Count == 0)
    	{
    		ParseOthers.StartStatement_Expression(compilationUnit);
    		return;
    	}
		
		if (compilationUnit.ParserModel.StatementBuilder.TryPeek(out var syntax) &&
			syntax.SyntaxKind == SyntaxKind.VariableDeclarationNode)
		{
			var variableDeclarationNode = (VariableDeclarationNode)syntax;
			
			compilationUnit.ParserModel.TokenWalker.Backtrack();
			var expression = ParseOthers.ParseExpression(compilationUnit);
			
			if (expression.SyntaxKind != SyntaxKind.VariableAssignmentExpressionNode)
			{
				// TODO: Report a diagnostic
				return;
			}
			
			compilationUnit.ParserModel.StatementBuilder.ChildList.Add(expression);
		}
	}

    public static void ParseMemberAccessToken(
        MemberAccessToken consumedMemberAccessToken,
        CSharpCompilationUnit compilationUnit)
    {
    }

	/// <summary>
	/// StatementDelimiterToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseStatementDelimiterToken(StatementDelimiterToken statementDelimiterToken, CSharpCompilationUnit compilationUnit)
    {
    	if (compilationUnit.ParserModel.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.NamespaceStatementNode)
        {
        	var closureCurrentCompilationUnitBuilder = compilationUnit.ParserModel.CurrentCodeBlockBuilder;
            ICodeBlockOwner? nextCodeBlockOwner = null;
            TypeClauseNode? scopeReturnTypeClauseNode = null;

            var namespaceStatementNode = (NamespaceStatementNode)compilationUnit.ParserModel.SyntaxStack.Pop();
            nextCodeBlockOwner = namespaceStatementNode;
            
            namespaceStatementNode.SetStatementDelimiterToken(statementDelimiterToken, compilationUnit);

            compilationUnit.ParserModel.Binder.OpenScope(
            	nextCodeBlockOwner,
                scopeReturnTypeClauseNode,
                statementDelimiterToken.TextSpan,
                compilationUnit);

            compilationUnit.ParserModel.Binder.AddNamespaceToCurrentScope(
                namespaceStatementNode.IdentifierToken.TextSpan.GetText(),
                compilationUnit);

            compilationUnit.ParserModel.CurrentCodeBlockBuilder = new(compilationUnit.ParserModel.CurrentCodeBlockBuilder, nextCodeBlockOwner);
        }
        else if (compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner is not null &&
        		 !compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner.OpenBraceToken.ConstructorWasInvoked)
        {
        	var pendingChild = compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner;
        
        	compilationUnit.ParserModel.Binder.OpenScope(pendingChild, CSharpFacts.Types.Void.ToTypeClause(), statementDelimiterToken.TextSpan, compilationUnit);
			compilationUnit.ParserModel.CurrentCodeBlockBuilder = new(compilationUnit.ParserModel.CurrentCodeBlockBuilder, pendingChild);
			pendingChild.OnBoundScopeCreatedAndSetAsCurrent(compilationUnit);
			
	        compilationUnit.ParserModel.Binder.CloseScope(statementDelimiterToken.TextSpan, compilationUnit);
	
	        if (compilationUnit.ParserModel.CurrentCodeBlockBuilder.Parent is not null)
	            compilationUnit.ParserModel.CurrentCodeBlockBuilder = compilationUnit.ParserModel.CurrentCodeBlockBuilder.Parent;
	            
	        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = null;
        }
    }

    public static void ParseKeywordToken(CSharpCompilationUnit compilationUnit)
    {
        // 'return', 'if', 'get', etc...
        switch (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind)
        {
            case SyntaxKind.AsTokenKeyword:
                ParseDefaultKeywords.HandleAsTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.BaseTokenKeyword:
                ParseDefaultKeywords.HandleBaseTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.BoolTokenKeyword:
                ParseDefaultKeywords.HandleBoolTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.BreakTokenKeyword:
                ParseDefaultKeywords.HandleBreakTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ByteTokenKeyword:
                ParseDefaultKeywords.HandleByteTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.CaseTokenKeyword:
                ParseDefaultKeywords.HandleCaseTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.CatchTokenKeyword:
                ParseDefaultKeywords.HandleCatchTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.CharTokenKeyword:
                ParseDefaultKeywords.HandleCharTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.CheckedTokenKeyword:
                ParseDefaultKeywords.HandleCheckedTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ConstTokenKeyword:
                ParseDefaultKeywords.HandleConstTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ContinueTokenKeyword:
                ParseDefaultKeywords.HandleContinueTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.DecimalTokenKeyword:
                ParseDefaultKeywords.HandleDecimalTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.DefaultTokenKeyword:
                ParseDefaultKeywords.HandleDefaultTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.DelegateTokenKeyword:
                ParseDefaultKeywords.HandleDelegateTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.DoTokenKeyword:
                ParseDefaultKeywords.HandleDoTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.DoubleTokenKeyword:
                ParseDefaultKeywords.HandleDoubleTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ElseTokenKeyword:
                ParseDefaultKeywords.HandleElseTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.EnumTokenKeyword:
                ParseDefaultKeywords.HandleEnumTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.EventTokenKeyword:
                ParseDefaultKeywords.HandleEventTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ExplicitTokenKeyword:
                ParseDefaultKeywords.HandleExplicitTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ExternTokenKeyword:
                ParseDefaultKeywords.HandleExternTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.FalseTokenKeyword:
                ParseDefaultKeywords.HandleFalseTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.FinallyTokenKeyword:
                ParseDefaultKeywords.HandleFinallyTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.FixedTokenKeyword:
                ParseDefaultKeywords.HandleFixedTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.FloatTokenKeyword:
                ParseDefaultKeywords.HandleFloatTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ForTokenKeyword:
                ParseDefaultKeywords.HandleForTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ForeachTokenKeyword:
                ParseDefaultKeywords.HandleForeachTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.GotoTokenKeyword:
                ParseDefaultKeywords.HandleGotoTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ImplicitTokenKeyword:
                ParseDefaultKeywords.HandleImplicitTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.InTokenKeyword:
                ParseDefaultKeywords.HandleInTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.IntTokenKeyword:
                ParseDefaultKeywords.HandleIntTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.IsTokenKeyword:
                ParseDefaultKeywords.HandleIsTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.LockTokenKeyword:
                ParseDefaultKeywords.HandleLockTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.LongTokenKeyword:
                ParseDefaultKeywords.HandleLongTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.NullTokenKeyword:
                ParseDefaultKeywords.HandleNullTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ObjectTokenKeyword:
                ParseDefaultKeywords.HandleObjectTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.OperatorTokenKeyword:
                ParseDefaultKeywords.HandleOperatorTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.OutTokenKeyword:
                ParseDefaultKeywords.HandleOutTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ParamsTokenKeyword:
                ParseDefaultKeywords.HandleParamsTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ProtectedTokenKeyword:
                ParseDefaultKeywords.HandleProtectedTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ReadonlyTokenKeyword:
                ParseDefaultKeywords.HandleReadonlyTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.RefTokenKeyword:
                ParseDefaultKeywords.HandleRefTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.SbyteTokenKeyword:
                ParseDefaultKeywords.HandleSbyteTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ShortTokenKeyword:
                ParseDefaultKeywords.HandleShortTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.SizeofTokenKeyword:
                ParseDefaultKeywords.HandleSizeofTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.StackallocTokenKeyword:
                ParseDefaultKeywords.HandleStackallocTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.StringTokenKeyword:
                ParseDefaultKeywords.HandleStringTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.StructTokenKeyword:
                ParseDefaultKeywords.HandleStructTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.SwitchTokenKeyword:
                ParseDefaultKeywords.HandleSwitchTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ThisTokenKeyword:
                ParseDefaultKeywords.HandleThisTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ThrowTokenKeyword:
                ParseDefaultKeywords.HandleThrowTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.TrueTokenKeyword:
                ParseDefaultKeywords.HandleTrueTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.TryTokenKeyword:
                ParseDefaultKeywords.HandleTryTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.TypeofTokenKeyword:
                ParseDefaultKeywords.HandleTypeofTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.UintTokenKeyword:
                ParseDefaultKeywords.HandleUintTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.UlongTokenKeyword:
                ParseDefaultKeywords.HandleUlongTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.UncheckedTokenKeyword:
                ParseDefaultKeywords.HandleUncheckedTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.UnsafeTokenKeyword:
                ParseDefaultKeywords.HandleUnsafeTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.UshortTokenKeyword:
                ParseDefaultKeywords.HandleUshortTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.VoidTokenKeyword:
                ParseDefaultKeywords.HandleVoidTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.VolatileTokenKeyword:
                ParseDefaultKeywords.HandleVolatileTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.WhileTokenKeyword:
                ParseDefaultKeywords.HandleWhileTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.UnrecognizedTokenKeyword:
                ParseDefaultKeywords.HandleUnrecognizedTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ReturnTokenKeyword:
                ParseDefaultKeywords.HandleReturnTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.NamespaceTokenKeyword:
                ParseDefaultKeywords.HandleNamespaceTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.ClassTokenKeyword:
                ParseDefaultKeywords.HandleClassTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.InterfaceTokenKeyword:
                ParseDefaultKeywords.HandleInterfaceTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.UsingTokenKeyword:
                ParseDefaultKeywords.HandleUsingTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.PublicTokenKeyword:
                ParseDefaultKeywords.HandlePublicTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.InternalTokenKeyword:
                ParseDefaultKeywords.HandleInternalTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.PrivateTokenKeyword:
                ParseDefaultKeywords.HandlePrivateTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.StaticTokenKeyword:
                ParseDefaultKeywords.HandleStaticTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.OverrideTokenKeyword:
                ParseDefaultKeywords.HandleOverrideTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.VirtualTokenKeyword:
                ParseDefaultKeywords.HandleVirtualTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.AbstractTokenKeyword:
                ParseDefaultKeywords.HandleAbstractTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.SealedTokenKeyword:
                ParseDefaultKeywords.HandleSealedTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.IfTokenKeyword:
                ParseDefaultKeywords.HandleIfTokenKeyword(compilationUnit);
                break;
            case SyntaxKind.NewTokenKeyword:
                ParseDefaultKeywords.HandleNewTokenKeyword(compilationUnit);
                break;
            default:
                ParseDefaultKeywords.HandleDefault(compilationUnit);
                break;
        }
    }

    public static void ParseKeywordContextualToken(CSharpCompilationUnit compilationUnit)
    {
        switch (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind)
        {
            case SyntaxKind.VarTokenContextualKeyword:
                ParseContextualKeywords.HandleVarTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.PartialTokenContextualKeyword:
                ParseContextualKeywords.HandlePartialTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.AddTokenContextualKeyword:
                ParseContextualKeywords.HandleAddTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.AndTokenContextualKeyword:
                ParseContextualKeywords.HandleAndTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.AliasTokenContextualKeyword:
                ParseContextualKeywords.HandleAliasTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.AscendingTokenContextualKeyword:
                ParseContextualKeywords.HandleAscendingTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.ArgsTokenContextualKeyword:
                ParseContextualKeywords.HandleArgsTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.AsyncTokenContextualKeyword:
                ParseContextualKeywords.HandleAsyncTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.AwaitTokenContextualKeyword:
                ParseContextualKeywords.HandleAwaitTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.ByTokenContextualKeyword:
                ParseContextualKeywords.HandleByTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.DescendingTokenContextualKeyword:
                ParseContextualKeywords.HandleDescendingTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.DynamicTokenContextualKeyword:
                ParseContextualKeywords.HandleDynamicTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.EqualsTokenContextualKeyword:
                ParseContextualKeywords.HandleEqualsTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.FileTokenContextualKeyword:
                ParseContextualKeywords.HandleFileTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.FromTokenContextualKeyword:
                ParseContextualKeywords.HandleFromTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.GetTokenContextualKeyword:
                ParseContextualKeywords.HandleGetTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.GlobalTokenContextualKeyword:
                ParseContextualKeywords.HandleGlobalTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.GroupTokenContextualKeyword:
                ParseContextualKeywords.HandleGroupTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.InitTokenContextualKeyword:
                ParseContextualKeywords.HandleInitTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.IntoTokenContextualKeyword:
                ParseContextualKeywords.HandleIntoTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.JoinTokenContextualKeyword:
                ParseContextualKeywords.HandleJoinTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.LetTokenContextualKeyword:
                ParseContextualKeywords.HandleLetTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.ManagedTokenContextualKeyword:
                ParseContextualKeywords.HandleManagedTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.NameofTokenContextualKeyword:
                ParseContextualKeywords.HandleNameofTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.NintTokenContextualKeyword:
                ParseContextualKeywords.HandleNintTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.NotTokenContextualKeyword:
                ParseContextualKeywords.HandleNotTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.NotnullTokenContextualKeyword:
                ParseContextualKeywords.HandleNotnullTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.NuintTokenContextualKeyword:
                ParseContextualKeywords.HandleNuintTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.OnTokenContextualKeyword:
                ParseContextualKeywords.HandleOnTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.OrTokenContextualKeyword:
                ParseContextualKeywords.HandleOrTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.OrderbyTokenContextualKeyword:
                ParseContextualKeywords.HandleOrderbyTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.RecordTokenContextualKeyword:
                ParseContextualKeywords.HandleRecordTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.RemoveTokenContextualKeyword:
                ParseContextualKeywords.HandleRemoveTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.RequiredTokenContextualKeyword:
                ParseContextualKeywords.HandleRequiredTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.ScopedTokenContextualKeyword:
                ParseContextualKeywords.HandleScopedTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.SelectTokenContextualKeyword:
                ParseContextualKeywords.HandleSelectTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.SetTokenContextualKeyword:
                ParseContextualKeywords.HandleSetTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.UnmanagedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnmanagedTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.ValueTokenContextualKeyword:
                ParseContextualKeywords.HandleValueTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.WhenTokenContextualKeyword:
                ParseContextualKeywords.HandleWhenTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.WhereTokenContextualKeyword:
                ParseContextualKeywords.HandleWhereTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.WithTokenContextualKeyword:
                ParseContextualKeywords.HandleWithTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.YieldTokenContextualKeyword:
                ParseContextualKeywords.HandleYieldTokenContextualKeyword(compilationUnit);
                break;
            case SyntaxKind.UnrecognizedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnrecognizedTokenContextualKeyword(compilationUnit);
                break;
            default:
            	compilationUnit.ParserModel.DiagnosticBag.ReportTodoException(compilationUnit.ParserModel.TokenWalker.Current.TextSpan, $"Implement the {compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind.ToString()} contextual keyword.");
            	break;
        }
    }
}

/*
	(2024-11-08) The issue is:
	==========================
	'int[] x = new { 1, 2, 3, };'
	'int[] MyMethod() { }'
	
	In my mind this was an issue, but it wouldn't be an issue because in order
	to get to this code I have to see an OpenParenthesisToken.
	
	The variable assignment doesn't conflict at all.
	
	But:
	'int[] x = new { 1, 2, 3, };'
	'int[2] = 7;'
	
	At the start of a statement, there can be a conflict here,
	but given the situation, one can see the second line has a '2' in the array's square brackets
	so therefore you know the second line is a variable assignment expression.
	(the first line is a variable declaration statement / variable assignment expression).

	https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays
	=======================================================================================
		// Declare a single-dimensional array of 5 integers.
		int[] array1 = new int[5];
		
		// Declare and set array element values.
		int[] array2 = [1, 2, 3, 4, 5, 6];
		
		// Declare a two dimensional array.
		int[,] multiDimensionalArray1 = new int[2, 3];
		
		// Declare and set array element values.
		int[,] multiDimensionalArray2 = { { 1, 2, 3 }, { 4, 5, 6 } };
		
		// Declare a jagged array.
		int[][] jaggedArray = new int[6][];
		
		// Set the values of the first array in the jagged array structure.
		jaggedArray[0] = [1, 2, 3, 4];
	---------------------------------------------------------------------------------------
	
	None of the array declaration examples contain a number inside the
	array square brackets at the start of the statement.
	
	When dealing with a function definition,
	the TypeClauseNode for its return type might be an array.
	
	So I have to properly make sense of the 'int[]'
	if I have 'int[] MyMethod() { }'.
	
	But, I just saw that it seems I can check inside the array brackets
	whether there is a number or not.
	
	And there being a number means that it is an expression of some sort,
	whereas the lack or one means it is a statement of some sort.
	
	In the case of:
	'Person MyMethod() { }'
	
	When I read 'Person', is there any ambiguity as to whether it is
		- TypeClauseNode
		- IdentifierToken
	
	I think the question is:
	"At what points do I convert a token that can be a 'TypeClauseNode' to a 'TypeClauseNode'?"
	
	If the 'TypeClauseNode' is an array, then the array square brackets will not
	contain a number, and this is the sign that it is a 'TypeClauseNode'.
	
	If the 'TypeClauseNode' is just an IdentifierToken, where could things go from there?
	
	ConstructorDefinitionNode can occur from the following:
		- IdentifierToken, OpenParenthesisToken
	
	FunctionDefinitionNode can occur from the following:
		- TypeClauseNode, IdentifierToken, OpenParenthesisToken
			- This occurs when the first token initially was a 'IdentifierToken' but was not ambiguous and therefore
				immediately interpreted as a 'TypeClauseNode' rather than as an 'IdentifierToken'.
		- IdentifierToken, IdentifierToken, OpenParenthesisToken
		
	VariableDeclarationNode can occur from the following:
		- TypeClauseNode, IdentifierToken, EqualsToken
		
	TypeDefinitionNode can occur from the following:
		- SyntaxKind.ClassTokenKeyword, IdentifierToken
	
	TypeDefinitionNode can occur from the following:
		- SyntaxKind.ClassTokenKeyword, IdentifierToken
*/
