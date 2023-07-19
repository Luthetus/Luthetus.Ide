using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public class CodeBlockBuilder
{
    public CodeBlockBuilder(CodeBlockBuilder? parent)
    {
        Parent = parent;
    }

    public bool IsExpression { get; set; }
    public List<ISyntax> Children { get; } = new();
    public CodeBlockBuilder? Parent { get; }

    public CodeBlockNode Build()
    {
        return new CodeBlockNode(
            IsExpression,
            Children.ToImmutableArray());
    }

    public CodeBlockNode Build(
        ImmutableArray<TextEditorDiagnostic> diagnostics)
    {
        return new CodeBlockNode(
            IsExpression,
            Children.ToImmutableArray(),
            diagnostics);
    }
}