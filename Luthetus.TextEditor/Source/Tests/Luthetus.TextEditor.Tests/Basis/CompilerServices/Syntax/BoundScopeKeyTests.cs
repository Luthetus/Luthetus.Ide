namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax;

public sealed record BoundScopeKeyTests(Guid Guid)
{
    public static readonly BoundScopeKey Empty = new BoundScopeKey(Guid.Empty);

    public static BoundScopeKey NewKey()
    {
        return new BoundScopeKey(Guid.NewGuid());
    }
}