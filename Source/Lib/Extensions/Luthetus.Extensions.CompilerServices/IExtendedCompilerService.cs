using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.Extensions.CompilerServices;

public interface IExtendedCompilerService : ICompilerService
{
	public IReadOnlyDictionary<string, List<ResourceUri>> ReferenceMap { get; }

	public ISyntaxNode? GetSyntaxNode(int positionIndex, ResourceUri resourceUri, ICompilerServiceResource? compilerServiceResource);
	public ISyntaxNode? GetDefinitionNode(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource, Symbol? symbol = null);
    public TextEditorTextSpan? GetDefinitionTextSpan(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource);
    public Scope GetScopeByPositionIndex(ResourceUri resourceUri, int positionIndex);    
}
