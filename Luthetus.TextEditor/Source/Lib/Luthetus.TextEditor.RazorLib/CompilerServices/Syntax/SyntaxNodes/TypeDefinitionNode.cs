using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="TypeDefinitionNode"/> is used anywhere a type is defined.
/// </summary>
public sealed record TypeDefinitionNode : ISyntaxNode
{
    public TypeDefinitionNode(
        AccessModifierKind accessModifierKind,
        StorageModifierKind storageModifierKind,
        IdentifierToken typeIdentifier,
        Type? valueType,
        GenericArgumentsListingNode? genericArgumentsListingNode,
        TypeClauseNode? inheritedTypeClauseNode,
        CodeBlockNode? typeBodyCodeBlockNode)
    {
        AccessModifierKind = accessModifierKind;
        StorageModifierKind = storageModifierKind;
        TypeIdentifier = typeIdentifier;
        ValueType = valueType;
        GenericArgumentsListingNode = genericArgumentsListingNode;
        InheritedTypeClauseNode = inheritedTypeClauseNode;
        TypeBodyCodeBlockNode = typeBodyCodeBlockNode;

        var children = new List<ISyntax>
        {
            TypeIdentifier,
        };

        if (GenericArgumentsListingNode is not null)
            children.Add(GenericArgumentsListingNode);

        if (InheritedTypeClauseNode is not null)
            children.Add(InheritedTypeClauseNode);

        if (TypeBodyCodeBlockNode is not null)
            children.Add(TypeBodyCodeBlockNode);

        ChildList = children.ToImmutableArray();
    }

    public AccessModifierKind AccessModifierKind { get; }
    public StorageModifierKind StorageModifierKind { get; }
    /// <summary>
    /// Given: 'public class Person { /* class definition here */ }'<br/>
    /// Then: 'Person' is the <see cref="TypeIdentifier"/><br/>
    /// And: <see cref="GenericArgumentsListingNode"/> would be null
    /// </summary>
    public IdentifierToken TypeIdentifier { get; }
    public Type? ValueType { get; }
    /// <summary>
    /// Given: 'public struct Array&lt;T&gt; { /* struct definition here */ }'<br/>
    /// Then: 'Array&lt;T&gt;' is the <see cref="TypeIdentifier"/><br/>
    /// And: '&lt;T&gt;' is the <see cref="GenericArgumentsListingNode"/>
    /// </summary>
    public GenericArgumentsListingNode? GenericArgumentsListingNode { get; }
    /// <summary>
    /// Given:<br/>
    /// public class Person : IPerson { ... }<br/><br/>
    /// Then: 'IPerson' is the <see cref="InheritedTypeClauseNode"/>
    /// </summary>
    public TypeClauseNode? InheritedTypeClauseNode { get; }
    public CodeBlockNode? TypeBodyCodeBlockNode { get; }
    public bool IsInterface => StorageModifierKind == StorageModifierKind.Interface;

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TypeDefinitionNode;

    public ImmutableArray<FunctionDefinitionNode> GetFunctionDefinitionNodes()
    {
        if (TypeBodyCodeBlockNode is null)
            return ImmutableArray<FunctionDefinitionNode>.Empty;

        return TypeBodyCodeBlockNode.ChildList
            .Where(child => child.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
            .Select(fd => (FunctionDefinitionNode)fd)
            .ToImmutableArray();
    }

    public TypeClauseNode ToTypeClause()
    {
        return new TypeClauseNode(
            TypeIdentifier,
            ValueType,
            null);
    }
}