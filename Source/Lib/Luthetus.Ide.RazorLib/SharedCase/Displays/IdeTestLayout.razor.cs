using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunner;
using Luthetus.Common.RazorLib.ComponentRunner.Internals;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Drag.Displays;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Panel.States;
using Luthetus.Common.RazorLib.Resize.Displays;
using Luthetus.Common.RazorLib.StateHasChangedBoundaryCase.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Lexing.Models;
using Luthetus.TextEditor.RazorLib.TextEditorCase.Model;
using Luthetus.TextEditor.RazorLib.TextEditorCase.Scenes;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Ide.RazorLib.SharedCase.Displays;

public partial class IdeTestLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelsState> PanelsStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;
    [Inject]
    private ComponentRunnerOptions ComponentRunnerOptions { get; set; } = null!;

    private Key<TextEditorModel> TextEditorModelKey => new Key<TextEditorModel>(SetContentDisplay.TextEditorModelKey.Guid);
    private Key<TextEditorViewModel> TextEditorViewModelKey = new Key<TextEditorViewModel>(SetContentDisplay.TextEditorViewModelKey.Guid);

    private string UnselectableClassCss => DragStateWrap.Value.ShouldDisplay
        ? "balc_unselectable"
        : string.Empty;

    private bool _previousDragStateWrapShouldDisplay;
    private ElementDimensions _bodyElementDimensions = new();
    private StateHasChangedBoundary _bodyAndFooterStateHasChangedBoundaryComponent = null!;
    private List<Type>? _componentTypes;

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;
        TextEditorService.OptionsStateWrap.StateChanged += TextEditorOptionsStateWrap_StateChanged;

        var bodyHeight = _bodyElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

        bodyHeight.DimensionUnits.AddRange(new[]
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
            await TextEditorService.Options.SetFromLocalStorageAsync();
            await AppOptionsService.SetFromLocalStorageAsync();

            if (File.Exists("C:\\Users\\hunte\\Repos\\Demos\\BlazorCrudApp\\BlazorCrudApp.sln"))
            {
                var absolutePath = new AbsolutePath(
                    "C:\\Users\\hunte\\Repos\\Demos\\BlazorCrudApp\\BlazorCrudApp.sln",
                    false,
                    EnvironmentProvider);

                Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionTask(
                    absolutePath,
                    DotNetSolutionSync));
            }

            _componentTypes = new();

            foreach (var assembly in ComponentRunnerOptions.AssembliesToScan)
            {
                _componentTypes.AddRange(assembly
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(ComponentBase)) && t.Name != "_Imports"));
            }

            _componentTypes = _componentTypes.OrderBy(x => x.Name).ToList();

            await _bodyAndFooterStateHasChangedBoundaryComponent.InvokeStateHasChangedAsync();

            var textEditorModel = new TextEditorModel(
                new ResourceUri("uniqueIdentifierGoesHere.cs"),
                DateTime.UtcNow,
                ".cs",
                "public class MyClass\n{\n\n}\n",
                new TextEditorDefaultCompilerService(),
                new GenericDecorationMapper(),
                null,
                new(),
                TextEditorModelKey);

            TextEditorService.Model.RegisterCustom(textEditorModel);

            TextEditorService.ViewModel.Register(
                TextEditorViewModelKey,
                TextEditorModelKey);
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