using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.Models;

public class StartupControlModel : IStartupControlModel
{
	public StartupControlModel(
		Key<IStartupControlModel> key,
		string title,
		string titleVerbose,
		IAbsolutePath startupProjectAbsolutePath,
		Key<TerminalCommand> terminalCommandKey,
		TerminalCommand? executingTerminalCommand,
		Type? componentType,
		Dictionary<string, object?>? componentParameterMap,
		Func<Task<TerminalCommand?>> getTerminalCommandFunc,
		Func<IStartupControlModel, Task> startButtonOnClickTask)
	{
		Key = key;
		Title = title;
		TitleVerbose = titleVerbose;
		StartupProjectAbsolutePath = startupProjectAbsolutePath;
		TerminalCommandKey = terminalCommandKey;
		ExecutingTerminalCommand = executingTerminalCommand;
		ComponentType = componentType;
		ComponentParameterMap = componentParameterMap;
		GetTerminalCommandFunc = getTerminalCommandFunc;
		StartButtonOnClickTask = startButtonOnClickTask;
	}
	
    private CancellationTokenSource _terminalCancellationTokenSource = new();

	public Key<IStartupControlModel> Key { get; }
	public string Title { get; }
	public string TitleVerbose { get; }
	public IAbsolutePath StartupProjectAbsolutePath { get; }
	public Key<TerminalCommand> TerminalCommandKey { get; }
	public TerminalCommand? ExecutingTerminalCommand { get; }
	public Type? ComponentType { get; }
	public Dictionary<string, object?>? ComponentParameterMap { get; }
	public Func<Task<TerminalCommand?>> GetTerminalCommandFunc { get; }
	public Func<IStartupControlModel, Task> StartButtonOnClickTask { get; }
	public bool IsExecuting { get; }
}
