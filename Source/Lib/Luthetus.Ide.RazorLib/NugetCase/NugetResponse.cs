namespace Luthetus.Ide.RazorLib.NugetCase;

public record NugetResponse(
    int TotalHits,
    NugetPackageRecord[] Data);