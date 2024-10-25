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
		
		var parenthesizedExpressionNode = (ExplicitCastNode)topCodeBlock.GetChildList().Single();
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
		
		var parenthesizedExpressionNode = (ExplicitCastNode)topCodeBlock.GetChildList().Single();
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
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "x => \"Abc\";";
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
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
    
    
}

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
		
/// <summary>
    ///        +
    ///      /   \
    ///     /     \
    ///    /       \
    ///   +         1
    ///  / \ 
    /// 1   1
    /// </summary>
    
// The idea:
		// =========
		//
		// Similarly to 'var x = 2;'
		//
		// I'm going to break out '() => "Abc";'
		// into two separate statements.
		//
		// Firstly the FunctionDefinitionNode,
		// but then for the second
		// statement I want a reference to the 'FunctionDefinitionNode'.
		
		// I think the best way to start this is to focus
		// on parsing a FunctionDefinitionNode from the lambda expression/function.
		//
		// This being opposed to worrying about whether the return type of a lambda expression/function
		// is a method group which contains the singular anonymous method that was declared
		// from the lambda express/function.
		
		// functionDefinitionNode.AccessModifierKind;
        // functionDefinitionNode.ReturnTypeClauseNode;
        // functionDefinitionNode.FunctionIdentifierToken;
        // functionDefinitionNode.GenericArgumentsListingNode;
        
        // FunctionArgumentsListingNode
        //{
        	//Assert.True(functionDefinitionNode.FunctionArgumentsListingNode.OpenParenthesisToken.ConstructorWasInvoked);
	        //Assert.Empty(functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
	        //Assert.True(functionDefinitionNode.FunctionArgumentsListingNode.CloseParenthesisToken.ConstructorWasInvoked);
        //}
        
        // CodeBlockNode
        //{
        	// In this scenario, the codeBlockNode is a "single statement codeblock"/expression.
        	// So I can start by drawing the scope at the semicolon just as I do for if statements.
        	//
        	// Then, once again as I do with if statements, I can draw the scope at the braces
        	// if it is more than just a "single statement codeblock"/expression.
        	
        	//Assert.Single(functionDefinitionNode.CodeBlockNode.ChildList);
        	
        	// It isn't identical to an if statement that doesn't have braces.
        	// Because the lambda expression is implicitly returning
        	// whatever "single statement codeblock" it has.
        	
        	// What interactions are these between this and,
        	// ExplicitCastNode and ParenthesizedExpressionNode?
        	// It seems like these 3 nodes could clobber eachother's code.
        	// So it is something to keep mind.
        	
        	// 
        //}
        
        // functionDefinitionNode.ConstraintNode;
        
        // VariableReferenceNode
        // Reference to the anonymously defined function is ultimately the TypeClauseNode for the expression's result.
        
        // It turns out that I cannot avoid returning an expression.
        // I'm going to try and get this input to return an EmptyExpressionNode,
        // instead of what it currently returns which is BadExpressionNode.
        
// I decided that I am going to get all of the 'LambdaFunction' tests to return
    	// a 'LambdaExpressionNode', then go back to the previous tests and
    	// fix any that broke.
    	//
    	// I'm just looking for any kind of progress I can find at the moment.
    	