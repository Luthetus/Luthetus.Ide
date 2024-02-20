using System.Collections.Immutable;

namespace Luthetus.Ide.Tests.Basis.FileSystems.Models;

public class FileTemplateProviderTests
{
    private List<IFileTemplate> _fileTemplatesList = new();

    /// <summary>
    /// The order of the entries in <see cref="_fileTemplatesList"/> is important
    /// as the .FirstOrDefault(x => ...true...) is used.
    /// </summary>
    public FileTemplateProvider()
    {
        _fileTemplatesList.Add(FileTemplateFacts.RazorCodebehind);
        _fileTemplatesList.Add(FileTemplateFacts.RazorMarkup);
        _fileTemplatesList.Add(FileTemplateFacts.CSharpClass);
    }

    public ImmutableArray<IFileTemplate> FileTemplatesList => _fileTemplatesList.ToImmutableArray();
}