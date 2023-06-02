using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
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

    [Fact]
    public void SHOULD_PARSE_CLASS_DECLARATION_WHICH_HAS_ONE_GENERIC_ARGUMENT()
    {
        string classIdentifier = "Box";
        string genericArgumentIdentifier = "T";

        string sourceText = @$"public class {classIdentifier}<{genericArgumentIdentifier}> : ComponentBase
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
    
    [Fact]
    public void SHOULD_PARSE_CLASS_DECLARATION_WHICH_HAS_THREE_GENERIC_ARGUMENTS()
    {
        string classIdentifier = "Box";
        string genericArgOne = "TItem";
        string genericArgTwo = "TPackager";
        string genericArgThree = "TDeliverer";

        string sourceText = @$"public class {classIdentifier}<{genericArgOne}, {genericArgTwo}, {genericArgThree}> : ComponentBase
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
}
