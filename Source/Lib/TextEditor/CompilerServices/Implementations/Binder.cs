using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class Binder : IBinder
{
	private readonly Dictionary<ResourceUri, ICompilationUnit> _compilationUnitMap = new();
	private readonly object _compilationUnitMapLock = new();

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
    
    public bool TryGetCompilationUnit(ResourceUri resourceUri, out ICompilationUnit compilationUnit)
    {
    	return _compilationUnitMap.TryGetValue(resourceUri, out compilationUnit);
    }
    
    public void UpsertCompilationUnit(ICompilationUnit compilationUnit)
    {
    	lock (_compilationUnitMapLock)
    	{
    		if (_compilationUnitMap.ContainsKey(compilationUnit.ResourceUri))
	    		_compilationUnitMap[compilationUnit.ResourceUri] = compilationUnit;
	    	else
	    		_compilationUnitMap.Add(compilationUnit.ResourceUri, compilationUnit);
    	}
    }
    
    public bool RemoveCompilationUnit(ResourceUri resourceUri)
    {
    	lock (_compilationUnitMapLock)
    	{
    		return _compilationUnitMap.Remove(resourceUri);
    	}
    }

    public void StartCompilationUnit(ResourceUri resourceUri)
    {
        // return;
    }
    
	public void FinalizeCompilationUnit(ICompilationUnit compilationUnit)
	{
		UpsertCompilationUnit(compilationUnit);
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
    
    public VariableDeclarationNode[] GetVariableDeclarationNodesByScope(ResourceUri resourceUri, int scopeIndexKey)
    {
		return Array.Empty<VariableDeclarationNode>();
    }
    
    public bool TryGetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
    	out VariableDeclarationNode variableDeclarationNode)
    {
    	variableDeclarationNode = null;
    	return false;
    }
    
    public bool TryAddVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
        VariableDeclarationNode variableDeclarationNode)
    {
    	return false;
    }
    
    public void SetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
        VariableDeclarationNode variableDeclarationNode)
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
    
    public void BindVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode, IParserModel model)
    {
    	return;
    }
}