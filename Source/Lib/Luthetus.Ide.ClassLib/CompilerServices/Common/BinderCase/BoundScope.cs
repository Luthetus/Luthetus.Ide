using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;

public sealed record BoundScope
{
    public BoundScope(
        BoundScope? parent,
        Type? scopeReturnType,
        int startingIndexInclusive,
        int? endingIndexExclusive,
        ResourceUri resourceUri,
        Dictionary<string, BoundClassDefinitionNode> classDefinitionMap,
        Dictionary<string, BoundFunctionDefinitionNode> functionDefinitionMap,
        Dictionary<string, BoundVariableDeclarationStatementNode> variableDeclarationMap)
    {
        Parent = parent;
        ScopeReturnType = scopeReturnType;
        StartingIndexInclusive = startingIndexInclusive;
        EndingIndexExclusive = endingIndexExclusive;
        ResourceUri = resourceUri;
        ClassDefinitionMap = classDefinitionMap;
        FunctionDefinitionMap = functionDefinitionMap;
        VariableDeclarationMap = variableDeclarationMap;
    }

    public BoundScopeKey BoundScopeKey { get; init; } = BoundScopeKey.NewBoundScopeKey();
    public BoundScope? Parent { get; init; }
    /// <summary>A <see cref="ScopeReturnType"/> with the value of "null" means refer to the <see cref="Parent"/> bound scope's <see cref="ScopeReturnType"/></summary>
    public Type? ScopeReturnType { get; init; }
    public int StartingIndexInclusive { get; init; }
    /// <summary>TODO: Remove the "internal set" hack and make a new immutable instance instead.</summary>
    public int? EndingIndexExclusive { get; internal set; }
    public ResourceUri ResourceUri { get; init; }
    public Dictionary<string, BoundClassDefinitionNode> ClassDefinitionMap { get; init; }
    public Dictionary<string, BoundFunctionDefinitionNode> FunctionDefinitionMap { get; init; }
    public Dictionary<string, BoundVariableDeclarationStatementNode> VariableDeclarationMap { get; init; }    
}
