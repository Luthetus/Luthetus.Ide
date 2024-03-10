using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public record TextEditorViewModelPolymorphicUi : IPolymorphicTab, IPolymorphicDialog, IPolymorphicDraggable
{
	public TextEditorViewModelPolymorphicUi(
		Key<TextEditorViewModel> viewModelKey)
	{
		ViewModelKey = viewModelKey;
		DialogElementDimensions = DialogConstructDefaultElementDimensions();
	}

	/// <summary>
	/// TODO: Aquire access to the TextEditorService via the constructor...
	/// ...This hack was used instead, because adding the TextEditorService
	/// to the view model registration logic seems like an odd thing to do.
	/// This needs to be looked into more.
	///
	/// The hack is that: the UI when it sees an instance of this type,
	/// it will set this type's TextEditorService property.
	/// </summary>
	public ITextEditorService? TextEditorService { get; set; }

	public Key<TextEditorViewModel> ViewModelKey { get; }
	public TextEditorGroup TextEditorGroup { get; set; }

	public Key<IPolymorphicUiRecord> Key { get; } = Key<IPolymorphicUiRecord>.NewKey();
	public string? CssClass { get; }
	public string? CssStyle { get; }
	public string Title => GetTitle();
	public Type RendererType { get; }
	public Dictionary<string, object?>? ParameterMap { get; }

	public ElementDimensions DialogElementDimensions { get; }
    public bool DialogIsMinimized { get; set; }
    public bool DialogIsMaximized { get; set; }
    public bool DialogIsResizable { get; set; }
    public string DialogFocusPointHtmlElementId => $"luth_dialog-focus-point_{Key.Guid}";

	public Type DraggableRendererType => RendererType;
	public Dictionary<string, object?> DraggableParameterMap => ParameterMap;
	public ElementDimensions DraggableElementDimensions => DialogElementDimensions;

    public ElementDimensions DialogConstructDefaultElementDimensions()
	{
		var elementDimensions = new ElementDimensions
        {
            ElementPositionKind = ElementPositionKind.Fixed
        };

        // Width
        {
            var width = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            width.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }

        // Height
        {
            var height = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            height.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }

        // Left
        {
            var left = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            left.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 20,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }

        // Top
        {
            var top = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            top.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 20,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }

        return elementDimensions;
	}

	public Task TabSetAsActiveAsync()
	{
		TextEditorService.GroupApi.SetActiveViewModel(TextEditorGroup.GroupKey, ViewModelKey);
		return Task.CompletedTask;
	}

	public Task OnDragStopAsync()
	{
		return Task.CompletedTask;
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

	public string TabGetDynamicCss()
	{
		return TextEditorGroup.ActiveViewModelKey == ViewModelKey
			? "luth_active"
		    : string.Empty;
	}
}