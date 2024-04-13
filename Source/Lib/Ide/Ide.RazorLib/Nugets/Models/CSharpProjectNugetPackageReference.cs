namespace Luthetus.Ide.RazorLib.Nugets.Models;

public record CSharpProjectNugetPackageReference(
    string CSharpProjectAbsolutePathString,
    LightWeightNugetPackageRecord LightWeightNugetPackageRecord);