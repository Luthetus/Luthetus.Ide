using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis;

public class ParserTests
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
			functionDefinitionNode.FunctionIdentifier.TextSpan.GetText());

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
		var genericArgumentText = "int";
		var genericArgumentValueType = typeof(int);
		var sourceText = $@"List<{genericArgumentText}> myList = new List<{genericArgumentText}>();";

		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

        // variableDeclarationNode
        {
            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];
        }

		// variableAssignmentNode
		{
            var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];
        }

        // TODO: Continue working on this test
        throw new NotImplementedException(@"Error Message:
   System.InvalidCastException : Unable to cast object of type 'Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.FunctionInvocationNode' to type 'Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.VariableDeclarationNode'.");
	}
	
	[Fact]
	public void PARSE_GenericArgumentsListingNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = @"public class MyClass<T, U>
{
	public T ItemOne { get; set; }
	public U ItemTwo { get; set; }
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];
		
		var genericParametersListingNode = variableDeclarationNode
			.TypeClauseNode.GenericParametersListingNode;

		Assert.Equal(2, genericParametersListingNode.GenericParameterEntryNodeList.Length);
		Assert.False(genericParametersListingNode.IsFabricated);
		Assert.Equal(SyntaxKind.GenericParametersListingNode, genericParametersListingNode.SyntaxKind);

		// TODO: Continue working on this test
		throw new NotImplementedException(@"Error Message:
   System.InvalidCastException : Unable to cast object of type 'Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.FunctionInvocationNode' to type 'Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.VariableDeclarationNode'.");
	}
	
	[Fact]
	public void PARSE_GenericParameterEntryNode()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = $@"Dictionary<string, int> myMap = new Dictionary<string, int>();";

		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();
		var parser = new CSharpParser(lexer);
		var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];

		throw new NotImplementedException();
	}
	
	[Fact]
	public void PARSE_GenericParametersListingNode()
	{
		var sourceText = $@"Dictionary<string, int> myMap = new Dictionary<string, int>();";

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
		var sourceText = @"if (true) { }";

		throw new NotImplementedException();
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
				var typeClauseNode = (TypeClauseNode)literalExpressionNode.ChildList[i++];
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

                    // Assert ExpressionNode.ResultTypeClauseNode.TypeIdentifier.TextSpan
                    {
						var textSpan = typeIdentifierToken.TextSpan;

                        Assert.Equal(0, textSpan.DecorationByte);
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
        var sourceText = $"x";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var variableReferenceNode = (VariableReferenceNode)topCodeBlock.ChildList.Single();

        throw new NotImplementedException();
	}
}
