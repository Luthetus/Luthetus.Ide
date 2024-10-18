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
    
    private class BinderTest
    {
    	/// <summary>
    	/// Returns the new primary expression which will be the passed in 'expressionPrimary'
    	/// if the parameters were not mergeable.
    	/// </summary>
    	public IExpressionNode Merge(
    		IExpressionNode expressionPrimary, ISyntaxToken token, List<ISyntaxToken> tokenList, Stack<ISyntax> expressionStack)
    	{
    		switch (expressionPrimary.SyntaxKind)
    		{
    			case SyntaxKind.EmptyExpressionNode:
    				return EmptyExpressionMerge((EmptyExpressionNode)expressionPrimary, token, tokenList, expressionStack);
    			case SyntaxKind.LiteralExpressionNode:
    				return LiteralExpressionMerge((LiteralExpressionNode)expressionPrimary, token, tokenList, expressionStack);
    			case SyntaxKind.BinaryExpressionNode:
    				return BinaryExpressionMerge((BinaryExpressionNode)expressionPrimary, token, tokenList, expressionStack);
    			case SyntaxKind.BadExpressionNode:
    				return BadExpressionMerge((BadExpressionNode)expressionPrimary, token, tokenList, expressionStack);
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), new List<ISyntax> { expressionPrimary, token });
    		};
    	}
    	
    	/// <summary>
    	/// Returns the new primary expression which will be the passed in 'expressionPrimary'
    	/// if the parameters were not mergeable.
    	/// </summary>
    	public IExpressionNode Merge(
    		IExpressionNode expressionPrimary, IExpressionNode expressionSecondary, List<ISyntaxToken> tokenList, Stack<ISyntax> expressionStack)
    	{
    		switch (expressionPrimary.SyntaxKind)
    		{
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), new List<ISyntax> { expressionPrimary, expressionSecondary });
    		};
    	}
    	
    	public IExpressionNode EmptyExpressionMerge(
    		EmptyExpressionNode emptyExpressionNode, ISyntaxToken token, List<ISyntaxToken> tokenList, Stack<ISyntax> expressionStack)
    	{
    		switch (token.SyntaxKind)
    		{
    			case SyntaxKind.NumericLiteralToken:
    			case SyntaxKind.StringLiteralToken:
    			case SyntaxKind.CharLiteralToken:
    				TypeClauseNode tokenTypeClauseNode;
    				
    				if (token.SyntaxKind == SyntaxKind.NumericLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.Int.ToTypeClause();
    				else if (token.SyntaxKind == SyntaxKind.StringLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.String.ToTypeClause();
    				else if (token.SyntaxKind == SyntaxKind.CharLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.Char.ToTypeClause();
    				else
    					goto default;
    					
    				return new LiteralExpressionNode(token, tokenTypeClauseNode);
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), new List<ISyntax> { emptyExpressionNode, token });
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
    		LiteralExpressionNode literalExpressionNode, ISyntaxToken token, List<ISyntaxToken> tokenList, Stack<ISyntax> expressionStack)
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
    				return new BinaryExpressionNode(literalExpressionNode, binaryOperatorNode, new EmptyExpressionNode(typeClauseNode));
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), new List<ISyntax> { literalExpressionNode, token });
    		}
    	}
    	
    	public IExpressionNode BinaryExpressionMerge(
    		BinaryExpressionNode binaryExpressionNode, ISyntaxToken token, List<ISyntaxToken> tokenList, Stack<ISyntax> expressionStack)
    	{
    		switch (token.SyntaxKind)
    		{
    			case SyntaxKind.NumericLiteralToken:
    			case SyntaxKind.StringLiteralToken:
    			case SyntaxKind.CharLiteralToken:
    				TypeClauseNode tokenTypeClauseNode;
    				
    				if (token.SyntaxKind == SyntaxKind.NumericLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.Int.ToTypeClause();
    				else if (token.SyntaxKind == SyntaxKind.StringLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.String.ToTypeClause();
    				else if (token.SyntaxKind == SyntaxKind.CharLiteralToken)
    					tokenTypeClauseNode = CSharpFacts.Types.Char.ToTypeClause();
    				else
    					goto default;
    					
    				var tokenTypeClauseNodeText = tokenTypeClauseNode.TypeIdentifierToken.TextSpan.GetText();
    			
    				var leftExpressionTypeClauseNodeText = binaryExpressionNode.LeftExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText();
    			
    				if (binaryExpressionNode.RightExpressionNode.SyntaxKind == SyntaxKind.EmptyExpressionNode &&
    					leftExpressionTypeClauseNodeText == tokenTypeClauseNodeText)
    				{
    					var rightExpressionNode = new LiteralExpressionNode(token, tokenTypeClauseNode);
    					binaryExpressionNode.SetRightExpressionNode(rightExpressionNode);
	    				return binaryExpressionNode;
    				}
    				else
    				{
    					goto default;
    				}
    			default:
    				return new BadExpressionNode(CSharpFacts.Types.Void.ToTypeClause(), new List<ISyntax> { binaryExpressionNode, token });
    		}
    	}
    	
    	public IExpressionNode BadExpressionMerge(
    		BadExpressionNode badExpressionNode, ISyntaxToken token, List<ISyntaxToken> tokenList, Stack<ISyntax> expressionStack)
    	{
    		badExpressionNode.SyntaxList.Add(token);
    		return badExpressionNode;
    	}
    }
    
    private static IExpressionNode ParseExpression(List<ISyntaxToken> tokenList, Stack<ISyntax> expressionStack)
    {
    	var binder = new BinderTest();
    	IExpressionNode expressionPrimary = new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
    	int position = 0;
    	
    	while (position < tokenList.Count)
    	{
    		var tokenCurrent = tokenList[position];
    		expressionPrimary = binder.Merge(expressionPrimary, tokenCurrent, tokenList, expressionStack);
    		position++;
    	}
    	
    	return expressionPrimary;
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
		var tokenList = new List<ISyntaxToken>
		{
			NumberFabricate("1"),
			PlusFabricate(),
			NumberFabricate("1"),
		};
		var expressionStack = new Stack<ISyntax>();
		var expression = ParseExpression(tokenList, expressionStack);
		
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
    public void Numeric_Subtract_BinaryExpressionNode()
    {
		var tokenList = new List<ISyntaxToken>
		{
			NumberFabricate("1"),
			MinusFabricate(),
			NumberFabricate("1"),
		};
		var expressionStack = new Stack<ISyntax>();
		var expression = ParseExpression(tokenList, expressionStack);
		
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
		var tokenList = new List<ISyntaxToken>
		{
			NumberFabricate("1"),
			StarFabricate(),
			NumberFabricate("1"),
		};
		var expressionStack = new Stack<ISyntax>();
		var expression = ParseExpression(tokenList, expressionStack);
		
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
		var tokenList = new List<ISyntaxToken>
		{
			NumberFabricate("1"),
			DivisionFabricate(),
			NumberFabricate("1"),
		};
		var expressionStack = new Stack<ISyntax>();
		var expression = ParseExpression(tokenList, expressionStack);
		
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
		var tokenList = new List<ISyntaxToken>
		{
			NumberFabricate("1"),
			EqualsEqualsFabricate(),
			NumberFabricate("1"),
		};
		var expressionStack = new Stack<ISyntax>();
		var expression = ParseExpression(tokenList, expressionStack);
		
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
		var tokenList = new List<ISyntaxToken>
		{
			StringFabricate("Asd"),
			PlusFabricate(),
			StringFabricate("Fgh"),
		};
		var expressionStack = new Stack<ISyntax>();
		var expression = ParseExpression(tokenList, expressionStack);
		
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
		var tokenList = new List<ISyntaxToken>
		{
			CharFabricate("a"),
			PlusFabricate(),
			CharFabricate("\n"),
		};
		var expressionStack = new Stack<ISyntax>();
		var expression = ParseExpression(tokenList, expressionStack);
		
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
}
