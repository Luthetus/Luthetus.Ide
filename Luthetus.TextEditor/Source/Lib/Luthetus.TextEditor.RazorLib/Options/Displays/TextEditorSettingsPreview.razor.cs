using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class TextEditorSettingsPreview : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string LabelElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string PreviewElementCssClassString { get; set; } = string.Empty;

    public static readonly ResourceUri SettingsPreviewTextEditorResourceUri = new ResourceUri("__LUTHETUS_SETTINGS_PREVIEW__");
    public static readonly Key<TextEditorViewModel> SettingsPreviewTextEditorViewModelKey = Key<TextEditorViewModel>.NewKey();

    private readonly TextEditorViewModelDisplayOptions _viewModelDisplayOptions = new()
    {
        WrapperStyleCssString = "height: var(--luth_te_text-editor-content-min-height);",
        TextEditorStyleCssString = "height: 100%;",
        IncludeHeaderHelperComponent = false,
        IncludeFooterHelperComponent = false,
        AfterOnKeyDownAsyncFactory = (_, _, _, _) => { return editContext => Task.CompletedTask; }
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            TextEditorService.ModelApi.RegisterTemplated(
                ExtensionNoPeriodFacts.TXT,
                SettingsPreviewTextEditorResourceUri,
                DateTime.UtcNow,
                "Preview settings here",
                "Settings Preview");

            TextEditorService.ViewModelApi.Register(
                SettingsPreviewTextEditorViewModelKey,
                SettingsPreviewTextEditorResourceUri,
                new TextEditorCategory(nameof(TextEditorSettingsPreview)));

            await InvokeAsync(StateHasChanged);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}