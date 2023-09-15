namespace Luthetus.Ide.RazorLib.NugetCase.Models;

public record NugetResponse(
    int TotalHits,
    NugetPackageRecord[] Data);