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
        compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleBaseTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleBoolTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleBreakTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleByteTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleCaseTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.ColonToken, null));
		var expressionNode = ParseOthers.ParseExpression(compilationUnit);
	    var colonToken = (ColonToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.ColonToken);
    }

    public static void HandleCatchTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var catchKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	var typeClause = compilationUnit.ParserModel.TokenWalker.MatchTypeClauseNode(compilationUnit);
    	
    	if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.CloseParenthesisToken)
    		_ = (IdentifierToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
    	
    	var closeParenthesisToken = (CloseParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
    
    	TryStatementNode? tryStatementNode = null;
    
    	if (compilationUnit.ParserModel.SyntaxStack.TryPeek(out var syntax) &&
    		syntax is TryStatementNode temporaryTryStatementNodeOne)
    	{
	        tryStatementNode = temporaryTryStatementNodeOne;
    	}
    	else if (compilationUnit.ParserModel.SyntaxStack.TryPeek(out syntax) &&
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
        	compilationUnit.ParserModel.SyntaxStack.Push(catchNode);
        	compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = catchNode;
    	}
    }

    public static void HandleCharTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleCheckedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleConstTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleContinueTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleDecimalTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleDefaultTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	// Switch statement default case.
        if (compilationUnit.ParserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.ColonToken)
        	_ = compilationUnit.ParserModel.TokenWalker.Consume();
		else
			compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleDelegateTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleDoTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var doKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var doWhileStatementNode = new DoWhileStatementNode(
	    	doKeywordToken,
	        openBraceToken: default,
	        codeBlockNode: null,
	        whileKeywordToken: default,
	        openParenthesisToken: default,
	        expressionNode: null,
	        closeParenthesisToken: default);
        	
        // Have to push twice so it is on the stack when the 'while' keyword is parsed.
		compilationUnit.ParserModel.SyntaxStack.Push(doWhileStatementNode);
		compilationUnit.ParserModel.SyntaxStack.Push(doWhileStatementNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = doWhileStatementNode;
    }

    public static void HandleDoubleTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleElseTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleEnumTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleStorageModifierTokenKeyword(compilationUnit);

        // Why was this method invocation here? (2024-01-23)
        //
        // HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleEventTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleExplicitTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleExternTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleFalseTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit);
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add(expressionNode);
    }

    public static void HandleFinallyTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var finallyKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	TryStatementNode? tryStatementNode = null;
    	
    	if (compilationUnit.ParserModel.SyntaxStack.TryPeek(out var syntax) &&
    		syntax is TryStatementNode temporaryTryStatementNodeOne)
    	{
	        tryStatementNode = temporaryTryStatementNodeOne;
    	}
        else if (compilationUnit.ParserModel.SyntaxStack.TryPeek(out syntax) &&
    		syntax is TryStatementTryNode tryNode)
    	{
	        if (tryNode.Parent is TryStatementNode temporaryTryStatementNodeTwo)
	        {
	        	tryStatementNode = temporaryTryStatementNodeTwo;
	        }
    	}
    	else if (compilationUnit.ParserModel.SyntaxStack.TryPeek(out syntax) &&
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
	    	compilationUnit.ParserModel.SyntaxStack.Push(finallyNode);
        	compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = finallyNode;
    	}
    }

    public static void HandleFixedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleFloatTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleForTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var forKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
        
        // Initialization Case One
        //     ;
        var initializationExpressionNode = (IExpressionNode)new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
        var initializationStatementDelimiterToken = (StatementDelimiterToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        var badStateInitialization = false;
        
        if (initializationStatementDelimiterToken.IsFabricated)
        {
        	// Initialization Case Two
        	//     i = 0;
        	var identifierToken = (IdentifierToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
        	
        	if (identifierToken.IsFabricated)
        	{
        		// Initialization Case Three
	    		//     int i = 0;
	        	var typeClauseNode = compilationUnit.ParserModel.TokenWalker.MatchTypeClauseNode(compilationUnit);
	        	var isCaseThree = !typeClauseNode.IsFabricated;
	        	
	        	if (isCaseThree)
	        	{
	        		identifierToken = (IdentifierToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
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
        		var equalsToken = (EqualsToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.EqualsToken);
        		
        		compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        		initializationExpressionNode = ParseOthers.ParseExpression(compilationUnit);
			    
			    initializationStatementDelimiterToken = (StatementDelimiterToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        	}
        }
        
        // Condition Case One
    	//     ;
    	var conditionExpressionNode = (IExpressionNode)new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
        var conditionStatementDelimiterToken = (StatementDelimiterToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        
        if (conditionStatementDelimiterToken.IsFabricated)
        {
        	// Condition Case Two
        	//     i < 10;
        
        	compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        	conditionExpressionNode = ParseOthers.ParseExpression(compilationUnit);
		    
		    conditionStatementDelimiterToken = (StatementDelimiterToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        }
        
        // Updation Case One
        //    )
        var updationExpressionNode = (IExpressionNode)new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
        var closeParenthesisToken = (CloseParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
        
        if (closeParenthesisToken.IsFabricated)
        {
        	compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        	updationExpressionNode = ParseOthers.ParseExpression(compilationUnit);
		    
		    closeParenthesisToken = (CloseParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		    
		    if (closeParenthesisToken.IsFabricated)
		    {
		    	while (!compilationUnit.ParserModel.TokenWalker.IsEof)
		    	{
		    		if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
		    			break;
		    		
		    		_ = compilationUnit.ParserModel.TokenWalker.Consume();
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
	        
        compilationUnit.ParserModel.SyntaxStack.Push(forStatementNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = forStatementNode;
    }

    public static void HandleForeachTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var foreachKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	var typeClauseNode = compilationUnit.ParserModel.TokenWalker.MatchTypeClauseNode(compilationUnit);
    	var variableIdentifierToken = (IdentifierToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
    	
    	var variableDeclarationStatementNode = new VariableDeclarationNode(
            typeClauseNode,
            variableIdentifierToken,
            VariableKind.Local,
            false);
    	
    	var inKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.InTokenKeyword);
    	
    	compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit);
		var closeParenthesisToken = (CloseParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var foreachStatementNode = new ForeachStatementNode(
	        foreachKeywordToken,
	        openParenthesisToken,
	        variableDeclarationStatementNode,
	        inKeywordToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        compilationUnit.ParserModel.SyntaxStack.Push(foreachStatementNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = foreachStatementNode;
    }

    public static void HandleGotoTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleImplicitTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleInTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleIntTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleIsTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleLockTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var lockKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit);
		
		var closeParenthesisToken = (CloseParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var lockStatementNode = new LockStatementNode(
			lockKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        compilationUnit.ParserModel.SyntaxStack.Push(lockStatementNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = lockStatementNode;
    }

    public static void HandleLongTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleNullTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleObjectTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleOperatorTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleOutTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleParamsTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleProtectedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var protectedTokenKeyword = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
        compilationUnit.ParserModel.StatementBuilder.ChildList.Add(protectedTokenKeyword);
    }

    public static void HandleReadonlyTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleRefTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleSbyteTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleShortTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleSizeofTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleStackallocTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleStringTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleStructTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleStorageModifierTokenKeyword(compilationUnit);
    }

    public static void HandleSwitchTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var switchKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit);
		
		var closeParenthesisToken = (CloseParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var switchStatementNode = new SwitchStatementNode(
			switchKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        compilationUnit.ParserModel.SyntaxStack.Push(switchStatementNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = switchStatementNode;
    }

    public static void HandleThisTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleThrowTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleTrueTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit);
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add(expressionNode);
    }

    public static void HandleTryTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var tryKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var tryStatementNode = new TryStatementNode(
			tryNode: null,
	        catchNode: null,
	        finallyNode: null);
    
	    var tryStatementTryNode = new TryStatementTryNode(
	    	tryStatementNode,
        	tryKeywordToken,
        	codeBlockNode: null);
        	
		tryStatementNode.SetTryStatementTryNode(tryStatementTryNode);
	        
	    compilationUnit.ParserModel.CurrentCodeBlockBuilder.ChildList.Add(tryStatementNode);
	        
		compilationUnit.ParserModel.SyntaxStack.Push(tryStatementNode);
		
		compilationUnit.ParserModel.SyntaxStack.Push(tryStatementTryNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = tryStatementTryNode;
    }

    public static void HandleTypeofTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleUintTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleUlongTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleUncheckedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleUnsafeTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleUshortTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleVoidTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleTypeIdentifierKeyword(compilationUnit);
    }

    public static void HandleVolatileTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleWhileTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var whileKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        var expressionNode = ParseOthers.ParseExpression(compilationUnit);
		
		var closeParenthesisToken = (CloseParenthesisToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		if (compilationUnit.ParserModel.SyntaxStack.TryPeek(out var syntax) &&
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
		        
	        compilationUnit.ParserModel.SyntaxStack.Push(whileStatementNode);
        	compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = whileStatementNode;
		}
    }

    public static void HandleUnrecognizedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

	/// <summary>The 'Default' of this method name is confusing.
	/// It seems to refer to the 'default' of switch statement rather than the 'default' keyword itself?
	/// </summary>
    public static void HandleDefault(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleTypeIdentifierKeyword(CSharpCompilationUnit compilationUnit)
    {
    	ParseTokens.ParseIdentifierToken(compilationUnit);
    }

    public static void HandleNewTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	if (compilationUnit.ParserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
    		UtilityApi.IsConvertibleToIdentifierToken(compilationUnit.ParserModel.TokenWalker.Next.SyntaxKind))
    	{
    		var expressionNode = ParseOthers.ParseExpression(compilationUnit);
    		compilationUnit.ParserModel.StatementBuilder.ChildList.Add(expressionNode);
    	}
    	else
    	{
    		compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    	}
    }

    public static void HandlePublicTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var publicKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
        compilationUnit.ParserModel.StatementBuilder.ChildList.Add(publicKeywordToken);
    }

    public static void HandleInternalTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var internalTokenKeyword = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
        compilationUnit.ParserModel.StatementBuilder.ChildList.Add(internalTokenKeyword);
    }

    public static void HandlePrivateTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var privateTokenKeyword = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
        compilationUnit.ParserModel.StatementBuilder.ChildList.Add(privateTokenKeyword);
    }

    public static void HandleStaticTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleOverrideTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleVirtualTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleAbstractTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleSealedTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleIfTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var ifTokenKeyword = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);

        if (openParenthesisToken.IsFabricated)
            return;

		compilationUnit.ParserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		var expression = ParseOthers.ParseExpression(compilationUnit);

        var boundIfStatementNode = compilationUnit.ParserModel.Binder.BindIfStatementNode(ifTokenKeyword, expression);
        compilationUnit.ParserModel.SyntaxStack.Push(boundIfStatementNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = boundIfStatementNode;
    }

    public static void HandleUsingTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var usingKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var handleNamespaceIdentifierResult = ParseOthers.HandleNamespaceIdentifier(compilationUnit);

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            compilationUnit.ParserModel.DiagnosticBag.ReportTodoException(usingKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        var usingStatementNode = new UsingStatementNode(
            usingKeywordToken,
            namespaceIdentifier);

        compilationUnit.ParserModel.Binder.BindUsingStatementNode(usingStatementNode, compilationUnit);
        compilationUnit.ParserModel.StatementBuilder.ChildList.Add(usingStatementNode);
    }

    public static void HandleInterfaceTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	HandleStorageModifierTokenKeyword(compilationUnit);
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
    	var storageModifierToken = compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	// Given: public partial class MyClass { }
		// Then: partial
        var hasPartialModifier = false;
        if (compilationUnit.ParserModel.StatementBuilder.TryPeek(out var syntax) && syntax is ISyntaxToken syntaxToken)
        {
            if (syntaxToken.SyntaxKind == SyntaxKind.PartialTokenContextualKeyword)
            {
                _ = compilationUnit.ParserModel.StatementBuilder.Pop();
                hasPartialModifier = true;
            }
        }
    
    	// TODO: Fix; the code that parses the accessModifierKind is a mess
		//
		// Given: public class MyClass { }
		// Then: public
		var accessModifierKind = AccessModifierKind.Public;
        if (compilationUnit.ParserModel.StatementBuilder.TryPeek(out syntax) && syntax is ISyntaxToken firstSyntaxToken)
        {
            var firstOutput = UtilityApi.GetAccessModifierKindFromToken(firstSyntaxToken);

            if (firstOutput is not null)
            {
                _ = compilationUnit.ParserModel.StatementBuilder.Pop();
                accessModifierKind = firstOutput.Value;

				// Given: protected internal class MyClass { }
				// Then: protected internal
                if (compilationUnit.ParserModel.StatementBuilder.TryPeek(out syntax) && syntax is ISyntaxToken secondSyntaxToken)
                {
                    var secondOutput = UtilityApi.GetAccessModifierKindFromToken(secondSyntaxToken);

                    if (secondOutput is not null)
                    {
                        _ = compilationUnit.ParserModel.StatementBuilder.Pop();

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
        	compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StructTokenKeyword)
        {
        	var structKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
        	storageModifierKind = StorageModifierKind.RecordStruct;
        }
    
		// Given: public class MyClass<T> { }
		// Then: MyClass
		IdentifierToken identifierToken;
		// Retrospective: What is the purpose of this 'if (contextualKeyword) logic'?
		// Response: maybe it is because 'var' contextual keyword is allowed to be a class name?
        if (UtilityApi.IsContextualKeywordSyntaxKind(compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind))
        {
            var contextualKeywordToken = (KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume();
            // Take the contextual keyword as an identifier
            identifierToken = new IdentifierToken(contextualKeywordToken.TextSpan);
        }
        else
        {
            identifierToken = (IdentifierToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

		// Given: public class MyClass<T> { }
		// Then: <T>
        GenericArgumentsListingNode? genericArgumentsListingNode = null;
        if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            genericArgumentsListingNode = ParseTypes.HandleGenericArguments(compilationUnit);

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

        compilationUnit.ParserModel.Binder.BindTypeDefinitionNode(typeDefinitionNode, compilationUnit);
        compilationUnit.ParserModel.Binder.BindTypeIdentifier(identifierToken, compilationUnit);
        compilationUnit.ParserModel.SyntaxStack.Push(typeDefinitionNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = typeDefinitionNode;
    }

    public static void HandleClassTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
        HandleStorageModifierTokenKeyword(compilationUnit);
    }

    public static void HandleNamespaceTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var namespaceKeywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	var handleNamespaceIdentifierResult = ParseOthers.HandleNamespaceIdentifier(compilationUnit);

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            compilationUnit.ParserModel.DiagnosticBag.ReportTodoException(namespaceKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        var namespaceStatementNode = new NamespaceStatementNode(
            namespaceKeywordToken,
            namespaceIdentifier,
            null);

        compilationUnit.ParserModel.Binder.SetCurrentNamespaceStatementNode(namespaceStatementNode, compilationUnit);
        
        compilationUnit.ParserModel.SyntaxStack.Push(namespaceStatementNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = namespaceStatementNode;
    }

    public static void HandleReturnTokenKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }
}
