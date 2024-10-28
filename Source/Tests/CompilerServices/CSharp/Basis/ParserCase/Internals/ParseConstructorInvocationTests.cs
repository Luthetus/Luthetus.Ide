using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseConstructorInvocationTests
{
	[Fact]
	public void Aaa()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"public class MyClass
{
	public MyClass Factory()
	{
		return new MyClass();
	}
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList().Single();
		
		var typeBodyCodeBlockNode = typeDefinitionNode.CodeBlockNode;
		
		var functionDefinitionNode = (FunctionDefinitionNode)typeBodyCodeBlockNode.GetChildList().Single();
		
		var functionBodyCodeBlockNode = functionDefinitionNode.CodeBlockNode;
		
		var returnStatementNode = (ReturnStatementNode)functionBodyCodeBlockNode.GetChildList().Single();
		
		// var expressionNode = (FunctionInvocationNode)returnStatementNode.ExpressionNode;
		
		var expressionNode = (ConstructorInvocationExpressionNode)returnStatementNode.ExpressionNode;
		
		// Console.WriteLine(expressionNode);
	}
}
