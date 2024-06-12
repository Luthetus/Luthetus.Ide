using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Luthetus.Common.RazorLib.StateHasChangedBoundaries.Displays;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Displays;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.ProgramExecutions.States;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class IdeMainLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelState> PanelStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private LuthetusIdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private ILuthetusIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    private bool _previousDragStateWrapShouldDisplay;
    private ElementDimensions _bodyElementDimensions = new();
    private StateHasChangedBoundary _bodyAndFooterStateHasChangedBoundaryComponent = null!;

    private string UnselectableClassCss => DragStateWrap.Value.ShouldDisplay ? "balc_unselectable" : string.Empty;

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;
        TextEditorService.OptionsStateWrap.StateChanged += TextEditorOptionsStateWrap_StateChanged;

        var bodyHeight = _bodyElementDimensions.DimensionAttributeList.Single(
            da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

        bodyHeight.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 78,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            },
            new DimensionUnit
            {
                Value = SizeFacts.Ide.Header.Height.Value / 2,
                DimensionUnitKind = SizeFacts.Ide.Header.Height.DimensionUnitKind,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await TextEditorService.OptionsApi
                .SetFromLocalStorageAsync()
                .ConfigureAwait(false);

            await AppOptionsService
                .SetFromLocalStorageAsync()
                .ConfigureAwait(false);

            await HACK_PersonalSettings().ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (_previousDragStateWrapShouldDisplay != DragStateWrap.Value.ShouldDisplay)
        {
            _previousDragStateWrapShouldDisplay = DragStateWrap.Value.ShouldDisplay;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async void TextEditorOptionsStateWrap_StateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Personal settings to have closing and reopening the IDE be exactly the state I want while developing.
    /// </summary>
    private async Task HACK_PersonalSettings()
    {
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

            IdeBackgroundTaskApi.DotNetSolution.SetDotNetSolution(slnAbsolutePath);

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

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
        TextEditorService.OptionsStateWrap.StateChanged -= TextEditorOptionsStateWrap_StateChanged;
    }
}