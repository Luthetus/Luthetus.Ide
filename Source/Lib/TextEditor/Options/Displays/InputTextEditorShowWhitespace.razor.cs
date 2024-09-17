using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorShowWhitespace : FluxorComponent
{
    [Inject]
    private IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    public bool GlobalShowWhitespace
    {
        get => TextEditorOptionsStateWrap.Value.Options.ShowWhitespace;
        set => TextEditorService.OptionsApi.SetShowWhitespace(value);
    }
}