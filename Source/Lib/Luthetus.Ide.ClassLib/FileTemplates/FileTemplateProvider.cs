using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.FileTemplates;

public class FileTemplateProvider : IFileTemplateProvider
{
    private List<IFileTemplate> _fileTemplates = new();

    /// <summary>
    /// The order of the entries in <see cref="_fileTemplates"/> is important
    /// as the .FirstOrDefault(x => ...true...) is used.
    /// </summary>
    public FileTemplateProvider()
    {

        _fileTemplates.Add(FileTemplateFacts.RazorCodebehind);
        _fileTemplates.Add(FileTemplateFacts.RazorMarkup);
        _fileTemplates.Add(FileTemplateFacts.CSharpClass);
    }

    public ImmutableArray<IFileTemplate> FileTemplates => _fileTemplates
        .ToImmutableArray();
}