using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseExpressionTests
{
	/// <summary>
	/// Goal: Rewrite expression parsing
	/// ================================
	///
	/// Short description:
	/// ------------------
	/// - Use a 'while' loop rather than a recursive method.
	/// - A parent expression can "short circuit"(wording?) a child expression.
	/// - Consider the role of the main 'Parse(...)' method.
	///
	/// Long description:
	/// -----------------
	/// - Use a 'while' loop rather than a recursive method.
	/// 	asd
	/// - A parent expression can "short circuit"(wording?) a child expression.
	/// 	Given: '( 1 + );'
	/// 	Then:
	/// 		'(' opens an expression (likely a parenthesized expression),
	/// 		'1 +' is read as a binary expression node,
	/// 		But, when the right operand is attempted to be read, a ')' is read instead.
	/// 		Since ')' is the closing delimiter to the initial '(',
	/// 			then fill the remainder of the binary expression with fabricated nodes,
	/// 			and then set the inner expression for the parenthesized expression
	/// 			to be the binary expression.
	/// - Consider the role of the main 'Parse(...)' method.
	/// 	There is a complication where the 'Parse(...)' method needs to
	/// 		identify a syntax, but not duplicate the code.
	/// 	What I just wrote was my attempt at wording the issue.
	/// 		I need to continue thinking about this. I think it is
	/// 		vitally important that I find my words for this idea.
	/// 	Consider if I typed the statement: '2;'
	/// 		The C# compiler will give me an error, because
	/// 		Literal expressions are not allowed to be statements.
	/// 		The fact that the C# compiler gives me an error however,
	/// 		does not stop a user from typing this into the editor.
	/// 		So, the parser needs to handle this case, and recover when
	/// 		it reads the semicolon, then move on to the next statement as if all is fine.
	/// 	On a related note, consider I type the statement: 'var x = 2;'
	/// 		The parser will see 'var', so it invokes something named similarly to 'ParseVarContextualKeyword(...)'.
	/// 		Then 'ParseVarContextualKeyword(...)' sees an IdentifierToken follows the 'VarContextualKeywordToken'.
	/// 		The '{IdentifierToken} {IdentifierToken}' pattern in this situation is nonsensical,
	/// 			because there is no TypeDeclarationNode with an IdentifierToken of text: "var".
	/// 			So the 'var' is presumed to be the implicit TypeClauseNode that comes from the 'var' contextual keyword.
	/// 		To skip ahead to the point... the text 'var x =' results in a variable declaration node,
	/// 			and a variable assignment node.
	/// 		But, now we know we need to parse an expression. To make the point more clearly,
	/// 			we just as in the previous example need to parse the literal expression '2'.
	/// 		Except now we aren't coming directly from the 'Parse(...)' method,
	/// 			we are coming from the 'ParseVarContextualKeyword(...)' method.
	/// 	Okay, I think I know what to do.
	/// 		There is no 'ParseLiteralToken(...)', instead one immediately turns to 'ParseExpression(...)'.
	/// 	Otherwise, when given '2;', you go from
	/// 		'Parse(...)' to
	/// 		'ParseLiteralToken(...)' to
	/// 		'ParseExpression'.
	/// 	This contrasts to being given 'var x = 2;'
	/// 		'ParseVarContextualKeyword(...)' to
	/// 		'ParseIdentifierToken(...)' to
	/// 		'ParseVariableAssignment(...)' to
	/// 		'ParseExpression(...)' to
	/// 	The methods I wrote are not an exact replication of what happens in the parser at the moment.
	/// 		But, they illustrate that 'ParseLiteralToken(...)' is NOT being invoked for the 'var x = 2;' input.
	/// 		Therefore, any logic tied to 'ParseLiteralToken(...)' is likely irrelevant, or it duplicates code
	/// 		that also exists in 'ParseExpression(...)'.
	/// 	I feel like the example I gave comes across as quite underwhelming.
	/// 		But this situation is all throughout the current parser code.
	/// 		Every syntax that can appear as an expression, has its own standalone method as well.
	/// 		Eventually I started to just invoke 'ParseExpression(...)' from the standalone method.
	/// 		And the standalone method essentially did nothing of its own.
	/// 	If I add object initialization, and collection initialization,
	/// 		I need to make sure the code doesn't get duplicated in a standlone method
	/// 		and in 'ParseExpression(...)'.
	/// 	The other consideration is, does the main 'Parse(...)' loop only ever start and end a syntax?
	/// 		i.e.: either you are starting a statement, or at a statement delimiter?
	/// </summary>
	[Fact]
	public void Aaa()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
var aaa = 1;
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];
        
        var identifierToken = (IdentifierToken)variableAssignmentExpressionNode.ChildList[0];
        var equalsToken = (EqualsToken)variableAssignmentExpressionNode.ChildList[1];
        
        var literalExpressionNode = (LiteralExpressionNode)variableAssignmentExpressionNode.ChildList[2];
    }
    
    private static TextEditorTextSpan TextSpanFabricate(string text)
    {
    	return new TextEditorTextSpan(
            0,
		    0,
		    0,
		    ResourceUri.Empty,
		    text,
		    text);
    }
    
    private static NumericLiteralToken NumberFabricate(string text)
    {
    	return new NumericLiteralToken(TextSpanFabricate(text));
    }
    
    private static StringLiteralToken StringFabricate(string text)
    {
    	return new StringLiteralToken(TextSpanFabricate(text));
    }
    
    private static CharLiteralToken CharFabricate(string text)
    {
    	return new CharLiteralToken(TextSpanFabricate(text));
    }
    
    private static KeywordToken FalseFabricate()
    {
    	return new KeywordToken(TextSpanFabricate("true"), SyntaxKind.FalseTokenKeyword);
    }
    
    private static KeywordToken TrueFabricate()
    {
    	return new KeywordToken(TextSpanFabricate("false"), SyntaxKind.TrueTokenKeyword);
    }
    
    private static PlusToken PlusFabricate()
    {
    	return new PlusToken(TextSpanFabricate("+"));
    }
    
    private static MinusToken MinusFabricate()
    {
    	return new MinusToken(TextSpanFabricate("-"));
    }
    
    private static StarToken StarFabricate()
    {
    	return new StarToken(TextSpanFabricate("*"));
    }
    
    private static DivisionToken DivisionFabricate()
    {
    	return new DivisionToken(TextSpanFabricate("/"));
    }
    
    private static EqualsEqualsToken EqualsEqualsFabricate()
    {
    	return new EqualsEqualsToken(TextSpanFabricate("=="));
    }
    
    private static OpenParenthesisToken OpenParenthesisFabricate()
    {
    	return new OpenParenthesisToken(TextSpanFabricate("("));
    }
    
    private static CloseParenthesisToken CloseParenthesisFabricate()
    {
    	return new CloseParenthesisToken(TextSpanFabricate(")"));
    }
    
    private class BinderTest
    {
    	/// <summary>
    	/// Returns the new primary expression which will be the passed in 'expressionPrimary'
    	/// if the parameters were not mergeable.
    	/// </summary>
    	public IExpressionNode Merge(
    		IExpressionNode expressionPrimary, ISyntaxToken token, ExpressionSession session)
    	{
    		switch (expressionPrimary.SyntaxKind)
    		{
    			case SyntaxKind.EmptyExpressionNode:
    				return EmptyExpressionMerge((EmptyExpressionNode)expressionPrimary, token, session);
    			case SyntaxKind.LiteralExpressionNode:
    				return LiteralExpressionMerge((LiteralExpressionNode)expressionPrimary, token, session);
    			case SyntaxKind.BinaryExpressionNode:
    				return BinaryExpressionMerge((BinaryExpressionNode)expressionPrimary, token, session);
    			case SyntaxKind.ParenthesizedExpressionNode:
    				return ParenthesizedExpressionMerge((ParenthesizedExpressionNode)expressionPrimary, token, session);
    			case SyntaxKind.BadExpressionNode:
    				return BadExpressionMerge((BadExpressionNode)expressionPrimary, token, session);
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), expressionPrimary, token);
    		};
    	}
    	
    	/// <summary>
    	/// Returns the new primary expression which will be the passed in 'expressionPrimary'
    	/// if the parameters were not mergeable.
    	/// </summary>
    	public IExpressionNode Merge(
    		IExpressionNode expressionPrimary, IExpressionNode expressionSecondary, ExpressionSession session)
    	{
    		switch (expressionPrimary.SyntaxKind)
    		{
    			case SyntaxKind.ParenthesizedExpressionNode:
    				return ParenthesizedExpressionMerge((ParenthesizedExpressionNode)expressionPrimary, expressionSecondary, session);
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), expressionPrimary, expressionSecondary);
    		};
    	}
    	
    	public IExpressionNode EmptyExpressionMerge(
    		EmptyExpressionNode emptyExpressionNode, ISyntaxToken token, ExpressionSession session)
    	{
    		switch (token.SyntaxKind)
    		{
    			case SyntaxKind.NumericLiteralToken:
    			case SyntaxKind.StringLiteralToken:
    			case SyntaxKind.CharLiteralToken:
    			case SyntaxKind.FalseTokenKeyword:
    			case SyntaxKind.TrueTokenKeyword:
    				TypeClauseNode tokenTypeClauseNode;
    				
    				if (token.SyntaxKind == SyntaxKind.NumericLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.Int.ToTypeClause();
    				else if (token.SyntaxKind == SyntaxKind.StringLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.String.ToTypeClause();
    				else if (token.SyntaxKind == SyntaxKind.CharLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.Char.ToTypeClause();
    				else if (token.SyntaxKind == SyntaxKind.FalseTokenKeyword || token.SyntaxKind == SyntaxKind.TrueTokenKeyword)
    					tokenTypeClauseNode = CSharpFacts.Types.Bool.ToTypeClause();
    				else
    					goto default;
    					
    				return new LiteralExpressionNode(token, tokenTypeClauseNode);
    			case SyntaxKind.OpenParenthesisToken:
    				var parenthesizedExpressionNode = new ParenthesizedExpressionNode((OpenParenthesisToken)token, CSharpFacts.Types.Void.ToTypeClause());
    				session.ShortCircuitList.Add((SyntaxKind.CloseParenthesisToken, parenthesizedExpressionNode));
    				return new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), emptyExpressionNode, token);
    		}
    	}
    	
    	/// <summary>
    	/// I am not evaluating anything when parsing for the IDE, so for now I'm going to ignore the precedence,
    	/// and just track the start and end of the expression more or less.
    	///
    	/// Reason for this being: object initialization and collection initialization
    	/// currently will at times break the Parser for an entire file, and therefore
    	/// they are much higher priority.
    	/// </summary>
    	public IExpressionNode LiteralExpressionMerge(
    		LiteralExpressionNode literalExpressionNode, ISyntaxToken token, ExpressionSession session)
    	{
    		switch (token.SyntaxKind)
    		{
    			case SyntaxKind.PlusToken:
    			case SyntaxKind.MinusToken:
    			case SyntaxKind.StarToken:
			    case SyntaxKind.DivisionToken:
			    case SyntaxKind.EqualsEqualsToken:
    				var typeClauseNode = literalExpressionNode.ResultTypeClauseNode;
    				var binaryOperatorNode = new BinaryOperatorNode(typeClauseNode, token, typeClauseNode, typeClauseNode);
    				return new BinaryExpressionNode(literalExpressionNode, binaryOperatorNode);
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), literalExpressionNode, token);
    		}
    	}
    	
    	public IExpressionNode BinaryExpressionMerge(
    		BinaryExpressionNode binaryExpressionNode, ISyntaxToken token, ExpressionSession session)
    	{
    		switch (token.SyntaxKind)
    		{
    			case SyntaxKind.NumericLiteralToken:
    			case SyntaxKind.StringLiteralToken:
    			case SyntaxKind.CharLiteralToken:
    			case SyntaxKind.FalseTokenKeyword:
    			case SyntaxKind.TrueTokenKeyword:
    				TypeClauseNode tokenTypeClauseNode;
    				
    				if (token.SyntaxKind == SyntaxKind.NumericLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.Int.ToTypeClause();
    				else if (token.SyntaxKind == SyntaxKind.StringLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.String.ToTypeClause();
    				else if (token.SyntaxKind == SyntaxKind.CharLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.Char.ToTypeClause();
    				else if (token.SyntaxKind == SyntaxKind.FalseTokenKeyword || token.SyntaxKind == SyntaxKind.TrueTokenKeyword)
    					tokenTypeClauseNode = CSharpFacts.Types.Bool.ToTypeClause();
    				else
    					goto default;
    					
    				var tokenTypeClauseNodeText = tokenTypeClauseNode.TypeIdentifierToken.TextSpan.GetText();
    			
    				var leftExpressionTypeClauseNodeText = binaryExpressionNode.LeftExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText();
    				if (leftExpressionTypeClauseNodeText != tokenTypeClauseNodeText)
    					goto default;
    			
					var rightExpressionNode = new LiteralExpressionNode(token, tokenTypeClauseNode);
					binaryExpressionNode.SetRightExpressionNode(rightExpressionNode);
    				return binaryExpressionNode;
    			case SyntaxKind.PlusToken:
    			case SyntaxKind.MinusToken:
    			case SyntaxKind.StarToken:
			    case SyntaxKind.DivisionToken:
			    case SyntaxKind.EqualsEqualsToken:
    				// TODO: More generally, the result will be a number, so all that matters is what operators a number can interact with instead of duplicating this code.
    				if (binaryExpressionNode.RightExpressionNode.SyntaxKind != SyntaxKind.EmptyExpressionNode)
		    		{
		    			var typeClauseNode = binaryExpressionNode.ResultTypeClauseNode;
	    				var binaryOperatorNode = new BinaryOperatorNode(typeClauseNode, token, typeClauseNode, typeClauseNode);
	    				return new BinaryExpressionNode(binaryExpressionNode, binaryOperatorNode, new EmptyExpressionNode(typeClauseNode));
		    		}
		    		else
		    		{
		    			goto default;
		    		}
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), binaryExpressionNode, token);
    		}
    	}
    	
    	public IExpressionNode ParenthesizedExpressionMerge(
    		ParenthesizedExpressionNode parenthesizedExpressionNode, ISyntaxToken token, ExpressionSession session)
    	{
    		switch (token.SyntaxKind)
    		{
    			case SyntaxKind.CloseParenthesisToken:
    				return parenthesizedExpressionNode.SetCloseParenthesisToken((CloseParenthesisToken)token);
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), parenthesizedExpressionNode, token);
    		}
    	}
    	
    	public IExpressionNode ParenthesizedExpressionMerge(
    		ParenthesizedExpressionNode parenthesizedExpressionNode, IExpressionNode expressionSecondary, ExpressionSession session)
    	{
    		if (parenthesizedExpressionNode.InnerExpression.SyntaxKind == SyntaxKind.EmptyExpressionNode)
    			return parenthesizedExpressionNode.SetInnerExpression(expressionSecondary);
    			
    		return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), parenthesizedExpressionNode, expressionSecondary);
    	}
    	
    	public IExpressionNode BadExpressionMerge(
    		BadExpressionNode badExpressionNode, ISyntaxToken token, ExpressionSession session)
    	{
    		badExpressionNode.SyntaxList.Add(token);
    		return badExpressionNode;
    	}
    }
    
    private static IExpressionNode ParseExpression(ExpressionSession session)
    {
    	var binder = new BinderTest();
    	IExpressionNode expressionPrimary = new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
    	int position = 0;
    	
    	while (position < session.TokenList.Count)
    	{
    		var tokenCurrent = session.TokenList[position];
    		if (tokenCurrent.SyntaxKind == SyntaxKind.StatementDelimiterToken)
    			break;
    		
    		// Check if the tokenCurrent is a token that is used as a end-delimiter
    		// before iterating the list?
    		if (tokenCurrent.SyntaxKind == SyntaxKind.CloseParenthesisToken)
    		{
    			for (int i = session.ShortCircuitList.Count - 1; i > -1; i--)
	    		{
	    			var tuple = session.ShortCircuitList[i];
	    			
	    			if (tuple.DelimiterSyntaxKind == tokenCurrent.SyntaxKind)
	    			{
	    				session.ShortCircuitList.RemoveRange(i, session.ShortCircuitList.Count - i);
	    				
		    			var expressionSecondary = expressionPrimary;
		    			expressionPrimary = tuple.ExpressionNode;
		    			expressionPrimary = binder.Merge(expressionPrimary, expressionSecondary, session);
		    			break;
	    			}
	    		}
    		}
    		
    		expressionPrimary = binder.Merge(expressionPrimary, tokenCurrent, session);

    		position++;
    		
    		/*
    		I need to add the recursive part of this.
    		I ignored operator precedence for now.
    		
    		But, something about ParenthesizedExpressionNode is resulting
    		in me needing the recursion.
    		
    		Presumably operator precedence would also require recursion,
    		and that 'ParenthesizedExpressionNode'
    		is a case where recursion is unavoidable?
    		
    		Because I have to leave behind an unfinished ParenthesizedExpressionNode
    		after I parse the OpenParenthesisToken.
    		
    		Then, only when I've parsed the inner expression (or short circuited)
    		I revisit the ParenthesizedExpressionNode and can set its inner expression.
    		
    		If I invoked 'ParseExpression(...)' recursively,
    		once I had the OpenParenthesisToken,
    		I could say:
    			'var innerExpression = ParseExpression(...);'
    		
    		But, I think it would be better for optimization, and readability
    		if I go about "primitive recursion" via a while loop here.
    		
    		A Stack<ISyntaxNode> of unresolved nodes might be useful.
    		I could push the unfinished ParenthesizedExpressionNode
    		onto it, then handle parsing the inner expression.
    		
    		Once I'm done with the inner expression, 
    		I can check the Stack<ISyntaxNode> and see that there was
    		an unresolved InnerExpression and invoke Merge(IExpressionNode, IExpressionNode).
    		
    		Perhaps I'm thinking too far ahead when I say this but...
    		I want to use a List<ISyntaxNode> because it lends itself more to the short circuiting logic.
    		Where the parent expression's delimiter appeared in the child expression,
    		therefore stop parsing the child expression,
    		and make the parent expression the 'expressionPrimary' again.
    		
    		If there were a count of 2 or more for the List<ISyntaxNode>, it is possible
    		that it isn't even a parent expression's delimiter that appeared
    		in the current expression.
    		But instead, that it was an ancestor expression that is older than the parent expression.
    		
    		In this case, I'd be "short circuiting" back to the ancestor expression and clearing
    		a few indices of the List.
    		
    		If the short circuit list stored a value tuple (DelimiterToken, Node),
    		then I could search the list to see if the current token is in one of the tuples.
    		
    		If it is, then: 'primaryExpression = tuple.Node;'
    		*/
    	}
    	
    	return expressionPrimary;
    }
    
    private class ExpressionSession
    {
		public ExpressionSession(
			List<ISyntaxToken> tokenList,
			Stack<ISyntax> expressionStack,
			List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> shortCircuitList)
		{
			TokenList = tokenList;
			ExpressionStack = expressionStack;
			ShortCircuitList = shortCircuitList;
		}

    	public List<ISyntaxToken> TokenList { get; }
		public Stack<ISyntax> ExpressionStack { get; }
		public List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> ShortCircuitList { get; }
    }
    
    [Fact]
    public void Numeric_Add_BinaryExpressionNode()
    {
		/*
		I can make a static method 'ParseExpression(...)' then ignore all the other sections of the parser
		and have my tests invoke just the one method.
		
		Then I move the method to the parser when I'm done.
		
		I want to try and make sure the method can be moved without any changes,
		but I'm open to making some changes when I'm done if it elucidates the answer for the time being.
		
		I think all I "need" is 'Stack<ISyntax> expressionStack' for the time being.
		So I can forgo thinking about the Parser as a whole for a moment.
		
		Well... I guess I need an IEnumerable<ISyntaxToken> too
		*/
		var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				NumberFabricate("1"),
				PlusFabricate(),
				NumberFabricate("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
			
		var expression = ParseExpression(session);
		
		var binaryExpressionNode = (BinaryExpressionNode)expression;
		var textTypeClause = "int";
		
		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
	    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
	    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
	    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
	    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    /// <summary>
    ///        +
    ///      /   \
    ///     /     \
    ///    /       \
    ///   +         1
    ///  / \ 
    /// 1   1
    /// </summary>
    [Fact]
    public void Numeric_Add_BinaryExpressionNode_More()
    {
		var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				NumberFabricate("1"),
				PlusFabricate(),
				NumberFabricate("1"),
				PlusFabricate(),
				NumberFabricate("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
			
		var expression = ParseExpression(session);
		
		var binaryExpressionNode = (BinaryExpressionNode)expression;
		var textTypeClause = "int";
		
		// Left Expression
		{
			var leftBinaryExpressionNode = (BinaryExpressionNode)binaryExpressionNode.LeftExpressionNode;
			Assert.Equal(textTypeClause, leftBinaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			
			// Temporarily swap variables for sanity #change
			var rememberBinaryExpressionNode = binaryExpressionNode;
			binaryExpressionNode = leftBinaryExpressionNode;
			// Inner Binary Expression
			{
				var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
				Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
				
				var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
			    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
			    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
				Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    
			    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
			    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    
			    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    }
		    
		    // Temporarily swap variables for sanity #restore
		    binaryExpressionNode = rememberBinaryExpressionNode;
		}
		
		// Operator
		{
		    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
		    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
		    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		}
	    
	    // Right Expression
	    {
		    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
		    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    }
	    
	    // Result
	    {
	    	Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    }
    }
    
    [Fact]
    public void Numeric_Subtract_BinaryExpressionNode()
    {
		var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				NumberFabricate("1"),
				MinusFabricate(),
				NumberFabricate("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = ParseExpression(session);
		
		var binaryExpressionNode = (BinaryExpressionNode)expression;
		var textTypeClause = "int";
		
		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
	    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
	    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
	    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
	    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void Numeric_Star_BinaryExpressionNode()
    {
		var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				NumberFabricate("1"),
				StarFabricate(),
				NumberFabricate("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = ParseExpression(session);
		
		var binaryExpressionNode = (BinaryExpressionNode)expression;
		var textTypeClause = "int";
		
		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
	    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
	    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
	    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
	    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void Numeric_Division_BinaryExpressionNode()
    {
		var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				NumberFabricate("1"),
				DivisionFabricate(),
				NumberFabricate("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = ParseExpression(session);
		
		var binaryExpressionNode = (BinaryExpressionNode)expression;
		var textTypeClause = "int";
		
		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
	    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
	    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
	    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
	    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void Numeric_EqualsEquals_BinaryExpressionNode()
    {
		var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				NumberFabricate("1"),
				EqualsEqualsFabricate(),
				NumberFabricate("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = ParseExpression(session);
		
		var binaryExpressionNode = (BinaryExpressionNode)expression;
		var textTypeClause = "int";
		
		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
	    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
	    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
	    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
	    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void String_BinaryExpressionNode()
    {
		var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				StringFabricate("Asd"),
				PlusFabricate(),
				StringFabricate("Fgh"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = ParseExpression(session);
		
		var binaryExpressionNode = (BinaryExpressionNode)expression;
		var textTypeClause = "string";
		
		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
	    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
	    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
	    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
	    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void Char_BinaryExpressionNode()
    {
		var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				CharFabricate("a"),
				PlusFabricate(),
				CharFabricate("\n"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = ParseExpression(session);
		
		var binaryExpressionNode = (BinaryExpressionNode)expression;
		var textTypeClause = "char";
		
		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
	    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
	    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
	    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
	    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void Bool_BinaryExpressionNode()
    {
		var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				FalseFabricate(),
				EqualsEqualsFabricate(),
				TrueFabricate(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = ParseExpression(session);
		
		var binaryExpressionNode = (BinaryExpressionNode)expression;
		var textTypeClause = "bool";
		
		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
	    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
	    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
	    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
	    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void ParenthesizedExpressionNode_Test()
    {
		var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				OpenParenthesisFabricate(),
				NumberFabricate("7"),
				CloseParenthesisFabricate(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = ParseExpression(session);
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)expression;
		var textTypeClause = "int";
		
		Assert.Equal(textTypeClause, parenthesizedExpressionNode.InnerExpression.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
		var literalExpressionNode = (LiteralExpressionNode)parenthesizedExpressionNode.InnerExpression;
		Assert.Equal("7", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		
		Assert.True(parenthesizedExpressionNode.OpenParenthesisToken.ConstructorWasInvoked);
    }
    
    [Fact]
    public void ExplicitCastNode_Test()
    {
    	throw new NotImplementedException();
    }
    
    [Fact]
    public void ShortCircuit()
    {
    	// ( 1 + );
    	
    	// ?
    	//
    	// Consume if it is your delimiter
    	// Stop, but do not consume if it is NOT your delimiter
    	
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				OpenParenthesisFabricate(),
				NumberFabricate("1"),
				PlusFabricate(),
				CloseParenthesisFabricate(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = ParseExpression(session);
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)expression;
		var textTypeClause = "int";
		
		Assert.Equal(textTypeClause, parenthesizedExpressionNode.InnerExpression.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
		var literalExpressionNode = (BinaryExpressionNode)parenthesizedExpressionNode.InnerExpression;
    	
    	throw new NotImplementedException();
    }
}
