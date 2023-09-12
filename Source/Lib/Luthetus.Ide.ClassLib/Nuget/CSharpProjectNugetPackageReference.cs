namespace Luthetus.Ide.ClassLib.Nuget;

public record CSharpProjectNugetPackageReference(
    string CSharpProjectAbsolutePathString,
    LightWeightNugetPackageRecord LightWeightNugetPackageRecord);