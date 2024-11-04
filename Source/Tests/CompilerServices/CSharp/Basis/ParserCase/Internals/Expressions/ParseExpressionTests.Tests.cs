using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals.Expressions;

public partial class ParseExpressionTests
{
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
        
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
        
        var identifierToken = (IdentifierToken)variableAssignmentExpressionNode.GetChildList()[0];
        var equalsToken = (EqualsToken)variableAssignmentExpressionNode.GetChildList()[1];
        
        var literalExpressionNode = (LiteralExpressionNode)variableAssignmentExpressionNode.GetChildList()[2];
    }
    
    [Fact]
	public void VariableReferenceNode_A()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
var compilationUnit = parser.Parse();
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        throw new NotImplementedException();
    }
    
    [Fact]
	public void VariableReferenceNode_B()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
var compilationUnit = Parse().parser;
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        throw new NotImplementedException();
    }
    
    [Fact]
	public void VariableReferenceNode_C()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        throw new NotImplementedException();
    }
    
    [Fact]
	public void VariableReferenceNode_D()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
var rememberBinaryExpressionNode = binaryExpressionNode;
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Goto definition isn't working for the variable assignment that is done to a variable on an outer scope.
    /// </summary>
    [Fact]
	public void VariableReferenceNode_E()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();

{
	binaryExpressionNode = leftBinaryExpressionNode;
}
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        throw new NotImplementedException();
    }

