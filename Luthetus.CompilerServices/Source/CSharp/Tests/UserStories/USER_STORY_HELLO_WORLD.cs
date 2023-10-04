using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.CompilerServices.Lang.CSharp.Tests.Misc;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.UserStories;

/// <summary>
/// User Story Description:
/// User wants to type up a hello world program in the C# programming language.
/// </summary>
public class USER_STORY_HELLO_WORLD : CSharpCompilerServiceTestsBase
{
    [Fact]
    public void SHOULD_COMPILE_SINGLE_LINE_COMMENT()
    {
        var text = HelloWorldInCSharp.SINGLE_LINE_COMMENT;

        var resourceUri = new ResourceUri("TestFile.cs");

        var textEditorModel = new TextEditorModel(
            resourceUri,
            DateTime.UtcNow,
            ".cs",
            text,
            new GenericDecorationMapper(),
            CSharpCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);
        CSharpCompilerService.RegisterResource(resourceUri);

        var compilerServiceResource = CSharpCompilerService.GetCompilerServiceResourceFor(resourceUri);

        Assert.NotNull(compilerServiceResource);

        var cSharpResource = (CSharpResource)compilerServiceResource;

        Assert.NotNull(cSharpResource.CompilationUnit);
        Assert.Empty(cSharpResource.CompilationUnit.DiagnosticsBag);
        Assert.Equal(SyntaxKind.CodeBlockNode, cSharpResource.CompilationUnit.TopLevelStatementsCodeBlockNode.SyntaxKind);

        // The comment should not appear in the parse result.
        // Therefore the children should be empty.
        Assert.Empty(cSharpResource.CompilationUnit.TopLevelStatementsCodeBlockNode.ChildBag);
    }

    [Fact]
    public void SHOULD_COMPILE_NAMESPACE_DEFINITION()
    {
        var text = HelloWorldInCSharp.NAMESPACE_DEFINITION;

        var resourceUri = new ResourceUri("TestFile.cs");

        var textEditorModel = new TextEditorModel(
            resourceUri,
            DateTime.UtcNow,
            ".cs",
            text,
            new GenericDecorationMapper(),
            CSharpCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);
        CSharpCompilerService.RegisterResource(resourceUri);

        var compilerServiceResource = CSharpCompilerService.GetCompilerServiceResourceFor(resourceUri);

        Assert.NotNull(compilerServiceResource);

        var cSharpResource = (CSharpResource)compilerServiceResource;

        Assert.NotNull(cSharpResource.CompilationUnit);
        Assert.Empty(cSharpResource.CompilationUnit.DiagnosticsBag);
        Assert.Equal(SyntaxKind.CodeBlockNode, cSharpResource.CompilationUnit.TopLevelStatementsCodeBlockNode.SyntaxKind);
        Assert.NotNull(cSharpResource.CompilationUnit.Binder);

        var cSharpBinder = (CSharpBinder)cSharpResource.CompilationUnit.Binder;

        // BoundNamespaceStatementNodes should contain the
        //     -'System'
        //     -'HelloWorld'
        // namespaces as of (2023-07-25) 
        //
        // 'System' comes by default.
        // 'HelloWorld' is the namespace that was parsed from the text.
        Assert.Equal(2, cSharpBinder.BoundNamespaceStatementNodes.Count);
        Assert.True(cSharpBinder.BoundNamespaceStatementNodes.ContainsKey("System"));
        Assert.True(cSharpBinder.BoundNamespaceStatementNodes.ContainsKey("HelloWorld"));
    }

