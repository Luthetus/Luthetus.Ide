using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.Ide.Tests.Basis.FileSystems.Models;

public class FileTemplateResultTests
{
    public FileTemplateResult(NamespacePath fileNamespacePath, string contents)
    {
        FileNamespacePath = fileNamespacePath;
        Contents = contents;
    }

    public NamespacePath FileNamespacePath { get; }
    public string Contents { get; }
}