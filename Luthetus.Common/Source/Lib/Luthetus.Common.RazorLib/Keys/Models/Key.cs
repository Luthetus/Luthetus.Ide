namespace Luthetus.Common.RazorLib.Keys.Models;

public record struct Key<T>(Guid Guid)
{
    public static readonly Key<T> Empty = new Key<T>(Guid.Empty);

    public static Key<T> NewKey()
    {
        return new(Guid.NewGuid());
    }
}