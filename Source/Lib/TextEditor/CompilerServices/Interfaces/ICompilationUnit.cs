using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

/// <summary>
/// Example usage: Every '.cs' file in a .NET Solution is an 'ICompilationUnit'
/// </summary>
public interface ICompilationUnit
{
	public CodeBlockNode RootCodeBlockNode { get; }
}
