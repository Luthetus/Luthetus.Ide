using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Displays;

public partial class SolutionEditorDisplay : ComponentBase, IDisposable
{
    [Inject]
    private ICompilerServiceRegistry InterfaceCompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<DotNetSolutionModel> DotNetSolutionModelKey { get; set; }
    [Parameter, EditorRequired]
    public ResourceUri DotNetSolutionResourceUri { get; set; } = null!;

    private CompilerServiceRegistry _compilerServiceRegistry = null!;

    private readonly HashSet<ResourceUri> _seenDotNetSolutionResourceUris = new();

    protected override async Task OnParametersSetAsync()
    {
        var localDotNetSolutionResourceUri = DotNetSolutionResourceUri;

        if (_seenDotNetSolutionResourceUris.Add(localDotNetSolutionResourceUri))
        {
            _compilerServiceRegistry = (CompilerServiceRegistry)InterfaceCompilerServiceRegistry;

            var content = await FileSystemProvider.File.ReadAllTextAsync(localDotNetSolutionResourceUri.Value);

            TextEditorService.Model.RegisterTemplated(
                DecorationMapperRegistry,
                InterfaceCompilerServiceRegistry,
                ExtensionNoPeriodFacts.DOT_NET_SOLUTION,
                localDotNetSolutionResourceUri,
                DateTime.UtcNow,
                content);

            _compilerServiceRegistry.DotNetSolutionCompilerService.RegisterResource(localDotNetSolutionResourceUri);
        }

        await base.OnParametersSetAsync();
    }

    protected override void OnInitialized()
    {
        _compilerServiceRegistry = (CompilerServiceRegistry)InterfaceCompilerServiceRegistry;

        _compilerServiceRegistry.DotNetSolutionCompilerService.ResourceParsed += DotNetSolutionCompilerService_ModelParsed;

        base.OnInitialized();
    }

    private async void DotNetSolutionCompilerService_ModelParsed()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _compilerServiceRegistry.DotNetSolutionCompilerService.ResourceParsed -= DotNetSolutionCompilerService_ModelParsed;
    }
}