using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

/// <summary>
/// The <see cref="CodeBlockNode"/> is used for storing a sequence of statements (or a single
/// expression-statement).<br/><br/>
/// Perhaps one might use <see cref="CodeBlockNode"/> for the body of a class definition, for example.
/// </summary>
public sealed record CodeBlockNode : ISyntaxNode
{
    public CodeBlockNode(bool isExpression, ImmutableArray<ISyntax> childBag)
    {
        IsExpression = isExpression;
        ChildBag = childBag;

        DiagnosticsBag = ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    public CodeBlockNode(
        bool isExpression,
        ImmutableArray<ISyntax> children,
        ImmutableArray<TextEditorDiagnostic> diagnostics)
    {
        IsExpression = isExpression;
        ChildBag = children;
        DiagnosticsBag = diagnostics;
    }

    public bool IsExpression { get; init; }
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag { get; init; }

    public ImmutableArray<ISyntax> ChildBag { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CodeBlockNode;
}