using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Displays;

public partial class InputFileDisplay : ComponentBase, IInputFileRendererType, IDisposable
{
    [Inject]
    private IIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IInputFileService InputFileService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private BackgroundTaskService BackgroundTaskService { get; set; } = null!;

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
    	InputFileService.InputFileStateChanged += OnInputFileStateChanged;
    	
        _inputFileTreeViewMouseEventHandler = new InputFileTreeViewMouseEventHandler(
            TreeViewService,
            InputFileService,
            SetInputFileContentTreeViewRootFunc,
			BackgroundTaskService);

        _inputFileTreeViewKeyboardEventHandler = new InputFileTreeViewKeyboardEventHandler(
            TreeViewService,
            InputFileService,
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
    	var appOptionsState = AppOptionsService.GetAppOptionsState();
    
        _sidebarElementDimensions.WidthDimensionAttribute.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit(
            	40,
            	DimensionUnitKind.Percentage),
            new DimensionUnit(
            	appOptionsState.Options.ResizeHandleWidthInPixels / 2,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract)
        });

        _contentElementDimensions.WidthDimensionAttribute.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit(
            	60,
            	DimensionUnitKind.Percentage),
            new DimensionUnit(
            	appOptionsState.Options.ResizeHandleWidthInPixels / 2,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract)
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
            TreeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
                InputFileContent.TreeViewContainerKey,
                adhocRootNode,
                activeNode is null
                    ? Array.Empty<TreeViewNoType>()
                    : new List<TreeViewNoType> { activeNode }));
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
    
    public async void OnInputFileStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	InputFileService.InputFileStateChanged -= OnInputFileStateChanged;
    }
}