using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.Internals;

public partial class AddIntegratedTerminalDisplay : ComponentBase
{
	[Inject]
	private TextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IDialogService DialogService { get; set; } = null!;
	[Inject]
	private INotificationService NotificationService { get; set; } = null!;
	[Inject]
	private IPanelService PanelService { get; set; } = null!;
	[Inject]
	private CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private BackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
	private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
	[Inject]
	private ITerminalService TerminalService { get; set; } = null!;
	
	[CascadingParameter]
	public IDialog Dialog { get; set; } = null!;

	public static Key<TerminalCommandRequest> TypeBashTerminalCommandRequestKey { get; } = Key<TerminalCommandRequest>.NewKey();

	private string _pathToShellExecutable = string.Empty;
	private string _integratedTerminalDisplayName = string.Empty;
	
	protected override void OnInitialized()
	{
		var terminalCommandRequest = new TerminalCommandRequest(
        	"bash -c \"type bash\"",
        	EnvironmentProvider.HomeDirectoryAbsolutePath.Value,
        	TypeBashTerminalCommandRequestKey)
        {
        	ContinueWithFunc = parsedCommand =>
        	{
        		if (string.IsNullOrWhiteSpace(_pathToShellExecutable))
        		{
        			var output = parsedCommand.OutputCache.ToString();
        			var identifierText = "bash is ";
        		
        			if (output.StartsWith(identifierText))
        			{
        				_pathToShellExecutable = output.Substring(identifierText.Length).Trim();
        				
        				if (string.IsNullOrWhiteSpace(_integratedTerminalDisplayName))
        					_integratedTerminalDisplayName = "Bash";
        				
        				return InvokeAsync(StateHasChanged);
        			}
        		}
        		
        		return Task.CompletedTask;
        	}
        };
        	
        TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
	}
	
	private void SubmitOnClick()
	{
		var pathToShellExecutableLocal = _pathToShellExecutable;
		var integratedTerminalDisplayNameLocal = _integratedTerminalDisplayName;
		
		var terminalIntegrated = new TerminalIntegrated(
			_integratedTerminalDisplayName,
			terminal => new TerminalInteractive(terminal),
			terminal => new TerminalInputStringBuilder(terminal),
			terminal => new TerminalOutput(
				terminal,
				new TerminalOutputFormatterExpand(
					terminal,
					TextEditorService,
					CompilerServiceRegistry,
					DialogService,
				    PanelService,
					CommonBackgroundTaskApi)),
			BackgroundTaskService,
			CommonComponentRenderers,
			NotificationService,
			EnvironmentProvider,
			_pathToShellExecutable)
		{
			Key = Key<ITerminal>.NewKey()
		};
		
		terminalIntegrated.Start();
		
		TerminalService.Register(terminalIntegrated);
			
		DialogService.ReduceDisposeAction(Dialog.DynamicViewModelKey);
	}
}