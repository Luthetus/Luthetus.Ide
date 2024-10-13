using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a function.
/// </summary>
public sealed record FunctionArgumentEntryNode : ISyntaxNode
{
    public FunctionArgumentEntryNode(
        VariableDeclarationNode variableDeclarationNode,
        ISyntaxToken? optionalCompileTimeConstantToken,
        bool isOptional,
        bool hasParamsKeyword,
        bool hasOutKeyword,
        bool hasInKeyword,
        bool hasRefKeyword)
    {
        VariableDeclarationNode = variableDeclarationNode;
        OptionalCompileTimeConstantToken = optionalCompileTimeConstantToken;
        IsOptional = isOptional;
        HasParamsKeyword = hasParamsKeyword;
        HasOutKeyword = hasOutKeyword;
        HasInKeyword = hasInKeyword;
        HasRefKeyword = hasRefKeyword;

        var children = new List<ISyntax>
        {
            VariableDeclarationNode
        };
        
        if (OptionalCompileTimeConstantToken is not null)
        	children.Add(OptionalCompileTimeConstantToken);

        ChildList = children.ToImmutableArray();
    }

    public VariableDeclarationNode VariableDeclarationNode { get; }
    public ISyntaxToken? OptionalCompileTimeConstantToken { get; }
    public bool IsOptional { get; }
    public bool HasParamsKeyword { get; }
    public bool HasOutKeyword { get; }
    public bool HasInKeyword { get; }
    public bool HasRefKeyword { get; }

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentEntryNode;
}