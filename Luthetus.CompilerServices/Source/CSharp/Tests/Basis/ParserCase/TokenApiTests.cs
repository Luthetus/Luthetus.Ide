using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase;

/// <summary>
/// <see cref="TokenApi"/>
/// </summary>
public class TokenApiTests
{
    /// <summary>
    /// <see cref="TokenApi.ParseNumericLiteralToken(ParserModel)"/>
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
    /// <see cref="TokenApi.ParseStringLiteralToken(ParserModel)"/>
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
    /// <see cref="TokenApi.ParsePreprocessorDirectiveToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParsePreprocessorDirectiveToken()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseIdentifierToken(ParserModel)"/>
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
    /// <see cref="TokenApi.ParseIdentifierToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseIdentifierToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseIdentifierToken_TryParseGenericParameters()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "Dictionary<string, int> map = new Dictionary<string, int>();";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseIdentifierToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseIdentifierToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseIdentifierToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseIdentifierToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseIdentifierToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseIdentifierToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseIdentifierToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ResolveAmbiguousIdentifier(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ResolveAmbiguousIdentifier()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParsePlusToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParsePlusPlusToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseMinusToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseStarToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseDollarSignToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseColonToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseOpenBraceToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseCloseBraceToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseOpenParenthesisToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseCloseParenthesisToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseOpenAngleBracketToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseCloseAngleBracketToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseOpenSquareBracketToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseCloseSquareBracketToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseMemberAccessToken(ParserModel)"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.StatementDelimiterToken"/>
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseKeywordToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseKeywordToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"public";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TokenApi.ParseKeywordContextualToken(ParserModel)"/>
    /// </summary>
    [Fact]
    public void ParseKeywordContextualToken()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = @"var";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        var compilationUnit = parser.Parse();
        throw new NotImplementedException();
    }
}