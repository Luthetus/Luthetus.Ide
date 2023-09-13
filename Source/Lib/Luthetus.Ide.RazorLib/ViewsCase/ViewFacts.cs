using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.ViewsCase;

public static class ViewFacts
{
    public static readonly View TerminalsView = new(
        ViewKey.NewKey("Terminals"),
        ViewKind.Terminals);

    public static readonly View NugetPackageManagerView = new(
        ViewKey.NewKey("NuGet"),
        ViewKind.NugetPackageManager);

    public static readonly View GitView = new(
        ViewKey.NewKey("Git"),
        ViewKind.GitDisplay);

    public static readonly ImmutableArray<View> Views = new[]
    {
    TerminalsView,
    NugetPackageManagerView,
    GitView
}.ToImmutableArray();
}