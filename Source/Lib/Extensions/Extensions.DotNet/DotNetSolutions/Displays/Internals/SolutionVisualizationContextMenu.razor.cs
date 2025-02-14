using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models.Internals;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Displays.Internals;

public partial class SolutionVisualizationContextMenu : ComponentBase
{
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;
	[Inject]
	private LuthetusCommonApi CommonApi { get; set; } = null!;
	
	[CascadingParameter]
    public DropdownRecord? Dropdown { get; set; }

	[Parameter, EditorRequired]
	public MouseEventArgs MouseEventArgs { get; set; } = null!;
	[Parameter, EditorRequired]
	public SolutionVisualizationModel SolutionVisualizationModel { get; set; } = null!;

	public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

	private MenuRecord? _menuRecord = null;
	private bool _htmlElementDimensionsChanged = false;

	protected override async Task OnInitializedAsync()
	{
		// Usage of 'OnInitializedAsync' lifecycle method ensure the context menu is only rendered once.
		// Otherwise, one might have the context menu's options change out from under them.
		_menuRecord = await GetMenuRecord(MouseEventArgs).ConfigureAwait(false);
		_htmlElementDimensionsChanged = true;
		await InvokeAsync(StateHasChanged);

		await base.OnInitializedAsync();
	}
	
	protected override void OnAfterRender(bool firstRender)
	{
		var localDropdown = Dropdown;

		if (localDropdown is not null && _htmlElementDimensionsChanged)
		{
			_htmlElementDimensionsChanged = false;
			localDropdown.OnHtmlElementDimensionsChanged();
		}
		
		base.OnAfterRender(firstRender);
	}

	private Task<MenuRecord> GetMenuRecord(MouseEventArgs mouseEventArgs)
	{
		var menuRecordsList = new List<MenuOptionRecord>();

		var localSolutionVisualizationModel = SolutionVisualizationModel;

		if (localSolutionVisualizationModel.Dimensions.DivBoundingClientRect is not null)
		{
			var relativeX = mouseEventArgs.ClientX - localSolutionVisualizationModel.Dimensions.DivBoundingClientRect.LeftInPixels;
			var viewBoxX = relativeX / localSolutionVisualizationModel.Dimensions.ScaleX;

			var relativeY = mouseEventArgs.ClientY - localSolutionVisualizationModel.Dimensions.DivBoundingClientRect.TopInPixels;
			var viewBoxY = relativeY / localSolutionVisualizationModel.Dimensions.ScaleY;

			foreach (var drawing in localSolutionVisualizationModel.SolutionVisualizationDrawingList)
			{
				if (drawing is ISolutionVisualizationDrawingCircle solutionVisualizationDrawingCircle)
				{
					var lowerX = solutionVisualizationDrawingCircle.CenterX - solutionVisualizationDrawingCircle.Radius;
					var upperX = solutionVisualizationDrawingCircle.CenterX + solutionVisualizationDrawingCircle.Radius;

					var lowerY = solutionVisualizationDrawingCircle.CenterY - solutionVisualizationDrawingCircle.Radius;
					var upperY = solutionVisualizationDrawingCircle.CenterY + solutionVisualizationDrawingCircle.Radius;

					var targetDisplayName = solutionVisualizationDrawingCircle.Item.GetType().Name;

					if (lowerX <= relativeX && upperX >= relativeX)
					{
						if (lowerY <= relativeY && upperY >= relativeY)
						{
							menuRecordsList.Add(solutionVisualizationDrawingCircle.GetMenuOptionRecord(
								localSolutionVisualizationModel,
                                CommonApi.EnvironmentProviderApi,
								TextEditorConfig,
								ServiceProvider));
						}
					}
				}
				else if (drawing is ISolutionVisualizationDrawingLine solutionVisualizationDrawingLine)
				{
					//menuRecordsList.Add(new MenuOptionRecord(
					//	"TODO: ISolutionVisualizationDrawingLine",
					//	MenuOptionKind.Other));
				}
			}
		}

		menuRecordsList.Add(new MenuOptionRecord(
			"Settings",
			MenuOptionKind.Other,
			widgetRendererType: typeof(SolutionVisualizationSettingsDisplay),
			widgetParameterMap: new()
			{
				{
					nameof(SolutionVisualizationSettingsDisplay.SolutionVisualizationModel),
					SolutionVisualizationModel
				}
			}));

		if (!menuRecordsList.Any())
			return Task.FromResult(MenuRecord.GetEmpty());

		return Task.FromResult(new MenuRecord(menuRecordsList));
	}

	//// Debugging
	//{
	//	menuRecordsList.Add(new MenuOptionRecord(
	//	    $"(sx{localSolutionVisualizationModel.Dimensions.ScaleX}, sy{localSolutionVisualizationModel.Dimensions.ScaleY})",
	//	    MenuOptionKind.Other));
	//
	//	menuRecordsList.Add(new MenuOptionRecord(
	//	    $"(rx{relativeX:N0}, ry{relativeY:N0})",
	//	    MenuOptionKind.Other));
	//
	//	menuRecordsList.Add(new MenuOptionRecord(
	//	    $"(vx{viewBoxX:N0}, vy{viewBoxY:N0})",
	//	    MenuOptionKind.Other));
	//
	//	targetMenuRecordsList.Add(new MenuOptionRecord(
	//	    $"(lowx{lowerX:N0}, lowy{lowerY:N0})",
	//	    MenuOptionKind.Other));
	//
	//	targetMenuRecordsList.Add(new MenuOptionRecord(
	//	    $"(upx{upperX:N0}, upy{upperY:N0})",
	//	    MenuOptionKind.Other));
	//
	//	targetMenuRecordsList.Add(new MenuOptionRecord(
	//	    $"cx{drawing.CenterX} cy{drawing.CenterY} r{drawing.Radius} f{drawing.Fill} rc{drawing.RenderCycle} rcs{drawing.RenderCycleSequence}",
	//		MenuOptionKind.Other));
	//}

}