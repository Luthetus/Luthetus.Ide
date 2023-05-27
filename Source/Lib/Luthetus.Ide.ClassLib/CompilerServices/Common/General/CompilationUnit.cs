using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public class CompilationUnit : ISyntaxNode
{
    public CompilationUnit(
        bool isExpression,
        ImmutableArray<ISyntax> children,
        string resourceUri)
    {
        IsExpression = isExpression;
        Children = children;
        ResourceUri = resourceUri;

        Diagnostics = ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    public CompilationUnit(
        bool isExpression,
        ImmutableArray<ISyntax> children,
        ImmutableArray<TextEditorDiagnostic> diagnostics,
        string resourceUri)
    {
        IsExpression = isExpression;
        Children = children;
        Diagnostics = diagnostics;
        ResourceUri = resourceUri;
    }

    public bool IsExpression { get; }
    public ImmutableArray<TextEditorDiagnostic> Diagnostics { get; }
    /// <summary>This might be used to refer to the absolute file path of the file on one's computer which was parsed.</summary>
    public string ResourceUri { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CompilationUnit;
}
