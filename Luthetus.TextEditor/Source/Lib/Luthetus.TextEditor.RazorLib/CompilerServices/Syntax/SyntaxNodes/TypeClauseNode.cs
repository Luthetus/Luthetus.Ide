using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

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
        TypeIdentifier = typeIdentifier;
        ValueType = valueType;
        GenericParametersListingNode = genericParametersListingNode;

        var children = new List<ISyntax>
        {
            TypeIdentifier
        };

        if (GenericParametersListingNode is not null)
            children.Add(GenericParametersListingNode);

        ChildBag = children.ToImmutableArray();
    }

    /// <summary>
    /// Given: 'string x = 2;'<br/>
    /// Then: 'string' is the <see cref="TypeIdentifier"/>
    /// And: <see cref="GenericParametersListingNode"/> would be null
    /// </summary>
    public ISyntaxToken TypeIdentifier { get; }
    public Type? ValueType { get; }
    /// <summary>
    /// Given: 'int[] x = 2;'<br/>
    /// Then: 'Array&lt;T&gt;' is the <see cref="TypeIdentifier"/><br/>
    /// And: '&lt;int&gt;' is the <see cref="GenericParametersListingNode"/>
    /// </summary>
    public GenericParametersListingNode? GenericParametersListingNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TypeClauseNode;
}