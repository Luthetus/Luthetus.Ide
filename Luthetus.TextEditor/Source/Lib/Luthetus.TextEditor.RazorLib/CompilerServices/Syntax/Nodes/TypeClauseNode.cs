using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="TypeClauseNode"/> is used anywhere a type is referenced.
/// </summary>
public sealed record TypeClauseNode : ISyntaxNode
{
    public TypeClauseNode(
        ISyntaxToken typeIdentifier,
        Type? valueType,
        GenericParametersListingNode? genericParametersListingNode)
    {
        TypeIdentifierToken = typeIdentifier;
        ValueType = valueType;
        GenericParametersListingNode = genericParametersListingNode;

        var children = new List<ISyntax>
        {
            TypeIdentifierToken
        };

        if (GenericParametersListingNode is not null)
            children.Add(GenericParametersListingNode);

        ChildList = children.ToImmutableArray();
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
    public GenericParametersListingNode? GenericParametersListingNode { get; }

    /// <summary>
    /// TODO: Change this attribute node property.
    /// </summary>
    public AttributeNode AttributeNode { get; set; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TypeClauseNode;
}