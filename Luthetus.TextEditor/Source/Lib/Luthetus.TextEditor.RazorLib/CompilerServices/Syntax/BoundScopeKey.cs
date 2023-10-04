namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public sealed record BoundScopeKey(Guid Guid)
{
    public static readonly BoundScopeKey Empty = new BoundScopeKey(Guid.Empty);

    public static BoundScopeKey NewKey()
    {
        return new BoundScopeKey(Guid.NewGuid());
    }
}