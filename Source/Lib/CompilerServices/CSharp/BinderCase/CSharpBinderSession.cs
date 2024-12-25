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
    
    /// <summary>
    /// If a symbol references a defintion that exists within a different ResourceUri,
    /// and is not "indexed" information, then this disambiguates the definition.
    ///
    /// Example: TypeClauseNodes reference TypeDefinitionNodes.
    /// If the TypeDefinitionNode being referenced exists in a different file,
    /// this is not an issue because all "top level TypeDefinitionNodes"
    /// are maintained in a global list / dictionary.
    ///
    /// But, if a VariableReferenceNode references a VariableDeclarationNode,
    /// and that 'VariableDeclarationNode' is a property of a 'TypeDefinitionNode',
    /// then this is now too much data to fit onto a symbol (without modifying the symbol type itself).
    ///
    /// So, this property allows one to say, "the 0th symbol I instantiated
    /// is a reference to this TextEditorTextSpan (which is in a different file)."
    ///
    /// The 'int' resets per parse of an entire file. And starts at 0
    /// and increments by 1 each symbol instantiated in that file.
    ///
    /// This requires the 'ISymbol' type to be modified.
    /// But, this solution uses minimal memory since it only requires an 'int', and it is more of an "opt-in".
    /// Since this dictionary property isn't required by any ICompilerService implementations.
    ///
    /// But, if a different ICompilerService implementation wanted to map
    /// from a symbol to arbitrary information, they'd be able to do so.
    /// </summary>
    public Dictionary<int, TextEditorTextSpan> SymbolIdToExternalTextSpanMap { get; } = new();
    
    IBinder IBinderSession.Binder => Binder;

    public int GetNextIndexKey()
    {
    	return ++_indexKey;
    }
}
