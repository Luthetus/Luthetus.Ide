using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorTheme : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi LuthetusCommonApi { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;
	
	protected override void OnInitialized()
    {
    	TextEditorService.OptionsApi.TextEditorOptionsStateChanged += TextEditorOptionsStateWrapOnStateChanged;
    	base.OnInitialized();
    }
    
    private void SelectedThemeChanged(ChangeEventArgs changeEventArgs)
    {
        var themeList = LuthetusCommonApi.ThemeApi.GetThemeState().ThemeList;

        var chosenThemeKeyGuidString = changeEventArgs.Value?.ToString() ?? string.Empty;

        if (Guid.TryParse(chosenThemeKeyGuidString,
                out var chosenThemeKeyGuid))
        {
            var chosenThemeKey = new Key<ThemeRecord>(chosenThemeKeyGuid);
            var foundTheme = themeList.FirstOrDefault(x => x.Key == chosenThemeKey);

            if (foundTheme is not null)
                TextEditorService.OptionsApi.SetTheme(foundTheme);
        }
        else
        {
            TextEditorService.OptionsApi.SetTheme(ThemeFacts.VisualStudioDarkThemeClone);
        }
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