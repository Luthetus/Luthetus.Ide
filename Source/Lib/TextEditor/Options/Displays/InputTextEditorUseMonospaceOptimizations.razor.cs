using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorUseMonospaceOptimizations : FluxorComponent
{
    [Inject]
    private IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

   [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    public bool UseMonospaceOptimizations
    {
        get => TextEditorOptionsStateWrap.Value.Options.UseMonospaceOptimizations;
        set => TextEditorService.OptionsApi.SetUseMonospaceOptimizations(value);
    }
}