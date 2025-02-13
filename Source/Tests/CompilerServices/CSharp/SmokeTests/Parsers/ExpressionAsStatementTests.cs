using System.Text;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Parsers;

/// <summary>
/// Anything parsed starts off as a statement,
/// then goes on to invoke the expression loop if the syntax is deemed an expression.
///
/// So, every test in <see cref="ExpressionTests"/> needs to be replicated
/// in this file.
///
/// And the difference between the tests is that <see cref="ExpressionTests"/>
/// needs to somehow start parsing the input as an expression.
///
/// This file parses the inputs as statements first, and goes into the
/// expression loop when it is deemed necessary.
///
/// This brings up the question... should parsing be "statement first" or "expression first"?
/// As of making this comment the CSharpParser is "statement first".
/// </summary>
public partial class ExpressionAsStatementTests
{
	public class Test
	{
		public Test(string sourceText)
		{
			SourceText = sourceText;
			ResourceUri = new ResourceUri("./unitTesting.txt");
			CompilationUnit = new CSharpCompilationUnit(ResourceUri, new CSharpBinder());
			var lexerOutput = CSharpLexer.Lex(ResourceUri, SourceText);
			CompilationUnit.BinderSession = (CSharpBinderSession)CompilationUnit.Binder.StartBinderSession(ResourceUri);
	        CSharpParser.Parse(CompilationUnit, ref lexerOutput);
		}
		
		public string SourceText { get; set; }
		public ResourceUri ResourceUri { get; set; }
		public CSharpLexerOutput LexerOutput { get; set; }
		public IBinder Binder => CompilationUnit.Binder;
		public CSharpCompilationUnit CompilationUnit { get; set; }
	}

