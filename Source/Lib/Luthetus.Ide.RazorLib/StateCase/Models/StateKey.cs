namespace Luthetus.Ide.RazorLib.StateCase.Models;

public record StateKey(Guid Guid)
{
    public static StateKey Empty { get; } = new(Guid.Empty);

    public static StateKey NewKey()
    {
        return new(Guid.NewGuid());
    }
}