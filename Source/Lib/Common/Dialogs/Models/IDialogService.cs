using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public interface IDialogService
{
    public IState<DialogState> DialogStateWrap { get; }

    public void RegisterDialogRecord(IDialog dialogRecord);
    public void SetDialogRecordIsMaximized(Key<IDynamicViewModel> dynamicViewModelKey, bool isMaximized);
    public void DisposeDialogRecord(Key<IDynamicViewModel> dynamicViewModelKey);
}