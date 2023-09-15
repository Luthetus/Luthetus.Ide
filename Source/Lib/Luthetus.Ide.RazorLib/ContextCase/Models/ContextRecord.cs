namespace Luthetus.Ide.RazorLib.ContextCase.Models;

namespace Luthetus.Ide.RazorLib.ContextCase;

public record ContextRecord(
    ContextKey ContextKey,
    string DisplayNameFriendly,
    string ContextNameInternal,
    Keymap Keymap)
{
    public string ContextElementId => $"luth_ide_context-{ContextKey.Guid}";
}