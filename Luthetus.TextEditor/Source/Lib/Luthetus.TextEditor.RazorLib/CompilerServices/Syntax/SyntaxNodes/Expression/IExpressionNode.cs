namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;

public interface IExpressionNode : ISyntaxNode
{
    public TypeClauseNode ResultTypeClauseNode { get; }
}