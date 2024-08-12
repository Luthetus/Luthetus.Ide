(2024-08-12)
============

Text editor virtualization:

public record TextEditorViewModelDisplayParameters(Key<TextEditorViewModel> ViewModelKey)
{
	public string WrapperStyleCssString { get; init; } = string.Empty;
    public string WrapperClassCssString { get; init; } = string.Empty;
    public string TextEditorStyleCssString { get; init; } = string.Empty;
    public string TextEditorClassCssString { get; init; } = string.Empty;
    
    /// <summary>
    /// TabIndex is used for the html attribute named: 'tabindex'
    /// </summary>
    public int TabIndex { get; init; } = -1;
    public RenderFragment<TextEditorRenderBatchValidated>? ContextMenuRenderFragmentOverride { get; init; }
    public RenderFragment<TextEditorRenderBatchValidated>? AutoCompleteMenuRenderFragmentOverride { get; init; }

    /// <summary>
    /// If left null, the default <see cref="HandleAfterOnKeyDownAsync"/> will be used.
    /// </summary>
    public
    	Func<ITextEditorEditContext, TextEditorModelModifier, TextEditorViewModelModifier, CursorModifierBagTextEditor, KeyboardEventArgs, TextEditorComponentData, Task>?
    	AfterOnKeyDownAsync
    	{ get; init; }

    /// <summary>
    /// If left null, the default <see cref="HandleAfterOnKeyDownRangeAsync"/> will be used.
    /// 
    /// If a batch handling of KeyboardEventArgs is performed, then this method will be invoked as opposed to
    /// <see cref="AfterOnKeyDownAsyncFactory"/>, and a list of <see cref="KeyboardEventArgs"/> will be provided,
    /// sorted such that the first index represents the first event fired, and the last index represents the last
    /// event fired.
    /// </summary>
    public
    	Func<ITextEditorEditContext, TextEditorModelModifier, TextEditorViewModelModifier, CursorModifierBagTextEditor, List<KeyboardEventArgs>, TextEditorComponentData, Task>?
    	AfterOnKeyDownRangeAsync
    	{ get; init; }

    /// <summary>
    /// If set to false the <see cref="Displays.Internals.Header"/> will NOT render above the text editor.
    /// </summary>
    public bool IncludeHeaderHelperComponent { get; init; } = true;

    /// <summary>
    /// <see cref="HeaderButtonKinds"/> contains the enum value that represents a button displayed in the optional component: <see cref="TextEditorHeader"/>.
    /// </summary>
    public ImmutableArray<HeaderButtonKind>? HeaderButtonKinds { get; init; }

    /// <summary>
    /// If set to false the <see cref="Displays.Internals.TextEditorFooter"/> will NOT render below the text editor.
    /// </summary>
    public bool IncludeFooterHelperComponent { get; init; } = true;

    /// <summary>
    /// If set to false: the <see cref="Displays.Internals.GutterSection"/> will NOT render. (i.e. line numbers will not render)
    /// </summary>
    public bool IncludeGutterComponent { get; init; } = true;

    public bool IncludeContextMenuHelperComponent { get; init; } = true;

    public ContextRecord ContextRecord { get; init; } = ContextFacts.TextEditorContext;

	/// <summary>
	/// The integrated terminal logic needs a keymap, separate to that of the 'global' keymap used by other text editors.
	/// Therefore, this property is used to provide the <see cref="Keymaps.Models.Terminals.TextEditorKeymapTerminal"/>
	/// to the integrated terminal.<br/><br/>
	/// 
	/// This property is not intended for use in any other scenario.
	/// </summary>
	public Keymap? KeymapOverride { get; init; }
}

public sealed partial class TextEditorViewModelDisplay : ComponentBase, IDisposable
{
	[Inject]
    public IState<TextEditorState> TextEditorStateWrap { get; set; } = null!;
    
    [Parameter]
    public TextEditorViewModelDisplayParameters ViewModelDisplayParameters { get; set; } = new();
    
    private (Key<TextEditorViewModel> ViewModelKey, int Sequence) _viewModelSequenceTuple = (Key<TextEditorViewModel>.Empty, 0);