/*
this,
out,
in,
ref,
???
*/
    
    [Fact]
    public void Numeric_Add_BinaryExpressionNode()
    {
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"1 + 1";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
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
    public void Numeric_Add_BinaryExpressionNode_More()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"1 + 1 + 1";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
			
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
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
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"1 - 1";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
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
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"1 * 1";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
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
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"1 / 1";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
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
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"1 == 1";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
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
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "\"Asd\" + \"Fgh\"";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
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
    public void String_Interpolated()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "$\"asd\"";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void String_Verbatim()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "@\"asd\"";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void String_InterpolatedVerbatim()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "$@\"asd\"";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void String_VerbatimInterpolated()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "@$\"asd\"";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void Char_BinaryExpressionNode()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "'a' + '\n'";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
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
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "false == true";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
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
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "(7)";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		var textTypeClause = "int";
		
		Assert.Equal(textTypeClause, parenthesizedExpressionNode.InnerExpression.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
		var literalExpressionNode = (LiteralExpressionNode)parenthesizedExpressionNode.InnerExpression;
		Assert.Equal("7", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		
		Assert.True(parenthesizedExpressionNode.OpenParenthesisToken.ConstructorWasInvoked);
    }
    
    [Fact]
    public void ShortCircuit()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "(1 + )";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
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
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "(MyClass)";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var explicitCastNode = (ExplicitCastNode)topCodeBlock.GetChildList().Single();
    }
    
    [Fact]
    public void ExplicitCastNode_KeywordToken()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "(int)";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var explicitCastNode = (ExplicitCastNode)topCodeBlock.GetChildList().Single();
    }
    
    [Fact]
    public void FunctionInvocationNode_Basic()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "MyMethod()";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var functionInvocationNode = (FunctionInvocationNode)topCodeBlock.GetChildList().Single();
		
		Assert.True(functionInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
		Assert.True(functionInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		
		Assert.Empty(functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
    }
    
    [Fact]
    public void FunctionInvocationNode_Parameters()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "MyMethod(7, \"Asdfg\")";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var functionInvocationNode = (FunctionInvocationNode)topCodeBlock.GetChildList().Single();

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
    public void FunctionInvocationNode_Generic()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "MyMethod<int, MyClass>()";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var functionInvocationNode = (FunctionInvocationNode)topCodeBlock.GetChildList().Single();

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
    public void FunctionInvocationNode_Generic_Parameters()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "MyMethod<int, MyClass>(7, \"Asdfg\")";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var functionInvocationNode = (FunctionInvocationNode)topCodeBlock.GetChildList().Single();
		
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
    public void ConstructorInvocationNode_Basic()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new Person()";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
		
		Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    //constructorInvocationExpressionNode.ResultTypeClauseNode;
	    
	    Assert.Null(constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode);
	    
	    Assert.NotNull(constructorInvocationExpressionNode.FunctionParametersListingNode);
		Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
		Assert.Empty(constructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
		Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
	    
	    Assert.Null(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
    }
    
    [Fact]
    public void ConstructorInvocationNode_Parameters()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new Person(18, \"John\")";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
    	
    	Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    //constructorInvocationExpressionNode.ResultTypeClauseNode;
	    
	    Assert.Null(constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode);
	    
	    Assert.NotNull(constructorInvocationExpressionNode.FunctionParametersListingNode);
		
		// FunctionParametersListingNode
		{
			Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
			Assert.Equal(2, constructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList.Count);
			Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
	    
	    Assert.Null(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
    }
    
    [Fact]
    public void ConstructorInvocationNode_Generic()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new Dictionary<int, Person>()";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
		
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // ResultTypeClauseNode.GenericParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode.OpenAngleBracketToken.ConstructorWasInvoked);
	    	Assert.Equal(2, constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode.GenericParameterEntryNodeList.Count);
	    	Assert.True(constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode.CloseAngleBracketToken.ConstructorWasInvoked);
	    }
	    
	    Assert.NotNull(constructorInvocationExpressionNode.FunctionParametersListingNode);
		Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
		Assert.Empty(constructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
		Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
	    
	    Assert.Null(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
    }
    
    [Fact]
    public void ConstructorInvocationNode_Generic_Parameters_MISSING_NumericLiteralToken_A()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
    	// The constructor parameters are nonsensical and just exist for the sake of this test case.
        var sourceText = "new Dictionary<int, Person>(0, \"Test\")";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        
        foreach (var token in lexer.SyntaxTokenList)
    	{
    		Console.WriteLine(token.SyntaxKind);
    	}
    }
    
    [Fact]
    public void ConstructorInvocationNode_Generic_Parameters_MISSING_NumericLiteralToken_B()
    {
    	/*
    	It turns out that "0" does not Lex to a NumericLiteralToken.
    	This is one of the most ridiculous bugs I've ever seen,
    	because how am I only now seeing this? How long was this an issue for???
    	
    	It doesn't Lex to anything, it just returns EndOfFileToken, or if more than "0" is in the text,
    	it skips over where the 0-token would be.
    	
    	Oh my gosh, I've checked the 'CSharpLexer.cs' and in the switch statement that
    	is within a while loop that goes over every character in the string,
    	I hardcoded cases 1...9 inclusive both ends but never '0'.
    	
    	Someone needs to take away my keyboard because I am dangerously stupid.
    	
    	TODO: Anything similar to this in the future should return a 'BadToken' or some sort.
    	*/
    
    	var resourceUri = new ResourceUri("./unitTesting.txt");
    	// The constructor parameters are nonsensical and just exist for the sake of this test case.
        var sourceText = "0";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        
        foreach (var token in lexer.SyntaxTokenList)
    	{
    		Console.WriteLine(token.SyntaxKind);
    	}
    }
    
    [Fact]
    public void ConstructorInvocationNode_Generic_Parameters()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
    	// The constructor parameters are nonsensical and just exist for the sake of this test case.
        var sourceText = "new Dictionary<int, Person>(0, \"Test\")";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // ResultTypeClauseNode.GenericParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode.OpenAngleBracketToken.ConstructorWasInvoked);
	    	
	    	Assert.Equal(2, constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode.GenericParameterEntryNodeList.Count);
	    	
	    	var intGenericParameterEntryNode = constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode.GenericParameterEntryNodeList[0];
	    	Assert.Equal("int", intGenericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    	
	    	var personGenericParameterEntryNode = constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode.GenericParameterEntryNodeList[1];
	    	Assert.Equal("Person", personGenericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    	
	    	Assert.True(constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode.CloseAngleBracketToken.ConstructorWasInvoked);
	    }

		// FunctionParametersListingNode
		{
			Assert.NotNull(constructorInvocationExpressionNode.FunctionParametersListingNode);
			Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
			Assert.Equal(2, constructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList.Count);
			Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
	    
	    Assert.Null(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
    }
    
    [Fact]
    public void ConstructorInvocationNode_NoTypeClauseNode()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new()";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    Assert.Null(constructorInvocationExpressionNode.ResultTypeClauseNode);

		// FunctionParametersListingNode
		{
			Assert.NotNull(constructorInvocationExpressionNode.FunctionParametersListingNode);
			Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
			Assert.Empty(constructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
			Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
	    
	    Assert.Null(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
    }
    
    [Fact]
    public void ObjectInitializationNode_Parameters_NoTrailingComma()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new MyClass { FirstName = firstName, LastName = lastName }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // constructorInvocationExpressionNode.ResultTypeClauseNode

		// FunctionParametersListingNode
		{
			Assert.Null(constructorInvocationExpressionNode.FunctionParametersListingNode);
		}
	    
	    // ObjectInitializationParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.OpenBraceToken.ConstructorWasInvoked);
	    	
	    	// { FirstName = firstName, LastName = lastName };
	    	{
	    		var firstNameObjectInitializationParameterEntryNode = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[0];
	    		
	    		Assert.Equal("FirstName", firstNameObjectInitializationParameterEntryNode.PropertyIdentifierToken.TextSpan.GetText());
		        Assert.True(firstNameObjectInitializationParameterEntryNode.EqualsToken.ConstructorWasInvoked);
		        
		        var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)firstNameObjectInitializationParameterEntryNode.ExpressionNode;
		        Assert.Equal("firstName", ambiguousIdentifierExpressionNode.Token.TextSpan.GetText());
	    	}
	    	{
	    		var lastNameObjectInitializationParameterEntryNode = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[1];
	    		
	    		Assert.Equal("LastName", lastNameObjectInitializationParameterEntryNode.PropertyIdentifierToken.TextSpan.GetText());
		        Assert.True(lastNameObjectInitializationParameterEntryNode.EqualsToken.ConstructorWasInvoked);
		        
		        var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)lastNameObjectInitializationParameterEntryNode.ExpressionNode;
		        Assert.Equal("lastName", ambiguousIdentifierExpressionNode.Token.TextSpan.GetText());
	    	}
	    	
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.CloseBraceToken.ConstructorWasInvoked);
	    }
    }
    
    [Fact]
    public void ObjectInitializationNode__Parameters_WithTrailingComma()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new MyClassAaa { FirstName = firstName, LastName = lastName, }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // constructorInvocationExpressionNode.ResultTypeClauseNode

		// FunctionParametersListingNode
		{
			Assert.Null(constructorInvocationExpressionNode.FunctionParametersListingNode);
		}
	    
	    // ObjectInitializationParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.OpenBraceToken.ConstructorWasInvoked);
	    	
	    	// { FirstName = firstName, LastName = lastName };
	    	{
	    		var firstNameObjectInitializationParameterEntryNode = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[0];
	    		
	    		Assert.Equal("FirstName", firstNameObjectInitializationParameterEntryNode.PropertyIdentifierToken.TextSpan.GetText());
		        Assert.True(firstNameObjectInitializationParameterEntryNode.EqualsToken.ConstructorWasInvoked);
		        
		        var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)firstNameObjectInitializationParameterEntryNode.ExpressionNode;
		        Assert.Equal("firstName", ambiguousIdentifierExpressionNode.Token.TextSpan.GetText());
	    	}
	    	{
	    		var lastNameObjectInitializationParameterEntryNode = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[1];
	    		
	    		Assert.Equal("LastName", lastNameObjectInitializationParameterEntryNode.PropertyIdentifierToken.TextSpan.GetText());
		        Assert.True(lastNameObjectInitializationParameterEntryNode.EqualsToken.ConstructorWasInvoked);
		        
		        var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)lastNameObjectInitializationParameterEntryNode.ExpressionNode;
		        Assert.Equal("lastName", ambiguousIdentifierExpressionNode.Token.TextSpan.GetText());
	    	}
	    	
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.CloseBraceToken.ConstructorWasInvoked);
	    }
    }
    
    [Fact]
    public void ObjectInitializationNode_NoParameters_NoTrailingComma()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new MyClassAaa { }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // constructorInvocationExpressionNode.ResultTypeClauseNode

		// FunctionParametersListingNode
		{
			Assert.Null(constructorInvocationExpressionNode.FunctionParametersListingNode);
		}
	    
	    // ObjectInitializationParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.OpenBraceToken.ConstructorWasInvoked);
	    	
	    	Assert.Empty(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList);
	    	
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.CloseBraceToken.ConstructorWasInvoked);
	    }
    }
    
    [Fact]
    public void ObjectInitializationNode_NoParameters_WithTrailingComma()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new MyClassAaa { , }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // constructorInvocationExpressionNode.ResultTypeClauseNode

		// FunctionParametersListingNode
		{
			Assert.Null(constructorInvocationExpressionNode.FunctionParametersListingNode);
		}
	    
	    // ObjectInitializationParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.OpenBraceToken.ConstructorWasInvoked);
	    	
	    	Assert.Empty(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList);
	    	
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.CloseBraceToken.ConstructorWasInvoked);
	    }
    }
    
    [Fact]
    public void ObjectInitializationNode_WithParenthesis()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new MyClassAaa() { }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // constructorInvocationExpressionNode.ResultTypeClauseNode

		// FunctionParametersListingNode
		{
			Assert.NotNull(constructorInvocationExpressionNode.FunctionParametersListingNode);
			Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
			Assert.Empty(constructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
			Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
	    
	    // ObjectInitializationParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.OpenBraceToken.ConstructorWasInvoked);
	    	
	    	Assert.Empty(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList);
	    	
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.CloseBraceToken.ConstructorWasInvoked);
	    }
    }
    
    [Fact]
    public void CollectionInitializationNode_Parameters_NoTrailingComma()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new List<int> { 1, 2 }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // constructorInvocationExpressionNode.ResultTypeClauseNode

		// FunctionParametersListingNode
		{
			Assert.Null(constructorInvocationExpressionNode.FunctionParametersListingNode);
		}
	    
	    // ObjectInitializationParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.OpenBraceToken.ConstructorWasInvoked);
	    	
	    	// { 1, 2 };
	    	{
	    		var firstCollectionInitializationParameterEntryNode = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[0];
				Assert.True(firstCollectionInitializationParameterEntryNode.IsCollectionInitialization);
	    		
	    		var literalExpressionNode = (LiteralExpressionNode)firstCollectionInitializationParameterEntryNode.ExpressionNode;
	    		Assert.Equal("1", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		        Assert.True(literalExpressionNode.LiteralSyntaxToken.ConstructorWasInvoked);
	    	}
	    	{
	    		var secondCollectionInitializationParameterEntryNode = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[1];
	    		Assert.True(secondCollectionInitializationParameterEntryNode.IsCollectionInitialization);
	    		
	    		var literalExpressionNode = (LiteralExpressionNode)secondCollectionInitializationParameterEntryNode.ExpressionNode;
	    		Assert.Equal("2", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		        Assert.True(literalExpressionNode.LiteralSyntaxToken.ConstructorWasInvoked);
	    	}
	    	
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.CloseBraceToken.ConstructorWasInvoked);
	    }
    }
    
    [Fact]
    public void CollectionInitializationNode_Parameters_WithTrailingComma()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new List<Person> { new Person(1, \"John\"), new(2, \"Jane\"), }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // constructorInvocationExpressionNode.ResultTypeClauseNode

		// FunctionParametersListingNode
		{
			Assert.Null(constructorInvocationExpressionNode.FunctionParametersListingNode);
		}
	    
	    // ObjectInitializationParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.OpenBraceToken.ConstructorWasInvoked);
	    	
	    	// { new Person(1, "John"), new(2, "Jane"), };
	    	//
	    	// new Person(1, "John"),
	    	{
	    		var firstCollectionInitializationParameterEntryNode = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[0];
				Assert.True(firstCollectionInitializationParameterEntryNode.IsCollectionInitialization);
	    		
	    		var innerConstructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)firstCollectionInitializationParameterEntryNode.ExpressionNode;
	    		
	    		Assert.True(innerConstructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		        // Assert.Equal(innerConstructorInvocationExpressionNode.ResultTypeClauseNode);
		        
		        // innerFunctionParametersListingNode (1, "John"),
		        {
		        	// 1,
		        	{
		        		var innerFunctionParametersListingNode = innerConstructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList[0];
		        		
		        		var literalExpressionNode = (LiteralExpressionNode)innerFunctionParametersListingNode.ExpressionNode;
		        		Assert.Equal("1", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		        		
				        Assert.False(innerFunctionParametersListingNode.HasOutKeyword);
				        Assert.False(innerFunctionParametersListingNode.HasInKeyword);
				        Assert.False(innerFunctionParametersListingNode.HasRefKeyword);
		        	}
		        	// "John"
		        	{
		        		var innerFunctionParametersListingNode = innerConstructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList[1];
		        		
		        		var literalExpressionNode = (LiteralExpressionNode)innerFunctionParametersListingNode.ExpressionNode;
		        		Assert.Equal("John", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		        		
				        Assert.False(innerFunctionParametersListingNode.HasOutKeyword);
				        Assert.False(innerFunctionParametersListingNode.HasInKeyword);
				        Assert.False(innerFunctionParametersListingNode.HasRefKeyword);
		        	}
		        }
		        
		        Assert.Null(innerConstructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	}
	    	// new(2, "Jane"),
	    	{
	    		var secondCollectionInitializationParameterEntryNode = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[1];
	    		Assert.True(secondCollectionInitializationParameterEntryNode.IsCollectionInitialization);
	    		
	    		var innerConstructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)secondCollectionInitializationParameterEntryNode.ExpressionNode;
	    		Assert.True(innerConstructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
	    		
	    		// innerFunctionParametersListingNode (2, "Jane"),
		        {
		        	// 2,
		        	{
		        		var innerFunctionParametersListingNode = innerConstructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList[0];
		        		
		        		var literalExpressionNode = (LiteralExpressionNode)innerFunctionParametersListingNode.ExpressionNode;
		        		Assert.Equal("2", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		        		
				        Assert.False(innerFunctionParametersListingNode.HasOutKeyword);
				        Assert.False(innerFunctionParametersListingNode.HasInKeyword);
				        Assert.False(innerFunctionParametersListingNode.HasRefKeyword);
		        	}
		        	// "Jane"
		        	{
		        		var innerFunctionParametersListingNode = innerConstructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList[1];
		        		
		        		var literalExpressionNode = (LiteralExpressionNode)innerFunctionParametersListingNode.ExpressionNode;
		        		Assert.Equal("Jane", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		        		
				        Assert.False(innerFunctionParametersListingNode.HasOutKeyword);
				        Assert.False(innerFunctionParametersListingNode.HasInKeyword);
				        Assert.False(innerFunctionParametersListingNode.HasRefKeyword);
		        	}
		        }
	    		
	    		//Assert.Equal("2", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		        //Assert.True(literalExpressionNode.LiteralSyntaxToken.ConstructorWasInvoked);
		        
		        Assert.Null(innerConstructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	}
	    	
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.CloseBraceToken.ConstructorWasInvoked);
	    }
    }
    
    [Fact]
    public void CollectionInitializationNode_NoParameters_NoTrailingComma()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new List<int> { }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // constructorInvocationExpressionNode.ResultTypeClauseNode

		// FunctionParametersListingNode
		{
			Assert.Null(constructorInvocationExpressionNode.FunctionParametersListingNode);
		}
	    
	    // ObjectInitializationParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.OpenBraceToken.ConstructorWasInvoked);
	    	
	    	Assert.Empty(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList);
	    	
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.CloseBraceToken.ConstructorWasInvoked);
	    }
    }
	
	[Fact]
    public void CollectionInitializationNode_NoParameters_WithTrailingComma()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new List<Person> { , }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // constructorInvocationExpressionNode.ResultTypeClauseNode

		// FunctionParametersListingNode
		{
			Assert.Null(constructorInvocationExpressionNode.FunctionParametersListingNode);
		}
	    
	    // ObjectInitializationParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.OpenBraceToken.ConstructorWasInvoked);
	    	
	    	Assert.Empty(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList);
	    	
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.CloseBraceToken.ConstructorWasInvoked);
	    }
    }
    
    [Fact]
    public void CollectionInitializationNode_WithParenthesis()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "new List<Person>() { }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)topCodeBlock.GetChildList().Single();
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    // constructorInvocationExpressionNode.ResultTypeClauseNode

		// FunctionParametersListingNode
		{
			Assert.NotNull(constructorInvocationExpressionNode.FunctionParametersListingNode);
			
			Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
	        Assert.Empty(constructorInvocationExpressionNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
	        Assert.True(constructorInvocationExpressionNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
	    
	    // ObjectInitializationParametersListingNode
	    {
	    	Assert.NotNull(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.OpenBraceToken.ConstructorWasInvoked);
	    	
	    	Assert.Empty(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList);
	    	
	    	Assert.True(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.CloseBraceToken.ConstructorWasInvoked);
	    }
    }
    
    [Fact]
    public void LambdaFunction_Expression_NoParameter()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "() => \"Abc\";";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LambdaFunction_Expression_SingleParameter()
    {
    	/*
    	Goal: Parse lambda expression variable declarations (2024-11-03)
    	================================================================
    	
    	'intPerson' is to signify 'int' or 'Person' because this is a 'keyword' and an 'identifier'.
    	So remember the general type clause matching not just an identifier.
    	
    		Rename 'x' to 'argsX'?
    	
    		Technically 'intPerson' does not encompass the case of a contextual keyword...
    			-10 points
    	
    	Cases:
    		================================================================
    		| () => ...;
    		| 
    		| 	BreakList: [(StatementDelimiterToken, null)]
    		| 	EmptyExpressionNode + OpenParenthesisToken =>
    		| 	{
    		| 		Push(new ParenthesizedExpressionNode());
    		| 		return new EmptyExpressionNode();
    		| 	}
    		| 		
    		| 		BreakList: [(StatementDelimiterToken, null), (CloseParenthesisToken, ParenthesizedExpressionNode)]
    		| 		EmptyExpressionNode + CloseParenthesisToken =>
    		| 		{
    		| 			if (peek(1)==EqualsToken && peek(2)==CloseAngleBracketToken)
    		| 				return LambdaExpressionNode;
    		| 			else
    		| 				return ParenthesizedExpressionNode;
    		| 		}
    		|
    		| Related case of ParenthesizedExpressionNode with empty inner expression then StatementDelimiterToken:
    		| 	();
    		
    		================================================================
    		| x => ...;
    		|
    		| Related case of VariableAssignmentExpressionNode:
    		| 	x = ...;
    		|
    		| Related case of FunctionInvocationNode:
    		| 	x();
    		
    		================================================================
    		| intPerson x => ...;
    		| 
    		| 	This case is actually quite unique.
    		| 		AmbiguousIdentifierNode AmbiguousIdentifierNode => ...;
    		|
    		| 	Two consecutive AmbiguousIdentifierNode(s) can happen in other syntax.
    		|     	But, these other syntax begin with a keyword.
    		| 		So, the lack of a keyword prior to the two nodes is indicative of it being a lambda expression.
    		|
    		|         Perhaps check if following the two nodes there is a '=>' syntax prior to deciding it is a lambda expression.
    		| 		If there is NOT '=>' then decide it is a BadExpressionNode.
    		| 
    		| 		Although, this actually is the exact same scenario as:
    		| 			(intPerson x) => ...;
    		|
    		| 		It is the same because they both start with an EmptyExpressionNode upon encountering the two consecutive
    		| 		AmbiguousIdentifierNode(s).
    		|
    		| 		So, only checking for the next few tokens to be '=>' won't work.
    		| 		Because there could be an unknown amount of parameters being defined, such that a ',' is found instead of a '=>'.
    		| 		As well, there could be ')' found if reading the final parameter.
    		
    		================================================================
    		| (x) => ...;
    		| 
    		| 	BreakList: [(StatementDelimiterToken, null)]
    		| 	EmptyExpressionNode + OpenParenthesisToken =>
    		| 	{
    		| 		Push(new ParenthesizedExpressionNode());
    		| 		return new EmptyExpressionNode();
    		| 	}
    		| 		
    		| 		BreakList: [(StatementDelimiterToken, null), (CloseParenthesisToken, ParenthesizedExpressionNode)]
    		| 		EmptyExpressionNode + IdentifierToken => AmbiguousIdentifierExpressionNode;
    		|
    		| 		BreakList: [(StatementDelimiterToken, null), (CloseParenthesisToken, ParenthesizedExpressionNode)]
    		| 		AmbiguousIdentifierExpressionNode + CloseParenthesisToken =>
    		| 		{
    		| 			if (peek(1)==EqualsToken && peek(2)==CloseAngleBracketToken)
    		| 				return LambdaExpressionNode;
    		| 			else
    		| 				return ParenthesizedExpressionNode;
    		| 		}
    		|
    		| Related case of explicit casting:
			| 	(intPerson)x;
			| 		EmptyExpressionNode + OpenParenthesisToken => ParenthesizedExpressionNode;
			| 		
			| 	(intPerson) => ...;
			| 		EmptyExpressionNode + OpenParenthesisToken => ParenthesizedExpressionNode;
    		
    		================================================================
    		| (intPerson x) => ...;
    		|
    		| 	One of the main issues that I'm having is how to track the 'intPerson x' during the
    		| 		time spent as a ParenthesizedExpressionNode.
    		|
    		| 	The same is true for 'x, y'.
    		|
    		| 	Because these syntax are nonsensical to the ParenthesizedExpressionNode.
    		|
    		| 	I could store each 'AmbiguousIdentifierNode' in the SyntaxList property of a 'BadExpressionNode'.
    		|
    		| 	Then, if I later determine that im looking at a LambdaExpressionNode
    		| 		I can give the LambdaExpressionNode the 'BadExpressionNode'.
    		|
    		| 	At that point the LambdaExpressionNode can then make sense of the 'BadExpressionNode' itself.
    		|
    		| 	If the BadExpressionNode's SyntaxList is: { 'intPerson', 'x' }.
    		| 		Then the LambdaExpressionNode can iterate over each item,
    		| 		and take two consecutive AmbiguousIdentifierNode(s) to be a TypeClauseNode and "variable identifier".
    		| 
    		| 	Maybe if the LambdaExpressionNode finds 3 consecutive AmbiguousIdentifierNode(s),
    		| 		then the LambdaExpressionNode itself is a BadExpressionNode?
    		|
    		| 	If the LambdaExpressionNode finds a CommaToken, then it can start tracking a new parameter.
    		|
    		| 	I'd prefer to not construct an instance of ParenthesizedExpressionNode, just to later replace
    		| 		it with the construction of a LambdaExpressionNode.
    		|
    		| 	But, that concern is likely a minor detail relative to the whole of getting the C# Parser to work correctly.
    		
    		================================================================
    		| (x, y, *) => ...;
    		
    		================================================================
    		| (intPerson x, y, *) => ...;
    		
    		================================================================
    		| (x, intPerson y, *) => ...;
    		
    		================================================================
    		| (intPerson x, intPerson y, *) => ...;
    		
    		================================================================
    		| async ... => ...;
    	*/
    	
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "(x => \"Abc\")";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		var lambdaExpressionNode = (LambdaExpressionNode)parenthesizedExpressionNode.InnerExpression;
		
		Assert.Equal(1, lambdaExpressionNode.VariableDeclarationNodeList.Count);
		
		var parameter = lambdaExpressionNode.VariableDeclarationNodeList[0];
		Assert.Equal(CSharpFacts.Types.Void.ToTypeClause(), parameter.TypeClauseNode);
        Assert.Equal("x", parameter.IdentifierToken.TextSpan.GetText());
        Assert.Equal(VariableKind.Local, parameter.VariableKind);
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LambdaFunction_Expression_ManyParameter_Async()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "async (x, index) => \"Abc\";";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LambdaFunction_CodeBlock_NoParameter_Async()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "async () => { WriteLine(\"Abc\"); return \"Cba\"; };";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LambdaFunction_CodeBlock_SingleParameter_Async()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "async x => { return \"Abc\"; };";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LambdaFunction_CodeBlock_ManyParameter()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "(x, index) => { return \"Abc\"; };";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void MethodGroup()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// public string GetPersonFirstNameMethod(Person person) { return person.FirstName; }
				// list.Select(GetPersonFirstNameMethod);
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Action_ImplicitInvocation_NoParameter()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// Action onSubmitAction = () => Console.WriteLine("Submitted");
				// onSubmitAction();
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Action_ImplicitInvocation_SingleParameter()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// Action<string> onSubmitAction = message => Console.WriteLine(message);
				// onSubmitAction("Hello World!");
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Action_ImplicitInvocation_ManyParameter()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// Action<string, Action> onSubmitAction = (message, callback) => { Console.WriteLine(message) callback.Invoke(); };
				// onSubmitAction("Hello World!", () => Console.WriteLine("Callback was invoked."));
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Func_ImplicitInvocation_NoParameter()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// Func<int> onSubmitAction = () => { Console.WriteLine("Submitted"); return 0; };
				// var statusCode = onSubmitAction();
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Func_ImplicitInvocation_SingleParameter()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// Func<string, Task> writeToConsoleAsync = async message => { await Task.Delay(500); Console.WriteLine(message); };
				// await writeToConsoleAsync("Hello World!");
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Func_ImplicitInvocation_ManyParameter()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// Func<string, CancellationToken, Task> writeToConsoleAsync = async (message, cancellationToken) => { await Task.Delay(500); Console.WriteLine(message); };
				// await writeToConsoleAsync("Hello World!", CancellationToken.None);
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void FunctionInvocation_NamedParameters()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// public static Person PersonFactoryMethod(string firstName, string lastName) { return new Person(firstName, lastName); }
				// PersonFactoryMethod(firstName: "John", lastName: "Doe");
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void ConstructorInvocation_NamedParameters()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// public Person(string firstName, string lastName) { FirstName = firstName; LastName = lastName; }
				// PersonFactoryMethod(firstName: "John", lastName: "Doe");
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Record_With_Keyword()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// person = person with { FirstName = "Jane", LastName = "Doe", }
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void ValueTuple()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				// List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> shortCircuitList = new();
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GetterAndSetterThatAreNotAutoImplemented()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				/*
	public string ShortCircuitListStringified
	{
		get
		{
			if (_shortCircuitListStringifiedIsDirty)
			{
				_shortCircuitListStringifiedIsDirty = false;
				_shortCircuitListStringified = string.Join(',', session.ShortCircuitList.Select(x => x.DelimiterSyntaxKind));
			}
				
			return _shortCircuitListStringified;
		}
		private set => _shortCircuitListStringified = value;
	} = string.Empty;
				*//*
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void MemberAccess()
    {
    	/*var session = new ExpressionSession(
			tokenList: new List<ISyntaxToken>
			{
				/*
	*//* x => x.FirstName;
				*//*
			},
			expressionStack: new Stack<ISyntax>());
		
		var expression = Parser_TEST.ParseExpression(session);*/
		
		throw new NotImplementedException();
    }
    
    /// <summary>TODO: Repeat this test for all the control keywords</summary>
    [Fact]
    public void WhileLoop_DoNotBreakScope()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"while (().) { ; }";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer); 
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var whileStatementNode = (WhileStatementNode)topCodeBlock.GetChildList().Single();
		
		((IBinder)parser.Binder).TryGetBinderSession(resourceUri, out var binderSession);
		Assert.Equal(2, binderSession.ScopeList.Count);
    }
    
    [Fact]
    public void IfStatement_StopExpressionsCreatingScope()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
