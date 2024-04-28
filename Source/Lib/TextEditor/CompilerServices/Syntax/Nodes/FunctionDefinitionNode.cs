using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// TODO: Track the open and close braces for the function body.
/// </summary>
public sealed record FunctionDefinitionNode : ISyntaxNode
{
    public FunctionDefinitionNode(
        AccessModifierKind accessModifierKind,
        TypeClauseNode returnTypeClauseNode,
        IdentifierToken functionIdentifierToken,
        GenericArgumentsListingNode? genericArgumentsListingNode,
        FunctionArgumentsListingNode functionArgumentsListingNode,
        CodeBlockNode? functionBodyCodeBlockNode,
        ConstraintNode? constraintNode)
    {
        AccessModifierKind = accessModifierKind;
        ReturnTypeClauseNode = returnTypeClauseNode;
        FunctionIdentifierToken = functionIdentifierToken;
        GenericArgumentsListingNode = genericArgumentsListingNode;
        FunctionArgumentsListingNode = functionArgumentsListingNode;
        FunctionBodyCodeBlockNode = functionBodyCodeBlockNode;
        ConstraintNode = constraintNode;

        var children = new List<ISyntax>
        {
            ReturnTypeClauseNode,
            FunctionIdentifierToken,
        };

        if (GenericArgumentsListingNode is not null)
            children.Add(GenericArgumentsListingNode);

        children.Add(FunctionArgumentsListingNode);

        if (FunctionBodyCodeBlockNode is not null)
            children.Add(FunctionBodyCodeBlockNode);
        
        if (ConstraintNode is not null)
            children.Add(ConstraintNode);

        ChildList = children.ToImmutableArray();
    }

    public AccessModifierKind AccessModifierKind { get; }
    public TypeClauseNode ReturnTypeClauseNode { get; }
    public IdentifierToken FunctionIdentifierToken { get; }
    public GenericArgumentsListingNode? GenericArgumentsListingNode { get; }
    public FunctionArgumentsListingNode FunctionArgumentsListingNode { get; }
    public CodeBlockNode? FunctionBodyCodeBlockNode { get; }
    public ConstraintNode? ConstraintNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionDefinitionNode;
}
