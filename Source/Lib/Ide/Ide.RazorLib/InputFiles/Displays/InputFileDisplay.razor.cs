using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;

namespace Luthetus.Ide.RazorLib.InputFiles.Displays;

public partial class InputFileDisplay : FluxorComponent, IInputFileRendererType
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IState<InputFileState> InputFileStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    /// <summary>
    /// Receives the <see cref="_selectedAbsolutePath"/> as
    /// a parameter to the <see cref="RenderFragment"/>
    /// </summary>
    [Parameter]
    public RenderFragment<AbsolutePath?>? HeaderRenderFragment { get; set; }
    /// <summary>
    /// Receives the <see cref="_selectedAbsolutePath"/> as
    /// a parameter to the <see cref="RenderFragment"/>
    /// </summary>
    [Parameter]
    public RenderFragment<AbsolutePath?>? FooterRenderFragment { get; set; }
    /// <summary>
    /// One would likely use <see cref="BodyClassCssString"/> in the case where
    /// either <see cref="HeaderRenderFragment"/> or <see cref="FooterRenderFragment"/>
    /// are being used.
    /// <br/><br/>
    /// This is due to one likely wanting to set a fixed height for the <see cref="HeaderRenderFragment"/>
    /// and a fixed height for the <see cref="FooterRenderFragment"/> and lastly
    /// the body gets a fixed height of calc(100% - HeightForHeaderRenderFragment - HeightForFooterRenderFragment); 
    /// </summary>
    [Parameter]
    public string BodyClassCssString { get; set; } = null!;
    /// <summary>
    /// One would likely use <see cref="BodyStyleCssString"/> in the case where
    /// either <see cref="HeaderRenderFragment"/> or <see cref="FooterRenderFragment"/>
    /// are being used.
    /// <br/><br/>
    /// This is due to one likely wanting to set a fixed height for the <see cref="HeaderRenderFragment"/>
    /// and a fixed height for the <see cref="FooterRenderFragment"/> and lastly
    /// the body gets a fixed height of calc(100% - HeightForHeaderRenderFragment - HeightForFooterRenderFragment); 
    /// </summary>
    [Parameter]
    public string BodyStyleCssString { get; set; } = null!;

    private readonly ElementDimensions _sidebarElementDimensions = new();
    private readonly ElementDimensions _contentElementDimensions = new();

    private AbsolutePath? _selectedAbsolutePath;
    private InputFileTreeViewMouseEventHandler _inputFileTreeViewMouseEventHandler = null!;
    private InputFileTreeViewKeyboardEventHandler _inputFileTreeViewKeyboardEventHandler = null!;
    private InputFileTopNavBar? _inputFileTopNavBarComponent;

    /// <summary>
    /// <see cref="_searchMatchTuples"/> feels a bit hacky.
    /// It is currently being used to track what TreeView nodes are both
    /// displayed on the UI and part of the user's search result.
    ///
    /// A presumption that any mutations to the HashSet are done
    /// via the UI thread. Therefore concurrency is not an issue.
    /// </summary>
    private List<(Key<TreeViewContainer> treeViewContainerKey, TreeViewAbsolutePath treeViewAbsolutePath)> _searchMatchTuples = new();

    public ElementReference? SearchElementReference => _inputFileTopNavBarComponent?.SearchElementReference;

    protected override void OnInitialized()
    {
        _inputFileTreeViewMouseEventHandler = new InputFileTreeViewMouseEventHandler(
            TreeViewService,
            Dispatcher,
            SetInputFileContentTreeViewRootFunc,
			BackgroundTaskService);

        _inputFileTreeViewKeyboardEventHandler = new InputFileTreeViewKeyboardEventHandler(
            TreeViewService,
            InputFileStateWrap,
            Dispatcher,
            IdeComponentRenderers,
            CommonComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            SetInputFileContentTreeViewRootFunc,
            async () =>
            {
                try
                {
                    if (SearchElementReference is not null)
                    {
                        await SearchElementReference.Value
                            .FocusAsync()
                            .ConfigureAwait(false);
                    }
                }
                catch (Exception)
                {
                    // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                    //             This bug is seemingly happening randomly. I have a suspicion
                    //             that there are race-condition exceptions occurring with "FocusAsync"
                    //             on an ElementReference.
                }
            },
            () => _searchMatchTuples,
            BackgroundTaskService);

        InitializeElementDimensions();

        base.OnInitialized();
    }

    private void InitializeElementDimensions()
    {
        var navMenuWidth = _sidebarElementDimensions.DimensionAttributeList
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

        navMenuWidth.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 40,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = AppOptionsStateWrap.Value.Options.ResizeHandleWidthInPixels / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        var contentWidth = _contentElementDimensions.DimensionAttributeList
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

        contentWidth.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = AppOptionsStateWrap.Value.Options.ResizeHandleWidthInPixels / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });
    }

    /// <summary>
    /// TODO: This code should be moved to an Effect, of which is throttled. (2024-05-03)
    /// </summary>
    private async Task SetInputFileContentTreeViewRootFunc(AbsolutePath absolutePath)
    {
        var pseudoRootNode = new TreeViewAbsolutePath(
            absolutePath,
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
}