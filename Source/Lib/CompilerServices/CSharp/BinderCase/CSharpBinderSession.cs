using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.CompilerServices.CSharp.BinderCase;

public class CSharpBinderSession : IBinderSession
{
	/// <summary>
	/// Should 0 be the global scope?
	/// </summary>
	private int _indexKey = 0;

    public CSharpBinderSession(
        ResourceUri resourceUri,
        CSharpBinder binder,
        int globalScopeIndexKey,
        NamespaceStatementNode topLevelNamespaceStatementNode)
    {
    	#if DEBUG
    	++LuthetusDebugSomething.BinderSession_ConstructorInvocationCount;
    	#endif
    
    	ResourceUri = resourceUri;
        Binder = binder;
        CurrentScopeIndexKey = globalScopeIndexKey;
        CurrentNamespaceStatementNode = topLevelNamespaceStatementNode;
        CurrentUsingStatementNodeList = new();
    }

	public ResourceUri ResourceUri { get; }
    public CSharpBinder Binder { get; }
    public int CurrentScopeIndexKey { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; set; }
    
    public DiagnosticBag DiagnosticBag { get; } = new();
    public List<IScope> ScopeList { get; } = new();
    public Dictionary<ScopeKeyAndIdentifierText, TypeDefinitionNode> ScopeTypeDefinitionMap { get; } = new();
    public Dictionary<ScopeKeyAndIdentifierText, FunctionDefinitionNode> ScopeFunctionDefinitionMap { get; } = new();
    public Dictionary<ScopeKeyAndIdentifierText, IVariableDeclarationNode> ScopeVariableDeclarationMap { get; } = new();
    public Dictionary<int, TypeClauseNode> ScopeReturnTypeClauseNodeMap { get; } = new();

    IBinder IBinderSession.Binder => Binder;
    int IBinderSession.CurrentScopeIndexKey { get => CurrentScopeIndexKey; set => CurrentScopeIndexKey = value; }
    
    public int GetNextIndexKey()
    {
    	return ++_indexKey;
    }
}
