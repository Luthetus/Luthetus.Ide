using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public static class DialogFacts
{
    public static readonly Key<IDialogViewModel> InputFileDialogKey = Key<IDialogViewModel>.NewKey();
}