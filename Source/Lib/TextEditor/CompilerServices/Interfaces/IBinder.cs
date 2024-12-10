using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface IBinder
{
    public TextEditorDiagnostic[] DiagnosticsList { get; }
    public ITextEditorSymbol[] SymbolsList { get; }
	
	/// <summary>
	/// Returns the text span at which the definition exists in the source code.
	/// </summary>
    public TextEditorTextSpan? GetDefinitionTextSpan(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource);
    
    /// <summary>
    /// Returns the <see cref="ISyntaxNode"/> that represents the definition in the <see cref="CompilationUnit"/>.
    /// </summary>
    public ISyntaxNode? GetDefinitionNode(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource);
    
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
    
    public IScope? GetScope(TextEditorTextSpan textSpan);
    public IScope? GetScopeByPositionIndex(ResourceUri resourceUri, int positionIndex);
    public IScope? GetScopeByScopeIndexKey(ResourceUri resourceUri, int scopeIndexKey);
    public IScope[]? GetScopeList(ResourceUri resourceUri);
    
    public TypeDefinitionNode[] GetTypeDefinitionNodesByScope(ResourceUri resourceUri, int scopeIndexKey);
    
    public bool TryGetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string typeIdentifierText,
    	out TypeDefinitionNode typeDefinitionNode);
    
    public bool TryAddTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode);
        
    public void SetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode);
    
    public FunctionDefinitionNode[] GetFunctionDefinitionNodesByScope(ResourceUri resourceUri, int scopeIndexKey);
    
    public bool TryGetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string functionIdentifierText,
    	out FunctionDefinitionNode functionDefinitionNode);
    
    public bool TryAddFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode);
        
    public void SetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode);
    
    public IVariableDeclarationNode[] GetVariableDeclarationNodesByScope(ResourceUri resourceUri, int scopeIndexKey);
    
    public bool TryGetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
    	out IVariableDeclarationNode variableDeclarationNode);
    
    public bool TryAddVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode);
        
    public void SetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode);
    
    public TypeClauseNode? GetReturnTypeClauseNodeByScope(ResourceUri resourceUri, int scopeIndexKey);
    
    public bool TryAddReturnTypeClauseNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
        TypeClauseNode typeClauseNode);
    
    public void ClearStateByResourceUri(ResourceUri resourceUri);
    public void AddNamespaceToCurrentScope(string namespaceString, IParserModel model);
    public void BindFunctionOptionalArgument(FunctionArgumentEntryNode functionArgumentEntryNode, IParserModel model);
    public void BindVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode, IParserModel model);
}
