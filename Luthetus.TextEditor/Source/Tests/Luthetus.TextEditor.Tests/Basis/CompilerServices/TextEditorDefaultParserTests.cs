using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

public class TextEditorDefaultParserTests
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag { get; } = ImmutableArray<TextEditorDiagnostic>.Empty;
}