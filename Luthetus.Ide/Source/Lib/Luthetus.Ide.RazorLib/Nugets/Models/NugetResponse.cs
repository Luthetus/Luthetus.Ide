namespace Luthetus.Ide.RazorLib.Nugets.Models;

public record NugetResponse(
    int TotalHits,
    NugetPackageRecord[] Data);