void Aaa()
{
    if (false)
        return; // Scope is being made here correctly.

    Console.WriteLine(); // But it erroneously continues defining scope at each expression following the if statement.
    
    var variable = 2;

    Console.WriteLine(); // But it erroneously continues defining scope at each expression following the if statement.
}
";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer); 
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		((IBinder)parser.Binder).TryGetBinderSession(resourceUri, out var binderSession);
		Assert.Equal(3, binderSession.ScopeList.Count);
		
		foreach (var child in topCodeBlock.GetChildList())
		{
			Console.WriteLine(child.SyntaxKind);
		}
		
		Console.WriteLine("aaaFunctionDefinitionNode.CodeBlockNode:");
		var aaaFunctionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.GetChildList()[2];
		foreach (var child in aaaFunctionDefinitionNode.CodeBlockNode.GetChildList())
		{
			Console.WriteLine(child.SyntaxKind);
		}
		
		Console.WriteLine(((AmbiguousIdentifierNode)topCodeBlock.GetChildList()[1]).IdentifierToken.TextSpan.GetText());
		var functionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.GetChildList().Single();
    }
    
    [Fact]
    public void CollectionIndex()
    {
    	/* var queue = _queueContainerMap[queueKey]; */
		throw new NotImplementedException();
    }
    
    [Fact]
    public void AwaitTask()
    {
    	/* await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false); */
		throw new NotImplementedException();
    }
    
    [Fact]
    public void SwitchStatement()
    {
    	/*
    	- The switch statement cannot be part of an expression.
    	- For this reason, it starts with the keyword switch,
    	  	so when parsing it can easily be seen at the start of the
    	  	statement, that it is a switch statement.
    	- Switch expression can be embedded,
    	  	and for that reason it starts with the IdentifierToken,
    	  	in order to enter the expression parsing code,
    	  	which then goes on to see the 'switch' keyword.
    	- It isn't to say that the language had to be written this way.
    	- But instead that it seems extremely intentional.
    	- The switch statement has a scope for its initial body.
    	- But, the individual case(s) do not create their own scopes.
    	- One can put an "arbitrary code block" after the 'case' definition,
    	  	in order to create a scope for that 'case' specifically.
    	  	- But note: this just always the case in C# it isn't 'switch' specific.
    	- In C# a switch statement cannot fall through its case(s) after
    	  	handling the initial case that jumped to.
    	- Many labels can be defined for the same "handling code".
    	- A 'default' label exists.
    	- 'goto default' also exists, but is this not just normal C#?
    	  	you can goto any label you have in scope this isn't something special.
    	- But maybe the 'default' name for the label is uniquely a 'switch' statement permitted name.
    	- Are you able to goto any of the " case 'a': " label definitions?
    		  - If you wanted to do this, but could not "goto case 'a': "
    		    	could you put a normal C# label alongside " case 'a': "
    		    	and goto the normal C# label?
    		    	- I presume you could ONLY if you put the normal C# label
    		    	  	after the 'case' and before the "handling code".
    		    	  	- You could probably put a label anywhere within the "handling code" too,
    		    	  	  	but I'm speaking of this example specifically where you
    		    	  	  	wanted to jump in a equivalent way as "goto case 'a': ".
    		    	  	  	So in this case you'd  want it after the label, and at the start of the "handling code".
    	- Is it possible to parse the switch statement's body as normal C# code, if I implemented C# label syntax?
    	  	- The issue would appear to be with the 'case' keyword that is at the start of "label".
    	  	- As well, the "label" doesn't have an IdentifierToken, it instead is a unique compile time constant.
    	- The 'switch' statement would be a code block builder that skips until 'case', then skips until ':'.
    	- At this point return to the main parser loop.
    	- Then hit the 'case' keyword which would actually invoke its own method so I could probably do this easily.
    		- public static void HandleCaseTokenKeyword(KeywordToken consumedKeywordToken, CSharpParserModel model) {...}
    			- ParseDefaultKeywords.cs:48 is where the method is defined.
    	- So, the switch statement needs to have its expression parsed (the one within its parentheses).
    	  	- Then it goes to main loop and starts parsing the code block node.
    	  	- This then hits the 'case' keyword.
    	  	- Read until the ':' then return back out again.
    		  - But, what about the 'default' keyword?
    		  - If the statement parsing code sees the 'default' keyword,
    		    	then check if the next token is the ColonToken.
    		    	- If so, then it is the 'default' leg of the switch statement.
    	- This sounds like an effective "first approach" to a solution.
    	- In order to track each leg of the switch statement I can create a new
    	  	CodeBlockBuilder but, have this builder act in a special way.
    	  	- It would capture the statements just as a normal CodeBlockBuilder,
    	  	  	but when the code block is finished, it moves its nodes to the parentCodeBlockBuilder.
    	  	  	- In the end, the "special" code block builder for the legs would just track the start and end indices.
    	  	  	- But, isn't the start the label, and the end the next label or CloseBraceToken?
		- Currently the expression parser is trying to parse the 'ColonToken' in " case 'a': ".
		  	- If the statement parser loop could "skip" over this, the parser would be in a better "state" for parsing the
		  	  	remainder of the file.
    	*/
    
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
switch (character)
{
	case 'a':
	{
		break;
	}
	case 'b':
	case 'c':
		break;
	case 'd':
		if (false)
			goto default;
		break;
	default:
		break;
}
";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer); 
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void SwitchExpression()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
return character switch
{
	'a' => 0,
	_ => 1,
};
";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer); 
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Parenthesized_ExplicitCast()
    {
    	/* ((IBinder)parser.Binder) */
		throw new NotImplementedException();
    }
    
    [Fact]
    public void FunctionParameter_OutKeyword()
    {
    	/* TryGetBinderSession(resourceUri, out IBinderSession binderSession) */
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LessThanBinaryOperator()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "if (_queue.Count < 0)";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer); 
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GreaterThanBinaryOperator()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "if (_queue.Count > 0)";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer); 
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LessThanEqualToBinaryOperator()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "if (_queue.Count <= 0)";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer); 
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GreaterThanEqualToBinaryOperator()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "if (_queue.Count >= 0)";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer); 
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void FunctionParameterOptional()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");

        var sourceText = 
