using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;

public class CSharpProjectDependencies
{
    public CSharpProjectDependencies(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }

    public NamespacePath CSharpProjectNamespacePath { get; }
}