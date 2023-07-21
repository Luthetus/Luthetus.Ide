namespace Luthetus.Ide.ClassLib.Views;

public static class ViewFacts
{
    public static readonly View TerminalsView = new(
        ViewKey.NewViewKey("Terminals"),
        ViewKind.Terminals);

    public static readonly View NugetPackageManagerView = new(
        ViewKey.NewViewKey("NuGet"),
        ViewKind.NugetPackageManager);

    public static readonly View GitView = new(
        ViewKey.NewViewKey("Git"),
        ViewKind.GitDisplay);

    public static readonly ImmutableArray<View> Views = new[]
    {
    TerminalsView,
    NugetPackageManagerView,
    GitView
}.ToImmutableArray();
}