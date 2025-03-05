using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ICompilerServiceResource
{
    public ResourceUri ResourceUri { get; }
    public ICompilerService CompilerService { get; }
    public ICompilationUnit CompilationUnit { get; set; }
}
