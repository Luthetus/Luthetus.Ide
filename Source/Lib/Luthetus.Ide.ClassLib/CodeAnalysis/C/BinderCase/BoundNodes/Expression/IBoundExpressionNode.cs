using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxNodes.Expression;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Expression;

public interface IBoundExpressionNode : IExpressionNode
{
    public Type ResultType { get; }
}
