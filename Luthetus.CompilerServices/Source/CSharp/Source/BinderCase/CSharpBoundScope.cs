using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.BinderCase;

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
        Dictionary<string, VariableDeclarationNode> variableDeclarationMap,
        IdentifierToken encompassingNamespaceIdentifierToken)
    {
        Parent = parent;
        ScopeReturnTypeClauseNode = scopeReturnTypeClauseNode;
        StartingIndexInclusive = startingIndexInclusive;
        EndingIndexExclusive = endingIndexExclusive;
        ResourceUri = resourceUri;
        TypeDefinitionMap = typeDefinitionMap;
        FunctionDefinitionMap = functionDefinitionMap;
        VariableDeclarationMap = variableDeclarationMap;
        EncompassingNamespaceIdentifierToken = encompassingNamespaceIdentifierToken;
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
    public Dictionary<string, VariableDeclarationNode> VariableDeclarationMap { get; init; }
    public IdentifierToken EncompassingNamespaceIdentifierToken { get; }
}