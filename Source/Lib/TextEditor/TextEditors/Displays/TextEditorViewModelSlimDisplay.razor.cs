using System.Text;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Icons.Displays;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays;

public sealed partial class TextEditorViewModelSlimDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    public IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
    public ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    public IDirtyResourceUriService DirtyResourceUriService { get; set; } = null!;
    [Inject]
    public IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    public IClipboardService ClipboardService { get; set; } = null!;
    [Inject]
    public IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    public ILuthetusTextEditorComponentRenderers TextEditorComponentRenderers { get; set; } = null!;
    [Inject]
    public ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    public INotificationService NotificationService { get; set; } = null!;
    [Inject]
    public IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    public IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    public IDialogService DialogService { get; set; } = null!;
    [Inject]
    public IDropdownService DropdownService { get; set; } = null!;
    [Inject]
    public LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private IFindAllService FindAllService { get; set; } = null!;
    // ScrollbarSection.razor.cs
    [Inject]
    private IDragService DragService { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TextEditorViewModel> TextEditorViewModelKey { get; set; } = Key<TextEditorViewModel>.Empty;
    
    [Parameter]
    public ViewModelDisplayOptions ViewModelDisplayOptions { get; set; } = new();

    private readonly Guid _textEditorHtmlElementId = Guid.NewGuid();
    /// <summary>Using this lock in order to avoid the Dispose implementation decrementing when it shouldn't</summary>
    private readonly object _linkedViewModelLock = new();
    // private readonly ThrottleAvailability _throttleAvailabilityShouldRender = new(TimeSpan.FromMilliseconds(30));

	private double _previousGutterWidthInPixels = 0;
	private double _previousLineHeightInPixels = 0;
	
	private double _previousTextEditorHeightInPixels = 0;
	private double _previousScrollHeightInPixels = 0;
	private double _previousScrollTopInPixels = 0;
	private double _previousTextEditorWidthInPixels = 0;
	private double _previousScrollWidthInPixels = 0;
	private double _previousScrollLeftInPixels = 0;

    private TextEditorComponentData _componentData = null!;
    
    // Active is the one given to the UI after the current was validated and found to be valid.
    public TextEditorRenderBatch _currentRenderBatch;
    private TextEditorRenderBatch? _previousRenderBatch;
    public TextEditorRenderBatch? _activeRenderBatch;
    
    private TextEditorViewModel? _linkedViewModel;
    
    private bool _thinksTouchIsOccurring;
    private DateTime? _touchStartDateTime = null;
    private TouchEventArgs? _previousTouchEventArgs = null;
    private bool _userMouseIsInside;
    
    /// <summary>
    /// Share this StringBuilder when used for rendering and no other function is currently using it.
    /// (i.e.: only use this for methods that were invoked from the .razor file)
    /// </summary>
    private StringBuilder _uiStringBuilder = new();
    
    private string _gutterPaddingStyleCssString;
    private string _gutterWidthStyleCssString;
    /// <summary>
    /// Each individual line number is a separate "gutter".
    /// Therefore, the UI in a loop will use a StringBuilder to .Append(...)
    /// - _lineHeightStyleCssString
    /// - _gutterWidthStyleCssString
    /// - _gutterPaddingStyleCssString
    ///
    /// Due to this occuring within a loop, it is presumed that
    /// pre-calculating all 3 strings together would be best.
    ///
    /// Issue with this: anytime any of the variables change that are used
    /// in this calculation, you need to re-calculate this string field,
    /// and it is a D.R.Y. code / omnipotent knowledge
    /// that this needs re-calculated nightmare.
    /// </summary>
    private string _gutterHeightWidthPaddingStyleCssString;
    
    private string _lineHeightStyleCssString;
    
    private string _previous_VERTICAL_GetSliderVerticalStyleCss_Result;
    private string _previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result;
    private string _previous_HORIZONTAL_GetScrollbarHorizontalStyleCss_Result;
    
    private string _bodyStyle = $"width: 100%; left: 0;";
    
    private string _scrollbarSizeInPixelsCssValue;
    
    private bool _previousIncludeHeader;
    private bool _previousIncludeFooter;
    private string _previousGetHeightCssStyleResult = "height: calc(100%);";
    
    private string _verticalVirtualizationBoundaryStyleCssString = "height: 0px;";
	private string _horizontalVirtualizationBoundaryStyleCssString = "width: 0px;";
	
	private double _previousTotalWidth;
	private double _previousTotalHeight;
    
    private string _personalWrapperCssClass;
    private string _personalWrapperCssStyle;
    
    private IconDriver _iconDriver = new IconDriver(widthInPixels: 15, heightInPixels: 15);
    
    private string ContentElementId { get; set; }
    
    public string WrapperCssClass { get; private set; }
    public string WrapperCssStyle { get; private set; }
    
	/// <summary>
	/// Unit of measurement is pixels (px).
	/// </summary>
	private const int DISTANCE_TO_RESET_SCROLL_POSITION = 300;

	private MouseEventArgs? _mouseDownEventArgs;

    private readonly Guid VERTICAL_scrollbarGuid = Guid.NewGuid();
	private readonly Guid HORIZONTAL_scrollbarGuid = Guid.NewGuid();

    private bool VERTICAL_thinksLeftMouseButtonIsDown;

	private double VERTICAL_clientXThresholdToResetScrollTopPosition;
	private double VERTICAL_scrollTopOnMouseDown;

    private string VERTICAL_ScrollbarElementId;
    private string VERTICAL_ScrollbarSliderElementId;

    private bool HORIZONTAL_thinksLeftMouseButtonIsDown;
	private double HORIZONTAL_clientYThresholdToResetScrollLeftPosition;
	private double HORIZONTAL_scrollLeftOnMouseDown;

    private string HORIZONTAL_ScrollbarElementId;
    private string HORIZONTAL_ScrollbarSliderElementId;
	
	private Func<MouseEventArgs, MouseEventArgs, Task>? _dragEventHandler = null;
	
	public bool GlobalShowNewlines => TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.ShowNewlines;
	
	/// <summary>This property will need to be used when multi-cursor is added.</summary>
    public bool IsFocusTarget => true;

    public ElementReference? _cursorDisplayElementReference;
    public int _menuShouldGetFocusRequestCount;
    public string _previousGetCursorStyleCss = string.Empty;
    public string _previousGetCaretRowStyleCss = string.Empty;

    private readonly CancellationTokenSource _onMouseMoveCancellationTokenSource = new();
    private MouseEventArgs? _onMouseMoveMouseEventArgs;
    private Task _onMouseMoveTask = Task.CompletedTask;
    
    private string _blinkAnimationCssClassOn;
    private string _blinkAnimationCssClassOff;

	public TextEditorComponentData ComponentData => _componentData;
	
	// _ = "luth_te_text-editor-cursor " + BlinkAnimationCssClass + " " + _activeRenderBatch.Options.Keymap.GetCursorCssClassString();
	public string BlinkAnimationCssClass => TextEditorService.ViewModelApi.CursorShouldBlink
        ? _blinkAnimationCssClassOn
        : _blinkAnimationCssClassOff;
	
	/// <summary>
	/// Any external UI that isn't a child component of this can subscribe to this event,
	/// and then synchronize with what the text editor is rendering.<br/><br/>
	///
	/// A result of this, is that one can have 'extra' components that are low priority rendering.
	/// Because one can throttle the rendering of those low priority components, and they
	/// will be up to date eventually regardless of how long the throttle is.<br/><br/>
	/// </summary>
	public event Action? RenderBatchChanged;

    protected override void OnInitialized()
    {
    	SetWrapperCssAndStyle();
    	
    	ContentElementId = $"luth_te_text-editor-content_{_textEditorHtmlElementId}";
	    
	    VERTICAL_ScrollbarElementId = $"luth_te_{VERTICAL_scrollbarGuid}";
	    VERTICAL_ScrollbarSliderElementId = $"luth_te_{VERTICAL_scrollbarGuid}-slider";
	    
	    HORIZONTAL_ScrollbarElementId = $"luth_te_{HORIZONTAL_scrollbarGuid}";
	    HORIZONTAL_ScrollbarSliderElementId = $"luth_te_{HORIZONTAL_scrollbarGuid}-slider";
	    
	    var paddingLeftInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS.ToCssValue();
	    var paddingRightInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS.ToCssValue();
        _gutterPaddingStyleCssString = $"padding-left: {paddingLeftInPixelsInvariantCulture}px; padding-right: {paddingRightInPixelsInvariantCulture}px;";
        
        _scrollbarSizeInPixelsCssValue = ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS.ToCssValue();

        ConstructRenderBatch();

        _blinkAnimationCssClassOn = $"luth_te_text-editor-cursor luth_te_blink ";
	    _blinkAnimationCssClassOff = $"luth_te_text-editor-cursor ";
	    
	    var cursorCssClassString = _activeRenderBatch?.Options?.Keymap?.GetCursorCssClassString();
        if (cursorCssClassString is not null)
        {
        	_blinkAnimationCssClassOn += cursorCssClassString;
        	_blinkAnimationCssClassOff += cursorCssClassString;
        }

		SetComponentData();

        TextEditorService.TextEditorStateChanged += GeneralOnStateChangedEventHandler;
        TextEditorService.OptionsApi.StaticStateChanged += OnOptionStaticStateChanged;
        TextEditorService.OptionsApi.MeasuredStateChanged += OnOptionMeasuredStateChanged;
        TextEditorService.ViewModelApi.CursorShouldBlinkChanged += ViewModel_CursorShouldBlinkChanged;
        
        // ScrollbarSection.razor.cs
        DragService.DragStateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
    }
    
    protected override void OnParametersSet()
    {
    	HandleTextEditorViewModelKeyChange();
    	base.OnParametersSet();
    }

    protected override bool ShouldRender()
    {
        if (_linkedViewModel is null)
            HandleTextEditorViewModelKeyChange();

        ConstructRenderBatch();

        if (_currentRenderBatch.ViewModel is not null && _currentRenderBatch.Options is not null)
        {
            if (_currentRenderBatch.ViewModel.DisplayTracker.ConsumeIsFirstDisplay())
				QueueCalculateVirtualizationResultBackgroundTask(_currentRenderBatch);
        }
        
        // Check if the gutter width changed. If so, re-measure text editor.
        if (_activeRenderBatch?.ViewModel is not null)
		{
			var gutterWidthInPixels = _activeRenderBatch.GutterWidthInPixels;
		
			if (_previousGutterWidthInPixels >= 0 && gutterWidthInPixels >= 0)
			{
	        	var absoluteValueDifference = Math.Abs(_previousGutterWidthInPixels - gutterWidthInPixels);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_previousGutterWidthInPixels = gutterWidthInPixels;
	        		var widthInPixelsInvariantCulture = gutterWidthInPixels.ToCssValue();
	        		
	        		_uiStringBuilder.Clear();
	        		_uiStringBuilder.Append("width: ");
	        		_uiStringBuilder.Append(widthInPixelsInvariantCulture);
	        		_uiStringBuilder.Append("px;");
	        		_gutterWidthStyleCssString = _uiStringBuilder.ToString();
	        		
	        		_uiStringBuilder.Clear();
	        		_uiStringBuilder.Append(_lineHeightStyleCssString);
			        _uiStringBuilder.Append(_gutterWidthStyleCssString);
			        _uiStringBuilder.Append(_gutterPaddingStyleCssString);
	        		_gutterHeightWidthPaddingStyleCssString = _uiStringBuilder.ToString();
	        		
	        		_uiStringBuilder.Clear();
	        		_uiStringBuilder.Append("width: calc(100% - ");
			        _uiStringBuilder.Append(widthInPixelsInvariantCulture);
			        _uiStringBuilder.Append("px); left: ");
			        _uiStringBuilder.Append(widthInPixelsInvariantCulture);
			        _uiStringBuilder.Append("px;");
	        		_bodyStyle = _uiStringBuilder.ToString();
	        			        		
	        		// (2025-03-28)
	        		// Console.WriteLine("_activeRenderBatch.ViewModel.DisplayTracker.PostScrollAndRemeasure();");
	        		_activeRenderBatch.ViewModel.DisplayTracker.PostScrollAndRemeasure();
	        		return true;
	        	}
			}
			
			// NOTE: The 'gutterWidth' version of this will do a re-measure,...
			// ...and therefore will return if its condition branch was entered.
			var lineHeightInPixels = _activeRenderBatch.ViewModel.CharAndLineMeasurements.LineHeight;
			
			// The 'gutterWidth' version of this code was written first,
			// and this is just more or less copying the template.
			//
			// TODO: What was the reason for 'gutterWidth' not just doing an '!='?
			//
			if (_previousLineHeightInPixels >= 0 && lineHeightInPixels >= 0)
			{
				var absoluteValueDifference = Math.Abs(_previousLineHeightInPixels - lineHeightInPixels);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_previousLineHeightInPixels = lineHeightInPixels;
					var heightInPixelsInvariantCulture = lineHeightInPixels.ToCssValue();
					
					_uiStringBuilder.Clear();
	        		_uiStringBuilder.Append("height: ");
			        _uiStringBuilder.Append(heightInPixelsInvariantCulture);
			        _uiStringBuilder.Append("px;");
			        _lineHeightStyleCssString = _uiStringBuilder.ToString();
			        
			        _uiStringBuilder.Clear();
	        		_uiStringBuilder.Append(_lineHeightStyleCssString);
			        _uiStringBuilder.Append(_gutterWidthStyleCssString);
			        _uiStringBuilder.Append(_gutterPaddingStyleCssString);
	        		_gutterHeightWidthPaddingStyleCssString = _uiStringBuilder.ToString();
        		}
			}
			
			var textEditorWidth = _activeRenderBatch.ViewModel.TextEditorDimensions.Width;
			var textEditorHeight = _activeRenderBatch.ViewModel.TextEditorDimensions.Height;
			var scrollLeft = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft;
			var scrollWidth = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;
			var scrollHeight = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollHeight;
			var scrollTop = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollTop;
			
			bool shouldCalculateVerticalSlider = false;
			bool shouldCalculateHorizontalSlider = false;
			bool shouldCalculateHorizontalScrollbar = false;
			
			if (_previousTextEditorHeightInPixels >= 0 && textEditorHeight >= 0)
			{
				var absoluteValueDifference = Math.Abs(_previousTextEditorHeightInPixels - textEditorHeight);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_previousTextEditorHeightInPixels = textEditorHeight;
	        		shouldCalculateVerticalSlider = true;
			    }
			}
			
			if (_previousScrollHeightInPixels >= 0 && scrollHeight >= 0)
			{
				var absoluteValueDifference = Math.Abs(_previousScrollHeightInPixels - scrollHeight);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_previousScrollHeightInPixels = scrollHeight;
	        		shouldCalculateVerticalSlider = true;
			    }
			}
			
			if (_previousScrollTopInPixels >= 0 && scrollTop >= 0)
			{
				var absoluteValueDifference = Math.Abs(_previousScrollTopInPixels - scrollTop);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_previousScrollTopInPixels = scrollTop;
	        		shouldCalculateVerticalSlider = true;
			    }
			}
			
			if (_previousTextEditorWidthInPixels >= 0 && textEditorWidth >= 0)
			{
				var absoluteValueDifference = Math.Abs(_previousTextEditorWidthInPixels - textEditorWidth);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_previousTextEditorWidthInPixels = textEditorWidth;
	        		shouldCalculateHorizontalSlider = true;
	        		shouldCalculateHorizontalScrollbar = true;
			    }
			}
			
			if (_previousScrollWidthInPixels >= 0 && scrollWidth >= 0)
			{
				var absoluteValueDifference = Math.Abs(_previousScrollWidthInPixels - scrollWidth);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_previousScrollWidthInPixels = scrollWidth;
	        		shouldCalculateHorizontalSlider = true;
			    }
			}
			
			if (_previousScrollLeftInPixels >= 0 && scrollLeft >= 0)
			{
				var absoluteValueDifference = Math.Abs(_previousScrollLeftInPixels - scrollLeft);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_previousScrollLeftInPixels = scrollLeft;
	        		shouldCalculateHorizontalSlider = true;
			    }
			}

			if (shouldCalculateVerticalSlider)
				VERTICAL_GetSliderVerticalStyleCss();
			
			if (shouldCalculateHorizontalSlider)
				HORIZONTAL_GetSliderHorizontalStyleCss();
			
			if (shouldCalculateHorizontalScrollbar)
				HORIZONTAL_GetScrollbarHorizontalStyleCss();
			
			GetCursorAndCaretRowStyleCss();
		}

        return true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await TextEditorService.JsRuntimeTextEditorApi
                .PreventDefaultOnWheelEvents(ContentElementId)
                .ConfigureAwait(false);

            QueueCalculateVirtualizationResultBackgroundTask(_currentRenderBatch);
        }

        if (_currentRenderBatch.ViewModel is not null && _currentRenderBatch.ViewModel.ShouldSetFocusAfterNextRender)
        {
            _currentRenderBatch.ViewModel.ShouldSetFocusAfterNextRender = false;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void ConstructRenderBatch()
    {
    	var localTextEditorState = TextEditorService.TextEditorState;
    
    	var model_viewmodel_tuple = localTextEditorState.GetModelAndViewModelOrDefault(
			TextEditorViewModelKey);
				
        var renderBatchUnsafe = new TextEditorRenderBatch(
            model_viewmodel_tuple.Model,
            model_viewmodel_tuple.ViewModel,
            TextEditorService.OptionsApi.GetTextEditorOptionsState().Options,
            TextEditorRenderBatch.DEFAULT_FONT_FAMILY,
            TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS,
            ViewModelDisplayOptions,
			_componentData);

        if (!string.IsNullOrWhiteSpace(renderBatchUnsafe.Options?.CommonOptions?.FontFamily))
        	renderBatchUnsafe.FontFamily = renderBatchUnsafe.Options!.CommonOptions!.FontFamily;

        if (renderBatchUnsafe.Options!.CommonOptions?.FontSizeInPixels is not null)
            renderBatchUnsafe.FontSizeInPixels = renderBatchUnsafe.Options!.CommonOptions.FontSizeInPixels;
        
        _previousRenderBatch = _currentRenderBatch;
        
        _currentRenderBatch = renderBatchUnsafe;
        
        _activeRenderBatch = renderBatchUnsafe.Validate() ? renderBatchUnsafe : null;
        
        RenderBatchChanged?.Invoke();
    }
    
    private void SetComponentData()
    {
		_componentData = new(
			_textEditorHtmlElementId,
			ViewModelDisplayOptions,
			_currentRenderBatch.Options,
			this,
			DropdownService,
			ClipboardService,
			CommonComponentRenderers,
			NotificationService,
			TextEditorService,
			TextEditorComponentRenderers,
			FindAllService,
			EnvironmentProvider,
			FileSystemProvider,
			ServiceProvider);
    }

    private async void GeneralOnStateChangedEventHandler() =>
        await InvokeAsync(StateHasChanged);
        
    private async void ViewModel_CursorShouldBlinkChanged() =>
        await InvokeAsync(StateHasChanged);

    private void HandleTextEditorViewModelKeyChange()
    {
        lock (_linkedViewModelLock)
        {
            var localTextEditorViewModelKey = TextEditorViewModelKey;

            var nextViewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(localTextEditorViewModelKey);

            Key<TextEditorViewModel> nextViewModelKey;

            if (nextViewModel is null)
                nextViewModelKey = Key<TextEditorViewModel>.Empty;
            else
                nextViewModelKey = nextViewModel.ViewModelKey;

            var linkedViewModelKey = _linkedViewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;
            var viewKeyChanged = nextViewModelKey != linkedViewModelKey;

            if (viewKeyChanged)
            {
                _linkedViewModel?.DisplayTracker.DecrementLinks();
                nextViewModel?.DisplayTracker.IncrementLinks();

                _linkedViewModel = nextViewModel;

                if (nextViewModel is not null)
                    nextViewModel.ShouldRevealCursor = true;
            }
        }
    }

  public async Task FocusTextEditorAsync()
  {
      var nextViewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(TextEditorViewModelKey);
      
      if (nextViewModel is not null)
      	await nextViewModel.FocusAsync();
  }
    
    private void ReceiveOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
    	// Inlining: 'EventUtils.IsKeyboardEventArgsNoise(keyboardEventArgs)'
    	if (keyboardEventArgs.Key == "Shift" ||
            keyboardEventArgs.Key == "Control" ||
            keyboardEventArgs.Key == "Alt" ||
            keyboardEventArgs.Key == "Meta")
        {
            return;
        }
        
        TextEditorService.WorkerUi.EnqueueOnKeyDown(
        	new OnKeyDown(
				_componentData,
	            new KeymapArgs(keyboardEventArgs),
	            TextEditorViewModelKey));
	}

    private void ReceiveOnContextMenu()
    {
		var localViewModelKey = TextEditorViewModelKey;

		TextEditorService.WorkerArbitrary.PostUnique(
			nameof(ReceiveOnContextMenu),
			editContext =>
			{
				var viewModelModifier = editContext.GetViewModelModifier(localViewModelKey);
				var modelModifier = editContext.GetModelModifier(viewModelModifier.ResourceUri);
				var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
        		var primaryCursor = cursorModifierBag.CursorModifier;

				if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursor is null)
					return ValueTask.CompletedTask;

				TextEditorCommandDefaultFunctions.ShowContextMenu(
			        editContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        primaryCursor,
			        DropdownService,
			        ComponentData);
				
				return ValueTask.CompletedTask;
			});
    }

    private void ReceiveOnDoubleClick(MouseEventArgs mouseEventArgs)
    {
        TextEditorService.WorkerUi.EnqueueOnDoubleClick(
        	new OnDoubleClick(
	            mouseEventArgs,
				_componentData,
	            TextEditorViewModelKey));
    }

    private void ReceiveContentOnMouseDown(MouseEventArgs mouseEventArgs)
    {
        _componentData.ThinksLeftMouseButtonIsDown = true;
        _onMouseMoveMouseEventArgs = null;

        TextEditorService.WorkerUi.EnqueueOnMouseDown(
        	new OnMouseDown(
	            mouseEventArgs,
				_componentData,
	            TextEditorViewModelKey));
    }

    private void ReceiveContentOnMouseMove(MouseEventArgs mouseEventArgs)
	{
	    _userMouseIsInside = true;
	
	    // Buttons is a bit flag '& 1' gets if left mouse button is held
	    if ((mouseEventArgs.Buttons & 1) == 0)
	        _componentData.ThinksLeftMouseButtonIsDown = false;
	
	    var localThinksLeftMouseButtonIsDown = _componentData.ThinksLeftMouseButtonIsDown;
	
	    // MouseStoppedMovingTask
	    _onMouseMoveMouseEventArgs = mouseEventArgs;
            
        if (_onMouseMoveTask.IsCompleted)
        {
            var cancellationToken = _onMouseMoveCancellationTokenSource.Token;

            _onMouseMoveTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                	var mouseMoveMouseEventArgs = _onMouseMoveMouseEventArgs;
                	
                	if (!_userMouseIsInside || _componentData.ThinksLeftMouseButtonIsDown || mouseMoveMouseEventArgs is null)
                	{
                		TextEditorService.WorkerArbitrary.PostUnique(
							nameof(ReceiveContentOnMouseMove),
							editContext =>
							{
								var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);

                                if (viewModelModifier is null)
                                    return ValueTask.CompletedTask;

                                viewModelModifier.TooltipViewModel = null;

								return ValueTask.CompletedTask;
							});
						break;
                	}
                	
                    await Task.Delay(400).ConfigureAwait(false);
                    
                    if (mouseMoveMouseEventArgs == _onMouseMoveMouseEventArgs)
                    {
                        await _componentData.ContinueRenderingTooltipAsync().ConfigureAwait(false);

				        TextEditorService.WorkerArbitrary.PostUnique(
				            nameof(TextEditorCommandDefaultFunctions.HandleMouseStoppedMovingEventAsync),
				            editContext =>
				            {
				            	var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
				                var modelModifier = editContext.GetModelModifier(viewModelModifier.ResourceUri);
				                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
				                var primaryCursorModifier = cursorModifierBag.CursorModifier;
				                
				                if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
				                    return ValueTask.CompletedTask;
				    
				                return TextEditorCommandDefaultFunctions.HandleMouseStoppedMovingEventAsync(
				                    editContext,
				                    modelModifier,
				                    viewModelModifier,
				                    mouseMoveMouseEventArgs,
				                    _componentData,
				                    TextEditorComponentRenderers,
				                    viewModelModifier.ResourceUri);
				            });

                        break;
                    }
                }
            });
        }
	
	    if (!_componentData.ThinksLeftMouseButtonIsDown)
	        return;
	    
	    if (localThinksLeftMouseButtonIsDown)
	    {
	        TextEditorService.WorkerUi.EnqueueOnMouseMove(
	            new OnMouseMove(
	            mouseEventArgs,
	            _componentData,
	            TextEditorViewModelKey));
	    }
	}

    private void ReceiveContentOnMouseOut(MouseEventArgs mouseEventArgs)
    {
        _userMouseIsInside = false;
    }
    
    private void ReceiveOnWheel(WheelEventArgs wheelEventArgs)
    {
    	TextEditorService.WorkerUi.EnqueueOnWheel(
        	new OnWheel(
	            wheelEventArgs,
	            _componentData,
	            TextEditorViewModelKey));
    }

    private void ReceiveOnTouchStart(TouchEventArgs touchEventArgs)
    {
        _touchStartDateTime = DateTime.UtcNow;

        _previousTouchEventArgs = touchEventArgs;
        _thinksTouchIsOccurring = true;
    }

    private void ReceiveOnTouchMove(TouchEventArgs touchEventArgs)
    {
        var localThinksTouchIsOccurring = _thinksTouchIsOccurring;

        if (!_thinksTouchIsOccurring)
             return;

        var previousTouchPoint = _previousTouchEventArgs?.ChangedTouches.FirstOrDefault(x => x.Identifier == 0);
        var currentTouchPoint = touchEventArgs.ChangedTouches.FirstOrDefault(x => x.Identifier == 0);

        if (previousTouchPoint is null || currentTouchPoint is null)
             return;

        // Natural scrolling for touch devices
        var diffX = previousTouchPoint.ClientX - currentTouchPoint.ClientX;
        var diffY = previousTouchPoint.ClientY - currentTouchPoint.ClientY;

        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(ReceiveOnTouchMove),
            editContext =>
			{
				var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
				
				if (viewModelModifier is null)
					return ValueTask.CompletedTask;
				
                TextEditorService.ViewModelApi.MutateScrollHorizontalPosition(
                	editContext,
			        viewModelModifier,
                	diffX);

                TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
                	editContext,
			        viewModelModifier,
                	diffY);
                	
                return ValueTask.CompletedTask;
			});

        _previousTouchEventArgs = touchEventArgs;
    }
    
    private void ClearTouch(TouchEventArgs touchEventArgs)
    {
        var rememberStartTouchEventArgs = _previousTouchEventArgs;

        _thinksTouchIsOccurring = false;
        _previousTouchEventArgs = null;

        var clearTouchDateTime = DateTime.UtcNow;
        var touchTimespan = clearTouchDateTime - _touchStartDateTime;

        if (touchTimespan is null)
            return;

        if (touchTimespan.Value.TotalMilliseconds < 200)
        {
            var startTouchPoint = rememberStartTouchEventArgs?.ChangedTouches.FirstOrDefault(x => x.Identifier == 0);

            if (startTouchPoint is null)
                return;

            ReceiveContentOnMouseDown(new MouseEventArgs
            {
                Buttons = 1,
                ClientX = startTouchPoint.ClientX,
                ClientY = startTouchPoint.ClientY,
            });
        }
    }

	private void QueueRemeasureBackgroundTask(
        TextEditorRenderBatch localRefCurrentRenderBatch,
        CancellationToken cancellationToken)
    {
        var viewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(TextEditorViewModelKey);
        if (viewModel is null)
            return;

        TextEditorService.WorkerArbitrary.PostRedundant(
            nameof(QueueRemeasureBackgroundTask),
			viewModel.ResourceUri,
			viewModel.ViewModelKey,
            editContext =>
            {
            	var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);

				if (viewModelModifier is null)
					return ValueTask.CompletedTask;

            	return TextEditorService.ViewModelApi.RemeasureAsync(
            		editContext,
			        viewModelModifier,
			        CancellationToken.None);
	        });
    }

    private void QueueCalculateVirtualizationResultBackgroundTask(
		TextEditorRenderBatch localCurrentRenderBatch)
    {
        var viewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(TextEditorViewModelKey);
        if (viewModel is null)
            return;

        TextEditorService.WorkerArbitrary.PostRedundant(
            nameof(QueueCalculateVirtualizationResultBackgroundTask),
			viewModel.ResourceUri,
			viewModel.ViewModelKey,
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);

				if (modelModifier is null || viewModelModifier is null)
					return ValueTask.CompletedTask;
            	
            	viewModelModifier.CharAndLineMeasurements = TextEditorService.OptionsApi.GetOptions().CharAndLineMeasurements;
            	
            	TextEditorService.ViewModelApi.CalculateVirtualizationResult(
            		editContext,
			        modelModifier,
			        viewModelModifier,
			        CancellationToken.None);
			    return ValueTask.CompletedTask;
            });
    }

    public string GetGutterStyleCss(string topCssStyle)
    {
    	_uiStringBuilder.Clear();
    
        _uiStringBuilder.Append(topCssStyle);
        _uiStringBuilder.Append(_gutterHeightWidthPaddingStyleCssString);

        return _uiStringBuilder.ToString();
    }
    
    public string GetGutterStyleCssImaginary(int index)
    {
    	_uiStringBuilder.Clear();
    
        var measurements = _activeRenderBatch.ViewModel.CharAndLineMeasurements;

        var topInPixelsInvariantCulture = (index * measurements.LineHeight).ToCssValue();
        _uiStringBuilder.Append("top: ");
        _uiStringBuilder.Append(topInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        _uiStringBuilder.Append(_gutterHeightWidthPaddingStyleCssString);

        return _uiStringBuilder.ToString();
    }

    public string GetGutterSectionStyleCss()
    {
        return _gutterWidthStyleCssString;
    }
    
    /* RowSection.razor Open */
    public string RowSection_GetRowStyleCss(string topCssStyle, double virtualizedRowLeftInPixels)
    {
    	// _activeRenderBatch, 
    
    	_uiStringBuilder.Clear();
    
        _uiStringBuilder.Append(topCssStyle);

        _uiStringBuilder.Append(_lineHeightStyleCssString);

		if (virtualizedRowLeftInPixels > 0)
		{
	        var virtualizedRowLeftInPixelsInvariantCulture = virtualizedRowLeftInPixels.ToCssValue();
	        _uiStringBuilder.Append("left: ");
	        _uiStringBuilder.Append(virtualizedRowLeftInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
        }

        return _uiStringBuilder.ToString();
    }

    public string GetCursorDisplayId()
    {
    	return _activeRenderBatch.ViewModel.PrimaryCursor.IsPrimaryCursor
	        ? _activeRenderBatch?.ViewModel?.PrimaryCursorContentId ?? string.Empty
	        : string.Empty;
    }
        
    public void GetCursorAndCaretRowStyleCss()
    {
        try
        {
        	var measurements = _activeRenderBatch.ViewModel.CharAndLineMeasurements;

	        var leftInPixels = 0d;
	
	        // Tab key column offset
	        {
	            var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
	                _activeRenderBatch.ViewModel.PrimaryCursor.LineIndex,
	                _activeRenderBatch.ViewModel.PrimaryCursor.ColumnIndex);
	
	            // 1 of the character width is already accounted for
	
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            leftInPixels += extraWidthPerTabKey *
	                tabsOnSameRowBeforeCursor *
	                measurements.CharacterWidth;
	        }
	
	        leftInPixels += measurements.CharacterWidth * _activeRenderBatch.ViewModel.PrimaryCursor.ColumnIndex;
	        
	        _uiStringBuilder.Clear();
	
	        var leftInPixelsInvariantCulture = leftInPixels.ToCssValue();
	        _uiStringBuilder.Append("left: ");
	        _uiStringBuilder.Append(leftInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
	
	        var topInPixelsInvariantCulture = (measurements.LineHeight * _activeRenderBatch.ViewModel.PrimaryCursor.LineIndex)
	            .ToCssValue();
	
			_uiStringBuilder.Append("top: ");
			_uiStringBuilder.Append(topInPixelsInvariantCulture);
			_uiStringBuilder.Append("px;");
	
	        _uiStringBuilder.Append(_lineHeightStyleCssString);
	
	        var widthInPixelsInvariantCulture = _activeRenderBatch.Options.CursorWidthInPixels.ToCssValue();
	        _uiStringBuilder.Append("width: ");
	        _uiStringBuilder.Append(widthInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
	
	        _uiStringBuilder.Append(((ITextEditorKeymap)_activeRenderBatch.Options.Keymap).GetCursorCssStyleString(
	            _activeRenderBatch.Model,
	            _activeRenderBatch.ViewModel,
	            _activeRenderBatch.Options));
	        
	        // This feels a bit hacky, exceptions are happening because the UI isn't accessing
	        // the text editor in a thread safe way.
	        //
	        // When an exception does occur though, the cursor should receive a 'text editor changed'
	        // event and re-render anyhow however.
	        // 
	        // So store the result of this method incase an exception occurs in future invocations,
	        // to keep the cursor on screen while the state works itself out.
	        _previousGetCursorStyleCss = _uiStringBuilder.ToString();
        
        	/////////////////////
        	/////////////////////
        	
        	// CaretRow starts here
        	
        	/////////////////////
        	/////////////////////
			
			_uiStringBuilder.Clear();
			
			_uiStringBuilder.Append("top: ");
			_uiStringBuilder.Append(topInPixelsInvariantCulture);
			_uiStringBuilder.Append("px;");
	
	        _uiStringBuilder.Append(_lineHeightStyleCssString);
	
	        var widthOfBodyInPixelsInvariantCulture =
	            (_activeRenderBatch.Model.MostCharactersOnASingleLineTuple.lineLength * measurements.CharacterWidth)
	            .ToCssValue();
	
			_uiStringBuilder.Append("width: ");
			_uiStringBuilder.Append(widthOfBodyInPixelsInvariantCulture);
			_uiStringBuilder.Append("px;");
	
	        // This feels a bit hacky, exceptions are happening because the UI isn't accessing
	        // the text editor in a thread safe way.
	        //
	        // When an exception does occur though, the cursor should receive a 'text editor changed'
	        // event and re-render anyhow however.
	        // 
	        // So store the result of this method incase an exception occurs in future invocations,
	        // to keep the cursor on screen while the state works itself out.
	        _previousGetCaretRowStyleCss = _uiStringBuilder.ToString();
        }
        catch (LuthetusTextEditorException e)
        {
        	Console.WriteLine(e);
        }
    }

    public async Task FocusAsync()
    {
        try
        {
            if (_cursorDisplayElementReference is not null)
            {
                await _cursorDisplayElementReference.Value
                    .FocusAsync(preventScroll: true)
                    .ConfigureAwait(false);
            }
        }
        catch (Exception)
        {
            // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
            //             This bug is seemingly happening randomly. I have a suspicion
            //             that there are race-condition exceptions occurring with "FocusAsync"
            //             on an ElementReference.
        }
    }

    public async Task SetFocusToActiveMenuAsync()
    {
        _menuShouldGetFocusRequestCount++;
        // await InvokeAsync(StateHasChanged);
    }

    public bool TextEditorMenuShouldTakeFocus()
    {
        if (_menuShouldGetFocusRequestCount > 0)
        {
            _menuShouldGetFocusRequestCount = 0;
            return true;
        }

        return false;
    }

    public int GetTabIndex()
    {
        if (!IsFocusTarget)
            return -1;

        return _activeRenderBatch.ViewModelDisplayOptions.TabIndex;
    }
    
    private void HORIZONTAL_GetScrollbarHorizontalStyleCss()
    {
    	var scrollbarWidthInPixels = _activeRenderBatch.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        var scrollbarWidthInPixelsInvariantCulture = scrollbarWidthInPixels.ToCssValue();
        
        _uiStringBuilder.Clear();
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(scrollbarWidthInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        _previous_HORIZONTAL_GetScrollbarHorizontalStyleCss_Result = _uiStringBuilder.ToString();
    }

    private void HORIZONTAL_GetSliderHorizontalStyleCss()
    {
    	var scrollbarWidthInPixels = _activeRenderBatch.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
        
        // Proportional Left
    	var sliderProportionalLeftInPixels = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft *
            scrollbarWidthInPixels /
            _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;

		var sliderProportionalLeftInPixelsInvariantCulture = sliderProportionalLeftInPixels.ToCssValue();

		_uiStringBuilder.Clear();
		
        _uiStringBuilder.Append("bottom: 0; height: ");
        _uiStringBuilder.Append(_scrollbarSizeInPixelsCssValue);
        _uiStringBuilder.Append("px; ");
        _uiStringBuilder.Append(_previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result);
        
        _uiStringBuilder.Append(" left: ");
        _uiStringBuilder.Append(sliderProportionalLeftInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");
        
        // Proportional Width
    	var pageWidth = _activeRenderBatch.ViewModel.TextEditorDimensions.Width;

        var sliderProportionalWidthInPixels = pageWidth *
            scrollbarWidthInPixels /
            _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;

        var sliderProportionalWidthInPixelsInvariantCulture = sliderProportionalWidthInPixels.ToCssValue();
        
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(sliderProportionalWidthInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");
        
        _previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result = _uiStringBuilder.ToString();
    }
	
    private void VERTICAL_GetSliderVerticalStyleCss()
    {
    	var textEditorDimensions = _activeRenderBatch.ViewModel.TextEditorDimensions;
        var scrollBarDimensions = _activeRenderBatch.ViewModel.ScrollbarDimensions;

        var scrollbarHeightInPixels = textEditorDimensions.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Top
        var sliderProportionalTopInPixels = scrollBarDimensions.ScrollTop *
            scrollbarHeightInPixels /
            scrollBarDimensions.ScrollHeight;

        var sliderProportionalTopInPixelsInvariantCulture = sliderProportionalTopInPixels.ToCssValue();

		_uiStringBuilder.Clear();
		
		_uiStringBuilder.Append("left: 0; width: ");
		_uiStringBuilder.Append(_scrollbarSizeInPixelsCssValue);
		_uiStringBuilder.Append("px; ");
		_uiStringBuilder.Append(_previous_VERTICAL_GetSliderVerticalStyleCss_Result);
		
		_uiStringBuilder.Append("top: ");
		_uiStringBuilder.Append(sliderProportionalTopInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        // Proportional Height
        var pageHeight = textEditorDimensions.Height;

        var sliderProportionalHeightInPixels = pageHeight *
            scrollbarHeightInPixels /
            scrollBarDimensions.ScrollHeight;

        var sliderProportionalHeightInPixelsInvariantCulture = sliderProportionalHeightInPixels.ToCssValue();

		_uiStringBuilder.Append("height: ");
		_uiStringBuilder.Append(sliderProportionalHeightInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        _previous_VERTICAL_GetSliderVerticalStyleCss_Result = _uiStringBuilder.ToString();
    }

    private async Task HORIZONTAL_HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
    	var renderBatchLocal = _activeRenderBatch;
    	if (renderBatchLocal is null)
    		return;
    		
    	HORIZONTAL_thinksLeftMouseButtonIsDown = true;
		HORIZONTAL_scrollLeftOnMouseDown = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft;

		var scrollbarBoundingClientRect = await TextEditorService.JsRuntimeCommonApi
			.MeasureElementById(HORIZONTAL_ScrollbarElementId)
			.ConfigureAwait(false);

		// Drag far up to reset scroll to original
		var textEditorDimensions = _activeRenderBatch.ViewModel.TextEditorDimensions;
		var distanceBetweenTopEditorAndTopScrollbar = scrollbarBoundingClientRect.TopInPixels - textEditorDimensions.BoundingClientRectTop;
		HORIZONTAL_clientYThresholdToResetScrollLeftPosition = scrollbarBoundingClientRect.TopInPixels - DISTANCE_TO_RESET_SCROLL_POSITION;

		// Subscribe to the drag events
		//
		// NOTE: '_mouseDownEventArgs' being non-null is what indicates that the subscription is active.
		//       So be wary if one intends to move its assignment elsewhere.
		{
			_mouseDownEventArgs = mouseEventArgs;
			_dragEventHandler = HORIZONTAL_DragEventHandlerScrollAsync;
	
			DragService.ReduceShouldDisplayAndMouseEventArgsSetAction(true, null);
		}
    }
    
    private async Task VERTICAL_HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
    	var renderBatchLocal = _activeRenderBatch;
    	if (renderBatchLocal is null)
    		return;
    
    	VERTICAL_thinksLeftMouseButtonIsDown = true;
		VERTICAL_scrollTopOnMouseDown = renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollTop;

		var scrollbarBoundingClientRect = await TextEditorService.JsRuntimeCommonApi
			.MeasureElementById(VERTICAL_ScrollbarElementId)
			.ConfigureAwait(false);

		// Drag far left to reset scroll to original
		var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
		var distanceBetweenLeftEditorAndLeftScrollbar = scrollbarBoundingClientRect.LeftInPixels - textEditorDimensions.BoundingClientRectLeft;
		VERTICAL_clientXThresholdToResetScrollTopPosition = scrollbarBoundingClientRect.LeftInPixels - DISTANCE_TO_RESET_SCROLL_POSITION;

		// Subscribe to the drag events
		//
		// NOTE: '_mouseDownEventArgs' being non-null is what indicates that the subscription is active.
		//       So be wary if one intends to move its assignment elsewhere.
		{
			_mouseDownEventArgs = mouseEventArgs;
			_dragEventHandler = VERTICAL_DragEventHandlerScrollAsync;
	
			DragService.ReduceShouldDisplayAndMouseEventArgsSetAction(true, null);
		}     
    }

    private async void DragStateWrapOnStateChanged()
    {
        if (!DragService.GetDragState().ShouldDisplay)
        {
            // NOTE: '_mouseDownEventArgs' being non-null is what indicates that the subscription is active.
			//       So be wary if one intends to move its assignment elsewhere.
            _mouseDownEventArgs = null;
        }
        else
        {
            var localMouseDownEventArgs = _mouseDownEventArgs;
            var dragEventArgs = DragService.GetDragState().MouseEventArgs;
			var localDragEventHandler = _dragEventHandler;

            if (localMouseDownEventArgs is not null && dragEventArgs is not null)
                await localDragEventHandler.Invoke(localMouseDownEventArgs, dragEventArgs).ConfigureAwait(false);
        }
    }

    private Task HORIZONTAL_DragEventHandlerScrollAsync(MouseEventArgs localMouseDownEventArgs, MouseEventArgs onDragMouseEventArgs)
    {
    	var renderBatchLocal = _activeRenderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    
    	var localThinksLeftMouseButtonIsDown = HORIZONTAL_thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return Task.CompletedTask;

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown && (onDragMouseEventArgs.Buttons & 1) == 1)
        {
			var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
			var scrollbarDimensions = renderBatchLocal.ViewModel.ScrollbarDimensions;
		
			OnScrollHorizontal onScrollHorizontal;

			if (onDragMouseEventArgs.ClientY < HORIZONTAL_clientYThresholdToResetScrollLeftPosition)
			{
				// Drag far left to reset scroll to original
				onScrollHorizontal = new OnScrollHorizontal(
					HORIZONTAL_scrollLeftOnMouseDown,
					renderBatchLocal.ComponentData,
					renderBatchLocal.ViewModel.ViewModelKey);
			}
			else
			{
				var diffX = onDragMouseEventArgs.ClientX - localMouseDownEventArgs.ClientX;
	
	            var scrollbarWidthInPixels = textEditorDimensions.Width - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
	
	            var scrollLeft = HORIZONTAL_scrollLeftOnMouseDown +
					diffX *
	                scrollbarDimensions.ScrollWidth /
	                scrollbarWidthInPixels;
	
	            if (scrollLeft + textEditorDimensions.Width > scrollbarDimensions.ScrollWidth)
	                scrollLeft = scrollbarDimensions.ScrollWidth - textEditorDimensions.Width;

				if (scrollLeft < 0)
					scrollLeft = 0;
	
				onScrollHorizontal = new OnScrollHorizontal(
					scrollLeft,
					renderBatchLocal.ComponentData,
					renderBatchLocal.ViewModel.ViewModelKey);
			}

			TextEditorService.WorkerUi.EnqueueOnScrollHorizontal(onScrollHorizontal);
        }
        else
        {
            HORIZONTAL_thinksLeftMouseButtonIsDown = false;
        }

        return Task.CompletedTask;
    }
    
    private Task VERTICAL_DragEventHandlerScrollAsync(MouseEventArgs localMouseDownEventArgs, MouseEventArgs onDragMouseEventArgs)
    {
    	var renderBatchLocal = _activeRenderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    
    	var localThinksLeftMouseButtonIsDown = VERTICAL_thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return Task.CompletedTask;

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown && (onDragMouseEventArgs.Buttons & 1) == 1)
        {
			var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
			var scrollbarDimensions = renderBatchLocal.ViewModel.ScrollbarDimensions;

			OnScrollVertical onScrollVertical;

			if (onDragMouseEventArgs.ClientX < VERTICAL_clientXThresholdToResetScrollTopPosition)
			{
				// Drag far left to reset scroll to original
				onScrollVertical = new OnScrollVertical(
					VERTICAL_scrollTopOnMouseDown,
					renderBatchLocal.ComponentData,
					renderBatchLocal.ViewModel.ViewModelKey);
			}
			else
			{
	    		var diffY = onDragMouseEventArgs.ClientY - localMouseDownEventArgs.ClientY;
	
	            var scrollbarHeightInPixels = textEditorDimensions.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
	
	            var scrollTop = VERTICAL_scrollTopOnMouseDown +
					diffY *
	                scrollbarDimensions.ScrollHeight /
	                scrollbarHeightInPixels;
	
	            if (scrollTop + textEditorDimensions.Height > scrollbarDimensions.ScrollHeight)
	                scrollTop = scrollbarDimensions.ScrollHeight - textEditorDimensions.Height;

				if (scrollTop < 0)
					scrollTop = 0;
	
				onScrollVertical = new OnScrollVertical(
					scrollTop,
					renderBatchLocal.ComponentData,
					renderBatchLocal.ViewModel.ViewModelKey);
			}

			TextEditorService.WorkerUi.EnqueueOnScrollVertical(onScrollVertical);
        }
        else
        {
            VERTICAL_thinksLeftMouseButtonIsDown = false;
        }

        return Task.CompletedTask;
    }
    
    public string PresentationGetCssStyleString(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
    	try
        {
            var charMeasurements = _activeRenderBatch.ViewModel.CharAndLineMeasurements;
			var textEditorDimensions = _activeRenderBatch.ViewModel.TextEditorDimensions;
            var scrollbarDimensions = _activeRenderBatch.ViewModel.ScrollbarDimensions;

            if (rowIndex >= _activeRenderBatch.Model.LineEndList.Count)
                return string.Empty;

            var line = _activeRenderBatch.Model.GetLineInformation(rowIndex);

            var startingColumnIndex = 0;
            var endingColumnIndex = line.EndPositionIndexExclusive - 1;

            var fullWidthOfRowIsSelected = true;

            if (lowerPositionIndexInclusive > line.StartPositionIndexInclusive)
            {
                startingColumnIndex = lowerPositionIndexInclusive - line.StartPositionIndexInclusive;
                fullWidthOfRowIsSelected = false;
            }

            if (upperPositionIndexExclusive < line.EndPositionIndexExclusive)
            {
                endingColumnIndex = upperPositionIndexExclusive - line.StartPositionIndexInclusive;
                fullWidthOfRowIsSelected = false;
            }

            var topInPixelsInvariantCulture = (rowIndex * charMeasurements.LineHeight).ToCssValue();
            
            _uiStringBuilder.Clear();
            _uiStringBuilder.Append("position: absolute; ");

			_uiStringBuilder.Append("top: ");
			_uiStringBuilder.Append(topInPixelsInvariantCulture);
			_uiStringBuilder.Append("px;");

            var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();

            _uiStringBuilder.Append("height: ");
            _uiStringBuilder.Append(heightInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            var startInPixels = startingColumnIndex * charMeasurements.CharacterWidth;

            // startInPixels offset from Tab keys a width of many characters
            {
                var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                    rowIndex,
                    startingColumnIndex);

                // 1 of the character width is already accounted for
                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                startInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
            }

            var startInPixelsInvariantCulture = startInPixels.ToCssValue();
            _uiStringBuilder.Append("left: ");
            _uiStringBuilder.Append(startInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            var widthInPixels = endingColumnIndex * charMeasurements.CharacterWidth - startInPixels;

            // Tab keys a width of many characters
            {
                var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                    rowIndex,
                    line.LastValidColumnIndex);

                // 1 of the character width is already accounted for
                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                widthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
            }

            _uiStringBuilder.Append("width: ");

            var fullWidthValue = scrollbarDimensions.ScrollWidth;

            if (textEditorDimensions.Width > scrollbarDimensions.ScrollWidth)
                fullWidthValue = textEditorDimensions.Width; // If content does not fill the viewable width of the Text Editor User Interface

            var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

            var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();

            if (fullWidthOfRowIsSelected)
            {
                _uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
                _uiStringBuilder.Append("px;");
            }
            else if (startingColumnIndex != 0 && upperPositionIndexExclusive > line.EndPositionIndexExclusive - 1)
            {
            	_uiStringBuilder.Append("calc(");
            	_uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
            	_uiStringBuilder.Append("px - ");
            	_uiStringBuilder.Append(startInPixelsInvariantCulture);
            	_uiStringBuilder.Append("px);");
            }
            else
            {
            	_uiStringBuilder.Append(widthInPixelsInvariantCulture);
            	_uiStringBuilder.Append("px;");;
            }

            return _uiStringBuilder.ToString();
        }
        catch (LuthetusTextEditorException)
        {
            return string.Empty;
        }
    }

    public string PresentationGetCssClass(TextEditorPresentationModel presentationModel, byte decorationByte)
    {
        return presentationModel.DecorationMapper.Map(decorationByte);
    }

    public IReadOnlyList<TextEditorTextSpan> PresentationVirtualizeAndShiftTextSpans(
        IReadOnlyList<TextEditorTextModification> textModifications,
        IReadOnlyList<TextEditorTextSpan> inTextSpanList)
    {
    	// (2025-01-22)
    	// ============
    	// The text spans need to be tied to the partitions they reside in
    	// (careful of a textspan that overlaps two partitions).
    	//
    	// This shouldn't be done here, it should be done during the editContext.
    	
    	try
        {
            // Virtualize the text spans
            var virtualizedTextSpanList = new List<TextEditorTextSpan>();
            if (_activeRenderBatch.ViewModel.VirtualizationResult.EntryList.Any())
            {
                var lowerLineIndexInclusive = _activeRenderBatch.ViewModel.VirtualizationResult.EntryList.First().LineIndex;
                var upperLineIndexInclusive = _activeRenderBatch.ViewModel.VirtualizationResult.EntryList.Last().LineIndex;

                var lowerLine = _activeRenderBatch.Model.GetLineInformation(lowerLineIndexInclusive);
                var upperLine = _activeRenderBatch.Model.GetLineInformation(upperLineIndexInclusive);

				// Awkward enumeration was modified 'for loop' (2025-01-22)
				// Also, this shouldn't be done here, it should be done during the editContext.
				var count = inTextSpanList.Count;
                for (int i = 0; i < count; i++)
                {
                	var textSpan = inTextSpanList[i];
                	
                    if (lowerLine.StartPositionIndexInclusive <= textSpan.StartingIndexInclusive &&
                        upperLine.EndPositionIndexExclusive >= textSpan.StartingIndexInclusive)
                    {
                        virtualizedTextSpanList.Add(textSpan);
                    }
                }
            }
            else
            {
                // No 'VirtualizationResult', so don't render any text spans.
                return Array.Empty<TextEditorTextSpan>();
            }

            var outTextSpansList = new List<TextEditorTextSpan>();
            // Shift the text spans
            {
                foreach (var textSpan in virtualizedTextSpanList)
                {
                    var startingIndexInclusive = textSpan.StartingIndexInclusive;
                    var endingIndexExclusive = textSpan.EndingIndexExclusive;

					// Awkward enumeration was modified 'for loop' (2025-01-22)
					// Also, this shouldn't be done here, it should be done during the editContext.
					var count = textModifications.Count;
                    for (int i = 0; i < count; i++)
                    {
                    	var textModification = textModifications[i];
                    
                        if (textModification.WasInsertion)
                        {
                            if (startingIndexInclusive >= textModification.TextEditorTextSpan.StartingIndexInclusive)
                            {
                                startingIndexInclusive += textModification.TextEditorTextSpan.Length;
                                endingIndexExclusive += textModification.TextEditorTextSpan.Length;
                            }
                        }
                        else // was deletion
                        {
                            if (startingIndexInclusive >= textModification.TextEditorTextSpan.StartingIndexInclusive)
                            {
                                startingIndexInclusive -= textModification.TextEditorTextSpan.Length;
                                endingIndexExclusive -= textModification.TextEditorTextSpan.Length;
                            }
                        }
                    }

                    outTextSpansList.Add(textSpan with
                    {
                        StartingIndexInclusive = startingIndexInclusive,
                        EndingIndexExclusive = endingIndexExclusive
                    });
                }
            }

            return outTextSpansList;
        }
        catch (LuthetusTextEditorException)
        {
            return Array.Empty<TextEditorTextSpan>();
        }
    }

    public (int FirstRowToSelectDataInclusive, int LastRowToSelectDataExclusive) PresentationGetBoundsInRowIndexUnits(
    	(int StartingIndexInclusive, int EndingIndexExclusive) boundsInPositionIndexUnits)
    {
    	try
        {
            var firstRowToSelectDataInclusive = _activeRenderBatch.Model
                .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.StartingIndexInclusive)
                .Index;

            var lastRowToSelectDataExclusive = _activeRenderBatch.Model
                .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.EndingIndexExclusive)
                .Index +
                1;

            return (firstRowToSelectDataInclusive, lastRowToSelectDataExclusive);
        }
        catch (LuthetusTextEditorException)
        {
            return (0, 0);
        }
    }
    
    public string GetTextSelectionStyleCss(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
    	try
		{
	        if (rowIndex >= _activeRenderBatch.Model.LineEndList.Count)
	            return string.Empty;
	
	        var line = _activeRenderBatch.Model.GetLineInformation(rowIndex);
	
	        var selectionStartingColumnIndex = 0;
	        var selectionEndingColumnIndex = line.EndPositionIndexExclusive - 1;
	
	        var fullWidthOfRowIsSelected = true;
	
	        if (lowerPositionIndexInclusive > line.StartPositionIndexInclusive)
	        {
	            selectionStartingColumnIndex = lowerPositionIndexInclusive - line.StartPositionIndexInclusive;
	            fullWidthOfRowIsSelected = false;
	        }
	
	        if (upperPositionIndexExclusive < line.EndPositionIndexExclusive)
	        {
	            selectionEndingColumnIndex = upperPositionIndexExclusive - line.StartPositionIndexInclusive;
	            fullWidthOfRowIsSelected = false;
	        }
	
	        var charMeasurements = _activeRenderBatch.ViewModel.CharAndLineMeasurements;
	
	        _uiStringBuilder.Clear();
	        
	        var topInPixelsInvariantCulture = (rowIndex * charMeasurements.LineHeight).ToCssValue();
	        _uiStringBuilder.Append("top: ");
	        _uiStringBuilder.Append(topInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
	
	        _uiStringBuilder.Append(_lineHeightStyleCssString);
	
	        var selectionStartInPixels = selectionStartingColumnIndex * charMeasurements.CharacterWidth;
	
	        // selectionStartInPixels offset from Tab keys a width of many characters
	        {
	            var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
	                rowIndex,
	                selectionStartingColumnIndex);
	
	            // 1 of the character width is already accounted for
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            selectionStartInPixels += 
	                extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
	        }
	
	        var selectionStartInPixelsInvariantCulture = selectionStartInPixels.ToCssValue();
	        _uiStringBuilder.Append("left: ");
	        _uiStringBuilder.Append(selectionStartInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
	
	        var selectionWidthInPixels = 
	            selectionEndingColumnIndex * charMeasurements.CharacterWidth - selectionStartInPixels;
	
	        // Tab keys a width of many characters
	        {
	            var lineInformation = _activeRenderBatch.Model.GetLineInformation(rowIndex);
	
	            selectionEndingColumnIndex = Math.Min(
	                selectionEndingColumnIndex,
	                lineInformation.LastValidColumnIndex);
	
	            var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
	                rowIndex,
	                selectionEndingColumnIndex);
	
	            // 1 of the character width is already accounted for
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            selectionWidthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
	        }
	
	        _uiStringBuilder.Append("width: ");
	        var fullWidthValue = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;
	
	        if (_activeRenderBatch.ViewModel.TextEditorDimensions.Width >
	            _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth)
	        {
	            // If content does not fill the viewable width of the Text Editor User Interface
	            fullWidthValue = _activeRenderBatch.ViewModel.TextEditorDimensions.Width;
	        }
	
	        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();
	
	        var selectionWidthInPixelsInvariantCulture = selectionWidthInPixels.ToCssValue();
	
	        if (fullWidthOfRowIsSelected)
	        {
	        	_uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
	        	_uiStringBuilder.Append("px;");
	        }
	        else if (selectionStartingColumnIndex != 0 &&
	                 upperPositionIndexExclusive > line.EndPositionIndexExclusive - 1)
	        {
	        	_uiStringBuilder.Append("calc(");
	        	_uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
	        	_uiStringBuilder.Append("px - ");
	        	_uiStringBuilder.Append(selectionStartInPixelsInvariantCulture);
	        	_uiStringBuilder.Append("px);");
	        }
	        else
	        {
	        	_uiStringBuilder.Append(selectionWidthInPixelsInvariantCulture);
	        	_uiStringBuilder.Append("px;");
	        }
	
	        return _uiStringBuilder.ToString();
		}
		catch (LuthetusTextEditorException e)
		{
			Console.WriteLine(e);
			return "display: none;";
		}
    }

    public (int lowerRowIndexInclusive, int upperRowIndexExclusive) GetSelectionBoundsInRowIndexUnits(
    	(int lowerPositionIndexInclusive, int upperPositionIndexExclusive) selectionBoundsInPositionIndexUnits)
    {
    	try
        {
            return TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                _activeRenderBatch.Model,
                selectionBoundsInPositionIndexUnits);
        }
        catch (LuthetusTextEditorException)
        {
            return (0, 0);
        }
    }

	/// <summary>
	/// WARNING: Do not use '_uiStringBuilder' in this method. This method can be invoked from outside the UI thread via events.
	/// </summary>
    private void SetWrapperCssAndStyle()
    {
    	var stringBuilder = new StringBuilder();
    	
    	WrapperCssClass = TextEditorService.ThemeCssClassString;
    	
    	stringBuilder.Append("luth_te_text-editor luth_unselectable luth_te_text-editor-css-wrapper ");
    	stringBuilder.Append(WrapperCssClass);
    	stringBuilder.Append(" ");
    	stringBuilder.Append(ViewModelDisplayOptions.TextEditorClassCssString);
    	_personalWrapperCssClass = stringBuilder.ToString();
    	
    	stringBuilder.Clear();
    	
    	var options = TextEditorService.OptionsApi.GetTextEditorOptionsState().Options;
    	
    	var fontSizeInPixels = TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS;
    	if (options.CommonOptions?.FontSizeInPixels is not null)
            fontSizeInPixels = options!.CommonOptions.FontSizeInPixels;
            
        stringBuilder.Append("font-size: ");
        stringBuilder.Append(fontSizeInPixels.ToCssValue());
        stringBuilder.Append("px;");
    	
    	var fontFamily = TextEditorRenderBatch.DEFAULT_FONT_FAMILY;
    	if (!string.IsNullOrWhiteSpace(options?.CommonOptions?.FontFamily))
        	fontFamily = options!.CommonOptions!.FontFamily;
    	
    	stringBuilder.Append("font-family: ");
    	stringBuilder.Append(fontFamily);
    	stringBuilder.Append(";");
    	
    	WrapperCssStyle = stringBuilder.ToString();
    	
    	stringBuilder.Append(WrapperCssStyle);
    	stringBuilder.Append(" ");
    	// string GetGlobalHeightInPixelsStyling()
	    {
	        var heightInPixels = TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.TextEditorHeightInPixels;
	
	        if (heightInPixels is not null)
	        {
	        	var heightInPixelsInvariantCulture = heightInPixels.Value.ToCssValue();
	        
		        stringBuilder.Append("height: ");
		        stringBuilder.Append(heightInPixelsInvariantCulture);
		        stringBuilder.Append("px;");
	        }
	    }
    	stringBuilder.Append(" ");
    	stringBuilder.Append(ViewModelDisplayOptions.TextEditorStyleCssString);
    	stringBuilder.Append(" ");
    	// string GetHeightCssStyle()
	    {
	    	if (_previousIncludeHeader != ViewModelDisplayOptions.HeaderComponentType is not null ||
	    	    _previousIncludeFooter != ViewModelDisplayOptions.FooterComponentType is not null)
	    	{
	    		// Start with a calc statement and a value of 100%
		        stringBuilder.Append("height: calc(100%");
		
		        if (ViewModelDisplayOptions.HeaderComponentType is not null)
		            stringBuilder.Append(" - var(--luth_te_text-editor-header-height)");
		
		        if (ViewModelDisplayOptions.FooterComponentType is not null)
		            stringBuilder.Append(" - var(--luth_te_text-editor-footer-height)");
		
		        // Close the calc statement, and the height style attribute
		        stringBuilder.Append(");");
		        
		        _previousGetHeightCssStyleResult = stringBuilder.ToString();
	    	}
	    }
    	_personalWrapperCssStyle = stringBuilder.ToString();
    }
    
    private void ConstructVirtualizationStyleCssStrings()
    {
    	if (_activeRenderBatch is null)
    		return;
    	
    	if (_activeRenderBatch.ViewModel.VirtualizationResult.TotalWidth != _previousTotalWidth)
    	{
    		
    		_previousTotalWidth = _activeRenderBatch.ViewModel.VirtualizationResult.TotalWidth;
    		
    		_uiStringBuilder.Clear();
	    	_uiStringBuilder.Append("width: ");
	    	_uiStringBuilder.Append(_activeRenderBatch.ViewModel.VirtualizationResult.TotalWidth);
	    	_uiStringBuilder.Append("px;");
	        _horizontalVirtualizationBoundaryStyleCssString = _uiStringBuilder.ToString();
    	}
	    	
    	if (_activeRenderBatch.ViewModel.VirtualizationResult.TotalHeight != _previousTotalHeight)
    	{
    		
    		_previousTotalHeight = _activeRenderBatch.ViewModel.VirtualizationResult.TotalHeight;
    	
    		_uiStringBuilder.Clear();
	    	_uiStringBuilder.Append("height: ");
	    	_uiStringBuilder.Append(_activeRenderBatch.ViewModel.VirtualizationResult.TotalHeight);
	    	_uiStringBuilder.Append("px;");
	    	_verticalVirtualizationBoundaryStyleCssString = _uiStringBuilder.ToString();
    	}
    }
    
    private async void OnOptionMeasuredStateChanged()
    {
    	SetWrapperCssAndStyle();
    	QueueCalculateVirtualizationResultBackgroundTask(_currentRenderBatch);
    }
    
    private async void OnOptionStaticStateChanged()
    {
    	SetWrapperCssAndStyle();
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	// ScrollbarSection.razor.cs
    	DragService.DragStateChanged -= DragStateWrapOnStateChanged;
    
    	// TextEditorViewModelDisplay.razor.cs
        TextEditorService.TextEditorStateChanged -= GeneralOnStateChangedEventHandler;
        TextEditorService.OptionsApi.StaticStateChanged -= OnOptionStaticStateChanged;
        TextEditorService.OptionsApi.MeasuredStateChanged -= OnOptionMeasuredStateChanged;
		TextEditorService.ViewModelApi.CursorShouldBlinkChanged -= ViewModel_CursorShouldBlinkChanged;

        lock (_linkedViewModelLock)
        {
            if (_linkedViewModel is not null)
            {
                _linkedViewModel.DisplayTracker.DecrementLinks();
                _linkedViewModel = null;
            }
        }

        _onMouseMoveCancellationTokenSource.Cancel();
        _onMouseMoveCancellationTokenSource.Dispose();
    }
}
