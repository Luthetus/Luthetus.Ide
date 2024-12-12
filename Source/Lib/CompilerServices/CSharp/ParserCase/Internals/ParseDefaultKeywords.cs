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
    public static void HandleAsTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleBaseTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleBoolTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleBreakTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleByteTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleCaseTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.ExpressionList.Add((SyntaxKind.ColonToken, null));
		var expressionNode = ParseOthers.ParseExpression(model);
	    var colonToken = (ColonToken)model.TokenWalker.Match(SyntaxKind.ColonToken);
    }

    public static void HandleCatchTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var catchKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
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
		        catchKeywordToken,
		        openParenthesisToken,
		        closeParenthesisToken,
		        codeBlockNode: null);
        
        	tryStatementNode.SetTryStatementCatchNode(catchNode);
        	model.SyntaxStack.Push(catchNode);
        	model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = catchNode;
    	}
    }

    public static void HandleCharTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleCheckedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleConstTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleContinueTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleDecimalTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleDefaultTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	// Switch statement default case.
        if (model.TokenWalker.Next.SyntaxKind == SyntaxKind.ColonToken)
        	_ = model.TokenWalker.Consume();
		else
			model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleDelegateTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleDoTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var doKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
    	var doWhileStatementNode = new DoWhileStatementNode(
	    	doKeywordToken,
	        openBraceToken: default,
	        codeBlockNode: null,
	        whileKeywordToken: default,
	        openParenthesisToken: default,
	        expressionNode: null,
	        closeParenthesisToken: default);
        	
        // Have to push twice so it is on the stack when the 'while' keyword is parsed.
		model.SyntaxStack.Push(doWhileStatementNode);
		model.SyntaxStack.Push(doWhileStatementNode);
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = doWhileStatementNode;
    }

    public static void HandleDoubleTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleElseTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleEnumTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleStorageModifierTokenKeyword(model);

        // Why was this method invocation here? (2024-01-23)
        //
        // HandleTypeIdentifierKeyword(model);
    }

    public static void HandleEventTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleExplicitTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleExternTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleFalseTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var expressionNode = ParseOthers.ParseExpression(model);
    	model.StatementBuilder.ChildList.Add(expressionNode);
    }

    public static void HandleFinallyTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var finallyKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
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
	        	finallyKeywordToken,
	        	codeBlockNode: null);
	    
	    	tryStatementNode.SetTryStatementFinallyNode(finallyNode);
	    	model.SyntaxStack.Push(finallyNode);
        	model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = finallyNode;
    	}
    }

    public static void HandleFixedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleFloatTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleForTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var forKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
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
		    		if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
		    			break;
		    		
		    		_ = model.TokenWalker.Consume();
		    	}
		    }
        }
		
		var forStatementNode = new ForStatementNode(
	        forKeywordToken,
	        openParenthesisToken,
	        ImmutableArray<ISyntax>.Empty,
	        initializationStatementDelimiterToken,
	        conditionExpressionNode,
	        conditionStatementDelimiterToken,
	        updationExpressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        model.SyntaxStack.Push(forStatementNode);
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = forStatementNode;
    }

    public static void HandleForeachTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var foreachKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
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
	        foreachKeywordToken,
	        openParenthesisToken,
	        variableDeclarationStatementNode,
	        inKeywordToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        model.SyntaxStack.Push(foreachStatementNode);
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = foreachStatementNode;
    }

    public static void HandleGotoTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleImplicitTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleInTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleIntTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleIsTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleLockTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var lockKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(model);
		
		var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var lockStatementNode = new LockStatementNode(
			lockKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        model.SyntaxStack.Push(lockStatementNode);
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = lockStatementNode;
    }

    public static void HandleLongTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleNullTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleObjectTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleOperatorTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleOutTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleParamsTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleProtectedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var protectedTokenKeyword = (KeywordToken)model.TokenWalker.Consume();
        model.StatementBuilder.ChildList.Add(protectedTokenKeyword);
    }

    public static void HandleReadonlyTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleRefTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleSbyteTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleShortTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleSizeofTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleStackallocTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleStringTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleStructTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleStorageModifierTokenKeyword(model);
    }

    public static void HandleSwitchTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var switchKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(model);
		
		var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var switchStatementNode = new SwitchStatementNode(
			switchKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        model.SyntaxStack.Push(switchStatementNode);
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = switchStatementNode;
    }

    public static void HandleThisTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleThrowTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleTrueTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var expressionNode = ParseOthers.ParseExpression(model);
    	model.StatementBuilder.ChildList.Add(expressionNode);
    }

    public static void HandleTryTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var tryKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
    	var tryStatementNode = new TryStatementNode(
			tryNode: null,
	        catchNode: null,
	        finallyNode: null);
    
	    var tryStatementTryNode = new TryStatementTryNode(
	    	tryStatementNode,
        	tryKeywordToken,
        	codeBlockNode: null);
        	
		tryStatementNode.SetTryStatementTryNode(tryStatementTryNode);
	        
	    model.CurrentCodeBlockBuilder.ChildList.Add(tryStatementNode);
	        
		model.SyntaxStack.Push(tryStatementNode);
		
		model.SyntaxStack.Push(tryStatementTryNode);
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = tryStatementTryNode;
    }

    public static void HandleTypeofTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleUintTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleUlongTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleUncheckedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleUnsafeTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleUshortTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleVoidTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleVolatileTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleWhileTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var whileKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        var expressionNode = ParseOthers.ParseExpression(model);
		
		var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		if (model.SyntaxStack.TryPeek(out var syntax) &&
    		syntax is DoWhileStatementNode doWhileStatementNode)
    	{
	        doWhileStatementNode.SetWhileProperties(
		    	whileKeywordToken,
		        openParenthesisToken,
		        expressionNode,
		        closeParenthesisToken);
    	}
		else
		{
			var whileStatementNode = new WhileStatementNode(
				whileKeywordToken,
		        openParenthesisToken,
		        expressionNode,
		        closeParenthesisToken,
		        codeBlockNode: null);
		        
	        model.SyntaxStack.Push(whileStatementNode);
        	model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = whileStatementNode;
		}
    }

    public static void HandleUnrecognizedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

	/// <summary>The 'Default' of this method name is confusing.
	/// It seems to refer to the 'default' of switch statement rather than the 'default' keyword itself?
	/// </summary>
    public static void HandleDefault(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleTypeIdentifierKeyword(CSharpCompilationUnit compilationUnit)
    {
    	ParseTokens.ParseIdentifierToken(model);
    }

    public static void HandleNewTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	if (model.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
    		UtilityApi.IsConvertibleToIdentifierToken(model.TokenWalker.Next.SyntaxKind))
    	{
    		var expressionNode = ParseOthers.ParseExpression(model);
    		model.StatementBuilder.ChildList.Add(expressionNode);
    	}
    	else
    	{
    		model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    	}
    }

    public static void HandlePublicTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var publicKeywordToken = (KeywordToken)model.TokenWalker.Consume();
        model.StatementBuilder.ChildList.Add(publicKeywordToken);
    }

    public static void HandleInternalTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var internalTokenKeyword = (KeywordToken)model.TokenWalker.Consume();
        model.StatementBuilder.ChildList.Add(internalTokenKeyword);
    }

    public static void HandlePrivateTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var privateTokenKeyword = (KeywordToken)model.TokenWalker.Consume();
        model.StatementBuilder.ChildList.Add(privateTokenKeyword);
    }

    public static void HandleStaticTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleOverrideTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleVirtualTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleAbstractTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleSealedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }

    public static void HandleIfTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var ifTokenKeyword = (KeywordToken)model.TokenWalker.Consume();
    	
    	var openParenthesisToken = model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);

        if (openParenthesisToken.IsFabricated)
            return;

		model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		var expression = ParseOthers.ParseExpression(model);

        var boundIfStatementNode = model.Binder.BindIfStatementNode(ifTokenKeyword, expression);
        model.SyntaxStack.Push(boundIfStatementNode);
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = boundIfStatementNode;
    }

    public static void HandleUsingTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var usingKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
    	var handleNamespaceIdentifierResult = ParseOthers.HandleNamespaceIdentifier(model);

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            model.DiagnosticBag.ReportTodoException(usingKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        var usingStatementNode = new UsingStatementNode(
            usingKeywordToken,
            namespaceIdentifier);

        model.Binder.BindUsingStatementNode(usingStatementNode, model);
        model.StatementBuilder.ChildList.Add(usingStatementNode);
    }

    public static void HandleInterfaceTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	HandleStorageModifierTokenKeyword(model);
    }

	/// <summary>
	/// Example:
	/// public class MyClass { }
	///              ^
	///
	/// Given the example the 'MyClass' is the next token
	/// upon invocation of this method.
	///
	/// Invocation of this method implies the current token was
	/// class, interface, struct, etc...
	/// </summary>
    public static void HandleStorageModifierTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var storageModifierToken = model.TokenWalker.Consume();
    	
    	// Given: public partial class MyClass { }
		// Then: partial
        var hasPartialModifier = false;
        if (model.StatementBuilder.TryPeek(out var syntax) && syntax is ISyntaxToken syntaxToken)
        {
            if (syntaxToken.SyntaxKind == SyntaxKind.PartialTokenContextualKeyword)
            {
                _ = model.StatementBuilder.Pop();
                hasPartialModifier = true;
            }
        }
    
    	// TODO: Fix; the code that parses the accessModifierKind is a mess
		//
		// Given: public class MyClass { }
		// Then: public
		var accessModifierKind = AccessModifierKind.Public;
        if (model.StatementBuilder.TryPeek(out syntax) && syntax is ISyntaxToken firstSyntaxToken)
        {
            var firstOutput = UtilityApi.GetAccessModifierKindFromToken(firstSyntaxToken);

            if (firstOutput is not null)
            {
                _ = model.StatementBuilder.Pop();
                accessModifierKind = firstOutput.Value;

				// Given: protected internal class MyClass { }
				// Then: protected internal
                if (model.StatementBuilder.TryPeek(out syntax) && syntax is ISyntaxToken secondSyntaxToken)
                {
                    var secondOutput = UtilityApi.GetAccessModifierKindFromToken(secondSyntaxToken);

                    if (secondOutput is not null)
                    {
                        _ = model.StatementBuilder.Pop();

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
        var storageModifierKind = UtilityApi.GetStorageModifierKindFromToken(storageModifierToken);
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
            genericArgumentsListingNode = ParseTypes.HandleGenericArguments(model);

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
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = typeDefinitionNode;
    }

    public static void HandleClassTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleStorageModifierTokenKeyword(model);
    }

    public static void HandleNamespaceTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var namespaceKeywordToken = (KeywordToken)model.TokenWalker.Consume();
    	
    	var handleNamespaceIdentifierResult = ParseOthers.HandleNamespaceIdentifier(model);

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            model.DiagnosticBag.ReportTodoException(namespaceKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        var namespaceStatementNode = new NamespaceStatementNode(
            namespaceKeywordToken,
            namespaceIdentifier,
            null);

        model.Binder.SetCurrentNamespaceStatementNode(namespaceStatementNode, model);
        
        model.SyntaxStack.Push(namespaceStatementNode);
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = namespaceStatementNode;
    }

    public static void HandleReturnTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordToken)model.TokenWalker.Consume());
    }
}
