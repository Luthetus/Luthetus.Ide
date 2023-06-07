using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundFunctionDeclarationNode : ISyntaxNode
{
    private CompilationUnit? _functionBodyCompilationUnit;
    private ImmutableArray<ISyntax> _children;
    private BoundGenericArgumentsNode? _boundGenericArgumentsNode;

    public BoundFunctionDeclarationNode(
            BoundClassDeclarationNode boundClassDeclarationNode,
            ISyntaxToken identifierToken,
            BoundFunctionArgumentsNode boundFunctionArgumentsNode,
            BoundGenericArgumentsNode? boundGenericArgumentsNode,
            CompilationUnit? functionBodyCompilationUnit)
    {
        BoundClassDeclarationNode = boundClassDeclarationNode;
        IdentifierToken = identifierToken;
        BoundFunctionArgumentsNode = boundFunctionArgumentsNode;
        BoundGenericArgumentsNode = boundGenericArgumentsNode;
        FunctionBodyCompilationUnit = functionBodyCompilationUnit;

        CalculateChildren();
    }

    public BoundClassDeclarationNode BoundClassDeclarationNode { get; init; }
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

    /// <summary>Only the null properties and "Children" itself need to have backing fields. This is due to the record needing to re-assign Children.</summary>
    public CompilationUnit? FunctionBodyCompilationUnit
    {
        get => _functionBodyCompilationUnit;
        init
        {
            _functionBodyCompilationUnit = value;
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
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionDeclarationNode;

    private void CalculateChildren()
    {
        var childrenList = new List<ISyntax>(3)
        {
            BoundClassDeclarationNode,
            IdentifierToken,
            BoundFunctionArgumentsNode
        };

        if (BoundGenericArgumentsNode is not null)
            childrenList.Add(BoundGenericArgumentsNode);
        
        if (FunctionBodyCompilationUnit is not null)
            childrenList.Add(FunctionBodyCompilationUnit);

        _children = childrenList.ToImmutableArray();
    }
}
