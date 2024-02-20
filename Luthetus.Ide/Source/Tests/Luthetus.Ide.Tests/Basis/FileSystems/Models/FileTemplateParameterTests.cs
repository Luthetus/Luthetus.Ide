using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.Ide.Tests.Basis.FileSystems.Models;

public class FileTemplateParameterTests
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