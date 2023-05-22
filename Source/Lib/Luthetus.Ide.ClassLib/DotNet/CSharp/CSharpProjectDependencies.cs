using Luthetus.Ide.ClassLib.Namespaces;

namespace Luthetus.Ide.ClassLib.DotNet.CSharp;

public class CSharpProjectDependencies
{
    public CSharpProjectDependencies(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }

    public NamespacePath CSharpProjectNamespacePath { get; }
}