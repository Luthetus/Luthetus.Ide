namespace Luthetus.Ide.ClassLib.Nuget;

public record NugetResponse(
    int TotalHits,
    NugetPackageRecord[] Data);