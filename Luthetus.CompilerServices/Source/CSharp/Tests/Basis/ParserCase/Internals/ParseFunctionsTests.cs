using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseFunctionsTests
{
    /*
     # FunctionDefinition with block body
         public int MyMethod() { return 2; }
     # FunctionDefinition with expression body
         public int MyMethod() => 2;
     # FunctionInvocation
         MyMethod();
     # FunctionInvocation with variable assignment
         var x = MyMethod();
     # LocalFunction with block at start
         public int MyMethod() { int LocalFunction() { return 2; } return LocalFunction(); }
     # LocalFunction with block at (somewhere between start and end)
         public int MyMethod() { var y = 2; int LocalFunction() { return 2; } return LocalFunction(); }
     # LocalFunction with block at end
         public int MyMethod() { return LocalFunction(); int LocalFunction() { return 2; } }
     # LocalFunction with expression
         public int MyMethod() { int LocalFunction() => 2; return LocalFunction(); }
     # Delegate
         public delegate Task TextEditorEdit(ITextEditorEditContext editContext);
     # Func
         Func<char, char> toUpperFunc = value => char.ToUpper(value);
     # Action
         Action<string> logConsoleAction = value => Console.WriteLine(value);
     # MethodGroup(?) Passing of a method identifier to match the signature of an Action or Func?
         public void SomeMethod(Action<bool> action) { }
             public void BoolMethod(bool someBool) { }
             SomeMethod(BoolMethod);
     # Lambda expression
         public List<int> SearchIntegers(Func<int, bool> predicate);
             var result = SearchIntegers(integer => integer > 0);
     # Lambda code block
         public List<int> SearchIntegers(Func<int, bool> predicate);
             var result = SearchIntegers(integer =>
                 {
                     return integer > 0;
                 });
     */

    [Fact]
    public void FunctionDefinition_WITH_BlockBody()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "public int MyMethod() { return 2; }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var functionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.ChildList.Single();

        Assert.Equal("int", functionDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(int), functionDefinitionNode.ReturnTypeClauseNode.ValueType);

        Assert.Equal("MyMethod", functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());

        Assert.Null(functionDefinitionNode.GenericArgumentsListingNode);

        Assert.Equal("(", functionDefinitionNode.FunctionArgumentsListingNode.OpenParenthesisToken.TextSpan.GetText());
        Assert.Empty(functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
        Assert.Equal(")", functionDefinitionNode.FunctionArgumentsListingNode.CloseParenthesisToken.TextSpan.GetText());

        Assert.NotNull(functionDefinitionNode.FunctionBodyCodeBlockNode);
        Assert.Single(functionDefinitionNode.FunctionBodyCodeBlockNode.ChildList);

        var returnStatementNode = (ReturnStatementNode)functionDefinitionNode.FunctionBodyCodeBlockNode.ChildList.Single();
        Assert.IsType<ReturnStatementNode>(returnStatementNode);

        Assert.Empty(compilationUnit.DiagnosticsList);
    }

    [Fact]
    public void FunctionDefinition_WITH_ExpressionBody()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "public int MyMethod() => 2;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var functionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.ChildList.Single();

        Assert.Equal("int", functionDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(int), functionDefinitionNode.ReturnTypeClauseNode.ValueType);

        Assert.Equal("MyMethod", functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());

        Assert.Null(functionDefinitionNode.GenericArgumentsListingNode);

        Assert.Equal("(", functionDefinitionNode.FunctionArgumentsListingNode.OpenParenthesisToken.TextSpan.GetText());
        Assert.Empty(functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
        Assert.Equal(")", functionDefinitionNode.FunctionArgumentsListingNode.CloseParenthesisToken.TextSpan.GetText());

        Assert.NotNull(functionDefinitionNode.FunctionBodyCodeBlockNode);
        Assert.Single(functionDefinitionNode.FunctionBodyCodeBlockNode.ChildList);

        var expressionNode = (IExpressionNode)functionDefinitionNode.FunctionBodyCodeBlockNode.ChildList.Single();
        Assert.IsAssignableFrom<IExpressionNode>(expressionNode);

        Assert.Empty(compilationUnit.DiagnosticsList);
    }

    [Fact]
    public void FunctionInvocation()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "MyMethod();";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    [Fact]
    public void FunctionInvocation_WITH_VariableAssignment()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "var x = MyMethod();";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    [Fact]
    public void LocalFunction_WITH_Block_AT_Start()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "public int MyMethod() { int LocalFunction() { return 2; } return LocalFunction(); }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    [Fact]
    public void LocalFunction_WITH_Block_AT_SomewhereBetweenStartAndEnd()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "public int MyMethod() { var y = 2; int LocalFunction() { return 2; } return LocalFunction(); }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    [Fact]
    public void LocalFunction_WITH_Block_AT_End()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "public int MyMethod() { return LocalFunction(); int LocalFunction() { return 2; } }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }
    
    [Fact]
    public void LocalFunction_WITH_Expression_AT_Start()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "public int MyMethod() { int LocalFunction() => 2; return LocalFunction(); }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    [Fact]
    public void LocalFunction_WITH_Expression_AT_SomewhereBetweenStartAndEnd()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "public int MyMethod() { var y = 2; int LocalFunction() => 2; return LocalFunction(); }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    [Fact]
    public void LocalFunction_WITH_Expression_AT_End()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "public int MyMethod() { return LocalFunction(); int LocalFunction() => 2; }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    [Fact]
    public void Delegate()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "public delegate Task TextEditorEdit(ITextEditorEditContext editContext);";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    [Fact]
    public void Func()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "Func<char, char> toUpperFunc = value => char.ToUpper(value);";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    [Fact]
    public void Action()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "Action<string> logConsoleAction = value => Console.WriteLine(value);";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    /// <summary>
    /// Passing of a method identifier to match the signature of an Action or Func?
    /// </summary>
    [Fact]
    public void MethodGroup()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"public void SomeMethod(Action<bool> action) { }
             public void BoolMethod(bool someBool) { };
             SomeMethod(BoolMethod);";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    public void LambdaExpression()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"public List<int> SearchIntegers(Func<int, bool> predicate);
             var result = SearchIntegers(integer => integer > 0);";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }

    public void LambdaCodeBlock()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"public List<int> SearchIntegers(Func<int, bool> predicate);
             var result = SearchIntegers(integer =>
                 {
                     return integer > 0;
                 });";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        throw new NotImplementedException();
    }
}
