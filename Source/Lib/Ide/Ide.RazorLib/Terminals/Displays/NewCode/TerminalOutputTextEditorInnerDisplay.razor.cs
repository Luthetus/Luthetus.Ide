using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

public partial class TerminalOutputTextEditorInnerDisplay : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	[Parameter, EditorRequired]
	public TerminalOutputTextEditor TerminalOutputTextEditor { get; set; } = null!;
	
	private readonly Throttle _throttle = new Throttle(TimeSpan.FromMilliseconds(700));

    protected override void OnAfterRender(bool firstRender)
    {
		if (firstRender)
		{
            _throttle.Run(_ =>
            {
                var textEditorViewModel = TextEditorService.ViewModelApi.GetOrDefault(
                    TerminalOutputTextEditor.TextEditorViewModelKey);

                if (textEditorViewModel is null)
                    return Task.CompletedTask;

                return Task.CompletedTask;
            });
        }

        base.OnAfterRender(firstRender);
    }
}