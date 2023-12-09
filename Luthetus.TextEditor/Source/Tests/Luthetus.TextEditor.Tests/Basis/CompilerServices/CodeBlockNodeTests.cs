using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// The <see cref="CodeBlockNode"/> is used for storing a sequence of statements (or a single
/// expression-statement).<br/><br/>
/// Perhaps one might use <see cref="CodeBlockNode"/> for the body of a class definition, for example.
/// </summary>
public sealed record CodeBlockNodeTests : ISyntaxNode
{
    public CodeBlockNode(ImmutableArray<ISyntax> childBag)
    {
        ChildBag = childBag;

        DiagnosticsBag = ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    public CodeBlockNode(
        ImmutableArray<ISyntax> children,
        ImmutableArray<TextEditorDiagnostic> diagnostics)
    {
        ChildBag = children;
        DiagnosticsBag = diagnostics;
    }

    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag { get; init; }

    public ImmutableArray<ISyntax> ChildBag { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CodeBlockNode;
}