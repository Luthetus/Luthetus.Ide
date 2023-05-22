using Luthetus.Ide.ClassLib.Namespaces;

namespace Luthetus.Ide.ClassLib.DotNet.CSharp;

public class CSharpProjectToProjectReferences
{
    public CSharpProjectToProjectReferences(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }

    public NamespacePath CSharpProjectNamespacePath { get; }
}