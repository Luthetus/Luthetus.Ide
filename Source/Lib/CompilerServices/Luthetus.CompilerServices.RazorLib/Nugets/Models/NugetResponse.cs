namespace Luthetus.CompilerServices.RazorLib.Nugets.Models;

public record NugetResponse(
    int TotalHits,
    NugetPackageRecord[] Data);