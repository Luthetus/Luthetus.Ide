using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunner;
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
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.AspNetCore.Components;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.Ide.RazorLib.EditorCase.States;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;

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
    [Inject]
    private RazorCompilerService RazorCompilerService { get; set; } = null!;
    [Inject]
    private CSharpCompilerService CSharpCompilerService { get; set; } = null!;
    [Inject]
    private EditorSync EditorSync { get; set; } = null!;

    private const string RAZOR_FILE_PATH_STRING = "C:\\Users\\hunte\\Repos\\Demos\\BlazorCrudApp\\Obsolete\\razorFile.razor";

    private string UnselectableClassCss => DragStateWrap.Value.ShouldDisplay
        ? "balc_unselectable"
        : string.Empty;

    private bool _previousDragStateWrapShouldDisplay;
    private ElementDimensions _bodyElementDimensions = new();
    private StateHasChangedBoundary _bodyAndFooterStateHasChangedBoundaryComponent = null!;
    private List<Type>? _componentTypes;
    private Key<TextEditorViewModel> _textEditorViewModelKey = Key<TextEditorViewModel>.Empty;

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

                DotNetSolutionSync.SetDotNetSolution(absolutePath);
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

            var text = @"using Microsoft.AspNetCore.Components;

namespace ConsoleApp1.Today;

public partial class PersonSimpleDisplay : ComponentBase
{
	
}";

            var resourceUri = new ResourceUri("TestFile.cs");

            var textEditorModel = new TextEditorModel(
                resourceUri,
                DateTime.UtcNow,
                ".cs",
                text,
                CSharpCompilerService,
                new GenericDecorationMapper(),
                null,
                new());

            TextEditorService.Model.RegisterCustom(textEditorModel);
            CSharpCompilerService.RegisterModel(textEditorModel);

            var razorFileAbsolutePath = new AbsolutePath(
                RAZOR_FILE_PATH_STRING,
                false,
                EnvironmentProvider);

            EditorSync.OpenInEditor(razorFileAbsolutePath, false);
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

    private async Task GetTextEditorViewModelAsync()
    {
        var textEditorModel = TextEditorService.Model.FindOrDefaultByResourceUri(new(RAZOR_FILE_PATH_STRING));

        if (textEditorModel is not null)
        {
            var textEditorViewModelBag = TextEditorService.Model.GetViewModelsOrEmpty(textEditorModel.ModelKey);

            var viewModel = textEditorViewModelBag.FirstOrDefault();

            if (viewModel is not null)
            {
                _textEditorViewModelKey = viewModel.ViewModelKey;
                await _bodyAndFooterStateHasChangedBoundaryComponent.InvokeStateHasChangedAsync();
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