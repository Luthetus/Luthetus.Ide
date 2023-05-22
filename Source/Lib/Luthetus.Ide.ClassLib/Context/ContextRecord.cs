namespace Luthetus.Ide.ClassLib.Context;

public record ContextRecord(
    ContextKey ContextKey,
    string DisplayNameFriendly,
    string ContextNameInternal);