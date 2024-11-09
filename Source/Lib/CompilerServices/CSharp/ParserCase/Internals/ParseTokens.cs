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
    public static void ParsePreprocessorDirectiveToken(
        PreprocessorDirectiveToken consumedPreprocessorDirectiveToken,
        CSharpParserModel model)
    {
        var consumedToken = model.TokenWalker.Consume();
    }

    public static void ParseIdentifierTokenWithPeek(CSharpParserModel model)
    {
    	ParseOthers.Force_ParseExpression(SyntaxKind.TypeClauseNode, model);
    	// ParseOthers.StartStatement_Expression(model);
    }
    
    public static void ParseIdentifierToken(CSharpParserModel model)
    {
    	IdentifierToken identifierToken;
    	
    	if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.VarTokenContextualKeyword)
    	{
    		var varTokenContextualKeyword = model.TokenWalker.Consume();
    		identifierToken = new IdentifierToken(varTokenContextualKeyword.TextSpan);
    	}
    	else
    	{
    		identifierToken = (IdentifierToken)model.TokenWalker.Consume();
    	}
    	
    	if (model.StatementBuilder.TryPeek(^1, out var oneTokenBackwards) &&
			UtilityApi.IsConvertibleToTypeClauseNode(oneTokenBackwards.SyntaxKind))
    	{
    		if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
    			model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
	    	{
	    		// One only ends up at this code via the main loop,
	    		// an expression would never see this code, so we know it is a declaration.
	    		var variableDeclarationNode = ParseVariables.HandleVariableDeclarationExpression(
					UtilityApi.ConvertToTypeClauseNode(oneTokenBackwards),
			        identifierToken,
			        VariableKind.Local,
			        model);
	    		
	    		//ParseVariables.HandleVariableDeclarationStatement(
	    		//	UtilityApi.ConvertToTypeClauseNode(oneTokenBackwards),
	    		//	identifierToken,
	    		//	VariableKind.Local,
	    		//	model);
	    		
	    		if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
		    	{
		    		var equalsToken = (EqualsToken)model.TokenWalker.Consume();
		    		
		    		model.StatementBuilder.ChildList.Clear();
		    		model.StatementBuilder.ChildList.Add(identifierToken);
		    		
		    		var expressionNode = ParseOthers.ParseExpression(model);
		    		
		    		var variableAssignmentExpressionNode = new VariableAssignmentExpressionNode(
		    			identifierToken,
				        equalsToken,
				        expressionNode);
		    		
		    		model.CurrentCodeBlockBuilder.ChildList.Add(variableAssignmentExpressionNode);
		    	}
	    	}
    	}
    	else
    	{
    		model.StatementBuilder.ChildList.Add(identifierToken);
    	}
    }

    public static void ParseColonToken(CSharpParserModel model)
    {
    	var colonToken = (ColonToken)model.TokenWalker.Consume();
    
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
        {
            var typeDefinitionNode = (TypeDefinitionNode)model.SyntaxStack.Pop();
            var inheritedTypeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

            model.Binder.BindTypeClauseNode(inheritedTypeClauseNode, model);

			typeDefinitionNode.SetInheritedTypeClauseNode(inheritedTypeClauseNode);

            model.SyntaxStack.Push(typeDefinitionNode);
            model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = typeDefinitionNode;
        }
        else
        {
            model.DiagnosticBag.ReportTodoException(colonToken.TextSpan, "Colon is in unexpected place.");
        }
    }

	/// <summary>
	/// OpenBraceToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseOpenBraceToken(OpenBraceToken openBraceToken, CSharpParserModel model)
    {    
		if (model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner is null)
		{
			var arbitraryCodeBlockNode = new ArbitraryCodeBlockNode(model.CurrentCodeBlockBuilder.CodeBlockOwner);
			model.SyntaxStack.Push(arbitraryCodeBlockNode);
        	model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = arbitraryCodeBlockNode;
		}
		
		model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner.SetOpenBraceToken(openBraceToken);

		var parentScopeDirection = model.CurrentCodeBlockBuilder?.CodeBlockOwner?.ScopeDirectionKind ?? ScopeDirectionKind.Both;
		if (parentScopeDirection == ScopeDirectionKind.Both)
		{
			if (!model.CurrentCodeBlockBuilder.PermitInnerPendingCodeBlockOwnerToBeParsed)
			{
				model.TokenWalker.DeferParsingOfChildScope(openBraceToken, model);
				return;
			}

			model.CurrentCodeBlockBuilder.PermitInnerPendingCodeBlockOwnerToBeParsed = false;
		}

		var nextCodeBlockOwner = model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner;
		var nextReturnTypeClauseNode = nextCodeBlockOwner.GetReturnTypeClauseNode();

        model.Binder.OpenScope(nextReturnTypeClauseNode, openBraceToken.TextSpan, model);
		model.CurrentCodeBlockBuilder = new(parent: model.CurrentCodeBlockBuilder, codeBlockOwner: nextCodeBlockOwner);
		nextCodeBlockOwner.OnBoundScopeCreatedAndSetAsCurrent(model);
    }

	/// <summary>
	/// CloseBraceToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseCloseBraceToken(CloseBraceToken closeBraceToken, CSharpParserModel model)
    {
		if (model.CurrentCodeBlockBuilder.ParseChildScopeQueue.TryDequeue(out var deferredChildScope))
		{
			deferredChildScope.PrepareMainParserLoop(model.TokenWalker.Index - 1, model);
			return;
		}

		if (model.CurrentCodeBlockBuilder.CodeBlockOwner is not null)
			model.CurrentCodeBlockBuilder.CodeBlockOwner.SetCloseBraceToken(closeBraceToken);
		
        model.Binder.CloseScope(closeBraceToken.TextSpan, model);
    }

    public static void ParseOpenParenthesisToken(CSharpParserModel model)
    {
    	if (TryHandleFunctionDefinition(model))
    		return;
    	
    	if (TryHandleConstructorDefinition(model))
    		return;
    }
    
    /// <summary>
    /// FunctionDefinitionNode can occur from the following:
	///	- TypeClauseNode, IdentifierToken, OpenParenthesisToken
	///		- This occurs when the first token initially was a 'IdentifierToken' but was not ambiguous and therefore
	///			immediately interpreted as a 'TypeClauseNode' rather than as an 'IdentifierToken'.
	///	- IdentifierToken, IdentifierToken, OpenParenthesisToken
	///	- "IsConvertibleToTypeClauseNode", "IsConvertibleToIdentifierToken", OpenParenthesisToken
	///		- Some keywords are "IsConvertibleToTypeClauseNode".
	///		- As well, a KeywordContextualToken might be convertable to either a TypeClauseNode or an IdentifierToken;
    /// </summary>
    public static bool TryHandleFunctionDefinition(CSharpParserModel model)
    {
    	bool isFunctionDefinition = true;
    	
    	// These are ordered backwards (i.e. from the current token's position traveling backwards).
    	GenericArgumentsListingNode? genericArgumentsListingNode = null;
    	IdentifierToken identifierToken = default;
    	TypeClauseNode? typeClauseNode = null;
    	
		if (model.StatementBuilder.TryPeek(^1, out var existingGenericArgumentsListingNode) &&
			existingGenericArgumentsListingNode.SyntaxKind == SyntaxKind.GenericArgumentsListingNode)
		{
			// public void CreateBox<TItem>() { }
			isFunctionDefinition = model.StatementBuilder.TryPeek(^2, out var twoTokenBackwards) &&
								   UtilityApi.IsConvertibleToIdentifierToken(twoTokenBackwards.SyntaxKind);
			
			if (isFunctionDefinition)
			{
				isFunctionDefinition = model.StatementBuilder.TryPeek(^3, out var threeTokenBackwards) &&
								   	UtilityApi.IsConvertibleToTypeClauseNode(threeTokenBackwards.SyntaxKind);
						  
				if (isFunctionDefinition)
				{
					genericArgumentsListingNode = (GenericArgumentsListingNode)existingGenericArgumentsListingNode;
					identifierToken = UtilityApi.ConvertToIdentifierToken(twoTokenBackwards);
					typeClauseNode = UtilityApi.ConvertToTypeClauseNode(threeTokenBackwards);
				}
			}
		}
		else
		{
			// public void CreateBox() { }
			isFunctionDefinition = model.StatementBuilder.TryPeek(^1, out var oneTokenBackwards) &&
								   UtilityApi.IsConvertibleToIdentifierToken(oneTokenBackwards.SyntaxKind);
			
			if (isFunctionDefinition)
			{
				isFunctionDefinition = model.StatementBuilder.TryPeek(^2, out var twoTokenBackwards) &&
								   	UtilityApi.IsConvertibleToTypeClauseNode(twoTokenBackwards.SyntaxKind);
						  
				if (isFunctionDefinition)
				{
					identifierToken = UtilityApi.ConvertToIdentifierToken(oneTokenBackwards);
					typeClauseNode = UtilityApi.ConvertToTypeClauseNode(twoTokenBackwards);
				}
			}
		}
    	
    	if (isFunctionDefinition)
    	{
	    	ParseFunctions.HandleFunctionDefinition(
	    		identifierToken,
	        	typeClauseNode,
	        	genericArgumentsListingNode,
	        	model);
        }
        
        return isFunctionDefinition;
    }
    
    public static bool TryHandleConstructorDefinition(CSharpParserModel model)
    {
    	bool isConstructorDefinition = true;
    	
		// These are ordered backwards (i.e. from the current token's position traveling backwards).
		IdentifierToken identifierToken = default;
	
		/*
		public class Person
		{
			public Person()
			{
			}
		}
		*/
		isConstructorDefinition = model.StatementBuilder.TryPeek(^1, out var oneTokenBackwards) &&
				  				UtilityApi.IsConvertibleToIdentifierToken(oneTokenBackwards.SyntaxKind);
		
		if (isConstructorDefinition)
			identifierToken = UtilityApi.ConvertToIdentifierToken(oneTokenBackwards);
    	
    	if (isConstructorDefinition)
    	{
	    	ParseFunctions.HandleConstructorDefinition(
	    		identifierToken,
	        	model);
        }
        
        return isConstructorDefinition;
    }

    public static void ParseCloseParenthesisToken(
        CloseParenthesisToken consumedCloseParenthesisToken,
        CSharpParserModel model)
    {
    	var closesParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Consume();
    }

    public static void ParseOpenAngleBracketToken(
        OpenAngleBracketToken consumedOpenAngleBracketToken,
        CSharpParserModel model)
    {
    }

    public static void ParseCloseAngleBracketToken(
        CloseAngleBracketToken consumedCloseAngleBracketToken,
        CSharpParserModel model)
    {
    }

    public static void ParseOpenSquareBracketToken(
        OpenSquareBracketToken consumedOpenSquareBracketToken,
        CSharpParserModel model)
    {
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
    }

    public static void ParseMemberAccessToken(
        MemberAccessToken consumedMemberAccessToken,
        CSharpParserModel model)
    {
    }

	/// <summary>
	/// StatementDelimiterToken is passed in to the method because it is a protected token,
	/// and is preferably consumed from the main loop so it can be more easily tracked.
	/// </summary>
    public static void ParseStatementDelimiterToken(StatementDelimiterToken statementDelimiterToken, CSharpParserModel model)
    {
    	if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.NamespaceStatementNode)
        {
        	var closureCurrentCompilationUnitBuilder = model.CurrentCodeBlockBuilder;
            ICodeBlockOwner? nextCodeBlockOwner = null;
            TypeClauseNode? scopeReturnTypeClauseNode = null;

            var namespaceStatementNode = (NamespaceStatementNode)model.SyntaxStack.Pop();
            nextCodeBlockOwner = namespaceStatementNode;
            
            namespaceStatementNode.SetStatementDelimiterToken(statementDelimiterToken);

            model.Binder.OpenScope(
                scopeReturnTypeClauseNode,
                statementDelimiterToken.TextSpan,
                model);

            model.Binder.AddNamespaceToCurrentScope(
                namespaceStatementNode.IdentifierToken.TextSpan.GetText(),
                model);

            model.CurrentCodeBlockBuilder = new(model.CurrentCodeBlockBuilder, nextCodeBlockOwner);
        }
        else if (model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner is not null)
        {
        	var pendingChild = model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner;
        
        	model.Binder.OpenScope(CSharpFacts.Types.Void.ToTypeClause(), statementDelimiterToken.TextSpan, model);
			model.CurrentCodeBlockBuilder = new(model.CurrentCodeBlockBuilder, pendingChild);
			pendingChild.OnBoundScopeCreatedAndSetAsCurrent(model);
			
	        model.Binder.CloseScope(statementDelimiterToken.TextSpan, model);
	
	        if (model.CurrentCodeBlockBuilder.Parent is not null)
	            model.CurrentCodeBlockBuilder = model.CurrentCodeBlockBuilder.Parent;
	            
	        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = null;
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
