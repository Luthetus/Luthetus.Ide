using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Displays;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.ProgramExecutions.States;
using Fluxor;

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
    private IDispatcher Dispatcher { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await HACK_PersonalSettings().ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Personal settings to have closing and reopening the IDE be exactly the state I want while developing.
    /// </summary>
    private async Task HACK_PersonalSettings()
    {
        //// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
        // =======================================================================
        string? slnPersonalPath = null;
        string? projectPersonalPath = null;
#if DEBUG
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            slnPersonalPath = "/home/hunter/Repos/BlazorCrudApp/BlazorCrudApp.sln";
            projectPersonalPath = "/home/hunter/Repos/BlazorCrudApp/BlazorCrudApp.ServerSide/BlazorCrudApp.ServerSide.csproj";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            slnPersonalPath = "C:\\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg.sln";
            projectPersonalPath = "C:\\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg.csproj";
        }
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            slnPersonalPath = "/home/hunter/Repos/Luthetus.Ide_Fork/Luthetus.Ide.sln";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            slnPersonalPath = "C:\\Users\\hunte\\Repos\\Luthetus.Ide_Fork\\Luthetus.Ide.sln";
        }
#endif
        if (!string.IsNullOrWhiteSpace(slnPersonalPath) &&
            await FileSystemProvider.File.ExistsAsync(slnPersonalPath).ConfigureAwait(false))
        {
            var slnAbsolutePath = EnvironmentProvider.AbsolutePathFactory(
                slnPersonalPath,
                false);

            DotNetBackgroundTaskApi.DotNetSolution.SetDotNetSolution(slnAbsolutePath);

            var parentDirectory = slnAbsolutePath.ParentDirectory;
            if (parentDirectory is not null)
            {
                var parentDirectoryAbsolutePath = EnvironmentProvider.AbsolutePathFactory(
                    parentDirectory.Value,
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
                    TreeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                        InputFileContent.TreeViewContainerKey,
                        adhocRootNode,
                        activeNode is null
                            ? ImmutableList<TreeViewNoType>.Empty
                            : new TreeViewNoType[] { activeNode }.ToImmutableList()));
                }
                else
                {
                    TreeViewService.SetRoot(InputFileContent.TreeViewContainerKey, adhocRootNode);

                    TreeViewService.SetActiveNode(
                        InputFileContent.TreeViewContainerKey,
                        activeNode,
                        true,
                        false);
                }
                await pseudoRootNode.LoadChildListAsync().ConfigureAwait(false);

                var setOpenedTreeViewModelAction = new InputFileState.SetOpenedTreeViewModelAction(
                    pseudoRootNode,
                    IdeComponentRenderers,
                    CommonComponentRenderers,
                    FileSystemProvider,
                    EnvironmentProvider);

                Dispatcher.Dispatch(setOpenedTreeViewModelAction);
            }

            if (!string.IsNullOrWhiteSpace(projectPersonalPath) &&
                await FileSystemProvider.File.ExistsAsync(projectPersonalPath).ConfigureAwait(false))
            {
                var projectAbsolutePath = EnvironmentProvider.AbsolutePathFactory(
                    projectPersonalPath,
                    false);

                Dispatcher.Dispatch(new ProgramExecutionState.SetStartupProjectAbsolutePathAction(
                    projectAbsolutePath));
            }
        }
    }
}