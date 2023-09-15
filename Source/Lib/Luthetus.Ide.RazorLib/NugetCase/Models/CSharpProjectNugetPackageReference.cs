namespace Luthetus.Ide.RazorLib.NugetCase.Models;

public record CSharpProjectNugetPackageReference(
    string CSharpProjectAbsolutePathString,
    LightWeightNugetPackageRecord LightWeightNugetPackageRecord);