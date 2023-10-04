using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.CompilerServices.Lang.CSharp.Tests.Misc;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.UserStories;

/// <summary>
/// User Story Description:
/// User wants to type up a "Program.cs" file which uses top level statements in the C# programming language.
/// </summary>
public class USER_STORY_PROGRAM_CS_FILE_TOP_LEVEL_STATEMENTS : CSharpCompilerServiceTestsBase
{
    [Fact]
    public void SHOULD_COMPILE_USING_BLAZOR_WASM_APP_STATEMENT()
    {
        string text = ProgramCsFileTopLevelStatements.USING_BLAZOR_WASM_APP_STATEMENT.ReplaceLineEndings("\n");

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
        Assert.Equal(4, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.UsingTokenKeyword, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[3].SyntaxKind);

        var symbols = cSharpResource.Symbols;
        Assert.Single(symbols);
        Assert.Equal(SyntaxKind.NamespaceSymbol, symbols[0].SyntaxKind);
    }

    [Fact]
    public void SHOULD_COMPILE_USING_MICROSOFT_ASPNETCORE_COMPONENTS_WEB_STATEMENT()
    {
        string text = ProgramCsFileTopLevelStatements.USING_MICROSOFT_ASPNETCORE_COMPONENTS_WEB_STATEMENT.ReplaceLineEndings("\n");

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
        Assert.Equal(10, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.UsingTokenKeyword, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[9].SyntaxKind);

        var symbols = cSharpResource.Symbols;
        Assert.Single(symbols);
        Assert.Equal(SyntaxKind.NamespaceSymbol, symbols[0].SyntaxKind);
    }
    
    [Fact]
    public void SHOULD_COMPILE_USING_MICROSOFT_ASPNETCORE_COMPONENTS_WEBASSEMBLY_HOSTING_STATEMENT()
    {
        string text = ProgramCsFileTopLevelStatements.USING_MICROSOFT_ASPNETCORE_COMPONENTS_WEBASSEMBLY_HOSTING_STATEMENT.ReplaceLineEndings("\n");

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
        Assert.Equal(12, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.UsingTokenKeyword, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[10].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[11].SyntaxKind);

        var symbols = cSharpResource.Symbols;
        Assert.Single(symbols);
        Assert.Equal(SyntaxKind.NamespaceSymbol, symbols[0].SyntaxKind);
    }
    
