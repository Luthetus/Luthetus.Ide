using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.RewriteForImmutability;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Displays;

public partial class SolutionEditorDisplay : ComponentBase, IDisposable
{
    [Inject]
    private DotNetSolutionCompilerService DotNetSolutionCompilerService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<DotNetSolutionModel> DotNetSolutionModelKey { get; set; }
    [Parameter, EditorRequired]
    public ResourceUri DotNetSolutionResourceUri { get; set; } = null!;

    protected override void OnParametersSet()
    {
        //TextEditorService.Model.RegisterTemplated(
        //    Key<TextEditorModel>.NewKey(),
        //    WellKnownModelKind.DotNetSolution,
        //    DotNetSolutionResourceUri,
        //    DateTime.UtcNow,
        //    ".sln",
        //    );

        //var model = TextEditorService.Model.FindOrDefaultByResourceUri(DotNetSolutionResourceUri);

        //if (model is not null)
        //    DotNetSolutionCompilerService.RegisterModel(model);

        base.OnParametersSet();
    }

    protected override void OnInitialized()
    {
        DotNetSolutionCompilerService.ResourceParsed += DotNetSolutionCompilerService_ModelParsed;
        base.OnInitialized();
    }

    private async void DotNetSolutionCompilerService_ModelParsed()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        DotNetSolutionCompilerService.ResourceParsed -= DotNetSolutionCompilerService_ModelParsed;
    }
}