using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public interface IDialogService : ILuthetusCommonService
{
    public IState<DialogState> DialogStateWrap { get; }

    public void RegisterDialogRecord(DialogRecord dialogRecord);
    public void SetDialogRecordIsMaximized(Key<DialogRecord> dialogKey, bool isMaximized);
    public void DisposeDialogRecord(Key<DialogRecord> dialogKey);
}