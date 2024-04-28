using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Luthetus.Common.RazorLib.StateHasChangedBoundaries.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.ProgramExecutions.States;
using Luthetus.TextEditor.RazorLib;
using Microsoft.AspNetCore.Components;
using System.Runtime.InteropServices;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class IdeMainLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelsState> PanelsStateWrap { get; set; } = null!;
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
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;

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
            await TextEditorService.OptionsApi.SetFromLocalStorageAsync();
            await AppOptionsService.SetFromLocalStorageAsync();

            string personalTestPath;

#if DEBUG
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                personalTestPath = "/home/hunter/Repos/BlazorCrudApp/BlazorCrudApp.sln";
            else
                personalTestPath = "C:\\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg.sln";
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                personalTestPath = "/home/hunter/Repos/Luthetus.Ide_Fork/Luthetus.Ide.sln";
            else
                personalTestPath = "C:\\Users\\hunte\\Repos\\Luthetus.Ide_Fork\\Luthetus.Ide.sln";
#endif

            if (await FileSystemProvider.File.ExistsAsync(personalTestPath))
            {
                var absolutePath = EnvironmentProvider.AbsolutePathFactory(
                    personalTestPath,
                    false);

                DotNetSolutionSync.SetDotNetSolution(absolutePath);

#if DEBUG
                string personalProjectPath;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    personalProjectPath = "/home/hunter/Repos/BlazorCrudApp/BlazorCrudApp.ServerSide/BlazorCrudApp.ServerSide.csproj";
                else
                    personalProjectPath = "C:\\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg.csproj";

                Dispatcher.Dispatch(new ProgramExecutionState.SetStartupProjectAbsolutePathAction(
                    EnvironmentProvider.AbsolutePathFactory(
                        personalProjectPath,
                        false)));
#endif
            }
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

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
        TextEditorService.OptionsStateWrap.StateChanged -= TextEditorOptionsStateWrap_StateChanged;
    }
}