    [Fact]
    public void SHOULD_COMPILE_CLASS_DEFINITION()
    {
        var text = HelloWorldInCSharp.CLASS_DEFINITION;

        var resourceUri = new ResourceUri("TestFile.cs");

        var textEditorModel = new TextEditorModel(
            resourceUri,
            DateTime.UtcNow,
            ".cs",
            text,
            new GenericDecorationMapper(),
            CSharpCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);
        CSharpCompilerService.RegisterResource(resourceUri);

        var compilerServiceResource = CSharpCompilerService.GetCompilerServiceResourceFor(resourceUri);

        Assert.NotNull(compilerServiceResource);

        var cSharpResource = (CSharpResource)compilerServiceResource;

        Assert.NotNull(cSharpResource.CompilationUnit);
        Assert.Empty(cSharpResource.CompilationUnit.DiagnosticsBag);
        Assert.Equal(SyntaxKind.CodeBlockNode, cSharpResource.CompilationUnit.TopLevelStatementsCodeBlockNode.SyntaxKind);
        Assert.NotNull(cSharpResource.CompilationUnit.Binder);

        var cSharpBinder = (CSharpBinder)cSharpResource.CompilationUnit.Binder;

        // GlobalScope is known to always be the 'First()' entry as of (2023-07-25).
        var globalScope = cSharpBinder.BoundScopes.First();

        Assert.True(globalScope.TypeDefinitionMap.ContainsKey("Hello"));

        // It is weird to check the globalScope for a user defined class.
        // This is because one usually would expect the class to be placed under a namespace.
        //
        // This UnitTest however is very specific. The text is just a class definition
        // and nothing else. So the global scope is where the class ends up.
    }

    [Fact]
    public void SHOULD_COMPILE_METHOD_DEFINITION()
    {
        var text = HelloWorldInCSharp.METHOD_DEFINITION;

        var resourceUri = new ResourceUri("TestFile.cs");

        var textEditorModel = new TextEditorModel(
            resourceUri,
            DateTime.UtcNow,
            ".cs",
            text,
            new GenericDecorationMapper(),
            CSharpCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);
        CSharpCompilerService.RegisterResource(resourceUri);

        var compilerServiceResource = CSharpCompilerService.GetCompilerServiceResourceFor(resourceUri);

        Assert.NotNull(compilerServiceResource);

        var cSharpResource = (CSharpResource)compilerServiceResource;

        Assert.NotNull(cSharpResource.CompilationUnit);
        Assert.Empty(cSharpResource.CompilationUnit.DiagnosticsBag);

        var topLevelStatementsCodeBlockNode = cSharpResource.CompilationUnit.TopLevelStatementsCodeBlockNode;

        Assert.Equal(SyntaxKind.CodeBlockNode, topLevelStatementsCodeBlockNode.SyntaxKind);
        Assert.Single(topLevelStatementsCodeBlockNode.ChildBag);

        var functionDefinitionNode = (FunctionDefinitionNode)topLevelStatementsCodeBlockNode.ChildBag
            .Single();

        Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionNode.SyntaxKind);

        var functionReturnTypeClauseNode = functionDefinitionNode.ReturnTypeClauseNode;

        Assert.Equal(SyntaxKind.VoidTokenKeyword, functionReturnTypeClauseNode.TypeIdentifier.SyntaxKind);
        Assert.Equal("Main", functionDefinitionNode.FunctionIdentifier.TextSpan.GetText());

        var functionArgumentsListingNode = functionDefinitionNode.FunctionArgumentsListingNode;

        Assert.Single(functionArgumentsListingNode.FunctionArgumentEntryNodeBag);

        var functionArgumentEntryNode = functionArgumentsListingNode.FunctionArgumentEntryNodeBag
            .Single();

        Assert.Equal(SyntaxKind.ArraySyntaxToken, functionArgumentEntryNode.VariableDeclarationStatementNode.TypeClauseNode.TypeIdentifier.SyntaxKind);
        Assert.NotNull(functionArgumentEntryNode.VariableDeclarationStatementNode.TypeClauseNode.GenericParametersListingNode);
        Assert.Single(functionArgumentEntryNode.VariableDeclarationStatementNode.TypeClauseNode.GenericParametersListingNode.GenericParameterEntryNodeBag);

        var stringGenericParameter = functionArgumentEntryNode.VariableDeclarationStatementNode.TypeClauseNode
            .GenericParametersListingNode.GenericParameterEntryNodeBag.Single();

