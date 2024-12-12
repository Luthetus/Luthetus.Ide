using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ICompilationUnit
{
	public CodeBlockNode RootCodeBlockNode { get; }
}
