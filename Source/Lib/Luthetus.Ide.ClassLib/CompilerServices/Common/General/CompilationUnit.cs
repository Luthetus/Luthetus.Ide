using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public class CompilationUnit : ISyntaxNode
{
    public CompilationUnit(
        bool isExpression,
        ImmutableArray<ISyntax> children)
    {
        IsExpression = isExpression;
        Children = children;

        Diagnostics = ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    public CompilationUnit(
        bool isExpression,
        ImmutableArray<ISyntax> children,
        ImmutableArray<TextEditorDiagnostic> diagnostics)
    {
        IsExpression = isExpression;
        Children = children;
        Diagnostics = diagnostics;
    }

    public bool IsExpression { get; }
    public ImmutableArray<TextEditorDiagnostic> Diagnostics { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CompilationUnit;
}
