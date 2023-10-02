using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public class FileTemplateProvider : IFileTemplateProvider
{
    private List<IFileTemplate> _fileTemplatesBag = new();

    /// <summary>
    /// The order of the entries in <see cref="_fileTemplatesBag"/> is important
    /// as the .FirstOrDefault(x => ...true...) is used.
    /// </summary>
    public FileTemplateProvider()
    {
        _fileTemplatesBag.Add(FileTemplateFacts.RazorCodebehind);
        _fileTemplatesBag.Add(FileTemplateFacts.RazorMarkup);
        _fileTemplatesBag.Add(FileTemplateFacts.CSharpClass);
    }

    public ImmutableArray<IFileTemplate> FileTemplatesBag => _fileTemplatesBag.ToImmutableArray();
}