using System.Text;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;


namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Parsers;

/// <summary>
/// Every test case starts with 'return' in order to start the expression loop within the parser.
///
/// Many expressions are compositions of other expressions.
/// As a result, these tests will assert the structure of the syntax tree,
/// and that various identifiers / unique numbers appear at the correct node position.
///
/// Any assertion of TypeClauseNode should be done as its own separate test.
/// (unless the TypeClauseNode comes from an Identifier / Keyword.
///  In these scenarios it perhaps is useful if there is a distinct name
///  being used, in order to identify that it appears at the correct node position).
/// </summary>
public partial class ExpressionTests
{
	[Fact]
	public void Literal_Bool_ResultTypeClauseNode()
	{
		var test = new Test(@"return false;");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		var literalExpressionNode = (LiteralExpressionNode)returnStatementNode.ExpressionNode;
		Assert.Equal("bool", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	}
	
	[Fact]
	public void Literal_Char_ResultTypeClauseNode()
	{
		var test = new Test(@"return 'a';");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		var literalExpressionNode = (LiteralExpressionNode)returnStatementNode.ExpressionNode;
		Assert.Equal("char", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	}
	
	[Fact]
	public void Literal_Number_ResultTypeClauseNode()
	{
		var test = new Test(@"return 1;");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		var literalExpressionNode = (LiteralExpressionNode)returnStatementNode.ExpressionNode;
		Assert.Equal("int", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	}
	
	[Fact]
	public void Literal_String_ResultTypeClauseNode()
	{
		var test = new Test(@"return ""a"";");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		var literalExpressionNode = (LiteralExpressionNode)returnStatementNode.ExpressionNode;
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	}
	
	[Fact]
	public void Literal_StringInterpolated_ResultTypeClauseNode()
	{
		var test = new Test(@"return $""a {myVariable} c"";");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		var literalExpressionNode = (InterpolatedStringNode)returnStatementNode.ExpressionNode;
		Console.WriteLine("aaa " + literalExpressionNode.SyntaxKind);
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	}
	
	[Fact]
	public void Literal_StringVerbatim_ResultTypeClauseNode()
	{
		var test = new Test(@"return @""abc"";");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		var literalExpressionNode = (LiteralExpressionNode)returnStatementNode.ExpressionNode;
		Assert.Equal("string", literalExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	}

    [Fact]
    public void BinaryExpressionNode_Lonely()
    {
		var test = new Test(@"return 1 + 2;");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var binaryExpressionNode = (BinaryExpressionNode)returnStatementNode.ExpressionNode;
		
		var leftExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		Assert.Equal("1", leftExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		
	    Assert.Equal(SyntaxKind.PlusToken, binaryExpressionNode.OperatorToken.SyntaxKind);
	    
	    var rightExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;
	    Assert.Equal("2", rightExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
    }
    
    [Fact]
    public void BinaryExpressionNode_Left_Precedence_GreaterThanOrEqualTo_Right()
    {
    	var test = new Test(@"return 1 + 2 + 3;");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
			
		var externalBinaryExpressionNode = (BinaryExpressionNode)returnStatementNode.ExpressionNode;
		
		// Left Expression
		{
			var innerBinaryExpressionNode = (BinaryExpressionNode)externalBinaryExpressionNode.LeftExpressionNode;
			
			var innerLeftExpressionNode = (LiteralExpressionNode)innerBinaryExpressionNode.LeftExpressionNode;
	    	Assert.Equal("1", innerLeftExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
			
			Assert.Equal(SyntaxKind.PlusToken, innerBinaryExpressionNode.OperatorToken.SyntaxKind);
			
			var innerRightExpressionNode = (LiteralExpressionNode)innerBinaryExpressionNode.RightExpressionNode;
	    	Assert.Equal("2", innerRightExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		}
		
		Assert.Equal(SyntaxKind.PlusToken, externalBinaryExpressionNode.OperatorToken.SyntaxKind);
	    
		var rightExpressionNode = (LiteralExpressionNode)externalBinaryExpressionNode.RightExpressionNode;
		Assert.Equal("3", rightExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
    }
    
    [Fact]
    public void BinaryExpressionNode_Left_Precedence_LessThan_Right()
    {
    	var test = new Test(@"return 1 + 2 * 3;");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
			
		var externalBinaryExpressionNode = (BinaryExpressionNode)returnStatementNode.ExpressionNode;
		
		var leftExpressionNode = (LiteralExpressionNode)externalBinaryExpressionNode.LeftExpressionNode;
		Assert.Equal("1", leftExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		
		Assert.Equal(SyntaxKind.PlusToken, externalBinaryExpressionNode.OperatorToken.SyntaxKind);
		
		// Right Expression
		{
			var innerBinaryExpressionNode = (BinaryExpressionNode)externalBinaryExpressionNode.RightExpressionNode;
			
			var innerLeftExpressionNode = (LiteralExpressionNode)innerBinaryExpressionNode.LeftExpressionNode;
	    	Assert.Equal("2", innerLeftExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
			
			Assert.Equal(SyntaxKind.StarToken, innerBinaryExpressionNode.OperatorToken.SyntaxKind);
			
			var innerRightExpressionNode = (LiteralExpressionNode)innerBinaryExpressionNode.RightExpressionNode;
	    	Assert.Equal("3", innerRightExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		}
    }
    
    [Fact]
    public void ParenthesizedExpressionNode_From_NumericLiteralToken()
    {
    	var test = new Test("return (7);");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)returnStatementNode.ExpressionNode;
		
		var openParenthesisToken = parenthesizedExpressionNode.OpenParenthesisToken;
		Assert.True(openParenthesisToken.ConstructorWasInvoked);
		
		var literalExpressionNode = (LiteralExpressionNode)parenthesizedExpressionNode.InnerExpression;
		Assert.Equal("7", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
		
		var closeParenthesisToken = parenthesizedExpressionNode.CloseParenthesisToken;
		Assert.True(closeParenthesisToken.ConstructorWasInvoked);
    }
    
    [Fact]
    public void ShortCircuit()
    {
    	var test = new Test("return (1 + );");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)returnStatementNode.ExpressionNode;
		
		var openParenthesisToken = parenthesizedExpressionNode.OpenParenthesisToken;
		Assert.True(openParenthesisToken.ConstructorWasInvoked);
		
		// InnerExpression
		{
			var binaryExpressionNode = (BinaryExpressionNode)parenthesizedExpressionNode.InnerExpression;
		
			var leftExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
			Assert.Equal("1", leftExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
			
			Assert.Equal(SyntaxKind.PlusToken, binaryExpressionNode.OperatorToken.SyntaxKind);
			
			var rightExpressionNode = (EmptyExpressionNode)binaryExpressionNode.RightExpressionNode;
			Assert.Equal(SyntaxKind.EmptyExpressionNode, rightExpressionNode.SyntaxKind);
		}
    
    	var closeParenthesisToken = parenthesizedExpressionNode.CloseParenthesisToken;
		Assert.True(closeParenthesisToken.ConstructorWasInvoked);
    }
    
    [Fact]
    public void ExplicitCastNode_From_IdentifierToken()
    {
    	var test = new Test("return (MyClass);");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var explicitCastNode = (ExplicitCastNode)returnStatementNode.ExpressionNode;
		
		var openParenthesisToken = explicitCastNode.OpenParenthesisToken;
		Assert.True(openParenthesisToken.ConstructorWasInvoked);
		
		var resultTypeClauseNode = explicitCastNode.ResultTypeClauseNode;
		Assert.Equal("MyClass", resultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
		var closeParenthesisToken = explicitCastNode.CloseParenthesisToken;
		Assert.True(closeParenthesisToken.ConstructorWasInvoked);
    }
    
    /// <summary>
    /// This test is perhaps useless. Since there ought to be
    /// a test that a TypeClauseNode can come from any of: identifier, keyword, contextual keyword.
    /// </summary>
    [Fact]
    public void ExplicitCastNode_KeywordToken()
    {
    	var test = new Test("return (int);");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var explicitCastNode = (ExplicitCastNode)returnStatementNode.ExpressionNode;
		
		var openParenthesisToken = explicitCastNode.OpenParenthesisToken;
		Assert.True(openParenthesisToken.ConstructorWasInvoked);
		
		var resultTypeClauseNode = explicitCastNode.ResultTypeClauseNode;
		Assert.Equal("int", resultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
		
		var closeParenthesisToken = explicitCastNode.CloseParenthesisToken;
		Assert.True(closeParenthesisToken.ConstructorWasInvoked);
    }
    
    [Fact]
    public void FunctionInvocationNode_Basic()
    {
    	var test = new Test("return MyMethod();");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var functionInvocationNode = (FunctionInvocationNode)returnStatementNode.ExpressionNode;
		
		var functionInvocationIdentifierToken = functionInvocationNode.FunctionInvocationIdentifierToken;
		Assert.Equal("MyMethod", functionInvocationIdentifierToken.TextSpan.GetText());
		
		var genericParametersListingNode = functionInvocationNode.GenericParametersListingNode;
		Assert.Null(genericParametersListingNode);
		
		var functionParametersListingNode = functionInvocationNode.FunctionParametersListingNode;
		{
			var openParenthesisToken = functionParametersListingNode.OpenParenthesisToken;
			Assert.True(openParenthesisToken.ConstructorWasInvoked);
			
			var functionParameterEntryNodeList = functionParametersListingNode.FunctionParameterEntryNodeList;
			Assert.Empty(functionParameterEntryNodeList);
			
			var closeParenthesisToken = functionParametersListingNode.CloseParenthesisToken;
			Assert.True(closeParenthesisToken.ConstructorWasInvoked);
		}
    }
    
    [Fact]
    public void FunctionInvocationNode_Parameters()
    {
    	var test = new Test("return MyMethod(7, \"Asdfg\");");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var functionInvocationNode = (FunctionInvocationNode)returnStatementNode.ExpressionNode;
		
		var functionInvocationIdentifierToken = functionInvocationNode.FunctionInvocationIdentifierToken;
		Assert.Equal("MyMethod", functionInvocationIdentifierToken.TextSpan.GetText());
		
		var genericParametersListingNode = functionInvocationNode.GenericParametersListingNode;
		Assert.Null(genericParametersListingNode);
		
		var functionParametersListingNode = functionInvocationNode.FunctionParametersListingNode;
		{
			var openParenthesisToken = functionParametersListingNode.OpenParenthesisToken;
			Assert.True(openParenthesisToken.ConstructorWasInvoked);
			
			var functionParameterEntryNodeList = functionParametersListingNode.FunctionParameterEntryNodeList;
			
			Assert.Equal(2, functionParameterEntryNodeList.Count);
			
			var zeroIndexParameter = (LiteralExpressionNode)functionParameterEntryNodeList[0].ExpressionNode;
			Assert.Equal("7", zeroIndexParameter.LiteralSyntaxToken.TextSpan.GetText());
			
			var oneIndexParameter = (LiteralExpressionNode)functionParameterEntryNodeList[1].ExpressionNode;
			Assert.Equal("\"Asdfg\"", oneIndexParameter.LiteralSyntaxToken.TextSpan.GetText());
			
			var closeParenthesisToken = functionParametersListingNode.CloseParenthesisToken;
			Assert.True(closeParenthesisToken.ConstructorWasInvoked);
		}
    }
    
    [Fact]
    public void FunctionInvocationNode_Generic()
    {
    	var test = new Test("return MyMethod<int, MyClass>();");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var functionInvocationNode = (FunctionInvocationNode)returnStatementNode.ExpressionNode;
		
		var functionInvocationIdentifierToken = functionInvocationNode.FunctionInvocationIdentifierToken;
		Assert.Equal("MyMethod", functionInvocationIdentifierToken.TextSpan.GetText());
		
		var genericParametersListingNode = functionInvocationNode.GenericParametersListingNode;
		{
			Assert.NotNull(genericParametersListingNode);
			Assert.True(genericParametersListingNode.OpenAngleBracketToken.ConstructorWasInvoked);
			
			Assert.Equal(2, genericParametersListingNode.GenericParameterEntryNodeList.Count);
			
			var intGenericParameterEntryNode = genericParametersListingNode.GenericParameterEntryNodeList[0];
			Assert.Equal("int", intGenericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			
			var stringGenericParameterEntryNode = genericParametersListingNode.GenericParameterEntryNodeList[1];
			Assert.Equal("MyClass", stringGenericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			
			Assert.True(genericParametersListingNode.CloseAngleBracketToken.ConstructorWasInvoked);
		}
		
		var functionParametersListingNode = functionInvocationNode.FunctionParametersListingNode;
		{
			var openParenthesisToken = functionParametersListingNode.OpenParenthesisToken;
			Assert.True(openParenthesisToken.ConstructorWasInvoked);
			
			var functionParameterEntryNodeList = functionParametersListingNode.FunctionParameterEntryNodeList;
			Assert.Empty(functionParameterEntryNodeList);
			
			var closeParenthesisToken = functionParametersListingNode.CloseParenthesisToken;
			Assert.True(closeParenthesisToken.ConstructorWasInvoked);
		}
    }
    
    [Fact]
    public void FunctionInvocationNode_Generic_Parameters()
    {
    	var test = new Test("return MyMethod<int, MyClass>(7, \"Asdfg\");");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var functionInvocationNode = (FunctionInvocationNode)returnStatementNode.ExpressionNode;
		
		var functionInvocationIdentifierToken = functionInvocationNode.FunctionInvocationIdentifierToken;
		Assert.Equal("MyMethod", functionInvocationIdentifierToken.TextSpan.GetText());
		
		var genericParametersListingNode = functionInvocationNode.GenericParametersListingNode;
		{
			Assert.NotNull(genericParametersListingNode);
			Assert.True(genericParametersListingNode.OpenAngleBracketToken.ConstructorWasInvoked);
			
			Assert.Equal(2, genericParametersListingNode.GenericParameterEntryNodeList.Count);
			
			var intGenericParameterEntryNode = genericParametersListingNode.GenericParameterEntryNodeList[0];
			Assert.Equal("int", intGenericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			
			var stringGenericParameterEntryNode = genericParametersListingNode.GenericParameterEntryNodeList[1];
			Assert.Equal("MyClass", stringGenericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
			
			Assert.True(genericParametersListingNode.CloseAngleBracketToken.ConstructorWasInvoked);
		}
		
		var functionParametersListingNode = functionInvocationNode.FunctionParametersListingNode;
		{
			var openParenthesisToken = functionParametersListingNode.OpenParenthesisToken;
			Assert.True(openParenthesisToken.ConstructorWasInvoked);
			
			var functionParameterEntryNodeList = functionParametersListingNode.FunctionParameterEntryNodeList;
			
			Assert.Equal(2, functionParameterEntryNodeList.Count);
			
			var zeroIndexParameter = (LiteralExpressionNode)functionParameterEntryNodeList[0].ExpressionNode;
			Assert.Equal("7", zeroIndexParameter.LiteralSyntaxToken.TextSpan.GetText());
			
			var oneIndexParameter = (LiteralExpressionNode)functionParameterEntryNodeList[1].ExpressionNode;
			Assert.Equal("\"Asdfg\"", oneIndexParameter.LiteralSyntaxToken.TextSpan.GetText());
			
			var closeParenthesisToken = functionParametersListingNode.CloseParenthesisToken;
			Assert.True(closeParenthesisToken.ConstructorWasInvoked);
		}
    }
    
    [Fact]
    public void ConstructorInvocationNode_Basic()
    {
    	var test = new Test("return new Person();");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)returnStatementNode.ExpressionNode;
		
		Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    Assert.Equal("Person", constructorInvocationExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
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
    	var test = new Test("return new Person(18, \"John\");");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)returnStatementNode.ExpressionNode;
    	
    	Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    Assert.Equal("Person", constructorInvocationExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    Assert.Null(constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode);
	    
	    var functionParametersListingNode = constructorInvocationExpressionNode.FunctionParametersListingNode;
	    {
	    	Assert.NotNull(functionParametersListingNode);
			Assert.True(functionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
			
			Assert.Equal(2, functionParametersListingNode.FunctionParameterEntryNodeList.Count);
			
			var zeroIndexParameter = (LiteralExpressionNode)functionParametersListingNode.FunctionParameterEntryNodeList[0].ExpressionNode;
			Assert.Equal("18", zeroIndexParameter.LiteralSyntaxToken.TextSpan.GetText());
			
			var oneIndexParameter = (LiteralExpressionNode)functionParametersListingNode.FunctionParameterEntryNodeList[1].ExpressionNode;
			Assert.Equal("\"John\"", oneIndexParameter.LiteralSyntaxToken.TextSpan.GetText());
			
			Assert.True(functionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
	    
	    Assert.Null(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
    }
    
    [Fact]
    public void ConstructorInvocationNode_Generic()
    {
    	var test = new Test("return new Dictionary<int, Person>();");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)returnStatementNode.ExpressionNode;
    	
    	Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    Assert.Equal("Dictionary", constructorInvocationExpressionNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
	    
	    var genericParametersListingNode = constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode;
	    {
	    	Assert.NotNull(genericParametersListingNode);
	    	Assert.True(genericParametersListingNode.OpenAngleBracketToken.ConstructorWasInvoked);
	    	
	    	Assert.Equal(2, genericParametersListingNode.GenericParameterEntryNodeList.Count);
	    	
	    	var zeroIndexParameter = (TypeClauseNode)genericParametersListingNode.GenericParameterEntryNodeList[0].TypeClauseNode;
	    	Assert.Equal("int", zeroIndexParameter.TypeIdentifierToken.TextSpan.GetText());
	    	
	    	var oneIndexParameter = genericParametersListingNode.GenericParameterEntryNodeList[1].TypeClauseNode;
	    	Assert.Equal("Person", oneIndexParameter.TypeIdentifierToken.TextSpan.GetText());
	    	
	    	Assert.True(genericParametersListingNode.CloseAngleBracketToken.ConstructorWasInvoked);
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
    	// The constructor parameters are nonsensical and just exist for the sake of this test case.
        var test = new Test("return new Dictionary<int, Person>(0, \"Test\");");
		var returnStatementNode = (ReturnStatementNode)test.CompilationUnit.RootCodeBlockNode.GetChildList().Single();
		
		var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)returnStatementNode.ExpressionNode;
        
        Assert.True(constructorInvocationExpressionNode.NewKeywordToken.ConstructorWasInvoked);
		
	    var genericParametersListingNode = constructorInvocationExpressionNode.ResultTypeClauseNode.GenericParametersListingNode;
	    {
	    	Assert.NotNull(genericParametersListingNode);
	    	Assert.True(genericParametersListingNode.OpenAngleBracketToken.ConstructorWasInvoked);
	    	
	    	Assert.Equal(2, genericParametersListingNode.GenericParameterEntryNodeList.Count);
	    	
	    	var zeroIndexParameter = (TypeClauseNode)genericParametersListingNode.GenericParameterEntryNodeList[0].TypeClauseNode;
	    	Assert.Equal("int", zeroIndexParameter.TypeIdentifierToken.TextSpan.GetText());
	    	
	    	var oneIndexParameter = genericParametersListingNode.GenericParameterEntryNodeList[1].TypeClauseNode;
	    	Assert.Equal("Person", oneIndexParameter.TypeIdentifierToken.TextSpan.GetText());
	    	
	    	Assert.True(genericParametersListingNode.CloseAngleBracketToken.ConstructorWasInvoked);
	    }

		var functionParametersListingNode = constructorInvocationExpressionNode.FunctionParametersListingNode;
	    {
	    	Assert.NotNull(functionParametersListingNode);
			Assert.True(functionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
			
			Assert.Equal(2, functionParametersListingNode.FunctionParameterEntryNodeList.Count);
			
			var zeroIndexParameter = (LiteralExpressionNode)functionParametersListingNode.FunctionParameterEntryNodeList[0].ExpressionNode;
			Assert.Equal("0", zeroIndexParameter.LiteralSyntaxToken.TextSpan.GetText());
			
			var oneIndexParameter = (LiteralExpressionNode)functionParametersListingNode.FunctionParameterEntryNodeList[1].ExpressionNode;
			Assert.Equal("\"Test\"", oneIndexParameter.LiteralSyntaxToken.TextSpan.GetText());
			
			Assert.True(functionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}
	    
	    Assert.Null(constructorInvocationExpressionNode.ObjectInitializationParametersListingNode);
    }
    
    [Fact]
    public void ConstructorInvocationNode_NoTypeClauseNode()
    {
    	var test = new Test("return new();");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void ObjectInitializationNode_Parameters_NoTrailingComma()
    {
    	var test = new Test("return new MyClass { FirstName = firstName, LastName = lastName };");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void ObjectInitializationNode__Parameters_WithTrailingComma()
    {
    	var test = new Test("return new MyClassAaa { FirstName = firstName, LastName = lastName, };");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void ObjectInitializationNode_NoParameters_NoTrailingComma()
    {
    	var test = new Test("return new MyClassAaa { };");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void ObjectInitializationNode_NoParameters_WithTrailingComma()
    {
    	var test = new Test("return new MyClassAaa { , };");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void ObjectInitializationNode_WithParenthesis()
    {
    	var test = new Test("return new MyClassAaa() { };");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void CollectionInitializationNode_Parameters_NoTrailingComma()
    {
    	var test = new Test("return new List<int> { 1, 2 };");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void CollectionInitializationNode_Parameters_WithTrailingComma()
    {
    	var test = new Test("return new List<Person> { new Person(1, \"John\"), new(2, \"Jane\"), };");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void CollectionInitializationNode_NoParameters_NoTrailingComma()
    {
    	var test = new Test("return new List<int> { };");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
	
	[Fact]
    public void CollectionInitializationNode_NoParameters_WithTrailingComma()
    {
    	var test = new Test("return new List<Person> { , };");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void CollectionInitializationNode_WithParenthesis()
    {
    	var test = new Test("return new List<Person>() { };");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void LambdaFunction_Expression_NoParameter()
    {
    	var test = new Test("return () => \"Abc\";");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		var lambdaExpressionNode = (LambdaExpressionNode)parenthesizedExpressionNode.InnerExpression;
		
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
    	
    	var test = new Test("return x => \"Abc\";");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		test.WriteChildrenIndented(topCodeBlock);
		
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
    	var test = new Test("return async (x, index) => \"Abc\";");
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
    	var test = new Test("return async () => { WriteLine(\"Abc\"); return \"Cba\"; };");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LambdaFunction_CodeBlock_SingleParameter_Async()
    {
    	var test = new Test("return async x => { return \"Abc\"; };");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		
		throw new NotImplementedException();
    }
    
    [Fact]
    public void LambdaFunction_CodeBlock_ManyParameter()
    {
    	var test = new Test("return (x, index) => { return \"Abc\"; };");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void Record_With_Keyword()
    {
    	var test = new Test("person = person with { FirstName = \"Jane\", LastName = \"Doe\", };");
		throw new NotImplementedException();
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    /// <summary>TODO: Repeat this test for all the control keywords</summary>
    [Fact]
    public void WhileLoop_DoNotBreakScope()
    {
    	var test = new Test(@"while (().) { ; }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var whileStatementNode = (WhileStatementNode)topCodeBlock.GetChildList().Single();
		
		test.Binder.TryGetCompilationUnit(null, test.ResourceUri, out var binderSession);
		Assert.Equal(2, binderSession.ScopeList.Count);
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
		
		test.Binder.TryGetCompilationUnit(null, test.ResourceUri, out var binderSession);
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void CollectionIndex()
    {
    	/* var queue = _queueContainerMap[queueKey]; */
		throw new NotImplementedException();
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void AwaitTask()
    {
    	/* await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false); */
		throw new NotImplementedException();
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void Parenthesized_ExplicitCast()
    {
    	/* ((IBinder)parser.Binder) */
		throw new NotImplementedException();
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void FunctionParameter_OutKeyword()
    {
    	/* TryGetBinderSession(resourceUri, out IBinderSession binderSession) */
		throw new NotImplementedException();
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void LessThanBinaryOperator()
    {
    	var test = new Test("if (_queue.Count < 0)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void GreaterThanBinaryOperator()
    {
    	var test = new Test("if (_queue.Count > 0)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void LessThanEqualToBinaryOperator()
    {
    	var test = new Test("if (_queue.Count <= 0)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void GreaterThanEqualToBinaryOperator()
    {
    	var test = new Test("if (_queue.Count >= 0)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		throw new NotImplementedException();
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
		
		test.WriteChildrenIndented(topCodeBlock);
		
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void ValueTuple()
    {
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
		test.WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var functionInvocationNode = (FunctionInvocationNode)topCodeBlock.GetChildList()[0];
		// test.WriteChildrenIndented(functionInvocationNode, nameof(functionInvocationNode));
		
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
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void TypeClauseNode_Identifier()
    {
    	var test = new Test(@"Person");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		test.WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		Console.WriteLine($"topCodeBlock: {topCodeBlock.GetChildList().Count}");
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void TypeClauseNode_Keyword()
    {
    	var test = new Test(@"int");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		test.WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void TypeClauseNode_Var()
    {
    	var test = new Test(@"var");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		test.WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void TypeClauseNode_Generic()
    {
    	var test = new Test(@"List<int>");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		test.WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void TypeClauseNode_Nullable_Keyword()
    {
    	var test = new Test(@"int?");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		test.WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void TypeClauseNode_Nullable_Identifier()
    {
    	var test = new Test(@"Person?");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		test.WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void TypeClauseNode_GenericNullable()
    {
    	var test = new Test(@"List<int>?");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		test.WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void VariableAssignmentExpressionNode()
    {
    	var test = new Test(@"someVariable = 2;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		//test.WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableAssignmentExpressionNode, variableAssignmentExpressionNode.SyntaxKind);
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void DoNotUseAwaitAsTypeClauseNodeForVariableDeclaration()
    {
    	var test = new Test(@"_ = await SomeMethodAsync();");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		//test.WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableAssignmentExpressionNode, variableAssignmentExpressionNode.SyntaxKind);
    
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
}