/*@"
public void SetProgress(double decimalPercentProgress)
{
}
";*/

/*@"
public void SetProgress(string? message = null)
{
}
";*/

/*@"
public void SetProgress(double? decimalPercentProgress, string? message = null)
{
}
";*/

@"
public void SetProgress(double? decimalPercentProgress, string? message = null, string? secondaryMessage = null)
{
}
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer); 
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		Console.WriteLine("foreach (var child in topCodeBlock.GetChildList())");
		foreach (var child in topCodeBlock.GetChildList())
		{
			Console.WriteLine("\t" + child.SyntaxKind);
		}
		Console.WriteLine();
		
		var functionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.GetChildList().Single();
		var functionArgumentEntryNodeList = functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList;
		
		int indexParameter;
		
		indexParameter = 0;
		{
			var functionArgumentEntryNode = (FunctionArgumentEntryNode)functionArgumentEntryNodeList[indexParameter];
			Assert.False(functionArgumentEntryNode.IsOptional);
		}
		
		indexParameter = 1;
		{
			var functionArgumentEntryNode = (FunctionArgumentEntryNode)functionArgumentEntryNodeList[indexParameter];
			Assert.True(functionArgumentEntryNode.IsOptional);
		}
		
		indexParameter = 2;
		{
			var functionArgumentEntryNode = (FunctionArgumentEntryNode)functionArgumentEntryNodeList[indexParameter];
			Assert.True(functionArgumentEntryNode.IsOptional);
		}
		//VariableDeclarationNode
	    //OptionalCompileTimeConstantToken
	    
	    
	    
	    //HasParamsKeyword
	    //HasOutKeyword
	    //HasInKeyword
	    //HasRefKeyword

		throw new NotImplementedException();
    }
    
    [Fact]
    public void SourceCodeThatIsNotParsing_Test()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = SourceCodeThatIsNotParsing_Data;
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer); 
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
}
