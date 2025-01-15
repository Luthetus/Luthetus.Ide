using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.CompilerServices.CSharp.Facts;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseDefaultKeywords
{
    public static void HandleAsTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleBaseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleBoolTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleBreakTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleByteTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleCaseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var caseKeyword = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	parserModel.ExpressionList.Add((SyntaxKind.ColonToken, null));
		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
	    var colonToken = (ColonToken)parserModel.TokenWalker.Match(SyntaxKind.ColonToken);
    }

    public static void HandleCatchTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var catchKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	var openParenthesisToken = (OpenParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
    	
    	var closeParenthesisToken = (CloseParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
    
    	TryStatementNode? tryStatementNode = null;
    
    	if (parserModel.SyntaxStack.TryPeek(out var syntax) &&
    		syntax is TryStatementNode temporaryTryStatementNodeOne)
    	{
	        tryStatementNode = temporaryTryStatementNodeOne;
    	}
    	else if (parserModel.SyntaxStack.TryPeek(out syntax) &&
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
		    
		    if (expressionNode.SyntaxKind == SyntaxKind.VariableDeclarationNode)
		    {
		    	var variableDeclarationNode = (VariableDeclarationNode)expressionNode;
    			compilationUnit.Binder.RemoveVariableDeclarationNodeFromActiveBinderSession(compilationUnit.BinderSession.CurrentScopeIndexKey, variableDeclarationNode, compilationUnit, ref parserModel);
		    	catchNode.SetVariableDeclarationNode(variableDeclarationNode);
		    }
        
        	tryStatementNode.SetTryStatementCatchNode(catchNode);
        	parserModel.SyntaxStack.Push(catchNode);
        	
        	compilationUnit.Binder.NewScopeAndBuilderFromOwner(
	        	catchNode,
		        catchNode.GetReturnTypeClauseNode(),
		        parserModel.TokenWalker.Current.TextSpan,
		        compilationUnit,
		        ref parserModel);
    	}
    }

    public static void HandleCharTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleCheckedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleConstTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleContinueTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleDecimalTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleDefaultTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	// Switch statement default case.
        if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.ColonToken)
        	_ = parserModel.TokenWalker.Consume();
		else
			parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleDelegateTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleDoTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var doKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	var doWhileStatementNode = new DoWhileStatementNode(
	    	doKeywordToken,
	        whileKeywordToken: default,
	        openParenthesisToken: default,
	        expressionNode: null,
	        closeParenthesisToken: default);
        	
        // Have to push twice so it is on the stack when the 'while' keyword is parsed.
		parserModel.SyntaxStack.Push(doWhileStatementNode);
		parserModel.SyntaxStack.Push(doWhileStatementNode);
		
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	doWhileStatementNode,
	        doWhileStatementNode.GetReturnTypeClauseNode(),
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    }

    public static void HandleDoubleTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleElseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleEnumTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleStorageModifierTokenKeyword(compilationUnit, ref parserModel);

        // Why was this method invocation here? (2024-01-23)
        //
        // HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleEventTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleExplicitTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleExternTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleFalseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
    	parserModel.StatementBuilder.ChildList.Add(expressionNode);
    }

    public static void HandleFinallyTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var finallyKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	TryStatementNode? tryStatementNode = null;
    	
    	if (parserModel.SyntaxStack.TryPeek(out var syntax) &&
    		syntax is TryStatementNode temporaryTryStatementNodeOne)
    	{
	        tryStatementNode = temporaryTryStatementNodeOne;
    	}
        else if (parserModel.SyntaxStack.TryPeek(out syntax) &&
    		syntax is TryStatementTryNode tryNode)
    	{
	        if (tryNode.Parent is TryStatementNode temporaryTryStatementNodeTwo)
	        {
	        	tryStatementNode = temporaryTryStatementNodeTwo;
	        }
    	}
    	else if (parserModel.SyntaxStack.TryPeek(out syntax) &&
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
	    	parserModel.SyntaxStack.Push(finallyNode);
	    	
        	compilationUnit.Binder.NewScopeAndBuilderFromOwner(
	        	finallyNode,
		        finallyNode.GetReturnTypeClauseNode(),
		        parserModel.TokenWalker.Current.TextSpan,
		        compilationUnit,
		        ref parserModel);
    	}
    }

    public static void HandleFixedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleFloatTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleForTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var forKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
        
        // Initialization Case One
        //     ;
        var initializationExpressionNode = (IExpressionNode)new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
        var initializationStatementDelimiterToken = (StatementDelimiterToken)parserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        var badStateInitialization = false;
        
        if (initializationStatementDelimiterToken.IsFabricated)
        {
        	// Initialization Case Two
        	//     i = 0;
        	var identifierToken = (IdentifierToken)parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
        	
        	if (identifierToken.IsFabricated)
        	{
        		// Initialization Case Three
	    		//     int i = 0;
	        	var typeClauseNode = parserModel.TokenWalker.MatchTypeClauseNode(compilationUnit, ref parserModel);
	        	var isCaseThree = !typeClauseNode.IsFabricated;
	        	
	        	if (isCaseThree)
	        	{
	        		identifierToken = (IdentifierToken)parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
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
        		var equalsToken = (EqualsToken)parserModel.TokenWalker.Match(SyntaxKind.EqualsToken);
        		
        		parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        		initializationExpressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
			    
			    initializationStatementDelimiterToken = (StatementDelimiterToken)parserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        	}
        }
        
        // Condition Case One
    	//     ;
    	var conditionExpressionNode = (IExpressionNode)new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
        var conditionStatementDelimiterToken = (StatementDelimiterToken)parserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        
        if (conditionStatementDelimiterToken.IsFabricated)
        {
        	// Condition Case Two
        	//     i < 10;
        
        	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        	conditionExpressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		    
		    conditionStatementDelimiterToken = (StatementDelimiterToken)parserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        }
        
        // Updation Case One
        //    )
        var updationExpressionNode = (IExpressionNode)new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
        var closeParenthesisToken = (CloseParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
        
        if (closeParenthesisToken.IsFabricated)
        {
        	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        	updationExpressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		    
		    closeParenthesisToken = (CloseParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		    
		    if (closeParenthesisToken.IsFabricated)
		    {
		    	while (!parserModel.TokenWalker.IsEof)
		    	{
		    		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
		    			break;
		    		
		    		_ = parserModel.TokenWalker.Consume();
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
	        
        parserModel.SyntaxStack.Push(forStatementNode);
        
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	forStatementNode,
	        forStatementNode.GetReturnTypeClauseNode(),
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    }

    public static void HandleForeachTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var foreachKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	var typeClauseNode = parserModel.TokenWalker.MatchTypeClauseNode(compilationUnit, ref parserModel);
    	var variableIdentifierToken = (IdentifierToken)parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
    	
    	var variableDeclarationStatementNode = new VariableDeclarationNode(
            typeClauseNode,
            variableIdentifierToken,
            VariableKind.Local,
            false);
    	
    	var inKeywordToken = (KeywordToken)parserModel.TokenWalker.Match(SyntaxKind.InTokenKeyword);
    	
    	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		var closeParenthesisToken = (CloseParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var foreachStatementNode = new ForeachStatementNode(
	        foreachKeywordToken,
	        openParenthesisToken,
	        variableDeclarationStatementNode,
	        inKeywordToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        parserModel.SyntaxStack.Push(foreachStatementNode);
        
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	foreachStatementNode,
	        foreachStatementNode.GetReturnTypeClauseNode(),
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    }

    public static void HandleGotoTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleImplicitTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleInTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleIntTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleIsTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleLockTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var lockKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		
		var closeParenthesisToken = (CloseParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var lockStatementNode = new LockStatementNode(
			lockKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        parserModel.SyntaxStack.Push(lockStatementNode);
        
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	lockStatementNode,
	        lockStatementNode.GetReturnTypeClauseNode(),
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    }

    public static void HandleLongTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleNullTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleObjectTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleOperatorTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleOutTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleParamsTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleProtectedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var protectedTokenKeyword = (KeywordToken)parserModel.TokenWalker.Consume();
        parserModel.StatementBuilder.ChildList.Add(protectedTokenKeyword);
    }

    public static void HandleReadonlyTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleRefTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleSbyteTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleShortTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleSizeofTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleStackallocTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleStringTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleStructTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleStorageModifierTokenKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleSwitchTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var switchKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		
		var closeParenthesisToken = (CloseParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var switchStatementNode = new SwitchStatementNode(
			switchKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        parserModel.SyntaxStack.Push(switchStatementNode);
        
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	switchStatementNode,
	        switchStatementNode.GetReturnTypeClauseNode(),
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    }

    public static void HandleThisTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleThrowTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleTrueTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
    	parserModel.StatementBuilder.ChildList.Add(expressionNode);
    }

    public static void HandleTryTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var tryKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	var tryStatementNode = new TryStatementNode(
			tryNode: null,
	        catchNode: null,
	        finallyNode: null);
    
	    var tryStatementTryNode = new TryStatementTryNode(
	    	tryStatementNode,
        	tryKeywordToken,
        	codeBlockNode: null);
        	
		tryStatementNode.SetTryStatementTryNode(tryStatementTryNode);
	        
	    parserModel.CurrentCodeBlockBuilder.ChildList.Add(tryStatementNode);
	        
		parserModel.SyntaxStack.Push(tryStatementNode);
		
		parserModel.SyntaxStack.Push(tryStatementTryNode);
        
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	tryStatementTryNode,
	        tryStatementTryNode.GetReturnTypeClauseNode(),
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    }

    public static void HandleTypeofTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleUintTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleUlongTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleUncheckedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleUnsafeTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleUshortTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleVoidTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleVolatileTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleWhileTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var whileKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = (OpenParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		
		var closeParenthesisToken = (CloseParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		if (parserModel.SyntaxStack.TryPeek(out var syntax) &&
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
		        
	        parserModel.SyntaxStack.Push(whileStatementNode);
        	
        	compilationUnit.Binder.NewScopeAndBuilderFromOwner(
	        	whileStatementNode,
		        whileStatementNode.GetReturnTypeClauseNode(),
		        parserModel.TokenWalker.Current.TextSpan,
		        compilationUnit,
		        ref parserModel);
		}
    }

    public static void HandleUnrecognizedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

	/// <summary>The 'Default' of this method name is confusing.
	/// It seems to refer to the 'default' of switch statement rather than the 'default' keyword itself?
	/// </summary>
    public static void HandleDefault(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleTypeIdentifierKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	ParseTokens.ParseIdentifierToken(compilationUnit, ref parserModel);
    }

    public static void HandleNewTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (parserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
    		UtilityApi.IsConvertibleToIdentifierToken(parserModel.TokenWalker.Next.SyntaxKind))
    	{
    		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
    		parserModel.StatementBuilder.ChildList.Add(expressionNode);
    	}
    	else
    	{
    		parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    	}
    }

    public static void HandlePublicTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var publicKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
        parserModel.StatementBuilder.ChildList.Add(publicKeywordToken);
    }

    public static void HandleInternalTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var internalTokenKeyword = (KeywordToken)parserModel.TokenWalker.Consume();
        parserModel.StatementBuilder.ChildList.Add(internalTokenKeyword);
    }

    public static void HandlePrivateTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var privateTokenKeyword = (KeywordToken)parserModel.TokenWalker.Consume();
        parserModel.StatementBuilder.ChildList.Add(privateTokenKeyword);
    }

    public static void HandleStaticTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleOverrideTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleVirtualTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleAbstractTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleSealedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleIfTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var ifTokenKeyword = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);

        if (openParenthesisToken.IsFabricated)
            return;

		parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		var expression = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		
		var closeParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        var boundIfStatementNode = compilationUnit.Binder.BindIfStatementNode(ifTokenKeyword, expression);
        parserModel.SyntaxStack.Push(boundIfStatementNode);
        
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	boundIfStatementNode,
	        boundIfStatementNode.GetReturnTypeClauseNode(),
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    }

    public static void HandleUsingTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var usingKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	var handleNamespaceIdentifierResult = ParseOthers.HandleNamespaceIdentifier(compilationUnit, ref parserModel);

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            parserModel.DiagnosticBag.ReportTodoException(usingKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        var usingStatementNode = new UsingStatementNode(
            usingKeywordToken,
            namespaceIdentifier);

        compilationUnit.Binder.BindUsingStatementNode(usingStatementNode, compilationUnit);
        parserModel.StatementBuilder.ChildList.Add(usingStatementNode);
    }

    public static void HandleInterfaceTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	HandleStorageModifierTokenKeyword(compilationUnit, ref parserModel);
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
    public static void HandleStorageModifierTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var storageModifierToken = parserModel.TokenWalker.Consume();
    	
    	// Given: public partial class MyClass { }
		// Then: partial
        var hasPartialModifier = false;
        if (parserModel.StatementBuilder.TryPeek(out var syntax) && syntax is ISyntaxToken syntaxToken)
        {
            if (syntaxToken.SyntaxKind == SyntaxKind.PartialTokenContextualKeyword)
            {
                _ = parserModel.StatementBuilder.Pop();
                hasPartialModifier = true;
            }
        }
    
    	// TODO: Fix; the code that parses the accessModifierKind is a mess
		//
		// Given: public class MyClass { }
		// Then: public
		var accessModifierKind = AccessModifierKind.Public;
        if (parserModel.StatementBuilder.TryPeek(out syntax) && syntax is ISyntaxToken firstSyntaxToken)
        {
            var firstOutput = UtilityApi.GetAccessModifierKindFromToken(firstSyntaxToken);

            if (firstOutput is not null)
            {
                _ = parserModel.StatementBuilder.Pop();
                accessModifierKind = firstOutput.Value;

				// Given: protected internal class MyClass { }
				// Then: protected internal
                if (parserModel.StatementBuilder.TryPeek(out syntax) && syntax is ISyntaxToken secondSyntaxToken)
                {
                    var secondOutput = UtilityApi.GetAccessModifierKindFromToken(secondSyntaxToken);

                    if (secondOutput is not null)
                    {
                        _ = parserModel.StatementBuilder.Pop();

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
        	parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StructTokenKeyword)
        {
        	var structKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
        	storageModifierKind = StorageModifierKind.RecordStruct;
        }
    
		// Given: public class MyClass<T> { }
		// Then: MyClass
		IdentifierToken identifierToken;
		// Retrospective: What is the purpose of this 'if (contextualKeyword) logic'?
		// Response: maybe it is because 'var' contextual keyword is allowed to be a class name?
        if (UtilityApi.IsContextualKeywordSyntaxKind(parserModel.TokenWalker.Current.SyntaxKind))
        {
            var contextualKeywordToken = (KeywordContextualToken)parserModel.TokenWalker.Consume();
            // Take the contextual keyword as an identifier
            identifierToken = new IdentifierToken(contextualKeywordToken.TextSpan);
        }
        else
        {
            identifierToken = (IdentifierToken)parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

		// Given: public class MyClass<T> { }
		// Then: <T>
        GenericArgumentsListingNode? genericArgumentsListingNode = null;
        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            genericArgumentsListingNode = ParseTypes.HandleGenericArguments(compilationUnit, ref parserModel);

        var typeDefinitionNode = new TypeDefinitionNode(
            accessModifierKind,
            hasPartialModifier,
            storageModifierKind.Value,
            identifierToken,
            valueType: null,
            genericArgumentsListingNode,
            primaryConstructorFunctionArgumentsListingNode: null,
            inheritedTypeClauseNode: null);

        compilationUnit.Binder.BindTypeDefinitionNode(typeDefinitionNode, compilationUnit);
        compilationUnit.Binder.BindTypeIdentifier(identifierToken, compilationUnit);
        
        parserModel.SyntaxStack.Push(typeDefinitionNode);
        parserModel.StatementBuilder.ChildList.Add(typeDefinitionNode);
        
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	typeDefinitionNode,
	        typeDefinitionNode.GetReturnTypeClauseNode(),
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    }

    public static void HandleClassTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleStorageModifierTokenKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleNamespaceTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var namespaceKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
    	
    	var handleNamespaceIdentifierResult = ParseOthers.HandleNamespaceIdentifier(compilationUnit, ref parserModel);

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            parserModel.DiagnosticBag.ReportTodoException(namespaceKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        var namespaceStatementNode = new NamespaceStatementNode(
            namespaceKeywordToken,
            namespaceIdentifier,
            null);

        compilationUnit.Binder.SetCurrentNamespaceStatementNode(namespaceStatementNode, compilationUnit);
        
        parserModel.SyntaxStack.Push(namespaceStatementNode);
        
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	namespaceStatementNode,
	        namespaceStatementNode.GetReturnTypeClauseNode(),
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    }

    public static void HandleReturnTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var returnKeywordToken = (KeywordToken)parserModel.TokenWalker.Consume();
   	 var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
   	 var returnStatementNode = new ReturnStatementNode(returnKeywordToken, expressionNode);
    	
		parserModel.StatementBuilder.ChildList.Add(returnStatementNode);
	}
}
