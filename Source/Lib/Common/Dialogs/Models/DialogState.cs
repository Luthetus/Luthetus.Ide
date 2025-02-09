using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.States;

/// <summary>
/// The list provided should not be modified after passing it as a parameter.
/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
/// </summary>
[FeatureState]
public partial record DialogState
{
    public DialogState()
    {
        DialogList = new List<IDialog>();
    }

    public List<IDialog> DialogList { get; init; }
    /// <summary>
    /// The active dialog is either:<br/><br/>
    /// -the one which has focus within it,<br/>
    /// -most recently focused dialog,<br/>
    /// -most recently registered dialog
    /// <br/><br/>
    /// The motivation for this property is when two dialogs are rendered
    /// at the same time, and one overlaps the other. One of those
    /// dialogs is hidden by the other. To be able to 'bring to front'
    /// the dialog one is interested in by setting focus to it, is useful.
    /// </summary>
    public Key<IDynamicViewModel> ActiveDialogKey { get; init; } = Key<IDynamicViewModel>.Empty;
}