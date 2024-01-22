using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.CompilerServices.Lang.CSharp.Tests.UserStories;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis;

/// <summary>
/// <see cref="Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase.CSharpParserTests"/>
/// for tests that are intended to be "1 to 1" foreach public API on the <see cref="CSharp.ParserCase.CSharpParser"/>
/// </summary>
public class CustomParserTests
{
	[Fact]
	public void PARSE_AmbiguousIdentifierNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = $"x";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		Assert.Single(topCodeBlock.ChildList);
		var ambiguousIdentifierNode = (AmbiguousIdentifierNode)topCodeBlock.ChildList.Single();

		// Assert ChildList
		{
			var i = 0;

			var identifierToken = ambiguousIdentifierNode.ChildList[i++];
			Assert.IsType<IdentifierToken>(identifierToken);
		}

		// Assert IdentifierToken
		{
			var identifierToken = ambiguousIdentifierNode.IdentifierToken;

			Assert.False(identifierToken.IsFabricated);
			Assert.Equal(SyntaxKind.IdentifierToken, identifierToken.SyntaxKind);

			// Assert IdentifierToken.TextSpan
			{
				var textSpan = identifierToken.TextSpan;

				Assert.Equal(0, textSpan.DecorationByte);
				Assert.Equal(1, textSpan.EndingIndexExclusive);
				Assert.Equal(1, textSpan.Length);
				Assert.Equal(resourceUri, textSpan.ResourceUri);
				Assert.Equal(sourceText, textSpan.SourceText);
				Assert.Equal(0, textSpan.StartingIndexInclusive);
				Assert.Equal("x", textSpan.GetText());
			}
		}

		Assert.False(ambiguousIdentifierNode.IsFabricated);
		Assert.Equal(SyntaxKind.AmbiguousIdentifierNode, ambiguousIdentifierNode.SyntaxKind);
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
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();

		var variableDeclarationNode = (VariableDeclarationNode)typeDefinitionNode.TypeBodyCodeBlockNode!.ChildList.Single();

		var attributeNode = variableDeclarationNode.TypeClauseNode.AttributeNode;

		// Assert ChildList
		{
			var i = 0;

			var openSquareBracketToken = (OpenSquareBracketToken)attributeNode.ChildList[i++];
			Assert.IsType<OpenSquareBracketToken>(openSquareBracketToken);

			var identifierToken = (IdentifierToken)attributeNode.ChildList[i++];
			Assert.IsType<IdentifierToken>(identifierToken);

			var closeSquareBracketToken = (CloseSquareBracketToken)attributeNode.ChildList[i++];
			Assert.IsType<CloseSquareBracketToken>(closeSquareBracketToken);
		}

		// Assert CloseSquareBracketToken
		{
			var closeSquareBracketToken = attributeNode.CloseSquareBracketToken;

			Assert.False(closeSquareBracketToken.IsFabricated);
			Assert.Equal(SyntaxKind.CloseSquareBracketToken, closeSquareBracketToken.SyntaxKind);

			// Assert CloseSquareBracketToken
			{
				var textSpan = closeSquareBracketToken.TextSpan;

				Assert.Equal(0, textSpan.DecorationByte);
				Assert.Equal(41, textSpan.EndingIndexExclusive);
				Assert.Equal(1, textSpan.Length);
				Assert.Equal(resourceUri, textSpan.ResourceUri);
				Assert.Equal(sourceText, textSpan.SourceText);
				Assert.Equal(40, textSpan.StartingIndexInclusive);
				Assert.Equal("]", textSpan.GetText());
			}
		}

		// Assert InnerTokens
		{
			Assert.Single(attributeNode.InnerTokens);
			var identifierToken = attributeNode.InnerTokens.Single();

			// Assert identifierToken from InnerTokens
			{
				Assert.False(identifierToken.IsFabricated);
				Assert.Equal(SyntaxKind.IdentifierToken, identifierToken.SyntaxKind);

				// Assert identifierToken.TextSpan
				{
					var textSpan = identifierToken.TextSpan;

					Assert.Equal(0, textSpan.DecorationByte);
					Assert.Equal(40, textSpan.EndingIndexExclusive);
					Assert.Equal(9, textSpan.Length);
					Assert.Equal(resourceUri, textSpan.ResourceUri);
					Assert.Equal(sourceText, textSpan.SourceText);
					Assert.Equal(31, textSpan.StartingIndexInclusive);
					Assert.Equal("Parameter", textSpan.GetText());
				}
			}
		}

		Assert.False(attributeNode.IsFabricated);

		// Assert OpenSquareBracketToken
		{
			var openSquareBracketToken = attributeNode.OpenSquareBracketToken;

			Assert.False(openSquareBracketToken.IsFabricated);
			Assert.Equal(SyntaxKind.OpenSquareBracketToken, openSquareBracketToken.SyntaxKind);

			// Assert OpenSquareBracketToken.TextSpan
			{
				var textSpan = openSquareBracketToken.TextSpan;

				Assert.Equal(0, textSpan.DecorationByte);
				Assert.Equal(31, textSpan.EndingIndexExclusive);
				Assert.Equal(1, textSpan.Length);
				Assert.Equal(resourceUri, textSpan.ResourceUri);
				Assert.Equal(sourceText, textSpan.SourceText);
				Assert.Equal(30, textSpan.StartingIndexInclusive);
				Assert.Equal("[", textSpan.GetText());
			}
		}

		Assert.Equal(SyntaxKind.AttributeNode, attributeNode.SyntaxKind);
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
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var binaryExpressionNode = (BinaryExpressionNode)topCodeBlock.ChildList.Single();

		var leftLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.LeftExpressionNode;
		var binaryOperatorNode = (BinaryOperatorNode)binaryExpressionNode.BinaryOperatorNode;
		var rightLiteralExpressionNode = (LiteralExpressionNode)binaryExpressionNode.RightExpressionNode;

		Assert.Equal(typeof(int), leftLiteralExpressionNode.ResultTypeClauseNode.ValueType);

		Assert.Equal(typeof(int), binaryOperatorNode.ResultTypeClauseNode.ValueType);
		Assert.Equal(SyntaxKind.PlusToken, binaryOperatorNode.OperatorToken.SyntaxKind);

		Assert.Equal(typeof(int), rightLiteralExpressionNode.ResultTypeClauseNode.ValueType);
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
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var binaryOperatorNode =
			((BinaryExpressionNode)topCodeBlock.ChildList.Single())
			.BinaryOperatorNode;

