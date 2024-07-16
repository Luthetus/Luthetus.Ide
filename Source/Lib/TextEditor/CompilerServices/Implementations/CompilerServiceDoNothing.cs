using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class CompilerServiceDoNothing : CompilerService
{
    public CompilerServiceDoNothing() : base(null)
    {
    }

    protected override void QueueParseRequest(ResourceUri resourceUri)
    {
		// Intentionally do nothing
        return;
    }
}
