namespace Luthetus.Ide.RazorLib.TerminalCase;

public record TerminalCommandKey(Guid Guid, string? DisplayName)
{
    public static TerminalCommandKey Empty { get; } = new(Guid.Empty, null);

    public static TerminalCommandKey NewKey(string? displayName = null)
    {
        return new(Guid.NewGuid(), displayName);
    }
}