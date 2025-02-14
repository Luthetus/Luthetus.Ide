using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

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
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    public ITextEditorRegistryWrap TextEditorRegistryWrap { get; set; } = null!;
    [Inject]
    public ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    public IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;
    
    public static Key<ContextSwitchGroup> ContextSwitchGroupKey { get; } = Key<ContextSwitchGroup>.NewKey();
    
    protected override void OnInitialized()
    {
    	TextEditorRegistryWrap.CompilerServiceRegistry = CompilerServiceRegistry;
    	TextEditorRegistryWrap.DecorationMapperRegistry = DecorationMapperRegistry;

		CommonApi.BackgroundTaskApi.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            nameof(LuthetusCommonInitializer),
            async () =>
            {
                if (TextEditorConfig.CustomThemeRecordList is not null)
                {
                    foreach (var themeRecord in TextEditorConfig.CustomThemeRecordList)
                    {
						CommonApi.ThemeApi.ReduceRegisterAction(themeRecord);
                    }
                }

                var initialThemeRecord = CommonApi.ThemeApi.GetThemeState().ThemeList.FirstOrDefault(
                    x => x.Key == TextEditorConfig.InitialThemeKey);

                if (initialThemeRecord is not null)
                    TextEditorService.OptionsApi.SetTheme(initialThemeRecord, updateStorage: false);

                await TextEditorService.OptionsApi.SetFromLocalStorageAsync().ConfigureAwait(false);

				CommonApi.ContextApi.ReduceRegisterContextSwitchGroupAction(
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
										
							        	var absolutePath = CommonApi.EnvironmentProviderApi.AbsolutePathFactory(
								        	viewModel.ResourceUri.Value,
								        	false);
							        	
							        	menuOptionList.Add(new MenuOptionRecord(
							        		absolutePath.NameWithExtension,
							        		MenuOptionKind.Other,
							        		onClickFunc: () =>
							        		{
							        			return TextEditorService.OpenInEditorAsync(
													absolutePath.Value,
													true,
													cursorPositionIndex: null,
													new Category("main"),
													viewModel.ViewModelKey);
							        		}));
							        }
								}
							}
							
							var menu = menuOptionList.Count == 0
								? MenuRecord.GetEmpty()
								: new MenuRecord(menuOptionList);
								
							return Task.FromResult(menu);
						}));

				CommonApi.KeymapApi.ReduceRegisterKeymapLayerAction(TextEditorKeymapDefaultFacts.HasSelectionLayer);
            });
            
        base.OnInitialized();
    }
}