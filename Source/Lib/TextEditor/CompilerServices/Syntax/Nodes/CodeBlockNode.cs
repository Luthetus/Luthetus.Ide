using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// The <see cref="CodeBlockNode"/> is used for storing a sequence of statements (or a single
/// expression-statement).<br/><br/>
/// Perhaps one might use <see cref="CodeBlockNode"/> for the body of a class definition, for example.
/// </summary>
public sealed record CodeBlockNode : ISyntaxNode
{
    public CodeBlockNode(ImmutableArray<ISyntax> childList)
    {
        ChildList = childList;

        DiagnosticsList = ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    public CodeBlockNode(
        ImmutableArray<ISyntax> childList,
        ImmutableArray<TextEditorDiagnostic> diagnostics)
    {
        ChildList = childList;
        DiagnosticsList = diagnostics;
    }

    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; init; }

    public ImmutableArray<ISyntax> ChildList { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CodeBlockNode;
}