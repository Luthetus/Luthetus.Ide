using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class LuthCompilerServiceDoNothing : LuthCompilerService
{
    public LuthCompilerServiceDoNothing() : base(null)
    {
    }

    protected override Task QueueParseRequest(ResourceUri resourceUri)
    {
        return Task.CompletedTask;
    }
}
