using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorShowNewLines : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    public bool GlobalShowNewlines
    {
        get => TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.ShowNewlines;
        set => TextEditorService.OptionsApi.SetShowNewlines(value);
    }
    
    protected override void OnInitialized()
    {
    	TextEditorService.OptionsApi.TextEditorOptionsStateChanged += TextEditorOptionsStateWrapOnStateChanged;
    	base.OnInitialized();
    }
    
    private async void TextEditorOptionsStateWrapOnStateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	TextEditorService.OptionsApi.TextEditorOptionsStateChanged -= TextEditorOptionsStateWrapOnStateChanged;
    }
}