using System.Diagnostics;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// Everytime one renders a unique <see cref="Luthetus.TextEditor.RazorLib.TextEditors.Displays.TextEditorViewModelSlimDisplay"/>,
/// a unique identifier for the HTML elements is created.
///
/// That unique identifier, and other data, is on this class.
///
/// This class is a "ui event broker" in order to avoid unnecessary invocations of
/// <see cref="ITextEditorService.Post"/.
///
/// For example, all <see cref="TextEditorViewModel"/> have a <see cref="TextEditorViewModel.TooltipViewModel"/>.
/// A sort of 'on mouse over' like event might set the current <see cref="TextEditorViewModel.TooltipViewModel"/>.
/// But, the event logic overhead is not handled by the <see cref="TextEditorViewModel"/>, but instead
/// this class, then the valid events are passed along to result in the <see cref="TextEditorViewModel.TooltipViewModel"/>
/// being set.
/// </summary>
public sealed class TextEditorComponentData
{
	private readonly Throttle _throttleApplySyntaxHighlighting = new(TimeSpan.FromMilliseconds(500));

	public static TimeSpan ThrottleDelayDefault { get; } = TimeSpan.FromMilliseconds(60);
    public static TimeSpan OnMouseOutTooltipDelay { get; } = TimeSpan.FromMilliseconds(1_000);
    public static TimeSpan MouseStoppedMovingDelay { get; } = TimeSpan.FromMilliseconds(400);

	public TextEditorComponentData(
		Guid textEditorHtmlElementId,
		ViewModelDisplayOptions viewModelDisplayOptions,
		TextEditorOptions options,
		TextEditorViewModelSlimDisplay textEditorViewModelSlimDisplay,
		IDropdownService dropdownService,
		IClipboardService clipboardService,
		ICommonComponentRenderers commonComponentRenderers,
		INotificationService notificationService,
		ITextEditorService textEditorService,
		ILuthetusTextEditorComponentRenderers textEditorComponentRenderers,
		IFindAllService findAllService,
		IEnvironmentProvider environmentProvider,
		IFileSystemProvider fileSystemProvider,
		IServiceProvider serviceProvider)
	{
		TextEditorHtmlElementId = textEditorHtmlElementId;
		ViewModelDisplayOptions = viewModelDisplayOptions;
		Options = options;
		TextEditorViewModelSlimDisplay = textEditorViewModelSlimDisplay;
		DropdownService = dropdownService;
		ClipboardService = clipboardService;
		CommonComponentRenderers = commonComponentRenderers;
		NotificationService = notificationService;
		TextEditorService = textEditorService;
		TextEditorComponentRenderers = textEditorComponentRenderers;
		FindAllService = findAllService;
		EnvironmentProvider = environmentProvider;
		FileSystemProvider = fileSystemProvider;
		ServiceProvider = serviceProvider;
	}

	/*public TextEditorComponentData(
		TextEditorComponentData otherComponentData,
		Keymap keymap)
	{
		TextEditorHtmlElementId = otherComponentData.TextEditorHtmlElementId;
		ViewModelDisplayOptions = otherComponentData.ViewModelDisplayOptions;

		Options = otherComponentData.Options with
		{
			Keymap = keymap
		};

		TextEditorViewModelSlimDisplay = otherComponentData.TextEditorViewModelSlimDisplay;
		ServiceProvider = otherComponentData.ServiceProvider;
	}*/

	/// <summary>
	/// This property contains the global options, with an extra step of overriding any specified options
	/// for a specific text editor. The current implementation of this is quite hacky and not obvious
	/// when reading the code.
	/// </summary>
	public TextEditorOptions Options { get; init; }

	public Guid TextEditorHtmlElementId { get; }
	public ViewModelDisplayOptions ViewModelDisplayOptions { get; }
	public TextEditorViewModelSlimDisplay TextEditorViewModelSlimDisplay { get; }
	public IDropdownService DropdownService { get; }
	public IClipboardService ClipboardService { get; }
	public ICommonComponentRenderers CommonComponentRenderers { get; }
	public INotificationService NotificationService { get; }
	public ITextEditorService TextEditorService { get; }
	public ILuthetusTextEditorComponentRenderers TextEditorComponentRenderers { get; }
	public IFindAllService FindAllService { get; }
	public IEnvironmentProvider EnvironmentProvider { get; }
	public IFileSystemProvider FileSystemProvider { get; }
	public IServiceProvider ServiceProvider { get; }
	public Task MouseStoppedMovingTask { get; set; } = Task.CompletedTask;
    public Task MouseNoLongerOverTooltipTask { get; set; } = Task.CompletedTask;
    public CancellationTokenSource MouseNoLongerOverTooltipCancellationTokenSource { get; set; } = new();
    public CancellationTokenSource MouseStoppedMovingCancellationTokenSource { get; set; } = new();
    
    public long MouseMovedTimestamp { get; private set; }

    /// <summary>
	/// This accounts for one who might hold down Left Mouse Button from outside the TextEditorDisplay's content div
	/// then move their mouse over the content div while holding the Left Mouse Button down.
	/// </summary>
    public bool ThinksLeftMouseButtonIsDown { get; set; }
    
    public bool MenuShouldTakeFocus { get; set; }

	public void ThrottleApplySyntaxHighlighting(TextEditorModel modelModifier)
    {
        _throttleApplySyntaxHighlighting.Run(_ =>
        {
            modelModifier.CompilerService.ResourceWasModified(modelModifier.ResourceUri, Array.Empty<TextEditorTextSpan>());
			return Task.CompletedTask;
        });
    }

	public Task ContinueRenderingTooltipAsync()
    {
        MouseNoLongerOverTooltipCancellationTokenSource.Cancel();
        MouseNoLongerOverTooltipCancellationTokenSource = new();

        return Task.CompletedTask;
    }
    
    public void OnMouseMoved()
    {
    	MouseMovedTimestamp = Stopwatch.GetTimestamp();
    }
}
