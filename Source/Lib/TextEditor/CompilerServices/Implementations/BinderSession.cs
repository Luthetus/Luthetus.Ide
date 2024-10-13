using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class BinderSession : IBinderSession
{
    public BinderSession(
        ResourceUri resourceUri,
        Key<IScope> globalScopeKey,
        NamespaceStatementNode topLevelNamespaceStatementNode,
        IBinder binder)
    {
        Binder = binder;

		ResourceUri = resourceUri;
        CurrentScopeKey = globalScopeKey;
        CurrentNamespaceStatementNode = topLevelNamespaceStatementNode;
        CurrentUsingStatementNodeList = new();
    }

	public ResourceUri ResourceUri { get; set; }
    public IBinder Binder { get; }
    public Key<IScope> CurrentScopeKey { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; set; }
    
    public List<IScope> ScopeList { get; }
    public Dictionary<ScopeKeyAndIdentifierText, TypeDefinitionNode> ScopeTypeDefinitionMap { get; } = new();
    public Dictionary<ScopeKeyAndIdentifierText, FunctionDefinitionNode> ScopeFunctionDefinitionMap { get; } = new();
    public Dictionary<ScopeKeyAndIdentifierText, IVariableDeclarationNode> ScopeVariableDeclarationMap { get; } = new();
    public Dictionary<Key<IScope>, TypeClauseNode> ScopeReturnTypeClauseNodeMap { get; } = new();
    
    public IScope GetScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
    	return Binder.GetScope(resourceUri, scopeKey);
    }
    
    public IScope GetScopeCurrent()
    {
    	return Binder.GetScope(ResourceUri, CurrentScopeKey);
    }
}
