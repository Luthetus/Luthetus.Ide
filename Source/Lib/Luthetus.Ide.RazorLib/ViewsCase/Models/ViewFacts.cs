using Luthetus.Common.RazorLib.KeyCase;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.ViewsCase.Models;

public static class ViewFacts
{
    public static readonly View TerminalsView = new(
        Key<View>.NewKey(),
        ViewKind.Terminals);

    public static readonly View NugetPackageManagerView = new(
        Key<View>.NewKey(),
        ViewKind.NugetPackageManager);

    public static readonly View GitView = new(
        Key<View>.NewKey(),
        ViewKind.GitDisplay);

    public static readonly ImmutableArray<View> Views = new[]
    {
    TerminalsView,
    NugetPackageManagerView,
    GitView
}.ToImmutableArray();
}