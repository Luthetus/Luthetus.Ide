using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalResource : ICompilerServiceResource
{
    /// <summary>
    /// The <see cref="ArgumentsTextSpan"/> and <see cref="TargetFilePathTextSpan"/> are currently
    /// mutable state. If these properties are re-written, then this lock is not needed.<br/><br/>
    /// 
    /// This lock is intended to be used only to read or write to <see cref="ArgumentsTextSpan"/> or <see cref="TargetFilePathTextSpan"/>
    /// and preferably, one would in bulk, read or write both properties from the same lock() { ... }
    /// </summary>
    public readonly object UnsafeStateLock = new();

    public TerminalResource(
    	ResourceUri resourceUri,
    	TerminalCompilerService terminalCompilerService)
    {
    	ResourceUri = resourceUri;
    	CompilerService = terminalCompilerService;
    }

	public ResourceUri ResourceUri { get; }
	public ICompilerService CompilerService { get; }
	public TerminalCompilationUnit CompilationUnit { get; } = new();
	
	ICompilationUnit ICompilerServiceResource.CompilationUnit { get => CompilationUnit; set => _ = value; }
}