using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.CompilerServices.DotNetSolution.Models.Project;

public class CSharpProjectNugetPackageReferences
{
    public CSharpProjectNugetPackageReferences(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }

    public NamespacePath CSharpProjectNamespacePath { get; }
}