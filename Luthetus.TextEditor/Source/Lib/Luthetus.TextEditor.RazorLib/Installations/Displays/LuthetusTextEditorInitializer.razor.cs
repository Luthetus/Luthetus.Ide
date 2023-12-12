using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Finds.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Installations.Displays;

public partial class LuthetusTextEditorInitializer : ComponentBase
{
    [Inject]
    private LuthetusTextEditorOptions LuthetusTextEditorOptions { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IThemeService ThemeRecordsCollectionService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (LuthetusTextEditorOptions.CustomThemeRecordBag is not null)
        {
            foreach (var themeRecord in LuthetusTextEditorOptions.CustomThemeRecordBag)
            {
                Dispatcher.Dispatch(new ThemeState.RegisterAction(themeRecord));
            }
        }

        var initialThemeRecord = ThemeRecordsCollectionService.ThemeStateWrap.Value.ThemeBag.FirstOrDefault(
            x => x.Key == LuthetusTextEditorOptions.InitialThemeKey);

        if (initialThemeRecord is not null)
            Dispatcher.Dispatch(new TextEditorOptionsState.SetThemeAction(initialThemeRecord));

        foreach (var findProvider in LuthetusTextEditorOptions.FindProviderBag)
        {
            Dispatcher.Dispatch(new TextEditorFindProviderState.RegisterAction(findProvider));
        }

        await TextEditorService.Options.SetFromLocalStorageAsync();

        await base.OnAfterRenderAsync(firstRender);
    }
}