using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// TODO: Track the open and close braces for the function body.
/// </summary>
public sealed record FunctionDefinitionNode : ICodeBlockOwner
{
    public FunctionDefinitionNode(
        AccessModifierKind accessModifierKind,
        TypeClauseNode returnTypeClauseNode,
        IdentifierToken functionIdentifierToken,
        GenericArgumentsListingNode? genericArgumentsListingNode,
        FunctionArgumentsListingNode functionArgumentsListingNode,
        CodeBlockNode? codeBlockNode,
        ConstraintNode? constraintNode)
    {
        AccessModifierKind = accessModifierKind;
        ReturnTypeClauseNode = returnTypeClauseNode;
        FunctionIdentifierToken = functionIdentifierToken;
        GenericArgumentsListingNode = genericArgumentsListingNode;
        FunctionArgumentsListingNode = functionArgumentsListingNode;
        CodeBlockNode = codeBlockNode;
        ConstraintNode = constraintNode;

        var children = new List<ISyntax>
        {
            ReturnTypeClauseNode,
            FunctionIdentifierToken,
        };

        if (GenericArgumentsListingNode is not null)
            children.Add(GenericArgumentsListingNode);

        children.Add(FunctionArgumentsListingNode);

        if (CodeBlockNode is not null)
            children.Add(CodeBlockNode);
        
        if (ConstraintNode is not null)
            children.Add(ConstraintNode);

        ChildList = children.ToImmutableArray();
    }

    public AccessModifierKind AccessModifierKind { get; }
    public TypeClauseNode ReturnTypeClauseNode { get; }
    public IdentifierToken FunctionIdentifierToken { get; }
    public GenericArgumentsListingNode? GenericArgumentsListingNode { get; }
    public FunctionArgumentsListingNode FunctionArgumentsListingNode { get; }
    public CodeBlockNode? CodeBlockNode { get; private set; }
    public ConstraintNode? ConstraintNode { get; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionDefinitionNode;
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return ReturnTypeClauseNode;
    }
    
    public ICodeBlockOwner WithCodeBlockNode(CSharpParserModel parserModel, CodeBlockNode codeBlockNode)
    {
    	CodeBlockNode = codeBlockNode;
    	return this;
    }
}
