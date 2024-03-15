using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public static class DialogFacts
{
    public static readonly Key<IPolymorphicUiRecord> InputFileDialogKey = Key<IPolymorphicUiRecord>.NewKey();
}