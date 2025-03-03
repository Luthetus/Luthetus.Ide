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

    public TextEditorDiagnostic[] DiagnosticsList { get; } = Array.Empty<TextEditorDiagnostic>();
    public Symbol[] SymbolsList { get; } = Array.Empty<Symbol>();
    public IReadOnlyDictionary<ResourceUri, List<Scope>> ScopeList { get; } = new Dictionary<ResourceUri, List<Scope>>();
    public IReadOnlyDictionary<string, TypeDefinitionNode> AllTypeDefinitions { get; } = new Dictionary<string, TypeDefinitionNode>();

    public TextEditorTextSpan? GetDefinitionTextSpan(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource)
    {
    	return null;
    }
    
    public ISyntaxNode? GetDefinitionNode(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource, Symbol? symbol = null)
    {
    	return null;
    }

    public ISyntaxNode? GetSyntaxNode(int positionIndex, ResourceUri resourceUri, ICompilerServiceResource? compilerServiceResource)
    {
        return null;
    }

	public Scope GetScopeByPositionIndex(ResourceUri resourceUri, int positionIndex)
    {
        return default;
    }

    public Scope GetScope(TextEditorTextSpan textSpan)
    {
        return default;
    }
    
    public Scope GetScopeByScopeIndexKey(ResourceUri resourceUri, int scopeIndexKey)
    {
    	return default;
    }
    
    public Scope[]? GetScopeList(ResourceUri resourceUri)
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
        return new BinderSession(resourceUri, 0, null, this);
    }
    
	public void FinalizeBinderSession(IBinderSession binderSession)
	{
		UpsertBinderSession(binderSession);
	}
    
    public TypeDefinitionNode[] GetTypeDefinitionNodesByScope(ResourceUri resourceUri, int scopeIndexKey)
	{
		return Array.Empty<TypeDefinitionNode>();
	}
	
	public bool TryGetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string typeIdentifierText,
    	out TypeDefinitionNode typeDefinitionNode)
    {
    	typeDefinitionNode = null;
    	return false;
    }
    
    public bool TryAddTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	return false;
    }
    
    public void SetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	return;
    }
	
    public FunctionDefinitionNode[] GetFunctionDefinitionNodesByScope(ResourceUri resourceUri, int scopeIndexKey)
    {
		return Array.Empty<FunctionDefinitionNode>();
    }
    
    public bool TryGetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string functionIdentifierText,
    	out FunctionDefinitionNode functionDefinitionNode)
    {
    	functionDefinitionNode = null;
    	return false;
    }
    
    public bool TryAddFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	return false;
    }
    
    public void SetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	return;
    }
    
    public IVariableDeclarationNode[] GetVariableDeclarationNodesByScope(ResourceUri resourceUri, int scopeIndexKey)
    {
		return Array.Empty<IVariableDeclarationNode>();
    }
    
    public bool TryGetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
    	out IVariableDeclarationNode variableDeclarationNode)
    {
    	variableDeclarationNode = null;
    	return false;
    }
    
    public bool TryAddVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode)
    {
    	return false;
    }
    
    public void SetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode)
    {
    	return;
    }
    
    public TypeClauseNode? GetReturnTypeClauseNodeByScope(ResourceUri resourceUri, int scopeIndexKey)
    {
		return null;
    }
    
    public bool TryAddReturnTypeClauseNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
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