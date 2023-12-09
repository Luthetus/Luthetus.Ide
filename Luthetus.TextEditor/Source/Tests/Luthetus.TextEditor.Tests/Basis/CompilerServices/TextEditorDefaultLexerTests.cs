using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

public class TextEditorDefaultLexerTests
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag { get; } = ImmutableArray<TextEditorDiagnostic>.Empty;
}