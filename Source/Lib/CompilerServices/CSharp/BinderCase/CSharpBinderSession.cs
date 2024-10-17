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
    
    public DiagnosticBag DiagnosticBag { get; } = new();
    public List<IScope> ScopeList { get; } = new();
    public Dictionary<ScopeKeyAndIdentifierText, TypeDefinitionNode> ScopeTypeDefinitionMap { get; } = new();
    public Dictionary<ScopeKeyAndIdentifierText, FunctionDefinitionNode> ScopeFunctionDefinitionMap { get; } = new();
    public Dictionary<ScopeKeyAndIdentifierText, IVariableDeclarationNode> ScopeVariableDeclarationMap { get; } = new();
    public Dictionary<Key<IScope>, TypeClauseNode> ScopeReturnTypeClauseNodeMap { get; } = new();

    IBinder IBinderSession.Binder => Binder;
    Key<IScope> IBinderSession.CurrentScopeKey { get => CurrentScopeKey; set => CurrentScopeKey = value; }
}
