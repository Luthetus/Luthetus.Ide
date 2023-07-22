using Luthetus.Common.RazorLib.Namespaces;

namespace Luthetus.Ide.ClassLib.FileTemplates;

public class FileTemplateResult
{
    public FileTemplateResult(
        NamespacePath fileNamespacePath,
        string contents)
    {
        FileNamespacePath = fileNamespacePath;
        Contents = contents;
    }

    public NamespacePath FileNamespacePath { get; }
    public string Contents { get; }
}