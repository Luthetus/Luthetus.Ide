namespace Luthetus.Ide.ClassLib.Views;

public record ViewKey(Guid Guid, string? DisplayName)
{
    public static ViewKey Empty { get; } = new(Guid.Empty, null);

    public static ViewKey NewViewKey(string? displayName = null)
    {
        return new(Guid.NewGuid(), displayName);
    }
}