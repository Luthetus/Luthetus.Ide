using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundClassDeclarationNode : ISyntaxNode
{
    private ISyntaxToken _identifierToken;
    private BoundInheritanceStatementNode? _boundInheritanceStatementNode;
    private CompilationUnit? _classBodyCompilationUnit;
    private ImmutableArray<ISyntax> _children;

    public BoundClassDeclarationNode(
        ISyntaxToken identifierToken,
        BoundInheritanceStatementNode? boundInheritanceStatementNode,
        CompilationUnit? classBodyCompilationUnit)
    {
        _identifierToken = identifierToken;
        _boundInheritanceStatementNode = boundInheritanceStatementNode;
        _classBodyCompilationUnit = classBodyCompilationUnit;

        SetChildren();
    }

    public ISyntaxToken IdentifierToken 
    {
        get => _identifierToken;
        init
        {
            _identifierToken = value;
            SetChildren();
        }
    }

    public BoundInheritanceStatementNode? BoundInheritanceStatementNode
    {
        get => _boundInheritanceStatementNode;
        init
        {
            _boundInheritanceStatementNode = value;
            SetChildren();
        }
    }

    public CompilationUnit? ClassBodyCompilationUnit
    {
        get => _classBodyCompilationUnit;
        init
        {
            _classBodyCompilationUnit = value;
            SetChildren();
        }
    }

    public ImmutableArray<ISyntax> Children 
    { 
        get => _children;
        init
        {
            _children = value;
        }
    }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundClassDeclarationNode;

    private void SetChildren()
    {
        var childrenList = new List<ISyntax>(3)
        {
            IdentifierToken
        };

        if (BoundInheritanceStatementNode is not null)
            childrenList.Add(BoundInheritanceStatementNode);

        if (ClassBodyCompilationUnit is not null)
            childrenList.Add(ClassBodyCompilationUnit);

        _children = childrenList.ToImmutableArray();
    }
}
