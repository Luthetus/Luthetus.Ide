using Luthetus.Ide.ClassLib.Namespaces;

namespace Luthetus.Ide.ClassLib.DotNet.CSharp;

public class CSharpProjectNugetPackageReferences
{
    public CSharpProjectNugetPackageReferences(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }

    public NamespacePath CSharpProjectNamespacePath { get; }
}