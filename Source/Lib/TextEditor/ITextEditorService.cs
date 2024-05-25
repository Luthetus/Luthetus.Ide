using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
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

namespace Luthetus.TextEditor.RazorLib;

public partial interface ITextEditorService
{
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

	public IEditContext OpenEditContext();
	public Task CloseEditContext(IEditContext editContext);

	/// <summary>
	/// This method wraps the provided work in an instance of
	/// <see cref="Luthetus.TextEditor.RazorLib.BackgroundTasks.Models.TextEditorBackgroundTask"/>
	/// and then enqueues the background task on the
	/// <see cref="Luthetus.Common.RazorLib.BackgroundTasks.Models.IBackgroundTaskService"/>.
	/// 
	/// When it is turn for the 'work' to be invoked, a
	/// <see cref="Luthetus.TextEditor.RazorLib.TextEditors.Models.IEditContext"/>
	/// will be provided to the 'work'.
	///
	/// "Contiguous enqueues" to the
    /// <see cref="Luthetus.Common.RazorLib.BackgroundTasks.Models.IBackgroundTaskService"/>
	/// will be batched via a 'foreach' loop at minimum. This 'foreach' loop batching
	/// is important, because every
	/// <see cref="Luthetus.TextEditor.RazorLib.BackgroundTasks.Models.TextEditorBackgroundTask"/>
	/// when finished, will trigger the text editor to re-render.
	///
	/// One can implement further batching logic, by creating an implementation of
	/// <see cref="Luthetus.TextEditor.RazorLib.BackgroundTasks.Models.ITextEditorWork"/>
	/// </summary>
	public Task Post(ITextEditorWork work);

	/// <summary>
	/// This method creates an instance of
	/// <see cref="Luthetus.TextEditor.RazorLib.BackgroundTasks.Models.TextEditorWorkComplex"/>
	///
	/// This overload takes 'cursorKey' and 'getCursorFunc' to allow for non-ui cursors.
	/// </summary>
	public Task Post(
		ResourceUri resourceUri,
		Key<TextEditorCursor> cursorKey,
		Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc,
		TextEditorEdit edit);

	/// <summary>
	/// This method creates an instance of
	/// <see cref="Luthetus.TextEditor.RazorLib.BackgroundTasks.Models.TextEditorWorkComplex"/>
	///
	/// This overload takes 'viewModelKey' and will synchronize state with the UI cursors.
	/// </summary>
	public Task Post(
		ResourceUri resourceUri,
		Key<TextEditorViewModel> viewModelKey,
		TextEditorEdit edit);
}
