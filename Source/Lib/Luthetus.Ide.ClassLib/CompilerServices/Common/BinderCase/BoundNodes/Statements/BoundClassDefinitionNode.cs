using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

/// <summary><see cref="BoundClassDefinitionNode"/> is used for specifically only definition of types such as the syntax: 'public class PersonModel" Whereas <see cref="BoundClassReferenceNode"/> is used for invoking a constructor or doing type comparison, etc...</summary>
public sealed record BoundClassDefinitionNode : ISyntaxNode
{
    private ISyntaxToken _typeClauseToken;
    private Type _type;
    private BoundGenericArgumentsNode? _boundGenericArgumentsNode;
    private BoundInheritanceStatementNode? _boundInheritanceStatementNode;
    private CompilationUnit? _classBodyCompilationUnit;
    private ImmutableArray<ISyntax> _children;

    public BoundClassDefinitionNode(
        ISyntaxToken typeClauseToken,
        Type type,
        BoundGenericArgumentsNode? boundGenericArgumentsNode,
        BoundInheritanceStatementNode? boundInheritanceStatementNode,
        CompilationUnit? classBodyCompilationUnit)
    {
        _typeClauseToken = typeClauseToken;
        _type = type;
        _boundGenericArgumentsNode = boundGenericArgumentsNode;
        _boundInheritanceStatementNode = boundInheritanceStatementNode;
        _classBodyCompilationUnit = classBodyCompilationUnit;

        SetChildren();
    }

    public ISyntaxToken Identifier
    {
        get => _typeClauseToken;
    }
    
    public ISyntaxToken TypeClauseToken
    {
        get => _typeClauseToken;
        init
        {
            _typeClauseToken = value;
            SetChildren();
        }
    }
    
    public Type Type
    {
        get => _type;
        init
        {
            _type = value;
            SetChildren();
        }
    }

    public BoundGenericArgumentsNode? BoundGenericArgumentsNode
    {
        get => _boundGenericArgumentsNode;
        init
        {
            _boundGenericArgumentsNode = value;
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
    public SyntaxKind SyntaxKind => SyntaxKind.BoundClassDefinitionNode;

    private void SetChildren()
    {
        var childrenList = new List<ISyntax>
        {
            TypeClauseToken,
        };

        if (BoundGenericArgumentsNode is not null)
            childrenList.Add(BoundGenericArgumentsNode);
        
        if (BoundInheritanceStatementNode is not null)
            childrenList.Add(BoundInheritanceStatementNode);

        if (ClassBodyCompilationUnit is not null)
            childrenList.Add(ClassBodyCompilationUnit);

        _children = childrenList.ToImmutableArray();
    }
}
