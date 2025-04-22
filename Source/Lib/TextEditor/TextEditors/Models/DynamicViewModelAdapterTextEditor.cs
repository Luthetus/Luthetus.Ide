using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Tabs.Displays;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// This type contains all data, and logic, necessary to render a text editor from within a dialog, a panel tab, or a text editor group tab.
/// </summary>
public sealed class DynamicViewModelAdapterTextEditor : ITabTextEditor, IPanelTab, IDialog, IDrag
{
    private readonly Type _dragTabComponentType;
    private readonly Dictionary<string, object?>? _dragTabComponentParameterMap;

    private readonly Type? _dragDialogComponentType = null;
    private readonly Dictionary<string, object?>? _dragDialogComponentParameterMap = null;

    public DynamicViewModelAdapterTextEditor(
        Key<TextEditorViewModel> viewModelKey,
        ITextEditorService textEditorService,
        IPanelService panelService,
        IDialogService dialogService,
        CommonBackgroundTaskApi commonBackgroundTaskApi)
    {
        ViewModelKey = viewModelKey;

        TextEditorService = textEditorService;
        PanelService = panelService;
        DialogService = dialogService;
        CommonBackgroundTaskApi = commonBackgroundTaskApi;

        ComponentType = typeof(TextEditorViewModelSlimDisplay);
        ComponentParameterMap = new()
        {
            { nameof(TextEditorViewModelSlimDisplay.TextEditorViewModelKey), ViewModelKey }
        };

        _dragTabComponentType = typeof(TabDisplay);
        _dragTabComponentParameterMap = new()
        {
            { nameof(TabDisplay.Tab), this },
            { nameof(TabDisplay.IsBeingDragged), true }
        };

        DialogFocusPointHtmlElementId = $"luth_dialog-focus-point_{DynamicViewModelKey.Guid}";
    }

    public ITextEditorService TextEditorService { get; }
    public IPanelService PanelService { get; }
    public IDialogService DialogService { get; }
    public CommonBackgroundTaskApi CommonBackgroundTaskApi { get; }

    public Key<TextEditorViewModel> ViewModelKey { get; }
    public Key<Panel> Key { get; }
    public Key<ContextRecord> ContextRecordKey { get; }
    public Key<IDynamicViewModel> DynamicViewModelKey { get; } = Key<IDynamicViewModel>.NewKey();
	public string? SetFocusOnCloseElementId { get; set; }

    public ITabGroup? TabGroup { get; set; }

    public string Title => GetTitle();

	public string TitleVerbose =>
		TextEditorService.ViewModelApi.GetModelOrDefault(ViewModelKey)?.ResourceUri.Value
			?? Title;

    public Type ComponentType { get; }

    public Dictionary<string, object?>? ComponentParameterMap { get; }

    public string? DialogCssClass { get; set; }
    public string? DialogCssStyle { get; set; }
    public ElementDimensions DialogElementDimensions { get; set; } = DialogHelper.ConstructDefaultElementDimensions();
    public bool DialogIsMinimized { get; set; }
    public bool DialogIsMaximized { get; set; }
    public bool DialogIsResizable { get; set; } = true;
    public string DialogFocusPointHtmlElementId { get; init; }
    public List<IDropzone> DropzoneList { get; set; }

    public ElementDimensions DragElementDimensions { get; set; } = DialogHelper.ConstructDefaultElementDimensions();

    public Type DragComponentType => TabGroup is null
        ? _dragDialogComponentType
        : _dragTabComponentType;

    public Dictionary<string, object?>? DragComponentParameterMap => TabGroup is null
        ? _dragDialogComponentParameterMap
        : _dragTabComponentParameterMap;

    public string? DragCssClass { get; set; }
    public string? DragCssStyle { get; set; }

	public IDialog SetDialogIsMaximized(bool isMaximized)
	{
		DialogIsMaximized = isMaximized;
		return this;
	}

    private string GetTitle()
    {
        if (TextEditorService is null)
            return "TextEditorService was null";

        var model = TextEditorService.ViewModelApi.GetModelOrDefault(ViewModelKey);
        var viewModel = TextEditorService.ViewModelApi.GetOrDefault(ViewModelKey);

        if (viewModel is null)
        {
            return "ViewModel not found";
        }
        else if (model is null)
        {
            return "Model not found";
        }
        else
        {
            var displayName = viewModel.GetTabDisplayNameFunc?.Invoke(model)
                ?? model.ResourceUri.Value;

            if (model.IsDirty)
                displayName += '*';

            return displayName;
        }
    }