	protected override void OnParametersSet()
	{
		if (ViewModelDisplayParameters.ViewModelKey != _viewModelSequenceTuple.ViewModelKey)
		{
			_viewModelSequenceTuple = (ViewModelDisplayParameters.ViewModelKey, 0);
		}
	
		base.OnParametersSet();
	}

	protected override bool ShouldRender()
	{
		// Is this invocation to the base necessary?
		// And, where would it go if it were necessary.
		var shouldRender = base.ShouldRender();
	
		// Goal is to create a back and forth communication where the UI notifies
		// some 'external' code that it wishes to render the text editor.
		// I'll just call the 'external' code the 'IState<TextEditorState> TextEditorStateWrap'
		// but that isn't quite so accurate.
		//
		// Upon the very first render, 'ShouldRender()' does not get invoked.
		// This should be fine, because we need to render at least once in order to draw
		// the text editor HTML element.
		//
		// The text editor HTML element goes on to take up 100% width and height of the parent element.
		// So, the very first render allows one to draw an empty text editor, and measure its width, and height.
		//
		// These width and height measurements can then be used for virtualization at a later point.
		//
		// Inside 'override Task OnAfterRenderAsync(bool firstRender)' there is a conditional branch
		// that will cause a re-render if it is the first render.
		//
		// So, we now are at the start of the second render, in which 'ShouldRender' will for the first time be ran.
		//
		// At this point we have the measurements of the text editor HTML element because an empty one was rendered.
		// 
		// But, we don't have a virtualization result that tells us what content to display.
		//
		// So, we need to notify the TextEditorStateWrap that we intend to render something.
		// We pass to the TextEditorStateWrap our '_viewModelSequenceTuple'.
		//
		// If the _viewModelSequenceTuple.Sequence == (the most recent sequence in the TextEditorStateWrap)
		// then no render is needed, the UI is up to date.
		//
		// Anytime a model is modified, all of its corresponding ViewModels need to have their
		// Sequence incremented.
		//
		// As well, anytime a ViewModel itself is modified, increment its sequence.
		//
		// And if the text editor's settings are changed (such as the font size) then increment
		// the sequence for every ViewModel.
		//
		// But, its important to note that the previous events solely incremented the Sequence.
		// 
		// It is up to the user interface (i.e. the text editor component) to notify its attempt
		// to render.
		//
		// At that moment, if the ViewModel's ObservedSequence is not equal to its
		// RunningSequence then the VirtualizationResult is to be calculated,
		// and 'false' should be returned, because it would have rendered outdated
		// information.
		//
		// After the VirtualizationResult is calculated, trigger the UI to render.
		// This once again will enter this 'ShouldRender' method.
		//
		// But, this time when notifying the 'TextEditorStateWrap' that it intends to render,
		// the response is 'true' because the ObservedSequence was updated to be equal to the
		// RunningSequence after the VirtualizationResult was calculated.
		//
		// So, true is returned and the content is rendered.
		
		// As for the virtualization, there is no need for the JavaScript intersection observer.
		// The ITextEditorEditContext knows everything about the scroll position, and other data relating to the text editor.
		// So the virtualization can just be done then and there.
		
		ViewModelDisplayParameters.ViewModelKey;
	}
	
	// Random note before I forget: options changes should be done via a throttled effect that 
	// triggers UI re-render 100x easier than what I'm currently doing.
	
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			// If we were to use the synchronous 'OnAfterRender' version of this method.
			// Could we guarantee that an invocation to 'StateHasChanged' would not
			// need to be put within an invocation to 'InvokeAsync'?
			//
			// The worry being, an exception due to not being on the correct synchronization context.
			// If this override serves only to cause a re-render after the very first render,
			// then it may be a worthwhile optimization to swap this method to the synchronous version
			// if it would be safe to do so.
			await InvokeAsync(StateHasChanged);
		}
	
		// Is this invocation to the base necessary?
		// And, where would it go if it were necessary.
		await base.OnAfterRenderAsync();
	}
}