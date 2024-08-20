using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.Widgets.States;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Installations.Displays;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: when one first starts interacting with this project,
/// 	this type might be one of the first types they interact with. So, the redundancy of namespace
/// 	and type containing 'Luthetus' feels reasonable here.
/// </remarks>
public partial class LuthetusTextEditorInitializer : ComponentBase
{
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IThemeService ThemeRecordsCollectionService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    private IState<TextEditorFindAllState> TextEditorFindAllStateWrap { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    
    public static Key<ContextSwitchGroup> ContextSwitchGroupKey { get; } = Key<ContextSwitchGroup>.NewKey();
    
    protected override void OnInitialized()
    {
    	BackgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            nameof(LuthetusCommonInitializer),
            async () =>
            {
                if (TextEditorConfig.CustomThemeRecordList is not null)
                {
                    foreach (var themeRecord in TextEditorConfig.CustomThemeRecordList)
                    {
                        Dispatcher.Dispatch(new ThemeState.RegisterAction(themeRecord));
                    }
                }

                var initialThemeRecord = ThemeRecordsCollectionService.ThemeStateWrap.Value.ThemeList.FirstOrDefault(
                    x => x.Key == TextEditorConfig.InitialThemeKey);

                if (initialThemeRecord is not null)
                    Dispatcher.Dispatch(new TextEditorOptionsState.SetThemeAction(initialThemeRecord));

                await TextEditorService.OptionsApi.SetFromLocalStorageAsync().ConfigureAwait(false);
                
                Dispatcher.Dispatch(new ContextSwitchState.RegisterContextSwitchGroupAction(
                	new ContextSwitchGroup(
                		ContextSwitchGroupKey,
						"Text Editor",
						() =>
						{
							var menuOptionList = new List<MenuOptionRecord>();
							
							var mainGroup = TextEditorService.GroupApi.GetGroups()
								.FirstOrDefault(x => x.Category.Value == "main");
								
							if (mainGroup is not null)
							{
								var viewModelList = new List<TextEditorViewModel>();
								
								foreach (var viewModelKey in mainGroup.ViewModelKeyList)
								{
									var viewModel = TextEditorService.ViewModelApi.GetOrDefault(viewModelKey);
									
									if (viewModel is not null)
									{
										viewModelList.Add(viewModel);
										
							        	var absolutePath = EnvironmentProvider.AbsolutePathFactory(
								        	viewModel.ResourceUri.Value,
								        	false);
							        	
							        	menuOptionList.Add(new MenuOptionRecord(
							        		absolutePath.NameWithExtension,
							        		MenuOptionKind.Other,
							        		OnClickFunc: () =>
							        		{
							        			Dispatcher.Dispatch(new WidgetState.SetWidgetAction(null));
							        			return Task.CompletedTask;
							        		}));
							        }
								}
							}
							
							var menu = menuOptionList.Count == 0
								? MenuRecord.Empty
								: new MenuRecord(menuOptionList.ToImmutableArray());
								
							return Task.FromResult(menu);
						})));
            });
            
        base.OnInitialized();
    }
}