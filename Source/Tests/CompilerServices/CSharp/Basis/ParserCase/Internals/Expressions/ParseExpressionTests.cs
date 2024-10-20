using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals.Expressions;

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
				// 1 + 1
				Fabricate.Number("1"),
				Fabricate.Plus(),
				Fabricate.Number("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
			
		var expression = Parser_TEST.ParseExpression(session);
		
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
				// 1 + 1 + 1
				Fabricate.Number("1"),
				Fabricate.Plus(),
				Fabricate.Number("1"),
				Fabricate.Plus(),
				Fabricate.Number("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
			
		var expression = Parser_TEST.ParseExpression(session);
		
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
				// 1 - 1
				Fabricate.Number("1"),
				Fabricate.Minus(),
				Fabricate.Number("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
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
				// 1 * 1
				Fabricate.Number("1"),
				Fabricate.Star(),
				Fabricate.Number("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
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
				// 1 / 1
				Fabricate.Number("1"),
				Fabricate.Division(),
				Fabricate.Number("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
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
				// 1 == 1
				Fabricate.Number("1"),
				Fabricate.EqualsEquals(),
				Fabricate.Number("1"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
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
				// "Asd" + "Fgh"
				Fabricate.String("Asd"),
				Fabricate.Plus(),
				Fabricate.String("Fgh"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
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
				// 'a' + '\n'
				Fabricate.Char("a"),
				Fabricate.Plus(),
				Fabricate.Char("\n"),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
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
				// false == true
				Fabricate.False(),
				Fabricate.EqualsEquals(),
				Fabricate.True(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
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
				// (7)
				Fabricate.OpenParenthesis(),
				Fabricate.Number("7"),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)expression;
		var textTypeClause = "int";
		
		Assert.Equal(textTypeClause, parenthesizedExpressionNode.InnerExpression.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
		var literalExpressionNode = (LiteralExpressionNode)parenthesizedExpressionNode.InnerExpression;
		Assert.Equal("7", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		
		Assert.True(parenthesizedExpressionNode.OpenParenthesisToken.ConstructorWasInvoked);
    }
    
    [Fact]
    public void ShortCircuit()
    {
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// (1 + )
				Fabricate.OpenParenthesis(),
				Fabricate.Number("1"),
				Fabricate.Plus(),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)expression;
		var textTypeClause = "int";
		
		Assert.Equal(textTypeClause, parenthesizedExpressionNode.InnerExpression.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
		var binaryExpressionNode = (BinaryExpressionNode)parenthesizedExpressionNode.InnerExpression;
		
		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
		var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
		Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
		var rightEmptyExpressionNode = (EmptyExpressionNode)binaryExpressionNode.RightExpressionNode;
		Assert.Equal(textTypeClause, rightEmptyExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void ExplicitCastNode_IdentifierToken()
    {
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// (MyClass)
				Fabricate.OpenParenthesis(),
				Fabricate.Identifier("MyClass"),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var parenthesizedExpressionNode = (ExplicitCastNode)expression;
    }
    
    [Fact]
    public void ExplicitCastNode_KeywordToken()
    {
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// (int)
				Fabricate.OpenParenthesis(),
				Fabricate.Int(),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var parenthesizedExpressionNode = (ExplicitCastNode)expression;
    }
    
    [Fact]
    public void FunctionInvocationNode_A()
    {
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// MyMethod()
				Fabricate.Identifier("MyMethod"),
				Fabricate.OpenParenthesis(),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var functionInvocationNode = (FunctionInvocationNode)expression;
		
		Assert.True(functionInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
		Assert.True(functionInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		
		Assert.Empty(functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
    }
    
    [Fact]
    public void FunctionInvocationNode_B()
    {
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// MyMethod(7, "Asdfg")
				Fabricate.Identifier("MyMethod"),
				Fabricate.OpenParenthesis(),
				/**/Fabricate.Number("7"),
				/**/Fabricate.Comma(),
				/**/Fabricate.String("Asdfg"),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var functionInvocationNode = (FunctionInvocationNode)expression;

		// FunctionParametersListingNode
		{
			Assert.True(functionInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
			
			var numericFunctionParameterEntryNode = functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList[0];
			Assert.Equal(SyntaxKind.NumericLiteralToken, ((LiteralExpressionNode)numericFunctionParameterEntryNode.ExpressionNode).LiteralSyntaxToken.SyntaxKind);
			
			var stringFunctionParameterEntryNode = functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList[1];
			Assert.Equal(SyntaxKind.StringLiteralToken, ((LiteralExpressionNode)stringFunctionParameterEntryNode.ExpressionNode).LiteralSyntaxToken.SyntaxKind);
			
			Assert.True(functionInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
    }
    
    [Fact]
    public void FunctionInvocationNode_C()
    {
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// MyMethod<int, MyClass>()
				Fabricate.Identifier("MyMethod"),
				Fabricate.OpenAngleBracket(),
				/**/Fabricate.Int(),
				/**/Fabricate.Comma(),
				/**/Fabricate.Identifier("MyClass"),
				Fabricate.CloseAngleBracket(),
				Fabricate.OpenParenthesis(),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var functionInvocationNode = (FunctionInvocationNode)expression;

		// GenericParametersListingNode
		{
			Assert.NotNull(functionInvocationNode.GenericParametersListingNode);
			Assert.True(functionInvocationNode.GenericParametersListingNode.OpenAngleBracketToken.ConstructorWasInvoked);
			
			var intGenericParameterEntryNode = functionInvocationNode.GenericParametersListingNode.GenericParameterEntryNodeList[0];
			Assert.Equal("int", intGenericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			
			var stringGenericParameterEntryNode = functionInvocationNode.GenericParametersListingNode.GenericParameterEntryNodeList[1];
			Assert.Equal("MyClass", stringGenericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			
			Assert.True(functionInvocationNode.GenericParametersListingNode.CloseAngleBracketToken.ConstructorWasInvoked);
		}
		
		Assert.True(functionInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
		Assert.True(functionInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
    }
    
    [Fact]
    public void FunctionInvocationNode_D()
    {
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// MyMethod<int, MyClass>(7, "Asdfg")
				Fabricate.Identifier("MyMethod"),
				Fabricate.OpenAngleBracket(),
				/**/Fabricate.Int(),
				/**/Fabricate.Comma(),
				/**/Fabricate.Identifier("MyClass"),
				Fabricate.CloseAngleBracket(),
				Fabricate.OpenParenthesis(),
				/**/Fabricate.Number("7"),
				/**/Fabricate.Comma(),
				/**/Fabricate.String("Asdfg"),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var functionInvocationNode = (FunctionInvocationNode)expression;
		
		// GenericParametersListingNode
		{
			Assert.NotNull(functionInvocationNode.GenericParametersListingNode);
			Assert.True(functionInvocationNode.GenericParametersListingNode.OpenAngleBracketToken.ConstructorWasInvoked);
			
			var intGenericParameterEntryNode = functionInvocationNode.GenericParametersListingNode.GenericParameterEntryNodeList[0];
			Assert.Equal("int", intGenericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			
			var stringGenericParameterEntryNode = functionInvocationNode.GenericParametersListingNode.GenericParameterEntryNodeList[1];
			Assert.Equal("MyClass", stringGenericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			
			Assert.True(functionInvocationNode.GenericParametersListingNode.CloseAngleBracketToken.ConstructorWasInvoked);
		}
		
		// FunctionParametersListingNode
		{
			Assert.True(functionInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
			
			var numericFunctionParameterEntryNode = functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList[0];
			Assert.Equal(SyntaxKind.NumericLiteralToken, ((LiteralExpressionNode)numericFunctionParameterEntryNode.ExpressionNode).LiteralSyntaxToken.SyntaxKind);
			
			var stringFunctionParameterEntryNode = functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList[1];
			Assert.Equal(SyntaxKind.StringLiteralToken, ((LiteralExpressionNode)stringFunctionParameterEntryNode.ExpressionNode).LiteralSyntaxToken.SyntaxKind);
			
			Assert.True(functionInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
    }
    
    [Fact]
    public void ConstructorInvocationNode_A()
    {
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// new Person()
				Fabricate.New(),
				Fabricate.Identifier("Person"),
				Fabricate.OpenParenthesis(),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)expression;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void ConstructorInvocationNode_B()
    {
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// new Person(18, "John")
				Fabricate.New(),
				Fabricate.Identifier("Person"),
				Fabricate.OpenParenthesis(),
				/**/Fabricate.Number("18"),
				/**/Fabricate.String("John"),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)expression;
    	
    	throw new NotImplementedException();
    }
    
    [Fact]
    public void ConstructorInvocationNode_C()
    {
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// new Dictionary<int, Person>()
				Fabricate.New(),
				Fabricate.Identifier("Dictionary"),
				Fabricate.OpenAngleBracket(),
				/**/Fabricate.Int(),
				/**/Fabricate.Identifier("Person"),
				Fabricate.CloseAngleBracket(),
				Fabricate.OpenParenthesis(),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)expression;
		
        throw new NotImplementedException();
    }
    
    [Fact]
    public void ConstructorInvocationNode_D()
    {
    	// The constructor parameters are nonsensical and just exist for the sake of this test case.
    	var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// new Dictionary<int, Person>(0, "Test")
				Fabricate.New(),
				Fabricate.Identifier("Dictionary"),
				Fabricate.OpenAngleBracket(),
				/**/Fabricate.Int(),
				/**/Fabricate.Identifier("Person"),
				Fabricate.CloseAngleBracket(),
				Fabricate.OpenParenthesis(),
				/**/Fabricate.Number("0"),
				/**/Fabricate.String("Test"),
				Fabricate.CloseParenthesis(),
			},
			expressionStack: new Stack<ISyntax>(),
			shortCircuitList: new());
		
		var expression = Parser_TEST.ParseExpression(session);
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)expression;
        
        throw new NotImplementedException();
    }
    
    // TODO: Object initialization and Collection initialization
}
