using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;

namespace Luthetus.Extensions.CompilerServices;

public interface IExtendedCompilationUnit : ICompilationUnit
{
	public IReadOnlyList<Symbol> SymbolList { get; }
	public Dictionary<ScopeKeyAndIdentifierText, TypeDefinitionNode> ScopeTypeDefinitionMap { get; }
	public Dictionary<ScopeKeyAndIdentifierText, FunctionDefinitionNode> ScopeFunctionDefinitionMap { get; }
}
