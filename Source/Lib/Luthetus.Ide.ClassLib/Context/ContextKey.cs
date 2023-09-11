namespace Luthetus.Ide.ClassLib.Context;

public record ContextKey(Guid Guid)
{
    public static readonly ContextKey Empty = new ContextKey(Guid.Empty);

    public static ContextKey NewKey()
    {
        return new(Guid.NewGuid());
    }
}