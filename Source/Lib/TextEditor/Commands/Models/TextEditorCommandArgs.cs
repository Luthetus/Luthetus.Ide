using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Commands.Models;

public class TextEditorCommandArgs : ICommandArgs
{
    public TextEditorCommandArgs(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        bool hasTextSelection,
		ITextEditorService textEditorService,
        TextEditorOptions options,
		TextEditorComponentData componentData,
        TextEditorViewModelDisplay.TextEditorEvents events,
        IServiceProvider serviceProvider,
        LuthetusTextEditorConfig textEditorConfig)
    {
        ModelResourceUri = modelResourceUri;
        ViewModelKey = viewModelKey;
        HasTextSelection = hasTextSelection;
        TextEditorService = textEditorService;
		Options = options;
		ComponentData = componentData;
        Events = events;
        ServiceProvider = serviceProvider;
        TextEditorConfig = textEditorConfig;
    }

    public ResourceUri ModelResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public ITextEditorService TextEditorService { get; }

    /// <summary>
    /// This property is a snapshot of the text editor's options at time
    /// of the event.
    /// </summary>
	public TextEditorOptions Options { get; }

    public TextEditorComponentData ComponentData { get; }
    public TextEditorViewModelDisplay.TextEditorEvents Events { get; }
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