        Assert.Equal(SyntaxKind.StringTokenKeyword, stringGenericParameter.TypeClauseNode.TypeIdentifier.SyntaxKind);
    }

    [Fact]
    public void SHOULD_COMPILE_METHOD_DEFINITION_AS_MEMBER_OF_ENCOMPASSING_CLASS_DEFINITION()
    {
        var text = HelloWorldInCSharp.METHOD_DEFINITION_AS_MEMBER_OF_ENCOMPASSING_CLASS_DEFINITION;

        var resourceUri = new ResourceUri("TestFile.cs");

        var textEditorModel = new TextEditorModel(
            resourceUri,
            DateTime.UtcNow,
            ".cs",
            text,
            new GenericDecorationMapper(),
            CSharpCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);
        CSharpCompilerService.RegisterResource(resourceUri);

        var compilerServiceResource = CSharpCompilerService.GetCompilerServiceResourceFor(resourceUri);

        Assert.NotNull(compilerServiceResource);

        var cSharpResource = (CSharpResource)compilerServiceResource;

        Assert.NotNull(cSharpResource.CompilationUnit);
        Assert.Empty(cSharpResource.CompilationUnit.DiagnosticsBag);

        var topLevelStatementsCodeBlockNode = cSharpResource.CompilationUnit.TopLevelStatementsCodeBlockNode;

        Assert.Equal(SyntaxKind.CodeBlockNode, topLevelStatementsCodeBlockNode.SyntaxKind);
        Assert.Single(topLevelStatementsCodeBlockNode.ChildBag);

        var typeDefinitionNode = (TypeDefinitionNode)topLevelStatementsCodeBlockNode.ChildBag
            .Single();

        Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionNode.SyntaxKind);
        Assert.NotNull(typeDefinitionNode.TypeBodyCodeBlockNode);
        Assert.Single(typeDefinitionNode.TypeBodyCodeBlockNode.ChildBag);

        var functionDefinitionNode = (FunctionDefinitionNode)typeDefinitionNode.TypeBodyCodeBlockNode.ChildBag
            .Single();

        var functionReturnTypeClauseNode = functionDefinitionNode.ReturnTypeClauseNode;

        Assert.Equal(SyntaxKind.VoidTokenKeyword, functionReturnTypeClauseNode.TypeIdentifier.SyntaxKind);
        Assert.Equal("Main", functionDefinitionNode.FunctionIdentifier.TextSpan.GetText());

        var functionArgumentsListingNode = functionDefinitionNode.FunctionArgumentsListingNode;

        Assert.Single(functionArgumentsListingNode.FunctionArgumentEntryNodeBag);

        var functionArgumentEntryNode = functionArgumentsListingNode.FunctionArgumentEntryNodeBag
            .Single();

        Assert.Equal(SyntaxKind.ArraySyntaxToken, functionArgumentEntryNode.VariableDeclarationStatementNode.TypeClauseNode.TypeIdentifier.SyntaxKind);
        Assert.NotNull(functionArgumentEntryNode.VariableDeclarationStatementNode.TypeClauseNode.GenericParametersListingNode);
        Assert.Single(functionArgumentEntryNode.VariableDeclarationStatementNode.TypeClauseNode.GenericParametersListingNode.GenericParameterEntryNodeBag);

        var stringGenericParameter = functionArgumentEntryNode
            .VariableDeclarationStatementNode.TypeClauseNode.GenericParametersListingNode.GenericParameterEntryNodeBag
            .Single();

        Assert.Equal(SyntaxKind.StringTokenKeyword, stringGenericParameter.TypeClauseNode.TypeIdentifier.SyntaxKind);
    }

    [Fact]
    public void SHOULD_RESOLVE_STATIC_CLASS_METHOD_INVOCATION_VIA_EXPLICIT_NAMESPACE_MEMBER_ACCESS()
    {
        var text = HelloWorldInCSharp.STATIC_CLASS_METHOD_INVOCATION;

        var resourceUri = new ResourceUri("TestFile.cs");

        var textEditorModel = new TextEditorModel(
            resourceUri,
            DateTime.UtcNow,
            ".cs",
            text,
            new GenericDecorationMapper(),
            CSharpCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);
        CSharpCompilerService.RegisterResource(resourceUri);

        var compilerServiceResource = CSharpCompilerService.GetCompilerServiceResourceFor(resourceUri);

        Assert.NotNull(compilerServiceResource);

        var cSharpResource = (CSharpResource)compilerServiceResource;

        Assert.NotNull(cSharpResource.CompilationUnit);
        Assert.Empty(cSharpResource.CompilationUnit.DiagnosticsBag);

        var topLevelStatementsCodeBlockNode = cSharpResource.CompilationUnit.TopLevelStatementsCodeBlockNode;

        Assert.Equal(SyntaxKind.CodeBlockNode, topLevelStatementsCodeBlockNode.SyntaxKind);
        Assert.Single(topLevelStatementsCodeBlockNode.ChildBag);

        var functionInvocationNode = (FunctionInvocationNode)topLevelStatementsCodeBlockNode.ChildBag
            .Single();

        Assert.Equal(SyntaxKind.FunctionInvocationNode, functionInvocationNode.SyntaxKind);
        Assert.Equal(3, functionInvocationNode.ChildBag.Length);

        var functionIdentifierToken = (IdentifierToken)functionInvocationNode.ChildBag[0];
        var functionDefinitionNode = (FunctionDefinitionNode)functionInvocationNode.ChildBag[1];
        var functionParametersListingNode = (FunctionParametersListingNode)functionInvocationNode.ChildBag[2];

        Assert.Equal("WriteLine", functionIdentifierToken.TextSpan.GetText());
        Assert.Equal("WriteLine", functionDefinitionNode.FunctionIdentifier.TextSpan.GetText());
        Assert.Empty(functionParametersListingNode.FunctionParameterEntryNodeBag);
    }

    [Fact]
    public void SHOULD_RESOLVE_STATIC_CLASS_METHOD_INVOCATION_WITH_PARAMETER_VIA_EXPLICIT_NAMESPACE_MEMBER_ACCESS()
    {
        var text = HelloWorldInCSharp.STATIC_CLASS_METHOD_INVOCATION_WITH_PARAMETER;

        var resourceUri = new ResourceUri("TestFile.cs");

        var textEditorModel = new TextEditorModel(
            resourceUri,
            DateTime.UtcNow,
            ".cs",
            text,
            new GenericDecorationMapper(),
            CSharpCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);
        CSharpCompilerService.RegisterResource(resourceUri);

        var compilerServiceResource = CSharpCompilerService.GetCompilerServiceResourceFor(resourceUri);

        Assert.NotNull(compilerServiceResource);

        var cSharpResource = (CSharpResource)compilerServiceResource;

        Assert.NotNull(cSharpResource.CompilationUnit);
        Assert.Empty(cSharpResource.CompilationUnit.DiagnosticsBag);

        var topLevelStatementsCodeBlockNode = cSharpResource.CompilationUnit.TopLevelStatementsCodeBlockNode;

        Assert.Equal(SyntaxKind.CodeBlockNode, topLevelStatementsCodeBlockNode.SyntaxKind);
        Assert.Single(topLevelStatementsCodeBlockNode.ChildBag);

        var functionInvocationNode = (FunctionInvocationNode)topLevelStatementsCodeBlockNode.ChildBag
            .Single();

        Assert.Equal(SyntaxKind.FunctionInvocationNode, functionInvocationNode.SyntaxKind);
        Assert.Equal(3, functionInvocationNode.ChildBag.Length);

        var functionIdentifierToken = (IdentifierToken)functionInvocationNode.ChildBag[0];
        var functionDefinitionNode = (FunctionDefinitionNode)functionInvocationNode.ChildBag[1];
        var functionParametersListingNode = (FunctionParametersListingNode)functionInvocationNode.ChildBag[2];

        Assert.Equal("WriteLine", functionIdentifierToken.TextSpan.GetText());
        Assert.Equal("WriteLine", functionDefinitionNode.FunctionIdentifier.TextSpan.GetText());
        Assert.Single(functionParametersListingNode.FunctionParameterEntryNodeBag);
    }

    [Fact]
    public void SHOULD_COMPILE_COMPLETE_PROGRAM()
    {
        var text = HelloWorldInCSharp.COMPLETE_PROGRAM.ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri("TestFile.cs");

        var textEditorModel = new TextEditorModel(
            resourceUri,
            DateTime.UtcNow,
            ".cs",
            text,
            new GenericDecorationMapper(),
            CSharpCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);
        CSharpCompilerService.RegisterResource(resourceUri);

        var compilerServiceResource = CSharpCompilerService.GetCompilerServiceResourceFor(resourceUri);

        Assert.NotNull(compilerServiceResource);

        var cSharpResource = (CSharpResource)compilerServiceResource;

        Assert.NotNull(cSharpResource.CompilationUnit);
        Assert.Empty(cSharpResource.CompilationUnit.DiagnosticsBag);

        var syntaxTokens = cSharpResource.SyntaxTokens;
        Assert.Equal(30, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.CommentSingleLineToken, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.NamespaceTokenKeyword, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.ClassTokenKeyword, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.StaticTokenKeyword, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.VoidTokenKeyword, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[10].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[11].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenSquareBracketToken, syntaxTokens[12].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseSquareBracketToken, syntaxTokens[13].SyntaxKind);
        Assert.Equal(SyntaxKind.ArgsTokenContextualKeyword, syntaxTokens[14].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[15].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[16].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[17].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[18].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[19].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[20].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[21].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[22].SyntaxKind);
        Assert.Equal(SyntaxKind.StringLiteralToken, syntaxTokens[23].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[24].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[25].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[26].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[27].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[28].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[29].SyntaxKind);

        var symbols = cSharpResource.Symbols;
        Assert.Equal(6, symbols.Length);
        Assert.Equal(SyntaxKind.NamespaceSymbol, symbols[0].SyntaxKind);
        Assert.Equal(SyntaxKind.TypeSymbol, symbols[1].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[2].SyntaxKind);
        Assert.Equal(SyntaxKind.NamespaceSymbol, symbols[3].SyntaxKind);
        Assert.Equal(SyntaxKind.TypeSymbol, symbols[4].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[5].SyntaxKind);
    }

    /// <summary>
    /// The <see cref="HelloWorldInCSharp"/> class is intended
    /// to break down the entirety of a 'Hello World' program,
    /// which is written in C#, into its most basic parts.
    /// <br/><br/>
    /// There will then be individual Unit Tests for each basic part
    /// of the 'Hello World' program.
    /// <br/><br/>
    /// In the end, a final Unit Test will be written which
    /// takes as input the entirety of the 'Hello World' program
    /// </summary>
    public class HelloWorldInCSharp
    {
        public const string COMPLETE_PROGRAM = @"// Hello World! program
namespace HelloWorld
{
    class Hello 
    {         
        static void Main(string[] args)
        {
            System.Console.WriteLine(""Hello World!"");
        }
    }
}";

        public const string SINGLE_LINE_COMMENT = @"// Hello World! program";

        public const string NAMESPACE_DEFINITION = @"namespace HelloWorld
{
}";

        public const string CLASS_DEFINITION = @"class Hello
{         
}";

        /// <summary>
        /// Method and function are separate terms but, I'm speaking rather loosely here.
        /// </summary>
        public const string METHOD_DEFINITION = @"static void Main(string[] args)
{
}";

        public const string METHOD_DEFINITION_AS_MEMBER_OF_ENCOMPASSING_CLASS_DEFINITION = @"class Hello 
{         
    static void Main(string[] args)
    {
    }
}";

        public const string METHOD_INVOCATION_STATEMENT = @"System.Console.WriteLine(""Hello World!"");";

        /// <summary>
        /// This input is incomplete. However, the test is to look in
        /// the namespace definitions and find the 'System' namespace.
        /// </summary>
        public const string EXPLICIT_NAMESPACE_QUALIFICATION = @"System.";
        public const string STATIC_CLASS_REFERENCE = @"System.Console";
        public const string STATIC_CLASS_MEMBER_ACCESS = @"System.Console.WriteLine";
        public const string STATIC_CLASS_METHOD_INVOCATION = @"System.Console.WriteLine();";

        /// <summary>
        /// The <see cref="STATIC_CLASS_METHOD_INVOCATION_WITH_PARAMETER"/> is equal to
        /// <see cref="METHOD_INVOCATION_STATEMENT"/>
        /// </summary>
        public const string STATIC_CLASS_METHOD_INVOCATION_WITH_PARAMETER = @"System.Console.WriteLine(""Hello World!"");";
    }
}