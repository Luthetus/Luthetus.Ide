namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;

public sealed record BoundScopeKey(Guid Guid)
{
    public static readonly BoundScopeKey Empty = new BoundScopeKey(Guid.Empty);

    public static BoundScopeKey NewBoundScopeKey()
    {
        return new BoundScopeKey(Guid.NewGuid());
    }
}