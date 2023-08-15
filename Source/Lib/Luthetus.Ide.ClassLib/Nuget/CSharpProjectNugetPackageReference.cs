namespace Luthetus.Ide.ClassLib.Nuget;

public record CSharpProjectNugetPackageReference(
    string CSharpProjectAbsoluteFilePathString,
    LightWeightNugetPackageRecord LightWeightNugetPackageRecord);