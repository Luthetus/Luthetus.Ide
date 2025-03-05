using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

/// <summary>
/// Example usage: Every '.cs' file in a .NET Solution is an 'ICompilationUnit'
/// </summary>
public interface ICompilationUnit
{
	public ResourceUri ResourceUri { get; }
	public ISyntaxNode RootCodeBlockNode { get; }
	public List<TextEditorDiagnostic> __DiagnosticList { get; }
    public IReadOnlyList<TextEditorDiagnostic> DiagnosticList { get; }
}
