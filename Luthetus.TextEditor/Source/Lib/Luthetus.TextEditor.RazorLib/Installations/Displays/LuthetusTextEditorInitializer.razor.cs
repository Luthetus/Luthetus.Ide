using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.SearchEngines.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;
using Microsoft.AspNetCore.Components;

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
	[Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
		if (firstRender)
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
	
	        foreach (var searchEngine in LuthetusTextEditorOptions.SearchEngineBag)
	        {
	            Dispatcher.Dispatch(new TextEditorSearchEngineState.RegisterAction(searchEngine));
	        }

			Dispatcher.Dispatch(new TextEditorSearchEngineState.RegisterAction(
				new SearchEngineFileSystem(FileSystemProvider)));
	
	        await TextEditorService.OptionsApi.SetFromLocalStorageAsync();
		}
	
		await base.OnAfterRenderAsync(firstRender);
    }
}