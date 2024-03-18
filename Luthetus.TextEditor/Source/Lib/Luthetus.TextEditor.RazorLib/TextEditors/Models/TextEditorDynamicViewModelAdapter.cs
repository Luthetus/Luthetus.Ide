using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Tabs.Displays;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// This type contains all data, and logic, necessary to render a text editor from within a dialog, a panel tab, or a text editor group tab.
/// </summary>
public class TextEditorDynamicViewModelAdapter : ITextEditorTab, IPanelTab, IDialog, IDrag
{
    private readonly Type _dragTabComponentType;
    private readonly Dictionary<string, object?>? _dragTabComponentParameterMap;

    private readonly Type? _dragDialogComponentType = null;
    private readonly Dictionary<string, object?>? _dragDialogComponentParameterMap = null;

    public TextEditorDynamicViewModelAdapter(Key<TextEditorViewModel> viewModelKey)
    {
        ViewModelKey = viewModelKey;

        ComponentType = typeof(TextEditorViewModelDisplay);
        ComponentParameterMap = new()
        {
            { nameof(TextEditorViewModelDisplay.TextEditorViewModelKey), ViewModelKey }
        };

        _dragTabComponentType = typeof(TabDisplay);
        _dragTabComponentParameterMap = new()
        {
            { nameof(TabDisplay.Tab), this },
            { nameof(TabDisplay.IsBeingDragged), true }
        };
    }

    public ITextEditorService TextEditorService { get; set; }
    public IDispatcher Dispatcher { get; set; }
    public IDialogService DialogService { get; set; }
    public IJSRuntime JsRuntime { get; set; }

    public Key<TextEditorViewModel> ViewModelKey { get; }
    public Key<Panel> Key { get; }
    public Key<ContextRecord> ContextRecordKey { get; }
    public Key<IDynamicViewModel> DynamicViewModelKey { get; } = Key<IDynamicViewModel>.NewKey();

    public ITabGroup TabGroup { get; set; }

    public string Title => GetTitle();

    public Type ComponentType { get; }

    public Dictionary<string, object?>? ComponentParameterMap { get; }

    public string? DialogCssClass { get; set; }
    public string? DialogCssStyle { get; set; }
    public ElementDimensions DialogElementDimensions { get; set; } = DialogHelper.ConstructDefaultElementDimensions();
    public bool DialogIsMinimized { get; set; }
    public bool DialogIsMaximized { get; set; }
    public bool DialogIsResizable { get; set; } = true;
    public string DialogFocusPointHtmlElementId { get; set; }
    public ImmutableArray<IDropzone> DropzoneList { get; set; }

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
        throw new NotImplementedException();
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
        await AddPanelDropzonesAsync(dropzoneList);

        var measuredHtmlElementDimensions = await JsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>(
            "luthetusIde.measureElementById",
            $"luth_te_group_{TextEditorService.GroupStateWrap.Value.GroupList.Single().GroupKey.Guid}");

        measuredHtmlElementDimensions = measuredHtmlElementDimensions with
        {
            ZIndex = 1,
        };

        var elementDimensions = new ElementDimensions();

        elementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

        // Width
        {
            var widthDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            widthDimensionAttribute.DimensionUnitList.Clear();
            widthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = measuredHtmlElementDimensions.WidthInPixels,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var heightDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            heightDimensionAttribute.DimensionUnitList.Clear();
            heightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = measuredHtmlElementDimensions.HeightInPixels,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var leftDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            leftDimensionAttribute.DimensionUnitList.Clear();
            leftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = measuredHtmlElementDimensions.LeftInPixels,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var topDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            topDimensionAttribute.DimensionUnitList.Clear();
            topDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = measuredHtmlElementDimensions.TopInPixels,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        dropzoneList.Add(new TextEditorGroupDropzone(
            measuredHtmlElementDimensions,
            TextEditorService.GroupStateWrap.Value.GroupList.Single().GroupKey,
            elementDimensions));

        DropzoneList = dropzoneList.ToImmutableArray();
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

                DialogService.RegisterDialogRecord(this);
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
                        DialogService.DisposeDialogRecord(DynamicViewModelKey);
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
                    DialogService.DisposeDialogRecord(DynamicViewModelKey);
                }

                TabGroup = null;
            }

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
                panelDropzone.PanelGroupKey,
                new Panel(
                    Title,
                    new Key<Panel>(ViewModelKey.Guid),
                    DynamicViewModelKey,
                    Key<ContextRecord>.Empty,
                    typeof(TextEditorViewModelDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(TextEditorViewModelDisplay.TextEditorViewModelKey),
                            ViewModelKey
                        },
                    }),
                true));

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
            var widthDimensionAttribute = fallbackElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            widthDimensionAttribute.DimensionUnitList.Clear();
            widthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 100,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }

        // Height
        {
            var heightDimensionAttribute = fallbackElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            heightDimensionAttribute.DimensionUnitList.Clear();
            heightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 100,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }

        // Left
        {
            var leftDimensionAttribute = fallbackElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            leftDimensionAttribute.DimensionUnitList.Clear();
            leftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 0,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var topDimensionAttribute = fallbackElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            topDimensionAttribute.DimensionUnitList.Clear();
            topDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 0,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
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
            (PanelFacts.LeftPanelRecordKey, "luth_ide_panel_left_tabs"),
            (PanelFacts.RightPanelRecordKey, "luth_ide_panel_right_tabs"),
            (PanelFacts.BottomPanelRecordKey, "luth_ide_panel_bottom_tabs"),
        };

        foreach (var panelGroupHtmlIdTuple in panelGroupHtmlIdTupleList)
        {
            var measuredHtmlElementDimensions = await JsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>(
                "luthetusIde.measureElementById",
                panelGroupHtmlIdTuple.HtmlElementId);

            measuredHtmlElementDimensions = measuredHtmlElementDimensions with
            {
                ZIndex = 1,
            };

            var elementDimensions = new ElementDimensions();

            elementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

            // Width
            {
                var widthDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                    x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

                widthDimensionAttribute.DimensionUnitList.Clear();
                widthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
                {
                    Value = measuredHtmlElementDimensions.WidthInPixels,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                });
            }

            // Height
            {
                var heightDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                    x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

                heightDimensionAttribute.DimensionUnitList.Clear();
                heightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
                {
                    Value = measuredHtmlElementDimensions.HeightInPixels,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                });
            }

            // Left
            {
                var leftDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                    x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

                leftDimensionAttribute.DimensionUnitList.Clear();
                leftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
                {
                    Value = measuredHtmlElementDimensions.LeftInPixels,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                });
            }

            // Top
            {
                var topDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                    x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

                topDimensionAttribute.DimensionUnitList.Clear();
                topDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
                {
                    Value = measuredHtmlElementDimensions.TopInPixels,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                });
            }

            dropzoneList.Add(new PanelGroupDropzone(
                measuredHtmlElementDimensions,
                panelGroupHtmlIdTuple.PanelGroupKey,
                elementDimensions));
        }
    }
}
