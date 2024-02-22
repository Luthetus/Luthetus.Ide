namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;

public interface IExpressionNode : ISyntaxNode
{
    public TypeClauseNode ResultTypeClauseNode { get; }
}