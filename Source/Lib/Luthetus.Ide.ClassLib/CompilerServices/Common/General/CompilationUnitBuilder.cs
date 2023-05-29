using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public class CompilationUnitBuilder
{
    public CompilationUnitBuilder(CompilationUnitBuilder? parent)
    {
        Parent = parent;
    }

    public bool IsExpression { get; set; }
    public List<ISyntax> Children { get; } = new();
    public CompilationUnitBuilder? Parent { get; }

    public CompilationUnit Build()
    {
        return new CompilationUnit(
            IsExpression,
            Children.ToImmutableArray());
    }

    public CompilationUnit Build(
        ImmutableArray<TextEditorDiagnostic> diagnostics)
    {
        return new CompilationUnit(
            IsExpression,
            Children.ToImmutableArray(),
            diagnostics);
    }
}