namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

public interface IExpressionNode : ISyntaxNode
{
    public TypeClauseNode ResultTypeClauseNode { get; }
}