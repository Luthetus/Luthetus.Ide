namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Variables;

public interface IVariableDeclarationNode : ISyntaxNode
{
    public TypeClauseNode TypeClauseNode { get; }
    public IdentifierToken IdentifierToken { get; }
    public VariableKind VariableKind { get; }
    public bool IsInitialized { get; set; }
    /// <summary>
    /// TODO: Remove the 'set;' on this property
    /// </summary>
    public bool HasGetter { get; set; }
    /// <summary>
    /// TODO: Remove the 'set;' on this property
    /// </summary>
    public bool GetterIsAutoImplemented { get; set; }
    /// <summary>
    /// TODO: Remove the 'set;' on this property
    /// </summary>
    public bool HasSetter { get; set; }
    /// <summary>
    /// TODO: Remove the 'set;' on this property
    /// </summary>
    public bool SetterIsAutoImplemented { get; set; }
}