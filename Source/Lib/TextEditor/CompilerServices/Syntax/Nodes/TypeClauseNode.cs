using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="TypeClauseNode"/> is used anywhere a type is referenced.
/// </summary>
public sealed class TypeClauseNode : ISyntaxNode
{
    public TypeClauseNode(
        ISyntaxToken typeIdentifier,
        Type? valueType,
        GenericParametersListingNode? genericParametersListingNode)
    {
        TypeIdentifierToken = typeIdentifier;
        ValueType = valueType;
        GenericParametersListingNode = genericParametersListingNode;
        
        SetChildList();
    }

    /// <summary>
    /// Given: 'int x = 2;'<br/>
    /// Then: 'int' is the <see cref="TypeIdentifierToken"/>
    /// And: <see cref="GenericParametersListingNode"/> would be null
    /// </summary>
    public ISyntaxToken TypeIdentifierToken { get; }
	/// <summary>
    /// Given: 'int x = 2;'<br/>
    /// Then: 'typeof(int)' is the <see cref="ValueType"/>
    /// And: <see cref="GenericParametersListingNode"/> would be null
	///<br/>
	/// In short, <see cref="ValueType"/> is non-null when the
	/// <see cref="TypeIdentifierToken"/> maps to a C# primitive type.
    /// </summary>
    public Type? ValueType { get; }
    /// <summary>
    /// Given: 'int[] x = 2;'<br/>
    /// Then: 'Array&lt;T&gt;' is the <see cref="TypeIdentifierToken"/><br/>
    /// And: '&lt;int&gt;' is the <see cref="GenericParametersListingNode"/>
    /// </summary>
    public GenericParametersListingNode? GenericParametersListingNode { get; private set; }
    
    public bool HasQuestionMark { get; set; }

    /// <summary>
    /// TODO: Change this attribute node property.
    /// </summary>
    public AttributeNode AttributeNode { get; set; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TypeClauseNode;
    
    public TypeClauseNode SetGenericParametersListingNode(GenericParametersListingNode genericParametersListingNode)
    {
    	GenericParametersListingNode = genericParametersListingNode;
    	
    	SetChildList();
    	return this;
    }
    
    public void SetChildList()
    {
    	var childCount = 1; // TypeIdentifierToken
        if (GenericParametersListingNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;
		
		childList[i++] = TypeIdentifierToken;
		if (GenericParametersListingNode is not null)
            childList[i++] = GenericParametersListingNode;
            
        ChildList = childList;
    }
}