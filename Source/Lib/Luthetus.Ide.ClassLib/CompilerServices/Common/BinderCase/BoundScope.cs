using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;

public sealed record BoundScope
{
    public BoundScope(
        BoundScope? parent,
        Type? scopeReturnType,
        int startingIndexInclusive,
        int? endingIndexExclusive,
        Dictionary<string, Type> typeMap,
        Dictionary<string, BoundClassDeclarationNode> classDeclarationMap,
        Dictionary<string, BoundFunctionDeclarationNode> functionDeclarationMap,
        Dictionary<string, BoundVariableDeclarationStatementNode> variableDeclarationMap)
    {
        Parent = parent;
        ScopeReturnType = scopeReturnType;
        StartingIndexInclusive = startingIndexInclusive;
        EndingIndexExclusive = endingIndexExclusive;
        TypeMap = typeMap;
        ClassDeclarationMap = classDeclarationMap;
        FunctionDeclarationMap = functionDeclarationMap;
        VariableDeclarationMap = variableDeclarationMap;
    }

    public BoundScope? Parent { get; init; }
    /// <summary>A <see cref="ScopeReturnType"/> with the value of "null" means refer to the <see cref="Parent"/> bound scope's <see cref="ScopeReturnType"/></summary>
    public Type? ScopeReturnType { get; init; }
    public int StartingIndexInclusive { get; init; }
    /// <summary>TODO: Remove the "internal set" hack and make a new immutable instance instead.</summary>
    public int? EndingIndexExclusive { get; internal set; }
    public Dictionary<string, Type> TypeMap { get; init; }
    public Dictionary<string, BoundClassDeclarationNode> ClassDeclarationMap { get; init; }
    public Dictionary<string, BoundFunctionDeclarationNode> FunctionDeclarationMap { get; init; }
    public Dictionary<string, BoundVariableDeclarationStatementNode> VariableDeclarationMap { get; init; }
}
