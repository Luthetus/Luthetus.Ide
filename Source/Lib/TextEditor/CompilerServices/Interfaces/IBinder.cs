using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

/// <summary>
/// The IBinder should only define the way in which the text editor will "query" the CompilationUnit(s) / Binder.
/// Any implementation details of parsing should NOT be included here.
/// </summary>
public interface IBinder
{
    public IReadOnlyDictionary<string, TypeDefinitionNode> AllTypeDefinitions { get; }
	
	/// <summary>
	/// Returns the text span at which the definition exists in the source code.
	/// </summary>
    public TextEditorTextSpan? GetDefinitionTextSpan(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource);
    
    /// <summary>
    /// Returns the <see cref="ISyntaxNode"/> that represents the definition in the <see cref="CompilationUnit"/>.
    ///
    /// The option argument 'symbol' can be provided if available. It might provide additional information to the method's implementation
    /// that is necessary to find certain nodes (ones that are in a separate file are most common to need a symbol to find).
    /// </summary>
    public ISyntaxNode? GetDefinitionNode(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource, Symbol? symbol = null);
    
    /// <summary>
    /// Looks up the <see cref="IScope"/> that encompasses the provided positionIndex.
    ///
    /// Then, checks the <see cref="IScope"/>.<see cref="IScope.CodeBlockOwner"/>'s children
    /// to determine which node exists at the positionIndex.
    ///
    /// If the <see cref="IScope"/> cannot be found, then as a fallback the provided compilationUnit's
    /// <see cref="CompilationUnit.RootCodeBlockNode"/> will be treated
    /// the same as if it were the <see cref="IScope"/>.<see cref="IScope.CodeBlockOwner"/>.
    ///
    /// If the provided compilerServiceResource?.CompilationUnit is null, then the fallback step will not occur.
    /// The fallback step is expected to occur due to the global scope being implemented with a null
    /// <see cref="IScope"/>.<see cref="IScope.CodeBlockOwner"/> at the time of this comment.
    /// </summary>
    public ISyntaxNode? GetSyntaxNode(int positionIndex, ResourceUri resourceUri, ICompilerServiceResource? compilerServiceResource);
    
    /// <summary><see cref="FinalizeBinderSession"/></summary>
    public IBinderSession StartBinderSession(ResourceUri resourceUri);
    
	/// <summary><see cref="StartBinderSession"/></summary>
	public void FinalizeBinderSession(IBinderSession binderSession);
    
    public bool TryGetBinderSession(ResourceUri resourceUri, out IBinderSession binderSession);
    public void UpsertBinderSession(IBinderSession binderSession);
    /// <summary>Returns true if the entry was removed</summary>
    public bool RemoveBinderSession(ResourceUri resourceUri);
    
    public Scope GetScope(TextEditorTextSpan textSpan);
    public Scope GetScopeByPositionIndex(ResourceUri resourceUri, int positionIndex);
    public Scope GetScopeByScopeIndexKey(ResourceUri resourceUri, int scopeIndexKey);
    public Scope[]? GetScopeList(ResourceUri resourceUri);
    
    public TypeDefinitionNode[] GetTypeDefinitionNodesByScope(ResourceUri resourceUri, int scopeIndexKey);
    
    public bool TryGetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string typeIdentifierText,
    	out TypeDefinitionNode typeDefinitionNode);
    
    public FunctionDefinitionNode[] GetFunctionDefinitionNodesByScope(ResourceUri resourceUri, int scopeIndexKey);
    
    public bool TryGetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string functionIdentifierText,
    	out FunctionDefinitionNode functionDefinitionNode);
    
    public IVariableDeclarationNode[] GetVariableDeclarationNodesByScope(ResourceUri resourceUri, int scopeIndexKey);
    
    public bool TryGetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
    	out IVariableDeclarationNode variableDeclarationNode);
    
    public TypeClauseNode? GetReturnTypeClauseNodeByScope(ResourceUri resourceUri, int scopeIndexKey);
    
    public void ClearStateByResourceUri(ResourceUri resourceUri);
    public void AddNamespaceToCurrentScope(string namespaceString, IParserModel model);
}
