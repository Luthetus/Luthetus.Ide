using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class FieldDefinitionNode : IVariableDeclarationNode
{
	public FieldDefinitionNode(
        TypeClauseNode typeClauseNode,
        IdentifierToken identifierToken,
        VariableKind variableKind,
        bool isInitialized)
    {
        TypeClauseNode = typeClauseNode;
        IdentifierToken = identifierToken;
        VariableKind = variableKind;
        IsInitialized = isInitialized;

        SetChildList();
    }

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

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FieldDefinitionNode;
    
    public void SetChildList()
    {
    	ChildList = new ISyntax[]
        {
            TypeClauseNode,
            IdentifierToken,
        }.ToImmutableArray();
    	throw new NotImplementedException();
    }
}
