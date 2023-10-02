using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Settings.Displays;

public partial class SettingsDisplay : ComponentBase
{
    public static readonly Key<DialogRecord> SettingsDialogKey = Key<DialogRecord>.NewKey();
}