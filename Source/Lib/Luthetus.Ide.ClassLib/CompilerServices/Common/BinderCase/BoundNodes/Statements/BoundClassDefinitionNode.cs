using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

/// <summary><see cref="BoundClassDefinitionNode"/> is used for specifically only definition of types such as the syntax: 'public class PersonModel" Whereas <see cref="BoundClassReferenceNode"/> is used for invoking a constructor or doing type comparison, etc...</summary>
public sealed record BoundClassDefinitionNode : ISyntaxNode
{
    private ISyntaxToken _typeClauseToken;
    private Type _type;
    private BoundGenericArgumentsNode? _boundGenericArgumentsNode;
    private BoundInheritanceStatementNode? _boundInheritanceStatementNode;
    private CodeBlockNode? _classBodyCodeBlockNode;
    private ImmutableArray<ISyntax> _children;

    public BoundClassDefinitionNode(
        ISyntaxToken typeClauseToken,
        Type type,
        BoundGenericArgumentsNode? boundGenericArgumentsNode,
        BoundInheritanceStatementNode? boundInheritanceStatementNode,
        CodeBlockNode? classBodyCodeBlockNode)
    {
        _typeClauseToken = typeClauseToken;
        _type = type;
        _boundGenericArgumentsNode = boundGenericArgumentsNode;
        _boundInheritanceStatementNode = boundInheritanceStatementNode;
        _classBodyCodeBlockNode = classBodyCodeBlockNode;

        SetChildren();
    }

    public ISyntaxToken Identifier => _typeClauseToken;
    
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

    public CodeBlockNode? ClassBodyCodeBlockNode
    {
        get => _classBodyCodeBlockNode;
        init
        {
            _classBodyCodeBlockNode = value;
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

        if (ClassBodyCodeBlockNode is not null)
            childrenList.Add(ClassBodyCodeBlockNode);

        _children = childrenList.ToImmutableArray();
    }
}
