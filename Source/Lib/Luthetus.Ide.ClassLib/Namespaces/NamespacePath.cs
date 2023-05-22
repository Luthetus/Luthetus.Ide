using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Namespaces;

public class NamespacePath
{
    public NamespacePath(string namespaceString, IAbsoluteFilePath absoluteFilePath)
    {
        Namespace = namespaceString;
        AbsoluteFilePath = absoluteFilePath;
    }

    public string Namespace { get; set; }
    public IAbsoluteFilePath AbsoluteFilePath { get; set; }
}