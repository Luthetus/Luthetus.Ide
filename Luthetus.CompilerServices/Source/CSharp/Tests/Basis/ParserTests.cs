using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis;

public class ParserTests
{
	[Fact]
	public void PARSE_AmbiguousIdentifierNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_AttributeNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_BinaryExpressionNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "2 + 2";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.ChildBag.Single();

		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		var binaryOperatorNode = (BinaryOperatorNode)binaryExpressionNode.BinaryOperatorNode;
		var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;

		Assert.Equal(typeof(int), leftLiteralExpressionNode.TypeClauseNode.ValueType);

		Assert.Equal(typeof(int), binaryOperatorNode.TypeClauseNode.ValueType);
		Assert.Equal(SyntaxKind.PlusToken, binaryOperatorNode.OperatorToken.SyntaxKind);

		Assert.Equal(typeof(int), rightLiteralExpressionNode.TypeClauseNode.ValueType);
	}
	
	[Fact]
	public void PARSE_BinaryOperatorNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_ConstraintNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_ConstructorDefinitionNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_FunctionArgumentEntryNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_FunctionArgumentsListingNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_FunctionDefinitionNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_FunctionInvocationNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_FunctionParameterEntryNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_FunctionParametersListingNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_GenericArgumentEntryNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_GenericArgumentsListingNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_GenericParameterEntryNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_GenericParametersListingNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_IdempotentExpressionNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_IfStatementNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_InheritanceStatementNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_LiteralExpressionNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "3";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

		var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.ChildBag.Single();
		Assert.Equal(typeof(int), literalExpressionNode.TypeClauseNode.ValueType);
	}
	
	[Fact]
	public void PARSE_NamespaceEntryNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_NamespaceStatementNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis;";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildBag.Single();
		Assert.Equal(SyntaxKind.NamespaceStatementNode, namespaceStatementNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_ObjectInitializationNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_ParenthesizedExpressionNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_PreprocessorLibraryReferenceStatementNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_ReturnStatementNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_TypeClauseNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_TypeDefinitionNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_UnaryExpressionNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_UnaryOperatorNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_UsingStatementNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "using Luthetus.TextEditor.RazorLib.Lexes.Models;";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

		var usingStatementNode = (UsingStatementNode)topCodeBlock.ChildBag.Single();
		Assert.Equal(SyntaxKind.UsingStatementNode, usingStatementNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_VariableAssignmentExpressionNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_VariableDeclarationNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_VariableExpressionNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_VariableReferenceNode()
	{
		throw new NotImplementedException();
	}
}