		Assert.Equal(typeof(int), binaryOperatorNode.LeftOperandTypeClauseNode.ValueType);
		Assert.Equal(SyntaxKind.StarToken, binaryOperatorNode.OperatorToken.SyntaxKind);
		Assert.Equal(typeof(int), binaryOperatorNode.RightOperandTypeClauseNode.ValueType);
		Assert.Equal(typeof(int), binaryOperatorNode.ResultTypeClauseNode.ValueType);
		Assert.False(binaryOperatorNode.IsFabricated);
		Assert.Equal(SyntaxKind.BinaryOperatorNode, binaryOperatorNode.SyntaxKind);
	}

	[Fact]
	public void PARSE_ConstraintNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var constraintText = @"where T : class";
		var sourceText = $@"public T Clone<T>(T item) {constraintText}
{{
	return item;
}}";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		Assert.Single(topCodeBlock.ChildList);
		var functionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.ChildList.Single();
		var constraintNode = functionDefinitionNode.ConstraintNode;

		Assert.NotNull(constraintNode);

		// Assert ChildList
		{
			var i = 0;

			var keywordContextualToken = (KeywordContextualToken)constraintNode.ChildList[i++];
			Assert.IsType<KeywordContextualToken>(keywordContextualToken);

			var identifierToken = (IdentifierToken)constraintNode.ChildList[i++];
			Assert.IsType<IdentifierToken>(identifierToken);

			var colonToken = (ColonToken)constraintNode.ChildList[i++];
			Assert.IsType<ColonToken>(colonToken);

			var keywordToken = (KeywordToken)constraintNode.ChildList[i++];
			Assert.IsType<KeywordToken>(keywordToken);
		}

		// Assert InnerTokens
		{
			var i = 0;

			var keywordContextualToken = (KeywordContextualToken)constraintNode.InnerTokens[i++];
			Assert.IsType<KeywordContextualToken>(keywordContextualToken);

			var identifierToken = (IdentifierToken)constraintNode.InnerTokens[i++];
			Assert.IsType<IdentifierToken>(identifierToken);

			var colonToken = (ColonToken)constraintNode.InnerTokens[i++];
			Assert.IsType<ColonToken>(colonToken);

			var keywordToken = (KeywordToken)constraintNode.InnerTokens[i++];
			Assert.IsType<KeywordToken>(keywordToken);
		}

		Assert.False(constraintNode.IsFabricated);
		Assert.Equal(SyntaxKind.ConstraintNode, constraintNode.SyntaxKind);
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
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var constructorDefinitionNode = (ConstructorDefinitionNode)(
			((TypeDefinitionNode)topCodeBlock.ChildList.Single())
			.TypeBodyCodeBlockNode.ChildList.Single());

		Assert.Equal(className,
			constructorDefinitionNode.ReturnTypeClauseNode.TypeIdentifier.TextSpan.GetText());

		Assert.Equal(className, constructorDefinitionNode.FunctionIdentifier.TextSpan.GetText());
		Assert.Null(constructorDefinitionNode.GenericArgumentsListingNode);

		Assert.Empty(constructorDefinitionNode
			.FunctionArgumentsListingNode
			.FunctionArgumentEntryNodeList);

		Assert.Empty(constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList);
		Assert.Null(constructorDefinitionNode.ConstraintNode);
		Assert.False(constructorDefinitionNode.IsFabricated);
		Assert.Equal(SyntaxKind.ConstructorDefinitionNode, constructorDefinitionNode.SyntaxKind);
	}

    [Fact]
    public void PARSE_ConstructorInvocation()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"List<int> intList = new List<int>();";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        // VariableDeclarationNode
        {
            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];
			Assert.IsType<VariableDeclarationNode>(variableDeclarationNode);
		}

        // VariableAssignmentNode
        {
            var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];
			Assert.IsType<VariableAssignmentExpressionNode>(variableAssignmentNode);

			var i = 0;

			var identifierToken = (IdentifierToken)variableAssignmentNode.ChildList[i++];
			Assert.IsType<IdentifierToken>(identifierToken);

			var equalsToken = (EqualsToken)variableAssignmentNode.ChildList[i++];
            Assert.IsType<EqualsToken>(equalsToken);

            // ConstructorInvocationExpressionNode
            {
                var constructorInvocationExpressionNode = (ConstructorInvocationExpressionNode)variableAssignmentNode.ChildList[i++];
                Assert.IsType<ConstructorInvocationExpressionNode>(constructorInvocationExpressionNode);

				var keywordToken = (KeywordToken)constructorInvocationExpressionNode.ChildList[0];
				Assert.IsType<KeywordToken>(keywordToken);

                var typeClauseNode = (TypeClauseNode)constructorInvocationExpressionNode.ChildList[1];
                Assert.IsType<TypeClauseNode>(typeClauseNode);
				Assert.Equal("List", typeClauseNode.TypeIdentifier.TextSpan.GetText());
				Assert.NotNull(typeClauseNode.GenericParametersListingNode);

				var genericParameterEntryNode = typeClauseNode.GenericParametersListingNode.GenericParameterEntryNodeList.Single();
				Assert.Equal("int", genericParameterEntryNode.TypeClauseNode.TypeIdentifier.TextSpan.GetText());
				Assert.Equal(typeof(int), genericParameterEntryNode.TypeClauseNode.ValueType);
            }
        }
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
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var functionDefinitionNode = 
			(FunctionDefinitionNode)topCodeBlock.ChildList.Single();

		Assert.Single(
			functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
		
		var functionArgumentEntryNode = functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList.Single();
		
		Assert.False(functionArgumentEntryNode.IsOptional);
		Assert.False(functionArgumentEntryNode.HasOutKeyword);
		Assert.False(functionArgumentEntryNode.HasInKeyword);
		Assert.False(functionArgumentEntryNode.HasRefKeyword);		
		Assert.False(functionArgumentEntryNode.IsFabricated);
		Assert.Equal(SyntaxKind.FunctionArgumentEntryNode, functionArgumentEntryNode.SyntaxKind);

		var variableDeclarationStatementNode = functionArgumentEntryNode.VariableDeclarationNode;
		
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
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationStatementNode.SyntaxKind);
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
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var functionDefinitionNode = 
			(FunctionDefinitionNode)topCodeBlock.ChildList.Single();

		Assert.Equal(2, functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList.Length);
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
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var functionDefinitionNode = 
			(FunctionDefinitionNode)topCodeBlock.ChildList.Single();

		Assert.Equal(returnTypeText,
			functionDefinitionNode.ReturnTypeClauseNode.TypeIdentifier.TextSpan.GetText());

		Assert.Equal(returnValueType,
			functionDefinitionNode.ReturnTypeClauseNode.ValueType);

		Assert.Equal(functionName,
			functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());

		Assert.Null(functionDefinitionNode.GenericArgumentsListingNode);
		Assert.NotNull(functionDefinitionNode.FunctionArgumentsListingNode);
		Assert.Empty(functionDefinitionNode.FunctionBodyCodeBlockNode.ChildList);
		Assert.Null(functionDefinitionNode.ConstraintNode);

		Assert.False(functionDefinitionNode.IsFabricated);
		Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_FunctionInvocationNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var functionName = "MyFunction";
		var sourceText = $@"{functionName}()";

		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var functionInvocationNode = 
			(FunctionInvocationNode)topCodeBlock.ChildList.Single();
		
		Assert.Equal(functionName,
			functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan.GetText());
		
		// The corresponding FunctionDefinitionNode should be null because,
		// the function was never defined.
		Assert.Null(functionInvocationNode.FunctionDefinitionNode);

		Assert.Null(functionInvocationNode.GenericParametersListingNode);
		Assert.Empty(functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
		
		Assert.False(functionInvocationNode.IsFabricated);
		Assert.Equal(SyntaxKind.FunctionInvocationNode, functionInvocationNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_FunctionParameterEntryNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var functionName = "MyFunction";
		var sourceText = $@"{functionName}(3)";

		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var functionParameterEntryNode = 
			((FunctionInvocationNode)topCodeBlock.ChildList.Single())
			.FunctionParametersListingNode
			.FunctionParameterEntryNodeList.Single();

	    Assert.False(functionParameterEntryNode.HasOutKeyword);
	    Assert.False(functionParameterEntryNode.HasInKeyword);
	    Assert.False(functionParameterEntryNode.HasRefKeyword);
		Assert.False(functionParameterEntryNode.IsFabricated);
		Assert.Equal(SyntaxKind.FunctionParameterEntryNode, functionParameterEntryNode.SyntaxKind);

		var expressionNode = functionParameterEntryNode.ExpressionNode;
		Assert.IsType<LiteralExpressionNode>(expressionNode);
	}
	
	[Fact]
	public void PARSE_FunctionParametersListingNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var functionName = "MyFunction";
		var sourceText = $@"{functionName}(3, 4)";

		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var functionParametersListingNode = 
			((FunctionInvocationNode)topCodeBlock.ChildList.Single())
			.FunctionParametersListingNode;

		Assert.NotNull(functionParametersListingNode.OpenParenthesisToken);
		Assert.Equal(2, functionParametersListingNode.FunctionParameterEntryNodeList.Length);
		Assert.NotNull(functionParametersListingNode.CloseParenthesisToken);

		Assert.False(functionParametersListingNode.IsFabricated);
		Assert.Equal(SyntaxKind.FunctionParametersListingNode, functionParametersListingNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_GenericArgumentEntryNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = @"public class Box<T>
{
}";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();

		var genericArgumentsListingNode = typeDefinitionNode.GenericArgumentsListingNode;
		Assert.NotNull(genericArgumentsListingNode);

		var genericArgumentEntryNode = genericArgumentsListingNode.GenericArgumentEntryNodeList
			.Single();

		// genericArgumentEntryNode.ChildList
		{
			var typeClauseNode = (TypeClauseNode)genericArgumentEntryNode.ChildList.Single();
			Assert.IsType<TypeClauseNode>(typeClauseNode);
		}

		Assert.False(genericArgumentEntryNode.IsFabricated);
		Assert.Equal(SyntaxKind.GenericArgumentEntryNode, genericArgumentEntryNode.SyntaxKind);

		// genericArgumentEntryNode.TypeClauseNode
		{
			var typeClauseNode = genericArgumentEntryNode.TypeClauseNode;

			// typeClauseNode.AttributeNode
			{
				Assert.Null(typeClauseNode.AttributeNode);
			}

			// typeClauseNode.ChildList
			{
				var identifierToken = (IdentifierToken)typeClauseNode.ChildList.Single();
				Assert.IsType<IdentifierToken>(identifierToken);
			}

			// typeClauseNode.GenericParametersListingNode
			{
				Assert.Null(typeClauseNode.GenericParametersListingNode);
			}

            Assert.False(typeClauseNode.IsFabricated);
            Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);

			// typeClauseNode.TypeIdentifier
			{
				var typeIdentifier = typeClauseNode.TypeIdentifier;

                Assert.False(typeIdentifier.IsFabricated);
                Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);

				// typeIdentifier.TextSpan
				{
					var textSpan = typeIdentifier.TextSpan;

                    Assert.Equal(0, textSpan.DecorationByte);
                    Assert.Equal(18, textSpan.EndingIndexExclusive);
                    Assert.Equal(1, textSpan.Length);
                    Assert.Equal(resourceUri, textSpan.ResourceUri);
                    Assert.Equal(sourceText, textSpan.SourceText);
                    Assert.Equal(17, textSpan.StartingIndexInclusive);
                    Assert.Equal("T", textSpan.GetText());
				}
			}

            Assert.Null(typeClauseNode.ValueType);
		}
	}
	
	[Fact]
	public void PARSE_GenericArgumentsListingNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = @"public class MyClass<T, U>
{
}";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();

        var genericArgumentsListingNode = typeDefinitionNode.GenericArgumentsListingNode;
        Assert.NotNull(genericArgumentsListingNode);

        // genericArgumentsListingNode.ChildList
        {
			var i = 0;

			var openAngleBracketToken = (OpenAngleBracketToken)genericArgumentsListingNode.ChildList[i++];
			Assert.IsType<OpenAngleBracketToken>(openAngleBracketToken);

			var genericArgumentEntryNodeFirst = (GenericArgumentEntryNode)genericArgumentsListingNode.ChildList[i++];
			Assert.IsType<GenericArgumentEntryNode>(genericArgumentEntryNodeFirst);

            var genericArgumentEntryNodeSecond = (GenericArgumentEntryNode)genericArgumentsListingNode.ChildList[i++];
            Assert.IsType<GenericArgumentEntryNode>(genericArgumentEntryNodeSecond);

            var closeAngleBracketToken = (CloseAngleBracketToken)genericArgumentsListingNode.ChildList[i++];
            Assert.IsType<CloseAngleBracketToken>(closeAngleBracketToken);
        }

        // genericArgumentsListingNode.CloseAngleBracketToken
        {
			var closeAngleBracketToken = genericArgumentsListingNode.CloseAngleBracketToken;

            Assert.False(closeAngleBracketToken.IsFabricated);
            Assert.Equal(SyntaxKind.CloseAngleBracketToken, closeAngleBracketToken.SyntaxKind);

			// closeAngleBracketToken.TextSpan
			{
				var textSpan = closeAngleBracketToken.TextSpan;

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(26, textSpan.EndingIndexExclusive);
                Assert.Equal(1, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(25, textSpan.StartingIndexInclusive);
                Assert.Equal(">", textSpan.GetText());
			}
		}

		// genericArgumentsListingNode.GenericArgumentEntryNodeList
		{
			var i = 0;

            // genericArgumentEntryNodeFirst
            {
                var genericArgumentEntryNodeFirst = genericArgumentsListingNode.GenericArgumentEntryNodeList[i++];

				// genericArgumentEntryNodeFirst.ChildList
				{
					var typeClauseNode = (TypeClauseNode)genericArgumentEntryNodeFirst.ChildList
						.Single();

					Assert.IsType<TypeClauseNode>(typeClauseNode);
				}

				Assert.False(genericArgumentEntryNodeFirst.IsFabricated);
				Assert.Equal(SyntaxKind.GenericArgumentEntryNode, genericArgumentEntryNodeFirst.SyntaxKind);

				// genericArgumentEntryNodeFirst.TypeClauseNode
				{
					var typeClauseNode = genericArgumentEntryNodeFirst.TypeClauseNode;

                    Assert.Null(typeClauseNode.AttributeNode);

					// typeClauseNode.ChildList
					{
						var identifierToken = (IdentifierToken)typeClauseNode.ChildList.Single();
						Assert.IsType<IdentifierToken>(identifierToken);
					}

                    Assert.Null(typeClauseNode.GenericParametersListingNode);

					Assert.False(typeClauseNode.IsFabricated);
                    Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);

					// typeClauseNode.TypeIdentifier
					{
						var typeIdentifier = typeClauseNode.TypeIdentifier;

                        Assert.False(typeIdentifier.IsFabricated);
                        Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);

						// typeIdentifier.TextSpan
						{
							var textSpan = typeIdentifier.TextSpan;
							
							Assert.Equal(0, textSpan.DecorationByte);
							Assert.Equal(22, textSpan.EndingIndexExclusive);
							Assert.Equal(1, textSpan.Length);
							Assert.Equal(resourceUri, textSpan.ResourceUri);
							Assert.Equal(sourceText, textSpan.SourceText);
							Assert.Equal(21, textSpan.StartingIndexInclusive);
							Assert.Equal("T", textSpan.GetText());
                        }
					}

                    Assert.Null(typeClauseNode.ValueType);
				}
            }

            // genericArgumentEntryNodeSecond
            {
                var genericArgumentEntryNodeSecond = genericArgumentsListingNode.GenericArgumentEntryNodeList[i++];

				// genericArgumentEntryNodeSecond.ChildList
				{
					var typeClauseNode = (TypeClauseNode)genericArgumentEntryNodeSecond.ChildList.Single();
					Assert.IsType<TypeClauseNode>(typeClauseNode);
				}

				Assert.False(genericArgumentEntryNodeSecond.IsFabricated);
				Assert.Equal(SyntaxKind.GenericArgumentEntryNode, genericArgumentEntryNodeSecond.SyntaxKind);

				// genericArgumentEntryNodeSecond.TypeClauseNode
				{
					var typeClauseNode = genericArgumentEntryNodeSecond.TypeClauseNode;

                    Assert.Null(typeClauseNode.AttributeNode);

					// typeClauseNode.ChildList
					{
						var identifierToken = (IdentifierToken)typeClauseNode.ChildList.Single();
						Assert.IsType<IdentifierToken>(identifierToken);
					}

                    Assert.Null(typeClauseNode.GenericParametersListingNode);

                    Assert.False(typeClauseNode.IsFabricated);
                    Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);

					// typeClauseNode.TypeIdentifier
					{
						var typeIdentifier = typeClauseNode.TypeIdentifier;

                        Assert.False(typeIdentifier.IsFabricated);
                        Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);

						// typeIdentifier.TextSpan
						{
							var textSpan = typeIdentifier.TextSpan;

                            Assert.Equal(0, textSpan.DecorationByte);
                            Assert.Equal(25, textSpan.EndingIndexExclusive);
                            Assert.Equal(1, textSpan.Length);
                            Assert.Equal(resourceUri, textSpan.ResourceUri);
                            Assert.Equal(sourceText, textSpan.SourceText);
                            Assert.Equal(24, textSpan.StartingIndexInclusive);
                            Assert.Equal("U", textSpan.GetText());
						}
					}

                    Assert.Null(typeClauseNode.ValueType);
				}
			}
		}

		Assert.False(genericArgumentsListingNode.IsFabricated);

		// genericArgumentsListingNode.OpenAngleBracketToken
		{
			var openAngleBracketToken = genericArgumentsListingNode.OpenAngleBracketToken;

            Assert.False(openAngleBracketToken.IsFabricated);
            Assert.Equal(SyntaxKind.OpenAngleBracketToken, openAngleBracketToken.SyntaxKind);

            // openAngleBracketToken.TextSpan
            {
				var textSpan = openAngleBracketToken.TextSpan;

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(21, textSpan.EndingIndexExclusive);
                Assert.Equal(1, textSpan.Length);
				Assert.Equal(resourceUri, textSpan.ResourceUri);
				Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(20, textSpan.StartingIndexInclusive);
                Assert.Equal("<", textSpan.GetText());
			}
		}

		Assert.Equal(SyntaxKind.GenericArgumentsListingNode, genericArgumentsListingNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_GenericParameterEntryNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = $@"List<int> myList;";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList.Single();

		var genericParametersListingNode = variableDeclarationNode.TypeClauseNode.GenericParametersListingNode;
		Assert.NotNull(genericParametersListingNode);
        
		var genericParameterEntryNode = genericParametersListingNode.GenericParameterEntryNodeList
            .Single();

		// genericParameterEntryNode.ChildList
		{
			var typeClauseNode = genericParameterEntryNode.ChildList.Single();
			Assert.IsType<TypeClauseNode>(typeClauseNode);
		}

		Assert.False(genericParameterEntryNode.IsFabricated);
		Assert.Equal(SyntaxKind.GenericParameterEntryNode, genericParameterEntryNode.SyntaxKind);

		// genericParameterEntryNode.TypeClauseNode
		{
			var typeClauseNode = genericParameterEntryNode.TypeClauseNode;

            Assert.Null(typeClauseNode.AttributeNode);

			// typeClauseNode.ChildList
			{
				var keywordToken = (KeywordToken)typeClauseNode.ChildList.Single();
				Assert.IsType<KeywordToken>(keywordToken);
			}

            Assert.Null(typeClauseNode.GenericParametersListingNode);
            Assert.False(typeClauseNode.IsFabricated);
            Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);

			// typeClauseNode.TypeIdentifier
			{
				var typeIdentifier = typeClauseNode.TypeIdentifier;

                Assert.False(typeIdentifier.IsFabricated);
                Assert.Equal(SyntaxKind.IntTokenKeyword, typeIdentifier.SyntaxKind);

				// typeIdentifier.TextSpan
				{
					var textSpan = typeIdentifier.TextSpan;

                    Assert.Equal(1, textSpan.DecorationByte);
                    Assert.Equal(8, textSpan.EndingIndexExclusive);
                    Assert.Equal(3, textSpan.Length);
                    Assert.Equal(resourceUri, textSpan.ResourceUri);
                    Assert.Equal(sourceText, textSpan.SourceText);
                    Assert.Equal(5, textSpan.StartingIndexInclusive);
                    Assert.Equal("int", textSpan.GetText());
				}
			}

            Assert.Equal(typeof(int), typeClauseNode.ValueType);
		}
	}
	
	[Fact]
	public void PARSE_GenericParametersListingNode()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"Dictionary<string, int> myMap;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList.Single();

        var genericParametersListingNode = variableDeclarationNode.TypeClauseNode.GenericParametersListingNode;
        Assert.NotNull(genericParametersListingNode);

		// genericParametersListingNode.ChildList
		{
			var i = 0;

			var openAngleBracketToken = genericParametersListingNode.ChildList[i++];
			Assert.IsType<OpenAngleBracketToken>(openAngleBracketToken);

			var genericParameterEntryNodeFirst = genericParametersListingNode.ChildList[i++];
            Assert.IsType<GenericParameterEntryNode>(genericParameterEntryNodeFirst);

            var genericParameterEntryNodeSecond = genericParametersListingNode.ChildList[i++];
            Assert.IsType<GenericParameterEntryNode>(genericParameterEntryNodeSecond);

            var closeAngleBracketToken = genericParametersListingNode.ChildList[i++];
            Assert.IsType<CloseAngleBracketToken>(closeAngleBracketToken);
        }

		// genericParametersListingNode.CloseAngleBracketToken
		{
			var closeAngleBracketToken = genericParametersListingNode.CloseAngleBracketToken;

            Assert.False(closeAngleBracketToken.IsFabricated);
			Assert.Equal(SyntaxKind.CloseAngleBracketToken, closeAngleBracketToken.SyntaxKind);

			// closeAngleBracketToken.TextSpan
			{
				var textSpan = closeAngleBracketToken.TextSpan;

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(23, textSpan.EndingIndexExclusive);
                Assert.Equal(1, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(22, textSpan.StartingIndexInclusive);
                Assert.Equal(">", textSpan.GetText());
			}
		}

		// genericParametersListingNode.GenericParameterEntryNodeList
		{
			var i = 0;

			// genericParameterEntryNodeFirst
			{
				var genericParameterEntryNodeFirst = genericParametersListingNode.GenericParameterEntryNodeList[i++];

				// genericParameterEntryNodeFirst.ChildList
				{
					var typeClauseNode = (TypeClauseNode)genericParameterEntryNodeFirst.ChildList.Single();
					Assert.IsType<TypeClauseNode>(typeClauseNode);
				}

				Assert.False(genericParameterEntryNodeFirst.IsFabricated);
				Assert.Equal(SyntaxKind.GenericParameterEntryNode, genericParameterEntryNodeFirst.SyntaxKind);

				// genericParameterEntryNodeFirst.TypeClauseNode
				{
					var typeClauseNode = genericParameterEntryNodeFirst.TypeClauseNode;

                    Assert.Null(typeClauseNode.AttributeNode);

					// typeClauseNode.ChildList
					{
						var keywordToken = (KeywordToken)typeClauseNode.ChildList.Single();
						Assert.IsType<KeywordToken>(keywordToken);
					}

                    Assert.Null(typeClauseNode.GenericParametersListingNode);
                    Assert.False(typeClauseNode.IsFabricated);
                    Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);

					// typeClauseNode.TypeIdentifier
					{
						var typeIdentifier = typeClauseNode.TypeIdentifier;

                        Assert.False(typeIdentifier.IsFabricated);
                        Assert.Equal(SyntaxKind.StringTokenKeyword, typeIdentifier.SyntaxKind);

						// typeIdentifier.TextSpan
						{
							var textSpan = typeIdentifier.TextSpan;

                            Assert.Equal(1, textSpan.DecorationByte);
                            Assert.Equal(17, textSpan.EndingIndexExclusive);
                            Assert.Equal(6, textSpan.Length);
                            Assert.Equal(resourceUri, textSpan.ResourceUri);
                            Assert.Equal(sourceText, textSpan.SourceText);
                            Assert.Equal(11, textSpan.StartingIndexInclusive);
                            Assert.Equal("string", textSpan.GetText());
						}
					}

                    Assert.Equal(typeof(string), typeClauseNode.ValueType);
				}
			}

			// genericParameterEntryNodeSecond
			{
				var genericParameterEntryNodeSecond = genericParametersListingNode.GenericParameterEntryNodeList[i++];

				// genericParameterEntryNodeSecond.ChildList
				{
					var typeClauseNode = (TypeClauseNode)genericParameterEntryNodeSecond.ChildList.Single();
					Assert.IsType<TypeClauseNode>(typeClauseNode);
				}

				Assert.False(genericParameterEntryNodeSecond.IsFabricated);
				Assert.Equal(SyntaxKind.GenericParameterEntryNode, genericParameterEntryNodeSecond.SyntaxKind);

                // genericParameterEntryNodeSecond.TypeClauseNode
                {
					var typeClauseNode = genericParameterEntryNodeSecond.TypeClauseNode;

                    Assert.Null(typeClauseNode.AttributeNode);

					// typeClauseNode.ChildList
					{
						var keywordToken = (KeywordToken)typeClauseNode.ChildList.Single();
						Assert.IsType<KeywordToken>(keywordToken);
					}

					Assert.Null(typeClauseNode.GenericParametersListingNode);
                    Assert.False(typeClauseNode.IsFabricated);
                    Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);

					// typeClauseNode.TypeIdentifier
					{
						var typeIdentifier = typeClauseNode.TypeIdentifier;

                        Assert.False(typeIdentifier.IsFabricated);
                        Assert.Equal(SyntaxKind.IntTokenKeyword, typeIdentifier.SyntaxKind);

						// typeIdentifier.TextSpan
						{
							var textSpan = typeIdentifier.TextSpan;

                            Assert.Equal(1, textSpan.DecorationByte);
                            Assert.Equal(22, textSpan.EndingIndexExclusive);
                            Assert.Equal(3, textSpan.Length);
                            Assert.Equal(resourceUri, textSpan.ResourceUri);
                            Assert.Equal(sourceText, textSpan.SourceText);
                            Assert.Equal(19, textSpan.StartingIndexInclusive);
                            Assert.Equal("int", textSpan.GetText());
						}
					}
					
                    Assert.Equal(typeof(int), typeClauseNode.ValueType);
                }
			}
		}

		Assert.False(genericParametersListingNode.IsFabricated);

		// genericParametersListingNode.OpenAngleBracketToken
		{
			var openAngleBracketToken = genericParametersListingNode.OpenAngleBracketToken;

            Assert.False(openAngleBracketToken.IsFabricated);
            Assert.Equal(SyntaxKind.OpenAngleBracketToken, openAngleBracketToken.SyntaxKind);

			// openAngleBracketToken.TextSpan
			{
				var textSpan = openAngleBracketToken.TextSpan;

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(11, textSpan.EndingIndexExclusive);
                Assert.Equal(1, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(10, textSpan.StartingIndexInclusive);
				Assert.Equal("<", textSpan.GetText());
			}
		}

		Assert.Equal(SyntaxKind.GenericParametersListingNode, genericParametersListingNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_EmptyExpressionNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_IfStatementNode()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"if (true) { }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var ifStatementNode = (IfStatementNode)topCodeBlock.ChildList.Single();

		// ifStatementNode.ChildList
		{
			var i = 0;

			var keywordToken = (KeywordToken)ifStatementNode.ChildList[i++];
			Assert.IsType<KeywordToken>(keywordToken);

			var literalExpressionNode = (LiteralExpressionNode)ifStatementNode.ChildList[i++];
            Assert.IsType<LiteralExpressionNode>(literalExpressionNode);

            var codeBlockNode = (CodeBlockNode)ifStatementNode.ChildList[i++];
            Assert.IsType<CodeBlockNode>(codeBlockNode);
		}

		// ifStatementNode.ExpressionNode
		{
			var expressionNode = (LiteralExpressionNode)ifStatementNode.ExpressionNode;

			// expressionNode.ChildList
			{
				var i = 0;

				var keywordToken = (KeywordToken)expressionNode.ChildList[i++];
				Assert.IsType<KeywordToken>(keywordToken);

				var typeClauseNode = (TypeClauseNode)expressionNode.ChildList[i++];
                Assert.IsType<TypeClauseNode>(typeClauseNode);
            }

            Assert.False(expressionNode.IsFabricated);

			// expressionNode.LiteralSyntaxToken
			{
				var literalSyntaxToken = expressionNode.LiteralSyntaxToken;

                Assert.False(literalSyntaxToken.IsFabricated);
				Assert.Equal(SyntaxKind.TrueTokenKeyword, literalSyntaxToken.SyntaxKind);

				// literalSyntaxToken.TextSpan
				{
					var textSpan = literalSyntaxToken.TextSpan;

					Assert.Equal(1, textSpan.DecorationByte);
					Assert.Equal(8, textSpan.EndingIndexExclusive);
					Assert.Equal(4, textSpan.Length);
					Assert.Equal(resourceUri, textSpan.ResourceUri);
					Assert.Equal(sourceText, textSpan.SourceText);
					Assert.Equal(4, textSpan.StartingIndexInclusive);
					Assert.Equal("true", textSpan.GetText());
				}
			}

			// expressionNode.ResultTypeClauseNode
			{
				var resultTypeClauseNode = expressionNode.ResultTypeClauseNode;

				Assert.Null(resultTypeClauseNode.AttributeNode);

				// resultTypeClauseNode.ChildList
				{
					var identifierToken = resultTypeClauseNode.ChildList.Single();
					Assert.IsType<IdentifierToken>(identifierToken);
				}

				Assert.Null(resultTypeClauseNode.GenericParametersListingNode);
				Assert.False(resultTypeClauseNode.IsFabricated);
				Assert.Equal(SyntaxKind.TypeClauseNode, resultTypeClauseNode.SyntaxKind);

				// resultTypeClauseNode.TypeIdentifier
				{
					var typeIdentifier = resultTypeClauseNode.TypeIdentifier;

                    Assert.False(typeIdentifier.IsFabricated);
                    Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);
				}

				Assert.Equal(typeof(bool), resultTypeClauseNode.ValueType);
			}

            Assert.Equal(SyntaxKind.LiteralExpressionNode, expressionNode.SyntaxKind);
		}

		// ifStatementNode.IfStatementBodyCodeBlockNode
		{
			var ifStatementBodyCodeBlockNode = ifStatementNode.IfStatementBodyCodeBlockNode;
			Assert.NotNull(ifStatementBodyCodeBlockNode);

            Assert.Empty(ifStatementBodyCodeBlockNode.ChildList);
            Assert.Empty(ifStatementBodyCodeBlockNode.DiagnosticsList);
            Assert.False(ifStatementBodyCodeBlockNode.IsFabricated);
            Assert.Equal(SyntaxKind.CodeBlockNode, ifStatementBodyCodeBlockNode.SyntaxKind);
		}

		Assert.False(ifStatementNode.IsFabricated);

		// ifStatementNode.KeywordToken
		{
			var keywordToken = ifStatementNode.KeywordToken;

            Assert.False(keywordToken.IsFabricated);
            Assert.Equal(SyntaxKind.IfTokenKeyword, keywordToken.SyntaxKind);

			// keywordToken.TextSpan
			{
				var textSpan = keywordToken.TextSpan;

                Assert.Equal(2, textSpan.DecorationByte);
                Assert.Equal(2, textSpan.EndingIndexExclusive);
                Assert.Equal(2, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(0, textSpan.StartingIndexInclusive);
                Assert.Equal("if", textSpan.GetText());
			}
		}

		Assert.Equal(SyntaxKind.IfStatementNode, ifStatementNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_InheritanceStatementNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var typeIdentifierText = "SolutionEditorDisplay";
		var inheritedTypeClauseText = "ComponentBase";
		var sourceText = $"public partial class {typeIdentifierText} : {inheritedTypeClauseText} {{ }}";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
		Assert.Equal(
			typeIdentifierText,
			typeDefinitionNode.TypeIdentifier.TextSpan.GetText());

		var inheritedTypeClauseNode = typeDefinitionNode.InheritedTypeClauseNode;
		Assert.Equal(
			inheritedTypeClauseText,
			inheritedTypeClauseNode.TypeIdentifier.TextSpan.GetText());
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
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var literalExpressionNode = (LiteralExpressionNode)topCodeBlock.ChildList.Single();
		Assert.Equal(typeof(int), literalExpressionNode.ResultTypeClauseNode.ValueType);
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
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();
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
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "(3 + 2)";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)topCodeBlock.ChildList.Single();

		// parenthesizedExpressionNode.ChildList
		{
			var i = 0;

			var openParenthesisToken = (OpenParenthesisToken)parenthesizedExpressionNode.ChildList[i++];
			Assert.IsType<OpenParenthesisToken>(openParenthesisToken);

			var binaryExpressionNode = (BinaryExpressionNode)parenthesizedExpressionNode.ChildList[i++];
            Assert.IsType<BinaryExpressionNode>(binaryExpressionNode);

            var closeParenthesisToken = (CloseParenthesisToken)parenthesizedExpressionNode.ChildList[i++];
            Assert.IsType<CloseParenthesisToken>(closeParenthesisToken);

            var typeClauseNode = (TypeClauseNode)parenthesizedExpressionNode.ChildList[i++];
            Assert.IsType<TypeClauseNode>(typeClauseNode);
        }

		// parenthesizedExpressionNode.CloseParenthesisToken
		{
			var closeParenthesisToken = parenthesizedExpressionNode.CloseParenthesisToken;

            Assert.False(closeParenthesisToken.IsFabricated);
            Assert.Equal(SyntaxKind.CloseParenthesisToken, closeParenthesisToken.SyntaxKind);

			// closeParenthesisToken.TextSpan
			{
				var textSpan = closeParenthesisToken.TextSpan;

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(7, textSpan.EndingIndexExclusive);
                Assert.Equal(1, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(6, textSpan.StartingIndexInclusive);
				Assert.Equal(")", textSpan.GetText());
			}
		}

		// parenthesizedExpressionNode.InnerExpression
		{
			var innerExpression = (BinaryExpressionNode)parenthesizedExpressionNode.InnerExpression;

			// innerExpression.BinaryOperatorNode
			{
				var binaryOperatorNode = innerExpression.BinaryOperatorNode;

				// binaryOperatorNode.ChildList
				{
					var plusToken = binaryOperatorNode.ChildList.Single();
					Assert.IsType<PlusToken>(plusToken);
				}

                Assert.False(binaryOperatorNode.IsFabricated);

				// binaryOperatorNode.LeftOperandTypeClauseNode
				{
					var leftOperandTypeClauseNode = binaryOperatorNode.LeftOperandTypeClauseNode;

                    Assert.Null(leftOperandTypeClauseNode.AttributeNode);

					// leftOperandTypeClauseNode.ChildList
					{
						var identifierToken = leftOperandTypeClauseNode.ChildList.Single();
						Assert.IsType<IdentifierToken>(identifierToken);
					}

                    Assert.Null(leftOperandTypeClauseNode.GenericParametersListingNode);
                    Assert.False(leftOperandTypeClauseNode.IsFabricated);
                    Assert.Equal(SyntaxKind.TypeClauseNode, leftOperandTypeClauseNode.SyntaxKind);

					// leftOperandTypeClauseNode.TypeIdentifier
					{
						var typeIdentifier = leftOperandTypeClauseNode.TypeIdentifier;

                        Assert.False(typeIdentifier.IsFabricated);
                        Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);
					}

                    Assert.Equal(typeof(int), leftOperandTypeClauseNode.ValueType);
				}

				// binaryOperatorNode.OperatorToken
				{
					var operatorToken = binaryOperatorNode.OperatorToken;

                    Assert.False(operatorToken.IsFabricated);
                    Assert.Equal(SyntaxKind.PlusToken, operatorToken.SyntaxKind);

					// operatorToken.TextSpan
					{
						var textSpan = operatorToken.TextSpan;

                        Assert.Equal(0, textSpan.DecorationByte);
                        Assert.Equal(4, textSpan.EndingIndexExclusive);
                        Assert.Equal(1, textSpan.Length);
                        Assert.Equal(resourceUri, textSpan.ResourceUri);
                        Assert.Equal(sourceText, textSpan.SourceText);
                        Assert.Equal(3, textSpan.StartingIndexInclusive);
                        Assert.Equal("+", textSpan.GetText());
					}
				}

				// binaryOperatorNode.ResultTypeClauseNode
				{
					var resultTypeClauseNode = binaryOperatorNode.ResultTypeClauseNode;

                    Assert.Null(resultTypeClauseNode.AttributeNode);

					// resultTypeClauseNode.ChildList
					{
						var identifierToken = resultTypeClauseNode.ChildList.Single();
						Assert.IsType<IdentifierToken>(identifierToken);
					}

                    Assert.Null(resultTypeClauseNode.GenericParametersListingNode);
                    Assert.False(resultTypeClauseNode.IsFabricated);
                    Assert.Equal(SyntaxKind.TypeClauseNode, resultTypeClauseNode.SyntaxKind);

					// resultTypeClauseNode.TypeIdentifier
					{
						var typeIdentifier = resultTypeClauseNode.TypeIdentifier;

                        Assert.False(typeIdentifier.IsFabricated);
                        Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);
					}

                    Assert.Equal(typeof(int), resultTypeClauseNode.ValueType);
				}

				// binaryOperatorNode.RightOperandTypeClauseNode
				{
					var rightOperandTypeClauseNode = binaryOperatorNode.RightOperandTypeClauseNode;

                    Assert.Null(rightOperandTypeClauseNode.AttributeNode);

                    // rightOperandTypeClauseNode.ChildList
                    {
						var identifierToken = rightOperandTypeClauseNode.ChildList.Single();
						Assert.IsType<IdentifierToken>(identifierToken);
					}

                    Assert.Null(rightOperandTypeClauseNode.GenericParametersListingNode);
                    Assert.False(rightOperandTypeClauseNode.IsFabricated);
                    Assert.Equal(SyntaxKind.TypeClauseNode, rightOperandTypeClauseNode.SyntaxKind);

                    // rightOperandTypeClauseNode.TypeIdentifier
                    {
						var typeIdentifier = rightOperandTypeClauseNode.TypeIdentifier;

                        Assert.False(typeIdentifier.IsFabricated);
                        Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);
					}

                    Assert.Equal(typeof(int), rightOperandTypeClauseNode.ValueType);
				}

                Assert.Equal(SyntaxKind.BinaryOperatorNode, binaryOperatorNode.SyntaxKind);
			}

			// innerExpression.ChildList
			{
				var i = 0;

				var literalExpressionNodeFirst = innerExpression.ChildList[i++];
				Assert.IsType<LiteralExpressionNode>(literalExpressionNodeFirst);

				var binaryOperatorNode = innerExpression.ChildList[i++];
                Assert.IsType<BinaryOperatorNode>(binaryOperatorNode);

                var literalExpressionNodeSecond = innerExpression.ChildList[i++];
                Assert.IsType<LiteralExpressionNode>(literalExpressionNodeSecond);
            }

            Assert.False(innerExpression.IsFabricated);

			// innerExpression.LeftExpressionNode
			{
				var leftExpressionNode = (LiteralExpressionNode)innerExpression.LeftExpressionNode;

				// leftExpressionNode.ChildList
				{
					var i = 0;

					var numericLiteralToken = leftExpressionNode.ChildList[i++];
					Assert.IsType<NumericLiteralToken>(numericLiteralToken);

					var typeClauseNode = leftExpressionNode.ChildList[i++];
                    Assert.IsType<TypeClauseNode>(typeClauseNode);
                }

                Assert.False(leftExpressionNode.IsFabricated);

				// leftExpressionNode.LiteralSyntaxToken
				{
					var literalSyntaxToken = leftExpressionNode.LiteralSyntaxToken;

                    Assert.False(literalSyntaxToken.IsFabricated);
                    Assert.Equal(SyntaxKind.NumericLiteralToken, literalSyntaxToken.SyntaxKind);

					// literalSyntaxToken.TextSpan
					{
						var textSpan = literalSyntaxToken.TextSpan;

                        Assert.Equal(0, textSpan.DecorationByte);
                        Assert.Equal(2, textSpan.EndingIndexExclusive);
                        Assert.Equal(1, textSpan.Length);
                        Assert.Equal(resourceUri, textSpan.ResourceUri);
                        Assert.Equal(sourceText, textSpan.SourceText);
                        Assert.Equal(1, textSpan.StartingIndexInclusive);
                        Assert.Equal("3", textSpan.GetText());
					}
				}

				// leftExpressionNode.ResultTypeClauseNode
				{
					var resultTypeClauseNode = leftExpressionNode.ResultTypeClauseNode;

                    Assert.Null(resultTypeClauseNode.AttributeNode);

					// resultTypeClauseNode.ChildList
					{
						var identifierToken = resultTypeClauseNode.ChildList.Single();
						Assert.IsType<IdentifierToken>(identifierToken);
					}

                    Assert.Null(resultTypeClauseNode.GenericParametersListingNode);
                    Assert.False(resultTypeClauseNode.IsFabricated);
                    Assert.Equal(SyntaxKind.TypeClauseNode, resultTypeClauseNode.SyntaxKind);

					// resultTypeClauseNode.TypeIdentifier
					{
						var typeIdentifier = resultTypeClauseNode.TypeIdentifier;

                        Assert.False(typeIdentifier.IsFabricated);
                        Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);
					}

                    Assert.Equal(typeof(int), resultTypeClauseNode.ValueType);
				}

                Assert.Equal(SyntaxKind.LiteralExpressionNode, leftExpressionNode.SyntaxKind);
			}

			// innerExpression.ResultTypeClauseNode
			{
				var resultTypeClauseNode = innerExpression.ResultTypeClauseNode;

                Assert.Null(resultTypeClauseNode.AttributeNode);

				// resultTypeClauseNode.ChildList
				{
					var identifierToken = resultTypeClauseNode.ChildList.Single();
					Assert.IsType<IdentifierToken>(identifierToken);
				}

                Assert.Null(resultTypeClauseNode.GenericParametersListingNode);
                Assert.False(resultTypeClauseNode.IsFabricated);
                Assert.Equal(SyntaxKind.TypeClauseNode, resultTypeClauseNode.SyntaxKind);

				// resultTypeClauseNode.TypeIdentifier
				{
					var typeIdentifier = resultTypeClauseNode.TypeIdentifier;

                    Assert.False(typeIdentifier.IsFabricated);
                    Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);
				}

                Assert.Equal(typeof(int), resultTypeClauseNode.ValueType);
			}

			// innerExpression.RightExpressionNode
			{
				var rightExpressionNode = (LiteralExpressionNode)innerExpression.RightExpressionNode;

				// rightExpressionNode.ChildList
				{
					var i = 0;

					var numericLiteralToken = rightExpressionNode.ChildList[i++];
					Assert.IsType<NumericLiteralToken>(numericLiteralToken);

					var typeClauseNode = rightExpressionNode.ChildList[i++];
                    Assert.IsType<TypeClauseNode>(typeClauseNode);
                }

                Assert.False(rightExpressionNode.IsFabricated);

				// rightExpressionNode.LiteralSyntaxToken
				{
					var literalSyntaxToken = rightExpressionNode.LiteralSyntaxToken;

                    Assert.False(literalSyntaxToken.IsFabricated);
                    Assert.Equal(SyntaxKind.NumericLiteralToken, literalSyntaxToken.SyntaxKind);

					// literalSyntaxToken.TextSpan
					{
						var textSpan = literalSyntaxToken.TextSpan;

                        Assert.Equal(0, textSpan.DecorationByte);
                        Assert.Equal(6, textSpan.EndingIndexExclusive);
                        Assert.Equal(1, textSpan.Length);
                        Assert.Equal(resourceUri, textSpan.ResourceUri);
                        Assert.Equal(sourceText, textSpan.SourceText);
                        Assert.Equal(5, textSpan.StartingIndexInclusive);
                        Assert.Equal("2", textSpan.GetText());
					}
				}

				// rightExpressionNode.ResultTypeClauseNode
				{
					var resultTypeClauseNode = rightExpressionNode.ResultTypeClauseNode;

                    Assert.Null(resultTypeClauseNode.AttributeNode);

					// resultTypeClauseNode.ChildList
					{
						var identifierToken = resultTypeClauseNode.ChildList.Single();
						Assert.IsType<IdentifierToken>(identifierToken);
					}

					Assert.Null(resultTypeClauseNode.GenericParametersListingNode);
                    Assert.False(resultTypeClauseNode.IsFabricated);
                    Assert.Equal(SyntaxKind.TypeClauseNode, resultTypeClauseNode.SyntaxKind);

                    // resultTypeClauseNode.TypeIdentifier
                    {
						var typeIdentifier = resultTypeClauseNode.TypeIdentifier;

                        Assert.False(typeIdentifier.IsFabricated);
                        Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);
                    }

                    Assert.Equal(typeof(int), resultTypeClauseNode.ValueType);
				}

                Assert.Equal(SyntaxKind.LiteralExpressionNode, rightExpressionNode.SyntaxKind);
			}

            Assert.Equal(SyntaxKind.BinaryExpressionNode, innerExpression.SyntaxKind);
		}

		Assert.False(parenthesizedExpressionNode.IsFabricated);

		// parenthesizedExpressionNode.OpenParenthesisToken
		{
			var openParenthesisToken = parenthesizedExpressionNode.OpenParenthesisToken;

            Assert.False(openParenthesisToken.IsFabricated);
            Assert.Equal(SyntaxKind.OpenParenthesisToken, openParenthesisToken.SyntaxKind);

			// openParenthesisToken.TextSpan
			{
				var textSpan = openParenthesisToken.TextSpan;

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(1, textSpan.EndingIndexExclusive);
                Assert.Equal(1, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(0, textSpan.StartingIndexInclusive);
                Assert.Equal("(", textSpan.GetText());
			}
		}

		// parenthesizedExpressionNode.ResultTypeClauseNode
		{
			var resultTypeClauseNode = parenthesizedExpressionNode.ResultTypeClauseNode;

            Assert.Null(resultTypeClauseNode.AttributeNode);

			// resultTypeClauseNode.ChildList
			{
				var identifierToken = resultTypeClauseNode.ChildList.Single();
				Assert.IsType<IdentifierToken>(identifierToken);
			}

            Assert.Null(resultTypeClauseNode.GenericParametersListingNode);
            Assert.False(resultTypeClauseNode.IsFabricated);
            Assert.Equal(SyntaxKind.TypeClauseNode, resultTypeClauseNode.SyntaxKind);

			// resultTypeClauseNode.TypeIdentifier
			{
				var typeIdentifier = resultTypeClauseNode.TypeIdentifier;

                Assert.False(typeIdentifier.IsFabricated);
                Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);
			}

            Assert.Equal(typeof(int), resultTypeClauseNode.ValueType);
		}

		Assert.Equal(SyntaxKind.ParenthesizedExpressionNode, parenthesizedExpressionNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_PreprocessorLibraryReferenceStatementNode()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_ReturnStatementNode()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"public int MyMethod()
{
	return 8;
}";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var functionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.ChildList.Single();
		Assert.NotNull(functionDefinitionNode.FunctionBodyCodeBlockNode);

		var returnStatementNode = (ReturnStatementNode)functionDefinitionNode.FunctionBodyCodeBlockNode.ChildList.Single();

		// returnStatementNode.ChildList
		{
			var i = 0;

			var keywordToken = returnStatementNode.ChildList[i++];
			Assert.IsType<KeywordToken>(keywordToken);

			var literalExpressionNode = returnStatementNode.ChildList[i++];
            Assert.IsType<LiteralExpressionNode>(literalExpressionNode);
		}

		// returnStatementNode.ExpressionNode
		{
			var expressionNode = (LiteralExpressionNode)returnStatementNode.ExpressionNode;

			// expressionNode.ChildList
			{
				var i = 0;

				var numericLiteralToken = expressionNode.ChildList[i++];
                Assert.IsType<NumericLiteralToken>(numericLiteralToken);

                var typeClauseNode = expressionNode.ChildList[i++];
				Assert.IsType<TypeClauseNode>(typeClauseNode);
			}

            Assert.False(expressionNode.IsFabricated);

			// expressionNode.LiteralSyntaxToken
			{
				var literalSyntaxToken = expressionNode.LiteralSyntaxToken;

                Assert.False(literalSyntaxToken.IsFabricated);
                Assert.Equal(SyntaxKind.NumericLiteralToken, literalSyntaxToken.SyntaxKind);

				// literalSyntaxToken.TextSpan
				{
					var textSpan = literalSyntaxToken.TextSpan;

                    Assert.Equal(0, textSpan.DecorationByte);
                    Assert.Equal(35, textSpan.EndingIndexExclusive);
                    Assert.Equal(1, textSpan.Length);
                    Assert.Equal(resourceUri, textSpan.ResourceUri);
                    Assert.Equal(sourceText, textSpan.SourceText);
                    Assert.Equal(34, textSpan.StartingIndexInclusive);
                    Assert.Equal("8", textSpan.GetText());
				}
			}

            // expressionNode.ResultTypeClauseNode
            {
				var resultTypeClauseNode = expressionNode.ResultTypeClauseNode;

				Assert.Null(resultTypeClauseNode.AttributeNode);

				// resultTypeClauseNode.ChildList
				{
					var identifierToken = resultTypeClauseNode.ChildList.Single();
					Assert.IsType<IdentifierToken>(identifierToken);
				}

                Assert.Null(resultTypeClauseNode.GenericParametersListingNode);
                Assert.False(resultTypeClauseNode.IsFabricated);
                Assert.Equal(SyntaxKind.TypeClauseNode, resultTypeClauseNode.SyntaxKind);

                Assert.Equal(typeof(int), resultTypeClauseNode.ValueType);
            }

            Assert.Equal(SyntaxKind.LiteralExpressionNode, expressionNode.SyntaxKind);
		}

		Assert.False(returnStatementNode.IsFabricated);

		// returnStatementNode.KeywordToken
		{
			var keywordToken = returnStatementNode.KeywordToken;

            Assert.False(keywordToken.IsFabricated);
            Assert.Equal(SyntaxKind.ReturnTokenKeyword, keywordToken.SyntaxKind);

			// keywordToken.TextSpan
			{
				var textSpan = keywordToken.TextSpan;

                Assert.Equal(2, textSpan.DecorationByte);
                Assert.Equal(33, textSpan.EndingIndexExclusive);
                Assert.Equal(6, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(27, textSpan.StartingIndexInclusive);
                Assert.Equal("return", textSpan.GetText());
			}
		}

		Assert.Equal(SyntaxKind.ReturnStatementNode, returnStatementNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_TypeClauseNode()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "int x;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList.Single();
		
		var typeClauseNode = variableDeclarationNode.TypeClauseNode;

		Assert.Null(typeClauseNode.AttributeNode);

		// typeClauseNode.ChildList
		{
			var keywordToken = (KeywordToken)typeClauseNode.ChildList.Single();
			Assert.IsType<KeywordToken>(keywordToken);
		}

		Assert.Null(typeClauseNode.GenericParametersListingNode);
		Assert.False(typeClauseNode.IsFabricated);
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);

        // typeClauseNode.TypeIdentifier
        {
			var typeIdentifier = typeClauseNode.TypeIdentifier;

            Assert.False(typeIdentifier.IsFabricated);
            Assert.Equal(SyntaxKind.IntTokenKeyword, typeIdentifier.SyntaxKind);

			// typeIdentifier.TextSpan
			{
				var textSpan = typeIdentifier.TextSpan;

                Assert.Equal(1, textSpan.DecorationByte);
                Assert.Equal(3, textSpan.EndingIndexExclusive);
                Assert.Equal(3, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(0, textSpan.StartingIndexInclusive);
                Assert.Equal("int", textSpan.GetText());
			}
		}

		Assert.Equal(typeof(int), typeClauseNode.ValueType);
	}
	
	[Fact]
	public void PARSE_TypeDefinitionNode()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"public class MyClass
{
}";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();

		// typeDefinitionNode.ChildList
		{
			var i = 0;

			var identifierToken = (IdentifierToken)typeDefinitionNode.ChildList[i++];
			Assert.IsType<IdentifierToken>(identifierToken);

			var codeBlockNode = (CodeBlockNode)typeDefinitionNode.ChildList[i++];
            Assert.IsType<CodeBlockNode>(codeBlockNode);
        }

        Assert.Null(typeDefinitionNode.GenericArgumentsListingNode);
		Assert.Null(typeDefinitionNode.InheritedTypeClauseNode);
		Assert.False(typeDefinitionNode.IsFabricated);
		Assert.False(typeDefinitionNode.IsInterface);
		Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionNode.SyntaxKind);

		// typeDefinitionNode.TypeBodyCodeBlockNode
		{
			var typeBodyCodeBlockNode = typeDefinitionNode.TypeBodyCodeBlockNode;
			Assert.NotNull(typeBodyCodeBlockNode);

            Assert.Empty(typeBodyCodeBlockNode.ChildList);
            Assert.Empty(typeBodyCodeBlockNode.DiagnosticsList);
            Assert.False(typeBodyCodeBlockNode.IsFabricated);
            Assert.Equal(SyntaxKind.CodeBlockNode, typeBodyCodeBlockNode.SyntaxKind);
		}

		// typeDefinitionNode.TypeIdentifier
		{
			var typeIdentifier = typeDefinitionNode.TypeIdentifier;

			Assert.False(typeIdentifier.IsFabricated);
			Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifier.SyntaxKind);

			// typeIdentifier.TextSpan
			{
				var textSpan = typeIdentifier.TextSpan;

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(20, textSpan.EndingIndexExclusive);
                Assert.Equal(7, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(13, textSpan.StartingIndexInclusive);
                Assert.Equal("MyClass", textSpan.GetText());
			}
		}

		Assert.Null(typeDefinitionNode.ValueType);
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
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var usingStatementNode = (UsingStatementNode)topCodeBlock.ChildList.Single();
		Assert.Equal(SyntaxKind.UsingStatementNode, usingStatementNode.SyntaxKind);
	}
	
	[Fact]
	public void PARSE_VariableAssignmentExpressionNode()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var identifierTokenText = "x";
        var expressionText = "2";
        var sourceText = $"{identifierTokenText} = {expressionText};";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList.Single();

		// Assert ChildList
		{
			var i = 0;

			var identifierToken = (IdentifierToken)variableAssignmentExpressionNode.ChildList[i++];
			Assert.IsType<IdentifierToken>(identifierToken);

            var equalsToken = (EqualsToken)variableAssignmentExpressionNode.ChildList[i++];
            Assert.IsType<EqualsToken>(equalsToken);

            var literalExpressionNode = (LiteralExpressionNode)variableAssignmentExpressionNode.ChildList[i++];
            Assert.IsType<LiteralExpressionNode>(literalExpressionNode);
        }

		// Assert EqualsToken
		{
			var equalsToken = variableAssignmentExpressionNode.EqualsToken;

            Assert.False(equalsToken.IsFabricated);
            Assert.Equal(SyntaxKind.EqualsToken, equalsToken.SyntaxKind);

            // Assert EqualsToken.TextSpan
            {
				var textSpan = equalsToken.TextSpan;

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(3, textSpan.EndingIndexExclusive);
                Assert.Equal(1, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(2, textSpan.StartingIndexInclusive);
                Assert.Equal("=", textSpan.GetText());
            }
		}

		// Assert ExpressionNode
		{
			var literalExpressionNode = (LiteralExpressionNode)variableAssignmentExpressionNode.ExpressionNode;

            // Assert ExpressionNode.ChildList
            {
				var i = 0;

				var numericLiteralToken = (NumericLiteralToken)literalExpressionNode.ChildList[i++];
				Assert.IsType<NumericLiteralToken>(numericLiteralToken);

				var typeClauseNode = (TypeClauseNode)literalExpressionNode.ChildList[i++];
                Assert.IsType<TypeClauseNode>(typeClauseNode);
            }

            Assert.False(literalExpressionNode.IsFabricated);

			// Assert ExpressionNode.LiteralSyntaxToken
			{
				var numericLiteralToken = (NumericLiteralToken)literalExpressionNode.LiteralSyntaxToken;

                Assert.False(numericLiteralToken.IsFabricated);
                Assert.Equal(SyntaxKind.NumericLiteralToken, numericLiteralToken.SyntaxKind);

				// Assert ExpressionNode.LiteralSyntaxToken.TextSpan
				{
					var textSpan = numericLiteralToken.TextSpan;

					Assert.Equal(0, textSpan.DecorationByte);
					Assert.Equal(5, textSpan.EndingIndexExclusive);
					Assert.Equal(1, textSpan.Length);
					Assert.Equal(resourceUri, textSpan.ResourceUri);
					Assert.Equal(sourceText, textSpan.SourceText);
					Assert.Equal(4, textSpan.StartingIndexInclusive);
					Assert.Equal("2", textSpan.GetText());
				}
			}

            // Assert ExpressionNode.ResultTypeClauseNode
            {
				var typeClauseNode = literalExpressionNode.ResultTypeClauseNode;
				Assert.IsType<TypeClauseNode>(typeClauseNode);

				// Assert ExpressionNode.ResultTypeClauseNode.ChildList
				{
					var i = 0;

					var identifierToken = (IdentifierToken)typeClauseNode.ChildList[i++];
                    Assert.IsType<IdentifierToken>(identifierToken);
				}

				Assert.Null(typeClauseNode.GenericParametersListingNode);
				Assert.False(typeClauseNode.IsFabricated);
				Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);

                // Assert ExpressionNode.ResultTypeClauseNode.TypeIdentifier
                {
					var typeIdentifierToken = typeClauseNode.TypeIdentifier;
					Assert.IsType<IdentifierToken>(typeIdentifierToken);
                    
					Assert.False(typeIdentifierToken.IsFabricated);
                    Assert.Equal(SyntaxKind.IdentifierToken, typeIdentifierToken.SyntaxKind);
                }

                Assert.Equal(typeof(int), typeClauseNode.ValueType);
            }

            Assert.Equal(SyntaxKind.LiteralExpressionNode, literalExpressionNode.SyntaxKind);
		}

		Assert.False(variableAssignmentExpressionNode.IsFabricated);

		Assert.Equal(SyntaxKind.VariableAssignmentExpressionNode, variableAssignmentExpressionNode.SyntaxKind);

		// Assert VariableIdentifierToken
		{
			var variableIdentifierToken = variableAssignmentExpressionNode.VariableIdentifierToken;

            Assert.False(variableIdentifierToken.IsFabricated);
            Assert.Equal(SyntaxKind.IdentifierToken, variableIdentifierToken.SyntaxKind);

            // Assert VariableIdentifierToken.TextSpan
            {
				var textSpan = variableIdentifierToken.TextSpan;

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(1, textSpan.EndingIndexExclusive);
                Assert.Equal(1, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(0, textSpan.StartingIndexInclusive);
                Assert.Equal("x", textSpan.GetText());
            }
        }
	}
	
	[Fact]
	public void PARSE_VariableDeclarationNode()
	{
        var resourceUri = new ResourceUri("UnitTests");
		var typeIdentifierText = "int";
		var identifierTokenText = "x";
        var sourceText = $"{typeIdentifierText} {identifierTokenText};";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		Assert.Single(topCodeBlock.ChildList);
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList.Single();

        // Assert ChildList
        {
			var i = 0;
            var typeClauseNode = (TypeClauseNode)variableDeclarationNode.ChildList[i++];
            var identifierToken = (IdentifierToken)variableDeclarationNode.ChildList[i++];

			Assert.NotNull(typeClauseNode);
			Assert.NotNull(identifierToken);
        }

        Assert.False(variableDeclarationNode.HasGetter);
		Assert.False(variableDeclarationNode.HasSetter);
		Assert.False(variableDeclarationNode.GetterIsAutoImplemented);
		Assert.False(variableDeclarationNode.SetterIsAutoImplemented);

        // Assert IdentifierToken
        {
			var identifierToken = variableDeclarationNode.IdentifierToken;
			Assert.IsType<IdentifierToken>(identifierToken);

			Assert.False(identifierToken.IsFabricated);
			Assert.Equal(SyntaxKind.IdentifierToken, identifierToken.SyntaxKind);

			// Assert IdentifierToken.TextSpan
			{
				var textSpan = identifierToken.TextSpan;
				Assert.IsType<TextEditorTextSpan>(textSpan);

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(5, textSpan.EndingIndexExclusive);
                Assert.Equal(1, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(4, textSpan.StartingIndexInclusive);
                Assert.Equal(identifierTokenText, textSpan.GetText());
			}
        }

		Assert.False(variableDeclarationNode.IsFabricated);
		Assert.False(variableDeclarationNode.IsInitialized);
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);

		// Assert TypeClauseNode
		{
            var typeClauseNode = variableDeclarationNode.TypeClauseNode;
            Assert.IsType<TypeClauseNode>(typeClauseNode);

            // Assert TypeClauseNode.ChildList
            {
				var i = 0;
				var keywordToken = (KeywordToken)typeClauseNode.ChildList[i++];
				Assert.IsType<KeywordToken>(keywordToken);
            }

            Assert.Null(typeClauseNode.GenericParametersListingNode);
            Assert.False(typeClauseNode.IsFabricated);
            Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);

            // Assert TypeClauseNode.TypeIdentifier
            {
				var typeIdentifier = typeClauseNode.TypeIdentifier;

                Assert.False(typeIdentifier.IsFabricated);
                Assert.Equal(SyntaxKind.IntTokenKeyword, typeIdentifier.SyntaxKind);
				
				// Assert TypeClauseNode.TypeIdentifier
				{
					var textSpan = typeIdentifier.TextSpan;

                    Assert.Equal(1, textSpan.DecorationByte);
                    Assert.Equal(3, textSpan.EndingIndexExclusive);
                    Assert.Equal(3, textSpan.Length);
                    Assert.Equal(resourceUri, textSpan.ResourceUri);
                    Assert.Equal(sourceText, textSpan.SourceText);
                    Assert.Equal(0, textSpan.StartingIndexInclusive);
                    Assert.Equal(typeIdentifierText, textSpan.GetText());
				}
            }

            Assert.Equal(typeof(int), typeClauseNode.ValueType);
        }

		Assert.Equal(VariableKind.Local, variableDeclarationNode.VariableKind);
	}
	
	[Fact]
	public void PARSE_VariableReferenceNode()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = $"int x; x;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var variableReferenceNode = (VariableReferenceNode)topCodeBlock.ChildList[1];

		// variableReferenceNode.ChildList
		{
			var i = 0;

			var identifierToken = variableReferenceNode.ChildList[i++];
			Assert.IsType<IdentifierToken>(identifierToken);

			var variableDeclarationNode = variableReferenceNode.ChildList[i++];
            Assert.IsType<VariableDeclarationNode>(variableDeclarationNode);
		}

		Assert.False(variableReferenceNode.IsFabricated);

		// variableReferenceNode.ResultTypeClauseNode
		{
			var resultTypeClauseNode = variableReferenceNode.ResultTypeClauseNode;

            Assert.Null(resultTypeClauseNode.AttributeNode);

            // resultTypeClauseNode.ChildList
            {
                var keywordToken = resultTypeClauseNode.ChildList.Single();
				Assert.IsType<KeywordToken>(keywordToken);
            }

            Assert.Null(resultTypeClauseNode.GenericParametersListingNode);
            Assert.False(resultTypeClauseNode.IsFabricated);
            Assert.Equal(SyntaxKind.TypeClauseNode, resultTypeClauseNode.SyntaxKind);

			// resultTypeClauseNode.TypeIdentifier
			{
				var typeIdentifier = resultTypeClauseNode.TypeIdentifier;

                Assert.False(typeIdentifier.IsFabricated);
                Assert.Equal(SyntaxKind.IntTokenKeyword, typeIdentifier.SyntaxKind);

				// typeIdentifier.TextSpan
				{
					var textSpan = typeIdentifier.TextSpan;

                    Assert.Equal(1, textSpan.DecorationByte);
                    Assert.Equal(3, textSpan.EndingIndexExclusive);
                    Assert.Equal(3, textSpan.Length);
                    Assert.Equal(resourceUri, textSpan.ResourceUri);
                    Assert.Equal(sourceText, textSpan.SourceText);
                    Assert.Equal(0, textSpan.StartingIndexInclusive);
                    Assert.Equal("int", textSpan.GetText());
				}
			}

            Assert.Equal(typeof(int), resultTypeClauseNode.ValueType);
		}

		Assert.Equal(SyntaxKind.VariableReferenceNode, variableReferenceNode.SyntaxKind);

		// variableReferenceNode.VariableDeclarationNode
		{
			var variableDeclarationNode = variableReferenceNode.VariableDeclarationNode;

			// variableDeclarationNode.ChildList
			{
				int i = 0;

				var typeClauseNode = variableDeclarationNode.ChildList[i++];
				Assert.IsType<TypeClauseNode>(typeClauseNode);

				var identifierToken = variableDeclarationNode.ChildList[i++];
                Assert.IsType<IdentifierToken>(identifierToken);
			}

            Assert.False(variableDeclarationNode.GetterIsAutoImplemented);
            Assert.False(variableDeclarationNode.HasGetter);
            Assert.False(variableDeclarationNode.HasSetter);

			// variableDeclarationNode.IdentifierToken
			{
				var identifierToken = variableDeclarationNode.IdentifierToken;

                Assert.False(identifierToken.IsFabricated);
                Assert.Equal(SyntaxKind.IdentifierToken, identifierToken.SyntaxKind);

				// identifierToken.TextSpan
				{
					var textSpan = identifierToken.TextSpan;

                    Assert.Equal(0, textSpan.DecorationByte);
                    Assert.Equal(5, textSpan.EndingIndexExclusive);
                    Assert.Equal(1, textSpan.Length);
                    Assert.Equal(resourceUri, textSpan.ResourceUri);
                    Assert.Equal(sourceText, textSpan.SourceText);
                    Assert.Equal(4, textSpan.StartingIndexInclusive);
                    Assert.Equal("x", textSpan.GetText());
				}
			}

            Assert.False(variableDeclarationNode.IsFabricated);
            Assert.False(variableDeclarationNode.IsInitialized);
            Assert.False(variableDeclarationNode.SetterIsAutoImplemented);
            Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);

			// variableDeclarationNode.TypeClauseNode
			{
				var typeClauseNode = variableDeclarationNode.TypeClauseNode;

                Assert.Null(typeClauseNode.AttributeNode);

				// typeClauseNode.ChildList
				{
					var keywordToken = typeClauseNode.ChildList.Single();
					Assert.IsType<KeywordToken>(keywordToken);
				}

                Assert.Null(typeClauseNode.GenericParametersListingNode);
                Assert.False(typeClauseNode.IsFabricated);
                Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);

                Assert.Equal(typeof(int), typeClauseNode.ValueType);
			}

            Assert.Equal(VariableKind.Local, variableDeclarationNode.VariableKind);
		}

		// variableReferenceNode.VariableIdentifierToken
		{
			var variableIdentifierToken = variableReferenceNode.VariableIdentifierToken;

            Assert.False(variableIdentifierToken.IsFabricated);
            Assert.Equal(SyntaxKind.IdentifierToken, variableIdentifierToken.SyntaxKind);

			// variableIdentifierToken.TextSpan
			{
				var textSpan = variableIdentifierToken.TextSpan;

                Assert.Equal(0, textSpan.DecorationByte);
                Assert.Equal(8, textSpan.EndingIndexExclusive);
                Assert.Equal(1, textSpan.Length);
                Assert.Equal(resourceUri, textSpan.ResourceUri);
                Assert.Equal(sourceText, textSpan.SourceText);
                Assert.Equal(7, textSpan.StartingIndexInclusive);
				Assert.Equal("x", textSpan.GetText());
			}
		}
	}

    [Fact]
    public void THIS_HAD_A_CRASH_WHEN_RUNNING_USER_TYPES_OUT_CODE()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = VeryLargeTestCase.Value;
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
    }

	/// <summary>
	/// Fixed.
	/// </summary>
    [Fact]
    public void THIS_INFINITE_LOOPED_WHEN_RUNNING_USER_TYPES_OUT_CODE()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"public class MyClass
{
    public MyClass NonsenseMethod()
    {
        MyClass myClass = new MyClass(
";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
    }

    /// <summary>
    /// Fixed.
    /// </summary>
    [Fact]
    public void THIS_CRASHED_WHEN_RUNNING_USER_TYPES_OUT_CODE()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"public class MyClass
{
    public MyClass NonsenseMethod()
    {
        MyClass myClass = new
";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
    }
	
	[Fact]
    public void DELETE_THIS_TEST_A()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"Console.WriteLine(""Hello World"");";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
    }

    [Fact]
    public void DELETE_THIS_TEST_B()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"public void MyMethod() { }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
    }
	
	[Fact]
    public void DELETE_THIS_TEST_C()
    {
		// Infinite loop?

        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"public class MyClass\n{\n    ";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
    }
}
