using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.Ide.RazorLib.Settings.Displays;

public partial class SettingsDisplay : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

    public static readonly Key<IDynamicViewModel> SettingsDialogKey = Key<IDynamicViewModel>.NewKey();
}