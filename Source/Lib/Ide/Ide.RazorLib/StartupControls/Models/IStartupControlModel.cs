using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.Models;

public interface IStartupControlModel
{
	public Key<IStartupControlModel> Key { get; }
	
	/// <summary>
	/// By default, this is used per option html element within the select dropdown.
	/// </summary>
	public string Title { get; }
	
	/// <summary>
	/// By default, this is used as hover text (HTML 'title' attribute on the select dropdown)
	/// </summary>
	public string TitleVerbose { get; }
	
	public IAbsolutePath StartupProjectAbsolutePath { get; }
	
	/// <summary>
	/// If more than a 'start button' is necessary, one can provide a Blazor component,
	/// and it will be rendered "to the left"/"prior" to the start button.
	/// </summary>
	public Type? ComponentType { get; }
	
	/// <summary>
	/// The Blazor parameters to pass to the <see cref="ComponentType"/>
	/// </summary>
	public Dictionary<string, object?>? ComponentParameterMap { get; }

	/// <summary>
	/// This func is invoked whether the program is currently executing or not.
	/// One can use <see cref="IsExecuting"/> and perhaps if it isn't executing,
	/// then start the program. And if it is executing, then stop the program.
	/// </summary>
	public Func<IStartupControlModel, Task> StartButtonOnClickTask { get; }
	
	public bool IsExecuting { get; }
	public bool IsCompleted { get; }
}