    public async Task OnDragStartAsync()
    {
        if (TextEditorService is null)
            return;

        if (TabGroup is not null)
        {
            // TODO: Setup the component that renders while dragging
            //
            // throw new NotImplementedException();
        }
        else
        {
            // TODO: Setup the component that renders while dragging
            //
            // throw new NotImplementedException();
        }

        var dropzoneList = new List<IDropzone>();
        AddFallbackDropzone(dropzoneList);
        await AddPanelDropzonesAsync(dropzoneList).ConfigureAwait(false);

        var measuredHtmlElementDimensions = await CommonBackgroundTaskApi.JsRuntimeCommonApi
            .MeasureElementById(
                $"luth_te_group_{TextEditorService.GroupApi.GetTextEditorGroupState().GroupList.Single().GroupKey.Guid}")
            .ConfigureAwait(false);

        measuredHtmlElementDimensions = measuredHtmlElementDimensions with
        {
            ZIndex = 1,
        };

        var elementDimensions = new ElementDimensions();

        elementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

        // Width
        {
            elementDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();
            elementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	measuredHtmlElementDimensions.WidthInPixels,
            	DimensionUnitKind.Pixels));
        }

        // Height
        {
            elementDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();
            elementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	measuredHtmlElementDimensions.HeightInPixels,
            	DimensionUnitKind.Pixels));
        }

        // Left
        {
            elementDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();
            elementDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	measuredHtmlElementDimensions.LeftInPixels,
            	DimensionUnitKind.Pixels));
        }

        // Top
        {
            elementDimensions.TopDimensionAttribute.DimensionUnitList.Clear();
            elementDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	measuredHtmlElementDimensions.TopInPixels,
            	DimensionUnitKind.Pixels));
        }

        dropzoneList.Add(new TextEditorGroupDropzone(
            measuredHtmlElementDimensions,
            TextEditorService.GroupApi.GetTextEditorGroupState().GroupList.Single().GroupKey,
            elementDimensions));

        DropzoneList = dropzoneList;
    }

    public Task OnDragEndAsync(MouseEventArgs mouseEventArgs, IDropzone? dropzone)
    {
        var localTextEditorGroup = TabGroup as TextEditorGroup;
        var localPanelGroup = TabGroup as PanelGroup;

        if (dropzone is TextEditorGroupDropzone textEditorGroupDropzone)
        {
            // Create Dialog
            if (textEditorGroupDropzone.TextEditorGroupKey == Key<TextEditorGroup>.Empty)
            {
                // Delete the current UI
                {
                    if (localTextEditorGroup is not null)
                    {
                        TextEditorService.GroupApi.RemoveViewModel(
                            localTextEditorGroup.GroupKey,
                            ViewModelKey);
                    }
                    else if (localPanelGroup is not null)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        // Is a dialog
                        //
                        // Already a dialog, so nothing needs to be done here.
                        return Task.CompletedTask;
                    }

                    TabGroup = null;
                }

                DialogService.ReduceRegisterAction(this);
            }

            // Create TextEditor Tab
            {
                // Check if to-be group is the same as current group.
                {
                    if (localTextEditorGroup is not null)
                    {
                        if (localTextEditorGroup.GroupKey == textEditorGroupDropzone.TextEditorGroupKey)
                            return Task.CompletedTask;
                    }
                }

                // Delete the current UI
                {
                    if (localTextEditorGroup is not null)
                    {
                        TextEditorService.GroupApi.RemoveViewModel(
                            localTextEditorGroup.GroupKey,
                            ViewModelKey);
                    }
                    else if (localPanelGroup is not null)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        // Is a dialog
                        DialogService.ReduceDisposeAction(DynamicViewModelKey);
                    }

                    TabGroup = null;
                }

                TextEditorService.GroupApi.AddViewModel(
                    textEditorGroupDropzone.TextEditorGroupKey,
                    ViewModelKey);
            }

            return Task.CompletedTask;
        }

        // Create Panel Tab
        if (dropzone is PanelGroupDropzone panelDropzone)
        {
            // Delete the current UI
            {
                if (localTextEditorGroup is not null)
                {
                    TextEditorService.GroupApi.RemoveViewModel(
                        localTextEditorGroup.GroupKey,
                        ViewModelKey);
                }
                else if (localPanelGroup is not null)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    // Is a dialog
                    DialogService.ReduceDisposeAction(DynamicViewModelKey);
                }

                TabGroup = null;
            }

            PanelService.RegisterPanelTab(
                panelDropzone.PanelGroupKey,
                new Panel(
                    Title,
                    new Key<Panel>(ViewModelKey.Guid),
                    DynamicViewModelKey,
                    Key<ContextRecord>.Empty,
                    typeof(TextEditorViewModelSlimDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(TextEditorViewModelSlimDisplay.TextEditorViewModelKey),
                            ViewModelKey
                        },
                    },
                    PanelService,
                    DialogService,
                    CommonBackgroundTaskApi),
                true);

            return Task.CompletedTask;
        }
        else
        {
            return Task.CompletedTask;
        }
    }

    private void AddFallbackDropzone(List<IDropzone> dropzoneList)
    {
        var fallbackElementDimensions = new ElementDimensions();

        fallbackElementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

        // Width
        {
            fallbackElementDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();
            fallbackElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	100,
            	DimensionUnitKind.ViewportWidth));
        }

        // Height
        {
            fallbackElementDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();
            fallbackElementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	100,
            	DimensionUnitKind.ViewportHeight));
        }

        // Left
        {
            fallbackElementDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();
            fallbackElementDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	0,
            	DimensionUnitKind.Pixels));
        }

        // Top
        {
            fallbackElementDimensions.TopDimensionAttribute.DimensionUnitList.Clear();
            fallbackElementDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	0,
            	DimensionUnitKind.Pixels));
        }

        dropzoneList.Add(new TextEditorGroupDropzone(
            new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0),
            Key<TextEditorGroup>.Empty,
            fallbackElementDimensions)
        {
            CssClass = "luth_dropzone-fallback"
        });
    }

    private async Task AddPanelDropzonesAsync(List<IDropzone> dropzoneList)
    {
        var panelGroupHtmlIdTupleList = new (Key<PanelGroup> PanelGroupKey, string HtmlElementId)[]
        {
            (PanelFacts.LeftPanelGroupKey, "luth_ide_panel_left_tabs"),
            (PanelFacts.RightPanelGroupKey, "luth_ide_panel_right_tabs"),
            (PanelFacts.BottomPanelGroupKey, "luth_ide_panel_bottom_tabs"),
        };

        foreach (var panelGroupHtmlIdTuple in panelGroupHtmlIdTupleList)
        {
            var measuredHtmlElementDimensions = await CommonBackgroundTaskApi.JsRuntimeCommonApi
                .MeasureElementById(panelGroupHtmlIdTuple.HtmlElementId)
                .ConfigureAwait(false);

            measuredHtmlElementDimensions = measuredHtmlElementDimensions with
            {
                ZIndex = 1,
            };

            var elementDimensions = new ElementDimensions();

            elementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

            // Width
            {
                elementDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();
                elementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
                	measuredHtmlElementDimensions.WidthInPixels,
                	DimensionUnitKind.Pixels));
            }

            // Height
            {
                elementDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();
                elementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
                	measuredHtmlElementDimensions.HeightInPixels,
                	DimensionUnitKind.Pixels));
            }

            // Left
            {
                elementDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();
                elementDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
                	measuredHtmlElementDimensions.LeftInPixels,
                	DimensionUnitKind.Pixels));
            }

            // Top
            {
                elementDimensions.TopDimensionAttribute.DimensionUnitList.Clear();
                elementDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
                	measuredHtmlElementDimensions.TopInPixels,
                	DimensionUnitKind.Pixels));
            }

            dropzoneList.Add(new PanelGroupDropzone(
                measuredHtmlElementDimensions,
                panelGroupHtmlIdTuple.PanelGroupKey,
                elementDimensions,
                Key<IDropzone>.NewKey(),
                null,
                null));
        }
    }
}
