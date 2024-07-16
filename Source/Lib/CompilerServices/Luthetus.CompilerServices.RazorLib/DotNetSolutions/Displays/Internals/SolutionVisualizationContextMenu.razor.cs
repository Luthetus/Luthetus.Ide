using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.Models.Internals;

namespace Luthetus.CompilerServices.RazorLib.DotNetSolutions.Displays.Internals;

public partial class SolutionVisualizationContextMenu : ComponentBase
{
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
	private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

	[Parameter, EditorRequired]
    public MouseEventArgs MouseEventArgs { get; set; } = null!;
	[Parameter, EditorRequired]
    public SolutionVisualizationModel SolutionVisualizationModel { get; set; } = null!;

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

	private MenuRecord? _menuRecord = null;

    protected override async Task OnInitializedAsync()
    {
        // Usage of 'OnInitializedAsync' lifecycle method ensure the context menu is only rendered once.
		// Otherwise, one might have the context menu's options change out from under them.
        _menuRecord = await GetMenuRecord(MouseEventArgs).ConfigureAwait(false);
		await InvokeAsync(StateHasChanged);

        await base.OnInitializedAsync();
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
								EnvironmentProvider,
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
		    WidgetRendererType: typeof(SolutionVisualizationSettingsDisplay),
			WidgetParameterMap: new()
			{
				{
					nameof(SolutionVisualizationSettingsDisplay.SolutionVisualizationModel),
					SolutionVisualizationModel
				}
			}));

        if (!menuRecordsList.Any())
            return Task.FromResult(MenuRecord.Empty);

        return Task.FromResult(new MenuRecord(menuRecordsList.ToImmutableArray()));
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