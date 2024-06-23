using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models.Internals;

public class SolutionVisualizationDrawingLine<TItem> : ISolutionVisualizationDrawingLine
{
	public TItem Item { get; set; }
	public SolutionVisualizationDrawingKind SolutionVisualizationDrawingKind => SolutionVisualizationDrawingKind.Line;
	public SolutionVisualizationItemKind SolutionVisualizationItemKind { get; set; }
	public (int x, int y) StartPoint { get; set; }
	public (int x, int y) EndPoint { get; set; }
	public string Fill { get; set; }
	public int RenderCycle { get; set; }
	public int RenderCycleSequence { get; set; }

	object ISolutionVisualizationDrawing.Item => Item;

	public MenuOptionRecord GetMenuOptionRecord(
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
			SubMenu: new MenuRecord(menuOptionRecordList.ToImmutableArray()));
	}
}
