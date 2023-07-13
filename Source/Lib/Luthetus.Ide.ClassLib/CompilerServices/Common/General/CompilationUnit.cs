using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

/// <summary>
/// TODO: CompilationUnit perhaps should be changed to refer to a C# file itself only. As of (2023-06-30) I've been using <see cref="CompilationUnit"/> for anything which contains an arbitrary amount of <see cref="ISyntaxNode"/>(s)
/// </summary>
public sealed record CompilationUnit : ISyntaxNode
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

    public bool IsExpression { get; init; }
    public ImmutableArray<TextEditorDiagnostic> Diagnostics { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CompilationUnitNode;
}
