using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorTheme : FluxorComponent
{
    [Inject]
    private IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<ThemeState> ThemeStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    private void SelectedThemeChanged(ChangeEventArgs changeEventArgs)
    {
        var themeList = ThemeStateWrap.Value.ThemeList;

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
}