using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;

public class CSharpProjectToProjectReference
{
    public CSharpProjectToProjectReference(
        NamespacePath modifyProjectNamespacePath,
        IAbsolutePath referenceProjectAbsolutePath)
    {
        ModifyProjectNamespacePath = modifyProjectNamespacePath;
        ReferenceProjectAbsolutePath = referenceProjectAbsolutePath;
    }

    /// <summary>The <see cref="ModifyProjectNamespacePath"/> is the <see cref="NamespacePath"/> of the Project which will have its XML data modified.</summary>
    public NamespacePath ModifyProjectNamespacePath { get; }
    public IAbsolutePath ReferenceProjectAbsolutePath { get; }
}