using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Displays;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.AppDatas.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.AppDatas.Models;
using Luthetus.Extensions.Config.BackgroundTasks.Models;

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
	private ConfigBackgroundTaskApi ConfigBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private IAppDataService AppDataService { get; set; } = null!;
	[Inject]
	private IInputFileService InputFileService { get; set; } = null!;

	protected override void OnInitialized()
	{
        ConfigBackgroundTaskApi.Enqueue(ConfigWorkKind.InitializeFooterJustifyEndComponents);
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

        DotNetBackgroundTaskApi.DotNetSolution.Enqueue_SetDotNetSolution(slnAbsolutePath);

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

            InputFileService.SetOpenedTreeViewModel(
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
}