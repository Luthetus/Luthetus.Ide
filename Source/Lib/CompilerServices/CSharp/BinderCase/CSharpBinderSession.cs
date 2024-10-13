using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.CSharp.BinderCase;

public class CSharpBinderSession : IBinderSession
{
	public readonly Dictionary<ResourceUri, List<IScope>> BoundScopes = new();
    /// <summary>
    /// Key is the name of the type, prefixed with the ScopeKey and '_' to separate the ScopeKey from the type.
    /// Given: public class MyClass { }
    /// Then: Key == new ScopeKeyAndIdentifierText(ScopeKey, "MyClass")
    /// </summary>
    public readonly Dictionary<ScopeKeyAndIdentifierText, TypeDefinitionNode> ScopeTypeDefinitionMap = new();
    /// <summary>
    /// Key is the name of the function, prefixed with the ScopeKey and '_' to separate the ScopeKey from the function.
    /// Given: public void MyMethod() { }
    /// Then: Key == new ScopeKeyAndIdentifierText(ScopeKey, "MyMethod")
    /// </summary>
    public readonly Dictionary<ScopeKeyAndIdentifierText, FunctionDefinitionNode> ScopeFunctionDefinitionMap = new();
    /// <summary>
    /// Key is the name of the variable, prefixed with the ScopeKey and '_' to separate the ScopeKey from the variable.
    /// Given: var myVariable = 2;
    /// Then: Key == new ScopeKeyAndIdentifierText(ScopeKey, "myVariable")
    /// </summary>
    public readonly Dictionary<ScopeKeyAndIdentifierText, IVariableDeclarationNode> ScopeVariableDeclarationMap = new();
    public readonly Dictionary<Key<IScope>, TypeClauseNode> ScopeReturnTypeClauseNodeMap = new();

    public CSharpBinderSession(
        ResourceUri resourceUri,
        CSharpBinder binder,
        Key<IScope> globalScopeKey,
        NamespaceStatementNode topLevelNamespaceStatementNode)
    {
    	ResourceUri = resourceUri;
        Binder = binder;
        CurrentScopeKey = globalScopeKey;
        CurrentNamespaceStatementNode = topLevelNamespaceStatementNode;
        CurrentUsingStatementNodeList = new();
    }

	public ResourceUri ResourceUri { get; }
    public CSharpBinder Binder { get; }
    public Key<IScope> CurrentScopeKey { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; set; }

    IBinder IBinderSession.Binder => Binder;
    Key<IScope> IBinderSession.CurrentScopeKey { get => CurrentScopeKey; set => CurrentScopeKey = value; }
    
    public IScope GetScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
    	return Binder.GetScope(resourceUri, scopeKey);
    }
    
    public IScope GetScopeCurrent()
    {
    	return Binder.GetScope(ResourceUri, CurrentScopeKey);
    }
    
    
}
