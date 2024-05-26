using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.RazorLib.Namespaces.Models;

/// <summary>
/// TODO: Move this type somehere else. This type currently exists in 'Luthetus.Common.csproj'...
/// ...because the 'Luthetus.CompilerServices.Lang.DotNetSolution.csproj' needed to reference the type.<br/>
/// |<br/>
/// And with it having originally been in 'Luthetus.Ide.csproj', this meant a circular
/// reference and it had to be moved here for now.
/// </summary>
public class NamespacePath
{
    public NamespacePath(string namespaceString, IAbsolutePath absolutePath)
    {
        Namespace = namespaceString;
        AbsolutePath = absolutePath;
    }

    public string Namespace { get; set; }
    public IAbsolutePath AbsolutePath { get; set; }
}