using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Outputs.Models;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.ErrorLists.Displays;

public partial class ErrorListDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<TerminalSessionState, TerminalSession?> TerminalSessionStateSelection { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionState> TerminalSessionsStateWrap { get; set; } = null!;

    /// <summary>
    /// <see cref="TerminalSessionKey"/> is used to narrow down the terminal session.
    /// </summary>
    [Parameter, EditorRequired]
    public Key<TerminalSession> TerminalSessionKey { get; set; } = Key<TerminalSession>.Empty;
    /// <summary>
    /// <see cref="TerminalCommandKey"/> is used to narrow down even further to the output of a
	/// specific command that was executed in a specific terminal session. (this parameter is optional)
    /// </summary>
    [Parameter]
    public Key<TerminalCommand> TerminalCommandKey { get; set; } = Key<TerminalCommand>.Empty;
    [Parameter]
    public IOutputParser OutputParser { get; set; } = new OutputParser();
    [Parameter]
    public RenderFragment<List<IOutputLine>>? ChildContent  { get; set; }

    protected override void OnInitialized()
    {
        // Supress un-used service, because I'm hackily injecting it so that 'FluxorComponent'
        // subscribes to its state changes, even though in this class its "unused".
        _ = TerminalSessionsStateWrap;

        TerminalSessionStateSelection.Select(x =>
        {
            if (x.TerminalSessionMap.TryGetValue(TerminalSessionKey, out var terminalSession))
                return terminalSession;

            return null;
        });

        base.OnInitialized();
    }
}