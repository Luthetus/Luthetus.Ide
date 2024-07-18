using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;

namespace Luthetus.BUnit.Tests.TextEditors;

public class EditLogicTestData
{
	public EditLogicTestData(
		IRenderedFragment cut,
		TextEditorViewModelDisplay? refTextEditorViewModelDisplay,
		TextEditorComponentData componentData,
		TextEditorModel inModel,
		Key<TextEditorViewModel> viewModelKey,
		ITextEditorService textEditorService)
	{
		Cut = cut;
		RefTextEditorViewModelDisplay = refTextEditorViewModelDisplay;
		ComponentData = componentData;
		InModel = inModel;
		ViewModelKey = viewModelKey;
		TextEditorService = textEditorService;
	}

	public IRenderedFragment Cut { get; }
	public TextEditorViewModelDisplay? RefTextEditorViewModelDisplay { get; }
	public TextEditorComponentData ComponentData { get; }
	public TextEditorModel InModel { get; }
	public Key<TextEditorViewModel> ViewModelKey { get; }
	public ITextEditorService TextEditorService { get; }
}