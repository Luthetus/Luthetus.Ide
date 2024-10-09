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
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; } = ImmutableArray<TextEditorDiagnostic>.Empty;
    public ImmutableArray<ITextEditorSymbol> SymbolsList { get; } = ImmutableArray<ITextEditorSymbol>.Empty;

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource)
    {
        return null;
    }

    public ISyntaxNode? GetSyntaxNode(int positionIndex, CompilationUnit compilationUnit)
    {
        return null;
    }

	public IScope? GetScope(int positionIndex, ResourceUri resourceUri)
    {
        return null;
    }

    public IScope? GetScope(TextEditorTextSpan textSpan)
    {
        return null;
    }
    
    public IScope? GetScope(Key<IScope> scopeKey)
    {
    	return null;
    }

	public TypeDefinitionNode[] GetTypeDefinitionNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
	{
		return Array.Empty<TypeDefinitionNode>();
	}
    
    public bool TryAddTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	return false;
    }
	
    public FunctionDefinitionNode[] GetFunctionDefinitionNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
		return Array.Empty<FunctionDefinitionNode>();
    }
    
    public bool TryAddFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	return false;
    }
    
    public IVariableDeclarationNode[] GetVariableDeclarationNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
		return Array.Empty<IVariableDeclarationNode>();
    }
    
    public bool TryAddVariableDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDefinitionNode)
    {
    	return false;
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

    public IBinderSession ConstructBinderSession(ResourceUri resourceUri)
    {
        return new BinderSession(resourceUri, Key<IScope>.Empty, null, this);
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