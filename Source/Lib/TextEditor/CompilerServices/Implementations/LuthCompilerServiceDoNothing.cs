using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class LuthCompilerServiceDoNothing : LuthCompilerService
{
    public LuthCompilerServiceDoNothing() : base(null)
    {
    }

    protected override void QueueParseRequest(ResourceUri resourceUri)
    {
        return;
    }
}
