using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

/// <summary>
/// The <see cref="CodeBlockNode"/> is used for storing a sequence of
/// statements (or a single expression-statement).
/// <br/><br/>
/// Perhaps one might use <see cref="CodeBlockNode"/> for the body of a class definition, for example.
/// </summary>
public sealed record CodeBlockNode : ISyntaxNode
{
    public CodeBlockNode(
        bool isExpression,
        ImmutableArray<ISyntax> children)
    {
        IsExpression = isExpression;
        Children = children;

        Diagnostics = ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    public CodeBlockNode(
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
