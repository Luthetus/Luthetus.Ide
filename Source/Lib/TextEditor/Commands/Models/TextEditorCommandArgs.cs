using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Commands.Models;

public class TextEditorCommandArgs : ICommandArgs
{
    public TextEditorCommandArgs(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
		TextEditorComponentData componentData,
		ITextEditorService textEditorService,
        IServiceProvider serviceProvider,
        TextEditorEditContext editContext)
    {
        ModelResourceUri = modelResourceUri;
        ViewModelKey = viewModelKey;
		ComponentData = componentData;
		TextEditorService = textEditorService;
        ServiceProvider = serviceProvider;
        EditContext = editContext;
    }

    public ResourceUri ModelResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }
    public ITextEditorService TextEditorService { get; }
    public IServiceProvider ServiceProvider { get; }
    public TextEditorEditContext EditContext { get; set; }

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
