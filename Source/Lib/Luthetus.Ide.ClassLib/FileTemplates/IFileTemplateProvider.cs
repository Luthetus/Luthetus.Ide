using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.FileTemplates;

public interface IFileTemplateProvider
{
    public ImmutableArray<IFileTemplate> FileTemplates { get; }
}