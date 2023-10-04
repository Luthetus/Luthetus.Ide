using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;

public class CSharpProjectNugetPackageReferences
{
    public CSharpProjectNugetPackageReferences(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }

    public NamespacePath CSharpProjectNamespacePath { get; }
}