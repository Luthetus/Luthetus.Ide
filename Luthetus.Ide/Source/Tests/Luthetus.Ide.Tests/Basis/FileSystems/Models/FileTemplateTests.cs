using System.Collections.Immutable;

namespace Luthetus.Ide.Tests.Basis.FileSystems.Models;

public class FileTemplateTests
{
    public FileTemplate(
        string displayName,
        string codeName,
        FileTemplateKind fileTemplateKind,
        Func<string, bool> isExactTemplate,
        Func<string, ImmutableArray<IFileTemplate>> relatedFileTemplatesFunc,
        bool initialCheckedStateWhenIsRelatedFile,
        Func<FileTemplateParameter, FileTemplateResult> constructFileContents)
    {
        DisplayName = displayName;
        CodeName = codeName;
        FileTemplateKind = fileTemplateKind;
        IsExactTemplate = isExactTemplate;
        RelatedFileTemplatesFunc = relatedFileTemplatesFunc;
        InitialCheckedStateWhenIsRelatedFile = initialCheckedStateWhenIsRelatedFile;
        ConstructFileContents = constructFileContents;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public string DisplayName { get; }
    public string CodeName { get; }
    public FileTemplateKind FileTemplateKind { get; }
    public Func<string, bool> IsExactTemplate { get; }
    public Func<string, ImmutableArray<IFileTemplate>> RelatedFileTemplatesFunc { get; }
    public bool InitialCheckedStateWhenIsRelatedFile { get; }
    public Func<FileTemplateParameter, FileTemplateResult> ConstructFileContents { get; }
}