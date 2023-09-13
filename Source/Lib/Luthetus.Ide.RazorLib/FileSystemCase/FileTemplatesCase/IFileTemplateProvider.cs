using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FileSystemCase.FileTemplatesCase;

public interface IFileTemplateProvider
{
    public ImmutableArray<IFileTemplate> FileTemplates { get; }
}