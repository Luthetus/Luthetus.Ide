using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface IBinder
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; }
    public ImmutableArray<ITextEditorSymbol> SymbolsList { get; }
    public ImmutableDictionary<ResourceUri, List<IScope>> ScopeList { get; }

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource);
    public ISyntaxNode? GetSyntaxNode(int positionIndex, CompilationUnit compilationUnit);
    
    public bool TryGetBinderSession(ResourceUri resourceUri, out IBinderSession binderSession);
    public void UpsertBinderSession(IBinderSession binderSession);
    /// <summary>Returns true if the entry was removed</summary>
    public bool RemoveBinderSession(ResourceUri resourceUri);
    
    public IScope? GetScope(TextEditorTextSpan textSpan);
    public IScope? GetScope(ResourceUri resourceUri, int positionIndex);
    public IScope? GetScope(ResourceUri resourceUri, Key<IScope> scopeKey);
    public IScope[]? GetScopeList(ResourceUri resourceUri);
    
    public TypeDefinitionNode[] GetTypeDefinitionNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey);
    
    public bool TryGetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
    	out TypeDefinitionNode typeDefinitionNode);
    
    public bool TryAddTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode);
        
    public void SetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode);
    
    public FunctionDefinitionNode[] GetFunctionDefinitionNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey);
    
    public bool TryGetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
    	out FunctionDefinitionNode functionDefinitionNode);
    
    public bool TryAddFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode);
        
    public void SetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode);
    
    public IVariableDeclarationNode[] GetVariableDeclarationNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey);
    
    public bool TryGetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
    	out IVariableDeclarationNode variableDeclarationNode);
    
    public bool TryAddVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode);
        
    public void SetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode);
    
    public TypeClauseNode? GetReturnTypeClauseNodeByScope(ResourceUri resourceUri, Key<IScope> scopeKey);
    
    public bool TryAddReturnTypeClauseNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
        TypeClauseNode typeClauseNode);
    
    public IBinderSession ConstructBinderSession(ResourceUri resourceUri);
    public void ClearStateByResourceUri(ResourceUri resourceUri);
    public void AddNamespaceToCurrentScope(string namespaceString, IParserModel model);
    public void BindFunctionOptionalArgument(FunctionArgumentEntryNode functionArgumentEntryNode, IParserModel model);
    public void BindVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode, IParserModel model);
}
