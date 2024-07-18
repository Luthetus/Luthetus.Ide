using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Settings.Displays;

public partial class SettingsDisplay : ComponentBase
{
    public static readonly Key<IDynamicViewModel> SettingsDialogKey = Key<IDynamicViewModel>.NewKey();
}