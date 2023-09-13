namespace Luthetus.Ide.ClassLib.TerminalCase;

public record TerminalSessionKey(Guid Guid, string? DisplayName)
{
    public static TerminalSessionKey Empty { get; } = new(Guid.Empty, null);

    public static TerminalSessionKey NewKey(string? displayName = null)
    {
        return new(Guid.NewGuid(), displayName);
    }
}