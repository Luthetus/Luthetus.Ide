using Luthetus.Ide.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FileSystems.Displays;

public partial class FileTemplatesDisplay : ComponentBase
{
    [Inject]
    private IFileTemplateProvider FileTemplateProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public string FileName { get; set; } = null!;

    private ImmutableArray<FileTemplatesFormWrapper> _fileTemplatesFormWrappersBag = ImmutableArray<FileTemplatesFormWrapper>.Empty;
    private ImmutableArray<FileTemplatesFormWrapper> _relatedMatchWrappersBag = ImmutableArray<FileTemplatesFormWrapper>.Empty;
    private FileTemplatesFormWrapper? _exactMatchWrapper;
    public IFileTemplate? ExactMatchFileTemplate => _exactMatchWrapper?.FileTemplate;

    public ImmutableArray<IFileTemplate>? RelatedMatchFileTemplates => _relatedMatchWrappersBag
        .Where(x => x.IsChecked)
        .Select(x => x.FileTemplate)
        .ToImmutableArray();

    protected override void OnInitialized()
    {
        _fileTemplatesFormWrappersBag = FileTemplateProvider.FileTemplatesBag
            .Select(x => new FileTemplatesFormWrapper(x, true))
            .ToImmutableArray();

        base.OnInitialized();
    }

    private class FileTemplatesFormWrapper
    {
        public FileTemplatesFormWrapper(IFileTemplate fileTemplate, bool isChecked)
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
            _relatedMatchWrappersBag = ImmutableArray<FileTemplatesFormWrapper>.Empty;
            return;
        }

        var relatedMatches = _exactMatchWrapper.FileTemplate.RelatedFileTemplatesFunc.Invoke(FileName);

        _relatedMatchWrappersBag = relatedMatches
            .Select(rel => _fileTemplatesFormWrappersBag.First(wrap => rel.Id == wrap.FileTemplate.Id))
            .ToImmutableArray();
    }
}