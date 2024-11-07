using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseDefaultKeywords
{
    public static void HandleAsTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleBaseTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleBoolTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleBreakTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleByteTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleCaseTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	model.ExpressionList.Add((SyntaxKind.ColonToken, null));
		var expressionNode = ParseOthers.ParseExpression(model);
	    var colonToken = (ColonToken)model.TokenWalker.Match(SyntaxKind.ColonToken);
    }

    public static void HandleCatchTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	var typeClause = model.TokenWalker.MatchTypeClauseNode(model);
    	
    	if (model.TokenWalker.Current.SyntaxKind != SyntaxKind.CloseParenthesisToken)
    		_ = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
    	
    	var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
    
    	TryStatementNode? tryStatementNode = null;
    
    	if (model.SyntaxStack.TryPeek(out var syntax) &&
    		syntax is TryStatementNode temporaryTryStatementNodeOne)
    	{
	        tryStatementNode = temporaryTryStatementNodeOne;
    	}
    	else if (model.SyntaxStack.TryPeek(out syntax) &&
    		syntax is TryStatementTryNode tryNode)
    	{
	        if (tryNode.Parent is TryStatementNode temporaryTryStatementNodeTwo)
	        {
	        	tryStatementNode = temporaryTryStatementNodeTwo;
	        }
    	}
    	
    	if (tryStatementNode is not null)
    	{
    		var catchNode = new TryStatementCatchNode(
	        	tryStatementNode,
		        consumedKeywordToken,
		        openParenthesisToken,
		        closeParenthesisToken,
		        codeBlockNode: null);
        
        	tryStatementNode.SetTryStatementCatchNode(catchNode);
        	model.SyntaxStack.Push(catchNode);
        	model.CurrentCodeBlockBuilder.PendingChild = catchNode;
    	}
    }

    public static void HandleCharTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleCheckedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleConstTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleContinueTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDecimalTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleDefaultTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	// Switch statement default case.
        if (model.TokenWalker.Next.SyntaxKind == SyntaxKind.ColonToken)
        	_ = model.TokenWalker.Consume();
    }

    public static void HandleDelegateTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleDoTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        var doWhileStatementNode = new DoWhileStatementNode(
	    	consumedKeywordToken,
	        openBraceToken: default,
	        codeBlockNode: null,
	        whileKeywordToken: default,
	        openParenthesisToken: default,
	        expressionNode: null,
	        closeParenthesisToken: default);
        	
        // Have to push twice so it is on the stack when the 'while' keyword is parsed.
		model.SyntaxStack.Push(doWhileStatementNode);
		model.SyntaxStack.Push(doWhileStatementNode);
        model.CurrentCodeBlockBuilder.PendingChild = doWhileStatementNode;
    }

    public static void HandleDoubleTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleElseTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleEnumTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);

        // Why was this method invocation here? (2024-01-23)
        //
        // HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleEventTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleExplicitTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleExternTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleFalseTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	var expressionNode = ParseOthers.ParseExpression(model);
    	model.SyntaxStack.Push(expressionNode);
    }

    public static void HandleFinallyTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	TryStatementNode? tryStatementNode = null;
    	
    	if (model.SyntaxStack.TryPeek(out var syntax) &&
    		syntax is TryStatementNode temporaryTryStatementNodeOne)
    	{
	        tryStatementNode = temporaryTryStatementNodeOne;
    	}
        else if (model.SyntaxStack.TryPeek(out syntax) &&
    		syntax is TryStatementTryNode tryNode)
    	{
	        if (tryNode.Parent is TryStatementNode temporaryTryStatementNodeTwo)
	        {
	        	tryStatementNode = temporaryTryStatementNodeTwo;
	        }
    	}
    	else if (model.SyntaxStack.TryPeek(out syntax) &&
    			 syntax is TryStatementCatchNode catchNode)
    	{
    		if (catchNode.Parent is TryStatementNode temporaryTryStatementNodeThree)
	        {
	        	tryStatementNode = temporaryTryStatementNodeThree;
	        }
    	}
    	
    	if (tryStatementNode is not null)
    	{
    		var finallyNode = new TryStatementFinallyNode(
    			tryStatementNode,
	        	consumedKeywordToken,
	        	codeBlockNode: null);
	    
	    	tryStatementNode.SetTryStatementFinallyNode(finallyNode);
	    	model.SyntaxStack.Push(finallyNode);
        	model.CurrentCodeBlockBuilder.PendingChild = finallyNode;
    	}
    }

    public static void HandleFixedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleFloatTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleForTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
        
        // Initialization Case One
        //     ;
        var initializationExpressionNode = (IExpressionNode)new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
        var initializationStatementDelimiterToken = (StatementDelimiterToken)model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        var badStateInitialization = false;
        
        if (initializationStatementDelimiterToken.IsFabricated)
        {
        	// Initialization Case Two
        	//     i = 0;
        	var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
        	
        	if (identifierToken.IsFabricated)
        	{
        		// Initialization Case Three
	    		//     int i = 0;
	        	var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);
	        	var isCaseThree = !typeClauseNode.IsFabricated;
	        	
	        	if (isCaseThree)
	        	{
	        		identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
	        	}
	        	else
	        	{
	        		// Initialization Case Four
	        		//     bad syntax?
	        		badStateInitialization = true;
	        	}
        	}
        	
        	if (!badStateInitialization)
        	{
        		// Read the remainder
        		//     = 0;
        		var equalsToken = (EqualsToken)model.TokenWalker.Match(SyntaxKind.EqualsToken);
        		
        		model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        		initializationExpressionNode = ParseOthers.ParseExpression(model);
			    
			    initializationStatementDelimiterToken = (StatementDelimiterToken)model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        	}
        }
        
        // Condition Case One
    	//     ;
    	var conditionExpressionNode = (IExpressionNode)new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
        var conditionStatementDelimiterToken = (StatementDelimiterToken)model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        
        if (conditionStatementDelimiterToken.IsFabricated)
        {
        	// Condition Case Two
        	//     i < 10;
        
        	model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        	conditionExpressionNode = ParseOthers.ParseExpression(model);
		    
		    conditionStatementDelimiterToken = (StatementDelimiterToken)model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        }
        
        // Updation Case One
        //    )
        var updationExpressionNode = (IExpressionNode)new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
        var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
        
        if (closeParenthesisToken.IsFabricated)
        {
        	model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        	updationExpressionNode = ParseOthers.ParseExpression(model);
		    
		    closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		    
		    if (closeParenthesisToken.IsFabricated)
		    {
		    	while (!model.TokenWalker.IsEof)
		    	{
		    		if (model.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseParenthesisToken)
		    			break;
		    		
		    		_ = model.TokenWalker.Consume();
		    	}
		    }
        }
		
		var forStatementNode = new ForStatementNode(
	        consumedKeywordToken,
	        openParenthesisToken,
	        ImmutableArray<ISyntax>.Empty,
	        initializationStatementDelimiterToken,
	        conditionExpressionNode,
	        conditionStatementDelimiterToken,
	        updationExpressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        model.SyntaxStack.Push(forStatementNode);
        model.CurrentCodeBlockBuilder.PendingChild = forStatementNode;
    }

    public static void HandleForeachTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);
    	var variableIdentifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
    	
    	var variableDeclarationStatementNode = new VariableDeclarationNode(
            typeClauseNode,
            variableIdentifierToken,
            VariableKind.Local,
            false);
    	
    	var inKeywordToken = (KeywordToken)model.TokenWalker.Match(SyntaxKind.InTokenKeyword);
    	
    	model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(model);
		var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var foreachStatementNode = new ForeachStatementNode(
	        consumedKeywordToken,
	        openParenthesisToken,
	        variableDeclarationStatementNode,
	        inKeywordToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        model.SyntaxStack.Push(foreachStatementNode);
        model.CurrentCodeBlockBuilder.PendingChild = foreachStatementNode;
    }

    public static void HandleGotoTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleImplicitTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleInTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleIntTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleIsTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleLockTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(model);
		
		var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var lockStatementNode = new LockStatementNode(
			consumedKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        model.SyntaxStack.Push(lockStatementNode);
        model.CurrentCodeBlockBuilder.PendingChild = lockStatementNode;
    }

    public static void HandleLongTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleNullTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleObjectTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleOperatorTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleOutTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleParamsTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleProtectedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandleReadonlyTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleRefTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleSbyteTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleShortTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleSizeofTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleStackallocTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleStringTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleStructTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);
    }

    public static void HandleSwitchTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(model);
		
		var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var switchStatementNode = new SwitchStatementNode(
			consumedKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        model.SyntaxStack.Push(switchStatementNode);
        model.CurrentCodeBlockBuilder.PendingChild = switchStatementNode;
    }

    public static void HandleThisTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleThrowTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleTrueTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	var expressionNode = ParseOthers.ParseExpression(model);
    	model.SyntaxStack.Push(expressionNode);
    }

    public static void HandleTryTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	var tryStatementNode = new TryStatementNode(
			tryNode: null,
	        catchNode: null,
	        finallyNode: null);
    
	    var tryStatementTryNode = new TryStatementTryNode(
	    	tryStatementNode,
        	consumedKeywordToken,
        	codeBlockNode: null);
        	
		tryStatementNode.SetTryStatementTryNode(tryStatementTryNode);
	        
	    model.CurrentCodeBlockBuilder.ChildList.Add(tryStatementNode);
	        
		model.SyntaxStack.Push(tryStatementNode);
		
		model.SyntaxStack.Push(tryStatementTryNode);
        model.CurrentCodeBlockBuilder.PendingChild = tryStatementTryNode;
    }

    public static void HandleTypeofTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUintTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleUlongTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleUncheckedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUnsafeTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUshortTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleVoidTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleVolatileTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleWhileTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {    
        var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        var expressionNode = ParseOthers.ParseExpression(model);
		
		var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		if (model.SyntaxStack.TryPeek(out var syntax) &&
    		syntax is DoWhileStatementNode doWhileStatementNode)
    	{
	        doWhileStatementNode.SetWhileProperties(
		    	consumedKeywordToken,
		        openParenthesisToken,
		        expressionNode,
		        closeParenthesisToken);
    	}
		else
		{
			var whileStatementNode = new WhileStatementNode(
				consumedKeywordToken,
		        openParenthesisToken,
		        expressionNode,
		        closeParenthesisToken,
		        codeBlockNode: null);
		        
	        model.SyntaxStack.Push(whileStatementNode);
        	model.CurrentCodeBlockBuilder.PendingChild = whileStatementNode;
		}
    }

    public static void HandleUnrecognizedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDefault(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        if (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(consumedKeywordToken.SyntaxKind))
        {
            var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);
            model.SyntaxStack.Push(typeClauseNode);
        }
        else
        {
        	model.DiagnosticBag.ReportTodoException(consumedKeywordToken.TextSpan, $"Implement the {consumedKeywordToken.SyntaxKind} keyword.");
        }
    }

    public static void HandleTypeIdentifierKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {

        if (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(consumedKeywordToken.SyntaxKind))
        {
            var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

            if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.AttributeNode)
                typeClauseNode.AttributeNode = (AttributeNode)model.SyntaxStack.Pop();

            model.SyntaxStack.Push(typeClauseNode);
        }
        else
        {
        	model.DiagnosticBag.ReportTodoException(consumedKeywordToken.TextSpan, $"Implement the {consumedKeywordToken.SyntaxKind} keyword.");
        }
    }

    public static void HandleNewTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	var constructorInvocationNode = ParseOthers.ParseExpression(model);
    	model.SyntaxStack.Push(constructorInvocationNode);
    
		// "explicit namespace qualification" OR "nested class"
		// if (model.TokenWalker.Peek(0).SyntaxKind == SyntaxKind.MemberAccessToken)
    }

    public static void HandlePublicTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandleInternalTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandlePrivateTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandleStaticTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleOverrideTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleVirtualTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleAbstractTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleSealedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleIfTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        var openParenthesisToken = model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);

        if (openParenthesisToken.IsFabricated)
            return;

		model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		var expression = ParseOthers.ParseExpression(model);

        var boundIfStatementNode = model.Binder.BindIfStatementNode(consumedKeywordToken, expression);
        model.SyntaxStack.Push(boundIfStatementNode);
        model.CurrentCodeBlockBuilder.PendingChild = boundIfStatementNode;
    }

    public static void HandleUsingTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        ParseOthers.HandleNamespaceIdentifier(model);

        var handleNamespaceIdentifierResult = model.SyntaxStack.Pop();

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            model.DiagnosticBag.ReportTodoException(consumedKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        var boundUsingStatementNode = model.Binder.BindUsingStatementNode(
            consumedKeywordToken,
            namespaceIdentifier,
            model);

        model.CurrentCodeBlockBuilder.ChildList.Add(boundUsingStatementNode);
        model.SyntaxStack.Push(boundUsingStatementNode);
    }

    public static void HandleInterfaceTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        ParseDefaultKeywords.HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);
    }

	/// <summary>
	/// Example:
	/// public class MyClass { }
	///              ^
	///
	/// Given the example the 'MyClass' is the current token
	/// upon invocation of this method.
	///
	/// Invocation of this method implies the previous token was
	/// class, interface, struct, etc...
	///
	/// The syntax token parameter to this method is said
	/// previous token.
	/// </summary>
    public static void HandleStorageModifierTokenKeyword(
        ISyntaxToken consumedStorageModifierToken,
        CSharpParserModel model)
    {
    	// Given: public partial class MyClass { }
		// Then: partial
        var hasPartialModifier = false;
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax is ISyntaxToken syntaxToken)
        {
            if (syntaxToken.SyntaxKind == SyntaxKind.PartialTokenContextualKeyword)
            {
                _ = model.SyntaxStack.Pop();
                hasPartialModifier = true;
            }
        }
    
    	// TODO: Fix; the code that parses the accessModifierKind is a mess
		//
		// Given: public class MyClass { }
		// Then: public
		var accessModifierKind = AccessModifierKind.Public;
        if (model.SyntaxStack.TryPeek(out syntax) && syntax is ISyntaxToken firstSyntaxToken)
        {
            var firstOutput = UtilityApi.GetAccessModifierKindFromToken(firstSyntaxToken);

            if (firstOutput is not null)
            {
                _ = model.SyntaxStack.Pop();
                accessModifierKind = firstOutput.Value;

				// Given: protected internal class MyClass { }
				// Then: protected internal
                if (model.SyntaxStack.TryPeek(out syntax) && syntax is ISyntaxToken secondSyntaxToken)
                {
                    var secondOutput = UtilityApi.GetAccessModifierKindFromToken(secondSyntaxToken);

                    if (secondOutput is not null)
                    {
                        _ = model.SyntaxStack.Pop();

                        if ((firstOutput.Value.ToString().ToLower() == "protected" &&
                                secondOutput.Value.ToString().ToLower() == "internal") ||
                            (firstOutput.Value.ToString().ToLower() == "internal" &&
                                secondOutput.Value.ToString().ToLower() == "protected"))
                        {
                            accessModifierKind = AccessModifierKind.ProtectedInternal;
                        }
                        else if ((firstOutput.Value.ToString().ToLower() == "private" &&
                                    secondOutput.Value.ToString().ToLower() == "protected") ||
                                (firstOutput.Value.ToString().ToLower() == "protected" &&
                                    secondOutput.Value.ToString().ToLower() == "private"))
                        {
                            accessModifierKind = AccessModifierKind.PrivateProtected;
                        }
                        // else use the firstOutput.
                    }
                }
            }
        }
    
    	// TODO: Fix nullability spaghetti code
        var storageModifierKind = UtilityApi.GetStorageModifierKindFromToken(consumedStorageModifierToken);
        if (storageModifierKind is null)
            return;
        if (storageModifierKind == StorageModifierKind.Record &&
        	model.TokenWalker.Current.SyntaxKind == SyntaxKind.StructTokenKeyword)
        {
        	var structKeywordToken = (KeywordToken)model.TokenWalker.Consume();
        	storageModifierKind = StorageModifierKind.RecordStruct;
        }
    
		// Given: public class MyClass<T> { }
		// Then: MyClass
		IdentifierToken identifierToken;
		// Retrospective: What is the purpose of this 'if (contextualKeyword) logic'?
		// Response: maybe it is because 'var' contextual keyword is allowed to be a class name?
        if (UtilityApi.IsContextualKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind))
        {
            var contextualKeywordToken = (KeywordContextualToken)model.TokenWalker.Consume();
            // Take the contextual keyword as an identifier
            identifierToken = new IdentifierToken(contextualKeywordToken.TextSpan);
        }
        else
        {
            identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

		// Given: public class MyClass<T> { }
		// Then: <T>
        GenericArgumentsListingNode? genericArgumentsListingNode = null;
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            ParseTypes.HandleGenericArguments(
                (OpenAngleBracketToken)model.TokenWalker.Consume(),
                model);

            genericArgumentsListingNode = (GenericArgumentsListingNode?)model.SyntaxStack.Pop();
        }

        var typeDefinitionNode = new TypeDefinitionNode(
            accessModifierKind,
            hasPartialModifier,
            storageModifierKind.Value,
            identifierToken,
            valueType: null,
            genericArgumentsListingNode,
            primaryConstructorFunctionArgumentsListingNode: null,
            inheritedTypeClauseNode: null,
            openBraceToken: default,
            codeBlockNode: null);

        model.Binder.BindTypeDefinitionNode(typeDefinitionNode, model);
        model.Binder.BindTypeIdentifier(identifierToken, model);
        model.SyntaxStack.Push(typeDefinitionNode);
        model.CurrentCodeBlockBuilder.PendingChild = typeDefinitionNode;
        
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.WhereTokenContextualKeyword)
        {
        	while (!model.TokenWalker.IsEof)
        	{
        		if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
        			model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
        		{
        			break;
        		}
        		
        		_ = model.TokenWalker.Consume();
        	}
        }
    }

    public static void HandleClassTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);
    }

    public static void HandleNamespaceTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        ParseOthers.HandleNamespaceIdentifier(model);

        var handleNamespaceIdentifierResult = model.SyntaxStack.Pop();

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            model.DiagnosticBag.ReportTodoException(consumedKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        if (model.FinalizeNamespaceFileScopeCodeBlockNodeAction is not null)
            model.DiagnosticBag.ReportTodoException(consumedKeywordToken.TextSpan, "Need to add logic to report diagnostic when there is already a file scoped namespace.");

        var namespaceStatementNode = new NamespaceStatementNode(
            consumedKeywordToken,
            namespaceIdentifier,
            new CodeBlockNode(ImmutableArray<ISyntax>.Empty));

        model.Binder.SetCurrentNamespaceStatementNode(namespaceStatementNode, model);
        
        model.SyntaxStack.Push(namespaceStatementNode);
        model.CurrentCodeBlockBuilder.PendingChild = namespaceStatementNode;
    }

    public static void HandleReturnTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	var returnExpression = ParseOthers.ParseExpression(model);

        var returnStatementNode = model.Binder.BindReturnStatementNode(
            consumedKeywordToken,
            returnExpression);

        model.CurrentCodeBlockBuilder.ChildList.Add(returnStatementNode);
        model.SyntaxStack.Push(returnStatementNode);
    }
}
