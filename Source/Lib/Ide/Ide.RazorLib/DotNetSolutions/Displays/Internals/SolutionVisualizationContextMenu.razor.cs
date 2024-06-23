using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using System.Collections.Immutable;
using System.Text;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.Ide.RazorLib.TestExplorers.States;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models.Internals;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Displays.Internals;

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

    private async Task<MenuRecord> GetMenuRecord(MouseEventArgs mouseEventArgs)
    {
        var menuRecordsList = new List<MenuOptionRecord>();

		var localSolutionVisualizationModel = SolutionVisualizationModel;

		if (localSolutionVisualizationModel.Dimensions.DivBoundingClientRect is not null)
		{
			var relativeX = mouseEventArgs.ClientX - localSolutionVisualizationModel.Dimensions.DivBoundingClientRect.LeftInPixels;
			var viewBoxX = relativeX / localSolutionVisualizationModel.Dimensions.ScaleX;

			var relativeY = mouseEventArgs.ClientY - localSolutionVisualizationModel.Dimensions.DivBoundingClientRect.TopInPixels;
			var viewBoxY = relativeY / localSolutionVisualizationModel.Dimensions.ScaleY;

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
			//}

			foreach (var drawing in localSolutionVisualizationModel.SolutionVisualizationDrawingList)
			{
				var lowerX = drawing.CenterX - drawing.Radius;
				var upperX = drawing.CenterX + drawing.Radius;

				var lowerY = drawing.CenterY - drawing.Radius;
				var upperY = drawing.CenterY + drawing.Radius;

				//// Debugging
				//{
				//	targetMenuRecordsList.Add(new MenuOptionRecord(
				//	    $"(lowx{lowerX:N0}, lowy{lowerY:N0})",
				//	    MenuOptionKind.Other));
	            //
				//	targetMenuRecordsList.Add(new MenuOptionRecord(
				//	    $"(upx{upperX:N0}, upy{upperY:N0})",
				//	    MenuOptionKind.Other));
				//}

				var targetDisplayName = drawing.Item.GetType().Name;

				if (lowerX <= relativeX && upperX >= relativeX)
				{
					if (lowerY <= relativeY && upperY >= relativeY)
					{
						//// Debugging
						//{
						//	targetMenuRecordsList.Add(new MenuOptionRecord(
						//	    $"cx{drawing.CenterX} cy{drawing.CenterY} r{drawing.Radius} f{drawing.Fill} rc{drawing.RenderCycle} rcs{drawing.RenderCycleSequence}",
						//		MenuOptionKind.Other));
						//}

						menuRecordsList.Add(drawing.GetMenuOptionRecord(
							EnvironmentProvider,
							TextEditorConfig,
							ServiceProvider));
					}
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
            return MenuRecord.Empty;

        return new MenuRecord(menuRecordsList.ToImmutableArray());
    }

    public static string GetContextMenuCssStyleString(
		MouseEventArgs mouseEventArgs,
        IDialog dialogRecord)
    {
        if (mouseEventArgs is null)
            return "display: none;";

        if (dialogRecord.DialogIsMaximized)
        {
            return
                $"left: {mouseEventArgs.ClientX.ToCssValue()}px;" +
                " " +
                $"top: {mouseEventArgs.ClientY.ToCssValue()}px;";
        }
            
        var dialogLeftDimensionAttribute = dialogRecord
            .DialogElementDimensions
            .DimensionAttributeList
            .First(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        var contextMenuLeftDimensionAttribute = new DimensionAttribute
        {
            DimensionAttributeKind = DimensionAttributeKind.Left
        };

        contextMenuLeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
        {
            DimensionUnitKind = DimensionUnitKind.Pixels,
            Value = mouseEventArgs.ClientX
        });

        foreach (var dimensionUnit in dialogLeftDimensionAttribute.DimensionUnitList)
        {
            contextMenuLeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Purpose = dimensionUnit.Purpose,
                Value = dimensionUnit.Value,
                DimensionOperatorKind = DimensionOperatorKind.Subtract,
                DimensionUnitKind = dimensionUnit.DimensionUnitKind
            });
        }

        var dialogTopDimensionAttribute = dialogRecord
            .DialogElementDimensions
            .DimensionAttributeList
            .First(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

        var contextMenuTopDimensionAttribute = new DimensionAttribute
        {
            DimensionAttributeKind = DimensionAttributeKind.Top
        };

        contextMenuTopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
        {
            DimensionUnitKind = DimensionUnitKind.Pixels,
            Value = mouseEventArgs.ClientY
        });

        foreach (var dimensionUnit in dialogTopDimensionAttribute.DimensionUnitList)
        {
            contextMenuTopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Purpose = dimensionUnit.Purpose,
                Value = dimensionUnit.Value,
                DimensionOperatorKind = DimensionOperatorKind.Subtract,
                DimensionUnitKind = dimensionUnit.DimensionUnitKind
            });
        }

        return $"{contextMenuLeftDimensionAttribute.StyleString} {contextMenuTopDimensionAttribute.StyleString}";
    }
}