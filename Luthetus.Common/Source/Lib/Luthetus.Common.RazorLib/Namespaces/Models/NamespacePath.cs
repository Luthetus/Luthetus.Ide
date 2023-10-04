using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.RazorLib.Namespaces.Models;

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