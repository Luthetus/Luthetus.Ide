using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
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
    public void SHOULD_PARSE_TOP_LEVEL_STATEMENTS()
    {
        throw new NotImplementedException("(2023-05-30) I am not sure how I want to test this yet.");
    }
}
