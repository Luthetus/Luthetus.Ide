using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.FileTemplatesCase;

public interface IFileTemplateProvider
{
    public ImmutableArray<IFileTemplate> FileTemplates { get; }
}