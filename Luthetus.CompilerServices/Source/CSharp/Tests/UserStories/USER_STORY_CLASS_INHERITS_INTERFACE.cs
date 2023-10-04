using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.CompilerServices.Lang.CSharp.Tests.Misc;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.UserStories;

/// <summary>
/// User Story Description:
/// User wants to have a class inherit an interface. I want this to eventually
/// be a full on CRUD application but I'm trying various things and getting a plan going.
/// </summary>
public class USER_STORY_CLASS_INHERITS_INTERFACE : CSharpCompilerServiceTestsBase
{
    [Fact]
    public void SHOULD_COMPILE_NAMESPACE_DEFINITION()
    {
        var text = ClassInheritsInterface.NAMESPACE_DEFINITION;

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

        // Template for Asserting over the SyntaxTokens
        var syntaxTokens = cSharpResource.SyntaxTokens;
        Assert.Equal(6, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.NamespaceTokenKeyword, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[5].SyntaxKind);

        // Template for Asserting over the Symbols
        var symbols = cSharpResource.Symbols;
        Assert.Single(symbols);
        Assert.Equal(SyntaxKind.NamespaceSymbol, symbols[0].SyntaxKind);
    }

    [Fact]
    public void SHOULD_COMPILE_INTERFACE_DEFINITION()
    {
        var text = ClassInheritsInterface.INTERFACE_DEFINITION;

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

        // Template for Asserting over the SyntaxTokens
        var syntaxTokens = cSharpResource.SyntaxTokens;
        Assert.Equal(34, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.InterfaceTokenKeyword, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[10].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[11].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[12].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[13].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[14].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[15].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[16].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[17].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[18].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[19].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[20].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[21].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[22].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[23].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[24].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[25].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[26].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[27].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[28].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[29].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[30].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[31].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[32].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[33].SyntaxKind);

        // Template for Asserting over the Symbols
        var symbols = cSharpResource.Symbols;
        Assert.Equal(3, symbols.Length);
        Assert.Equal(SyntaxKind.TypeSymbol, symbols[0].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[1].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[2].SyntaxKind);
    }

    [Fact]
    public void SHOULD_COMPILE_CLASS_DEFINITION()
    {
        var text = ClassInheritsInterface.CLASS_DEFINITION;

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

        // Template for Asserting over the SyntaxTokens
        var syntaxTokens = cSharpResource.SyntaxTokens;
        Assert.Equal(63, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.ClassTokenKeyword, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.ColonToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[10].SyntaxKind);
        Assert.Equal(SyntaxKind.CommaToken, syntaxTokens[11].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[12].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[13].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[14].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[15].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[16].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[17].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[18].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[19].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[20].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[21].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[22].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[23].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[24].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[25].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[26].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[27].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[28].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[29].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[30].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[31].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[32].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[33].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[34].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[35].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[36].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[37].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[38].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[39].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[40].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[41].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[42].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[43].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[44].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[45].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[46].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[47].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[48].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[49].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[50].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[51].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[52].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[53].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[54].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[55].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[56].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseAngleBracketToken, syntaxTokens[57].SyntaxKind);
        Assert.Equal(SyntaxKind.DollarSignToken, syntaxTokens[58].SyntaxKind);
        Assert.Equal(SyntaxKind.StringLiteralToken, syntaxTokens[59].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[60].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[61].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[62].SyntaxKind);

        // Template for Asserting over the Symbols
        var symbols = cSharpResource.Symbols;
        Assert.Equal(9, symbols.Length);
        Assert.Equal(SyntaxKind.TypeSymbol, symbols[0].SyntaxKind);
        Assert.Equal(SyntaxKind.TypeSymbol, symbols[1].SyntaxKind);
        Assert.Equal(SyntaxKind.TypeSymbol, symbols[2].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[3].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[4].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[5].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[6].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[7].SyntaxKind);
        Assert.Equal(SyntaxKind.StringInterpolationSymbol, symbols[8].SyntaxKind);
    }

