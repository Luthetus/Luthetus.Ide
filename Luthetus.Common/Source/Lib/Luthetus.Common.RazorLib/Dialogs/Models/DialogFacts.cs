using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public static class DialogFacts
{
    public static readonly Key<IDynamicViewModel> InputFileDialogKey = Key<IDynamicViewModel>.NewKey();
}