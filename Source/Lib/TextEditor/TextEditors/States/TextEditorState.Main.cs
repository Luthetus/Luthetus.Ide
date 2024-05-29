using System.Collections.Immutable;
using Fluxor;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

/// <summary>
/// This type will replace what was 'TextEditorModelState' and 'TextEditorViewModelState'.
///
/// Edits to a text editor are done via the <see cref="IEditContext"/>.
/// And prior to making this change, both 'TextEditorModelState' and 'TextEditorViewModelState'
/// needed to be dispatched to, once the <see cref="IEditContext"/> was finalized.
///
/// The result of this double dispatching is that the text editor UI is told to re-render
/// twice.
///
/// One can fix this double re-render by tracking the last rendered model and viewModel,
/// but this approach requires extreme precision and is prone to errors: missed re-renders
/// when there should've been one.
///
/// So, the fix for double re-renders will be to combine the [FeatureState] for 'TextEditorModelState'
/// and 'TextEditorViewModelState' into a single [FeatureState].
///
/// A concern with this fix might be, "what if I only want to subscribe to 'TextEditorModelState' changes?"
///
/// (Okay, I'm writing this note after the fact. It seems like no matter what one would need to
///  introduce some 'HashCode' of sorts, that indicates that the model or viewModel has changed.
///  This use case is not in the app at the moment, but if it becomes of need then it can be added.)
/// The solution would be to use:
///
/// ```csharp
/// [Inject]
/// private IStateSelection<TextEditorState, TextEditorModelState?> TextEditorStateSelection { get; set; } = null!;
///
/// protected override void OnInitialized()
/// {
///     TextEditorStateSelection.Select(textEditorState =>
///     {
///         if (textEditorState.ModelList.TryGetValue(
///                 ResourceUri,
///                 out var model))
///		{
///				return model;
///		}
///
///         return null;
///     });
///
///     base.OnInitialized();
/// }
/// ```
/// </summary>
[FeatureState]
public partial record TextEditorState(
	ImmutableList<TextEditorModel> ModelList,
	ImmutableList<TextEditorViewModel> ViewModelList)
{
	public TextEditorState() : this(
		ImmutableList<TextEditorModel>.Empty,
		ImmutableList<TextEditorViewModel>.Empty)
	{
	}
}
