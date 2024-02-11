using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.IntegratedTerminal;

public partial class IntegratedTerminalDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    private readonly object TerminalLock = new();
    
    private IntegratedTerminal _integratedTerminal = null!;
    private CancellationTokenSource _terminalCancellationTokenSource = new();
    private Task _terminalTask = Task.CompletedTask;

    protected override void OnInitialized()
    {
        _integratedTerminal = new CliWrapIntegratedTerminal(
            EnvironmentProvider.HomeDirectoryAbsolutePath.Value,
            EnvironmentProvider);

        _integratedTerminal.StateChanged += IntegratedTerminal_StateChanged;

        StartTerminal();

        base.OnInitialized();
    }

    private async void IntegratedTerminal_StateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void StartTerminal()
    {
        if (_terminalTask.IsCompleted)
        {
            lock (TerminalLock)
            {
                if (_terminalTask.IsCompleted)
                {
                    _terminalCancellationTokenSource.Cancel();
                    _terminalCancellationTokenSource = new();

                    var cancellationToken = _terminalCancellationTokenSource.Token;

                    _terminalTask = Task.Run(async () => 
                    {
                        await _integratedTerminal.StartAsync(cancellationToken);
                    });

                    _terminalTask.ContinueWith(async _ =>
                    {
                        // Stop UI rendering the spinner
                        await InvokeAsync(StateHasChanged);
                    });
                }
            }
        }
    }

    private void CancelTerminalOnClick()
    {
        _terminalCancellationTokenSource.Cancel();
        _terminalCancellationTokenSource = new();
    }

    public void Dispose()
    {
        _terminalCancellationTokenSource.Cancel();
        _integratedTerminal.StateChanged -= IntegratedTerminal_StateChanged;
    }
}