    [Fact]
    public void SHOULD_COMPILE_VARIABLE_DECLARATION_AND_ASSIGNMENT_STATEMENT()
    {
        string text = ProgramCsFileTopLevelStatements.VARIABLE_DECLARATION_AND_ASSIGNMENT_STATEMENT.ReplaceLineEndings("\n");

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
        Assert.Equal(11, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.VarTokenContextualKeyword, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.ArgsTokenContextualKeyword, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[10].SyntaxKind);

        var symbols = cSharpResource.Symbols;
        Assert.Equal(2, symbols.Length);
        // Declaration of Variable
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[0].SyntaxKind);
        // Assignment of Variable
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[1].SyntaxKind);
    }
    
    [Fact]
    public void SHOULD_COMPILE_APP_GENERIC_FUNCTION_INVOCATION_STATEMENT()
    {
        string text = ProgramCsFileTopLevelStatements.APP_GENERIC_FUNCTION_INVOCATION_STATEMENT.ReplaceLineEndings("\n");

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
        Assert.Equal(13, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenAngleBracketToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseAngleBracketToken, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.StringLiteralToken, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[10].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[11].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[12].SyntaxKind);

        var symbols = cSharpResource.Symbols;
        Assert.Equal(3, symbols.Length);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[0].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[1].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[2].SyntaxKind);
    }
    
    [Fact]
    public void SHOULD_COMPILE_HEAD_OUTLET_GENERIC_FUNCTION_INVOCATION_STATEMENT()
    {
        string text = ProgramCsFileTopLevelStatements.HEAD_OUTLET_GENERIC_FUNCTION_INVOCATION_STATEMENT.ReplaceLineEndings("\n");

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
        Assert.Equal(13, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenAngleBracketToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseAngleBracketToken, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.StringLiteralToken, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[10].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[11].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[12].SyntaxKind);

        var symbols = cSharpResource.Symbols;
        Assert.Equal(3, symbols.Length);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[0].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[1].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[2].SyntaxKind);
    }
    
    [Fact]
    public void SHOULD_COMPILE_FUNCTION_INVOCATION_WITH_LAMBDA_EXPRESSION_PARAMETER_STATEMENT()
    {
        string text = ProgramCsFileTopLevelStatements.FUNCTION_INVOCATION_WITH_LAMBDA_EXPRESSION_PARAMETER_STATEMENT.ReplaceLineEndings("\n");

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
        Assert.Equal(27, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseAngleBracketToken, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.NewTokenKeyword, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[10].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[11].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[12].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[13].SyntaxKind);
        Assert.Equal(SyntaxKind.NewTokenKeyword, syntaxTokens[14].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[15].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[16].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[17].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[18].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[19].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[20].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[21].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[22].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[23].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[24].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[25].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[26].SyntaxKind);

        var symbols = cSharpResource.Symbols;
        Assert.Equal(3, symbols.Length);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[0].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[1].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[2].SyntaxKind);
    }
    
    [Fact]
    public void SHOULD_COMPILE_ASYNC_FUNCTION_INVOCATION_STATEMENT()
    {
        string text = ProgramCsFileTopLevelStatements.ASYNC_FUNCTION_INVOCATION_STATEMENT.ReplaceLineEndings("\n");

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
        Assert.Equal(12, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.AwaitTokenContextualKeyword, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[10].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[11].SyntaxKind);

        var symbols = cSharpResource.Symbols;
        Assert.Equal(3, symbols.Length);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[0].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[1].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[2].SyntaxKind);
    }

    [Fact]
    public void SHOULD_COMPILE_COMPLETE_PROGRAM()
    {
        var text = ProgramCsFileTopLevelStatements.COMPLETE_PROGRAM.ReplaceLineEndings("\n");

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
        Assert.Equal(95, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.UsingTokenKeyword, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.UsingTokenKeyword, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[10].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[11].SyntaxKind);
        Assert.Equal(SyntaxKind.UsingTokenKeyword, syntaxTokens[12].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[13].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[14].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[15].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[16].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[17].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[18].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[19].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[20].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[21].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[22].SyntaxKind);
        Assert.Equal(SyntaxKind.VarTokenContextualKeyword, syntaxTokens[23].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[24].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[25].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[26].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[27].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[28].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[29].SyntaxKind);
        Assert.Equal(SyntaxKind.ArgsTokenContextualKeyword, syntaxTokens[30].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[31].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[32].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[33].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[34].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[35].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[36].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[37].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenAngleBracketToken, syntaxTokens[38].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[39].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseAngleBracketToken, syntaxTokens[40].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[41].SyntaxKind);
        Assert.Equal(SyntaxKind.StringLiteralToken, syntaxTokens[42].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[43].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[44].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[45].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[46].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[47].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[48].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[49].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenAngleBracketToken, syntaxTokens[50].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[51].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseAngleBracketToken, syntaxTokens[52].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[53].SyntaxKind);
        Assert.Equal(SyntaxKind.StringLiteralToken, syntaxTokens[54].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[55].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[56].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[57].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[58].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[59].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[60].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[61].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[62].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[63].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[64].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseAngleBracketToken, syntaxTokens[65].SyntaxKind);
        Assert.Equal(SyntaxKind.NewTokenKeyword, syntaxTokens[66].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[67].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[68].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[69].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[70].SyntaxKind);
        Assert.Equal(SyntaxKind.NewTokenKeyword, syntaxTokens[71].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[72].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[73].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[74].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[75].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[76].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[77].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[78].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[79].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[80].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[81].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[82].SyntaxKind);
        Assert.Equal(SyntaxKind.AwaitTokenContextualKeyword, syntaxTokens[83].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[84].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[85].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[86].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[87].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[88].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[89].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[90].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[91].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[92].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[93].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[94].SyntaxKind);

        var symbols = cSharpResource.Symbols;
        Assert.Equal(16, symbols.Length);
        Assert.Equal(SyntaxKind.NamespaceSymbol, symbols[0].SyntaxKind);
        Assert.Equal(SyntaxKind.NamespaceSymbol, symbols[1].SyntaxKind);
        Assert.Equal(SyntaxKind.NamespaceSymbol, symbols[2].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[3].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[4].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[5].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[6].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[7].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[8].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[9].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[10].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[11].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[12].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[13].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[14].SyntaxKind);
        Assert.Equal(SyntaxKind.FunctionSymbol, symbols[15].SyntaxKind);
    }

    /// <summary>
    /// The <see cref="ProgramCsFileTopLevelStatements"/> class is intended
    /// to break down the entirety of a 'ProgramCsFileTopLevelStatements' program,
    /// which is written in C#, into its most basic parts.
    /// <br/><br/>
    /// There will then be individual Unit Tests for each basic part
    /// of the 'ProgramCsFileTopLevelStatements' program.
    /// <br/><br/>
    /// In the end, a final Unit Test will be written which
    /// takes as input the entirety of the 'ProgramCsFileTopLevelStatements' program
    /// </summary>
    public class ProgramCsFileTopLevelStatements
    {
        public const string COMPLETE_PROGRAM = @"using BlazorWasmApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>(""#app"");
builder.RootComponents.Add<HeadOutlet>(""head::after"");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
";

        public const string USING_BLAZOR_WASM_APP_STATEMENT = @"using BlazorWasmApp;";
        public const string USING_MICROSOFT_ASPNETCORE_COMPONENTS_WEB_STATEMENT = @"using Microsoft.AspNetCore.Components.Web;";
        public const string USING_MICROSOFT_ASPNETCORE_COMPONENTS_WEBASSEMBLY_HOSTING_STATEMENT = @"using Microsoft.AspNetCore.Components.WebAssembly.Hosting;";

        public const string VARIABLE_DECLARATION_AND_ASSIGNMENT_STATEMENT = @"var builder = WebAssemblyHostBuilder.CreateDefault(args);";
        
        public const string APP_GENERIC_FUNCTION_INVOCATION_STATEMENT = @"builder.RootComponents.Add<App>(""#app"");";
        public const string HEAD_OUTLET_GENERIC_FUNCTION_INVOCATION_STATEMENT = @"builder.RootComponents.Add<HeadOutlet>(""head::after"");";
        
        public const string FUNCTION_INVOCATION_WITH_LAMBDA_EXPRESSION_PARAMETER_STATEMENT = @"builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });";
        
        public const string ASYNC_FUNCTION_INVOCATION_STATEMENT = @"await builder.Build().RunAsync();";
    }
}