    [Fact]
    public void Numeric_Add_BinaryExpressionNode()
    {
		var test = new Test(@"1 + 1");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    
    /// <summary>
    /// When encountering a binary operator,
    /// change control from the expressionPrimary to the binary operator making the decisions.
    ///
    /// "Push" the binary expression node onto the ExpressionList with an EndOfFileToken delimiter.
    ///
    /// Then return an EmptyExpressionNode.
    ///
    /// Whatever the EmptyExpressionNode becomes will then eventually bubble back up to
    /// be used as the right operand.
    ///
    /// This is being said in reference to how things currently are erroneously done.
    /// In which a BinaryExpressionNode explicitly is looking for certain SyntaxKind.
    ///
    /// Checking the SyntaxKind is fine for binding whether there is a syntax error due
    /// to an operator not being defined for various operands.
    ///
    /// But in terms of the parsing of an expression, it needs to just start a fresh EmptyExpressionNode
    /// and grab the right operand that way.
    /// </summary>
    [Fact]
    public void Numeric_Add_BinaryExpressionNode_Number_FunctionInvocation()
    {
		var test = new Test(@"1 + SomeMethod()");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
		var textTypeClause = "int";
		
		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
	    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
	    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    //public ISyntaxToken binaryOperatorNode.OperatorToken { get; }
	    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    var rightFunctionInvocationNode = (FunctionInvocationNode)binaryExpressionNode.RightExpressionNode;
	    // Assert.Equal(textTypeClause, rightFunctionInvocationNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void Numeric_Add_BinaryExpressionNode_More()
    {
    	var test = new Test(@"1 + 1 + 1");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
		WriteChildrenIndentedRecursive(topCodeBlock);
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
    	var test = new Test(@"1 - 1");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test(@"1 * 1");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    public void Numeric_Star_BinaryExpressionNode_Precedence_Parent_LessThan_Child()
    {
    	var test = new Test(@"1 + 2 * 3");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
		WriteChildrenIndentedRecursive(topCodeBlock);
		var textTypeClause = "int";
		
		// Left Expression
	    {
		    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    Assert.Equal("1", rightLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
	    }
		
		// Operator
		{
		    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
		    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    Assert.Equal("+", binaryOperatorNode.OperatorToken.TextSpan.GetText());
		    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		}
	    
	    // Right Expression
		{
			var rightBinaryExpressionNode = (BinaryExpressionNode)binaryExpressionNode.RightExpressionNode;
			Assert.Equal(textTypeClause, rightBinaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			
			// Temporarily swap variables for sanity #change
			var rememberBinaryExpressionNode = binaryExpressionNode;
			binaryExpressionNode = rightBinaryExpressionNode;
			// Inner Binary Expression
			{
				var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
				Assert.Equal(textTypeClause, leftLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
				Assert.Equal("2", leftLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
				
				var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
			    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    Assert.Equal("*", binaryOperatorNode.OperatorToken.TextSpan.GetText());
			    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
				Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    
			    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
			    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    Assert.Equal("3", rightLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
			    
			    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    }
		    
		    // Temporarily swap variables for sanity #restore
		    binaryExpressionNode = rememberBinaryExpressionNode;
		}
	    
	    // Result
	    {
	    	Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    }
    }
    
    [Fact]
    public void Numeric_Star_BinaryExpressionNode_Precedence_Parent_GreaterThan_Child()
    {
    	var test = new Test(@"1 * 2 + 3");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
		WriteChildrenIndentedRecursive(topCodeBlock);
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
				Assert.Equal("1", leftLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
				
				var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
			    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    Assert.Equal("*", binaryOperatorNode.OperatorToken.TextSpan.GetText());
			    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
				Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    
			    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
			    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    Assert.Equal("2", rightLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
			    
			    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    }
		    
		    // Temporarily swap variables for sanity #restore
		    binaryExpressionNode = rememberBinaryExpressionNode;
		}
		
		// Operator
		{
		    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
		    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    Assert.Equal("+", binaryOperatorNode.OperatorToken.TextSpan.GetText());
		    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		}
	    
	    // Right Expression
	    {
		    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
		    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    Assert.Equal("3", rightLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
	    }
	    
	    // Result
	    {
	    	Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    }
    }
    
    [Fact]
    public void Numeric_Star_BinaryExpressionNode_Precedence_Parent_EqualTo_Child()
    {
    	var test = new Test(@"1 * 2 * 3");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList().Single();
		WriteChildrenIndentedRecursive(topCodeBlock);
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
				Assert.Equal("1", leftLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
				
				var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
			    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    Assert.Equal("*", binaryOperatorNode.OperatorToken.TextSpan.GetText());
			    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
				Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    
			    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
			    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			    Assert.Equal("2", rightLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
			    
			    Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    }
		    
		    // Temporarily swap variables for sanity #restore
		    binaryExpressionNode = rememberBinaryExpressionNode;
		}
		
		// Operator
		{
		    var binaryOperatorNode = binaryExpressionNode.BinaryOperatorNode;
		    Assert.Equal(textTypeClause, binaryOperatorNode.LeftOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    Assert.Equal("*", binaryOperatorNode.OperatorToken.TextSpan.GetText());
		    Assert.Equal(textTypeClause, binaryOperatorNode.RightOperandTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			Assert.Equal(textTypeClause, binaryOperatorNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		}
	    
	    // Right Expression
	    {
		    var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
		    Assert.Equal(textTypeClause, rightLiteralExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		    Assert.Equal("3", rightLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
	    }
	    
	    // Result
	    {
	    	Assert.Equal(textTypeClause, binaryExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    }
    }
    
    [Fact]
    public void Numeric_Division_BinaryExpressionNode()
    {
    	var test = new Test(@"1 / 1");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test(@"1 == 1");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("\"Asd\" + \"Fgh\"");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("$\"asd\"");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void String_Interpolated_Aaa()
    {
    	var aaa = 2;
    	var bbb = $"abc {aaa} 123";
    
    	var test = new Test(
@"
$""abc {aaa} 123"";
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
		// var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.GetChildList().Single();
		// Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
		/*
		If the 'TextEditorTextSpan' had been lexed then I could do some sort of 'deferred parsing' once I reached the end of the
		statement.
		
		Because the $"abc {aaa} 123" ought to be of type 'string' regardless of the interpolated expression '{aaa}',
		and since this is just static analysis, then the code surrounding '$"abc {aaa} 123"'
		only is considering the fact that the result is a string. The evaluation of the interpolated expressions is irrelevant to them.
		
		I wonder if '{' is permitted to appear anywhere within an interpolated expression.
		Either due to constructor invocation or lambda expression with a statement code block.
		
		lambda expression with a statement code block as a declaration alone probably isn't valid
		since it isn't 'expression-y' enough.
		
		But if for some reason someone had a constructor invocation that had an argument which
		was an Action or Func then that argument might contain a lambda expression with a statement code block.
		
		I don't think this will be an issue though.
		
		I am going to add lambda expression with a statement code block parsing in the future,
		but when I do I expect to have it be 'deferred parsing'.
		
		Once again, the static analysis only cares about the existence of the lambda expression
		(edge case perhaps being inferred type clause nodes),
		if the deferred parsing for an interpolated string contained a lambda expression with a statement code block,
		then the interpolated string could enqueue the lambda expression for deferred parsing,
		on the same code block.
		
		This would eventually resolve no matter how many times an interpolated string contained
		a lambda expression with a statement code block or vice versa.
		
		There might be a way to alternate between parsing the string literal parts
		and the interpolated expressions but I am going to try 'deferred lexing'.
		
		I think long term there is no 'deferred' anything. It seems less efficient
		than just doing it in one pass but I'm just not sure how to do so at the moment.
		
		This way I can Lex the interpolated string as if it were a 'string literal'
		and just track the text spans of the interpolated expressions separately.
		
		Then when I return to the main Lex() loop, I can set the state such that it starts lexing those
		text spans. Then returns back to where it left off when done.
		
		I wonder if I want to put the lex'd text span tokens into the main List<ISyntaxToken>
		or if I want to have Trivia be a List<ISyntaxToken> instead of TextEditorTextSpan.
		
		Because, with the ISyntaxToken I'd still be able to do the weird "surveying" logic
		since each ISyntaxToken has a TextEditorTextSpan property.
		
		But, for something like GenericParameters, I don't store the tokens between the '<...>'
		in a different place.
		
		I just have a delimiter for the start and end of the GenericParameters instead.
		
		I suppose if I had a delimiter for the state and end of the interpolated string
		that I'd be able to keep its interpolated expression tokens inside the same List<ISyntaxToken>
		as everything else.
		
		I'm still going to try and stick with the 'Trivia' list for this though just using ISyntaxToken.
		
		Side note: avoid list allocation by yield returning?
		
		-----------------------------------------------------
		
		If the tokens for the interpolated expressions are not in the main List<ISyntaxToken>
		that the parser iterates over how do I expect to have the parser parse them?
		
		-----------------------------------------------------
		
		I don't have to put the interpolated expressions in the Trivia list.
		And I don't need to have a token starting delimter and a separate one for ending delimiter.
		
		Because the StringInterpolatedToken itself perhaps could contain the start and end token indices
		for its various interpolated expressions?
		
		This would end up being a List of sorts because there can be an arbitrary amount of
		interpolated expressions in any given interpolated string.
		
		Having every StringInterpolatedToken instantiate a List is a bad idea
		for performance / memory / garbage collection overhead,
		so I could put this extra information in the 'Trivia' list and the tokens in the
		main ISyntaxToken list that the parser iterates?
		
		I think this conflicts with my desire to convert 'text' to 'syntax tree' then back to 'text'.
		
		I've considered breaking the StringInterpolatedToken into string concatenations
		but I think this again would conflict with the 'text' to 'syntax tree' then back to 'text' goal.
		
		Because I'd lose the fact that it was string interpolation previously.
		
		
		*/
    }
    
    [Fact]
    public void String_Interpolated_More()
    {
    	var aaa = 2;
    	var bbb = $"abc {aaa} 123";
    
    	var test = new Test(
@"
var aaa = 2;
var bbb = 7;
var ccc = $""abc {aaa} 123 {bbb}"";
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
		// var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.GetChildList().Single();
		// Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void String_Verbatim()
    {
    	var test = new Test("@\"asd\"");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void String_InterpolatedVerbatim()
    {
    	var test = new Test("$@\"asd\"");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void String_VerbatimInterpolated()
    {
    	var test = new Test("@$\"asd\"");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void Char_BinaryExpressionNode()
    {
    	var test = new Test("'a' + '\n'");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("false == true");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("(7)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("(1 + )");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("(MyClass)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
		var explicitCastNode = (ExplicitCastNode)topCodeBlock.GetChildList().Single();
    }
    
    [Fact]
    public void ExplicitCastNode_KeywordToken()
    {
    	var test = new Test("(int)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var explicitCastNode = (ExplicitCastNode)topCodeBlock.GetChildList().Single();
    }
    
    [Fact]
    public void FunctionInvocationNode_Basic()
    {
    	var test = new Test("MyMethod()");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var functionInvocationNode = (FunctionInvocationNode)topCodeBlock.GetChildList().Single();
		
		Assert.True(functionInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
		Assert.True(functionInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		
		Assert.Empty(functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
    }
    
    [Fact]
    public void FunctionInvocationNode_Parameters()
    {
    	var test = new Test("MyMethod(7, \"Asdfg\")");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("MyMethod<int, MyClass>()");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("MyMethod<int, MyClass>(7, \"Asdfg\")");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    public void FunctionInvocationNode_LambdaFunction_Parameter()
    {
    	var test = new Test("MyMethod(x => 2);");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var functionInvocationNode = (FunctionInvocationNode)topCodeBlock.GetChildList().Single();
		
		// FunctionParametersListingNode
		{
			Assert.True(functionInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
			
			var lambdaFunctionParameterEntryNode = functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList[0];
			Assert.Equal(SyntaxKind.LambdaExpressionNode, lambdaFunctionParameterEntryNode.ExpressionNode.SyntaxKind);

			Assert.True(functionInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
    }
    
    [Fact]
    public void ConstructorInvocationNode_Basic()
    {
    	var test = new Test("new Person()");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new Person(18, \"John\")");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new Dictionary<int, Person>()");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	// The constructor parameters are nonsensical and just exist for the sake of this test case.
        var test = new Test("new Dictionary<int, Person>(0, \"Test\")");
        
        foreach (var token in test.LexerOutput.SyntaxTokenList)
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
    
    	// The constructor parameters are nonsensical and just exist for the sake of this test case.
        var test = new Test("0");
        
        foreach (var token in test.LexerOutput.SyntaxTokenList)
    	{
    		Console.WriteLine(token.SyntaxKind);
    	}
    }
    
    [Fact]
    public void ConstructorInvocationNode_Generic_Parameters()
    {
    	// The constructor parameters are nonsensical and just exist for the sake of this test case.
        var test = new Test("new Dictionary<int, Person>(0, \"Test\")");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new()");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new MyClass { FirstName = firstName, LastName = lastName }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
	    		
	    		Assert.True(firstNameObjectInitializationParameterEntryNode.PropertyIdentifierToken.ConstructorWasInvoked);
	    		Assert.Equal("FirstName", firstNameObjectInitializationParameterEntryNode.PropertyIdentifierToken.TextSpan.GetText());
		        Assert.True(firstNameObjectInitializationParameterEntryNode.EqualsToken.ConstructorWasInvoked);
		        
		        var ambiguousIdentifierExpressionNode = (AmbiguousIdentifierExpressionNode)firstNameObjectInitializationParameterEntryNode.ExpressionNode;
		        Assert.Equal("firstName", ambiguousIdentifierExpressionNode.Token.TextSpan.GetText());
	    	}
	    	{
	    		var lastNameObjectInitializationParameterEntryNode = constructorInvocationExpressionNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[1];
	    		
	    		Assert.True(lastNameObjectInitializationParameterEntryNode.PropertyIdentifierToken.ConstructorWasInvoked);
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
    	var test = new Test("new MyClassAaa { FirstName = firstName, LastName = lastName, }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new MyClassAaa { }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new MyClassAaa { , }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new MyClassAaa() { }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new List<int> { 1, 2 }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new List<Person> { new Person(1, \"John\"), new(2, \"Jane\"), }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new List<int> { }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new List<Person> { , }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("new List<Person>() { }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("return () => 3;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var returnStatementNode = (ReturnStatementNode)topCodeBlock.GetChildList().Single();
		var lambdaExpressionNode = (LambdaExpressionNode)returnStatementNode.ExpressionNode;
		Assert.True(lambdaExpressionNode.CodeBlockNodeIsExpression);
		Assert.Empty(lambdaExpressionNode.VariableDeclarationNodeList);
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
    	
    	var test = new Test("(x => \"Abc\")");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndented(topCodeBlock);
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)topCodeBlock.GetChildList()[1];
		var lambdaExpressionNode = (LambdaExpressionNode)parenthesizedExpressionNode.InnerExpression;
		
		Assert.True(lambdaExpressionNode.CodeBlockNodeIsExpression);
		
		Assert.Equal(1, lambdaExpressionNode.VariableDeclarationNodeList.Count);
		
		var parameter = lambdaExpressionNode.VariableDeclarationNodeList[0];
		Assert.Equal(TypeFacts.Empty.ToTypeClause(), parameter.TypeClauseNode);
        Assert.Equal("x", parameter.IdentifierToken.TextSpan.GetText());
        Assert.Equal(VariableKind.Local, parameter.VariableKind);
    }
    
    [Fact]
    public void LambdaFunction_Expression_ManyParameter_Async()
    {
    	var test = new Test("(async (x, index) => \"Abc\")");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		var lambdaExpressionNode = (LambdaExpressionNode)parenthesizedExpressionNode.InnerExpression;
		
		Assert.True(lambdaExpressionNode.CodeBlockNodeIsExpression);
		
		Assert.Equal(2, lambdaExpressionNode.VariableDeclarationNodeList.Count);
		
		// First element
		{
			var parameter = lambdaExpressionNode.VariableDeclarationNodeList[0];
			Assert.Equal(TypeFacts.Empty.ToTypeClause(), parameter.TypeClauseNode);
	        Assert.Equal("x", parameter.IdentifierToken.TextSpan.GetText());
	        Assert.Equal(VariableKind.Local, parameter.VariableKind);
        }
        
        // Second element
		{
			var parameter = lambdaExpressionNode.VariableDeclarationNodeList[1];
			Assert.Equal(TypeFacts.Empty.ToTypeClause(), parameter.TypeClauseNode);
	        Assert.Equal("index", parameter.IdentifierToken.TextSpan.GetText());
	        Assert.Equal(VariableKind.Local, parameter.VariableKind);
        }
    }
    
    [Fact]
    public void LambdaFunction_CodeBlock_NoParameter_Async()
    {
    	var test = new Test("async () => { WriteLine(\"Abc\"); return \"Cba\"; };");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LambdaFunction_CodeBlock_SingleParameter_Async()
    {
    	var test = new Test("async x => { return \"Abc\"; };");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LambdaFunction_CodeBlock_ManyParameter()
    {
    	var test = new Test("(x, index) => { return \"Abc\"; };");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LambdaFunction_CodeBlock_SingleParameter()
    {
    	var test = new Test("return x => { return \"Abc\"; };");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Lambda_Aaa()
    {
    	var test = new Test(@"void Aaa()
{
    var aaa = 3;

    SomeMethod(x =>
    {
        var bbb = 7;
        return aaa - ccc;
    });

    var ccc = bbb;
}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
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
    	var test = new Test("person with { FirstName = \"Jane\", LastName = \"Doe\", };");
		
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
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
    	var test = new Test(@"while (().) { ; }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var whileStatementNode = (WhileStatementNode)topCodeBlock.GetChildList().Single();
		
		((IBinder)test.Binder).TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.Equal(2, binderSession.ScopeList.Count);
    }
    
    [Fact]
    public void IfStatement_StopExpressionsCreatingScope()
    {
    	var test = new Test(
@"
void Aaa()
{
    if (false)
        return; // Scope is being made here correctly.

    Console.WriteLine(); // But it erroneously continues defining scope at each expression following the if statement.
    
    var variable = 2;

    Console.WriteLine(); // But it erroneously continues defining scope at each expression following the if statement.
}
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		((IBinder)test.Binder).TryGetBinderSession(test.ResourceUri, out var binderSession);
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
    
    	var test = new Test(
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
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void SwitchExpression()
    {
    	var test = new Test(
@"
return character switch
{
	'a' => 0,
	_ => 1,
};
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    	var test = new Test("if (_queue.Count < 0)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GreaterThanBinaryOperator()
    {
    	var test = new Test("if (_queue.Count > 0)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LessThanEqualToBinaryOperator()
    {
    	var test = new Test("if (_queue.Count <= 0)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GreaterThanEqualToBinaryOperator()
    {
    	var test = new Test("if (_queue.Count >= 0)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void FunctionParameterOptional()
    {
    	var test = new Test(
@"
public void SetProgress(double? decimalPercentProgress, string? message = null, string? secondaryMessage = null)
{
}
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndented(topCodeBlock);
		
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
    public void ConstructorInvokesConstructor()
    {
    	var test = new Test(
@"
public class ProgressBarModel
{
	public ProgressBarModel(CancellationToken? cancellationToken)
		: base(cancellationToken)
	{
	}
}
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void ValueTuple()
    {
    	var test = new Test(@"var x = (decimalPercentProgress, null, cancellationToken);");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndented(topCodeBlock);
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		var commaSeparatedExpressionNode = (CommaSeparatedExpressionNode)variableAssignmentExpressionNode.ExpressionNode;
		
		WriteChildrenIndented(commaSeparatedExpressionNode);
		
		var badExpressionNode = (BadExpressionNode)commaSeparatedExpressionNode.GetChildList()[0];
		
		WriteChildrenIndented(badExpressionNode);
		
		/*var variableReferenceNode = (VariableReferenceNode)commaSeparatedExpressionNode.InnerExpressionList[0];
		var nullKeywordToken = (KeywordToken)commaSeparatedExpressionNode.InnerExpressionList[1];
		var variableReferenceNode = (VariableReferenceNode)commaSeparatedExpressionNode.InnerExpressionList[2];*/
		
		throw new NotImplementedException();
    }
    
    /// <summary>
    /// This parses incorrectly: 'Func(decimalPercentProgress);'
    ///
    /// Whereas this parses fine: 'var x = Func(decimalPercentProgress);'
    ///
    /// The reason is suspected to be the transition from the 'statement' parser loop to the 'expression' parser loop.
    ///
    /// It actually seems to be related to 'int decimalPercentProgress;', i.e.: declaring the variable
    /// results in it erroneously being interpreted as a TypeClauseNode, yet if it is undeclared then it
    /// comes out to be VariableReferenceNode.
    /// </summary>
    [Fact]
    public void FunctionInvocationExpressionStatement()
    {
    	var test = new Test(
@"
int decimalPercentProgress;
Func(decimalPercentProgress);
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var functionInvocationNode = (FunctionInvocationNode)topCodeBlock.GetChildList()[0];
		// WriteChildrenIndented(functionInvocationNode, nameof(functionInvocationNode));
		
		Assert.Equal(SyntaxKind.IdentifierToken, functionInvocationNode.GetChildList()[0].SyntaxKind);
		
		var functionParametersListingNode = (FunctionParametersListingNode)functionInvocationNode.GetChildList()[1];
		{
			// Assertions relating to functionParametersListingNode's properties are in this code block.
			Assert.True(functionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
	        Assert.Equal(1, functionParametersListingNode.FunctionParameterEntryNodeList.Count);
	        Assert.True(functionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
		
		var typeClauseNode = (TypeClauseNode)functionInvocationNode.GetChildList()[2];
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void TypeClauseNode_Identifier()
    {
    	var test = new Test(@"Person");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		Console.WriteLine($"topCodeBlock: {topCodeBlock.GetChildList().Length}");
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    }
    
    [Fact]
    public void TypeClauseNode_Keyword()
    {
    	var test = new Test(@"int");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    }
    
    [Fact]
    public void TypeClauseNode_Var()
    {
    	var test = new Test(@"var");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    }
    
    [Fact]
    public void TypeClauseNode_Generic()
    {
    	var test = new Test(@"List<int>");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    }
    
    [Fact]
    public void TypeClauseNode_Nullable_Keyword()
    {
    	var test = new Test(@"int?");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    }
    
    [Fact]
    public void TypeClauseNode_Nullable_Identifier()
    {
    	var test = new Test(@"Person?");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    }
    
    [Fact]
    public void TypeClauseNode_GenericNullable()
    {
    	var test = new Test(@"List<int>?");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableAssignmentExpressionNode()
    {
    	var test = new Test(@"someVariable = 2;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableAssignmentExpressionNode, variableAssignmentExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void Array_Definition_RankOne()
    {
    	var test = new Test(@"int[] someList;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
		Assert.Equal(1, variableDeclarationNode.TypeClauseNode.ArrayRank);
    }
    
    [Fact]
    public void Array_Definition_RankTwo()
    {
    	var test = new Test(@"int[][] someList;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
		Assert.Equal(2, variableDeclarationNode.TypeClauseNode.ArrayRank);
    }
    
    [Fact]
    public void Array_Definition_Nullable()
    {
    	var test = new Test(@"int[]? someList;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
		Assert.Equal(1, variableDeclarationNode.TypeClauseNode.ArrayRank);
		Assert.True(variableDeclarationNode.TypeClauseNode.HasQuestionMark);
    }
    
    [Fact]
    public void Array_Assignment()
    {
    	var test = new Test(@"someList = new int[1];");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var returnStatementNode = (ReturnStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ReturnStatementNode, returnStatementNode.SyntaxKind);
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Array_Access()
    {
    	var test = new Test(@"someList[0];");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var returnStatementNode = (ReturnStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ReturnStatementNode, returnStatementNode.SyntaxKind);
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_CloseAngleBracketToken()
    {
    	var test = new Test(@"if (aaa >) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_CloseAngleBracketEqualsToken()
    {
    	var test = new Test(@"if (aaa >=) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_Pipe()
    {
    	var test = new Test(@"if (aaa |) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_PipePipe()
    {
    	var test = new Test(@"if (aaa ||) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_Ampersand()
    {
    	var test = new Test(@"if (aaa &) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_AmpersandAmpersand()
    {
    	var test = new Test(@"if (aaa &&) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_Equals()
    {
    	var test = new Test(@"if (aaa =) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_EqualsEquals()
    {
    	var test = new Test(@"if (aaa ==) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_BangEquals()
    {
    	var test = new Test(@"if (aaa !=) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    /// <summary>
    /// This isn't ambiguous between 'GenericParametersListingNode' and a 'less than' operator.
    ///
    /// Reason being, you would use the Binder to determine that the identifier prior to the '>'
    /// is a variable.
    ///
    /// Thus, if it is a variable then the only outcome is 'less than'.
    ///
    /// If it is a Type then the only outcome is 'GenericParametersListingNode'.
    /// </summary>
    [Fact]
    public void Identifier_OpenAngleBracketToken()
    {
    	var test = new Test(@"if (aaa <) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_OpenAngleBracketEqualsToken()
    {
    	var test = new Test(@"if (aaa <=) ;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		var expressionNode = ifStatementNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Identifier_MemberAccess_Identifier()
    {
    	var test = new Test(
@"
Person person = new();
person.FirstName;
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList()[1];
		// var expressionNode = binaryExpressionNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Keyword_Functions_KeywordTypeClause()
    {
    	// if (typeof(string))
		// {
		// }
    
    	var test = new Test(
@"
if (typeof(string))
{
}
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		// var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList()[1];
		// var expressionNode = binaryExpressionNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Keyword_Functions_IdentifierTypeClause()
    {
    	var test = new Test(
@"
if (typeof(Apple))
{
}
");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		// var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.GetChildList()[1];
		// var expressionNode = binaryExpressionNode.ExpressionNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void Aaa_LambdaFunction_CodeBlock_NoParameter_Async()
    {
    	var test = new Test("return async () => 3;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    private void WriteChildrenIndented(ISyntaxNode node, string name = "node")
    {
    	Console.WriteLine($"foreach (var child in {name}.GetChildList())");
		foreach (var child in node.GetChildList())
		{
			Console.WriteLine("\t" + child.SyntaxKind);
		}
		Console.WriteLine();
    }
    
    private void WriteChildrenIndentedRecursive(ISyntaxNode node, string name = "node", int indentation = 0)
    {
    	var indentationStringBuilder = new StringBuilder();
    	for (int i = 0; i < indentation; i++)
    		indentationStringBuilder.Append('\t');
    	
    	Console.WriteLine($"{indentationStringBuilder.ToString()}{node.SyntaxKind}");
    	
    	// For the child tokens
    	indentationStringBuilder.Append('\t');
    	var childIndentation = indentationStringBuilder.ToString();
    	
		foreach (var child in node.GetChildList())
		{
			if (child is ISyntaxNode syntaxNode)
			{
				WriteChildrenIndentedRecursive(syntaxNode, "node", indentation + 1);
			}
			else if (child is SyntaxToken syntaxToken)
			{
				Console.WriteLine($"{childIndentation}{child.SyntaxKind}__{syntaxToken.TextSpan.GetText()}");
			}
		}
		
		if (indentation == 0)
			Console.WriteLine();
    }
}
