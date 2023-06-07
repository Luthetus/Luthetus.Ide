using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

/// <summary><see cref="BoundClassReferenceNode"/> is used for invoking a constructor or doing type comparison, etc... Whereas <see cref="BoundClassDefinitionNode"/> is used for specifically only definition of types such as the syntax: 'public class PersonModel"</summary>
public sealed record BoundClassReferenceNode : ISyntaxNode
{
    public BoundClassReferenceNode(
        ISyntaxToken typeClauseToken,
        Type? type,
        BoundGenericArgumentsNode? boundGenericArgumentsNode)
    {
        TypeClauseToken = typeClauseToken;
        Type = type;
        BoundGenericArgumentsNode = boundGenericArgumentsNode;

        var childrenList = new List<ISyntax>
        {
            TypeClauseToken,
        };

        if (BoundGenericArgumentsNode is not null)
            childrenList.Add(BoundGenericArgumentsNode);

        Children = childrenList.ToImmutableArray();
    }

    public ISyntaxToken TypeClauseToken { get; init; }
    public Type Type { get; init; }
    public BoundGenericArgumentsNode? BoundGenericArgumentsNode { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundClassReferenceNode;
}