using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models.Internals;

public class SolutionVisualizationDrawingLine<TItem> : ISolutionVisualizationDrawingLine
{
	public TItem Item { get; set; }
	public SolutionVisualizationDrawingKind SolutionVisualizationDrawingKind => SolutionVisualizationDrawingKind.Line;
	public SolutionVisualizationItemKind SolutionVisualizationItemKind { get; set; }
	public (int x, int y) StartPoint { get; set; }
	public (int x, int y) EndPoint { get; set; }
	public string Stroke { get; set; }
	public int RenderCycle { get; set; }
	public int RenderCycleSequence { get; set; }

	object ISolutionVisualizationDrawing.Item => Item;

	public MenuOptionRecord GetMenuOptionRecord(
		SolutionVisualizationModel localSolutionVisualizationModel,
		IEnvironmentProvider environmentProvider,
		LuthetusTextEditorConfig textEditorConfig,
		IServiceProvider serviceProvider)
	{
		var menuOptionRecordList = new List<MenuOptionRecord>();

		menuOptionRecordList.Add(new MenuOptionRecord(
			"Connection",
			MenuOptionKind.Other));

		return new MenuOptionRecord(
			"Connection",
			MenuOptionKind.Other,
			subMenu: new MenuRecord(menuOptionRecordList));
	}
}
