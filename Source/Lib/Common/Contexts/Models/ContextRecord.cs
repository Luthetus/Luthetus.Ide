using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public record struct ContextRecord(
    Key<ContextRecord> ContextKey,
    string DisplayNameFriendly,
    string ContextNameInternal,
    IKeymap Keymap)
{
    public string ContextElementId => $"luth_ide_context-{ContextKey.Guid}";
}
