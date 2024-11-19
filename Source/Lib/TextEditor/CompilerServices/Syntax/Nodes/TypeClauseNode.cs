using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="TypeClauseNode"/> is used anywhere a type is referenced.
/// </summary>
public sealed class TypeClauseNode : IExpressionNode
{
    public TypeClauseNode(
        ISyntaxToken typeIdentifier,
        Type? valueType,
        GenericParametersListingNode? genericParametersListingNode)
    {
        TypeIdentifierToken = typeIdentifier;
        ValueType = valueType;
        GenericParametersListingNode = genericParametersListingNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

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
    public Type? ValueType { get; private set; }
    /// <summary>
    /// Given: 'int[] x = 2;'<br/>
    /// Then: 'Array&lt;T&gt;' is the <see cref="TypeIdentifierToken"/><br/>
    /// And: '&lt;int&gt;' is the <see cref="GenericParametersListingNode"/>
    /// </summary>
    public GenericParametersListingNode? GenericParametersListingNode { get; private set; }
    TypeClauseNode IExpressionNode.ResultTypeClauseNode => TypeFacts.Pseudo.ToTypeClause();
    
    public bool HasQuestionMark { get; set; }

    /// <summary>
    /// TODO: Change this attribute node property.
    /// </summary>
    public AttributeNode AttributeNode { get; set; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TypeClauseNode;
    
    public TypeClauseNode SetGenericParametersListingNode(GenericParametersListingNode genericParametersListingNode)
    {
    	GenericParametersListingNode = genericParametersListingNode;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public TypeClauseNode SetValueType(Type? valueType)
    {
    	ValueType = valueType;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public int GetStartInclusiveIndex()
    {
    	return TypeIdentifierToken.TextSpan.StartingIndexInclusive;
    }
    
    public int GetEndExclusiveIndex()
    {
    	return TypeIdentifierToken.TextSpan.EndingIndexExclusive;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 1; // TypeIdentifierToken
        if (GenericParametersListingNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;
		
		childList[i++] = TypeIdentifierToken;
		if (GenericParametersListingNode is not null)
            childList[i++] = GenericParametersListingNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}