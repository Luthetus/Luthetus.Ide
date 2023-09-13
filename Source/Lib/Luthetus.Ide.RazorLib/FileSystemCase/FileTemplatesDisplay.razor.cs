using Luthetus.Ide.RazorLib.FileSystemCase.FileTemplatesCase;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FileSystemCase;

public partial class FileTemplatesDisplay : ComponentBase
{
    [Inject]
    private IFileTemplateProvider FileTemplateProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public string FileName { get; set; } = null!;

    private ImmutableArray<FileTemplatesFormWrapper> _fileTemplatesFormWrappers =
        ImmutableArray<FileTemplatesFormWrapper>.Empty;

    private FileTemplatesFormWrapper? _exactMatchWrapper;
    private ImmutableArray<FileTemplatesFormWrapper> _relatedMatchWrappers =
        ImmutableArray<FileTemplatesFormWrapper>.Empty;

    public IFileTemplate? ExactMatchFileTemplate => _exactMatchWrapper?.FileTemplate;
    public ImmutableArray<IFileTemplate>? RelatedMatchFileTemplates => _relatedMatchWrappers
        .Where(x => x.IsChecked)
        .Select(x => x.FileTemplate)
        .ToImmutableArray();

    protected override void OnInitialized()
    {
        _fileTemplatesFormWrappers = FileTemplateProvider.FileTemplates
            .Select(x => new FileTemplatesFormWrapper(x, true))
            .ToImmutableArray();

        base.OnInitialized();
    }

    private class FileTemplatesFormWrapper
    {
        public FileTemplatesFormWrapper(
            IFileTemplate fileTemplate,
            bool isChecked)
        {
            FileTemplate = fileTemplate;
            IsChecked = isChecked;
        }

        public IFileTemplate FileTemplate { get; }
        public bool IsChecked { get; set; }
    }

    private void GetRelatedFileTemplates()
    {
        if (_exactMatchWrapper is null)
        {
            _relatedMatchWrappers = ImmutableArray<FileTemplatesFormWrapper>.Empty;
            return;
        }

        var relatedMatches = _exactMatchWrapper
            .FileTemplate.RelatedFileTemplatesFunc.Invoke(FileName);

        _relatedMatchWrappers = relatedMatches.Select(rel =>
            _fileTemplatesFormWrappers.First(wrap =>
                rel.Id == wrap.FileTemplate.Id))
            .ToImmutableArray();
    }
}