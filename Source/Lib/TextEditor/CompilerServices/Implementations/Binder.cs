using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class Binder : IBinder
{
	private readonly Dictionary<ResourceUri, IBinderSession> _binderSessionMap = new();
	private readonly object _binderSessionMapLock = new();

    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; } = ImmutableArray<TextEditorDiagnostic>.Empty;
    public ImmutableArray<ITextEditorSymbol> SymbolsList { get; } = ImmutableArray<ITextEditorSymbol>.Empty;
    public ImmutableDictionary<ResourceUri, List<IScope>> ScopeList { get; } = ImmutableDictionary<ResourceUri, List<IScope>>.Empty;
    

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource)
    {
    	return null;
    }

    public ISyntaxNode? GetSyntaxNode(int positionIndex, CompilationUnit compilationUnit)
    {
        return null;
    }

	public IScope? GetScope(ResourceUri resourceUri, int positionIndex)
    {
        return null;
    }

    public IScope? GetScope(TextEditorTextSpan textSpan)
    {
        return null;
    }
    
    public IScope? GetScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
    	return null;
    }
    
    public IScope[]? GetScopeList(ResourceUri resourceUri)
    {
    	return null;
    }
    
    public bool TryGetBinderSession(ResourceUri resourceUri, out IBinderSession binderSession)
    {
    	return _binderSessionMap.TryGetValue(resourceUri, out binderSession);
    }
    
    public void UpsertBinderSession(IBinderSession binderSession)
    {
    	lock (_binderSessionMapLock)
    	{
    		if (_binderSessionMap.ContainsKey(binderSession.ResourceUri))
	    		_binderSessionMap[binderSession.ResourceUri] = binderSession;
	    	else
	    		_binderSessionMap.Add(binderSession.ResourceUri, binderSession);
    	}
    }
    
    public bool RemoveBinderSession(ResourceUri resourceUri)
    {
    	lock (_binderSessionMapLock)
    	{
    		return _binderSessionMap.Remove(resourceUri);
    	}
    }

    public IBinderSession StartBinderSession(ResourceUri resourceUri)
    {
        return new BinderSession(resourceUri, Key<IScope>.Empty, null, this);
    }
    
	public void FinalizeBinderSession(IBinderSession binderSession)
	{
		UpsertBinderSession(binderSession);
	}
    
    public TypeDefinitionNode[] GetTypeDefinitionNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
	{
		return Array.Empty<TypeDefinitionNode>();
	}
	
	public bool TryGetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
    	out TypeDefinitionNode typeDefinitionNode)
    {
    	typeDefinitionNode = null;
    	return false;
    }
    
    public bool TryAddTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	return false;
    }
    
    public void SetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	return;
    }
	
    public FunctionDefinitionNode[] GetFunctionDefinitionNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
		return Array.Empty<FunctionDefinitionNode>();
    }
    
    public bool TryGetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
    	out FunctionDefinitionNode functionDefinitionNode)
    {
    	functionDefinitionNode = null;
    	return false;
    }
    
    public bool TryAddFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	return false;
    }
    
    public void SetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	return;
    }
    
    public IVariableDeclarationNode[] GetVariableDeclarationNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
		return Array.Empty<IVariableDeclarationNode>();
    }
    
    public bool TryGetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
    	out IVariableDeclarationNode variableDeclarationNode)
    {
    	variableDeclarationNode = null;
    	return false;
    }
    
    public bool TryAddVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode)
    {
    	return false;
    }
    
    public void SetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode)
    {
    	return;
    }
    
    public TypeClauseNode? GetReturnTypeClauseNodeByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
		return null;
    }
    
    public bool TryAddReturnTypeClauseNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
        TypeClauseNode typeClauseNode)
    {
    	return false;
    }

    public void ClearStateByResourceUri(ResourceUri resourceUri)
    {
        return;
    }
    
    public void AddNamespaceToCurrentScope(string namespaceString, IParserModel model)
    {
    	return;
    }
    
    public void BindFunctionOptionalArgument(FunctionArgumentEntryNode functionArgumentEntryNode, IParserModel model)
    {
    	return;
    }
    
    public void BindVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode, IParserModel model)
    {
    	return;
    }
}