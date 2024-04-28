using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;

public class CSharpProjectDependencies
{
    public CSharpProjectDependencies(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }

    public NamespacePath CSharpProjectNamespacePath { get; }
}