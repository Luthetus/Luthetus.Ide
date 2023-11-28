using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers;

public partial class TestExplorerDisplay : FluxorComponent
{
	[Inject]
    private IState<TerminalSessionState> TerminalSessionsStateWrap { get; set; } = null!;
	[Inject]
    private InputFileSync InputFileSync { get; set; } = null!;

	private const string DOTNET_TEST_LIST_TESTS_COMMAND = "dotnet test -t";

	private string _directoryNameForTestDiscovery = string.Empty;
	private List<string> _dotNetTestListTestsCommandOutput = new();

    public Key<TerminalCommand> DotNetTestListTestsTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
    public Key<TerminalCommand> DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
    public CancellationTokenSource DotNetTestListTestsCancellationTokenSource { get; set; } = new();

    private FormattedCommand FormattedCommand => DotNetCliCommandFormatter.FormatDotNetTestListTests();

	private async Task StartDotNetTestListTestsCommandOnClick()
    {
        var localFormattedCommand = FormattedCommand;

		if (String.IsNullOrWhiteSpace(_directoryNameForTestDiscovery))
			return;

		var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

        var dotNetTestListTestsCommand = new TerminalCommand(
            DotNetTestListTestsTerminalCommandKey,
            localFormattedCommand,
            _directoryNameForTestDiscovery,
            DotNetTestListTestsCancellationTokenSource.Token,
            async () => 
			{
				var output = generalTerminalSession.ReadStandardOut(DotNetTestListTestsTerminalCommandKey);

				_dotNetTestListTestsCommandOutput = DotNetCliOutputLexer.LexDotNetTestListTestsTerminalOutput(output);
                await InvokeAsync(StateHasChanged);
			});

        await generalTerminalSession.EnqueueCommandAsync(dotNetTestListTestsCommand);
    }

	private void RequestInputFileForTestDiscovery()
    {
        InputFileSync.RequestInputFileStateForm("Directory for Test Discovery",
            async afp =>
            {
                if (afp is null)
                    return;

                _directoryNameForTestDiscovery = afp.Value;

                await InvokeAsync(StateHasChanged);
            },
            afp =>
            {
                if (afp is null || !afp.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", afp => afp.IsDirectory)
            }.ToImmutableArray());
    }

	private async Task RunTestByFullyQualifiedName(string fullyQualifiedName)
	{
		var dotNetTestByFullyQualifiedNameFormattedCommand = DotNetCliCommandFormatter.FormatDotNetTestByFullyQualifiedName(fullyQualifiedName);

		if (String.IsNullOrWhiteSpace(_directoryNameForTestDiscovery) ||
			String.IsNullOrWhiteSpace(fullyQualifiedName))
		{
			return;
		}

		var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

        var dotNetTestByFullyQualifiedNameTerminalCommand = new TerminalCommand(
            DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey,
            dotNetTestByFullyQualifiedNameFormattedCommand,
            _directoryNameForTestDiscovery,
            CancellationToken.None,
            () => Task.CompletedTask);

        await generalTerminalSession.EnqueueCommandAsync(dotNetTestByFullyQualifiedNameTerminalCommand);
	}
}
 
    