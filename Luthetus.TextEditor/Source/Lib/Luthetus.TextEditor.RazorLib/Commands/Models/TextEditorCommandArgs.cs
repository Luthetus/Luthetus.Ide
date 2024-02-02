using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models;

public class TextEditorCommandArgs : ICommandArgs
{
    public TextEditorCommandArgs(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        bool hasTextSelection,
        IClipboardService clipboardService,
        ITextEditorService textEditorService,
        Func<MouseEventArgs, Task>? handleMouseStoppedMovingEventAsyncFunc,
        IJSRuntime? jsRuntime,
        IDispatcher dispatcher,
        IServiceProvider serviceProvider,
        LuthetusTextEditorConfig textEditorConfig)
    {
        ModelResourceUri = modelResourceUri;
        ViewModelKey = viewModelKey;
        HasTextSelection = hasTextSelection;
        ClipboardService = clipboardService;
        TextEditorService = textEditorService;
        HandleMouseStoppedMovingEventAsyncFunc = handleMouseStoppedMovingEventAsyncFunc;
        JsRuntime = jsRuntime;
        Dispatcher = dispatcher;
        ServiceProvider = serviceProvider;
        TextEditorConfig = textEditorConfig;
    }

    public ResourceUri ModelResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public IClipboardService ClipboardService { get; }
    public ITextEditorService TextEditorService { get; }
    /// <summary>
    /// This property is used so a keyboard event can trigger a tooltip at the cursor's position.
    /// </summary>
    public Func<MouseEventArgs, Task>? HandleMouseStoppedMovingEventAsyncFunc { get; }
    public IJSRuntime? JsRuntime { get; }
    public IDispatcher Dispatcher { get; }
    public IServiceProvider ServiceProvider { get; }
    public LuthetusTextEditorConfig TextEditorConfig { get; }
    public bool HasTextSelection { get; set; }

    /// <summary>
    /// Hack for <see cref="Defaults.TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(bool)"/>
    /// to be able to select text. (2023-12-15)
    /// </summary>
    public bool ShouldSelectText { get; set; }

    /// <summary>
    /// Hack for <see cref="Vims.TextEditorCommandVimFacts.Motions.GetVisual(TextEditorCommand, string)"/>
    /// to be able to select text. (2023-12-15)
    /// </summary>
    public TextEditorCommand InnerCommand { get; set; }
    /// <summary>
    /// Hack for <see cref="Vims.TextEditorCommandVimFacts.Motions.GetVisual(TextEditorCommand, string)"/>
    /// to be able to select text. (2023-12-15)
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// true if the shift key was down when the event was fired. false otherwise.
    /// </summary>
    public bool ShiftKey { get; set; }
}
