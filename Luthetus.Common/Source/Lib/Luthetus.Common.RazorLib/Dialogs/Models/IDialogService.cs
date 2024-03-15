using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public interface IDialogService
{
    public IState<DialogState> DialogStateWrap { get; }

    public void RegisterDialogRecord(IDialogViewModel dialogRecord);
    public void SetDialogRecordIsMaximized(Key<IDialogViewModel> dialogKey, bool isMaximized);
    public void DisposeDialogRecord(Key<IDialogViewModel> dialogKey);
}