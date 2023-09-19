using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.SettingsCase.Displays;

public partial class SettingsDisplay : ComponentBase
{
    public static readonly Key<DialogRecord> SettingsDialogKey = Key<DialogRecord>.NewKey();
}