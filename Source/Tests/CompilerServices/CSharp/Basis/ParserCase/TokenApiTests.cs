using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase;

/// <summary>
/// <see cref="ParseTokens"/>
/// </summary>
public class TokenApiTests
{
    /// <summary>
    /// <see cref="ParseTokens.ParseNumericLiteralToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseNumericLiteralToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "1";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Single(compilationUnit.RootCodeBlockNode.ChildList);
        var literalExpressionNode = (LiteralExpressionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.IsType<LiteralExpressionNode>(literalExpressionNode);
        Assert.Equal(typeof(int), literalExpressionNode.ResultTypeClauseNode.ValueType);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseStringLiteralToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseStringLiteralToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "\"Hello World\"";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Single(compilationUnit.RootCodeBlockNode.ChildList);
        var literalExpressionNode = (LiteralExpressionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.IsType<LiteralExpressionNode>(literalExpressionNode);
        Assert.Equal(typeof(string), literalExpressionNode.ResultTypeClauseNode.ValueType);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParsePreprocessorDirectiveToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParsePreprocessorDirectiveToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"#";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "MyClass";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Single(compilationUnit.RootCodeBlockNode.ChildList);
        var ambiguousIdentifierNode = (AmbiguousIdentifierNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.IsType<AmbiguousIdentifierNode>(ambiguousIdentifierNode);
        Assert.Equal(sourceText, ambiguousIdentifierNode.IdentifierToken.TextSpan.GetText());
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken_TryParseGenericArguments()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "public class Box<T, U> { }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var typeDefinitionNode = (TypeDefinitionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.IsType<TypeDefinitionNode>(typeDefinitionNode);

        var genericArgumentsListNode = typeDefinitionNode.GenericArgumentsListingNode;
        Assert.NotNull(genericArgumentsListNode);
        Assert.Equal(2, genericArgumentsListNode.GenericArgumentEntryNodeList.Length);

        // First generic argument
        {
            var genericArgumentEntryNode = genericArgumentsListNode.GenericArgumentEntryNodeList[0];
            Assert.Equal("T", genericArgumentEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        }

        // Second generic argument
        {
            var genericArgumentEntryNode = genericArgumentsListNode.GenericArgumentEntryNodeList[1];
            Assert.Equal("U", genericArgumentEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        }
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken_TryParseGenericParameters()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "Dictionary<string, int> map;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var variableDeclarationNode = (VariableDeclarationNode)compilationUnit.RootCodeBlockNode.ChildList[0];

        var genericParametersListingNode = variableDeclarationNode.TypeClauseNode.GenericParametersListingNode;
        Assert.NotNull(genericParametersListingNode);

        // First generic parameter
        {
            var genericParameterEntryNode = genericParametersListingNode.GenericParameterEntryNodeList[0];
            Assert.Equal("string", genericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        }

        // Second generic parameter
        {
            var genericParameterEntryNode = genericParametersListingNode.GenericParameterEntryNodeList[1];
            Assert.Equal("int", genericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        }
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken_TryParseConstructorDefinition()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"public class MyClass
{
    public MyClass()
    {
    }
}";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var typeDefinitionNode = (TypeDefinitionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        
        var constructorDefinitionNode = (ConstructorDefinitionNode)typeDefinitionNode.TypeBodyCodeBlockNode!.ChildList.Single();

        Assert.Equal("MyClass", constructorDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal("MyClass", constructorDefinitionNode.FunctionIdentifier.TextSpan.GetText());
        Assert.Empty(constructorDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
        Assert.Empty(constructorDefinitionNode.FunctionBodyCodeBlockNode!.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken_TryParseTypedIdentifier()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"public string MyMethod()
{
}";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var functionDefinitionNode = (FunctionDefinitionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();

        Assert.Equal("string", functionDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal("MyMethod", functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());
        Assert.Empty(functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
        Assert.Empty(functionDefinitionNode.FunctionBodyCodeBlockNode!.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken_TryParseReference()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"var x; x;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var variableDeclarationNode = (VariableDeclarationNode)compilationUnit.RootCodeBlockNode.ChildList[0];
        var variableReferenceNode = (VariableReferenceNode)compilationUnit.RootCodeBlockNode.ChildList[1];

        // Compare with the separate VariableDeclarationNode
        {
            Assert.Equal("x", variableDeclarationNode.IdentifierToken.TextSpan.GetText());
            Assert.Equal("x", variableReferenceNode.VariableIdentifierToken.TextSpan.GetText());

            Assert.Equal(
                variableDeclarationNode.IdentifierToken.TextSpan.GetText(),
                variableReferenceNode.VariableIdentifierToken.TextSpan.GetText());
        }

        // Compare with the matched VariableDeclarationNode
        {
            Assert.Equal("x", variableReferenceNode.VariableDeclarationNode.IdentifierToken.TextSpan.GetText());
            Assert.Equal("x", variableReferenceNode.VariableIdentifierToken.TextSpan.GetText());

            Assert.Equal(
                variableReferenceNode.VariableDeclarationNode.IdentifierToken.TextSpan.GetText(),
                variableReferenceNode.VariableIdentifierToken.TextSpan.GetText());
        }
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken_TryParseVariableAssignment()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"var x; x = 2;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var variableDeclarationNode = (VariableDeclarationNode)compilationUnit.RootCodeBlockNode.ChildList[0];
        var variableAssignmentNode = (VariableAssignmentExpressionNode)compilationUnit.RootCodeBlockNode.ChildList[1];

        Assert.Equal("x", variableDeclarationNode.IdentifierToken.TextSpan.GetText());
        Assert.Equal("x", variableAssignmentNode.VariableIdentifierToken.TextSpan.GetText());

        Assert.Equal(
            variableDeclarationNode.IdentifierToken.TextSpan.GetText(),
            variableAssignmentNode.VariableIdentifierToken.TextSpan.GetText());

        var literalExpressionNode = (LiteralExpressionNode)variableAssignmentNode.ExpressionNode;
        Assert.Equal(typeof(int), literalExpressionNode.ResultTypeClauseNode.ValueType);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken_TryParseGenericTypeOrFunctionInvocation()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"public T Clone<T>(T item)
{
    return T;
}

Clone<int>(3);";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();

        var functionDefinitionNode = (FunctionDefinitionNode)compilationUnit.RootCodeBlockNode.ChildList[0];
        Assert.IsType<FunctionDefinitionNode>(functionDefinitionNode);

        var functionInvocationNode = (FunctionInvocationNode)compilationUnit.RootCodeBlockNode.ChildList[1];
        Assert.Equal("Clone", functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan.GetText());

        var genericParameterEntryNode = functionInvocationNode.GenericParametersListingNode!.GenericParameterEntryNodeList.Single();
        Assert.Equal("int", genericParameterEntryNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(int), genericParameterEntryNode.TypeClauseNode.ValueType);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken_TryParseFunctionDefinition()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"public void MyMethod()
{
}";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();

        var functionDefinitionNode = (FunctionDefinitionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.Equal("void", functionDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(void), functionDefinitionNode.ReturnTypeClauseNode.ValueType);
        Assert.Equal("MyMethod", functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());
        Assert.Null(functionDefinitionNode.GenericArgumentsListingNode);
        Assert.Empty(functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
        Assert.Empty(functionDefinitionNode.FunctionBodyCodeBlockNode!.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken_TryParseVariableDeclaration()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"int x;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();

        var variableDeclarationNode = (VariableDeclarationNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.Equal("int", variableDeclarationNode.TypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(int), variableDeclarationNode.TypeClauseNode.ValueType);
        Assert.Equal("x", variableDeclarationNode.IdentifierToken.TextSpan.GetText());
    }

    /// <summary>
    /// <see cref="ParseTokens.ResolveAmbiguousIdentifier(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ResolveAmbiguousIdentifier()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ParseTokens.ParsePlusToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParsePlusToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"+";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var literalExpressionNode = (LiteralExpressionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.IsType<LiteralExpressionNode>(literalExpressionNode);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParsePlusPlusToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParsePlusPlusToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"++";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseMinusToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseMinusToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"-";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var literalExpressionNode = (LiteralExpressionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.IsType<LiteralExpressionNode>(literalExpressionNode);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseStarToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseStarToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"*";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var literalExpressionNode = (LiteralExpressionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.IsType<LiteralExpressionNode>(literalExpressionNode);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseDollarSignToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseDollarSignToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"$";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var literalExpressionNode = (LiteralExpressionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.IsType<LiteralExpressionNode>(literalExpressionNode);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseColonToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseColonToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @":";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseOpenBraceToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseOpenBraceToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"{";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseCloseBraceToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseCloseBraceToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"}";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseOpenParenthesisToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseOpenParenthesisToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"(";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        var literalExpressionNode = (LiteralExpressionNode)compilationUnit.RootCodeBlockNode.ChildList.Single();
        Assert.IsType<LiteralExpressionNode>(literalExpressionNode);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseCloseParenthesisToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseCloseParenthesisToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @")";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseOpenAngleBracketToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseOpenAngleBracketToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"<";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseCloseAngleBracketToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseCloseAngleBracketToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @">";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseOpenSquareBracketToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseOpenSquareBracketToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"[";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseCloseSquareBracketToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseCloseSquareBracketToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"]";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseMemberAccessToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseMemberAccessToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @".";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.NotEmpty(compilationUnit.DiagnosticsList);
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseStatementDelimiterToken"/>
    /// </summary>
    [Fact]
    public void StatementDelimiterToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @";";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseKeywordToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseKeywordToken()
    {
        foreach (var keywordToken in CSharpKeywords.NON_CONTEXTUAL_KEYWORDS)
        {
            var resourceUri = new ResourceUri("./unitTesting.txt");
            var sourceText = keywordToken;
            var lexer = new CSharpLexer(resourceUri, sourceText);
            lexer.Lex();
            var parser = new CSharpParser(lexer);

            var compilationUnit = parser.Parse();
        }

        // This test (for now) only serves to ensure typing a
        // keyword and nothing else will NOT throw an exception.
        Assert.True(true);
    }

    /// <summary>
    /// <see cref="ParseTokens.ParseKeywordContextualToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseKeywordContextualToken()
    {
        foreach (var keywordContextualToken in CSharpKeywords.CONTEXTUAL_KEYWORDS)
        {
            var resourceUri = new ResourceUri("./unitTesting.txt");
            var sourceText = keywordContextualToken;
            var lexer = new CSharpLexer(resourceUri, sourceText);
            lexer.Lex();
            var parser = new CSharpParser(lexer);

            var compilationUnit = parser.Parse();
            Assert.Empty(compilationUnit.RootCodeBlockNode.ChildList);
        }
    }
}