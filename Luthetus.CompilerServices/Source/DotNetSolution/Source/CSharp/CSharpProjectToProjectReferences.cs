using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;

public class CSharpProjectToProjectReferences
{
    public CSharpProjectToProjectReferences(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }

    public NamespacePath CSharpProjectNamespacePath { get; }
}