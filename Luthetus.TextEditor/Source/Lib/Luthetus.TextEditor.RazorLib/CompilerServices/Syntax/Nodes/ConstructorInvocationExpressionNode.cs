using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record ConstructorInvocationExpressionNode : IExpressionNode
{
    public ConstructorInvocationExpressionNode(
        KeywordToken newKeywordToken,
        TypeClauseNode typeClauseNode,
        FunctionParametersListingNode? functionParametersListingNode,
        ObjectInitializationParametersListingNode? objectInitializationParametersListingNode)
    {
        NewKeywordToken = newKeywordToken;
        ResultTypeClauseNode = typeClauseNode;
        FunctionParametersListingNode = functionParametersListingNode;
        ObjectInitializationParametersListingNode = objectInitializationParametersListingNode;

        var children = new List<ISyntax>
        {
            NewKeywordToken,
            ResultTypeClauseNode,
        };

        if (FunctionParametersListingNode is not null)
            children.Add(FunctionParametersListingNode);
        
        if (ObjectInitializationParametersListingNode is not null)
            children.Add(ObjectInitializationParametersListingNode);

        ChildList = children.ToImmutableArray();
    }

    public KeywordToken NewKeywordToken { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }
    public FunctionParametersListingNode? FunctionParametersListingNode { get; }
    public ObjectInitializationParametersListingNode? ObjectInitializationParametersListingNode { get; }
    
    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ConstructorInvocationExpressionNode;
}
