using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseDefaultKeywords
{
    public static void HandleAsTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleBaseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleBoolTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleBreakTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleByteTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleCaseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var caseKeyword = parserComputation.TokenWalker.Consume();
    	
    	parserComputation.ExpressionList.Add((SyntaxKind.ColonToken, null));
		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
	    var colonToken = parserComputation.TokenWalker.Match(SyntaxKind.ColonToken);
    }

    public static void HandleCatchTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var catchKeywordToken = parserComputation.TokenWalker.Consume();
    	var openParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserComputation.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
    	
    	var closeParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
    
    	TryStatementNode? tryStatementNode = null;
    	
    	var catchNode = new TryStatementCatchNode(
        	tryStatementNode,
	        catchKeywordToken,
	        openParenthesisToken,
	        closeParenthesisToken,
	        codeBlockNode: null);
	    
	    if (expressionNode.SyntaxKind == SyntaxKind.VariableDeclarationNode)
	    {
	    	var variableDeclarationNode = (VariableDeclarationNode)expressionNode;
			parserComputation.Binder.RemoveVariableDeclarationNodeFromActiveBinderSession(parserComputation.CurrentScopeIndexKey, variableDeclarationNode, compilationUnit, ref parserComputation);
	    	catchNode.SetVariableDeclarationNode(variableDeclarationNode);
	    }
    
    	// This was done with CSharpParserModel's SyntaxStack, but that property is now being removed. A different way to accomplish this needs to be done. (2025-02-06)
    	// tryStatementNode.SetTryStatementCatchNode(catchNode);
    	
    	parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	catchNode,
	        catchNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
	        
	    if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
    		parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleCharTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleCheckedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleConstTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleContinueTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleDecimalTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleDefaultTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	// Switch statement default case.
        if (parserComputation.TokenWalker.Next.SyntaxKind == SyntaxKind.ColonToken)
        	_ = parserComputation.TokenWalker.Consume();
		else
			parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleDelegateTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleDoTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var doKeywordToken = parserComputation.TokenWalker.Consume();
    	
    	var doWhileStatementNode = new DoWhileStatementNode(
	    	doKeywordToken,
	        whileKeywordToken: default,
	        openParenthesisToken: default,
	        expressionNode: null,
	        closeParenthesisToken: default);
		
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	doWhileStatementNode,
	        doWhileStatementNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
	    
	    if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        	parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleDoubleTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleElseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleEnumTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleStorageModifierTokenKeyword(compilationUnit, ref parserComputation);

        // Why was this method invocation here? (2024-01-23)
        //
        // HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleEventTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleExplicitTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleExternTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleFalseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
    	parserComputation.StatementBuilder.ChildList.Add(expressionNode);
    }

    public static void HandleFinallyTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var finallyKeywordToken = parserComputation.TokenWalker.Consume();
    	
    	TryStatementNode? tryStatementNode = null;
    	
		var finallyNode = new TryStatementFinallyNode(
			tryStatementNode,
        	finallyKeywordToken,
        	codeBlockNode: null);
    
    	// This was done with CSharpParserModel's SyntaxStack, but that property is now being removed. A different way to accomplish this needs to be done. (2025-02-06)
		// tryStatementNode.SetTryStatementFinallyNode(finallyNode);
    	
    	parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	finallyNode,
	        finallyNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
	
		if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
    		parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleFixedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleFloatTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleForTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var forKeywordToken = parserComputation.TokenWalker.Consume();
    	var openParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	var forStatementNode = new ForStatementNode(
	        forKeywordToken,
	        openParenthesisToken,
	        Array.Empty<ISyntax>(),
	        initializationStatementDelimiterToken: default,
	        conditionExpressionNode: null,
	        conditionStatementDelimiterToken: default,
	        updationExpressionNode: null,
	        closeParenthesisToken: default,
	        codeBlockNode: null);
	        
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	forStatementNode,
	        forStatementNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
	    
	    parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = false;
        
        for (int i = 0; i < 3; i++)
        {
	        parserComputation.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
			_ = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
			
			var statementDelimiterToken = parserComputation.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
			
			if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
				break;
		}
		
		var closeParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
        
		if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        	parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleForeachTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var foreachKeywordToken = parserComputation.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	var typeClauseNode = parserComputation.TokenWalker.MatchTypeClauseNode(compilationUnit, ref parserComputation);
    	var variableIdentifierToken = parserComputation.TokenWalker.Match(SyntaxKind.IdentifierToken);
    	
    	var variableDeclarationStatementNode = new VariableDeclarationNode(
            typeClauseNode,
            variableIdentifierToken,
            VariableKind.Local,
            false);
    	
    	var inKeywordToken = parserComputation.TokenWalker.Match(SyntaxKind.InTokenKeyword);
    	
    	parserComputation.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
		var closeParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var foreachStatementNode = new ForeachStatementNode(
	        foreachKeywordToken,
	        openParenthesisToken,
	        variableDeclarationStatementNode,
	        inKeywordToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	foreachStatementNode,
	        foreachStatementNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
	        
	    if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        		parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleGotoTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleImplicitTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleInTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleIntTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleIsTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleLockTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var lockKeywordToken = parserComputation.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserComputation.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
		
		var closeParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var lockStatementNode = new LockStatementNode(
			lockKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	lockStatementNode,
	        lockStatementNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
    
    	if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        	parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleLongTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleNullTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleObjectTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleOperatorTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleOutTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleParamsTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleProtectedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var protectedTokenKeyword = parserComputation.TokenWalker.Consume();
        parserComputation.StatementBuilder.ChildList.Add(protectedTokenKeyword);
    }

    public static void HandleReadonlyTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleRefTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleSbyteTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleShortTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleSizeofTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleStackallocTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleStringTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleStructTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleStorageModifierTokenKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleSwitchTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var switchKeywordToken = parserComputation.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserComputation.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
		
		var closeParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var switchStatementNode = new SwitchStatementNode(
			switchKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	switchStatementNode,
	        switchStatementNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
    }

    public static void HandleThisTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleThrowTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleTrueTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
    	parserComputation.StatementBuilder.ChildList.Add(expressionNode);
    }

    public static void HandleTryTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var tryKeywordToken = parserComputation.TokenWalker.Consume();
    	
    	var tryStatementNode = new TryStatementNode(
			tryNode: null,
	        catchNode: null,
	        finallyNode: null);
    
	    var tryStatementTryNode = new TryStatementTryNode(
	    	tryStatementNode,
        	tryKeywordToken,
        	codeBlockNode: null);
        	
		tryStatementNode.SetTryStatementTryNode(tryStatementTryNode);
	        
	    parserComputation.CurrentCodeBlockBuilder.ChildList.Add(tryStatementNode);
        
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	tryStatementTryNode,
	        tryStatementTryNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
    
    	if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        	parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleTypeofTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleUintTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleUlongTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleUncheckedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleUnsafeTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleUshortTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleVoidTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleVolatileTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleWhileTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var whileKeywordToken = parserComputation.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserComputation.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
		
		var closeParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		/* This was done with CSharpParserModel's SyntaxStack, but that property is now being removed. A different way to accomplish this needs to be done. (2025-02-06)
    	{
		    doWhileStatementNode.SetWhileProperties(
		    	whileKeywordToken,
		        openParenthesisToken,
		        expressionNode,
		        closeParenthesisToken);
		    
		    return;
    	}*/
		
		var whileStatementNode = new WhileStatementNode(
			whileKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
    	parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	whileStatementNode,
	        whileStatementNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
	
		if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
    		parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleUnrecognizedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

	/// <summary>The 'Default' of this method name is confusing.
	/// It seems to refer to the 'default' of switch statement rather than the 'default' keyword itself?
	/// </summary>
    public static void HandleDefault(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleTypeIdentifierKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	ParseTokens.ParseIdentifierToken(compilationUnit, ref parserComputation);
    }

    public static void HandleNewTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	if (parserComputation.TokenWalker.Next.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
    		UtilityApi.IsConvertibleToIdentifierToken(parserComputation.TokenWalker.Next.SyntaxKind))
    	{
    		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
    		parserComputation.StatementBuilder.ChildList.Add(expressionNode);
    	}
    	else
    	{
    		parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    	}
    }

    public static void HandlePublicTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var publicKeywordToken = parserComputation.TokenWalker.Consume();
        parserComputation.StatementBuilder.ChildList.Add(publicKeywordToken);
    }

    public static void HandleInternalTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var internalTokenKeyword = parserComputation.TokenWalker.Consume();
        parserComputation.StatementBuilder.ChildList.Add(internalTokenKeyword);
    }

    public static void HandlePrivateTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var privateTokenKeyword = parserComputation.TokenWalker.Consume();
        parserComputation.StatementBuilder.ChildList.Add(privateTokenKeyword);
    }

    public static void HandleStaticTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleOverrideTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleVirtualTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleAbstractTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleSealedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleIfTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var ifTokenKeyword = parserComputation.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);

        if (openParenthesisToken.IsFabricated)
            return;

		parserComputation.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		var expression = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
		
		var closeParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        var ifStatementNode = new IfStatementNode(
            ifTokenKeyword,
            expression,
            null);
        
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	ifStatementNode,
	        ifStatementNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
	    
	    if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        	parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleUsingTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var usingKeywordToken = parserComputation.TokenWalker.Consume();
    	
    	var handleNamespaceIdentifierResult = ParseOthers.HandleNamespaceIdentifier(compilationUnit, ref parserComputation);

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            // compilationUnit.DiagnosticBag.ReportTodoException(usingKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        
        var namespaceIdentifier = handleNamespaceIdentifierResult;

        var usingStatementNode = new UsingStatementNode(
            usingKeywordToken,
            (SyntaxToken)namespaceIdentifier);

        parserComputation.Binder.BindUsingStatementNode(usingStatementNode, compilationUnit, ref parserComputation);
        parserComputation.StatementBuilder.ChildList.Add(usingStatementNode);
    }

    public static void HandleInterfaceTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	HandleStorageModifierTokenKeyword(compilationUnit, ref parserComputation);
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
    public static void HandleStorageModifierTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var storageModifierToken = parserComputation.TokenWalker.Consume();
    	
    	// Given: public partial class MyClass { }
		// Then: partial
        var hasPartialModifier = false;
        if (parserComputation.StatementBuilder.TryPeek(out var syntax) && syntax is SyntaxToken syntaxToken)
        {
            if (syntaxToken.SyntaxKind == SyntaxKind.PartialTokenContextualKeyword)
            {
                _ = parserComputation.StatementBuilder.Pop();
                hasPartialModifier = true;
            }
        }
    
    	// TODO: Fix; the code that parses the accessModifierKind is a mess
		//
		// Given: public class MyClass { }
		// Then: public
		var accessModifierKind = AccessModifierKind.Public;
        if (parserComputation.StatementBuilder.TryPeek(out syntax) && syntax is SyntaxToken firstSyntaxToken)
        {
            var firstOutput = UtilityApi.GetAccessModifierKindFromToken(firstSyntaxToken);

            if (firstOutput is not null)
            {
                _ = parserComputation.StatementBuilder.Pop();
                accessModifierKind = firstOutput.Value;

				// Given: protected internal class MyClass { }
				// Then: protected internal
                if (parserComputation.StatementBuilder.TryPeek(out syntax) && syntax is SyntaxToken secondSyntaxToken)
                {
                    var secondOutput = UtilityApi.GetAccessModifierKindFromToken(secondSyntaxToken);

                    if (secondOutput is not null)
                    {
                        _ = parserComputation.StatementBuilder.Pop();

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
        	parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.StructTokenKeyword)
        {
        	var structKeywordToken = parserComputation.TokenWalker.Consume();
        	storageModifierKind = StorageModifierKind.RecordStruct;
        }
    
		// Given: public class MyClass<T> { }
		// Then: MyClass
		SyntaxToken identifierToken;
		// Retrospective: What is the purpose of this 'if (contextualKeyword) logic'?
		// Response: maybe it is because 'var' contextual keyword is allowed to be a class name?
        if (UtilityApi.IsContextualKeywordSyntaxKind(parserComputation.TokenWalker.Current.SyntaxKind))
        {
            var contextualKeywordToken = parserComputation.TokenWalker.Consume();
            // Take the contextual keyword as an identifier
            identifierToken = new SyntaxToken(SyntaxKind.IdentifierToken, contextualKeywordToken.TextSpan);
        }
        else
        {
            identifierToken = parserComputation.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

		// Given: public class MyClass<T> { }
		// Then: <T>
        GenericArgumentsListingNode? genericArgumentsListingNode = null;
        if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            genericArgumentsListingNode = ParseTypes.HandleGenericArguments(compilationUnit, ref parserComputation);

        var typeDefinitionNode = new TypeDefinitionNode(
            accessModifierKind,
            hasPartialModifier,
            storageModifierKind.Value,
            identifierToken,
            valueType: null,
            genericArgumentsListingNode,
            primaryConstructorFunctionArgumentsListingNode: null,
            inheritedTypeClauseNode: null);

        parserComputation.Binder.BindTypeDefinitionNode(typeDefinitionNode, compilationUnit, ref parserComputation);
        parserComputation.Binder.BindTypeIdentifier(identifierToken, compilationUnit, ref parserComputation);
        
        parserComputation.StatementBuilder.ChildList.Add(typeDefinitionNode);
        
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	typeDefinitionNode,
	        typeDefinitionNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
	        
	    parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = false;
	    
	    if (storageModifierKind == StorageModifierKind.Enum)
	    {
	    	ParseTypes.HandleEnumDefinitionNode(typeDefinitionNode, compilationUnit, ref parserComputation);
	    	return;
	    }
    
    	if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
    	{
    		ParseTypes.HandlePrimaryConstructorDefinition(
		        typeDefinitionNode,
		        compilationUnit,
		        ref parserComputation);
    	}
    	
    	if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
    	{
    		_ = parserComputation.TokenWalker.Consume(); // Consume the ColonToken
            var inheritedTypeClauseNode = parserComputation.TokenWalker.MatchTypeClauseNode(compilationUnit, ref parserComputation);
            parserComputation.Binder.BindTypeClauseNode(inheritedTypeClauseNode, compilationUnit, ref parserComputation);
			typeDefinitionNode.SetInheritedTypeClauseNode(inheritedTypeClauseNode);
			
			while (!parserComputation.TokenWalker.IsEof)
			{
				if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
				{
					_ = parserComputation.TokenWalker.Consume(); // Consume the CommaToken
				
					var consumeCounter = parserComputation.TokenWalker.ConsumeCounter;
					
            		parserComputation.Binder.BindTypeClauseNode(
            			parserComputation.TokenWalker.MatchTypeClauseNode(compilationUnit, ref parserComputation),
            			compilationUnit,
            			ref parserComputation);
            		
            		if (consumeCounter == parserComputation.TokenWalker.ConsumeCounter)
            			break;
				}
				else
				{
					break;
				}
			}
    	}
    	
    	if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.WhereTokenContextualKeyword)
    	{
    		parserComputation.ExpressionList.Add((SyntaxKind.OpenBraceToken, null));
			var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
    	}
    	
    	if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
			parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleClassTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        HandleStorageModifierTokenKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleNamespaceTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var namespaceKeywordToken = parserComputation.TokenWalker.Consume();
    	
    	var handleNamespaceIdentifierResult = ParseOthers.HandleNamespaceIdentifier(compilationUnit, ref parserComputation);

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            // compilationUnit.DiagnosticBag.ReportTodoException(namespaceKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        
        var namespaceIdentifier = handleNamespaceIdentifierResult;

        var namespaceStatementNode = new NamespaceStatementNode(
            namespaceKeywordToken,
            (SyntaxToken)namespaceIdentifier,
            null);

        parserComputation.Binder.SetCurrentNamespaceStatementNode(namespaceStatementNode, compilationUnit, ref parserComputation);
        
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	namespaceStatementNode,
	        namespaceStatementNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
	    
	    // Do not set 'IsImplicitOpenCodeBlockTextSpan' for namespace file scoped.
    }

    public static void HandleReturnTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var returnKeywordToken = parserComputation.TokenWalker.Consume();
   	 var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
   	 var returnStatementNode = new ReturnStatementNode(returnKeywordToken, expressionNode);
    	
		parserComputation.StatementBuilder.ChildList.Add(returnStatementNode);
	}
}
