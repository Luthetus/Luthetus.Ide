using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

/// <summary>TODO: Correctly implement this node. For now, just skip over it when parsing.</summary>
public sealed record BoundConstructorInvocationNode : ISyntaxNode
{
    public BoundConstructorInvocationNode(
        KeywordToken keywordToken,
        BoundTypeNode? boundTypeNode,
        BoundFunctionArgumentsNode? boundFunctionArgumentsNode,
        BoundObjectInitializationNode? boundObjectInitializationNode)
    {
        KeywordToken = keywordToken;
        BoundTypeNode = boundTypeNode;
        BoundFunctionArgumentsNode = boundFunctionArgumentsNode;
        BoundObjectInitializationNode = boundObjectInitializationNode;

        var children = new List<ISyntax>
        {
            KeywordToken
        };

        if (BoundTypeNode is not null)
            children.Add(BoundTypeNode);
        
        if (BoundFunctionArgumentsNode is not null)
            children.Add(BoundFunctionArgumentsNode);
        
        if (BoundObjectInitializationNode is not null)
            children.Add(BoundObjectInitializationNode);

        Children = children.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public BoundTypeNode? BoundTypeNode { get; }
    public BoundFunctionArgumentsNode? BoundFunctionArgumentsNode { get; }
    public BoundObjectInitializationNode? BoundObjectInitializationNode { get; }

    public bool IsFabricated { get; init; }
    public ImmutableArray<ISyntax> Children { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundConstructorInvocationNode;
}
