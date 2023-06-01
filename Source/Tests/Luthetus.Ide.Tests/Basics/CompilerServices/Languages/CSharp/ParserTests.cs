using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp;

public class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_NUMERIC_LITERAL_EXPRESSION()
    {
        string sourceText = "3".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(typeof(int), boundLiteralExpressionNode.ResultType);
    }

    [Fact]
    public void SHOULD_PARSE_STRING_LITERAL_EXPRESSION()
    {
        string sourceText = "\"123abc\"".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(typeof(string), boundLiteralExpressionNode.ResultType);
    }

    [Fact]
    public void SHOULD_PARSE_NUMERIC_BINARY_EXPRESSION()
    {
        string sourceText = "3 + 3".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundBinaryExpressionNode = (BoundBinaryExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(
            typeof(int),
            boundBinaryExpressionNode.LeftBoundExpressionNode.ResultType);

        Assert.Equal(
            typeof(int),
            boundBinaryExpressionNode.BoundBinaryOperatorNode.ResultType);

        Assert.Equal(
            typeof(int),
            boundBinaryExpressionNode.RightBoundExpressionNode.ResultType);
    }

    [Fact]
    public void SHOULD_NOT_PARSE_COMMENT_SINGLE_LINE_STATEMENT()
    {
        string sourceText = @"// C:\Users\hunte\Repos\Aaa\"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Empty(compilationUnit.Children);
    }

    [Fact]
    public void SHOULD_PARSE_VARIABLE_DECLARATION_STATEMENT()
    {
        string sourceText = @"int x;"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundVariableDeclarationStatementNode =
            (BoundVariableDeclarationStatementNode)compilationUnit.Children
                .Single();

        Assert.Equal(
            SyntaxKind.BoundVariableDeclarationStatementNode,
            boundVariableDeclarationStatementNode.SyntaxKind);

        Assert.Equal(
            2,
            boundVariableDeclarationStatementNode.Children.Length);

        var boundTypeNode = (BoundTypeNode)boundVariableDeclarationStatementNode
            .Children[0];

        Assert.Equal(
            SyntaxKind.BoundTypeNode,
            boundTypeNode.SyntaxKind);

        Assert.Equal(
            typeof(int),
            boundTypeNode.Type);

        var identifierToken = boundVariableDeclarationStatementNode.Children[1];

        Assert.Equal(
            SyntaxKind.IdentifierToken,
            identifierToken.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_VARIABLE_DECLARATION_STATEMENT_THEN_VARIABLE_ASSIGNMENT_STATEMENT()
    {
        string sourceText = @"int x;
x = 42;"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Equal(2, compilationUnit.Children.Length);

        var boundVariableDeclarationStatementNode =
            (BoundVariableDeclarationStatementNode)compilationUnit.Children[0];

        Assert.Equal(
            SyntaxKind.BoundVariableDeclarationStatementNode,
            boundVariableDeclarationStatementNode.SyntaxKind);

        var boundVariableAssignmentStatementNode =
            (BoundVariableAssignmentStatementNode)compilationUnit.Children[1];

        Assert.Equal(
            SyntaxKind.BoundVariableAssignmentStatementNode,
            boundVariableAssignmentStatementNode.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_COMPOUND_VARIABLE_DECLARATION_AND_ASSIGNMENT_STATEMENT()
    {
        string sourceText = @"int x = 42;"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Equal(2, compilationUnit.Children.Length);

        var boundVariableDeclarationStatementNode =
            (BoundVariableDeclarationStatementNode)compilationUnit.Children[0];

        Assert.Equal(
            SyntaxKind.BoundVariableDeclarationStatementNode,
            boundVariableDeclarationStatementNode.SyntaxKind);

        var boundVariableAssignmentStatementNode =
            (BoundVariableAssignmentStatementNode)compilationUnit.Children[1];

        Assert.Equal(
            SyntaxKind.BoundVariableAssignmentStatementNode,
            boundVariableAssignmentStatementNode.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT()
    {
        string sourceText = @"void WriteHelloWorldToConsole()
{
}"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundFunctionDeclarationNode =
            (BoundFunctionDeclarationNode)compilationUnit.Children[0];

        Assert.Equal(
            SyntaxKind.BoundFunctionDeclarationNode,
            boundFunctionDeclarationNode.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT()
    {
        string sourceText = @"void WriteHelloWorldToConsole()
{
}

WriteHelloWorldToConsole();"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Equal(2, compilationUnit.Children.Length);

        var boundFunctionDeclarationNode =
            (BoundFunctionDeclarationNode)compilationUnit.Children[0];

        Assert.Equal(
            SyntaxKind.BoundFunctionDeclarationNode,
            boundFunctionDeclarationNode.SyntaxKind);

        var boundFunctionInvocationNode =
            (BoundFunctionInvocationNode)compilationUnit.Children[1];

        Assert.Equal(
            SyntaxKind.BoundFunctionInvocationNode,
            boundFunctionInvocationNode.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_WITH_DIAGNOSTIC_FOR_UNDEFINED_FUNCTION()
    {
        string sourceText = @"printf();"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        // BoundFunctionInvocationNode Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundFunctionInvocationNode =
                (BoundFunctionInvocationNode)compilationUnit.Children.Single();

            Assert.Equal(
                SyntaxKind.BoundFunctionInvocationNode,
                boundFunctionInvocationNode.SyntaxKind);
        }

        // Diagnostic Assertions
        {
            Assert.Single(compilationUnit.Diagnostics);

            var errorDiagnostic = compilationUnit.Diagnostics
                .Single();

            Assert.Equal(
                TextEditorDiagnosticLevel.Error,
                errorDiagnostic.DiagnosticLevel);
        }
    }

    [Fact]
    public void SHOULD_PARSE_CLASS_DECLARATION_EMPTY()
    {
        string classIdentifier = "PersonModel";

        string sourceText = @$"public class {classIdentifier}
{{
}}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var globalScope = parser.Binder.BoundScopes.First();

        var personModel = globalScope.ClassDeclarationMap.Single();

        Assert.Equal(classIdentifier, personModel.Key);

        var boundClassDeclarationNode = 
            (BoundClassDeclarationNode)compilationUnit.Children.Single();

        if (boundClassDeclarationNode.ClassBodyCompilationUnit is null)
            throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

        Assert.Empty(
            boundClassDeclarationNode.ClassBodyCompilationUnit.Children);
    }
    
    [Fact]
    public void SHOULD_PARSE_CLASS_DECLARATION_WHICH_CONTAINS_A_METHOD()
    {
        string classIdentifier = "PersonModel";
        string methodIdentifier = "Walk";

        string sourceText = @$"public class {classIdentifier}
{{
    public void {methodIdentifier}()
    {{
    }}
}}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var globalScope = parser.Binder.BoundScopes.First();

        var personModel = globalScope.ClassDeclarationMap.Single();

        Assert.Equal(classIdentifier, personModel.Key);

        var boundClassDeclarationNode = 
            (BoundClassDeclarationNode)compilationUnit.Children.Single();

        if (boundClassDeclarationNode.ClassBodyCompilationUnit is null)
            throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

        var boundFunctionDeclarationNode = 
            (BoundFunctionDeclarationNode)boundClassDeclarationNode
                .ClassBodyCompilationUnit.Children.Single();

        Assert.Equal(
            SyntaxKind.BoundFunctionDeclarationNode,
            boundFunctionDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void SHOULD_PARSE_CLASS_DECLARATION_WHICH_HAS_PARTIAL_MODIFIER()
    {
        string classIdentifier = "PersonModel";

        string sourceText = @$"public partial class {classIdentifier}
{{
}}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var globalScope = parser.Binder.BoundScopes.First();

        var personModel = globalScope.ClassDeclarationMap.Single();

        Assert.Equal(classIdentifier, personModel.Key);

        var boundClassDeclarationNode = 
            (BoundClassDeclarationNode)compilationUnit.Children.Single();

        if (boundClassDeclarationNode.ClassBodyCompilationUnit is null)
            throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

        Assert.Empty(
            boundClassDeclarationNode.ClassBodyCompilationUnit.Children);
    }
    
    [Fact]
    public void SHOULD_PARSE_CLASS_DECLARATION_WHICH_IS_INHERITING()
    {
        string classIdentifier = "PersonDisplay";

        string sourceText = @$"public class {classIdentifier} : ComponentBase
{{
}}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var globalScope = parser.Binder.BoundScopes.First();

        var personModel = globalScope.ClassDeclarationMap.Single();

        Assert.Equal(classIdentifier, personModel.Key);

        var boundClassDeclarationNode = 
            (BoundClassDeclarationNode)compilationUnit.Children.Single();

        if (boundClassDeclarationNode.ClassBodyCompilationUnit is null)
            throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

        Assert.NotNull(
            boundClassDeclarationNode.BoundInheritanceStatementNode);

        Assert.Empty(
            boundClassDeclarationNode.ClassBodyCompilationUnit.Children);
    }

    /// <summary>GOAL: Add "HelloWorld" key to NamespaceDictionary with a single CompilationUnit child which has a CompilationUnit without any children.</summary>
    [Fact]
    public void SHOULD_PARSE_NAMESPACE_DEFINITION_EMPTY()
    {
        string sourceText = @"namespace HelloWorld {}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var boundNamespaceStatementNode =
            (BoundNamespaceStatementNode)compilationUnit.Children.Single();

        var namespaceCompilationUnit =
            (CompilationUnit)boundNamespaceStatementNode.Children.Single();

        Assert.Empty(namespaceCompilationUnit.Children);

        // Assert SyntaxKinds are correct
        {
            Assert.Equal(
                SyntaxKind.BoundNamespaceStatementNode,
                boundNamespaceStatementNode.SyntaxKind);

            Assert.Equal(
                SyntaxKind.CompilationUnit,
                namespaceCompilationUnit.SyntaxKind);
        }
    }

    [Fact]
    public void SHOULD_PARSE_NAMESPACE_BLOCK_SCOPED()
    {
        var classIdentifier = "PersonModel";

        var sourceText = @$"namespace PersonCase
{{
    public class {classIdentifier}
    {{
    }}
}}"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var boundNamespaceStatementNode =
            (BoundNamespaceStatementNode)compilationUnit.Children.Single();

        var namespaceCompilationUnit =
            (CompilationUnit)boundNamespaceStatementNode.Children.Single();

        var boundClassDeclarationNode =
            (BoundClassDeclarationNode)namespaceCompilationUnit.Children.Single();

        var namespaceScope = parser.Binder.BoundScopes[1];

        var personModel = namespaceScope.ClassDeclarationMap.Single();

        Assert.Equal(classIdentifier, personModel.Key);
    }
    
    [Fact]
    public void SHOULD_PARSE_NAMESPACE_FILE_SCOPED()
    {
        var classIdentifier = "PersonModel";

        var sourceText = @$"namespace PersonCase;

public class {classIdentifier}
{{
}}"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var boundNamespaceStatementNode =
            (BoundNamespaceStatementNode)compilationUnit.Children.Single();

        var namespaceCompilationUnit =
            (CompilationUnit)boundNamespaceStatementNode.Children.Single();

        var boundClassDeclarationNode =
            (BoundClassDeclarationNode)namespaceCompilationUnit.Children.Single();

        var namespaceScope = parser.Binder.BoundScopes[1];

        var personModelKeyValuePair = namespaceScope.ClassDeclarationMap.Single();

        Assert.Equal(classIdentifier, personModelKeyValuePair.Key);
    }
    
    /// <summary>A file scope namespace results in the file not being allowed to have any block namespaces. So this test should result in the proper Diagnostics being reported.</summary>
    [Fact]
    public void SHOULD_PARSE_NAMESPACE_MIXED_FILE_SCOPE_THEN_BLOCK_SCOPE()
    {
        var classIdentifier = "PersonModel";

        var sourceText = @$"namespace PersonCase;

public class {classIdentifier}
{{
}}

namespace Pages
{{
    public class {classIdentifier}
    {{
    }}
}}"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        throw new NotImplementedException();
    }
    
    /// <summary>A file scope namespace results in the file not being allowed to have any block namespaces. So this test should result in the proper Diagnostics being reported.</summary>
    [Fact]
    public void SHOULD_PARSE_NAMESPACE_MIXED_BLOCK_SCOPE_THEN_FILE_SCOPE()
    {
        var classIdentifier = "PersonModel";

        var sourceText = @$"namespace Pages
{{
    public class {classIdentifier}
    {{
    }}
}}

namespace PersonCase;

public class {classIdentifier}
{{
}}"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        throw new NotImplementedException();
    }

    /// <summary>GOAL: Add "PersonCase" key to NamespaceDictionary with two CompilationUnit children: 'PersonModel.cs', and 'PersonDisplay.razor.cs'.<br/><br/>Afterwards convert the Namespace to a BoundScope which would contain the two classes: 'PersonModel', and 'PersonDisplay'</summary>
    [Fact]
    public void SHOULD_PARSE_TWO_NAMESPACE_DECLARATIONS_WITH_THE_SAME_IDENTIFIER_INTO_A_SINGLE_SCOPE()
    {
        var personModelClassIdentifier = "PersonModel";
        var personDisplayClassIdentifier = "PersonDisplay";

        var modelFile = new TestResource(
            "PersonModel.cs",
            @$"namespace PersonCase
{{
    public class {personModelClassIdentifier}
    {{
    }}
}}".ReplaceLineEndings("\n"));

        var displayFile = new TestResource(
            "PersonDisplay.razor.cs",
            @$"namespace PersonCase
{{
    public partial class {personDisplayClassIdentifier} : ComponentBase
    {{
    }}
}}".ReplaceLineEndings("\n"));

        var modelResourceUri = new ResourceUri("PersonModel.cs");
        var displayResourceUri = new ResourceUri("PersonDisplay.razor.cs");

        CompilationUnit modelCompilationUnit;
        CompilationUnit displayCompilationUnit;

        // personModelFile
        var modelLexer = new Lexer(
            modelResourceUri,
            modelFile.Content);

        modelLexer.Lex();

        var modelParser = new Parser(
            modelLexer.SyntaxTokens,
            modelLexer.Diagnostics);

        modelCompilationUnit = modelParser.Parse();

        // personDisplayFile
        var displayLexer = new Lexer(
            displayResourceUri,
            displayFile.Content);

        displayLexer.Lex();

        var displayParser = new Parser(
            displayLexer.SyntaxTokens,
            displayLexer.Diagnostics);

        displayCompilationUnit = displayParser
            .Parse(modelParser.Binder);

        var boundNamespaceStatementNode =
            (BoundNamespaceStatementNode)displayCompilationUnit.Children.Single();

        Assert.Equal(
            SyntaxKind.BoundNamespaceStatementNode,
            boundNamespaceStatementNode.SyntaxKind);

        Assert.Equal(2, boundNamespaceStatementNode.Children.Length);

        // TODO: (2023-05-28) The way a reference to namespaceScope is obtained is hacky and perhaps should be changed. The BoundScopes[2] is at index 2 specifically. Index 0 is global scope. Index 1 is the first time the namespace is declared. Index 2 is the second time the namespace is declared.
        var namespaceScope = displayParser.Binder.BoundScopes[2];

        Assert.Equal(2, namespaceScope.ClassDeclarationMap.Count);

        var personModelKeyValuePair = namespaceScope.ClassDeclarationMap.Single(x => x.Key == personModelClassIdentifier);
        var personDisplayKeyValuePair = namespaceScope.ClassDeclarationMap.Single(x => x.Key == personDisplayClassIdentifier);

        // These assertions feel redundant but the previous code which does a .Single() matching doesn't convey the intention. A Dictionary might not be sorted as one expects as well so cannot constant index lookup.
        Assert.Equal(personModelClassIdentifier, personModelKeyValuePair.Key);
        Assert.Equal(personDisplayClassIdentifier, personDisplayKeyValuePair.Key);
    }

    /// <summary>GOAL: Add "PersonCase" key to NamespaceDictionary with two CompilationUnit children: PersonModel.cs, and PersonDisplay.razor.cs. Afterwards evaluate the Namespace as a BoundScope which would contain the two classes: PersonModel, and PersonDisplay. Afterwards add "Pages" key to NamespaceDictionary with one CompilationUnit child: PersonPage.razor. Have PersonPage.razor.cs include a using statement that includes the "PersonCase" namespace.</summary>
    [Fact]
    public void SHOULD_PARSE_USING_STATEMENT()
    {
        var modelFile = new TestResource(
            "PersonModel.cs",
            @"namespace PersonCase
{
    public class PersonModel
    {
    }
}".ReplaceLineEndings("\n"));

        var displayFile = new TestResource(
            "PersonDisplay.razor.cs",
            @"namespace PersonCase
{
    public partial class PersonDisplay : ComponentBase
    {
    }
}".ReplaceLineEndings("\n"));
        
        var pageFile = new TestResource(
            "PersonPage.razor.cs",
            @"using PersonCase;

namespace Pages
{
    public partial class PersonPage : ComponentBase
    {
    }
}".ReplaceLineEndings("\n"));

        var modelResourceUri = new ResourceUri("PersonModel.cs");
        var displayResourceUri = new ResourceUri("PersonDisplay.razor.cs");
        var pageResourceUri = new ResourceUri("PersonPage.razor.cs");

        CompilationUnit modelCompilationUnit;
        CompilationUnit displayCompilationUnit;
        CompilationUnit pageCompilationUnit;

        // personModelFile
        var modelLexer = new Lexer(
            modelResourceUri,
            modelFile.Content);

        modelLexer.Lex();

        var modelParser = new Parser(
            modelLexer.SyntaxTokens,
            modelLexer.Diagnostics);

        modelCompilationUnit = modelParser.Parse();

        // personDisplayFile
        var displayLexer = new Lexer(
            displayResourceUri,
            displayFile.Content);

        displayLexer.Lex();

        var displayParser = new Parser(
            displayLexer.SyntaxTokens,
            displayLexer.Diagnostics);

        displayCompilationUnit = displayParser
            .Parse(modelParser.Binder);
        
        // personPageFile
        var pageLexer = new Lexer(
            pageResourceUri,
            pageFile.Content);

        pageLexer.Lex();

        var pageParser = new Parser(
            pageLexer.SyntaxTokens,
            pageLexer.Diagnostics);

        pageCompilationUnit = pageParser
            .Parse(displayParser.Binder);

        Assert.Equal(
            2,
            displayParser.Binder.BoundScopes.First().ClassDeclarationMap.Count);
    }
    
    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_CLASS_INSTANCE()
    {
        var sourceText = @"System.Console.WriteLine(""Hello World!"");"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_A_CLASS_INSTANCE_WHICH_IS_NESTED_A_MEMBER_ACCESS_EXPRESSION()
    {
        var sourceText = @"namespace PersonCase;

public class PersonModel
{
    public BodyModel BodyModel { get; set; }
}

public class BodyModel
{
    public void Walk()
    {
    }
}

public class World
{
    private PersonModel _person = new PersonModel();

    public void Tick()
    {
        _person.BodyModel.Walk();
    }
}"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_STATIC_CLASS()
    {
        var sourceText = @"
using System;

Console.WriteLine(""Hello World!"");"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_STATIC_CLASS_WITH_EXPLICIT_NAMESPACE_QUALIFICATION()
    {
        var sourceText = @"System.Console.WriteLine(""Hello World!"");"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_PARSE_TOP_LEVEL_STATEMENTS()
    {
        throw new NotImplementedException("(2023-05-30) I am not sure how I want to test this yet.");
    }

    [Fact]
    public void SHOULD_PARSE_CONDITIONAL_VAR_KEYWORD()
    {
        var sourceText = @"var var = 2;

var x = var * 2;"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_PARSE_VARIABLE_REFERENCE()
    {
        var sourceText = @"private int _count;

private void IncrementCountOnClick()
{
	_count++;
}"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_PARSE_IF_STATEMENT()
    {
        var sourceText = @"if (true)
{
}"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }
}