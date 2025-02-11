using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorUseMonospaceOptimizations : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    public bool UseMonospaceOptimizations
    {
        get => TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.UseMonospaceOptimizations;
        set => TextEditorService.OptionsApi.SetUseMonospaceOptimizations(value);
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