using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class LuthBinder : ILuthBinder
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; } = ImmutableArray<TextEditorDiagnostic>.Empty;
    public ImmutableArray<ITextEditorSymbol> SymbolsList { get; } = ImmutableArray<ITextEditorSymbol>.Empty;

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan)
    {
        return null;
    }

    public ISyntaxNode? GetSyntaxNode(int positionIndex, CompilationUnit compilationUnit)
    {
        return null;
    }

    public IBoundScope? GetBoundScope(TextEditorTextSpan textSpan)
    {
        return null;
    }

    public ILuthBinderSession ConstructBinderSession(ResourceUri resourceUri)
    {
        return new LuthBinderSession(resourceUri, null, null, this);
    }

    public void ClearStateByResourceUri(ResourceUri resourceUri)
    {
        return;
    }
}