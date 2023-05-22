using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Namespaces;

namespace Luthetus.Ide.ClassLib.FileTemplates;

public class FileTemplateParameter
{
    public FileTemplateParameter(
        string filename,
        NamespacePath parentDirectory,
        IEnvironmentProvider environmentProvider)
    {
        Filename = filename;
        ParentDirectory = parentDirectory;
        EnvironmentProvider = environmentProvider;
    }

    public string Filename { get; }
    public NamespacePath ParentDirectory { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }
}