using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a function.
/// </summary>
public sealed record FunctionArgumentEntryNode : ISyntaxNode
{
    public FunctionArgumentEntryNode(
        VariableDeclarationNode variableDeclarationNode,
        bool isOptional,
        bool hasOutKeyword,
        bool hasInKeyword,
        bool hasRefKeyword)
    {
        VariableDeclarationNode = variableDeclarationNode;
        IsOptional = isOptional;
        HasOutKeyword = hasOutKeyword;
        HasInKeyword = hasInKeyword;
        HasRefKeyword = hasRefKeyword;

        var children = new List<ISyntax>
        {
            VariableDeclarationNode
        };

        ChildList = children.ToImmutableArray();
    }

    public VariableDeclarationNode VariableDeclarationNode { get; }
    public bool IsOptional { get; }
    public bool HasOutKeyword { get; }
    public bool HasInKeyword { get; }
    public bool HasRefKeyword { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentEntryNode;
}