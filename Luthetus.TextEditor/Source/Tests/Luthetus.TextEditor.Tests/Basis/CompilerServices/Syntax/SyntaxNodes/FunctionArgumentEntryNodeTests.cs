using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record FunctionArgumentEntryNodeTests
{
    public FunctionArgumentEntryNode(
        VariableDeclarationNode variableDeclarationStatementNode,
        bool isOptional,
        bool hasOutKeyword,
        bool hasInKeyword,
        bool hasRefKeyword)
    {
        VariableDeclarationStatementNode = variableDeclarationStatementNode;
        IsOptional = isOptional;
        HasOutKeyword = hasOutKeyword;
        HasInKeyword = hasInKeyword;
        HasRefKeyword = hasRefKeyword;

        var children = new List<ISyntax>
        {
            VariableDeclarationStatementNode
        };

        ChildBag = children.ToImmutableArray();
    }

    public VariableDeclarationNode VariableDeclarationStatementNode { get; }
    public bool IsOptional { get; }
    public bool HasOutKeyword { get; }
    public bool HasInKeyword { get; }
    public bool HasRefKeyword { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentEntryNode;
}