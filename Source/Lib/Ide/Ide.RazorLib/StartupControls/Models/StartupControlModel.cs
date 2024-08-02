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
		Key<TerminalCommandRequest> terminalCommandKey,
		TerminalCommandRequest? executingTerminalCommandRequest,
		Type? componentType,
		Dictionary<string, object?>? componentParameterMap,
		Func<Task<TerminalCommandRequest?>> getTerminalCommandRequestFunc,
		Func<IStartupControlModel, Task> startButtonOnClickTask)
	{
		Key = key;
		Title = title;
		TitleVerbose = titleVerbose;
		StartupProjectAbsolutePath = startupProjectAbsolutePath;
		TerminalCommandRequestKey = terminalCommandKey;
		ExecutingTerminalCommandRequest = executingTerminalCommandRequest;
		ComponentType = componentType;
		ComponentParameterMap = componentParameterMap;
		GetTerminalCommandRequestFunc = getTerminalCommandRequestFunc;
		StartButtonOnClickTask = startButtonOnClickTask;
	}
	
    private CancellationTokenSource _terminalCancellationTokenSource = new();

	public Key<IStartupControlModel> Key { get; }
	public string Title { get; }
	public string TitleVerbose { get; }
	public IAbsolutePath StartupProjectAbsolutePath { get; }
	public Key<TerminalCommandRequest> TerminalCommandRequestKey { get; }
	public TerminalCommandRequest? ExecutingTerminalCommandRequest { get; }
	public Type? ComponentType { get; }
	public Dictionary<string, object?>? ComponentParameterMap { get; }
	public Func<Task<TerminalCommandRequest?>> GetTerminalCommandRequestFunc { get; }
	public Func<IStartupControlModel, Task> StartButtonOnClickTask { get; }
	public bool IsExecuting { get; }
	public bool IsCompleted { get; }
}
