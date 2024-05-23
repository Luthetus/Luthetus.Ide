using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.Tests.Adhoc.Rewrite;

public class TestTextEditorService : ITextEditorService
{
	private readonly IBackgroundTaskService _backgroundTaskService;

	public TestTextEditorService(IBackgroundTaskService backgroundTaskService)
	{
		_backgroundTaskService = backgroundTaskService;
	}

    /// <summary>This is used when interacting with the <see cref="IStorageService"/> to set and get data.</summary>
    public string StorageKey { get; }
    public string ThemeCssClassString { get; }

    public ITextEditorModelApi ModelApi { get; }
    public ITextEditorViewModelApi ViewModelApi { get; }
    public ITextEditorGroupApi GroupApi { get; }
    public ITextEditorDiffApi DiffApi { get; }
    public ITextEditorOptionsApi OptionsApi { get; }

    public IState<TextEditorModelState> ModelStateWrap { get; }
    public IState<TextEditorViewModelState> ViewModelStateWrap { get; }
    public IState<TextEditorGroupState> GroupStateWrap { get; }
    public IState<TextEditorDiffState> DiffStateWrap { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
    public IState<TextEditorOptionsState> OptionsStateWrap { get; }
    public IState<TextEditorFindAllState> FindAllStateWrap { get; }

	public IEditContext OpenEditContext()
	{
		throw new NotImplementedException();
	}

	public Task CloseEditContext(IEditContext editContext)
	{
		throw new NotImplementedException();
	}

	public Task Post(ITextEditorWork work)
	{
		var backgroundTask = new TextEditorBackgroundTask(work);
		throw new NotImplementedException();
		return Task.CompletedTask;
	}

	public Task Post(ITextEditorTask textEditorTask)
	{
		throw new NotImplementedException();
	}

    public Task PostSimpleBatch(
        string name,
        string identifier,
		string? redundancy,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null)
	{
		throw new NotImplementedException();
	}
}
