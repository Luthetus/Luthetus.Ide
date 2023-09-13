namespace Luthetus.Ide.ClassLib.StateCase;

public record StateKey(Guid Guid)
{
    public static StateKey Empty { get; } = new(Guid.Empty);

    public static StateKey NewKey()
    {
        return new(Guid.NewGuid());
    }
}