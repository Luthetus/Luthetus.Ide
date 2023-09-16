using Luthetus.Common.RazorLib.KeyCase;
using Luthetus.Ide.RazorLib.KeymapCase.Models;

namespace Luthetus.Ide.RazorLib.ContextCase.Models;

public record ContextRecord(
    Key<ContextRecord> ContextKey,
    string DisplayNameFriendly,
    string ContextNameInternal,
    Keymap Keymap)
{
    public string ContextElementId => $"luth_ide_context-{ContextKey.Guid}";
}