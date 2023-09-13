namespace Luthetus.Ide.ClassLib.NugetCase;

public record NugetResponse(
    int TotalHits,
    NugetPackageRecord[] Data);