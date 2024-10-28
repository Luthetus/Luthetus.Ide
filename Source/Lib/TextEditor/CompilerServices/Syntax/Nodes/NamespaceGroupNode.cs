using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class NamespaceGroupNode : ISyntaxNode
{
    public NamespaceGroupNode(
        string namespaceString,
        ImmutableArray<NamespaceStatementNode> namespaceStatementNodeList)
    {
        NamespaceString = namespaceString;
        NamespaceStatementNodeList = namespaceStatementNodeList;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public string NamespaceString { get; }
    public ImmutableArray<NamespaceStatementNode> NamespaceStatementNodeList { get; } = ImmutableArray<NamespaceStatementNode>.Empty;

    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.NamespaceGroupNode;

    /// <summary>
    /// <see cref="GetTopLevelTypeDefinitionNodes"/> provides a collection
    /// which contains all top level type definitions of the namespace.
    /// <br/><br/>
    /// This is to say that, any type definitions which are nested, would not
    /// be in this collection.
    /// </summary>
    public ImmutableArray<TypeDefinitionNode> GetTopLevelTypeDefinitionNodes()
    {
        return NamespaceStatementNodeList
            .SelectMany(x => x.GetTopLevelTypeDefinitionNodes())
            .ToImmutableArray();
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = NamespaceStatementNodeList.Select(x => (ISyntax)x).ToArray();
        
    	_childListIsDirty = false;
    	return _childList;
    }
}