    [Fact]
    public void SHOULD_COMPILE_COMPLETE_PROGRAM()
    {
        var text = ClassInheritsInterface.COMPLETE_PROGRAM;

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

        // Template for Asserting over the SyntaxTokens
        var syntaxTokens = cSharpResource.SyntaxTokens;
        Assert.Equal(101, syntaxTokens.Length);
        Assert.Equal(SyntaxKind.NamespaceTokenKeyword, syntaxTokens[0].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[1].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[2].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[3].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[4].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[5].SyntaxKind);
        Assert.Equal(SyntaxKind.InterfaceTokenKeyword, syntaxTokens[6].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[7].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[8].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[9].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[10].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[11].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[12].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[13].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[14].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[15].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[16].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[17].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[18].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[19].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[20].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[21].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[22].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[23].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[24].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[25].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[26].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[27].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[28].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[29].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[30].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[31].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[32].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[33].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[34].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[35].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[36].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[37].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[38].SyntaxKind);
        Assert.Equal(SyntaxKind.ClassTokenKeyword, syntaxTokens[39].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[40].SyntaxKind);
        Assert.Equal(SyntaxKind.ColonToken, syntaxTokens[41].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[42].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[43].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[44].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[45].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[46].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[47].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[48].SyntaxKind);
        Assert.Equal(SyntaxKind.CommaToken, syntaxTokens[49].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[50].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[51].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[52].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[53].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[54].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[55].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[56].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[57].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[58].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[59].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[60].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[61].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[62].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[63].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[64].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[65].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[66].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[67].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[68].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[69].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[70].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[71].SyntaxKind);
        Assert.Equal(SyntaxKind.MemberAccessToken, syntaxTokens[72].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[73].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, syntaxTokens[74].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, syntaxTokens[75].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[76].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[77].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[78].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[79].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[80].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[81].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[82].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[83].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[84].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[85].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[86].SyntaxKind);
        Assert.Equal(SyntaxKind.OpenBraceToken, syntaxTokens[87].SyntaxKind);
        Assert.Equal(SyntaxKind.GetTokenContextualKeyword, syntaxTokens[88].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[89].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[90].SyntaxKind);
        Assert.Equal(SyntaxKind.PublicTokenKeyword, syntaxTokens[91].SyntaxKind);
        Assert.Equal(SyntaxKind.StringTokenKeyword, syntaxTokens[92].SyntaxKind);
        Assert.Equal(SyntaxKind.IdentifierToken, syntaxTokens[93].SyntaxKind);
        Assert.Equal(SyntaxKind.EqualsToken, syntaxTokens[94].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseAngleBracketToken, syntaxTokens[95].SyntaxKind);
        Assert.Equal(SyntaxKind.DollarSignToken, syntaxTokens[96].SyntaxKind);
        Assert.Equal(SyntaxKind.StringLiteralToken, syntaxTokens[97].SyntaxKind);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, syntaxTokens[98].SyntaxKind);
        Assert.Equal(SyntaxKind.CloseBraceToken, syntaxTokens[99].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, syntaxTokens[100].SyntaxKind);

        // Template for Asserting over the Symbols
        var symbols = cSharpResource.Symbols;
        Assert.Equal(13, symbols.Length);
        Assert.Equal(SyntaxKind.NamespaceSymbol, symbols[0].SyntaxKind);
        Assert.Equal(SyntaxKind.TypeSymbol, symbols[1].SyntaxKind);
        Assert.Equal(SyntaxKind.TypeSymbol, symbols[2].SyntaxKind);
        Assert.Equal(SyntaxKind.TypeSymbol, symbols[3].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[4].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[5].SyntaxKind);
        Assert.Equal(SyntaxKind.TypeSymbol, symbols[6].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[7].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[8].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[9].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[10].SyntaxKind);
        Assert.Equal(SyntaxKind.VariableSymbol, symbols[11].SyntaxKind);
        Assert.Equal(SyntaxKind.StringInterpolationSymbol, symbols[12].SyntaxKind);
    }

    public class ClassInheritsInterface
    {
        public const string COMPLETE_PROGRAM = @"namespace BlazorApp3.PersonCase;

public interface IPersonModel
{
    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string DisplayName { get; }
}

public class PersonModel : IPersonModel
{
    public PersonModel(
        string firstName,
        string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public string FirstName { get; }
    public string LastName { get; }
    public string DisplayName => $""{FirstName} {LastName}"";
}";

        public const string NAMESPACE_DEFINITION = @"namespace BlazorApp3.PersonCase;";

        public const string INTERFACE_DEFINITION = @"public interface IPersonModel
{
    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string DisplayName { get; }
}";

        public const string CLASS_DEFINITION = @"public class PersonModel : IPersonModel
{
    public PersonModel(
        string firstName,
        string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public string FirstName { get; }
    public string LastName { get; }
    public string DisplayName => $""{FirstName} {LastName}"";
}";
    }
}