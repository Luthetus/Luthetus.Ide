using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorDevToolsAnchorDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;
	
	private static bool _hasRendered = false;
	
	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			if (!_hasRendered)
			{
				_hasRendered = true;
				
				var dialogRecord = new DialogViewModel(
		            Key<IDynamicViewModel>.NewKey(),
		            nameof(TextEditorDevToolsDisplay),
		            typeof(TextEditorDevToolsDisplay),
		            new Dictionary<string, object?>
		            {
		                {
		                    nameof(ITextEditorDependentComponent.TextEditorViewModelDisplay),
		                    TextEditorViewModelDisplay
		                }
		            },
		            null,
					true,
					null);
					
				Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
			}
		}
	}
	
	public void Dispose()
	{
	}
}