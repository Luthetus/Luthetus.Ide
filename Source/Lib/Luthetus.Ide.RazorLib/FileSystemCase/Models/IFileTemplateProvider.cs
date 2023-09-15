using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FileSystemCase.Models;

public interface IFileTemplateProvider
{
    public ImmutableArray<IFileTemplate> FileTemplates { get; }
}