using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
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
	
	private int _symbolId = 0;

    public CSharpBinderSession(
        ResourceUri resourceUri,
        CSharpBinder binder,
        int globalScopeIndexKey,
        NamespaceStatementNode topLevelNamespaceStatementNode)
    {
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
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// Using the text span is a bad idea,
    /// it will reference the source text and make it so the memory is not cleared when
    /// a new TextEditorModel is instantiated due to an edit to the file.
    ///
    /// Store only the required information (most importantly don't store the source text)
    /// (the source text is on the TextEditorTextSpan so it is stored as a result.).
	/// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	///
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
    ///
    /// -----------------------------------------------------------------------
    /// Possible Future Optimization if needed:
    ///
    /// Making a symbol foreach of the member accesses is thought to be a large amount
    /// of memory to hold.
    ///
    /// Instead of this, we can probably create a symbol for the first identifier
    /// in a chain of member accesses.
    ///
    /// Then, use the cursor position to determine where-in the member access
    /// chain they are hovering.
    ///
    /// And finally, calculate a symbol on demand for that section of the member access chain
    /// instead of pre-calculating it and storing it.
    ///
    /// -----------------------------------------------------------------------
    /// SIDE NOTE:
    /// Track references by having the binder contain a Dictionary<FullyQualifiedName, List<ResourceUri>>
    /// such that the 'List<ResourceUri>' is a list that contains all the files which contain at least 1 or more
    /// references to the 'FullyQualifiedName'.
    ///
    /// In order to know the count of references to the 'FullyQualifiedName',
    /// visit each 'ResourceUri' and at this point decide how to go from here.
    ///
    /// Maybe one could check the 'CSharpBinderSession' for each resource uri and
    /// it has a Dictionary<FullyQualifiedName, List<TextEditorTextSpan>>
    ///
    /// Where each 'List<TextEditorTextSpan>' is a list containing all the text spans
    /// that reference the 'FullyQualifiedName' within that file.
    /// </summary>
    public Dictionary<int, (ResourceUri ResourceUri, int StartInclusiveIndex)> SymbolIdToExternalTextSpanMap { get; } = new();
    
    IBinder IBinderSession.Binder => Binder;

    public int GetNextIndexKey()
    {
    	return ++_indexKey;
    }
    
    public int GetNextSymbolId()
    {
    	return ++_symbolId;
    }
}
