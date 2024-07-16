using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.CSharp.BinderCase;

public sealed record CSharpBoundScope : IBoundScope
{
    public CSharpBoundScope(
        CSharpBoundScope? parent,
        TypeClauseNode? scopeReturnTypeClauseNode,
        int startingIndexInclusive,
        int? endingIndexExclusive,
        ResourceUri resourceUri,
        Dictionary<string, TypeDefinitionNode> typeDefinitionMap,
        Dictionary<string, FunctionDefinitionNode> functionDefinitionMap,
        Dictionary<string, IVariableDeclarationNode> variableDeclarationMap,
        NamespaceStatementNode encompassingNamespaceStatementNode,
        List<UsingStatementNode> currentUsingStatementNodeList)
    {
        Parent = parent;
        ScopeReturnTypeClauseNode = scopeReturnTypeClauseNode;
        StartingIndexInclusive = startingIndexInclusive;
        EndingIndexExclusive = endingIndexExclusive;
        ResourceUri = resourceUri;
        TypeDefinitionMap = typeDefinitionMap;
        FunctionDefinitionMap = functionDefinitionMap;
        VariableDeclarationMap = variableDeclarationMap;
        EncompassingNamespaceStatementNode = encompassingNamespaceStatementNode;
        CurrentUsingStatementNodeList = currentUsingStatementNodeList;
    }

    public BoundScopeKey BoundScopeKey { get; init; } = BoundScopeKey.NewKey();
    public CSharpBoundScope? Parent { get; init; }
    /// <summary>A <see cref="ScopeReturnType"/> with the value of "null" means refer to the <see cref="Parent"/> bound scope's <see cref="ScopeReturnType"/></summary>
    public TypeClauseNode? ScopeReturnTypeClauseNode { get; init; }
    public int StartingIndexInclusive { get; init; }
    /// <summary>TODO: Remove the "set" hack and make a new immutable instance instead.</summary>
    public int? EndingIndexExclusive { get; set; }
    public ResourceUri ResourceUri { get; init; }
    /// <summary>
    /// The 'string' Key for <see cref="TypeDefinitionMap"/> is the name of the type.<br/><br/>
    /// Given: public class MyClass { /* ... */ }<br/>
    /// Then: the key for <see cref="TypeDefinitionMap"/> is 'MyClass' 
    /// </summary>
    public Dictionary<string, TypeDefinitionNode> TypeDefinitionMap { get; init; }
    public Dictionary<string, FunctionDefinitionNode> FunctionDefinitionMap { get; init; }
    public Dictionary<string, IVariableDeclarationNode> VariableDeclarationMap { get; init; }
    public NamespaceStatementNode EncompassingNamespaceStatementNode { get; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; }
}