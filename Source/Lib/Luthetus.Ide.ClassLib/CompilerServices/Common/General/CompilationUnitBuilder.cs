using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public class CompilationUnitBuilder
{
    public CompilationUnitBuilder()
        : this(null, string.Empty)
    {
    }
    
    public CompilationUnitBuilder(
        CompilationUnitBuilder parent)
        : this(parent, parent.ResourceUri)
    {
        Parent = parent;
        ResourceUri = parent.ResourceUri;
    }
    
    public CompilationUnitBuilder(
        CompilationUnitBuilder? parent,
        string resourceUri)
    {
        Parent = parent;
        ResourceUri = resourceUri;
    }

    public bool IsExpression { get; set; }
    public List<ISyntax> Children { get; } = new();
    public CompilationUnitBuilder? Parent { get; }
    public string ResourceUri { get; }

    public CompilationUnit Build()
    {
        return new CompilationUnit(
            IsExpression,
            Children.ToImmutableArray(),
            ResourceUri);
    }

    public CompilationUnit Build(
        ImmutableArray<TextEditorDiagnostic> diagnostics)
    {
        return new CompilationUnit(
            IsExpression,
            Children.ToImmutableArray(),
            diagnostics,
            ResourceUri);
    }
}