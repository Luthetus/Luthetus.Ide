using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Expression;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;

public interface IBoundExpressionNode : IExpressionNode
{
    public BoundClassReferenceNode? BoundClassReferenceNode { get; init; }
    public Type ResultType { get; }
}
