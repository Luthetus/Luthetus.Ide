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
        Dictionary<string, Type> typeMap,
        Dictionary<string, BoundClassDeclarationNode> classDeclarationMap,
        Dictionary<string, BoundFunctionDeclarationNode> functionDeclarationMap,
        Dictionary<string, BoundVariableDeclarationStatementNode> variableDeclarationMap)
    {
        Parent = parent;
        ScopeReturnType = scopeReturnType;
        StartingIndexInclusive = startingIndexInclusive;
        EndingIndexExclusive = endingIndexExclusive;
        ResourceUri = resourceUri;
        TypeMap = typeMap;
        ClassDeclarationMap = classDeclarationMap;
        FunctionDeclarationMap = functionDeclarationMap;
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
    public Dictionary<string, Type> TypeMap { get; init; }
    public Dictionary<string, BoundClassDeclarationNode> ClassDeclarationMap { get; init; }
    public Dictionary<string, BoundFunctionDeclarationNode> FunctionDeclarationMap { get; init; }
    public Dictionary<string, BoundVariableDeclarationStatementNode> VariableDeclarationMap { get; init; }    
}
