using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public class CompilationUnitBuilder
{
    public CompilationUnitBuilder()
        : this(null, new ResourceUri(string.Empty))
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
        ResourceUri resourceUri)
    {
        Parent = parent;
        ResourceUri = resourceUri;
    }

    public bool IsExpression { get; set; }
    public List<ISyntax> Children { get; } = new();
    public CompilationUnitBuilder? Parent { get; }
    public ResourceUri ResourceUri { get; }

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