namespace Luthetus.CompilerServices.RazorLib.Nugets.Models;

public record CSharpProjectNugetPackageReference(
    string CSharpProjectAbsolutePathString,
    LightWeightNugetPackageRecord LightWeightNugetPackageRecord);