using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public interface ICompilerServiceResource
{
	public ResourceUri ResourceUri { get; }
	public ICompilerService CompilerService { get; }
	public ICompilationUnit CompilationUnit { get; set; }
}
