using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ILuthBinder
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; }
    public ImmutableArray<ITextEditorSymbol> SymbolsList { get; }

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan);
    public ISyntaxNode? GetSyntaxNode(int positionIndex, CompilationUnit compilationUnit);
    public IBoundScope? GetBoundScope(TextEditorTextSpan textSpan);
    public ILuthBinderSession ConstructBinderSession(ResourceUri resourceUri);
    public void ClearStateByResourceUri(ResourceUri resourceUri);
}
