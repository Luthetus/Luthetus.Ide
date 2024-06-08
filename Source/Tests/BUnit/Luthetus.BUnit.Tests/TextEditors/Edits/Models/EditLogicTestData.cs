using Fluxor;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.Displays;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.Ide.RazorLib.Installations.Models;

namespace Luthetus.BUnit.Tests.TextEditors.Edits.Models;

public class EditLogicTestData
{
	public EditLogicTestData(
		IRenderedFragment cut,
		TextEditorViewModelDisplay? refTextEditorViewModelDisplay,
		TextEditorViewModelDisplay.TextEditorEvents events,
		TextEditorModel inModel,
		Key<TextEditorViewModel> viewModelKey,
		ITextEditorService textEditorService)
	{
		Cut = cut;
		RefTextEditorViewModelDisplay = refTextEditorViewModelDisplay;
		Events = events;
		InModel = inModel;
		ViewModelKey = viewModelKey;
		TextEditorService = textEditorService;
	}

	public IRenderedFragment Cut { get; }
	public TextEditorViewModelDisplay? RefTextEditorViewModelDisplay { get; }
	public TextEditorViewModelDisplay.TextEditorEvents Events { get; }
	public TextEditorModel InModel { get; }
	public Key<TextEditorViewModel> ViewModelKey { get; }
	public ITextEditorService TextEditorService { get; }
}