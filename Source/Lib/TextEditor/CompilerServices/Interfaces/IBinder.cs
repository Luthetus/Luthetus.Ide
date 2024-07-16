using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface IBinder
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; }
    public ImmutableArray<ITextEditorSymbol> SymbolsList { get; }

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan);
    public ISyntaxNode? GetSyntaxNode(int positionIndex, CompilationUnit compilationUnit);
    public IBoundScope? GetBoundScope(TextEditorTextSpan textSpan);
    public IBinderSession ConstructBinderSession(ResourceUri resourceUri);
    public void ClearStateByResourceUri(ResourceUri resourceUri);
}
