namespace Luthetus.Ide.ClassLib.State;

public record StateKey(Guid Guid)
{
    public static StateKey Empty { get; } = new(Guid.Empty);

    public static StateKey NewStateKey()
    {
        return new(Guid.NewGuid());
    }
}