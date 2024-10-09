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

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource);
    public ISyntaxNode? GetSyntaxNode(int positionIndex, CompilationUnit compilationUnit);
    
    public IScope? GetScope(TextEditorTextSpan textSpan);
    public IScope? GetScope(int positionIndex, ResourceUri resourceUri);
    public IScope? GetScope(Key<IScope> scopeKey);
    
    public TypeDefinitionNode[] GetTypeDefinitionNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey);
    
    public bool TryAddTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode);
    
    public FunctionDefinitionNode[] GetFunctionDefinitionNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey);
    
    public bool TryAddFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode);
    
    public IVariableDeclarationNode[] GetVariableDeclarationNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey);
    
    public bool TryAddVariableDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDefinitionNode);
    
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
