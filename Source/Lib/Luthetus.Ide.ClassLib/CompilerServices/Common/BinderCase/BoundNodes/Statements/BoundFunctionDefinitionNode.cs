using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundFunctionDefinitionNode : ISyntaxNode
{
    private CodeBlockNode? _functionBodyCodeBlockNode;
    private ImmutableArray<ISyntax> _children;
    private BoundGenericArgumentsNode? _boundGenericArgumentsNode;

    public BoundFunctionDefinitionNode(
            BoundClassReferenceNode returnBoundClassReferenceNode,
            ISyntaxToken identifierToken,
            BoundFunctionArgumentsNode boundFunctionArgumentsNode,
            BoundGenericArgumentsNode? boundGenericArgumentsNode,
            CodeBlockNode? functionBodyCodeBlockNode)
    {
        ReturnBoundClassReferenceNode = returnBoundClassReferenceNode;
        IdentifierToken = identifierToken;
        BoundFunctionArgumentsNode = boundFunctionArgumentsNode;
        BoundGenericArgumentsNode = boundGenericArgumentsNode;
        FunctionBodyCodeBlockNode = functionBodyCodeBlockNode;

        CalculateChildren();
    }

    public BoundClassReferenceNode ReturnBoundClassReferenceNode { get; init; }
    public ISyntaxToken IdentifierToken { get; init; }
    public BoundFunctionArgumentsNode BoundFunctionArgumentsNode { get; init; }

    public BoundGenericArgumentsNode? BoundGenericArgumentsNode 
    { 
        get => _boundGenericArgumentsNode;
        init 
        {
            _boundGenericArgumentsNode = value;
            CalculateChildren();
        }
    }

    /// <summary>
    /// Only the null properties and "Children" itself need to have backing fields.
    /// This is due to the record needing to re-assign Children.
    /// </summary>
    public CodeBlockNode? FunctionBodyCodeBlockNode
    {
        get => _functionBodyCodeBlockNode;
        init
        {
            _functionBodyCodeBlockNode = value;
            CalculateChildren();
        }
    }

    public ImmutableArray<ISyntax> Children
    {
        get => _children;
        init
        {
            _children = value;
            CalculateChildren();
        }
    }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionDefinitionNode;

    private void CalculateChildren()
    {
        var childrenList = new List<ISyntax>
        {
            ReturnBoundClassReferenceNode,
            IdentifierToken,
            BoundFunctionArgumentsNode
        };

        if (BoundGenericArgumentsNode is not null)
            childrenList.Add(BoundGenericArgumentsNode);
        
        if (FunctionBodyCodeBlockNode is not null)
            childrenList.Add(FunctionBodyCodeBlockNode);

        _children = childrenList.ToImmutableArray();
    }
}
