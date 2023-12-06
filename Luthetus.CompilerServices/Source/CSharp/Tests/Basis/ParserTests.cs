using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;

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
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = @"public class MyComponent
{
	[Parameter]
	public int MyParameter { get; set; }
}";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.ChildBag.Single();

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
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "7 * 3";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

		var binaryOperatorNode = 
			((BinaryExpressionNode)topCodeBlock.ChildBag.Single())
			.BinaryOperatorNode;

		Assert.Equal(typeof(int), binaryOperatorNode.LeftOperandTypeClauseNode.ValueType);
		Assert.Equal(SyntaxKind.StarToken, binaryOperatorNode.OperatorToken.SyntaxKind);
		Assert.Equal(typeof(int), binaryOperatorNode.RightOperandTypeClauseNode.ValueType);
		Assert.Equal(typeof(int), binaryOperatorNode.TypeClauseNode.ValueType);
		Assert.False(binaryOperatorNode.IsFabricated);
		Assert.Equal(SyntaxKind.BinaryOperatorNode, binaryOperatorNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_ConstraintNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_ConstructorDefinitionNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var className = "MyClass";
		var sourceText = $@"public class {className}
{{
	public {className}()
	{{
	}}
}}";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

		var constructorDefinitionNode = (ConstructorDefinitionNode)(
			((TypeDefinitionNode)topCodeBlock.ChildBag.Single())
			.TypeBodyCodeBlockNode.ChildBag.Single());

		Assert.Equal(className,
			constructorDefinitionNode.ReturnTypeClauseNode.TypeIdentifier.TextSpan.GetText());

		Assert.Equal(className, constructorDefinitionNode.FunctionIdentifier.TextSpan.GetText());
		Assert.Null(constructorDefinitionNode.GenericArgumentsListingNode);

		Assert.Empty(constructorDefinitionNode
			.FunctionArgumentsListingNode
			.FunctionArgumentEntryNodeBag);

		Assert.Empty(constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildBag);
		Assert.Null(constructorDefinitionNode.ConstraintNode);
		Assert.False(constructorDefinitionNode.IsFabricated);
		Assert.Equal(SyntaxKind.ConstructorDefinitionNode, constructorDefinitionNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_FunctionArgumentEntryNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var argumentTypeText = "int";
		var argumentValueType = typeof(int);
		var argumentName = "someArgument";
		var sourceText = $@"void MyFunction({argumentTypeText} {argumentName})
{{
}}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

		var functionDefinitionNode = 
			(FunctionDefinitionNode)topCodeBlock.ChildBag.Single();

		Assert.Single(
			functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeBag);
		
		var functionArgumentEntryNode = functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeBag.Single();
		
		Assert.False(functionArgumentEntryNode.IsOptional);
		Assert.False(functionArgumentEntryNode.HasOutKeyword);
		Assert.False(functionArgumentEntryNode.HasInKeyword);
		Assert.False(functionArgumentEntryNode.HasRefKeyword);		
		Assert.False(functionArgumentEntryNode.IsFabricated);
		Assert.Equal(SyntaxKind.FunctionArgumentEntryNode, functionArgumentEntryNode.SyntaxKind);

		var variableDeclarationStatementNode = functionArgumentEntryNode.VariableDeclarationStatementNode;
		
		Assert.Equal(argumentTypeText,
			variableDeclarationStatementNode.TypeClauseNode.TypeIdentifier.TextSpan.GetText());

		Assert.Equal(argumentValueType,
			variableDeclarationStatementNode.TypeClauseNode.ValueType);

		Assert.Equal(argumentName,
			variableDeclarationStatementNode.IdentifierToken.TextSpan.GetText());

		Assert.Equal(VariableKind.Local, variableDeclarationStatementNode.VariableKind);

	    Assert.False(variableDeclarationStatementNode.IsInitialized);
	    Assert.False(variableDeclarationStatementNode.HasGetter);
	    Assert.False(variableDeclarationStatementNode.GetterIsAutoImplemented);
	    Assert.False(variableDeclarationStatementNode.HasSetter);
	    Assert.False(variableDeclarationStatementNode.SetterIsAutoImplemented);		
		Assert.False(variableDeclarationStatementNode.IsFabricated);
		Assert.Equal(SyntaxKind.VariableDeclarationStatementNode, variableDeclarationStatementNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_FunctionArgumentsListingNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = $@"void MyFunction(int someInt, string someString)
{{
}}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

		var functionDefinitionNode = 
			(FunctionDefinitionNode)topCodeBlock.ChildBag.Single();

		Assert.Equal(2, functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeBag.Length);
	}
	
	[Fact]
	public void PARSE_FunctionDefinitionNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var returnTypeText = "int";
		var returnValueType = typeof(int);
		var functionName = "MyFunction";
		var sourceText = $@"int {functionName}()
{{
}}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

		var functionDefinitionNode = 
			(FunctionDefinitionNode)topCodeBlock.ChildBag.Single();

		Assert.Equal(returnTypeText,
			functionDefinitionNode.ReturnTypeClauseNode.TypeIdentifier.TextSpan.GetText());

		Assert.Equal(returnValueType,
			functionDefinitionNode.ReturnTypeClauseNode.ValueType);

		Assert.Equal(functionName,
			functionDefinitionNode.FunctionIdentifier.TextSpan.GetText());

		Assert.Null(functionDefinitionNode.GenericArgumentsListingNode);
		Assert.NotNull(functionDefinitionNode.FunctionArgumentsListingNode);
		Assert.Empty(functionDefinitionNode.FunctionBodyCodeBlockNode.ChildBag);
		Assert.Null(functionDefinitionNode.ConstraintNode);

		Assert.False(functionDefinitionNode.IsFabricated);
		Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionNode.SyntaxKind);
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
