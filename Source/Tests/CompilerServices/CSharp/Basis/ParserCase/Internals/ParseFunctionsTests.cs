using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

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

        Assert.Equal(AccessModifierKind.Public, functionDefinitionNode.AccessModifierKind);

        Assert.Equal("int", functionDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(int), functionDefinitionNode.ReturnTypeClauseNode.ValueType);

        Assert.Equal("MyMethod", functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());

        Assert.Null(functionDefinitionNode.GenericArgumentsListingNode);

        Assert.Equal("(", functionDefinitionNode.FunctionArgumentsListingNode.OpenParenthesisToken.TextSpan.GetText());
        Assert.Empty(functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
        Assert.Equal(")", functionDefinitionNode.FunctionArgumentsListingNode.CloseParenthesisToken.TextSpan.GetText());

        Assert.NotNull(functionDefinitionNode.CodeBlockNode);
        Assert.Single(functionDefinitionNode.CodeBlockNode.ChildList);

        var returnStatementNode = (ReturnStatementNode)functionDefinitionNode.CodeBlockNode.ChildList.Single();
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

        Assert.Equal(AccessModifierKind.Public, functionDefinitionNode.AccessModifierKind);

        Assert.Equal("int", functionDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(int), functionDefinitionNode.ReturnTypeClauseNode.ValueType);

        Assert.Equal("MyMethod", functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());

        Assert.Null(functionDefinitionNode.GenericArgumentsListingNode);

        Assert.Equal("(", functionDefinitionNode.FunctionArgumentsListingNode.OpenParenthesisToken.TextSpan.GetText());
        Assert.Empty(functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
        Assert.Equal(")", functionDefinitionNode.FunctionArgumentsListingNode.CloseParenthesisToken.TextSpan.GetText());

        Assert.NotNull(functionDefinitionNode.CodeBlockNode);
        Assert.Single(functionDefinitionNode.CodeBlockNode.ChildList);

        var expressionNode = (IExpressionNode)functionDefinitionNode.CodeBlockNode.ChildList.Single();
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

        Assert.Single(topCodeBlock.ChildList);

        var functionInvocationNode = (FunctionInvocationNode)topCodeBlock.ChildList.Single();
        Assert.Equal("MyMethod", functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan.GetText());
        // In this test the function is undefined
        Assert.Null(functionInvocationNode.FunctionDefinitionNode);
        Assert.Null(functionInvocationNode.GenericParametersListingNode);

        Assert.Equal("(", functionInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.TextSpan.GetText());
        Assert.Empty(functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
        Assert.Equal(")", functionInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.TextSpan.GetText());

        Assert.False(functionInvocationNode.IsFabricated);

        // In this test the function is undefined
        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new DiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedFunction(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
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

        Assert.Equal(2, topCodeBlock.ChildList.Length);

        // variableDeclarationNode
        {
            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];
            Assert.Equal("x", variableDeclarationNode.IdentifierToken.TextSpan.GetText());
        }

        // variableAssignmentNode
        {
            var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];

            var functionInvocationNode = (FunctionInvocationNode)variableAssignmentNode.ExpressionNode;
            
            Assert.Equal("MyMethod", functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan.GetText());
            // In this test the function is undefined
            Assert.Null(functionInvocationNode.FunctionDefinitionNode);
            Assert.Null(functionInvocationNode.GenericParametersListingNode);

            Assert.Equal("(", functionInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.TextSpan.GetText());
            Assert.Empty(functionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
            Assert.Equal(")", functionInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.TextSpan.GetText());

            Assert.False(functionInvocationNode.IsFabricated);
        }

        // In this test the function is undefined
        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new DiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedFunction(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
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

        Assert.Single(topCodeBlock.ChildList);
        var outerFunctionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.ChildList.Single();

        Assert.Equal(AccessModifierKind.Public, outerFunctionDefinitionNode.AccessModifierKind);

        Assert.Equal("int", outerFunctionDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(int), outerFunctionDefinitionNode.ReturnTypeClauseNode.ValueType);
        Assert.Equal("MyMethod", outerFunctionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());
        Assert.Null(outerFunctionDefinitionNode.GenericArgumentsListingNode);
        Assert.Equal("(", outerFunctionDefinitionNode.FunctionArgumentsListingNode.OpenParenthesisToken.TextSpan.GetText());
        Assert.Empty(outerFunctionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
        Assert.Equal(")", outerFunctionDefinitionNode.FunctionArgumentsListingNode.CloseParenthesisToken.TextSpan.GetText());
        Assert.False(outerFunctionDefinitionNode.IsFabricated);
        Assert.NotNull(outerFunctionDefinitionNode.CodeBlockNode);
        Assert.Equal(2, outerFunctionDefinitionNode.CodeBlockNode.ChildList.Length);

        // Local FunctionDefinitionNode
        {
            var localFunctionDefinitionNode = (FunctionDefinitionNode)outerFunctionDefinitionNode.CodeBlockNode.ChildList[0];

            Assert.Equal("int", localFunctionDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
            Assert.Equal(typeof(int), localFunctionDefinitionNode.ReturnTypeClauseNode.ValueType);
            Assert.Equal("LocalFunction", localFunctionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());
            Assert.Null(localFunctionDefinitionNode.GenericArgumentsListingNode);
            Assert.Equal("(", localFunctionDefinitionNode.FunctionArgumentsListingNode.OpenParenthesisToken.TextSpan.GetText());
            Assert.Empty(localFunctionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
            Assert.Equal(")", localFunctionDefinitionNode.FunctionArgumentsListingNode.CloseParenthesisToken.TextSpan.GetText());
            Assert.False(localFunctionDefinitionNode.IsFabricated);
            Assert.NotNull(localFunctionDefinitionNode.CodeBlockNode);
            Assert.Single(localFunctionDefinitionNode.CodeBlockNode.ChildList);

            var localReturnStatementNode = (ReturnStatementNode)localFunctionDefinitionNode.CodeBlockNode.ChildList.Single();
            var boolLiteralExpressionNode = (LiteralExpressionNode)localReturnStatementNode.ExpressionNode;
            Assert.Equal(typeof(int), boolLiteralExpressionNode.ResultTypeClauseNode.ValueType);
            Assert.Equal("2", boolLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
        }

        // ReturnStatementNode
        {
            var outerReturnStatementNode = (ReturnStatementNode)outerFunctionDefinitionNode.CodeBlockNode.ChildList[1];
            var functionInvocationNode = (FunctionInvocationNode)outerReturnStatementNode.ExpressionNode;
            Assert.Equal(typeof(int), functionInvocationNode.ResultTypeClauseNode.ValueType);
        }

        Assert.Empty(compilationUnit.DiagnosticsList);
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
        var sourceText = "public bool MyMethod() { bool LocalFunction() => true; return LocalFunction(); }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var outerFunctionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.ChildList.Single();

        Assert.Equal(AccessModifierKind.Public, outerFunctionDefinitionNode.AccessModifierKind);

        Assert.Equal("bool", outerFunctionDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(bool), outerFunctionDefinitionNode.ReturnTypeClauseNode.ValueType);
        Assert.Equal("MyMethod", outerFunctionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());
        Assert.Null(outerFunctionDefinitionNode.GenericArgumentsListingNode);
        Assert.Equal("(", outerFunctionDefinitionNode.FunctionArgumentsListingNode.OpenParenthesisToken.TextSpan.GetText());
        Assert.Empty(outerFunctionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
        Assert.Equal(")", outerFunctionDefinitionNode.FunctionArgumentsListingNode.CloseParenthesisToken.TextSpan.GetText());
        Assert.False(outerFunctionDefinitionNode.IsFabricated);
        Assert.NotNull(outerFunctionDefinitionNode.CodeBlockNode);
        Assert.Equal(2, outerFunctionDefinitionNode.CodeBlockNode.ChildList.Length);

        // Local FunctionDefinitionNode
        {
            var localFunctionDefinitionNode = (FunctionDefinitionNode)outerFunctionDefinitionNode.CodeBlockNode.ChildList[0];

            Assert.Equal("bool", localFunctionDefinitionNode.ReturnTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());
            Assert.Equal(typeof(bool), localFunctionDefinitionNode.ReturnTypeClauseNode.ValueType);
            Assert.Equal("LocalFunction", localFunctionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText());
            Assert.Null(localFunctionDefinitionNode.GenericArgumentsListingNode);
            Assert.Equal("(", localFunctionDefinitionNode.FunctionArgumentsListingNode.OpenParenthesisToken.TextSpan.GetText());
            Assert.Empty(localFunctionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList);
            Assert.Equal(")", localFunctionDefinitionNode.FunctionArgumentsListingNode.CloseParenthesisToken.TextSpan.GetText());
            Assert.False(localFunctionDefinitionNode.IsFabricated);
            Assert.NotNull(localFunctionDefinitionNode.CodeBlockNode);
            Assert.Single(localFunctionDefinitionNode.CodeBlockNode.ChildList);

            var boolLiteralExpressionNode = (LiteralExpressionNode)localFunctionDefinitionNode.CodeBlockNode.ChildList.Single();
            Assert.Equal(typeof(bool), boolLiteralExpressionNode.ResultTypeClauseNode.ValueType);
            Assert.Equal("true", boolLiteralExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
        }

        // ReturnStatementNode
        {
            var outerReturnStatementNode = (ReturnStatementNode)outerFunctionDefinitionNode.CodeBlockNode.ChildList[1];
            var functionInvocationNode = (FunctionInvocationNode)outerReturnStatementNode.ExpressionNode;
            Assert.Equal(typeof(bool), functionInvocationNode.ResultTypeClauseNode.ValueType);
        }

        Assert.Empty(compilationUnit.DiagnosticsList);
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

    [Fact]
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

    [Fact]
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
