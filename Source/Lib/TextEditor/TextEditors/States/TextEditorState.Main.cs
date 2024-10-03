using System.Collections.Immutable;
using Fluxor;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

/// <summary>
/// This type will replace what was 'TextEditorModelState' and 'TextEditorViewModelState'.
///
/// Edits to a text editor are done via the <see cref="ITextEditorEditContext"/>.
/// And prior to making this change, both 'TextEditorModelState' and 'TextEditorViewModelState'
/// needed to be dispatched to, once the <see cref="ITextEditorEditContext"/> was finalized.
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
///
/// ImmutableList shouldn't be used here, (at least not the way it currently is).
/// Every time we want to get a specific TextEditorModel via their ResourceUri,
/// then this code is used: ModelList.GetFirstOrDefault(x => x.ResourceUri == resourceUri).
/// This was the way things were for a few reasons. One because not enough files were being processed
/// by the IDE for it to matter. Two because ImmutableList is a tree datastructure internally.
/// But, that "two" point is completely pointless if one isn't searching by a hash.
/// It likely is just iterating the tree and checking the predicate until it finds true. (2024-10-02)
///
/// With the Luthetus.Ide solution open, I have
/// 1,833 text editor models (post parsing the entire solution).
/// |
/// And I have 11 view models (post parsing the entire solution 
/// as well having opened a couple of files.)
/// |
/// I was concerned about the text editor models, and these numbers make sense.
/// We were recreating the ImmutableList<TextEditorModel> every time a user interacted
/// with the text editor.
/// |
/// The count of 1,833 isn't actually as bad as I thought it would be.
/// It might be only a minor optimization to have made it a dictionary.
/// |
/// But, the Dictionary of <TextEditorModel> isn't being recreated everytime
/// a user interacts with the text editor anymore.
/// |
/// And that should be quite noticible. Granted that it was only recreating
/// a List which contained the references to objects (not the objects themselves).
/// |
/// It still probably was quite a lot to have to create a new instance and add the
/// references to it. Also garbage collection of "throwing away" the previous immutable list
/// perhaps could've been an issue.
/// |
/// It is probably a good idea to replicate these changes (list to dictionary, and don't recreate)
/// for the text editor view-models. But, the only way to get 1,833 view models (at the moment)
/// would be to manually open 1,833 files in the text editor.
/// |
/// So theoretically the view models over time would cause minor slowing of the IDE as it
/// has more and more to churn through (until the changes are made)
/// |
/// I used <see cref="__ModelRegisterDisposeLock"/> specifically when
/// adding or removing from the dictionary. Because its presumed that this would
/// have a chance to be an enumeration was modified, if someone was reading it.
/// |
/// As for reading an individual value by providing a key, its presumed that
/// whatever value they get is what they get, it doesn't matter so long as
/// it isn't deleted while they try to get it.
/// |
/// I, Hunter Freeman, named these variables very oddly.
/// I do this when I'm feeling tired, but see an interesting scenario
/// such as this optimization. It caught my attention so I figured I would
/// push through and "just get it done". So I could see how it ends up.
/// I need to rename the variables now though. The scope shouldn't be public either.
/// |
/// The idea is that some work leaves behind a bit of techincal debt,
/// but if the work being done will long term be a source of motivation, then
/// that technical debt is actually a techincal loan, which will be paid off. (2024-10-02).
/// </summary>
[FeatureState]
public partial record TextEditorState(ImmutableList<TextEditorViewModel> ViewModelList)
{
	public readonly object __ModelRegisterDisposeLock = new();

	public Dictionary<ResourceUri, TextEditorModel> __ModelList { get; init; } = new();
	
	public TextEditorState() : this(ImmutableList<TextEditorViewModel>.Empty)
	{
	}
	
	public TextEditorModel? Model_GetOrDefault(ResourceUri resourceUri)
    {
    	lock (__ModelRegisterDisposeLock)
    	{
    		var exists = __ModelList.TryGetValue(resourceUri, out var inModel);
    		return inModel;
    	}
    }

    public Dictionary<ResourceUri, TextEditorModel> Model_GetModels()
    {
    	lock (__ModelRegisterDisposeLock)
    	{
    		return new Dictionary<ResourceUri, TextEditorModel>(__ModelList);
    	}
    }
    
    public int Model_GetModelsCount()
    {
    	lock (__ModelRegisterDisposeLock)
    	{
    		return __ModelList.Count;
    	}
    }
}
