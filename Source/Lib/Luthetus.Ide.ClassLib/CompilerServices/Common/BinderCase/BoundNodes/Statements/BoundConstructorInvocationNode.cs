using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

/// <summary>TODO: Correctly implement this node. For now, just skip over it when parsing.</summary>
public sealed record BoundConstructorInvocationNode : ISyntaxNode
{
    public BoundConstructorInvocationNode(
        KeywordToken keywordToken,
        BoundClassReferenceNode? boundClassReferenceNode,
        BoundFunctionParametersNode? boundFunctionParametersNode,
        BoundObjectInitializationNode? boundObjectInitializationNode)
    {
        KeywordToken = keywordToken;
        BoundClassReferenceNode = boundClassReferenceNode;
        BoundFunctionParametersNode = boundFunctionParametersNode;
        BoundObjectInitializationNode = boundObjectInitializationNode;

        var children = new List<ISyntax>
        {
            KeywordToken
        };

        if (BoundClassReferenceNode is not null)
            children.Add(BoundClassReferenceNode);
        
        if (BoundFunctionParametersNode is not null)
            children.Add(BoundFunctionParametersNode);
        
        if (BoundObjectInitializationNode is not null)
            children.Add(BoundObjectInitializationNode);

        Children = children.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public BoundClassReferenceNode? BoundClassReferenceNode { get; }
    public BoundFunctionParametersNode? BoundFunctionParametersNode { get; }
    public BoundObjectInitializationNode? BoundObjectInitializationNode { get; }

    public bool IsFabricated { get; init; }
    public ImmutableArray<ISyntax> Children { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundConstructorInvocationNode;
}
