using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseDefaultKeywords
{
    public static void HandleAsTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleBaseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleBoolTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleBreakTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleByteTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleCaseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var caseKeyword = parserModel.TokenWalker.Consume();
    	
    	parserModel.ExpressionList.Add((SyntaxKind.ColonToken, null));
		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
	    var colonToken = parserModel.TokenWalker.Match(SyntaxKind.ColonToken);
    }

    public static void HandleCatchTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var catchKeywordToken = parserModel.TokenWalker.Consume();
    	var openParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	var catchNode = new TryStatementCatchNode(
        	parent: null,
	        catchKeywordToken,
	        openParenthesisToken,
	        closeParenthesisToken: default,
	        codeBlockNode: null);
    	
    	parserModel.Binder.NewScopeAndBuilderFromOwner(
        	catchNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    	
    	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
    	
    	var closeParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
    
    	TryStatementNode? tryStatementNode = null;
	    
	    if (expressionNode.SyntaxKind == SyntaxKind.VariableDeclarationNode)
	    {
	    	var variableDeclarationNode = (VariableDeclarationNode)expressionNode;
	    	catchNode.SetVariableDeclarationNode(variableDeclarationNode);
	    }
	    
	    if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.WhenTokenContextualKeyword)
	    {
	    	_ = parserModel.TokenWalker.Consume(); // WhenTokenContextualKeyword
	    	
	    	parserModel.ExpressionList.Add((SyntaxKind.OpenBraceToken, null));
			_ = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
	    }
	    
	    // Not valid C# -- catch requires brace deliminated code block --, but here for parser recovery.
	    if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
    		parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleCharTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleCheckedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleConstTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleContinueTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
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
			parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleDelegateTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleDoTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var doKeywordToken = parserModel.TokenWalker.Consume();
    	
    	var doWhileStatementNode = new DoWhileStatementNode(
	    	doKeywordToken,
	        whileKeywordToken: default,
	        openParenthesisToken: default,
	        expressionNode: null,
	        closeParenthesisToken: default);
		
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	doWhileStatementNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
	    
	    if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        	parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleDoubleTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleElseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
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
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleExplicitTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleExternTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleFalseTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
    	parserModel.StatementBuilder.ChildList.Add(expressionNode);
    }

    public static void HandleFinallyTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var finallyKeywordToken = parserModel.TokenWalker.Consume();
    	
    	TryStatementNode? tryStatementNode = null;
    	
		var finallyNode = new TryStatementFinallyNode(
			tryStatementNode,
        	finallyKeywordToken,
        	codeBlockNode: null);
    
    	// This was done with CSharpParserModel's SyntaxStack, but that property is now being removed. A different way to accomplish this needs to be done. (2025-02-06)
		// tryStatementNode.SetTryStatementFinallyNode(finallyNode);
    	
    	parserModel.Binder.NewScopeAndBuilderFromOwner(
        	finallyNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
	
		if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
    		parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleFixedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleFloatTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleForTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var forKeywordToken = parserModel.TokenWalker.Consume();
    	var openParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
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
	        
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	forStatementNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
	    
	    parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = false;
        
        for (int i = 0; i < 3; i++)
        {
	        parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
			_ = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
			
			var statementDelimiterToken = parserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
			
			if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
				break;
		}
		
		var closeParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
        
		if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        	parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleForeachTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var foreachKeywordToken = parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	var typeClauseNode = parserModel.TokenWalker.MatchTypeClauseNode(compilationUnit, ref parserModel);
    	var variableIdentifierToken = parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
    	
    	var variableDeclarationStatementNode = new VariableDeclarationNode(
            new TypeReference(typeClauseNode),
            variableIdentifierToken,
            VariableKind.Local,
            false);
    	
    	var inKeywordToken = parserModel.TokenWalker.Match(SyntaxKind.InTokenKeyword);
    	
    	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		var closeParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var foreachStatementNode = new ForeachStatementNode(
	        foreachKeywordToken,
	        openParenthesisToken,
	        variableDeclarationStatementNode,
	        inKeywordToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	foreachStatementNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
	        
	    if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        		parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleGotoTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleImplicitTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleInTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleIntTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleTypeIdentifierKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleIsTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleLockTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var lockKeywordToken = parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		
		var closeParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var lockStatementNode = new LockStatementNode(
			lockKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	lockStatementNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    
    	if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        	parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
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
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleOutTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleParamsTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleProtectedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var protectedTokenKeyword = parserModel.TokenWalker.Consume();
        parserModel.StatementBuilder.ChildList.Add(protectedTokenKeyword);
    }

    public static void HandleReadonlyTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleRefTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
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
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleStackallocTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
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
    	var switchKeywordToken = parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		
		var closeParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var switchStatementNode = new SwitchStatementNode(
			switchKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	switchStatementNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    }

    public static void HandleThisTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleThrowTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleTrueTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
    	parserModel.StatementBuilder.ChildList.Add(expressionNode);
    }

    public static void HandleTryTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var tryKeywordToken = parserModel.TokenWalker.Consume();
    	
    	var tryStatementNode = new TryStatementNode(
			tryNode: null,
	        catchNode: null,
	        finallyNode: null);
    
	    var tryStatementTryNode = new TryStatementTryNode(
	    	tryStatementNode,
        	tryKeywordToken,
        	codeBlockNode: null);
        	
		tryStatementNode.SetTryStatementTryNode(tryStatementTryNode);
	        
	    parserModel.CurrentCodeBlockBuilder.AddChild(tryStatementNode);
        
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	tryStatementTryNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
    
    	if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        	parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleTypeofTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
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
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleUnsafeTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
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
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleWhileTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var whileKeywordToken = parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
    	parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		
		var closeParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
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
	        
    	parserModel.Binder.NewScopeAndBuilderFromOwner(
        	whileStatementNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
	
		if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
    		parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleUnrecognizedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

	/// <summary>The 'Default' of this method name is confusing.
	/// It seems to refer to the 'default' of switch statement rather than the 'default' keyword itself?
	/// </summary>
    public static void HandleDefault(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
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
    		parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    	}
    }

    public static void HandlePublicTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var publicKeywordToken = parserModel.TokenWalker.Consume();
        parserModel.StatementBuilder.ChildList.Add(publicKeywordToken);
    }

    public static void HandleInternalTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var internalTokenKeyword = parserModel.TokenWalker.Consume();
        parserModel.StatementBuilder.ChildList.Add(internalTokenKeyword);
    }

    public static void HandlePrivateTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var privateTokenKeyword = parserModel.TokenWalker.Consume();
        parserModel.StatementBuilder.ChildList.Add(privateTokenKeyword);
    }

    public static void HandleStaticTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleOverrideTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleVirtualTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleAbstractTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleSealedTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add(parserModel.TokenWalker.Consume());
    }

    public static void HandleIfTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var ifTokenKeyword = parserModel.TokenWalker.Consume();
    	
    	var openParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);

        if (openParenthesisToken.IsFabricated)
            return;

		parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		var expression = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		
		var closeParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        var ifStatementNode = new IfStatementNode(
            ifTokenKeyword,
            expression,
            null);
        
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	ifStatementNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
	    
	    if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
        	parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleUsingTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (parserModel.UsingStatementListingNode is null)
    	{
    		parserModel.UsingStatementListingNode ??= new();
    		parserModel.StatementBuilder.ChildList.Add(parserModel.UsingStatementListingNode);
    	}
    	
    	var usingKeywordToken = parserModel.TokenWalker.Consume();
    	
    	var namespaceIdentifierToken = ParseOthers.HandleNamespaceIdentifier(compilationUnit, ref parserModel, isNamespaceStatement: false);

        if (!namespaceIdentifierToken.ConstructorWasInvoked)
        {
            // compilationUnit.DiagnosticBag.ReportTodoException(usingKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        
        parserModel.Binder.BindUsingStatementTuple(usingKeywordToken, namespaceIdentifierToken, compilationUnit, ref parserModel);
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
        if (parserModel.StatementBuilder.TryPeek(out var syntax) && syntax is SyntaxToken syntaxToken)
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
        if (parserModel.StatementBuilder.TryPeek(out syntax) && syntax is SyntaxToken firstSyntaxToken)
        {
            var firstOutput = UtilityApi.GetAccessModifierKindFromToken(firstSyntaxToken);

            if (firstOutput != AccessModifierKind.None)
            {
                _ = parserModel.StatementBuilder.Pop();
                accessModifierKind = firstOutput;

				// Given: protected internal class MyClass { }
				// Then: protected internal
                if (parserModel.StatementBuilder.TryPeek(out syntax) && syntax is SyntaxToken secondSyntaxToken)
                {
                    var secondOutput = UtilityApi.GetAccessModifierKindFromToken(secondSyntaxToken);

                    if (secondOutput != AccessModifierKind.None)
                    {
                        _ = parserModel.StatementBuilder.Pop();

                        if ((firstOutput.ToString().ToLower() == "protected" &&
                                secondOutput.ToString().ToLower() == "internal") ||
                            (firstOutput.ToString().ToLower() == "internal" &&
                                secondOutput.ToString().ToLower() == "protected"))
                        {
                            accessModifierKind = AccessModifierKind.ProtectedInternal;
                        }
                        else if ((firstOutput.ToString().ToLower() == "private" &&
                                    secondOutput.ToString().ToLower() == "protected") ||
                                (firstOutput.ToString().ToLower() == "protected" &&
                                    secondOutput.ToString().ToLower() == "private"))
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
        if (storageModifierKind == StorageModifierKind.None)
            return;
        if (storageModifierKind == StorageModifierKind.Record &&
        	parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StructTokenKeyword)
        {
        	var structKeywordToken = parserModel.TokenWalker.Consume();
        	storageModifierKind = StorageModifierKind.RecordStruct;
        }
    
		// Given: public class MyClass<T> { }
		// Then: MyClass
		SyntaxToken identifierToken;
		// Retrospective: What is the purpose of this 'if (contextualKeyword) logic'?
		// Response: maybe it is because 'var' contextual keyword is allowed to be a class name?
        if (UtilityApi.IsContextualKeywordSyntaxKind(parserModel.TokenWalker.Current.SyntaxKind))
        {
            var contextualKeywordToken = parserModel.TokenWalker.Consume();
            // Take the contextual keyword as an identifier
            identifierToken = new SyntaxToken(SyntaxKind.IdentifierToken, contextualKeywordToken.TextSpan);
        }
        else
        {
            identifierToken = parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

		// Given: public class MyClass<T> { }
		// Then: <T>
        GenericParameterListing genericParameterListing = default;
        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            genericParameterListing = ParseTypes.HandleGenericParameters(compilationUnit, ref parserModel);

        var typeDefinitionNode = new TypeDefinitionNode(
            accessModifierKind,
            hasPartialModifier,
            storageModifierKind,
            identifierToken,
            valueType: null,
            genericParameterListing,
            primaryConstructorFunctionArgumentListing: default,
            inheritedTypeReference: TypeFacts.NotApplicable.ToTypeReference(),
            namespaceName: parserModel.CurrentNamespaceStatementNode.IdentifierToken.TextSpan.GetText(),
			referenceHashSet: new());

        parserModel.Binder.BindTypeDefinitionNode(typeDefinitionNode, compilationUnit, ref parserModel);
        parserModel.Binder.BindTypeIdentifier(identifierToken, compilationUnit, ref parserModel);
        
        parserModel.StatementBuilder.ChildList.Add(typeDefinitionNode);
        
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	typeDefinitionNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
	        
	    parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = false;
	    
	    if (storageModifierKind == StorageModifierKind.Enum)
	    {
	    	ParseTypes.HandleEnumDefinitionNode(typeDefinitionNode, compilationUnit, ref parserModel);
	    	return;
	    }
    
    	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
    	{
    		ParseTypes.HandlePrimaryConstructorDefinition(
		        typeDefinitionNode,
		        compilationUnit,
		        ref parserModel);
    	}
    	
    	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
    	{
    		_ = parserModel.TokenWalker.Consume(); // Consume the ColonToken
            var inheritedTypeClauseNode = parserModel.TokenWalker.MatchTypeClauseNode(compilationUnit, ref parserModel);
            parserModel.Binder.BindTypeClauseNode(inheritedTypeClauseNode, compilationUnit, ref parserModel);
			typeDefinitionNode.SetInheritedTypeReference(new TypeReference(inheritedTypeClauseNode));
			
			while (!parserModel.TokenWalker.IsEof)
			{
				if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
				{
					_ = parserModel.TokenWalker.Consume(); // Consume the CommaToken
				
					var consumeCounter = parserModel.TokenWalker.ConsumeCounter;
					
            		parserModel.Binder.BindTypeClauseNode(
            			parserModel.TokenWalker.MatchTypeClauseNode(compilationUnit, ref parserModel),
            			compilationUnit,
            			ref parserModel);
            		
            		if (consumeCounter == parserModel.TokenWalker.ConsumeCounter)
            			break;
				}
				else
				{
					break;
				}
			}
    	}
    	
    	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.WhereTokenContextualKeyword)
    	{
    		parserModel.ExpressionList.Add((SyntaxKind.OpenBraceToken, null));
			var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
    	}
    	
    	if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
			parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
    }

    public static void HandleClassTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        HandleStorageModifierTokenKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleNamespaceTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var namespaceKeywordToken = parserModel.TokenWalker.Consume();
    	
    	var namespaceIdentifier = ParseOthers.HandleNamespaceIdentifier(compilationUnit, ref parserModel, isNamespaceStatement: true);

        if (!namespaceIdentifier.ConstructorWasInvoked)
        {
            // compilationUnit.DiagnosticBag.ReportTodoException(namespaceKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }

        var namespaceStatementNode = new NamespaceStatementNode(
            namespaceKeywordToken,
            (SyntaxToken)namespaceIdentifier,
            null);

        parserModel.Binder.SetCurrentNamespaceStatementNode(namespaceStatementNode, compilationUnit, ref parserModel);
        
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	namespaceStatementNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
	    
	    // Do not set 'IsImplicitOpenCodeBlockTextSpan' for namespace file scoped.
    }

    public static void HandleReturnTokenKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var returnKeywordToken = parserModel.TokenWalker.Consume();
   	 var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
   	 var returnStatementNode = new ReturnStatementNode(returnKeywordToken, expressionNode);
    	
		parserModel.StatementBuilder.ChildList.Add(returnStatementNode);
	}
}
