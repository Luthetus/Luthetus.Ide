using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Fluxor;

namespace Luthetus.Common.RazorLib.Panels.Models;

/// <summary>
/// Each PanelTab maintains its own element dimensions as
/// each panel might need different amounts of space to be functionally usable.
/// </summary>
public record Panel
{
	public Panel(
		Key<Panel> key,
		ElementDimensions elementDimensions,
		Type contentRendererType,
		string title)
	{
		Key = key;
		ElementDimensions = elementDimensions;
		ContentRendererType = contentRendererType;
		Title = title;
	}

	public ElementDimensions ElementDimensions { get; }
    public ElementDimensions BeingDraggedDimensions { get; }
    public Type IconRendererType { get; }

	public Key<Panel> Key { get; }
	public string? CssClass { get; }
	public string? CssStyle { get; }
	public string Title { get; }
	public Type ContentRendererType { get; }
	public Type RendererType { get; }

	public PanelGroup? PanelGroup { get; set; }

    /// <summary>
    /// TODO: In progress feature: working on keymap that sets focus to a context record...
    /// ... and if the JavaScript set focus returns false (implying focus was NOT set) then
    /// perhaps the ContextRecord is tied to a PanelTab. If so, set the PanelTab as active
    /// then try again to set focus to the now rendered ContextRecord.
    /// </summary>
    public Key<ContextRecord>? ContextRecordKey { get; set; }

	public Dictionary<string, object?>? ParameterMap => new Dictionary<string, object?>();
}
