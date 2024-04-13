using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.JSInterop;

namespace Luthetus.Common.RazorLib.Panels.Models;

public interface IPanelTab : ITab
{
    public Key<Panel> Key { get; }
    public Key<ContextRecord> ContextRecordKey { get; }
    public IDispatcher Dispatcher { get; }
    public IDialogService DialogService { get; }
    public IJSRuntime JsRuntime { get; }
}
