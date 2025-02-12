using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Edits.Displays;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Displays;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.StartupControls.States;
using Luthetus.Ide.RazorLib.AppDatas.Models;
using Luthetus.Ide.RazorLib.Shareds.Models;
using Luthetus.Ide.RazorLib.Shareds.States;
using Luthetus.Extensions.Git.Displays;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.AppDatas.Models;

namespace Luthetus.Extensions.Config.Installations.Displays;

public partial class LuthetusConfigInitializer : ComponentBase
{
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private IState<StartupControlState> StartupControlStateWrap { get; set; } = null!;
	[Inject]
	private IAppDataService AppDataService { get; set; } = null!;
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private IInputFileService InputFileService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

	protected override void OnInitialized()
	{
		BackgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
			nameof(LuthetusConfigInitializer),
			() =>
			{
				InitializeFooterJustifyEndComponents();
                return ValueTask.CompletedTask;
            });
			
		base.OnInitialized();
	}

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
        	var dotNetAppData = await AppDataService
        		.ReadAppDataAsync<DotNetAppData>(
        			DotNetAppData.AssemblyName, DotNetAppData.TypeName, uniqueIdentifier: null, forceRefreshCache: false)
        		.ConfigureAwait(false);
        		
        	await SetSolution(dotNetAppData).ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
    
    private async Task SetSolution(DotNetAppData dotNetAppData)
    {
    	var solutionMostRecent = dotNetAppData?.SolutionMostRecent;
    
    	if (solutionMostRecent is null)
    		return;
    
    	var slnAbsolutePath = EnvironmentProvider.AbsolutePathFactory(
            solutionMostRecent,
            false);

        DotNetBackgroundTaskApi.DotNetSolution.SetDotNetSolution(slnAbsolutePath);

        var parentDirectory = slnAbsolutePath.ParentDirectory;
        if (parentDirectory is not null)
        {
            var parentDirectoryAbsolutePath = EnvironmentProvider.AbsolutePathFactory(
                parentDirectory,
                true);

            var pseudoRootNode = new TreeViewAbsolutePath(
                parentDirectoryAbsolutePath,
                IdeComponentRenderers,
                CommonComponentRenderers,
                FileSystemProvider,
                EnvironmentProvider,
                true,
                false);

            await pseudoRootNode.LoadChildListAsync().ConfigureAwait(false);

            var adhocRootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(pseudoRootNode.ChildList.ToArray());

            foreach (var child in adhocRootNode.ChildList)
            {
                child.IsExpandable = false;
            }

            var activeNode = adhocRootNode.ChildList.FirstOrDefault();

            if (!TreeViewService.TryGetTreeViewContainer(InputFileContent.TreeViewContainerKey, out var treeViewContainer))
            {
                TreeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
                    InputFileContent.TreeViewContainerKey,
                    adhocRootNode,
                    activeNode is null
                        ? new List<TreeViewNoType>()
                        : new() { activeNode }));
            }
            else
            {
                TreeViewService.ReduceWithRootNodeAction(InputFileContent.TreeViewContainerKey, adhocRootNode);

                TreeViewService.ReduceSetActiveNodeAction(
                    InputFileContent.TreeViewContainerKey,
                    activeNode,
                    true,
                    false);
            }
            await pseudoRootNode.LoadChildListAsync().ConfigureAwait(false);

            InputFileService.ReduceSetOpenedTreeViewModelAction(
                pseudoRootNode,
                IdeComponentRenderers,
                CommonComponentRenderers,
                FileSystemProvider,
                EnvironmentProvider);
        }

		/*
        if (!string.IsNullOrWhiteSpace(projectPersonalPath) &&
            await FileSystemProvider.File.ExistsAsync(projectPersonalPath).ConfigureAwait(false))
        {
            var projectAbsolutePath = EnvironmentProvider.AbsolutePathFactory(
                projectPersonalPath,
                false);

			var startupControl = StartupControlStateWrap.Value.StartupControlList.FirstOrDefault(
				x => x.StartupProjectAbsolutePath.Value == projectAbsolutePath.Value);
				
			if (startupControl is null)
				return;
			
			Dispatcher.Dispatch(new StartupControlState.SetActiveStartupControlKeyAction(startupControl.Key));	
        }
        */
    }
    
    private void InitializeFooterJustifyEndComponents()
    {
    	Dispatcher.Dispatch(new IdeMainLayoutState.RegisterFooterJustifyEndComponentAction(
    		new FooterJustifyEndComponent(
    			Key<FooterJustifyEndComponent>.NewKey(),
				typeof(GitInteractiveIconDisplay),
				new Dictionary<string, object?>
				{
					{
						nameof(GitInteractiveIconDisplay.CssStyleString),
						"margin-right: 15px;"
					}
				})));
				
		Dispatcher.Dispatch(new IdeMainLayoutState.RegisterFooterJustifyEndComponentAction(
    		new FooterJustifyEndComponent(
    			Key<FooterJustifyEndComponent>.NewKey(),
				typeof(DirtyResourceUriInteractiveIconDisplay),
				new Dictionary<string, object?>
				{
					{
						nameof(GitInteractiveIconDisplay.CssStyleString),
						"margin-right: 15px;"
					}
				})));
				
		Dispatcher.Dispatch(new IdeMainLayoutState.RegisterFooterJustifyEndComponentAction(
    		new FooterJustifyEndComponent(
    			Key<FooterJustifyEndComponent>.NewKey(),
				typeof(NotificationsInteractiveIconDisplay),
				ComponentParameterMap: null)));
    }
}