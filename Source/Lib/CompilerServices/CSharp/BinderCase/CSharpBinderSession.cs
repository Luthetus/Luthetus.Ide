using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.CSharp.BinderCase;

public class CSharpBinderSession : IBinderSession
{
    public CSharpBinderSession(
        ResourceUri resourceUri,
        Key<IScope> globalScopeKey,
        NamespaceStatementNode topLevelNamespaceStatementNode,
        CSharpBinder binder)
    {
        Binder = binder;

		ResourceUri = resourceUri;
        CurrentScopeKey = globalScopeKey;
        CurrentNamespaceStatementNode = topLevelNamespaceStatementNode;
        CurrentUsingStatementNodeList = new();
    }

    public CSharpBinder Binder { get; }

    public Key<IScope> CurrentScopeKey { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; set; }
    public ResourceUri ResourceUri { get; set; }

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
