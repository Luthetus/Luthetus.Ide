using Luthetus.Ide.ClassLib.KeymapCase;

namespace Luthetus.Ide.ClassLib.Context;

public record ContextRecord(
    ContextKey ContextKey,
    string DisplayNameFriendly,
    string ContextNameInternal,
    Keymap Keymap)
{
    public string ContextElementId => $"luth_ide_context-{ContextKey.Guid}";
}