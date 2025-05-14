using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class TextEditorSettingsPreview : ComponentBase
{
    [Inject]
    private TextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string LabelElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string PreviewElementCssClassString { get; set; } = string.Empty;

    public static readonly Key<TextEditorViewModel> SettingsPreviewTextEditorViewModelKey = Key<TextEditorViewModel>.NewKey();

    private readonly ViewModelDisplayOptions _viewModelDisplayOptions = new()
    {
        HeaderComponentType = null,
        FooterComponentType = null,
        AfterOnKeyDownAsync = (_, _, _, _, _) => Task.CompletedTask,
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
        	TextEditorService.WorkerArbitrary.PostUnique(nameof(TextEditorSettingsPreview), async editContext =>
        	{
        		TextEditorService.ModelApi.RegisterTemplated(
        			editContext,
	                ExtensionNoPeriodFacts.TXT,
	                ResourceUriFacts.SettingsPreviewTextEditorResourceUri,
	                DateTime.UtcNow,
	                "Preview settings here",
	                "Settings Preview");
	
	            TextEditorService.ViewModelApi.Register(
	            	editContext,
	                SettingsPreviewTextEditorViewModelKey,
	                ResourceUriFacts.SettingsPreviewTextEditorResourceUri,
	                new Category(nameof(TextEditorSettingsPreview)));
	                
	            await InvokeAsync(StateHasChanged);
        	});
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}