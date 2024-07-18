using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.CompilerServices.DotNetSolution.Models.Project;

public class CSharpProjectToProjectReferences
{
    public CSharpProjectToProjectReferences(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }

    public NamespacePath CSharpProjectNamespacePath { get; }
}