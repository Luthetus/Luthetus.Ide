using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public interface IDialogService
{
    public IState<DialogState> DialogStateWrap { get; }

    public void RegisterDialogRecord(IPolymorphicDialog dialogRecord);
    public void SetDialogRecordIsMaximized(Key<IPolymorphicUiRecord> dialogKey, bool isMaximized);
    public void DisposeDialogRecord(Key<IPolymorphicUiRecord> dialogKey);
}