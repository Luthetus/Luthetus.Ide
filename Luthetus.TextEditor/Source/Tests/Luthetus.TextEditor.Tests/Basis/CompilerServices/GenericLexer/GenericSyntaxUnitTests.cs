using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxObjects;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer;

public class GenericSyntaxUnitTests
{
    public GenericSyntaxUnit(
        GenericDocumentSyntax genericDocumentSyntax,
        LuthetusDiagnosticBag diagnosticBag)
    {
        GenericDocumentSyntax = genericDocumentSyntax;
        DiagnosticBag = diagnosticBag;
    }

    public GenericDocumentSyntax GenericDocumentSyntax { get; }
    public LuthetusDiagnosticBag DiagnosticBag { get; }
}