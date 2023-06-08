using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT_WITH_KEYWORD_RETURN_TYPE_NO_ARGUMENTS()
    {
        string functionIdentifier = "WriteHelloWorldToConsole";
        string sourceText = @$"void {functionIdentifier}(){{}}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundFunctionDefinitionNode = (BoundFunctionDefinitionNode)compilationUnit.Children[0];

            Assert.Equal(SyntaxKind.BoundFunctionDefinitionNode, boundFunctionDefinitionNode.SyntaxKind);
            Assert.Equal("void", boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.TypeClauseToken.TextSpan.GetText());
            Assert.Equal(functionIdentifier, boundFunctionDefinitionNode.IdentifierToken.TextSpan.GetText());
            Assert.NotNull(boundFunctionDefinitionNode.FunctionBodyCompilationUnit);
            Assert.Empty(boundFunctionDefinitionNode.FunctionBodyCompilationUnit!.Children);
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT_WITH_INTERFACE_RETURN_TYPE_NO_ARGUMENTS()
    {
        string functionIdentifier = "WriteHelloWorldToConsole";
        string sourceText = @$"IPerson {functionIdentifier}(){{}}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundFunctionDefinitionNode = (BoundFunctionDefinitionNode)compilationUnit.Children[0];

            Assert.Equal(SyntaxKind.BoundFunctionDefinitionNode, boundFunctionDefinitionNode.SyntaxKind);
            Assert.Equal("IPerson", boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.TypeClauseToken.TextSpan.GetText());
            Assert.Equal(functionIdentifier, boundFunctionDefinitionNode.IdentifierToken.TextSpan.GetText());
            Assert.NotNull(boundFunctionDefinitionNode.FunctionBodyCompilationUnit);
            Assert.Empty(boundFunctionDefinitionNode.FunctionBodyCompilationUnit!.Children);
        }
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT_WITH_ONE_ARGUMENT()
    {
        string functionReturnTypeClauseText = "void";
        string functionIdentifierText = "WriteHelloWorldToConsole";
        string argumentTypeClauseText = "int";
        string argumentIdentifierText = "times";
        string sourceText = @$"{functionReturnTypeClauseText} {functionIdentifierText}({argumentTypeClauseText} {argumentIdentifierText}) {{}}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);
            var boundFunctionDefinitionNode = (BoundFunctionDefinitionNode)compilationUnit.Children.Single();
            Assert.Equal(SyntaxKind.BoundFunctionDefinitionNode, boundFunctionDefinitionNode.SyntaxKind);

            // Return Type
            Assert.Equal(functionReturnTypeClauseText, boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.TypeClauseToken.TextSpan.GetText());
            Assert.Equal(typeof(void), boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.Type);

            // Function Identifier
            Assert.Equal(functionIdentifierText, boundFunctionDefinitionNode.IdentifierToken.TextSpan.GetText());

            // Argument Type
            var boundClassReferenceNode = (BoundClassReferenceNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[0];
            Assert.Equal(typeof(int), boundClassReferenceNode.Type);
            Assert.Equal(argumentTypeClauseText, boundClassReferenceNode.Identifier.TextSpan.GetText());

            // Argument Identifier
            var boundVariableDeclarationStatementNode = (BoundVariableDeclarationStatementNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[1];
            Assert.Equal(argumentIdentifierText, boundVariableDeclarationStatementNode.IdentifierToken.TextSpan.GetText());

            // Body CompilationUnit
            Assert.Empty(boundFunctionDefinitionNode.FunctionBodyCompilationUnit!.Children);
        }
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT_WITH_TWO_ARGUMENTS()
    {
        string functionReturnTypeClauseText = "void";
        string functionIdentifierText = "WriteHelloWorldToConsole";
        string firstArgumentTypeClauseText = "int";
        string firstArgumentIdentifierText = "times";
        string secondArgumentTypeClauseText = "bool";
        string secondArgumentIdentifierText = "usePurpleText";
        string sourceText = @$"{functionReturnTypeClauseText} {functionIdentifierText}({firstArgumentTypeClauseText} {firstArgumentIdentifierText}, {secondArgumentTypeClauseText} {secondArgumentIdentifierText}){{}}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);
            var boundFunctionDefinitionNode = (BoundFunctionDefinitionNode)compilationUnit.Children.Single();
            Assert.Equal(SyntaxKind.BoundFunctionDefinitionNode, boundFunctionDefinitionNode.SyntaxKind);

            // Return Type
            Assert.Equal(functionReturnTypeClauseText, boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.TypeClauseToken.TextSpan.GetText());
            Assert.Equal(typeof(void), boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.Type);

            // Function Identifier
            Assert.Equal(functionIdentifierText, boundFunctionDefinitionNode.IdentifierToken.TextSpan.GetText());

            // First Argument
            {
                // Argument Type
                var boundClassReferenceNode = (BoundClassReferenceNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[0];
                Assert.Equal(typeof(int), boundClassReferenceNode.Type);
                Assert.Equal(firstArgumentTypeClauseText, boundClassReferenceNode.Identifier.TextSpan.GetText());

                // Argument Identifier
                var boundVariableDeclarationStatementNode = (BoundVariableDeclarationStatementNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[1];
                Assert.Equal(firstArgumentIdentifierText, boundVariableDeclarationStatementNode.IdentifierToken.TextSpan.GetText());
            }

            // Second Argument
            {
                // Argument Type
                var boundClassReferenceNode = (BoundClassReferenceNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[2];
                Assert.Equal(typeof(bool), boundClassReferenceNode.Type);
                Assert.Equal(secondArgumentTypeClauseText, boundClassReferenceNode.Identifier.TextSpan.GetText());

                // Argument Identifier
                var boundVariableDeclarationStatementNode = (BoundVariableDeclarationStatementNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[3];
                Assert.Equal(secondArgumentIdentifierText, boundVariableDeclarationStatementNode.IdentifierToken.TextSpan.GetText());
            }

            // Body CompilationUnit
            Assert.Empty(boundFunctionDefinitionNode.FunctionBodyCompilationUnit!.Children);
        }
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT_WITH_THREE_ARGUMENTS()
    {
        string functionReturnTypeClauseText = "void";
        string functionIdentifierText = "WriteHelloWorldToConsole";
        string firstArgumentTypeClauseText = "int";
        string firstArgumentIdentifierText = "times";
        string secondArgumentTypeClauseText = "bool";
        string secondArgumentIdentifierText = "usePurpleText";
        string thirdArgumentTypeClauseText = "string";
        string thirdArgumentIdentifierText = "additionalText";
        string sourceText = @$"{functionReturnTypeClauseText} {functionIdentifierText}({firstArgumentTypeClauseText} {firstArgumentIdentifierText}, {secondArgumentTypeClauseText} {secondArgumentIdentifierText}, {thirdArgumentTypeClauseText} {thirdArgumentIdentifierText}){{}}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);
            var boundFunctionDefinitionNode = (BoundFunctionDefinitionNode)compilationUnit.Children.Single();
            Assert.Equal(SyntaxKind.BoundFunctionDefinitionNode, boundFunctionDefinitionNode.SyntaxKind);

            // Return Type
            Assert.Equal(functionReturnTypeClauseText, boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.TypeClauseToken.TextSpan.GetText());
            Assert.Equal(typeof(void), boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.Type);

            // Function Identifier
            Assert.Equal(functionIdentifierText, boundFunctionDefinitionNode.IdentifierToken.TextSpan.GetText());

            // First Argument
            {
                // Argument Type
                var boundClassReferenceNode = (BoundClassReferenceNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[0];
                Assert.Equal(typeof(int), boundClassReferenceNode.Type);
                Assert.Equal(firstArgumentTypeClauseText, boundClassReferenceNode.Identifier.TextSpan.GetText());

                // Argument Identifier
                var boundVariableDeclarationStatementNode = (BoundVariableDeclarationStatementNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[1];
                Assert.Equal(firstArgumentIdentifierText, boundVariableDeclarationStatementNode.IdentifierToken.TextSpan.GetText());
            }

            // Second Argument
            {
                // Argument Type
                var boundClassReferenceNode = (BoundClassReferenceNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[2];
                Assert.Equal(typeof(bool), boundClassReferenceNode.Type);
                Assert.Equal(secondArgumentTypeClauseText, boundClassReferenceNode.Identifier.TextSpan.GetText());

                // Argument Identifier
                var boundVariableDeclarationStatementNode = (BoundVariableDeclarationStatementNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[3];
                Assert.Equal(secondArgumentIdentifierText, boundVariableDeclarationStatementNode.IdentifierToken.TextSpan.GetText());
            }
            
            // Third Argument
            {
                // Argument Type
                var boundClassReferenceNode = (BoundClassReferenceNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[4];
                Assert.Equal(typeof(string), boundClassReferenceNode.Type);
                Assert.Equal(thirdArgumentTypeClauseText, boundClassReferenceNode.Identifier.TextSpan.GetText());

                // Argument Identifier
                var boundVariableDeclarationStatementNode = (BoundVariableDeclarationStatementNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[5];
                Assert.Equal(thirdArgumentIdentifierText, boundVariableDeclarationStatementNode.IdentifierToken.TextSpan.GetText());
            }

            // Body CompilationUnit
            Assert.Empty(boundFunctionDefinitionNode.FunctionBodyCompilationUnit!.Children);
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT_WITH_OPTIONAL_ARGUMENT()
    {
        string sourceText = @"void WriteHelloWorldToConsole(int times = 1){}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_NO_ARGUMENT()
    {
        string functionInvocationIdentifier = "WriteToConsole";
        string sourceText = @$"void {functionInvocationIdentifier}(){{}} {functionInvocationIdentifier}();".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            var boundFunctionInvocationNode = (BoundFunctionInvocationNode)compilationUnit.Children[1];

            Assert.Empty(boundFunctionInvocationNode.BoundFunctionParametersNode.BoundFunctionParameterListing);
            Assert.Equal(functionInvocationIdentifier, boundFunctionInvocationNode.IdentifierToken.TextSpan.GetText());
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_WITH_STRING_LITERAL_ARGUMENT()
    {
        string sourceText = @"void WriteToConsole(string input){} WriteToConsole(""Aaa"");".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_WITH_NUMERIC_LITERAL_ARGUMENT()
    {
        string sourceText = @"void WriteToConsole(int input){} WriteToConsole(31);".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_WITH_EXPRESSION_ARGUMENT()
    {
        string sourceText = @"void WriteToConsole(string input){} WriteToConsole(""a"" + ""b"");".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_WITH_VARIABLE_ARGUMENT()
    {
        string sourceText = @"int x = 2; void WriteToConsole(int input){} WriteToConsole(x);".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_HAVING_OUT_VARIABLE_ARGUMENT_REFERENCE()
    {
        string sourceText = @"int x = 2; void WriteToConsole(out int input){} WriteToConsole(out x);".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_HAVING_OUT_VARIABLE_ARGUMENT_DECLARATION()
    {
        string sourceText = @"void WriteToConsole(out int input){} WriteToConsole(out int x);".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_HAVING_REF_VARIABLE_ARGUMENT_REFERENCE()
    {
        string sourceText = @"int x = 2; void WriteToConsole(ref int input){} WriteToConsole(ref x);".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_WITH_DIAGNOSTIC_FOR_UNDEFINED_FUNCTION()
    {
        string sourceText = @"printf();".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            // BoundFunctionInvocationNode
            Assert.Single(compilationUnit.Children);

            var boundFunctionInvocationNode =
                (BoundFunctionInvocationNode)compilationUnit.Children.Single();

            Assert.Equal(
                SyntaxKind.BoundFunctionInvocationNode,
                boundFunctionInvocationNode.SyntaxKind);

            // Diagnostic
            Assert.Single(compilationUnit.Diagnostics);

            var errorDiagnostic = compilationUnit.Diagnostics
                .Single();

            Assert.Equal(
                TextEditorDiagnosticLevel.Error,
                errorDiagnostic.DiagnosticLevel);
        }
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_CLASS_INSTANCE()
    {
        var sourceText = @"TODO".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_STATIC_CLASS_WITH_USING_STATEMENT()
    {
        var sourceText = @"using System; Console.WriteLine(""Hello World!"");".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_STATIC_CLASS_WITH_EXPLICIT_NAMESPACE_QUALIFICATION()
    {
        var sourceText = @"System.Console.WriteLine(""Hello World!"");".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_DEFINITION_WITH_GENERIC_ARGUMENT()
    {
        var genericArgumentIdentifier = "T";
        var functionIdentifier = "Clone";
        var functionArgumentIdentifier = "item";
        var sourceText = @$"public {genericArgumentIdentifier} {functionIdentifier}<{genericArgumentIdentifier}>({genericArgumentIdentifier} {functionArgumentIdentifier}) {{ return {functionArgumentIdentifier}; }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundFunctionDefinitionNode = (BoundFunctionDefinitionNode)compilationUnit.Children[0];

            Assert.Equal(SyntaxKind.BoundFunctionDefinitionNode, boundFunctionDefinitionNode.SyntaxKind);
            Assert.Equal(genericArgumentIdentifier, boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.TypeClauseToken.TextSpan.GetText());
            Assert.Equal(functionIdentifier, boundFunctionDefinitionNode.IdentifierToken.TextSpan.GetText());
            Assert.NotNull(boundFunctionDefinitionNode.FunctionBodyCompilationUnit);

            var argumentBoundClassDefinitionNode = (BoundClassDefinitionNode)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[0];
            Assert.Equal(genericArgumentIdentifier, argumentBoundClassDefinitionNode.TypeClauseToken.TextSpan.GetText());
            var argumentIdentifierToken = (IdentifierToken)boundFunctionDefinitionNode.BoundFunctionArgumentsNode.BoundFunctionArgumentListing[1];
            Assert.Equal(functionArgumentIdentifier, argumentIdentifierToken.TextSpan.GetText());

            Assert.NotNull(boundFunctionDefinitionNode.BoundGenericArgumentsNode);
            Assert.Single(boundFunctionDefinitionNode.BoundGenericArgumentsNode!.BoundGenericArgumentListing);
            var genericBoundClassDefinitionNode = (BoundClassDefinitionNode)boundFunctionDefinitionNode.BoundGenericArgumentsNode.BoundGenericArgumentListing[0];
            Assert.Equal(genericArgumentIdentifier, genericBoundClassDefinitionNode.TypeClauseToken.TextSpan.GetText());
        }
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_DEFINITION_WITH_GENERIC_ARGUMENT_CLAUSE()
    {
        var sourceText = @"public T Clone<T>(T item) where T : class { return item; }".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_WITH_GENERIC_ARGUMENT()
    {
        var sourceText = @"Clone<int>(3){}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
}
