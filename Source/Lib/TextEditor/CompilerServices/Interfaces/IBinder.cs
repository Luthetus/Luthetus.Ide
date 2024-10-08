using System.Collections.Immutable;
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
    public IBoundScope? GetBoundScope(TextEditorTextSpan textSpan);
    public IBinderSession ConstructBinderSession(ResourceUri resourceUri);
    public void ClearStateByResourceUri(ResourceUri resourceUri);
    public void AddNamespaceToCurrentScope(string namespaceString, IParserModel model);
    public void BindFunctionOptionalArgument(FunctionArgumentEntryNode functionArgumentEntryNode, IParserModel model);
    public void BindVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode, IParserModel model);
}
