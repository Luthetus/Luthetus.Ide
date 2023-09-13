namespace Luthetus.Ide.RazorLib.ViewsCase;

public record ViewKey(Guid Guid, string? DisplayName)
{
    public static ViewKey Empty { get; } = new(Guid.Empty, null);

    public static ViewKey NewKey(string? displayName = null)
    {
        return new(Guid.NewGuid(), displayName);
    }
}