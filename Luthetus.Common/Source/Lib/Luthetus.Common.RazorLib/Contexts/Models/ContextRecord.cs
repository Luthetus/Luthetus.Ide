using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public record ContextRecord(
    Key<ContextRecord> ContextKey,
    string DisplayNameFriendly,
    string ContextNameInternal,
    Keymap Keymap)
{
    public string ContextElementId => $"luth_ide_context-{ContextKey.Guid}";
}
