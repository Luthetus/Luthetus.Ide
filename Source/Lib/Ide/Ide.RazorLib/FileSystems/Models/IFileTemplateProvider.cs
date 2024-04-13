using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public interface IFileTemplateProvider
{
    public ImmutableArray<IFileTemplate> FileTemplatesList { get; }
}