using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

/// <summary>
/// <inheritdoc cref="IBinderSession"/>
/// </summary>
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

    public IBinder Binder { get; }

    public Key<IScope> CurrentScopeKey { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; set; }
    public ResourceUri ResourceUri { get; set; }
    
    public IScope GetScope(Key<IScope> scopeKey)
    {
    	return Binder.GetScope(scopeKey);
    }
    
    public IScope GetScopeCurrent()
    {
    	return Binder.GetScope(CurrentScopeKey);
    }
}
