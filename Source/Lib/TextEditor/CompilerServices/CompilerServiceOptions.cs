using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class CompilerServiceOptions
{
	public Func<ResourceUri, ICompilerServiceResource>? RegisterResourceFunc { get; init; }
}
